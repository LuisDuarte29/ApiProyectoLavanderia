# Arquitectura Backend

## 1. Resumen general

El backend principal del repositorio es `PracticaJWTcore`. Es una API ASP.NET Core sobre `.NET 8`, configurada desde `Program.cs`, con controladores HTTP en `Controllers/`, clases de servicio en `Services/`, repositorios en `Repositorios/`, DTOs en `Dtos/` y entidades/modelos en `Models/`.

La API usa SQL Server como base de datos. El acceso a datos es mixto:

- Entity Framework Core mediante `PracticaJWTcoreContext`.
- ADO.NET mediante `SqlConnection` y `SqlCommand` para algunos procedimientos almacenados.

La aplicacion registra JWT Bearer, Swagger, CORS abierto, `DbContext` y dependencias con `AddScoped`. Tambien existe un frontend Blazor WebAssembly en `BlazorApp1`, que consume el backend usando `HttpClient` con base URL `https://localhost:7184/`.

La arquitectura esta parcialmente organizada por capas, pero no de forma uniforme. Algunos flujos siguen `Controller -> Service -> Repository -> DbContext/SQL Server`, mientras otros controllers acceden directamente al `DbContext` y concentran consultas, validaciones y reglas de negocio.

## 2. Tipo de arquitectura detectada

La arquitectura detectada es una combinacion de:

- API simple ASP.NET Core.
- Arquitectura por capas parcial.
- MVC/API con services y repositories.
- Monolito backend.

No es Clean Architecture completa. Hay separacion inicial entre controllers, services y repositories, pero no existe un proyecto separado de dominio, aplicacion o infraestructura. Tampoco hay una capa `Data/` independiente; el `DbContext` vive dentro de `Models/`.

Evidencias:

- `Program.cs` registra controllers, Swagger, JWT, CORS, EF Core y todas las dependencias.
- `AppointmentController`, `CustomerController`, `ServiceController` y `PedidosController` delegan principalmente en services.
- `AppointmentRepository`, `CustomerRepository`, `ServicesRepository`, `PedidosRepository`, `UsuarioRepository` y `AutenticacionRespository` concentran parte del acceso a datos.
- `VentasController`, `StockController` y `ArticulosController` inyectan directamente `PracticaJWTcoreContext` y ejecutan reglas, validaciones, consultas y persistencia dentro del controller.

La clasificacion mas honesta es: API monolitica por capas parcial.

## 3. Mapa de carpetas y responsabilidades

### Controllers

Ubicacion: `PracticaJWTcore/Controllers/`.

Contiene endpoints HTTP. Algunos controllers estan livianos y delegan:

- `AppointmentController`
- `CustomerController`
- `ServiceController`
- `PedidosController`
- `PaginaBaseController`

Otros contienen mas logica de negocio y acceso directo a datos:

- `VentasController`
- `StockController`
- `ArticulosController`
- `UsuariosController`
- `AutenticacionController`

### Services

Ubicacion: `PracticaJWTcore/Services/`.

Contiene interfaces e implementaciones para varias areas: appointments, customers, servicios, autenticacion, pedidos y usuarios. En muchos casos los services actuan como pasamanos hacia repositorios.

Ejemplos:

- `ServiceServices` llama a `IServicesRepository`.
- `CustomerServices` llama a `ICustomerRepository`.
- `AppoitmentServices` llama a `IAppointmentRepository`.
- `PedidosServices` llama a `IPedidosRepository`.

Tambien hay services que acceden directamente al `DbContext` para datos de modales:

- `VehicleModalServices`
- `CustomerModalServices`
- `ServiciosModalServices`

### Repositories

Ubicacion: `PracticaJWTcore/Repositorios/`.

Existe capa de repositorios, pero no cubre todo el backend. Contiene acceso a EF Core y ADO.NET:

- EF Core: `AppointmentRepository`, `CustomerRepository`, `ServicesRepository`, `PedidosRepository`, parte de `UsuarioRepository`.
- ADO.NET/procedimientos: `AutenticacionRespository`, parte de `UsuarioRepository`.

No hay repositorios para ventas, stock ni articulos. Esas areas trabajan directamente desde controllers.

### Models

Ubicacion: `PracticaJWTcore/Models/`.

Contiene entidades de base de datos, modelos auxiliares y el `DbContext`.

Ejemplos de entidades:

- `Appointment`
- `AppointmentService`
- `Service`
- `Vehicle`
- `Usuarios`
- `Roles`
- `Permisos`
- `Articulos`
- `Venta`
- `VentaDetalle`
- `StockMovimiento`
- `Categoria`
- `DocumentoElectronico`

Tambien contiene modelos mas cercanos a proyecciones o combos/modal:

- `CustomerModal`
- `VehicleModal`
- `ServiciosModal`
- `RoleModal`
- `PermisosModal`

### DTOs

Ubicacion: `PracticaJWTcore/Dtos/`.

Contiene objetos para entrada/salida de API, por ejemplo:

- `CreateCustomerDto`
- `CustomerDto`
- `CreateAppoitmentDetailsDTO`
- `UpdateAppoitmentDetailsDTO`
- `AppoitmentDTO`
- `AppoitmentDetailsDTO`
- `ServiceDto`
- `CreateUsuariosDTO`
- `UsuarioDTO`
- `TokenRolDTO`
- `RolesPermisoDTO`

Hay un punto importante: `Dtos/Customer.cs` declara `namespace PracticaJWTcore.Entities`, no `PracticaJWTcore.Dtos`, y funciona como entidad `Customer`. Esto mezcla ubicacion fisica con responsabilidad semantica.

### Data

No existe carpeta `Data/`.

El `DbContext` esta en `Models/PracticaJWTcoreContext.cs`. En un proyecto mas ordenado convendria evaluar mover el contexto a `Data/` o `Infrastructure/Data/`, pero no conviene hacerlo sin una etapa de refactor controlada porque impactaria namespaces y referencias.

### Helpers

No existe carpeta `Helpers/`.

No es obligatorio crearla. Solo convendria si aparecen utilidades repetidas y estables, por ejemplo helpers de respuesta, mapping o claims. Crear helpers sin necesidad podria sumar abstraccion innecesaria.

### Middlewares

No existe carpeta `Middlewares/`.

Actualmente no se detecta middleware propio de errores, logs o auditoria. El proyecto usa `UseDeveloperExceptionPage()` en desarrollo y manejo puntual de errores en algunos controllers.

### Configuracion

Configuracion principal:

- `PracticaJWTcore/Program.cs`
- `PracticaJWTcore/appsettings.json`
- `PracticaJWTcore/appsettings.Development.json`
- `PracticaJWTcore/Properties/launchSettings.json`

`Program.cs` concentra CORS, JWT, Swagger, DI, logging, EF Core y pipeline HTTP.

## 4. Flujo completo de un endpoint

Endpoint elegido: crear una venta.

### Ruta HTTP

`POST /api/Ventas`

Se define en `VentasController` con:

- `[Route("api/[controller]")]`
- `[HttpPost]`
- metodo `CreateVenta([FromBody] CreateVentaDto dto)`

### Controller

Archivo: `PracticaJWTcore/Controllers/VentasController.cs`.

El controller inyecta directamente `PracticaJWTcoreContext`:

```csharp
private readonly PracticaJWTcoreContext _db;
```

### Request recibido

El request usa un DTO declarado dentro del mismo controller:

```csharp
public class CreateVentaDto
{
    public long? IdCliente { get; set; }
    public int? IdUsuario { get; set; }
    public string? MetodoPago { get; set; }
    public string? Estado { get; set; }
    public List<VentaItemDto> Items { get; set; } = new List<VentaItemDto>();
}
```

Cada item contiene:

```csharp
public int ArticuloId { get; set; }
public decimal Cantidad { get; set; }
```

### DTO usado

`CreateVentaDto` y `VentaItemDto` existen como clases internas dentro de `VentasController`, no dentro de `Dtos/`.

Esto funciona, pero reduce reutilizacion, documentacion y consistencia con otros endpoints.

### Service llamado

No se llama ningun service.

El controller contiene directamente:

- validacion de items;
- consulta de articulos;
- validacion de stock;
- inicio de transaccion;
- creacion de venta;
- creacion de detalles;
- descuento de stock;
- creacion de movimientos de stock;
- calculo de subtotal, IVA y total;
- respuesta HTTP.

### Repository o acceso a datos

No usa repository.

Usa EF Core directamente con `_db`:

- `_db.Articulos`
- `_db.Ventas`
- `_db.VentaDetalles`
- `_db.StockMovimientos`
- `_db.Database.BeginTransactionAsync()`
- `_db.SaveChangesAsync()`

### Tabla o procedimiento SQL involucrado

Por el mapeo en `PracticaJWTcoreContext`, se identifican estas tablas:

- `Ventas`
- `VentaDetalles`
- `Articulos`
- `StockMovimientos`

No se identifica procedimiento almacenado para este flujo.

### Response devuelto al frontend

Devuelve `201 Created` con ruta `api/ventas/{venta.IdVenta}` y un objeto anonimo con datos de la venta y detalles:

- `IdVenta`
- `FechaVenta`
- `IdCliente`
- `Total`
- `Detalles`

## 5. Controllers

Los controllers estan usados de forma desigual.

Controllers mas livianos:

- `AppointmentController`: delega en `IAppointmentServices`.
- `CustomerController`: delega en `ICustomerServices`.
- `ServiceController`: delega en `IServiceServices`.
- `PedidosController`: delega en `IPedidosServices`.
- `PaginaBaseController`: delega en servicios para listas de vehicle/customer/servicios.

Controllers con demasiada logica:

- `VentasController`: contiene reglas de venta, stock, calculo de IVA, transaccion, persistencia y respuesta.
- `StockController`: valida y persiste movimientos de stock directamente.
- `ArticulosController`: consulta categorias, valida y persiste articulos directamente.
- `UsuariosController`: llama directo a repository y maneja errores en controller.
- `AutenticacionController`: llama directo a repository aunque existe `IAutenticacionServices`.

Problemas concretos:

- Hay controllers que acceden directo a SQL/EF Core.
- Hay DTOs internos dentro de controllers.
- Algunas respuestas son objetos anonimos, otras entidades, otras DTOs.
- Algunos `CreatedResult` usan URLs hardcodeadas con `localhost:7184`.
- La responsabilidad de negocio no esta concentrada en services.

## 6. Services

Los services existen y dan una base de separacion, pero su responsabilidad actual es limitada.

En muchos casos son wrappers directos:

- `CustomerServices.CreateCustomer` llama a `CustomerRepository.CreateCustomer`.
- `ServiceServices.CreateService` llama a `ServicesRepository.CreateService`.
- `PedidosServices.GetPedidos` llama a `PedidosRepository.GetPedidos`.
- `UsuarioServices` delega casi todo en `IUsuariosRepository`.

Esto no esta mal para empezar, pero significa que las reglas de negocio no estan claramente ubicadas en services. Algunas reglas estan en:

- controllers;
- repositories;
- stored procedures;
- posiblemente frontend;
- modelos/EF mapping.

Services con acceso directo a `DbContext`:

- `CustomerModalServices`
- `VehicleModalServices`
- `ServiciosModalServices`

Eso mezcla el concepto de service con acceso a datos. Puede ser aceptable en proyectos chicos, pero para crecer convendria definir una regla clara: o los services contienen negocio y usan repositories, o se elimina la capa repository para flujos simples. La situacion intermedia actual es la mas dificil de mantener.

No se detectan services especificos para:

- Ventas.
- Stock.
- Articulos.
- Documentos electronicos.
- Reglas fiscales/reportes.

## 7. Repositories o acceso a datos

El backend habla con SQL Server de tres formas:

1. EF Core mediante `PracticaJWTcoreContext`.
2. ADO.NET con `System.Data.SqlClient`.
3. Procedimientos almacenados.

### EF Core

EF Core se registra en `Program.cs`:

```csharp
builder.Services.AddDbContext<PracticaJWTcoreContext>(sqlBuilder =>
{
    sqlBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
```

El `DbContext` declara `DbSet` para entidades como:

- `Appointments`
- `Customer`
- `CustomerEntity`
- `Services`
- `Usuarios`
- `Vehicles`
- `Roles`
- `Permisos`
- `RolesPermisos`
- `AppointmentServices`
- `Articulos`
- `Ventas`
- `VentaDetalles`
- `StockMovimientos`
- `Categorias`
- `DocumentoElectronicos`
- `ComponentsForm`

### ADO.NET y procedimientos almacenados

Se detectan procedimientos almacenados:

- `proc_ComparePass`
- `proc_CambioClave`
- `proc_InsertUsuario`

Usados en:

- `AutenticacionRespository.Login`
- `AutenticacionRespository.CambioClave`
- `UsuarioRepository.CreateUsuarios`

### Centralizacion

El acceso a datos no esta totalmente centralizado.

Hay acceso a datos en:

- repositories;
- services de modales;
- controllers de ventas, stock y articulos.

### SQL repetido

No se ve SQL inline textual repetido grande, pero si hay consultas LINQ repetidas o acopladas, por ejemplo:

- buscar nombre de articulo desde ventas/stock;
- buscar cliente/usuario desde ventas;
- proyectar datos relacionados con subconsultas dentro de controllers.

### Riesgo de acoplamiento fuerte

El acoplamiento con SQL Server es fuerte porque:

- EF mapping depende de nombres de tablas/columnas.
- procedimientos almacenados tienen nombres y parametros esperados.
- algunos controllers usan directamente tablas via `DbContext`.
- no hay una capa unica de acceso a datos para todos los modulos.

## 8. Models, Entities y DTOs

### Diferencias conceptuales

Entity:

Representa una tabla o concepto persistido. Ejemplo: `Venta`, `VentaDetalle`, `Articulos`, `Appointment`, `Usuarios`.

Model:

En este proyecto se usa como contenedor general. Hay entities reales, modelos de UI/modal y el `DbContext` dentro de `Models/`.

DTO:

Objeto para transportar datos entre backend y frontend o entre capas. Ejemplo: `CreateCustomerDto`, `AppoitmentDTO`, `TokenRolDTO`.

Request:

DTO de entrada recibido por un endpoint. Ejemplo: `CreateAppoitmentDetailsDTO`, `CreateUsuariosDTO`, `CreateVentaDto`.

Response:

DTO u objeto que la API devuelve al frontend. Ejemplo: `CustomerDto`, `AppoitmentDetailsDTO`, `TokenRolDTO`, o los objetos anonimos de `VentasController`.

### Uso en el proyecto

Hay uso correcto de DTOs en varios flujos:

- Crear customer usa `CreateCustomerDto`.
- Listar customers usa `CustomerDto`.
- Crear appointment usa `CreateAppoitmentDetailsDTO`.
- Pedidos usa `AppoitmentDetailsDTO` y `ServiceDto`.
- Login devuelve `TokenRolDTO` envuelto como `tokenRol`.

Pero tambien hay problemas:

- `ServiceController` recibe y devuelve `Service`, que parece entidad EF.
- `StockController` recibe y devuelve `StockMovimiento`, entidad EF.
- `ArticulosController` recibe `Articulos`, entidad EF.
- `VentasController` usa DTOs internos y objetos anonimos.
- `Dtos/Customer.cs` esta fisicamente en `Dtos`, pero su namespace es `PracticaJWTcore.Entities` y se usa como entidad relacionada con EF.
- Hay nombres inconsistentes: `Appoitment` esta mal escrito en varias clases y archivos; `AutenticacionRespository` tambien tiene typo en `Respository`.
- Hay mezcla de ingles y espanol: `Customer`, `Service`, `Usuarios`, `Ventas`, `Articulos`, `Permisos`.

### Exposicion de entidades

Si, el backend expone entidades directamente en algunos endpoints:

- `ServiceController` expone `Service`.
- `ArticulosController` recibe `Articulos`.
- `StockController` recibe `StockMovimiento`.
- `CustomerController.UpdateCustomer` recibe `Customer`.

Esto puede ser riesgoso porque cambios en la entidad o en el modelo de base de datos pueden romper el contrato del frontend.

## 9. Reglas de negocio

### Ventas

Las reglas mas importantes estan en `VentasController`:

- Validar que existan items.
- Validar existencia de articulos.
- Validar stock suficiente.
- Crear venta.
- Crear detalles.
- Descontar stock.
- Registrar movimientos.
- Calcular IVA fijo de `10.0m`.
- Restaurar stock al eliminar venta.

Problema: estas reglas estan en controller, no en service.

### Stock

Parte de stock esta en `VentasController` y parte en `StockController`.

- `VentasController` descuenta o restaura stock por ventas.
- `StockController` permite crear, actualizar y borrar movimientos.

Riesgo: se puede generar inconsistencia si se crean movimientos manuales sin actualizar `Articulos.StockActual`, porque `StockController.CreateMovimiento` registra el movimiento pero no actualiza el articulo.

### Productos/articulos

Reglas en `ArticulosController`:

- `NombreArticulo` requerido.
- Categoria debe existir.
- No se puede eliminar articulo con ventas o movimientos asociados.

### Usuarios

Reglas distribuidas entre:

- `UsuariosController`
- `UsuarioRepository`
- stored procedure `proc_InsertUsuario`
- tablas `Usuarios`, `Roles`, `Permisos`, `RolesPermisos`, `ComponentsForm`

### Permisos

La asignacion de permisos esta en `UsuarioRepository.PermisosRoleCreate`, comparando permisos existentes contra permisos recibidos y agregando/removiendo relaciones.

No se observa que esos permisos se apliquen como autorizacion real en endpoints con `[Authorize(Roles = ...)]` o politicas.

### Documentos

Existen modelo y tabla mapeada para `DocumentoElectronico`/`DocumentosElectronicos`, pero no se detecta controller, service ni repository para documentos electronicos.

### Reportes

No se detectan controllers, services o reportes RDLC/SSRS en el backend inspeccionado.

## 10. Manejo de errores

El manejo de errores es parcial.

Se observa:

- `UseDeveloperExceptionPage()` en desarrollo.
- `builder.Logging.AddConsole()`.
- `try/catch` en `UsuariosController.CreateUsuario`.
- `try/catch` con rollback y `throw` en `VentasController`.
- respuestas manuales `BadRequest`, `NotFound`, `Ok`, `NoContent`, `Created`.

Problemas:

- No hay middleware global de excepciones.
- No hay formato unico de error para el frontend.
- Algunos errores devuelven texto plano.
- `UsuariosController` devuelve `ex.Message` al cliente, lo cual puede filtrar informacion interna.
- Algunos repositories usan `FirstAsync`; si no existe el registro, lanzan excepcion en vez de devolver `null` o un resultado controlado.
- `AutenticacionController.Login` devuelve `200` si `response != null`, pero `GenerateToken` devuelve un objeto vacio cuando falla, no `null`.

Mejora profesional recomendada:

- Crear un middleware global de errores o usar `UseExceptionHandler`.
- Definir un contrato de error uniforme, por ejemplo `{ message, code, details }`.
- Loguear excepciones con `ILogger`.
- Evitar exponer mensajes internos al frontend.
- Convertir errores esperados en respuestas controladas.

## 11. Validaciones

Las validaciones estan repartidas.

### Frontend

En `Login.razor` se valida que correo y clave no esten vacios antes de llamar a la API.

### Controller

Hay validaciones en:

- `VentasController`: items requeridos, articulo existente, stock suficiente, cliente/usuario existente.
- `ArticulosController`: nombre requerido, categoria existente, restricciones antes de borrar.
- `StockController`: movimiento requerido, articulo existente, cantidad mayor que cero.
- `UsuariosController`: maneja resultado booleano de creacion.

### Service

No se observa mucha validacion en services. La mayoria delega directo.

### Base de datos

El `DbContext` configura tipos, longitudes y relaciones. Tambien los stored procedures probablemente validan o procesan reglas, pero no se inspecciono SQL real.

### Validaciones que deberian estar si o si en backend

- Requeridos de requests.
- Existencia de IDs relacionados.
- Stock suficiente.
- Cantidades positivas.
- Precios no negativos.
- Permisos para operaciones sensibles.
- Estado permitido de venta, movimiento o documento.
- Consistencia de usuario/rol/permiso.
- Validacion de credenciales sin filtrar detalles.
- Validacion de longitud y formato de datos antes de persistir.

## 12. Seguridad

Existe configuracion de autenticacion JWT:

- paquete `Microsoft.AspNetCore.Authentication.JwtBearer`.
- `AddAuthentication(JwtBearerDefaults.AuthenticationScheme)`.
- `UseAuthentication()`.
- `UseAuthorization()`.
- Swagger configurado para Bearer token.
- token generado en `AutenticacionRespository.GenerateToken`.
- claims: `ClaimTypes.Role` y `ClaimTypes.NameIdentifier`.

Pero la proteccion efectiva de endpoints es limitada:

- Se detecta `[AllowAnonymous]` en `TestController`.
- No se detectan `[Authorize]` aplicados a controllers o acciones principales.
- El frontend envia header Bearer en algunas paginas, por ejemplo `Services.razor` y `ServicesCreacion.razor`, pero si el backend no exige `[Authorize]`, el token no protege realmente esos endpoints.
- Existen roles y permisos en base de datos, pero no se observa autorizacion por rol o permiso en endpoints.

Riesgos:

- Endpoints sensibles podrian estar accesibles sin token.
- Las reglas de permisos parecen usarse para UI o asignacion, no como barrera backend.
- CORS permite cualquier origen, metodo y header con policy `PermitirTodo`.
- La clave JWT esta en `appsettings.json` como texto simple y es corta para un entorno profesional.

Que revisar:

- Agregar `[Authorize]` donde corresponda.
- Definir endpoints publicos explicitamente.
- Revisar si roles/permisos deben aplicarse en backend.
- Mover secretos fuera de `appsettings.json` para produccion.
- Ajustar CORS a origenes concretos.

## 13. Dependencias e inyeccion de dependencias

Las dependencias se registran en `Program.cs` con `AddScoped`.

Repositorios registrados:

- `ICustomerRepository -> CustomerRepository`
- `IServicesRepository -> ServicesRepository`
- `IAppointmentRepository -> AppointmentRepository`
- `IAutenticacionRepository -> AutenticacionRespository`
- `IUsuariosRepository -> UsuarioRepository`
- `IPedidosRepository -> PedidosRepository`

Services registrados:

- `ICustomerServices -> CustomerServices`
- `IServiceServices -> ServiceServices`
- `IAppointmentServices -> AppoitmentServices`
- `IAutenticacionServices -> AutenticacionServices`
- `IPedidosServices -> PedidosServices`
- `IVehicleModal -> VehicleModalServices`
- `ICustomerModal -> CustomerModalServices`
- `IServicioModal -> ServiciosModalServices`
- `IUsuarioServices -> UsuarioServices`

Observaciones:

- Hay services registrados que no siempre se usan. Por ejemplo `AutenticacionController` inyecta repository, no service.
- `UsuariosController` tambien inyecta repository, no service, aunque existe `IUsuarioServices`.
- `Program.cs` esta creciendo como archivo central de configuracion.
- Se llama `builder.Services.AddControllers()` dos veces; una vez simple y otra con `AddJsonOptions`.
- La variable `connectionString` se obtiene pero no se usa directamente.

Para un proyecto mas grande convendria separar la configuracion en metodos de extension, por ejemplo:

- `AddApplicationServices`
- `AddInfrastructure`
- `AddJwtAuthentication`
- `AddSwaggerDocumentation`

Eso seria una mejora estructural, no necesaria para funcionamiento inmediato.

## 14. Contrato con el frontend

El frontend principal observado es `BlazorApp1`.

Configuracion:

- `BlazorApp1/Program.cs` registra `HttpClient` con base URL `https://localhost:7184/`.

Endpoints consumidos:

- `POST api/Autenticacion` desde `Login.razor`.
- `GET api/service` desde `Services.razor`.
- `DELETE api/service/{id}` desde `Services.razor`.
- `GET api/service/{idServicioLong}` desde `ServicesCreacion.razor`.
- `POST api/service` desde `ServicesCreacion.razor`.
- `PUT api/service/` desde `ServicesCreacion.razor`.
- `GET api/pedidos` desde `Pedidos.razor`.
- `GET api/test` desde `Test.razor`.

Riesgos de contrato:

- El frontend espera nombres concretos de propiedades. Cambiar `ServiceId`, `ServiceName`, `Price`, `AppointmentId`, `Services`, `Estado`, etc. rompe pantallas.
- `Program.cs` configura JSON camelCase. Eso puede cambiar nombres serializados: por ejemplo `TokenRol` puede salir como `tokenRol`.
- El login backend devuelve `{ tokenRol = response }`, pero `BlazorApp1/Modelos/TokenResponse.cs` espera `token`. Hay posible inconsistencia: el frontend lee `TokenResponse token = await response.Content.ReadFromJsonAsync<TokenResponse>();` y luego guarda `token.token`.
- Algunos endpoints devuelven entidades completas y otros objetos anonimos; esto vuelve mas fragil el contrato.
- Si se renombran propiedades en entities EF, puede romper frontend aunque la base de datos no cambie.

Recomendacion:

- Definir DTOs de request/response por endpoint importante.
- Evitar devolver entidades EF directamente.
- Documentar contratos usados por Blazor.
- Crear tests o ejemplos de payload para endpoints criticos.

## 15. Contrato con SQL Server

La conexion se define en `PracticaJWTcore/appsettings.json`:

```json
"DefaultConnection": "Server=LAPTOP-JUFB9LAM\\MSSQLSERVER2022;Database=ApiProyecto;Integrated Security=True;TrustServerCertificate=True;"
```

### Tablas usadas o mapeadas

Desde `PracticaJWTcoreContext`:

- `Appointment`
- `Customer`
- `Services`
- `Usuarios`
- `Vehicles`
- `Roles`
- `UsuariosRoles`
- `Permisos`
- `RolesPermisos`
- `AppointmentServices`
- `Articulos`
- `Ventas`
- `VentaDetalles`
- `StockMovimientos`
- `Categorias`
- `DocumentosElectronicos`
- `ComponentsForm`

Algunas tablas tienen nombre configurado explicitamente:

- `Usuarios`
- `Roles`
- `ComponentsForm`
- `Permisos`
- `Articulos`
- `Ventas`
- `VentaDetalles`
- `StockMovimientos`
- `Categorias`
- `DocumentosElectronicos`

### Procedimientos usados

- `proc_ComparePass`
- `proc_CambioClave`
- `proc_InsertUsuario`

### Columnas importantes identificadas

Ventas:

- `IdVenta`
- `FechaVenta`
- `IdCliente`
- `IdUsuario`
- `SubTotal`
- `IvaTotal`
- `Total`
- `MetodoPago`
- `Estado`

VentaDetalles:

- `IdVentaDetalle`
- `IdVenta`
- `IdArticulo`
- `Cantidad`
- `PrecioUnitario`
- `PorcentajeIva`
- `SubTotal`

Articulos:

- `IdArticulo`
- `NombreArticulo`
- `Precio`
- `Codigo`
- `CodigoBarra`
- `Descripcion`
- `PrecioCosto`
- `PrecioVenta`
- `StockActual`
- `StockMinimo`
- `Activo`
- `IdCategoria`

StockMovimientos:

- `IdStockMovimiento`
- `FechaMovimiento`
- `IdArticulo`
- `TipoMovimiento`
- `Cantidad`
- `StockAnterior`
- `StockNuevo`
- `Referencia`
- `Observacion`

Usuarios y permisos:

- `IdUsuario`
- `correo`
- `clave`
- `RoleId`
- `CustomerID`
- `RoleName`
- `PermisoId`
- `PermisoNombre`
- `ComponentsId`
- `ComponentsName`

### Riesgos

- Si cambia un nombre de columna o tabla, se rompe EF mapping.
- Si cambia un procedimiento o parametro, se rompe ADO.NET.
- Si cambia una entidad expuesta al frontend, tambien se puede romper el contrato UI.
- Al mezclar EF y stored procedures, hay reglas que pueden quedar duplicadas o invisibles desde C#.

## 16. Estado actual de la arquitectura

Clasificacion: intermedio con arquitectura por capas parcial.

No es basico porque:

- Tiene controllers separados.
- Tiene services e interfaces.
- Tiene repositories e interfaces.
- Tiene EF Core.
- Tiene JWT configurado.
- Tiene Swagger.
- Tiene DTOs.

No es profesional/escalable todavia porque:

- Las capas no se aplican de forma consistente.
- Hay reglas de negocio en controllers.
- Hay acceso directo a `DbContext` desde controllers y algunos services.
- Hay mezcla de entidades, DTOs, modelos de modal y contexto en carpetas no totalmente claras.
- No hay manejo global de errores.
- No hay autorizacion aplicada de forma sistematica.
- No hay tests visibles para los flujos principales de negocio.
- No hay separacion por modulos o features.

## 17. Problemas o riesgos detectados

- `VentasController` tiene demasiada logica de negocio.
- `StockController` accede directo a `DbContext` y permite registrar movimientos sin actualizar stock del articulo.
- `ArticulosController` usa entidades como request/response.
- `AutenticacionController` y `UsuariosController` llaman directo a repositories aunque existen services.
- Los services son en gran parte wrappers sin reglas claras.
- Hay acceso a datos disperso entre controllers, services y repositories.
- No hay repositories para ventas, stock ni articulos.
- Hay DTOs internos dentro de controllers.
- Algunas entidades se exponen directamente al frontend.
- El manejo de errores no es uniforme.
- Algunos errores pueden exponer mensajes internos.
- Hay poco uso efectivo de `[Authorize]`.
- CORS permite cualquier origen.
- La clave JWT esta en configuracion simple.
- Hay nombres inconsistentes y typos: `Appoitment`, `AutenticacionRespository`.
- `Dtos/Customer.cs` funciona como entity pero esta en carpeta DTOs.
- `Program.cs` concentra demasiada configuracion.
- Hay objetos anonimos como responses, dificiles de versionar.
- Hay dependencia fuerte de SQL Server, tablas y procedimientos.
- No se observa testing de flujos criticos como ventas/stock/autenticacion.

## 18. Mejoras recomendadas

### Mejoras rapidas

1. Documentar endpoints y contratos actuales.

Que cambiar: crear documentacion de rutas, requests y responses.

Por que: evita romper frontend por cambios de nombres.

Riesgo: bajo, solo documentacion.

Como verificar: comparar con controllers y consumo Blazor.

2. Agregar DTOs explicitos para ventas, stock y articulos.

Que cambiar: mover `CreateVentaDto` y `UpdateVentaDto` a `Dtos/`, crear requests/responses para articulos y stock.

Por que: separa contrato API de entidades EF.

Riesgo: medio si se cambia serializacion o nombres.

Como verificar: probar endpoints existentes y frontend.

3. Uniformar respuestas de error simples.

Que cambiar: usar un formato comun para `BadRequest`, `NotFound` y errores esperados.

Por que: el frontend puede mostrar mensajes consistentes.

Riesgo: medio si el frontend espera texto plano.

Como verificar: revisar pantallas que muestran errores.

4. Aplicar `[Authorize]` gradualmente.

Que cambiar: proteger endpoints no publicos.

Por que: JWT ya esta configurado pero poco aplicado.

Riesgo: medio, puede bloquear pantallas si el frontend no envia token.

Como verificar: login, servicios, pedidos y Swagger con token.

### Mejoras medianas

1. Crear services para ventas, stock y articulos.

Que cambiar: mover reglas de `VentasController`, `StockController` y `ArticulosController` a services.

Por que: controllers quedarian como entrada/salida HTTP.

Riesgo: medio-alto por reglas de stock y transacciones.

Como verificar: crear venta, listar venta, eliminar venta y revisar stock/movimientos.

2. Crear repositories para ventas, stock y articulos.

Que cambiar: centralizar queries EF de esos modulos.

Por que: reduce acceso directo a `DbContext` desde controllers.

Riesgo: medio por cambios de comportamiento en consultas.

Como verificar: comparar payloads antes/despues.

3. Introducir middleware global de errores.

Que cambiar: agregar middleware o `UseExceptionHandler`.

Por que: respuestas uniformes y logs centralizados.

Riesgo: medio si cambia el formato que espera el frontend.

Como verificar: probar errores esperados e inesperados.

4. Separar configuracion de `Program.cs`.

Que cambiar: extension methods para DI, JWT, Swagger y CORS.

Por que: mejora mantenibilidad.

Riesgo: bajo-medio si se respeta comportamiento.

Como verificar: build, Swagger, login, endpoints.

### Mejoras grandes

1. Evolucionar a arquitectura por features.

Que cambiar: agrupar por modulos como `Features/Ventas`, `Features/Stock`, `Features/Articulos`, `Features/Usuarios`.

Por que: el dominio esta creciendo y ventas/stock ya tienen reglas propias.

Riesgo: alto si se mueven muchos archivos de golpe.

Como verificar: por etapas, con tests y comparacion de endpoints.

2. Separar proyectos por capas.

Que cambiar: dividir en proyectos tipo `Api`, `Application`, `Domain`, `Infrastructure`.

Por que: acerca a Clean Architecture.

Riesgo: alto; requiere plan y pruebas.

Como verificar: build completo, tests, Swagger y pruebas manuales.

3. Testing de negocio.

Que cambiar: agregar tests para ventas, stock, autenticacion, permisos y repositories.

Por que: permite refactor seguro.

Riesgo: medio por preparacion de base de datos o mocks.

Como verificar: ejecutar suite automatizada.

## 19. Arquitectura recomendada ideal

Para este proyecto conviene evolucionar primero hacia una arquitectura por capas consistente, antes de saltar a Clean Architecture completa.

Estructura recomendada gradual:

```text
Controllers/
Services/
Repositories/
Dtos/
Models/
Data/
Helpers/
Middlewares/
```

Responsabilidades:

- `Controllers/`: solo HTTP, status codes, binding de requests y llamada a services.
- `Services/`: reglas de negocio y coordinacion de operaciones.
- `Repositories/`: acceso a EF Core, ADO.NET y stored procedures.
- `Dtos/`: contratos request/response.
- `Models/`: entidades de dominio/base de datos.
- `Data/`: `PracticaJWTcoreContext` y configuraciones EF.
- `Helpers/`: utilidades compartidas solo si realmente se repiten.
- `Middlewares/`: errores globales, logging o auditoria.

Cuando el proyecto crezca mas, podria convenir estructura por modulos:

```text
Features/
  Ventas/
    VentasController.cs
    VentasService.cs
    VentasRepository.cs
    Dtos/
  Stock/
  Articulos/
  Usuarios/
  Autenticacion/
```

Para el estado actual, recomiendo capas consistentes primero. La estructura por features seria mejor cuando haya mas endpoints por modulo y tests que protejan los contratos.

## 20. Como deberian trabajar los agentes en este backend

Antes de tocar un endpoint:

- Revisar controller correspondiente.
- Revisar service si existe.
- Revisar repository si existe.
- Revisar DTOs usados por request/response.
- Revisar modelos/entidades relacionadas.
- Revisar consumo en `BlazorApp1`.
- Revisar tablas/procedimientos en `PracticaJWTcoreContext` o ADO.NET.

Antes de cambiar un DTO:

- Buscar todos los usos en backend.
- Buscar todos los usos en frontend.
- Confirmar serializacion JSON esperada.
- No renombrar propiedades sin plan.
- Documentar impacto en Blazor.

Antes de cambiar logica de negocio:

- Identificar si la regla esta en controller, service, repository, frontend o stored procedure.
- Revisar ventas/stock si afecta productos o movimientos.
- Verificar transacciones y consistencia de stock.
- No duplicar reglas.

Antes de cambiar acceso a datos:

- Revisar `PracticaJWTcoreContext`.
- Revisar `appsettings.json`.
- Revisar stored procedures si aplica.
- Revisar nombres de tablas/columnas.
- No cambiar migrations ni base de datos sin aprobacion.

No modificar sin aprobacion:

- `Program.cs`.
- `appsettings*.json`.
- entidades EF.
- DTOs publicos usados por frontend.
- stored procedures.
- cadena de conexion.
- configuracion JWT/CORS.
- reglas de venta/stock.
- nombres de endpoints.
- estructura de base de datos.

## 21. Plan gradual de mejora

### Etapa 1: ordenar sin cambiar comportamiento

Objetivo:

Entender y documentar contratos actuales.

Cambios propuestos:

- Documentar endpoints.
- Documentar DTOs reales.
- Identificar responses anonimas.
- Marcar endpoints que devuelven entidades.

Archivos impactados:

- Solo documentacion al principio.

Riesgos:

- Bajo.

Verificacion:

- Comparar documentacion con controllers y frontend.

### Etapa 2: separar responsabilidades

Objetivo:

Evitar controllers con logica de negocio.

Cambios propuestos:

- Crear `VentasService`, `StockService`, `ArticulosService`.
- Mover reglas desde controllers a services sin cambiar rutas ni responses.
- Mantener endpoints iguales.

Archivos impactados:

- `VentasController`
- `StockController`
- `ArticulosController`
- nuevos services e interfaces
- `Program.cs` para DI

Riesgos:

- Medio-alto, especialmente en venta/stock/transacciones.

Verificacion:

- Crear venta.
- Eliminar venta.
- Validar stock.
- Listar movimientos.
- Verificar respuestas.

### Etapa 3: mejorar DTOs y contratos

Objetivo:

Separar API publica de entidades EF.

Cambios propuestos:

- Crear request/response DTOs para articulos, stock y ventas.
- Mover DTOs internos de `VentasController` a `Dtos/`.
- Evitar exponer `Service`, `Articulos`, `StockMovimiento` directamente.

Archivos impactados:

- `Dtos/`
- controllers afectados
- frontend si cambia algun nombre

Riesgos:

- Medio, por contrato con Blazor.

Verificacion:

- Probar pantallas que consumen endpoints.
- Comparar JSON antes/despues.

### Etapa 4: mejorar validaciones y errores

Objetivo:

Tener respuestas predecibles y validaciones backend consistentes.

Cambios propuestos:

- Definir formato comun de error.
- Agregar middleware global.
- Mover validaciones de negocio a services.
- Evitar exponer `ex.Message` al cliente.

Archivos impactados:

- `Program.cs`
- posible carpeta `Middlewares/`
- controllers
- services

Riesgos:

- Medio si frontend espera texto plano.

Verificacion:

- Probar errores de login, venta sin stock, articulo inexistente y not found.

### Etapa 5: preparar testing

Objetivo:

Permitir refactors seguros.

Cambios propuestos:

- Agregar tests para services criticos.
- Tests de repository donde sea viable.
- Tests de endpoints principales si se incorpora testing de integracion.

Archivos impactados:

- proyectos de test existentes o nuevo proyecto de test.
- posiblemente configuracion de test DB/in-memory segun decision.

Riesgos:

- Medio, porque EF Core SQL Server y stored procedures pueden requerir entorno real o dobles de prueba.

Verificacion:

- Ejecutar suite completa.
- Validar que tests cubran ventas, stock, login y permisos.

## 22. Glosario de conceptos

Controller:

Clase que recibe HTTP y devuelve HTTP. En el proyecto: `ServiceController` recibe `GET api/service` y devuelve servicios.

Service:

Clase donde deberian vivir reglas de negocio. En el proyecto: `CustomerServices` existe, pero mayormente delega en repository.

Repository:

Clase que encapsula acceso a datos. En el proyecto: `ServicesRepository` usa `_context.Services` y EF Core.

Entity:

Clase que representa datos persistidos. En el proyecto: `Venta`, `VentaDetalle`, `Articulos`, `Usuarios`.

DTO:

Objeto para transportar datos sin exponer toda la entidad. En el proyecto: `CustomerDto`, `CreateUsuariosDTO`, `AppoitmentDetailsDTO`.

Request:

Objeto que entra a un endpoint. En el proyecto: `CreateCustomerDto` entra a `POST api/Customer`.

Response:

Objeto que sale de un endpoint. En el proyecto: `AppoitmentDetailsDTO` sale de `GET api/Pedidos`.

Dependency Injection:

Mecanismo para registrar y recibir dependencias. En el proyecto: `Program.cs` registra `IServiceServices -> ServiceServices` y el controller lo recibe por constructor.

Regla de negocio:

Decision propia del sistema. En el proyecto: no permitir vender si `StockActual < Cantidad`.

Capa de acceso a datos:

Codigo que consulta o modifica base de datos. En el proyecto: repositories y `PracticaJWTcoreContext`.

Acoplamiento:

Grado de dependencia entre partes. Ejemplo: si el frontend depende de `ServiceName`, cambiar esa propiedad rompe la pantalla.

Cohesion:

Que tan enfocada esta una clase en una responsabilidad. `VentasController` tiene baja cohesion porque maneja HTTP, negocio, stock, transacciones y queries.

Arquitectura por capas:

Organizacion donde controller, service y repository tienen responsabilidades separadas. El proyecto la usa parcialmente.

Clean Architecture:

Arquitectura donde dominio y casos de uso no dependen de infraestructura. El proyecto no llega a ese nivel porque API, EF, modelos y servicios estan en un mismo proyecto.

Refactor seguro:

Cambio interno sin modificar comportamiento observable. Ejemplo: mover logica de `VentasController` a `VentasService` manteniendo la misma ruta, request, response y efectos en SQL.

## Conclusion

El backend esta mejor que una API basica porque ya tiene controllers, services, repositories, DTOs, EF Core, JWT y Swagger. La parte mas ordenada esta en flujos como appointments, customers, services y pedidos, donde existe una separacion parcial entre controller, service y repository.

Las partes mas riesgosas son ventas, stock y articulos, porque concentran reglas de negocio y acceso directo a datos en controllers. Tambien son sensibles autenticacion y usuarios, porque mezclan JWT, roles, permisos, EF Core, ADO.NET y stored procedures.

Lo primero que deberia mejorar no es una gran migracion arquitectonica. La prioridad deberia ser estabilizar contratos y mover gradualmente reglas de ventas/stock/articulos a services, manteniendo las mismas rutas y respuestas. Despues conviene ordenar DTOs, errores, autorizacion y testing. Ese camino permite evolucionar hacia una arquitectura mas profesional sin romper el frontend ni la base de datos.
