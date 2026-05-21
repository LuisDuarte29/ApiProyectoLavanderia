# Arquitectura Backend Actualizada

Documento generado a partir del codigo actual del repositorio `ApiProyectoLavanderia`.

Fecha de revision: 2026-05-18.

Referencia solicitada: `ARQUITECTURA_BACKEND(3).pdf`.

Nota de evidencia: en `C:\Users\user\Downloads` no se encontro un archivo con ese nombre exacto. La referencia disponible inspeccionada fue `ARQUITECTURA_BACKEND.pdf`, que describe el diagnostico anterior. Las conclusiones de este documento se basan principalmente en el codigo actual del repositorio.

## 1. Resumen general

El backend principal es `PracticaJWTcore`, una API ASP.NET Core sobre .NET 8.

Usa SQL Server como base de datos. El acceso a datos es mixto: Entity Framework Core mediante `PracticaJWTcoreContext` y ADO.NET con `SqlConnection`/`SqlCommand` para procedimientos almacenados en autenticacion y usuarios.

La API tiene JWT Bearer configurado, Swagger con esquema Bearer, CORS abierto con la politica `PermitirTodo`, inyeccion de dependencias con `AddScoped` y serializacion JSON con `JsonNamingPolicy.CamelCase`.

Existe un frontend Blazor WebAssembly en `BlazorApp1` que consume la API con base URL `https://localhost:7184/`. Tambien existen `BlazorFrontend` y `ApiProyectoFrontend`, pero el consumo real de endpoints se encontro principalmente en `BlazorApp1`.

La arquitectura actual es una arquitectura por capas parcial, ahora mas ordenada que el diagnostico anterior en los modulos de ventas, stock y articulos. No es Clean Architecture: no hay separacion por proyectos de dominio/aplicacion/infraestructura y el `DbContext` vive dentro de `Models/`.

Flujo predominante esperado:

```text
Controller -> Service -> Repository -> PracticaJWTcoreContext / SQL Server
```

## 2. Comparacion con el diagnostico anterior

| Problema detectado antes | Estado actual | Se corrigio | Evidencia en codigo | Observacion |
|---|---|---:|---|---|
| `VentasController` tenia demasiada logica | Ahora delega en `IVentasService` | Corregido | `Controllers/VentasController.cs`, `Services/VentasService.cs`, `Repositorios/VentasRepository.cs` | La logica de venta, stock, totales y transaccion salio del controller. |
| `StockController` accedia directo a `DbContext` | Ahora delega en `IStockService` | Corregido | `Controllers/StockController.cs`, `Services/StockService.cs`, `Repositorios/StockRepository.cs` | El registro manual de movimientos no actualiza `Articulos.StockActual`; esto parece decision compatible conservada. |
| `ArticulosController` usaba entidades como request/response | Ahora usa `ArticuloRequestDto` y `ArticuloResponseDto` | Corregido | `Dtos/Articulos/*`, `Controllers/ArticulosController.cs` | Se mantiene una entidad `Articulos` en capa interna. |
| Faltaban services para ventas, stock y articulos | Existen services dedicados | Corregido | `IVentasService`, `IStockService`, `IArticulosService` | Tienen reglas de negocio reales. |
| Faltaban repositories para ventas, stock y articulos | Existen repositories dedicados | Corregido | `IVentasRepository`, `IStockRepository`, `IArticulosRepository` | Acceden a EF Core y proyectan DTOs. |
| Habia DTOs internos dentro de controllers | Los DTOs de ventas/stock/articulos estan en `Dtos/` | Corregido | `Dtos/Ventas`, `Dtos/Stock`, `Dtos/Articulos` | No se detectaron DTOs internos actuales en esos controllers. |
| `AutenticacionController` llamaba directo a repository | Ahora llama a `IAutenticacionServices` | Corregido | `Controllers/AutenticacionController.cs` | El service delega al repository. |
| `UsuariosController` llamaba directo a repository | Ahora llama a `IUsuarioServices` | Corregido | `Controllers/UsuariosController.cs` | Persisten try/catch y respuestas mixtas. |
| `Program.cs` tenia `AddControllers` duplicado | Se ve un solo registro | Corregido | `Program.cs` | Mantiene Swagger, JWT, CORS y DI. |
| Faltaba manejo global o consistente de errores | No hay middleware global | Pendiente | No existe carpeta `Middlewares/` ni middleware propio | Hay manejo por controller/service con `ServiceResult` en modulos nuevos. |
| Faltaba autorizacion efectiva con `[Authorize]` | JWT existe, pero no hay uso sistematico de `[Authorize]` | Pendiente | `TestController` usa `[AllowAnonymous]`; controllers principales no tienen `[Authorize]` | El frontend envia token en varias pantallas, pero backend no lo exige globalmente. |
| Habia entidades expuestas directamente al frontend | Algunos modulos nuevos usan DTOs; otros siguen exponiendo entidades | Parcialmente corregido | `ServiceController` usa `Service`; `CustomerController.UpdateCustomer` usa `Customer` | Ventas/stock/articulos estan mejor aislados. |
| Habia respuestas anonimas dificiles de versionar | Siguen existiendo algunas | Parcialmente corregido | Login devuelve `{ tokenRol = response }`; usuarios devuelve `{ mensaje = ... }` | Se conservaron contratos historicos. |
| Faltaba documentacion de contratos | Este documento la actualiza | En proceso | `docs/arquitectura-backend-actualizada.md` | No reemplaza Swagger ni pruebas contractuales. |

## 3. Mapa actual de carpetas

| Carpeta | Contenido | Responsabilidad | Estado |
|---|---|---|---|
| `PracticaJWTcore/Controllers` | Controllers HTTP | Rutas, binding, status codes y llamada a services | Bien usada en modulos nuevos; algunos controllers historicos aun devuelven entidades. |
| `PracticaJWTcore/Services` | Interfaces e implementaciones de negocio | Validaciones, coordinacion y delegacion a repositorios | Ventas/stock/articulos tienen reglas reales; varios services historicos son pasamanos. |
| `PracticaJWTcore/Repositorios` | Interfaces y repositorios | EF Core, ADO.NET, stored procedures y persistencia | Bien usado; autenticacion/usuarios mezclan EF y ADO.NET. |
| `PracticaJWTcore/Dtos` | DTOs planos y subcarpetas por modulo | Contratos request/response | Mejorado para ventas, stock y articulos; conserva nombres historicos y algunos namespaces inconsistentes. |
| `PracticaJWTcore/Models` | Entidades, modelos auxiliares y `PracticaJWTcoreContext` | Persistencia y mapeo EF Core | Mezcla entidades con modelos auxiliares/modal. |
| `PracticaJWTcore/Properties` | `launchSettings.json` | Perfiles de ejecucion | Configuracion local. |
| `PracticaJWTCoreTest` | Tests xUnit/Moq | Verificacion de services y wiring | Existen pruebas para ventas, stock, articulos y locations. |
| `BlazorApp1` | Frontend Blazor WebAssembly | Consumo de API | Consume autenticacion, servicios, pedidos y test. |
| `docs` | Documentacion Markdown | Documentacion tecnica | Ya existia con documentos previos. |

No se detectaron carpetas `Data`, `Helpers` ni `Middlewares`.

## 4. Flujo arquitectonico recomendado actual

Flujo de entrada:

```text
Frontend -> Controller -> Service -> Repository -> PracticaJWTcoreContext / SQL Server
```

Flujo de salida:

```text
SQL Server -> Repository -> Service -> Controller -> Frontend
```

Responsabilidades:

| Capa | Que debe hacer | Ejemplo actual |
|---|---|---|
| Controller | HTTP, status codes, binding, llamada al service | `VentasController.CreateVenta` recibe `CreateVentaDto` y llama a `IVentasService`. |
| Service | Reglas de negocio, validaciones, coordinacion y transacciones si aplica | `VentasService` valida items, stock, calcula totales y coordina transaccion. |
| Repository | EF Core, ADO.NET, stored procedures, queries y persistencia | `VentasRepository` usa `PracticaJWTcoreContext` y `BeginTransactionAsync`. |
| DTOs | Requests y responses estables para frontend/API | `VentaResponseDto`, `StockMovimientoRequestDto`, `ArticuloRequestDto`. |
| Models | Entidades persistidas y `DbContext` | `Venta`, `VentaDetalle`, `Articulos`, `StockMovimiento`, `PracticaJWTcoreContext`. |

## 5. Recorrido completo por modulo

### Autenticacion

- Controller: `AutenticacionController`.
- Service: `IAutenticacionServices` / `AutenticacionServices`.
- Repository: `IAutenticacionRepository` / `AutenticacionRespository`.
- DTOs/modelos: `UsuarioLogin`, `CambioClave`, `TokenRolDTO`.
- Stored procedures: `proc_ComparePass`, `proc_CambioClave`.
- Endpoints: `POST /api/Autenticacion`, `POST /api/Autenticacion/CambioClave`.

Flujo de login: el controller recibe `UsuarioLogin`, llama al service y el repository ejecuta `proc_ComparePass`. Si la comparacion es valida, consulta rol con EF Core y genera un JWT con claims `ClaimTypes.Role` y `ClaimTypes.NameIdentifier`. La respuesta conserva el contrato historico `{ tokenRol = response }`.

Riesgos pendientes: el response es anonimo, el token expira en 1 hora, no hay refresh token y no hay `[AllowAnonymous]` explicito en login aunque tampoco hay autorizacion global.

### Usuarios

- Controller: `UsuariosController`.
- Service: `IUsuarioServices` / `UsuarioServices`.
- Repository: `IUsuariosRepository` / `UsuarioRepository`.
- DTOs: `CreateUsuariosDTO`, `UsuarioDTO`, `RolesDTO`, `RolesPermisoDTO`, `PermisosDTO`.
- Stored procedure: `proc_InsertUsuario`.
- Endpoints: `CreateUsuario`, roles, usuarios, permisos, componentes.

El modulo gestiona usuarios, roles, permisos y componentes de formulario. La asignacion de permisos se guarda en `RolesPermisos`. El codigo sugiere que los permisos se consumen principalmente para UI, porque no se detecto aplicacion sistematica de `[Authorize]` o policies en endpoints.

Riesgos pendientes: `UsuariosController` mantiene try/catch con `Console.WriteLine`, y `UsuarioRepository` tambien captura excepciones y devuelve `false`.

### Ventas

- Controller actual: `VentasController`.
- Service actual: `VentasService`.
- Repository actual: `VentasRepository`.
- DTOs actuales: `CreateVentaDto`, `VentaItemDto`, `UpdateVentaDto`, `VentaResponseDto`, `VentaDetalleResponseDto`.
- Entidades: `Venta`, `VentaDetalle`, `Articulos`, `StockMovimiento`.
- Tablas: `Ventas`, `VentaDetalles`, `Articulos`, `StockMovimientos`, tambien consulta `Customer` y `Usuarios` para nombres.
- Endpoints: `POST /api/Ventas`, `GET /api/Ventas`, `GET /api/Ventas/{id}`, `PUT /api/Ventas/{id}`, `DELETE /api/Ventas/{id}`.

Flujo de crear venta:

1. `VentasController.CreateVenta` recibe `CreateVentaDto`.
2. `VentasService.CreateVenta` valida que existan items.
3. Valida cantidades positivas.
4. Obtiene articulos por id.
5. Verifica existencia y stock suficiente.
6. Ejecuta transaccion mediante `VentasRepository.ExecuteInTransaction`.
7. Crea `Venta`.
8. Crea `VentaDetalle` por item.
9. Descuenta `Articulos.StockActual`.
10. Registra `StockMovimiento` de tipo `Salida`.
11. Calcula subtotal, IVA del 10% y total.
12. Devuelve `VentaResponseDto`.

La logica ya salio del controller. El controller no accede directo a `PracticaJWTcoreContext`. El contrato principal de `POST /api/Ventas` se mantiene con DTOs externos y response de venta/detalles.

Riesgo pendiente: `UpdateVenta` devuelve `ServiceResult<object>` con entidad `Venta` interna como valor; convendria normalizarlo a DTO en una mejora futura.

### Stock

- Controller: `StockController`.
- Service: `StockService`.
- Repository: `StockRepository`.
- DTOs: `StockMovimientoRequestDto`, `StockMovimientoResponseDto`.
- Entidades/tablas: `StockMovimiento` / `StockMovimientos`, `Articulos`.
- Endpoints: `GET /api/stock/movimientos`, `GET /api/stock/movimientos/{id}`, `POST /api/stock/movimientos`, `PUT /api/stock/movimientos/{id}`, `DELETE /api/stock/movimientos/{id}`.

El service valida movimiento requerido, cantidad positiva, articulo existente y coincidencia de id en update. El repository proyecta el nombre del articulo.

Estado de `Articulos.StockActual`: los movimientos manuales de stock crean/actualizan/eliminan registros en `StockMovimientos`, pero no modifican `Articulos.StockActual`. Las ventas si actualizan `StockActual`. Esto conserva compatibilidad, pero deja riesgo de inconsistencia si se espera que un movimiento manual ajuste stock fisico.

### Articulos / Productos

- Controller: `ArticulosController`.
- Service: `ArticulosService`.
- Repository: `ArticulosRepository`.
- DTOs: `ArticuloRequestDto`, `ArticuloResponseDto`.
- Entidades/tablas: `Articulos`, `Categorias`, `VentaDetalles`, `StockMovimientos`.
- Endpoints: `GET /api/Articulos`, `GET /api/Articulos/{id}`, `POST /api/Articulos`, `PUT /api/Articulos/{id}`, `DELETE /api/Articulos/{id}`.

Validaciones actuales: `NombreArticulo` requerido, precios no negativos, stock no negativo y categoria existente cuando se informa `IdCategoria`. Al eliminar, bloquea si existen ventas o movimientos asociados.

Estado: ya no expone directamente entidad EF en los endpoints principales de articulos; usa DTOs.

### Pedidos / Appointments

- Controllers: `PedidosController`, `AppointmentController`.
- Services: `IPedidosServices` / `PedidosServices`, `IAppointmentServices` / `AppoitmentServices`.
- Repositories: `IPedidosRepository` / `PedidosRepository`, `IAppointmentRepository` / `AppointmentRepository`.
- DTOs: `AppoitmentDTO`, `AppoitmentDetailsDTO`, `CreateAppoitmentDetailsDTO`, `UpdateAppoitmentDetailsDTO`, `ServiceDto`.
- Entidades/tablas: `Appointment`, `AppointmentService`, `Customer`, `Vehicle`, `Service`.
- Endpoints: `GET /api/Pedidos`; CRUD parcial en `/api/Appointment`.

`PedidosRepository.GetPedidos` proyecta appointments con vehiculo, empleado y servicios. `AppointmentRepository` crea cabecera y detalles, lista appointments y actualiza servicios asociados.

Riesgos pendientes: typo historico `Appoitment`, services principalmente pasamanos y manejo de inexistentes con `FirstAsync` que puede lanzar excepciones.

### Clientes

- Controller: `CustomerController`.
- Service: `ICustomerServices` / `CustomerServices`.
- Repository: `ICustomerRepository` / `CustomerRepository`.
- DTOs: `CreateCustomerDto`, `CustomerDto`; tambien se usa `Customer` en update.
- Endpoints: `GET /api/Customer`, `GET /api/Customer/{id}`, `POST /api/Customer`, `PUT /api/Customer`, `DELETE /api/Customer/{id}`.

Flujo CRUD: el controller delega al service y el repository usa EF Core. `GET` lista `CustomerDto`, `POST` recibe `CreateCustomerDto`, pero `PUT` recibe entidad `Customer`.

Riesgo pendiente: `Dtos/Customer.cs` declara namespace `PracticaJWTcore.Entities`, lo que mezcla ubicacion fisica y responsabilidad.

### Servicios

- Controller: `ServiceController`.
- Service: `IServiceServices` / `ServiceServices`.
- Repository: `IServicesRepository` / `ServicesRepository`.
- DTOs o entidades: usa principalmente entidad `Service`; `GetArticulos` devuelve `Articulos`.
- Endpoints: `GET /api/Service`, `GET /api/Service/{idServicioLong}`, `POST /api/Service`, `PUT /api/Service`, `DELETE /api/Service/{id}`, `GET /api/Service/Articulos`.

El modulo delega correctamente por capas, pero sigue exponiendo entidades EF como contrato API.

### Roles y permisos

- Controller: `UsuariosController`.
- Service: `UsuarioServices`.
- Repository: `UsuarioRepository`.
- DTOs/modelos: `RolesDTO`, `RolesPermisoDTO`, `PermisosDTO`, `PermisosModal`, `ComponentsForm`.
- Tablas: `Roles`, `Permisos`, `RolesPermisos`, `ComponentsForm`, `Usuarios`.
- Endpoints: `GetRoles`, `GetRoleList`, `GetPermisosList`, `GetListPermisos`, `GetListPermisosAsignacion`, `CreatePermisosRole`, `GetComponentsForms`.

Los permisos se asignan por rol y componente. No se confirmo que estos permisos protejan endpoints backend; la evidencia apunta a uso principalmente de UI o consultas de permisos.

## 6. Contratos API actuales

| Metodo | Endpoint | Controller | Request DTO | Response DTO | Service | Repository | Frontend que lo consume | Observacion |
|---|---|---|---|---|---|---|---|---|
| POST | `/api/Autenticacion` | `AutenticacionController` | `UsuarioLogin` | objeto anonimo `{ tokenRol }` | `IAutenticacionServices` | `IAutenticacionRepository` | `BlazorApp1/Pages/Login/Login.razor` | Contrato historico. |
| POST | `/api/Autenticacion/CambioClave` | `AutenticacionController` | `CambioClave` | `Ok` u objeto anonimo error | `IAutenticacionServices` | `IAutenticacionRepository` | No confirmado | Usa SP. |
| POST | `/api/Usuarios/CreateUsuario` | `UsuariosController` | `CreateUsuariosDTO` | objeto anonimo `{ mensaje }` | `IUsuarioServices` | `IUsuariosRepository` | No confirmado | Crea con SP. |
| GET | `/api/Usuarios/GetRoles` | `UsuariosController` | none | `RoleModal` | `IUsuarioServices` | `IUsuariosRepository` | No confirmado | Modelo modal. |
| GET | `/api/Usuarios/GetUsuarios` | `UsuariosController` | none | `UsuarioDTO` | `IUsuarioServices` | `IUsuariosRepository` | No confirmado | Proyeccion DTO. |
| GET | `/api/Usuarios/GetRoleList` | `UsuariosController` | none | `RolesDTO` | `IUsuarioServices` | `IUsuariosRepository` | No confirmado | Lista roles. |
| GET | `/api/Usuarios/GetListPermisos/{roleId}/{componentsFormSelect}` | `UsuariosController` | route | `PermisosDTO` | `IUsuarioServices` | `IUsuariosRepository` | No confirmado | Permisos por componente. |
| GET | `/api/Usuarios/GetListPermisosAsignacion/{roleId}/{componentsFormSelect}` | `UsuariosController` | route | `PermisosDTO` | `IUsuarioServices` | `IUsuariosRepository` | No confirmado | Asignacion. |
| GET | `/api/Usuarios/GetPermisosList` | `UsuariosController` | none | `PermisosModal` | `IUsuarioServices` | `IUsuariosRepository` | No confirmado | Modelo modal. |
| PUT | `/api/Usuarios/CreatePermisosRole` | `UsuariosController` | `RolesPermisoDTO` | `Ok/NotFound` | `IUsuarioServices` | `IUsuariosRepository` | No confirmado | Actualiza permisos. |
| GET | `/api/Usuarios/GetComponentsForms` | `UsuariosController` | none | `ComponentsForm` | `IUsuarioServices` | `IUsuariosRepository` | No confirmado | Devuelve entidad/modelo. |
| POST | `/api/Ventas` | `VentasController` | `CreateVentaDto` | `VentaResponseDto` | `IVentasService` | `IVentasRepository` | No confirmado | Crea venta y descuenta stock. |
| GET | `/api/Ventas` | `VentasController` | none | `VentaResponseDto` | `IVentasService` | `IVentasRepository` | No confirmado | Lista ventas. |
| GET | `/api/Ventas/{id}` | `VentasController` | route | `VentaResponseDto` | `IVentasService` | `IVentasRepository` | No confirmado | Detalle. |
| PUT | `/api/Ventas/{id}` | `VentasController` | `UpdateVentaDto` | objeto/entidad interna | `IVentasService` | `IVentasRepository` | No confirmado | Parcialmente normalizado. |
| DELETE | `/api/Ventas/{id}` | `VentasController` | route | `NoContent` | `IVentasService` | `IVentasRepository` | No confirmado | Restaura stock. |
| GET | `/api/stock/movimientos` | `StockController` | none | `StockMovimientoResponseDto` | `IStockService` | `IStockRepository` | No confirmado | Proyeccion DTO. |
| GET | `/api/stock/movimientos/{id}` | `StockController` | route | `StockMovimientoResponseDto` | `IStockService` | `IStockRepository` | No confirmado | Detalle. |
| POST | `/api/stock/movimientos` | `StockController` | `StockMovimientoRequestDto` | `StockMovimientoResponseDto` | `IStockService` | `IStockRepository` | No confirmado | No ajusta `StockActual`. |
| PUT | `/api/stock/movimientos/{id}` | `StockController` | `StockMovimientoRequestDto` | `StockMovimientoResponseDto` | `IStockService` | `IStockRepository` | No confirmado | Valida id. |
| DELETE | `/api/stock/movimientos/{id}` | `StockController` | route | `NoContent` | `IStockService` | `IStockRepository` | No confirmado | Elimina movimiento. |
| GET | `/api/Articulos` | `ArticulosController` | none | `ArticuloResponseDto` | `IArticulosService` | `IArticulosRepository` | No confirmado | DTO. |
| GET | `/api/Articulos/{id}` | `ArticulosController` | route | `ArticuloResponseDto` | `IArticulosService` | `IArticulosRepository` | No confirmado | DTO. |
| POST | `/api/Articulos` | `ArticulosController` | `ArticuloRequestDto` | `ArticuloResponseDto` | `IArticulosService` | `IArticulosRepository` | No confirmado | Valida categoria. |
| PUT | `/api/Articulos/{id}` | `ArticulosController` | `ArticuloRequestDto` | `ArticuloResponseDto` | `IArticulosService` | `IArticulosRepository` | No confirmado | Valida id. |
| DELETE | `/api/Articulos/{id}` | `ArticulosController` | route | `NoContent` | `IArticulosService` | `IArticulosRepository` | No confirmado | Bloquea dependencias. |
| GET | `/api/Service` | `ServiceController` | none | `Service` | `IServiceServices` | `IServicesRepository` | `BlazorApp1/Pages/Services.razor` | Devuelve entidad EF. |
| GET | `/api/Service/{idServicioLong}` | `ServiceController` | route | `Service` | `IServiceServices` | `IServicesRepository` | `BlazorApp1/Pages/ServicesCreacion.razor` | Devuelve entidad EF. |
| POST | `/api/Service` | `ServiceController` | `Service` | `CreatedResult` sin body | `IServiceServices` | `IServicesRepository` | `BlazorApp1/Pages/ServicesCreacion.razor` | Recibe entidad EF. |
| PUT | `/api/Service` | `ServiceController` | `Service` | `Service` list | `IServiceServices` | `IServicesRepository` | `BlazorApp1/Pages/ServicesCreacion.razor` | Recibe entidad EF. |
| DELETE | `/api/Service/{id}` | `ServiceController` | route | `Ok/NotFound` | `IServiceServices` | `IServicesRepository` | `BlazorApp1/Pages/Services.razor` | CRUD historico. |
| GET | `/api/Service/Articulos` | `ServiceController` | none | `Articulos` | `IServiceServices` | `IServicesRepository` | No confirmado | Devuelve entidad EF. |
| GET | `/api/Pedidos` | `PedidosController` | none | `AppoitmentDetailsDTO` | `IPedidosServices` | `IPedidosRepository` | `BlazorApp1/Pages/PagesAppoiment/Pedidos.razor` | DTO. |
| GET | `/api/Appointment` | `AppointmentController` | none | `AppoitmentDTO` | `IAppointmentServices` | `IAppointmentRepository` | No confirmado | DTO. |
| POST | `/api/Appointment` | `AppointmentController` | `CreateAppoitmentDetailsDTO` | `CreatedResult` sin body | `IAppointmentServices` | `IAppointmentRepository` | No confirmado | Crea cabecera/detalles. |
| PUT | `/api/Appointment` | `AppointmentController` | `UpdateAppoitmentDetailsDTO` | `AppoitmentDTO` list | `IAppointmentServices` | `IAppointmentRepository` | No confirmado | Actualiza servicios. |
| GET | `/api/Customer` | `CustomerController` | none | `CustomerDto` | `ICustomerServices` | `ICustomerRepository` | No confirmado | DTO. |
| POST | `/api/Customer` | `CustomerController` | `CreateCustomerDto` | `CreatedResult` sin body | `ICustomerServices` | `ICustomerRepository` | No confirmado | DTO request. |
| PUT | `/api/Customer` | `CustomerController` | `Customer` | `Customer` | `ICustomerServices` | `ICustomerRepository` | No confirmado | Recibe entidad usada como DTO. |
| GET | `/api/PaginaBase/vehicle` | `PaginaBaseController` | none | `VehicleModal` | `IVehicleModal` | service directo a DbContext | No confirmado | Service accede a DbContext. |
| GET | `/api/PaginaBase/customer` | `PaginaBaseController` | none | `CustomerModal` | `ICustomerModal` | service directo a DbContext | No confirmado | Service accede a DbContext. |
| GET | `/api/PaginaBase/servicios` | `PaginaBaseController` | none | `ServiciosModal` | `IServicioModal` | service directo a DbContext | No confirmado | Service accede a DbContext. |
| GET | `/api/Test` | `TestController` | none | texto | none | none | `BlazorApp1/Pages/Test.razor` | `[AllowAnonymous]`. |

## 7. Contrato con SQL Server

`PracticaJWTcoreContext` es el `DbContext` principal. La connection string `DefaultConnection` esta en `appsettings.json` y apunta a SQL Server local y base `ApiProyecto`. No se debe modificar desde esta documentacion.

Tablas principales mapeadas:

| Modulo | Tablas / DbSets | Observacion |
|---|---|---|
| Ventas | `Ventas`, `VentaDetalles`, `Articulos`, `StockMovimientos`, `DocumentosElectronicos` | Ventas descuenta stock y registra movimientos. |
| Stock | `StockMovimientos`, `Articulos` | Movimientos manuales no ajustan `StockActual`. |
| Articulos | `Articulos`, `Categorias`, `VentaDetalles`, `StockMovimientos` | Delete valida dependencias. |
| Usuarios | `Usuarios`, `Roles`, `Permisos`, `RolesPermisos`, `ComponentsForm`, `UsuariosRoles` | Mezcla EF Core y SP. |
| Autenticacion | `Usuarios`, `Roles`; SP `proc_ComparePass`, `proc_CambioClave` | Genera JWT despues de validar clave. |
| Pedidos | `Appointments`, `AppointmentServices`, `Customer`, `Vehicles`, `Services` | Appointments vinculados a servicios. |
| Clientes | `Customer` / `CustomerEntity` | Hay dos DbSet hacia `Customer`. |
| Servicios | `Services`, `Articulos` | `GetArticulos` devuelve articulos desde servicios. |

Stored procedures detectados:

- `proc_ComparePass`
- `proc_CambioClave`
- `proc_InsertUsuario`

## 8. Estado actual de DTOs

| DTO | Tipo | Usado en | Request/Response | Observacion |
|---|---|---|---|---|
| `CreateVentaDto` | Request | Ventas | Request | Movido a `Dtos/Ventas`. |
| `VentaItemDto` | Request | Ventas | Request | Items de venta. |
| `UpdateVentaDto` | Request | Ventas | Request | Actualizacion parcial. |
| `VentaResponseDto` | Response | Ventas | Response | Contrato principal. |
| `VentaDetalleResponseDto` | Response | Ventas | Response | Detalles de venta. |
| `StockMovimientoRequestDto` | Request | Stock | Request | Entrada manual de movimientos. |
| `StockMovimientoResponseDto` | Response | Stock | Response | Incluye `NombreArticulo`. |
| `ArticuloRequestDto` | Request | Articulos | Request | Evita recibir entidad EF. |
| `ArticuloResponseDto` | Response | Articulos | Response | Incluye categoria. |
| `CreateUsuariosDTO` | Request | Usuarios | Request | Crear usuario. |
| `UsuarioDTO` | Response | Usuarios | Response | Lista usuarios con rol/customer. |
| `TokenRolDTO` | Response interno | Autenticacion | Response dentro de `tokenRol` | Se envuelve en objeto anonimo. |
| `CreateCustomerDto` | Request | Clientes | Request | Namespace `ApiSwagger.Dtos`. |
| `CustomerDto` | Response | Clientes | Response | Namespace `ApiSwagger.Dtos`. |
| `Customer` en `Dtos/Customer.cs` | Entity usada como DTO | Clientes | Mixto | Namespace `PracticaJWTcore.Entities`; deuda tecnica. |
| `Service` | Entity usada como DTO | Servicios | Mixto | Expuesto por `ServiceController`. |
| `AppoitmentDTO` | Response | Appointments | Response | Mantiene typo historico. |
| `CreateAppoitmentDetailsDTO` | Request | Appointments | Request | Crear appointment. |
| `UpdateAppoitmentDetailsDTO` | Request | Appointments | Request | Actualizar appointment. |
| `AppoitmentDetailsDTO` | Response | Pedidos | Response | Listado de pedidos. |
| `RolesPermisoDTO` | Request | Roles/permisos | Request | Actualiza permisos por rol. |
| `RolesDTO`, `PermisosDTO` | Response | Roles/permisos | Response | Proyecciones. |

No se detectaron DTOs internos actuales dentro de los controllers inspeccionados.

## 9. Estado actual de services

| Service | Interface | Controller que lo usa | Repository que usa | Responsabilidad | Observacion |
|---|---|---|---|---|---|
| `VentasService` | `IVentasService` | `VentasController` | `IVentasRepository` | Crear/listar/actualizar/eliminar ventas, validar stock, calcular totales | Tiene reglas de negocio reales. |
| `StockService` | `IStockService` | `StockController` | `IStockRepository` | Validar y coordinar movimientos | No ajusta stock actual para movimientos manuales. |
| `ArticulosService` | `IArticulosService` | `ArticulosController` | `IArticulosRepository` | Validaciones de articulos y delete seguro | Bien ubicado. |
| `AutenticacionServices` | `IAutenticacionServices` | `AutenticacionController` | `IAutenticacionRepository` | Login/cambio clave | Wrapper simple; la logica JWT esta en repository. |
| `UsuarioServices` | `IUsuarioServices` | `UsuariosController` | `IUsuariosRepository` | Usuarios, roles y permisos | Mayormente delega. |
| `CustomerServices` | `ICustomerServices` | `CustomerController` | `ICustomerRepository` | CRUD clientes | Mayormente delega. |
| `ServiceServices` | `IServiceServices` | `ServiceController` | `IServicesRepository` | CRUD servicios y articulos | Expone entidades. |
| `AppoitmentServices` | `IAppointmentServices` | `AppointmentController` | `IAppointmentRepository` | CRUD appointments | Mayormente delega. |
| `PedidosServices` | `IPedidosServices` | `PedidosController` | `IPedidosRepository` | Listado pedidos | Wrapper simple. |
| `VehicleModalServices` | `IVehicleModal` | `PaginaBaseController` | ninguno | Lista modal vehiculos | Accede directo a DbContext. |
| `CustomerModalServices` | `ICustomerModal` | `PaginaBaseController` | ninguno | Lista modal clientes | Accede directo a DbContext. |
| `ServiciosModalServices` | `IServicioModal` | `PaginaBaseController` | ninguno | Lista modal servicios | Accede directo a DbContext. |

## 10. Estado actual de repositories

| Repository | Interface | Acceso a datos | Usa EF Core | Usa ADO.NET | Usa stored procedures | Observacion |
|---|---|---|---:|---:|---:|---|
| `VentasRepository` | `IVentasRepository` | Ventas, detalles, articulos, stock | Si | No | No | Maneja transaccion EF. |
| `StockRepository` | `IStockRepository` | StockMovimientos, Articulos | Si | No | No | Proyecta DTOs. |
| `ArticulosRepository` | `IArticulosRepository` | Articulos, Categorias, dependencias | Si | No | No | Valida dependencias via queries. |
| `AutenticacionRespository` | `IAutenticacionRepository` | Usuarios, Roles, SPs | Si | Si | Si | Genera JWT y valida clave. |
| `UsuarioRepository` | `IUsuariosRepository` | Usuarios, roles, permisos | Si | Si | Si | Usa `proc_InsertUsuario`. |
| `CustomerRepository` | `ICustomerRepository` | Customer | Si | No | No | Devuelve DTOs en listado. |
| `ServicesRepository` | `IServicesRepository` | Services, Articulos | Si | No | No | Devuelve entidades. |
| `AppointmentRepository` | `IAppointmentRepository` | Appointments, AppointmentServices | Si | No | No | Contiene logica de sincronizacion de servicios. |
| `PedidosRepository` | `IPedidosRepository` | Appointments y relacionados | Si | No | No | Proyeccion de pedidos. |

Repositories importantes existentes: ventas, stock y articulos ya existen.

Controllers que siguen saltando repository directamente: no se detecto acceso directo a `PracticaJWTcoreContext` dentro de controllers actuales. Algunos services modal si acceden directo al DbContext.

## 11. Manejo de errores actual

No existe middleware global de errores. En desarrollo se usa `UseDeveloperExceptionPage()`.

Modulos nuevos usan `ServiceResult<T>` para errores esperados:

- `ITEMS_REQUIRED`
- `INVALID_QUANTITY`
- `ARTICLE_NOT_FOUND`
- `STOCK_INSUFFICIENT`
- `MOVEMENT_NOT_FOUND`
- `ARTICLE_HAS_DEPENDENCIES`

Los controllers transforman esos resultados en `BadRequest`, `NotFound` o `NoContent`. Sin embargo, el cuerpo de error suele ser texto (`result.Message`), no un objeto uniforme `{ message, code }`.

`UsuariosController` captura excepciones, escribe en consola y devuelve `500` generico. `UsuarioRepository` tambien captura excepciones al crear usuario y devuelve `false`.

Pendiente: formato uniforme de error, logging con `ILogger`, middleware global para errores inesperados y evitar `FirstAsync` sin manejo en algunos repositorios.

## 12. Seguridad actual

JWT esta configurado en `Program.cs` con:

- `JwtBearerDefaults.AuthenticationScheme`.
- `IssuerSigningKey`.
- `ValidateIssuerSigningKey = true`.
- `ValidateIssuer = false`.
- `ValidateAudience = false`.

Swagger define Bearer y aplica security requirement. El pipeline usa:

```text
UseAuthentication()
UseAuthorization()
```

Claims generados:

- `ClaimTypes.Role`
- `ClaimTypes.NameIdentifier`

Uso real de atributos:

- `TestController` tiene `[AllowAnonymous]`.
- No se detecto uso sistematico de `[Authorize]` en controllers principales.

CORS:

- Politica `PermitirTodo`.
- Permite cualquier origen, metodo y header.

Riesgos pendientes: endpoints privados no estan protegidos de forma efectiva, permisos parecen no aplicarse como authorization policies, CORS abierto es comodo para desarrollo pero riesgoso en produccion.

## 13. Program.cs

`Program.cs` registra:

- CORS `PermitirTodo`.
- Configuracion desde `appsettings.json`.
- JWT Bearer.
- Swagger/OpenAPI con Bearer.
- Repositories con `AddScoped`.
- Services con `AddScoped`.
- `PracticaJWTcoreContext` con SQL Server.
- Controllers con opciones JSON camelCase.
- Swagger UI en desarrollo.
- Routing, CORS, authentication, authorization y `MapControllers`.

Estado:

- `AddControllers` no esta duplicado.
- `Program.cs` sigue concentrando toda la configuracion.
- No hay metodos de extension para DI/JWT/Swagger/CORS.
- Mantiene connection string y CORS historico.

## 14. Estado actual de arquitectura

Clasificacion honesta: **Intermedio con arquitectura por capas parcial**.

Motivos:

- Los modulos criticos ventas, stock y articulos ya siguen `Controller -> Service -> Repository -> DbContext`.
- Autenticacion y usuarios tambien pasan por service.
- Persisten modulos historicos que exponen entidades EF como contratos.
- Algunos services son pasamanos.
- El `DbContext` esta dentro de `Models`.
- No hay proyectos separados ni boundaries propios de Clean Architecture.

## 15. Problemas pendientes

### Alta prioridad

- Movimientos manuales de stock no actualizan `Articulos.StockActual`; si el negocio espera ajuste fisico, puede generar inconsistencia.
- Falta autorizacion efectiva con `[Authorize]` o policies en endpoints privados.
- Algunos repositorios usan `FirstAsync` sin manejo y pueden generar 500 ante ids inexistentes.

### Media prioridad

- No hay middleware global de errores ni formato uniforme.
- Algunos endpoints siguen usando entidades EF como request/response.
- Login y otros endpoints devuelven objetos anonimos.
- La logica JWT vive en repository, no en service.
- Services modal acceden directo a `DbContext`.

### Baja prioridad

- Typos historicos como `Appoitment`.
- `AutenticacionRespository` mantiene typo en nombre.
- Namespaces inconsistentes como `Dtos/Customer.cs` con `PracticaJWTcore.Entities`.
- `Program.cs` podria organizarse en metodos de extension si crece mas.

## 16. Mejoras recomendadas a futuro

### Corto plazo

| Mejora | Por que | Riesgo | Como verificar |
|---|---|---|---|
| Agregar `[Authorize]` gradualmente | Proteger endpoints privados | Puede romper frontend si no envia token | Probar login y pantallas Blazor con token. |
| Normalizar errores esperados | Mejor contrato frontend | Puede romper si frontend espera texto | Revisar consumidores antes. |
| Manejar ids inexistentes sin `FirstAsync` | Evitar 500 | Bajo | Tests de NotFound. |

### Mediano plazo

| Mejora | Por que | Riesgo | Como verificar |
|---|---|---|---|
| DTOs para `Service` y `Customer` update | Reducir exposicion EF | Medio por contratos frontend | Buscar consumo Blazor y probar CRUD. |
| Repositories para services modal | Consistencia de capas | Bajo | Build y pruebas de combos. |
| Revisar stock manual vs `StockActual` | Evitar datos inconsistentes | Alto si cambia comportamiento | Tests de stock y ventas. |

### Largo plazo

| Mejora | Por que | Riesgo | Como verificar |
|---|---|---|---|
| Separar configuracion de `Program.cs` en extensiones | Mantenibilidad | Bajo | Build y Swagger. |
| Mover `DbContext` a carpeta `Data` | Claridad semantica | Medio por namespaces | Refactor controlado. |
| Diseñar policies de permisos | Seguridad real backend | Medio/alto | Tests de autorizacion. |

## 17. Reglas para futuros cambios

- No cambiar endpoints sin revisar `BlazorApp1` y otros consumidores.
- No cambiar nombres de propiedades DTO sin revisar modelos frontend.
- No tocar entidades EF sin revisar `PracticaJWTcoreContext` y SQL Server.
- No mover logica de stock sin probar ventas, anulacion de ventas y movimientos.
- No modificar permisos sin revisar backend y UI.
- No cambiar `Program.cs` sin ejecutar `dotnet build`.
- No mezclar refactor con feature.
- No renombrar `Appoitment`, `AutenticacionRespository` ni nombres historicos sin plan de impacto.
- No cambiar stored procedures desde C#.
- Mantener el flujo `Controller -> Service -> Repository`.
- Preferir DTOs para nuevos endpoints.
- No exponer `ex.Message` al cliente en produccion.
- Documentar cualquier cambio de contrato.

## 18. Glosario actualizado

- Controller: clase que define rutas HTTP. Ejemplo: `VentasController`.
- Service: clase que contiene reglas de negocio. Ejemplo: `VentasService` valida stock y calcula totales.
- Repository: clase que accede a datos. Ejemplo: `VentasRepository` usa EF Core.
- Entity: clase persistida/mapeada a SQL. Ejemplo: `Venta`, `Articulos`.
- DTO: contrato de entrada/salida. Ejemplo: `CreateVentaDto`.
- Request: datos que envia el frontend. Ejemplo: `ArticuloRequestDto`.
- Response: datos que recibe el frontend. Ejemplo: `VentaResponseDto`.
- DbContext: puerta de acceso EF Core a SQL Server. Ejemplo: `PracticaJWTcoreContext`.
- Stored Procedure: procedimiento SQL invocado desde C#. Ejemplo: `proc_ComparePass`.
- Dependency Injection: registro y resolucion de dependencias con `AddScoped`.
- Regla de negocio: decision del dominio. Ejemplo: no vender si no hay stock.
- Contrato API: ruta, metodo, request, response y status codes.
- Contrato SQL: tablas, columnas, procedimientos y relaciones usadas por el backend.
- Refactor seguro: cambio interno sin romper endpoints, frontend ni base de datos.
- Acoplamiento: dependencia fuerte entre partes. Ejemplo: frontend consumiendo entidad EF directamente.
- Cohesion: que una clase tenga una responsabilidad clara. Ejemplo: `StockService` concentrando validaciones de stock.

## 19. Resumen final

Respecto al diagnostico anterior, el backend mejoro especialmente en ventas, stock y articulos. Esos modulos ahora tienen controllers livianos, services con reglas de negocio, repositories para EF Core y DTOs propios.

Tambien mejoro la ruta de autenticacion y usuarios porque los controllers ya llaman a services en lugar de repositories directamente.

Sigue pendiente consolidar seguridad con `[Authorize]`, mejorar el manejo uniforme de errores, reducir entidades EF expuestas en servicios/clientes y revisar la consistencia entre movimientos manuales de stock y `Articulos.StockActual`.

Modulos mas ordenados: ventas, stock, articulos.

Modulos que siguen siendo riesgosos: seguridad/autorizacion, stock manual, servicios/clientes por uso de entidades, appointments por typos historicos y manejo de inexistentes.

Antes del proximo refactor deberia revisarse el frontend consumidor, ejecutar build/tests, validar Swagger y confirmar reglas reales de negocio para stock y permisos.

