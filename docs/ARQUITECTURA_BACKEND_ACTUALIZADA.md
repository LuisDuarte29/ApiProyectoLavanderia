# Arquitectura Backend Actualizada

Fecha de actualizacion: 2026-05-18

## 1. Resumen general

El backend principal del repositorio es `PracticaJWTcore`, una API ASP.NET Core sobre .NET 8 que usa SQL Server mediante Entity Framework Core y algunos procedimientos almacenados llamados con ADO.NET.

Despues de la mejora arquitectonica aplicada, el backend quedo mas alineado con una arquitectura por capas dentro del mismo proyecto:

```text
Controller -> Service -> Repository -> PracticaJWTcoreContext / SQL Server
```

El objetivo no fue convertir el sistema a Clean Architecture ni separar proyectos. El objetivo fue ordenar responsabilidades sin romper frontend, base de datos, rutas publicas ni contratos actuales.

Los modulos mas importantes que ahora siguen mejor la separacion por capas son:

- Ventas.
- Stock.
- Articulos.
- Usuarios.
- Autenticacion.

Tambien se corrigio una deuda puntual detectada en controllers historicos: los `CreatedResult` que usaban `localhost` hardcodeado ahora devuelven rutas relativas.

## 2. Nota sobre el PDF de frontend

El archivo de referencia `C:\Users\user\Downloads\ARQUITECTURA_FRONTEND.pdf` describe un frontend React 18 + Vite con carpetas como `src/app`, `src/components`, `src/Hooks`, `src/services` y un cliente Axios centralizado.

En el workspace actual inspeccionado no aparece ese frontend React. Los frontends visibles son proyectos Blazor/.NET:

- `BlazorApp1`
- `BlazorFrontend`
- `ApiProyectoFrontend`

Por eso, las recomendaciones del PDF frontend sobre React, Vite, Axios, hooks React y componentes JSX no se aplicaron directamente. Se usaron como referencia conceptual de contratos y separacion de responsabilidades, pero el documento actualizado se centra en el backend real existente.

## 3. Estado actual de carpetas backend

Estructura relevante actual:

```text
PracticaJWTcore/
  Controllers/
    ArticulosController.cs
    AutenticacionController.cs
    StockController.cs
    UsuariosController.cs
    VentasController.cs
    ...
  Services/
    IArticulosService.cs
    ArticulosService.cs
    IStockService.cs
    StockService.cs
    IVentasService.cs
    VentasService.cs
    ServiceResult.cs
    ...
  Repositorios/
    IArticulosRepository.cs
    ArticulosRepository.cs
    IStockRepository.cs
    StockRepository.cs
    IVentasRepository.cs
    VentasRepository.cs
    ...
  Dtos/
    Articulos/
      ArticuloRequestDto.cs
      ArticuloResponseDto.cs
    Stock/
      StockMovimientoRequestDto.cs
      StockMovimientoResponseDto.cs
    Ventas/
      CreateVentaDto.cs
      UpdateVentaDto.cs
      VentaDetalleResponseDto.cs
      VentaItemDto.cs
      VentaResponseDto.cs
    ...
  Models/
    PracticaJWTcoreContext.cs
    Articulos.cs
    Venta.cs
    VentaDetalle.cs
    StockMovimiento.cs
    Categoria.cs
    ...
  Program.cs
```

## 4. Responsabilidades por capa

### Controllers

Los controllers prioritarios quedaron orientados a HTTP:

- Reciben requests.
- Llaman al service correspondiente.
- Traducen resultados a `Ok`, `Created`, `BadRequest`, `NotFound` o `NoContent`.
- Ya no contienen reglas complejas de ventas, stock o articulos.
- Ya no inyectan directamente `PracticaJWTcoreContext` en los modulos prioritarios.

Controllers especialmente mejorados:

- `VentasController`
- `StockController`
- `ArticulosController`
- `UsuariosController`
- `AutenticacionController`

### Services

Los services concentran reglas de negocio, validaciones y coordinacion:

- `VentasService`
- `StockService`
- `ArticulosService`
- `UsuarioServices`
- `AutenticacionServices`

Se agrego `ServiceResult<T>` como resultado simple de aplicacion para devolver:

- exito o fallo;
- valor de respuesta;
- mensaje controlado;
- codigo interno de error.

### Repositories

Los repositories concentran acceso a datos:

- Queries EF Core.
- `Add`, `Remove`, `SaveChanges`.
- Proyecciones a DTOs.
- Transacciones en el caso de ventas.

Repositorios nuevos:

- `VentasRepository`
- `StockRepository`
- `ArticulosRepository`

Repositorios existentes preservados:

- `CustomerRepository`
- `ServicesRepository`
- `AppointmentRepository`
- `PedidosRepository`
- `UsuarioRepository`
- `AutenticacionRespository`

### DTOs

Se agregaron DTOs explicitos para contratos importantes:

- Ventas:
  - `CreateVentaDto`
  - `UpdateVentaDto`
  - `VentaItemDto`
  - `VentaResponseDto`
  - `VentaDetalleResponseDto`
- Stock:
  - `StockMovimientoRequestDto`
  - `StockMovimientoResponseDto`
- Articulos:
  - `ArticuloRequestDto`
  - `ArticuloResponseDto`

Esto reduce la exposicion directa de entidades EF en endpoints criticos.

## 5. Flujo actualizado de ventas

### Ruta principal

```text
POST /api/Ventas
```

### Controller

`VentasController` recibe `CreateVentaDto`, llama a `IVentasService.CreateVenta` y devuelve:

- `201 Created` si la venta se crea.
- `400 BadRequest` si hay validaciones de negocio fallidas.

### Service

`VentasService` contiene ahora las reglas principales:

- Validar que existan items.
- Validar cantidad positiva.
- Validar existencia de articulos.
- Validar stock suficiente.
- Crear cabecera de venta.
- Crear detalles.
- Calcular subtotal, IVA y total.
- Descontar stock.
- Registrar movimiento de stock.
- Coordinar eliminacion de venta y restauracion de stock.

### Repository

`VentasRepository` contiene:

- Consultas a `Articulos`.
- Inserciones en `Ventas`.
- Inserciones en `VentaDetalles`.
- Inserciones en `StockMovimientos`.
- Proyecciones de ventas y detalles.
- Transaccion mediante `Database.BeginTransactionAsync`.

### Tablas involucradas

- `Ventas`
- `VentaDetalles`
- `Articulos`
- `StockMovimientos`
- `Customer`
- `Usuarios`

### Contrato conservado

El request principal conserva las propiedades:

- `idCliente`
- `idUsuario`
- `metodoPago`
- `estado`
- `items`
- `articuloId`
- `cantidad`

La respuesta principal conserva:

- `idVenta`
- `fechaVenta`
- `idCliente`
- `total`
- `detalles`

## 6. Flujo actualizado de stock

### Rutas

```text
GET    /api/stock/movimientos
GET    /api/stock/movimientos/{id}
POST   /api/stock/movimientos
PUT    /api/stock/movimientos/{id}
DELETE /api/stock/movimientos/{id}
```

### Controller

`StockController` ahora solo traduce HTTP hacia `IStockService`.

### Service

`StockService` valida:

- Movimiento requerido.
- Cantidad mayor a cero.
- Articulo existente.
- Coincidencia de ID en updates.

### Repository

`StockRepository` contiene:

- Listado de movimientos.
- Busqueda de movimiento.
- Busqueda de articulo.
- Alta, modificacion y baja de movimiento.

### Decision importante de compatibilidad

Se preservo el comportamiento manual existente: crear o editar movimientos desde `StockController` no modifica automaticamente `Articulos.StockActual`.

Motivo:

- El comportamiento anterior registraba movimientos manuales sin ajustar stock actual.
- Cambiarlo de golpe podria duplicar ajustes o romper flujos existentes.
- La actualizacion de stock por ventas sigue ocurriendo dentro de `VentasService`.

Esta decision queda documentada como deuda funcional a revisar con reglas de negocio mas precisas.

## 7. Flujo actualizado de articulos

### Rutas

```text
GET    /api/Articulos
GET    /api/Articulos/{id}
POST   /api/Articulos
PUT    /api/Articulos/{id}
DELETE /api/Articulos/{id}
```

### Controller

`ArticulosController` ahora llama a `IArticulosService`.

### Service

`ArticulosService` valida:

- Articulo requerido.
- `NombreArticulo` requerido.
- Categoria existente cuando se informa `IdCategoria`.
- Precios no negativos.
- Stock no negativo.
- No eliminar articulo con ventas o movimientos asociados.

### Repository

`ArticulosRepository` contiene:

- Listado de articulos con nombre de categoria.
- Busqueda por ID.
- Busqueda de entidad para update/delete.
- Validacion de categoria.
- Validacion de dependencias en `VentaDetalles` y `StockMovimientos`.
- Persistencia EF Core.

## 8. Usuarios y autenticacion

### Usuarios

`UsuariosController` ahora depende de `IUsuarioServices` en vez de llamar directo a `IUsuariosRepository`.

Se mantiene:

- `POST /api/Usuarios/CreateUsuario`
- `GET /api/Usuarios/GetRoles`
- `GET /api/Usuarios/GetUsuarios`
- `GET /api/Usuarios/GetRoleList`
- endpoints de permisos y componentes.

Tambien se dejo de exponer `ex.Message` directamente al cliente en creacion de usuario. El detalle se escribe en consola y el cliente recibe un mensaje generico:

```text
Error interno al crear el usuario
```

### Autenticacion

`AutenticacionController` ahora depende de `IAutenticacionServices`.

Se mantiene:

- `POST /api/Autenticacion`
- `POST /api/Autenticacion/CambioClave`

El shape principal de login se conserva:

```json
{
  "tokenRol": {
    "token": "...",
    "rolId": 1
  }
}
```

No se modificaron stored procedures ni estrategia de password.

## 9. Program.cs actualizado

`Program.cs` mantiene:

- CORS actual `PermitirTodo`.
- JWT Bearer.
- Swagger.
- `DbContext`.
- JSON camelCase.
- `UseAuthentication`.
- `UseAuthorization`.

Mejoras realizadas:

- Se elimino `AddControllers()` duplicado.
- Se registraron nuevos repositories:
  - `IVentasRepository -> VentasRepository`
  - `IStockRepository -> StockRepository`
  - `IArticulosRepository -> ArticulosRepository`
- Se registraron nuevos services:
  - `IVentasService -> VentasService`
  - `IStockService -> StockService`
  - `IArticulosService -> ArticulosService`

No se modifico:

- `DefaultConnection`.
- JWT key.
- CORS.
- Swagger security.

## 10. Mejoras adicionales aplicadas despues del refactor

Se corrigieron `CreatedResult` con `localhost` hardcodeado en:

- `AppointmentController`
- `CustomerController`
- `ServiceController`

Antes devolvian locations como:

```text
http://localhost:7184/api/Service/{id}
https://localhost:7184/api/Appointment/{id}
```

Ahora devuelven rutas relativas:

```text
api/Service/{id}
api/Appointment/{id}
api/customer/{id}
```

Esto evita acoplar el backend a un host fijo de desarrollo.

## 11. Contrato con frontend Blazor actual

El frontend visible principal es `BlazorApp1`.

`BlazorApp1/Program.cs` configura:

```text
https://localhost:7184/
```

Consumos detectados:

- `POST api/Autenticacion`
- `GET api/service`
- `GET api/service/{idServicioLong}`
- `POST api/service`
- `PUT api/service/`
- `DELETE api/service/{id}`
- `GET api/pedidos`
- `GET api/test`

No se detecto consumo directo actual desde Blazor de:

- `api/Ventas`
- `api/stock/movimientos`
- `api/Articulos`

Aun asi, esos contratos se mantuvieron para compatibilidad con Swagger, otros clientes o pantallas futuras.

## 12. Seguridad y autorizacion

JWT esta configurado y el frontend Blazor envia Bearer token en algunas pantallas de servicios.

No se aplico `[Authorize]` masivamente porque podria bloquear pantallas actuales si algun flujo todavia no envia token correctamente.

Pendiente recomendado:

- Definir lista de endpoints publicos.
- Aplicar `[Authorize]` gradualmente.
- Mantener `[AllowAnonymous]` solo en login/test u otros endpoints publicos reales.
- Revisar roles/permisos como barrera backend, no solo como UI.

## 13. Errores y validaciones

Mejoras realizadas:

- Validaciones de ventas pasaron a `VentasService`.
- Validaciones de stock pasaron a `StockService`.
- Validaciones de articulos pasaron a `ArticulosService`.
- `UsuariosController` dejo de exponer detalles internos de excepcion.

Pendiente:

- Middleware global de errores.
- Formato uniforme `{ message, code }` para todos los errores esperados.
- Logs con `ILogger` en vez de `Console.WriteLine`.

No se implemento middleware global en esta etapa para no cambiar demasiado el contrato de errores del frontend.

## 14. Estado de pruebas

Se agregaron pruebas en `PracticaJWTCoreTest` para cubrir comportamientos clave:

- `VentasServiceTests`
- `StockServiceTests`
- `ArticulosServiceTests`
- `ControllerServiceWiringTests`
- `CreatedLocationTests`

Cobertura funcional agregada:

- Venta rechaza items vacios.
- Venta rechaza stock insuficiente sin abrir transaccion.
- Stock rechaza cantidad no positiva.
- Stock manual preserva comportamiento de no ajustar `StockActual`.
- Articulos rechaza nombre vacio.
- Articulos bloquea delete con dependencias.
- Login conserva wrapper `tokenRol`.
- Usuarios no expone mensaje interno en error 500.
- CreatedResult historicos no devuelven localhost hardcodeado.

## 15. Verificacion ejecutada

### Build

Comando:

```powershell
dotnet build .\PracticaJWTcore.sln --no-restore
```

Resultado observado:

```text
Compilacion correcta.
0 Advertencia(s)
0 Errores
```

### Tests

Comando:

```powershell
dotnet test .\PracticaJWTcore.sln --no-restore
```

Resultado observado:

```text
Correctas! - Con error: 0, Superado: 11, Omitido: 0, Total: 11
```

### Swagger

Antes de esta actualizacion se verifico Swagger con:

```text
https://localhost:7184/swagger/v1/swagger.json
```

Resultado:

- HTTP 200.
- Contiene `/api/Ventas`.
- Contiene `/api/stock/movimientos`.
- Contiene `/api/Articulos`.

## 16. Riesgos pendientes

- `ServicesContext`, hooks React y Axios mencionados en el PDF frontend no existen en el frontend actual visible. No se aplicaron cambios React.
- Algunos services de modal siguen accediendo directamente a `PracticaJWTcoreContext`.
- `ServiceController`, `CustomerController` y `AppointmentController` aun exponen entidades o contratos historicos en algunos metodos.
- No hay middleware global de errores.
- No hay autorizacion aplicada sistematicamente.
- `AutenticacionRespository` conserva typo historico en el nombre para no romper referencias.
- `Appoitment` conserva typo historico para no renombrar muchas referencias.
- `Dtos/Customer.cs` sigue siendo una entidad ubicada fisicamente dentro de `Dtos`.
- `UsuarioRepository` y `AutenticacionRespository` siguen usando stored procedures y `System.Data.SqlClient`.
- La base de datos no fue modificada.
- No se agregaron migraciones.

## 17. Que no se toco para evitar romper el sistema

- Connection string.
- Stored procedures.
- Tablas SQL Server.
- Columnas SQL Server.
- CORS.
- JWT key.
- Rutas publicas existentes.
- Frontend Blazor.
- Payload de login.
- Nombres historicos como `Appoitment` y `AutenticacionRespository`.
- Contratos existentes de servicios, pedidos, customers y appointments.

## 18. Criterio de estado actual

El backend quedo en un estado mas consistente que antes:

- Ventas tiene controller liviano, service con reglas y repository con EF/transaccion.
- Stock tiene controller liviano, service con validaciones y repository con EF.
- Articulos tiene controller liviano, service con reglas y repository con EF.
- Usuarios y autenticacion ya pasan por services.
- Program.cs registra dependencias sin duplicar `AddControllers`.
- DTOs importantes salieron de controllers.
- Los endpoints principales mantienen rutas.
- El proyecto compila.
- Las pruebas actuales pasan.
- Swagger puede exponer los endpoints nuevos.

No es una Clean Architecture completa. Sigue siendo una API monolitica por capas dentro de un solo proyecto, que es el objetivo correcto para este estado del sistema.

