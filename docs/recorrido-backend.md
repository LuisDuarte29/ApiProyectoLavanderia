# Recorrido del backend

Este documento explica como viajan los datos dentro del backend `PracticaJWTcore` y como se relacionan controllers, services, repositories, `PracticaJWTcoreContext`, SQL Server y el frontend `BlazorApp1`.

## 1. Idea general

El patron recomendado del proyecto es:

```text
Frontend -> Controller -> Service -> Repository -> DbContext / SQL Server
```

Cuando vuelve la respuesta:

```text
SQL Server -> DbContext -> Repository -> Service -> Controller -> Frontend
```

Cada capa tiene una responsabilidad:

- Controller: recibe HTTP, interpreta rutas, hace binding del request, llama al service y devuelve status codes.
- Service: contiene reglas de negocio, validaciones y coordinacion de operaciones.
- Repository: accede a datos con EF Core, ADO.NET o stored procedures.
- `PracticaJWTcoreContext`: representa EF Core y centraliza `DbSet` y mapeos SQL.
- DTOs: separan el contrato de la API de las entidades persistidas.
- Models: representan entidades o modelos persistidos.

## 2. Capas principales

### Controllers

Ubicacion: `PracticaJWTcore/Controllers`.

Exponen endpoints HTTP. Los controllers nuevos de ventas, stock y articulos son livianos: reciben DTOs, llaman al service y traducen el resultado a `Ok`, `Created`, `BadRequest`, `NotFound` o `NoContent`.

### Services

Ubicacion: `PracticaJWTcore/Services`.

Contienen reglas de negocio y coordinacion. Ejemplos claros:

- `VentasService`: valida items, stock, calcula IVA/total y coordina transaccion.
- `StockService`: valida movimientos manuales.
- `ArticulosService`: valida nombre, precios, stock, categoria y dependencias.

Algunos services historicos son principalmente pasamanos hacia repositories, pero mantienen el flujo por capas.

### Repositorios

Ubicacion: `PracticaJWTcore/Repositorios`.

Concentran acceso a datos. Usan:

- EF Core con `PracticaJWTcoreContext`.
- ADO.NET con `SqlConnection`/`SqlCommand`.
- Stored procedures como `proc_ComparePass`, `proc_CambioClave` y `proc_InsertUsuario`.

### Dtos

Ubicacion: `PracticaJWTcore/Dtos`.

Son contratos de entrada y salida. Evitan que el frontend dependa siempre de entidades EF.

Ejemplos:

- `CreateVentaDto`
- `VentaResponseDto`
- `StockMovimientoRequestDto`
- `ArticuloRequestDto`
- `UsuarioDTO`
- `TokenRolDTO`

### Models

Ubicacion: `PracticaJWTcore/Models`.

Incluye entidades persistidas como:

- `Venta`
- `VentaDetalle`
- `Articulos`
- `StockMovimiento`
- `Usuarios`
- `Roles`
- `Permisos`
- `Appointment`
- `Service`

### PracticaJWTcoreContext

Archivo: `PracticaJWTcore/Models/PracticaJWTcoreContext.cs`.

Centraliza `DbSet`, mapeos de tablas, columnas, relaciones y tipos SQL. Es el punto donde EF Core sabe como mapear C# contra SQL Server.

### Program.cs

Archivo: `PracticaJWTcore/Program.cs`.

Registra CORS, JWT, Swagger, DbContext, repositories, services, controllers y el pipeline HTTP.

### appsettings.json

Archivo: `PracticaJWTcore/appsettings.json`.

Contiene logging, connection string `DefaultConnection` y clave JWT. No debe modificarse sin revisar impacto.

### BlazorApp1

Proyecto frontend que consume la API. Usa `HttpClient` con base URL `https://localhost:7184/`.

Consume endpoints como:

- `api/Autenticacion`
- `api/service`
- `api/pedidos`
- `api/test`

### Tests

Proyecto: `PracticaJWTCoreTest`.

Prueba reglas de services y contratos de controllers, por ejemplo ventas, stock, articulos, responses `tokenRol` y Locations relativas.

## 3. Recorrido de autenticacion

1. El frontend envia correo y clave desde `BlazorApp1/Pages/Login/Login.razor`.
2. `AutenticacionController` recibe `UsuarioLogin` en `POST /api/Autenticacion`.
3. `AutenticacionServices` procesa la solicitud y delega en el repository.
4. `AutenticacionRespository` ejecuta `proc_ComparePass` con ADO.NET.
5. Si la clave es valida, consulta rol con EF Core.
6. Genera `TokenRolDTO` con JWT y `RolId`.
7. El controller devuelve `{ tokenRol = response }`.
8. El frontend guarda el token en localStorage.
9. En siguientes peticiones algunas pantallas envian `Authorization: Bearer {token}`.
10. `Program.cs` valida JWT cuando un endpoint exige autorizacion.

Archivos involucrados:

- `Controllers/AutenticacionController.cs`
- `Services/IAutenticacionServices.cs`
- `Services/AutenticacionServices.cs`
- `Repositorios/IAutenticacionRepository.cs`
- `Repositorios/AutenticacionRespository.cs`
- `Dtos/TokenRolDTO.cs`
- `Models/UsuarioLogin.cs`
- `Program.cs`
- `BlazorApp1/Pages/Login/Login.razor`

## 4. Recorrido por modulo

### Ventas

Flujo:

```text
VentasController -> VentasService -> VentasRepository -> PracticaJWTcoreContext -> SQL Server
```

Archivos:

- `Controllers/VentasController.cs`
- `Services/IVentasService.cs`
- `Services/VentasService.cs`
- `Repositorios/IVentasRepository.cs`
- `Repositorios/VentasRepository.cs`
- `Dtos/Ventas/*`
- `Models/Venta.cs`
- `Models/VentaDetalle.cs`
- `Models/Articulos.cs`
- `Models/StockMovimiento.cs`
- `Models/PracticaJWTcoreContext.cs`

Crear venta:

- Recibe `CreateVentaDto`.
- Valida items.
- Valida cantidades positivas.
- Consulta articulos.
- Valida stock suficiente.
- Calcula subtotal, IVA y total.
- Crea `Venta`.
- Crea `VentaDetalle`.
- Descuenta `Articulos.StockActual`.
- Crea `StockMovimiento`.
- Guarda en transaccion.
- Devuelve `VentaResponseDto`.

### Stock

Flujo:

```text
StockController -> StockService -> StockRepository -> PracticaJWTcoreContext -> SQL Server
```

El modulo permite listar, crear, editar y eliminar movimientos de stock.

Reglas confirmadas:

- Valida que la cantidad sea mayor a cero.
- Valida que el articulo exista.
- Los movimientos manuales se registran en `StockMovimientos`.
- El comportamiento actual no actualiza `Articulos.StockActual` desde `StockService`.

### Articulos / Productos

Flujo:

```text
ArticulosController -> ArticulosService -> ArticulosRepository -> PracticaJWTcoreContext -> SQL Server
```

Operaciones:

- Listar articulos.
- Obtener por id.
- Crear.
- Editar.
- Eliminar.

Validaciones:

- `NombreArticulo` requerido.
- Precios no negativos.
- Stock no negativo.
- Categoria existente si se informa.
- No eliminar si tiene ventas o movimientos.

### Usuarios

Flujo:

```text
UsuariosController -> UsuarioServices -> UsuarioRepository -> EF Core / ADO.NET / Stored Procedures
```

Funciones:

- Crear usuario con `proc_InsertUsuario`.
- Listar usuarios.
- Listar roles.
- Listar permisos.
- Asignar permisos por rol y componente.
- Listar `ComponentsForm`.

### Roles y permisos

Tablas involucradas:

- `Roles`
- `Permisos`
- `RolesPermisos`
- `ComponentsForm`
- `Usuarios`

El backend lista permisos y permite sincronizar permisos por rol/componente. No se confirma uso como policies de autorizacion backend; el uso parece principalmente orientado a UI y permisos de pantalla.

### Clientes

Flujo:

```text
CustomerController -> CustomerServices -> CustomerRepository -> PracticaJWTcoreContext
```

Operaciones:

- Obtener cliente.
- Listar clientes.
- Crear cliente.
- Actualizar cliente.
- Eliminar cliente.

DTOs:

- `CreateCustomerDto`
- `CustomerDto`

Riesgo confirmado: `UpdateCustomer` sigue recibiendo `Customer` como payload historico.

### Servicios

Flujo:

```text
ServiceController -> ServiceServices -> ServicesRepository -> PracticaJWTcoreContext
```

Operaciones:

- CRUD de servicios.
- Endpoint auxiliar `GET /api/Service/Articulos`.

Riesgo confirmado: varios endpoints reciben o devuelven la entidad EF `Service`.

### Pedidos / Appointments

Flujo:

```text
AppointmentController / PedidosController -> AppoitmentServices / PedidosServices -> AppointmentRepository / PedidosRepository -> PracticaJWTcoreContext
```

Operaciones:

- Crear appointment.
- Actualizar appointment.
- Eliminar appointment.
- Listar appointments.
- Listar pedidos con servicios asociados.

Relaciones:

- `Appointment`
- `AppointmentService`
- `Service`
- `Customer`
- `Vehicle`

### Pagina base / combos

Flujo:

```text
PaginaBaseController -> VehicleModalServices / CustomerModalServices / ServiciosModalServices -> PracticaJWTcoreContext
```

Estos endpoints alimentan combos o selects del frontend:

- `GET /api/PaginaBase/vehicle`
- `GET /api/PaginaBase/customer`
- `GET /api/PaginaBase/servicios`

Deuda confirmada: estos services acceden directo al `DbContext`, sin repository propio.

## 5. Ejemplos detallados de recorrido

### Ejemplo: crear venta

1. El frontend llama a `POST /api/Ventas`.
2. `VentasController` recibe `CreateVentaDto`.
3. El controller llama a `VentasService.CreateVenta`.
4. `VentasService` valida que haya items.
5. Valida cantidades positivas.
6. Pide articulos a `VentasRepository`.
7. Valida stock suficiente.
8. Calcula subtotal, IVA del 10% y total.
9. Ejecuta transaccion.
10. Crea `Venta`.
11. Crea `VentaDetalle`.
12. Descuenta `Articulos.StockActual`.
13. Registra `StockMovimiento`.
14. Guarda cambios.
15. Devuelve `VentaResponseDto`.

### Ejemplo: crear movimiento de stock

1. El frontend llama a `POST /api/stock/movimientos`.
2. `StockController` recibe `StockMovimientoRequestDto`.
3. `StockController` llama a `StockService.CreateMovimiento`.
4. `StockService` valida request, cantidad y articulo.
5. `StockRepository` consulta `Articulos`.
6. `StockService` crea `StockMovimiento`.
7. `StockRepository` agrega el movimiento y guarda cambios.
8. El controller devuelve `201 Created` con `StockMovimientoResponseDto`.

### Ejemplo: crear articulo

1. El frontend llama a `POST /api/Articulos`.
2. `ArticulosController` recibe `ArticuloRequestDto`.
3. El controller llama a `ArticulosService.Create`.
4. El service valida nombre.
5. Valida precios y stock.
6. Valida categoria si se informo `IdCategoria`.
7. Convierte DTO a entidad `Articulos`.
8. `ArticulosRepository` agrega la entidad.
9. Se guarda con `SaveChangesAsync`.
10. El controller devuelve `201 Created` con `ArticuloResponseDto`.

### Ejemplo: login

1. `BlazorApp1` envia correo/clave a `POST /api/Autenticacion`.
2. `AutenticacionController` recibe `UsuarioLogin`.
3. `AutenticacionServices.Login` delega.
4. `AutenticacionRespository.Login` ejecuta `proc_ComparePass`.
5. Si es valido, consulta rol del usuario.
6. `GenerateToken` crea JWT con rol y correo.
7. Devuelve `TokenRolDTO`.
8. El controller responde `{ tokenRol = response }`.
9. El frontend guarda el token.

## 6. Tabla resumen por modulo

| Modulo | Controller | Service | Repository | DTO principal | Tablas principales | Endpoint aproximado | Observacion |
|---|---|---|---|---|---|---|---|
| Autenticacion | `AutenticacionController` | `AutenticacionServices` | `AutenticacionRespository` | `TokenRolDTO` | `Usuarios`, `Roles` | `/api/Autenticacion` | Usa SP y JWT. |
| Usuarios | `UsuariosController` | `UsuarioServices` | `UsuarioRepository` | `CreateUsuariosDTO`, `UsuarioDTO` | `Usuarios`, `Roles` | `/api/Usuarios/*` | Usa EF Core y SP. |
| Roles/permisos | `UsuariosController` | `UsuarioServices` | `UsuarioRepository` | `RolesPermisoDTO` | `RolesPermisos`, `Permisos`, `ComponentsForm` | `/api/Usuarios/GetPermisosList` | Permisos por rol/componente. |
| Ventas | `VentasController` | `VentasService` | `VentasRepository` | `CreateVentaDto`, `VentaResponseDto` | `Ventas`, `VentaDetalles`, `Articulos`, `StockMovimientos` | `/api/Ventas` | Usa transaccion y afecta stock. |
| Stock | `StockController` | `StockService` | `StockRepository` | `StockMovimientoRequestDto` | `StockMovimientos`, `Articulos` | `/api/stock/movimientos` | Movimiento manual no ajusta `StockActual`. |
| Articulos | `ArticulosController` | `ArticulosService` | `ArticulosRepository` | `ArticuloRequestDto` | `Articulos`, `Categorias` | `/api/Articulos` | Valida dependencias al eliminar. |
| Clientes | `CustomerController` | `CustomerServices` | `CustomerRepository` | `CreateCustomerDto`, `CustomerDto` | `Customer` | `/api/Customer` | Update usa entidad `Customer`. |
| Servicios | `ServiceController` | `ServiceServices` | `ServicesRepository` | `Service` | `Services` | `/api/Service` | Devuelve entidad EF. |
| Pedidos/Appointments | `AppointmentController`, `PedidosController` | `AppoitmentServices`, `PedidosServices` | `AppointmentRepository`, `PedidosRepository` | `AppoitmentDTO`, `AppoitmentDetailsDTO` | `Appointments`, `AppointmentServices`, `Services` | `/api/Appointment`, `/api/Pedidos` | Mantiene typo historico. |
| Pagina base | `PaginaBaseController` | Modal services | Sin repository dedicado | Modal models | `Vehicles`, `Customer`, `Services` | `/api/PaginaBase/*` | Services acceden directo a DbContext. |

## 7. Tabla de endpoints importantes

| Metodo | Endpoint | Controller | Service | Repository | Request | Response | Observacion |
|---|---|---|---|---|---|---|---|
| POST | `/api/Autenticacion` | `AutenticacionController` | `AutenticacionServices` | `AutenticacionRespository` | `UsuarioLogin` | objeto anonimo `{ tokenRol }` | Usa SP y JWT. |
| POST | `/api/Autenticacion/CambioClave` | `AutenticacionController` | `AutenticacionServices` | `AutenticacionRespository` | `CambioClave` | `Ok` u objeto anonimo | Usa SP. |
| POST | `/api/Usuarios/CreateUsuario` | `UsuariosController` | `UsuarioServices` | `UsuarioRepository` | `CreateUsuariosDTO` | objeto anonimo `{ mensaje }` | Usa `proc_InsertUsuario`. |
| GET | `/api/Usuarios/GetUsuarios` | `UsuariosController` | `UsuarioServices` | `UsuarioRepository` | none | `UsuarioDTO` | Usa DTO. |
| PUT | `/api/Usuarios/CreatePermisosRole` | `UsuariosController` | `UsuarioServices` | `UsuarioRepository` | `RolesPermisoDTO` | `Ok/NotFound` | Actualiza permisos. |
| POST | `/api/Ventas` | `VentasController` | `VentasService` | `VentasRepository` | `CreateVentaDto` | `VentaResponseDto` | Usa transaccion y afecta stock. |
| GET | `/api/Ventas` | `VentasController` | `VentasService` | `VentasRepository` | none | `VentaResponseDto` | DTO. |
| DELETE | `/api/Ventas/{id}` | `VentasController` | `VentasService` | `VentasRepository` | route | `NoContent` | Restaura stock. |
| POST | `/api/stock/movimientos` | `StockController` | `StockService` | `StockRepository` | `StockMovimientoRequestDto` | `StockMovimientoResponseDto` | DTO. |
| POST | `/api/Articulos` | `ArticulosController` | `ArticulosService` | `ArticulosRepository` | `ArticuloRequestDto` | `ArticuloResponseDto` | DTO. |
| DELETE | `/api/Articulos/{id}` | `ArticulosController` | `ArticulosService` | `ArticulosRepository` | route | `NoContent` | Bloquea dependencias. |
| GET | `/api/Service` | `ServiceController` | `ServiceServices` | `ServicesRepository` | none | `Service` | Devuelve entidad EF. |
| POST | `/api/Service` | `ServiceController` | `ServiceServices` | `ServicesRepository` | `Service` | `CreatedResult` | Recibe entidad EF. |
| GET | `/api/Pedidos` | `PedidosController` | `PedidosServices` | `PedidosRepository` | none | `AppoitmentDetailsDTO` | Consumido por BlazorApp1. |
| POST | `/api/Appointment` | `AppointmentController` | `AppoitmentServices` | `AppointmentRepository` | `CreateAppoitmentDetailsDTO` | `CreatedResult` | Crea cabecera y servicios. |
| GET | `/api/PaginaBase/vehicle` | `PaginaBaseController` | `VehicleModalServices` | none | none | `VehicleModal` | Service accede a DbContext. |

## 8. Reglas para futuras mejoras

- Los controllers no deberian acceder directamente al DbContext.
- Los controllers deberian llamar a services.
- Los services deberian contener reglas de negocio.
- Los repositories deberian contener acceso a datos.
- Los DTOs deberian proteger el contrato con el frontend.
- No conviene exponer entidades EF directamente.
- No cambiar endpoints sin revisar frontend.
- No cambiar DTOs sin revisar consumidores.
- No tocar stock sin probar ventas.
- No tocar permisos sin revisar frontend y backend.
- No mezclar refactor con nuevas funcionalidades.
- No cambiar stored procedures sin revisar C# y SQL Server.

## 9. Riesgos actuales o deuda tecnica

Confirmado en codigo:

- Endpoints principales sin `[Authorize]` sistematico.
- CORS abierto con `PermitirTodo`.
- Services modal que acceden directo a `PracticaJWTcoreContext`.
- Entidades EF expuestas en `ServiceController` y parte de `CustomerController`.
- Objetos anonimos como response en login y usuarios.
- Typos historicos como `Appoitment` y `AutenticacionRespository`.
- No se detecta middleware global de errores.
- No hay formato uniforme de error en todos los controllers.
- `StockService` registra movimientos manuales sin actualizar `Articulos.StockActual`.
- `Program.cs` concentra muchas configuraciones.

No confirmado:

- Uso de permisos como policies de autorizacion backend.
- Consumo frontend actual de ventas, stock o articulos.

## 10. Glosario simple

- Controller: clase que expone endpoints HTTP. Ejemplo: `VentasController`.
- Service: clase que contiene reglas de negocio. Ejemplo: `VentasService`.
- Repository: clase que consulta o guarda datos. Ejemplo: `VentasRepository`.
- Entity: clase persistida o mapeada a tabla. Ejemplo: `Venta`.
- DTO: objeto de request/response. Ejemplo: `CreateVentaDto`.
- Request: datos que entran desde el frontend.
- Response: datos que vuelve la API al frontend.
- DbContext: clase EF Core que representa la base. Ejemplo: `PracticaJWTcoreContext`.
- EF Core: ORM usado para consultar SQL Server con LINQ.
- ADO.NET: acceso directo a SQL con `SqlConnection` y `SqlCommand`.
- Stored Procedure: procedimiento SQL llamado desde C#. Ejemplo: `proc_ComparePass`.
- Dependency Injection: mecanismo que registra services/repositories en `Program.cs`.
- JWT: token firmado usado para identificar al usuario.
- Claim: dato dentro del JWT, como rol o correo.
- Middleware: componente del pipeline HTTP, como authentication o authorization.
- Endpoint: ruta publica de la API.
- Payload: cuerpo JSON enviado o recibido.
- Regla de negocio: decision del dominio, como validar stock suficiente.
- Transaccion: grupo de escrituras que se confirman o revierten juntas.
- StockActual: cantidad disponible del articulo.
- Contrato API: metodo, ruta, request, response y status codes.
- Contrato SQL: tablas, columnas, relaciones y stored procedures.
- Refactor seguro: mejora interna sin cambiar comportamiento externo.

