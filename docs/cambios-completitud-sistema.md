# Cambios aplicados con antes y despues

Estos ejemplos son extractos resumidos del codigo para entender que se cambio y por que. No copian todos los metodos completos, solo la parte importante.

## 1. Permisos en backend

**Antes**

```csharp
[Authorize]
[HttpPost]
public async Task<IActionResult> CreateVenta([FromBody] CreateVentaDto dto)
{
    var result = await _ventasService.CreateVenta(dto);
    return Created($"api/ventas/{result.Value!.IdVenta}", result.Value);
}
```

**Despues**

```csharp
[Authorize]
[Permiso("NuevaVenta", "Crear")]
[HttpPost]
public async Task<IActionResult> CreateVenta([FromBody] CreateVentaDto dto)
{
    var result = await _ventasService.CreateVenta(dto);
    return Created($"api/ventas/{result.Value!.IdVenta}", result.Value);
}
```

**Que se aplico y por que**

Se agrego `PermisoAttribute` y `PermisoFilter`. Antes bastaba con estar logueado; ahora el backend revisa si el rol tiene permiso sobre el componente real del frontend. Esto protege la API aunque alguien intente llamar el endpoint manualmente.

## 2. Validacion real del permiso

**Antes**

```csharp
// No habia filtro propio de permisos.
// El frontend decidia si mostrar u ocultar botones.
```

**Despues**

```csharp
var tienePermiso = await (
    from rp in _context.RolesPermisos.AsNoTracking()
    join c in _context.ComponentsForm on rp.ComponentsId equals c.ComponentsId
    join p in _context.Permisos on rp.PermisoId equals p.PermisoId
    where rp.RoleId == roleId
       && c.ComponentsName == _componentName
       && p.PermisoNombre == _permisoNombre
    select rp
).AnyAsync();

if (!tienePermiso)
    context.Result = new ForbidResult();
```

**Que se aplico y por que**

El filtro consulta `RolesPermisos`, `ComponentsForm` y `Permisos`. Si no existe la asignacion, devuelve `403 Forbidden`. Esto hace que permisos sea una regla de backend, no solo una regla visual.

## 3. Stock al crear movimientos

**Antes**

```csharp
var created = await _stockRepository.CreateMovimiento(entity);
return ServiceResult<StockMovimientoResponseDto>.Ok(ToResponse(created));
```

**Despues**

```csharp
return await _stockRepository.ExecuteInTransaction(async () =>
{
    var articulo = await _stockRepository.GetArticulo(movimiento.IdArticulo);
    var stockAnterior = articulo.StockActual;

    if (tipo == "Salida" && stockAnterior < movimiento.Cantidad)
        return ServiceResult<StockMovimientoResponseDto>.Fail("Stock insuficiente");

    articulo.StockActual = tipo == "Entrada"
        ? stockAnterior + movimiento.Cantidad
        : stockAnterior - movimiento.Cantidad;

    entity.StockAnterior = stockAnterior;
    entity.StockNuevo = articulo.StockActual;

    var created = await _stockRepository.CreateMovimiento(entity);
    return ServiceResult<StockMovimientoResponseDto>.Ok(ToResponse(created));
});
```

**Que se aplico y por que**

Ahora el movimiento y la actualizacion de `StockActual` ocurren juntos en una transaccion. Esto evita que se guarde un movimiento sin afectar el stock, o que una salida deje stock negativo.

## 4. Eliminar venta ahora anula

**Antes**

```csharp
await _repository.RemoveStockMovimientos(movimientos);
await _repository.RemoveVenta(venta);
await _repository.SaveChanges();
```

**Despues**

```csharp
foreach (var detalle in venta.VentaDetalles)
{
    detalle.Articulo.StockActual += detalle.Cantidad;

    await _repository.AddStockMovimiento(new StockMovimiento
    {
        IdArticulo = detalle.IdArticulo,
        TipoMovimiento = "Entrada",
        Cantidad = detalle.Cantidad,
        Referencia = $"AnulacionVenta:{venta.IdVenta}"
    });
}

venta.Estado = "ANULADA";
venta.FechaAnulacion = DateTime.UtcNow;
venta.MotivoAnulacion = "Anulacion desde API";

await _repository.SaveChanges();
```

**Que se aplico y por que**

Antes se perdia historial. Ahora una venta eliminada queda anulada y el stock vuelve con un movimiento compensatorio. Esto permite auditar ventas, anulaciones y stock.

## 5. Clientes: respuestas mas claras

**Antes**

```csharp
Customer customerEntity = await _customerServices.GetCustomer(id);
return Ok(customerEntity.ToDto());
```

**Despues**

```csharp
Customer? customerEntity = await _customerServices.GetCustomer(id);

if (customerEntity == null)
    return NotFound();

return Ok(customerEntity.ToDto());
```

**Que se aplico y por que**

Antes un cliente inexistente podia terminar en error interno o respuesta confusa. Ahora la API devuelve `404 NotFound`, que el frontend puede manejar mejor.

## 6. Usuarios: correo duplicado y clave

**Antes**

```csharp
return await _context.CreateUsuarios(usuarioCreate);
```

**Despues**

```csharp
if (string.IsNullOrWhiteSpace(usuarioCreate.clave))
    return false;

if (await _context.UsuarioCorreoExists(usuarioCreate.correo))
    return false;

return await _context.CreateUsuarios(usuarioCreate);
```

**Que se aplico y por que**

Se evita crear usuarios sin clave inicial y se bloquean correos duplicados. Ademas se agrego un indice unico en SQL para que la base tambien proteja ese dato.

## 7. JWT y CORS

**Antes**

```csharp
ValidateIssuer = false,
ValidateAudience = false
```

**Despues**

```csharp
ValidateIssuer = true,
ValidIssuer = issuer,
ValidateAudience = true,
ValidAudience = audience
```

```csharp
policy.WithOrigins(origins)
    .AllowAnyMethod()
    .AllowAnyHeader();
```

**Que se aplico y por que**

El token ahora debe haber sido emitido para esta API y para esta audiencia. CORS tambien queda configurado por origenes conocidos, en lugar de quedar abierto para cualquier frontend.

## 8. Endpoint viejo de articulos en frontend

**Antes**

```javascript
const response = await api.get("/api/Service/Articulos");
```

**Despues**

```javascript
const response = await api.get("/api/articulos");
```

**Que se aplico y por que**

El frontend ya no debe depender del controlador viejo de servicios/lavadero. Ahora productos, stock y nueva venta consumen el endpoint actual de articulos.

## 9. SQL Server

**Antes**

```sql
-- Ventas no guardaba datos de anulacion.
-- Usuarios permitia correos duplicados.
-- StockMovimientos podia tener cantidades invalidas.
```

**Despues**

```sql
ALTER TABLE dbo.Ventas ADD
    FechaAnulacion datetime NULL,
    MotivoAnulacion varchar(255) NULL,
    IdUsuarioAnulacion int NULL;

CREATE UNIQUE INDEX UX_Usuarios_Correo
ON dbo.Usuarios(correo)
WHERE correo IS NOT NULL;

ALTER TABLE dbo.StockMovimientos
ADD CONSTRAINT CK_StockMovimientos_Cantidad_Positiva
CHECK (Cantidad > 0);
```

**Que se aplico y por que**

La base ahora conserva informacion de anulaciones y bloquea datos que no deberian existir. Esto ayuda aunque en algun momento entre un dato incorrecto desde la API.

## Verificacion ejecutada

- `dotnet test`: 21 pruebas correctas.
- `dotnet build`: backend compila correctamente.
- API iniciada en `http://127.0.0.1:5099`: Swagger respondio HTTP 200.
- `npm run lint`: correcto.
- `npm run build`: correcto.
- SQL Server: columnas, constraints e indices presentes; sin correos duplicados ni cantidades invalidas de stock.

## Pendiente

No se implemento facturacion fiscal/comprobantes legales. Esa parte requiere definir reglas de negocio antes de codificar: tipo de comprobante, numeracion, impuestos, anulaciones fiscales y reportes.
