# Arquitectura SQL Server

## 1. Resumen general

La base de datos analizada es `ApiProyecto` en SQL Server, conectada desde el backend mediante la cadena `DefaultConnection` definida en `PracticaJWTcore/appsettings.json`.

La base esta organizada alrededor de modulos de lavanderia/servicios, clientes, turnos o pedidos, usuarios/roles/permisos, articulos, ventas, stock y documentos electronicos.

Se detectaron 17 tablas reales en el esquema `dbo`:

- `Appointments`
- `AppointmentServices`
- `Articulos`
- `Categorias`
- `ComponentsForm`
- `Customer`
- `DocumentosElectronicos`
- `Permisos`
- `Roles`
- `RolesPermisos`
- `Services`
- `StockMovimientos`
- `Usuarios`
- `UsuariosRoles`
- `Vehicles`
- `VentaDetalles`
- `Ventas`

La base tiene primary keys, foreign keys reales, defaults en varias columnas e indices clustered de primary key. No se detectaron vistas ni funciones. Se detectaron tres procedimientos almacenados reales: `proc_ComparePass`, `proc_InsertUsuario` y `PruebaMSSQL`.

El backend usa SQL Server de dos maneras:

- Entity Framework Core, a traves de `PracticaJWTcoreContext`.
- ADO.NET con `SqlConnection` y `SqlCommand` para procedimientos de usuarios/autenticacion.

## 2. Mapa de tablas principales

### Appointments

Sirve para registrar turnos, pedidos o citas de servicio.

Columnas principales:

- `AppointmentID` bigint, identity, no null.
- `VehicleID` bigint, nullable.
- `EmployeeID` bigint, nullable.
- `AppointmentDate` datetime, nullable.
- `Comments` varchar(255), nullable.

Clave primaria:

- `PK_Appointments` sobre `AppointmentID`.

Relaciones detectadas:

- `VehicleID` -> `Vehicles.VehicleID`.
- `EmployeeID` -> `Customer.Id`.
- Relacion uno a muchos con `AppointmentServices`.

Modulo:

- Pedidos/appointments/servicios.

### AppointmentServices

Sirve como detalle o tabla puente entre appointments y services. Representa los servicios asociados a un turno.

Columnas principales:

- `IdAppointmentServices` bigint, identity, no null.
- `AppointmentID` bigint, no null.
- `ServiceID` bigint, no null.
- `Estado` nvarchar(50), nullable.

Clave primaria:

- `PK_AppointmentServices` sobre `IdAppointmentServices`.

Relaciones detectadas:

- `AppointmentID` -> `Appointments.AppointmentID`.
- `ServiceID` -> `Services.ServiceID`.

Modulo:

- Pedidos/appointments/servicios.

### Services

Catalogo de servicios ofrecidos.

Columnas principales:

- `ServiceID` bigint, identity, no null.
- `ServiceName` varchar(100), nullable.
- `Description` varchar(255), nullable.
- `Price` decimal(10,2), nullable.

Clave primaria:

- `PK_Services` sobre `ServiceID`.

Relaciones detectadas:

- Referenciada por `AppointmentServices`.

Modulo:

- Catalogo de servicios.

### Customer

Tabla de clientes o personas. Tambien se usa como empleado en `Appointments.EmployeeID`, por lo que actualmente cumple mas de un rol.

Columnas principales:

- `Id` bigint, identity, no null.
- `FirstName` varchar(200), nullable.
- `Email` varchar(200), nullable.
- `Phone` varchar(200), nullable.
- `Address` varchar(200), nullable.

Clave primaria:

- `PK_Customers` sobre `Id`.

Relaciones detectadas:

- Referenciada por `Appointments.EmployeeID`.
- Referenciada por `Usuarios.CustomerID`.
- Referenciada por `Ventas.IdCliente`.

Modulo:

- Clientes/personas/usuarios/ventas.

### Vehicles

Tabla de vehiculos asociados a appointments.

Columnas principales:

- `VehicleID` bigint, identity, no null.
- `LicensePlate` varchar(50), nullable.
- `Make` varchar(50), nullable.
- `Model` varchar(50), nullable.
- `OwnerName` varchar(200), nullable.

Clave primaria:

- `PK_Vehicles` sobre `VehicleID`.

Relaciones detectadas:

- Referenciada por `Appointments.VehicleID`.

Modulo:

- Vehiculos/appointments.

### Usuarios

Tabla de usuarios de sistema.

Columnas principales:

- `IdUsuario` int, identity, no null.
- `correo` nvarchar(100), no null.
- `clave` varbinary(max), no null.
- `CustomerID` bigint, nullable.
- `RoleId` int, nullable.

Clave primaria:

- `PK__Usuarios__3214EC077D9114D9` sobre `IdUsuario`.

Relaciones detectadas:

- `CustomerID` -> `Customer.Id`.
- `RoleId` -> `Roles.RoleId`.
- Referenciada por `UsuariosRoles.UsuarioId`.
- Referenciada por `Ventas.IdUsuario`.

Modulo:

- Seguridad/usuarios.

### Roles

Catalogo de roles.

Columnas principales:

- `RoleId` int, identity, no null.
- `RoleName` nvarchar(50), no null.

Clave primaria:

- `PK__Roles__3214EC07628B083F` sobre `RoleId`.

Indices:

- Unique index sobre `RoleName`.

Relaciones detectadas:

- Referenciada por `Usuarios.RoleId`.
- Referenciada por `UsuariosRoles.RolId`.
- Referenciada por `RolesPermisos.RoleId`.

Modulo:

- Seguridad/roles.

### Permisos

Catalogo de permisos.

Columnas principales:

- `PermisoId` int, identity, no null.
- `PermisoNombre` nvarchar(50), nullable.

Clave primaria:

- `PK__Permisos__3214EC0776FC9810` sobre `PermisoId`.

Indices:

- Unique index sobre `PermisoNombre`.

Relaciones detectadas:

- Referenciada por `RolesPermisos.PermisoId`.

Modulo:

- Seguridad/permisos.

### RolesPermisos

Tabla de asignacion de permisos por rol y componente.

Columnas principales:

- `RolePermisoId` int, identity, no null.
- `RoleId` int, no null.
- `PermisoId` int, no null.
- `ComponentsId` int, no null.

Clave primaria:

- `PK__RolesPer__3214EC079A8FD9E2` sobre `RolePermisoId`.

Relaciones detectadas:

- `RoleId` -> `Roles.RoleId`.
- `PermisoId` -> `Permisos.PermisoId`.
- `ComponentsId` -> `ComponentsForm.ComponentsId`.

Modulo:

- Seguridad/permisos por pantalla o componente.

### ComponentsForm

Catalogo de componentes o formularios usados para permisos.

Columnas principales:

- `ComponentsId` int, identity, no null.
- `ComponentsName` varchar(100), nullable.

Clave primaria:

- `PK__Componen__C532CD47C01129DC` sobre `ComponentsId`.

Relaciones detectadas:

- Referenciada por `RolesPermisos.ComponentsId`.

Modulo:

- Seguridad/UI/permisos.

### UsuariosRoles

Tabla puente entre usuarios y roles.

Columnas principales:

- `UsuariosRolesId` int, identity, no null.
- `UsuarioId` int, no null.
- `RolId` int, no null.

Clave primaria:

- `PK__Usuarios__3214EC0757EFD620` sobre `UsuariosRolesId`.

Relaciones detectadas:

- `UsuarioId` -> `Usuarios.IdUsuario`.
- `RolId` -> `Roles.RoleId`.

Modulo:

- Seguridad/roles.

Observacion:

- Tambien existe `Usuarios.RoleId`, por lo que hay dos formas de modelar rol de usuario: directa en `Usuarios` y many-to-many en `UsuariosRoles`. Esto puede ser redundante si ambas se usan para lo mismo.

### Articulos

Catalogo de productos/articulos vendibles o stockeables.

Columnas principales:

- `IdArticulo` int, identity, no null.
- `NombreArticulo` varchar(100), nullable.
- `Precio` decimal(18,0), nullable.
- `Codigo` varchar(50), nullable.
- `CodigoBarra` varchar(100), nullable.
- `Descripcion` varchar(255), nullable.
- `PrecioCosto` decimal(18,2), no null, default 0.
- `PrecioVenta` decimal(18,2), no null, default 0.
- `StockActual` decimal(18,2), no null, default 0.
- `StockMinimo` decimal(18,2), no null, default 0.
- `Activo` bit, no null, default 1.
- `IdCategoria` int, nullable.

Clave primaria:

- `PK__Articulo__F8FF5D5214535B28` sobre `IdArticulo`.

Relaciones detectadas:

- `IdCategoria` -> `Categorias.IdCategoria`.
- Referenciada por `VentaDetalles.IdArticulo`.
- Referenciada por `StockMovimientos.IdArticulo`.

Modulo:

- Productos/articulos/stock/ventas.

### Categorias

Catalogo de categorias de articulos.

Columnas principales:

- `IdCategoria` int, identity, no null.
- `NombreCategoria` varchar(100), no null.
- `Activo` bit, no null, default 1.

Clave primaria:

- `PK__Categori__A3C02A10CF6FE39C` sobre `IdCategoria`.

Relaciones detectadas:

- Referenciada por `Articulos.IdCategoria`.

Modulo:

- Catalogo de productos.

### Ventas

Cabecera transaccional de ventas.

Columnas principales:

- `IdVenta` bigint, identity, no null.
- `FechaVenta` datetime, no null, default `getdate()`.
- `IdCliente` bigint, nullable.
- `IdUsuario` int, nullable.
- `SubTotal` decimal(18,2), no null, default 0.
- `IvaTotal` decimal(18,2), no null, default 0.
- `Total` decimal(18,2), no null, default 0.
- `MetodoPago` varchar(50), nullable.
- `Estado` varchar(30), no null, default `CONFIRMADA`.

Clave primaria:

- `PK__Ventas__BC1240BD016AB5CF` sobre `IdVenta`.

Relaciones detectadas:

- `IdCliente` -> `Customer.Id`.
- `IdUsuario` -> `Usuarios.IdUsuario`.
- Relacion uno a muchos con `VentaDetalles`.
- Referenciada por `DocumentosElectronicos.IdVenta`.

Modulo:

- Ventas/facturacion/documentos.

### VentaDetalles

Detalle de articulos vendidos.

Columnas principales:

- `IdVentaDetalle` bigint, identity, no null.
- `IdVenta` bigint, no null.
- `IdArticulo` int, no null.
- `Cantidad` decimal(18,2), no null.
- `PrecioUnitario` decimal(18,2), no null.
- `PorcentajeIva` decimal(5,2), no null, default 10.
- `SubTotal` decimal(18,2), no null.

Clave primaria:

- `PK__VentaDet__2787211D6CA9B994` sobre `IdVentaDetalle`.

Relaciones detectadas:

- `IdVenta` -> `Ventas.IdVenta`.
- `IdArticulo` -> `Articulos.IdArticulo`.

Modulo:

- Ventas/detalle.

### StockMovimientos

Historial de movimientos de stock.

Columnas principales:

- `IdStockMovimiento` bigint, identity, no null.
- `FechaMovimiento` datetime, no null, default `getdate()`.
- `IdArticulo` int, no null.
- `TipoMovimiento` varchar(50), no null.
- `Cantidad` decimal(18,2), no null.
- `StockAnterior` decimal(18,2), nullable.
- `StockNuevo` decimal(18,2), nullable.
- `Referencia` varchar(100), nullable.
- `Observacion` varchar(255), nullable.

Clave primaria:

- `PK__StockMov__11B152EBFED85478` sobre `IdStockMovimiento`.

Relaciones detectadas:

- `IdArticulo` -> `Articulos.IdArticulo`.

Modulo:

- Stock/historial.

### DocumentosElectronicos

Tabla para documentos fiscales/electronicos asociados a ventas.

Columnas principales:

- `IdDocumentoElectronico` bigint, identity, no null.
- `IdVenta` bigint, no null.
- `TipoDocumento` varchar(50), no null.
- `NumeroDocumento` varchar(50), nullable.
- `EstadoFiscal` varchar(50), no null, default `PENDIENTE`.
- `XmlContenido` nvarchar(max), nullable.
- `XmlFirmado` nvarchar(max), nullable.
- `CodigoRespuesta` varchar(50), nullable.
- `MensajeRespuesta` nvarchar(max), nullable.
- `FechaAprobacion` datetime, nullable.
- `FechaCreacion` datetime, no null, default `getdate()`.

Clave primaria:

- `PK__Document__A31A4B7EB1162F1E` sobre `IdDocumentoElectronico`.

Relaciones detectadas:

- `IdVenta` -> `Ventas.IdVenta`.

Modulo:

- Documentos electronicos/fiscal/ventas.

## 3. Mapa de procedimientos almacenados

### proc_ComparePass

Objetivo:

- Comparar una clave recibida con la clave almacenada en `Usuarios.clave`.

Parametros:

- `@Correo` varchar(20), entrada.
- `@Clave` varchar(100), entrada.
- `@Compare` int output.

Tablas que consulta:

- `Usuarios`.

Quien parece usarlo:

- Backend `AutenticacionRespository.Login`.
- Backend `AutenticacionRespository.CambioClave`, antes de intentar cambiar clave.

Riesgos si se modifica:

- Puede romper login.
- Puede romper cambio de clave.
- `@Correo` es varchar(20), pero `Usuarios.correo` es nvarchar(100) y el backend envia correos que podrian superar 20 caracteres. Esto es un riesgo real de truncamiento o fallo.
- Usa `PWDENCRYPT/PWDCOMPARE`, funciones antiguas de SQL Server. Podrian ser insuficientes para un esquema moderno de seguridad.

### proc_InsertUsuario

Objetivo:

- Insertar un usuario con correo, cliente y rol.
- Genera una clave inicial fija cifrada con `PWDENCRYPT('123')`.

Parametros:

- `@Correo` varchar(200).
- `@CustomerID` bigint.
- `@RoleId` int.

Tablas que modifica:

- `Usuarios`.

Quien parece usarlo:

- Backend `UsuarioRepository.CreateUsuarios`.

Riesgos si se modifica:

- Rompe creacion de usuarios desde `UsuariosController`.
- Cambiar parametros rompe ADO.NET porque el backend los manda por nombre.
- La clave default `'123'` es un riesgo de seguridad.
- `@Correo` es varchar, mientras `Usuarios.correo` es nvarchar. Puede haber conversion implicita.

### PruebaMSSQL

Objetivo:

- Procedimiento simple de prueba que hace `SELECT * FROM dbo.Customer`.

Parametros:

- No tiene parametros.

Tablas que consulta:

- `Customer`.

Quien parece usarlo:

- No se detecto uso desde el backend ni frontend.
- Parece procedimiento de prueba/manual.

Riesgos si se modifica:

- Bajo si no lo consume ninguna aplicacion.
- Si alguien lo usa manualmente para pruebas, cambiar columnas por `SELECT *` depende de la estructura de `Customer`.

### proc_CambioClave

Estado:

- El backend lo llama en `AutenticacionRespository.CambioClave`.
- No fue encontrado en `INFORMATION_SCHEMA.ROUTINES` ni en `sys.objects` de la base `ApiProyecto`.

Riesgo:

- El endpoint `api/Autenticacion/CambioClave` puede fallar en ejecucion si intenta ejecutar este procedimiento.
- Si existe en otra base o ambiente, hay diferencia entre ambientes.

## 4. Flujo completo de datos

Caso elegido: crear venta.

### Pantalla o endpoint que inicia el flujo

El flujo inicia en el backend con `POST api/Ventas`, implementado en `PracticaJWTcore/Controllers/VentasController.cs`.

No se detecto una pantalla Blazor especifica que consuma `api/Ventas` en los archivos revisados, pero el endpoint existe y esta conectado a la base real por EF Core.

### Procedimiento o consulta usada

No usa procedimiento almacenado. Usa Entity Framework Core sobre `PracticaJWTcoreContext`.

Consultas y operaciones principales:

- Consulta `Articulos` para validar articulos del request.
- Inserta cabecera en `Ventas`.
- Inserta filas en `VentaDetalles`.
- Actualiza `Articulos.StockActual`.
- Inserta filas en `StockMovimientos`.

### Tablas participantes

- `Articulos`
- `Ventas`
- `VentaDetalles`
- `StockMovimientos`
- `Customer`, para enriquecer consultas con nombre de cliente.
- `Usuarios`, para enriquecer consultas con correo/nombre de usuario.

### Datos que vuelven al sistema

El endpoint devuelve una respuesta `Created` con:

- `IdVenta`
- `FechaVenta`
- `IdCliente`
- `Total`
- `Detalles`

En listados devuelve tambien nombres derivados:

- `NombreCliente` desde `Customer.FirstName`.
- `NombreUsuario` desde `Usuarios.correo`.
- `NombreArticulo` desde `Articulos.NombreArticulo`.

### Observacion arquitectonica

La base modela correctamente el patron cabecera/detalle con `Ventas` y `VentaDetalles`, y guarda historial en `StockMovimientos`. El riesgo esta en que la regla de negocio que mantiene consistencia entre venta, detalle, stock actual y movimiento vive en el controller, no en un procedimiento ni en un service especializado.

## 5. Diseño de tablas

### Nombres claros

La mayoria de nombres son claros:

- `Ventas`, `VentaDetalles`, `StockMovimientos`, `Articulos`, `Categorias`.
- `Roles`, `Permisos`, `RolesPermisos`.
- `DocumentosElectronicos`.

Hay nombres mixtos en ingles y espanol:

- `Customer`, `Services`, `Vehicles`, `Appointments`.
- `Usuarios`, `Roles`, `Permisos`, `Ventas`.

Esto no rompe la base, pero reduce consistencia y hace mas dificil mantener convenciones.

### Columnas consistentes

Hay mezcla de estilos:

- `IdVenta`, `IdArticulo`, `IdUsuario`.
- `ServiceID`, `AppointmentID`, `VehicleID`.
- `CustomerID`.
- `RoleId`.

Conviene elegir una convencion por proyecto, aunque no se deberia renombrar sin analizar impacto completo.

### Tipos de datos

Se usan tipos razonables en general:

- IDs como int o bigint identity.
- Importes como decimal.
- Fechas como datetime.
- Estados/nombres como varchar/nvarchar.

Riesgos:

- `Articulos.Precio` es decimal(18,0), sin decimales, mientras `PrecioCosto` y `PrecioVenta` son decimal(18,2).
- `proc_ComparePass.@Correo` varchar(20) no coincide con `Usuarios.correo` nvarchar(100).
- `AppointmentServices.Estado` aparece en metadata como nvarchar(50), mientras `sys.columns.max_length` muestra 100 bytes, que corresponde a nvarchar(50). Correcto, pero hay que tener presente la diferencia bytes/caracteres.

### Claves primarias

Todas las tablas detectadas tienen primary key.

### Claves foraneas

Hay FKs reales para las relaciones principales:

- Appointments con Customer y Vehicles.
- AppointmentServices con Appointments y Services.
- Articulos con Categorias.
- Ventas con Customer y Usuarios.
- VentaDetalles con Ventas y Articulos.
- StockMovimientos con Articulos.
- DocumentosElectronicos con Ventas.
- Usuarios con Customer y Roles.
- RolesPermisos con Roles, Permisos y ComponentsForm.
- UsuariosRoles con Usuarios y Roles.

### Auditoria

La auditoria es limitada:

- `DocumentosElectronicos` tiene `FechaCreacion`.
- `Ventas` tiene `FechaVenta`.
- `StockMovimientos` tiene `FechaMovimiento`.
- No se detectan campos generales como `UsuarioCreacion`, `FechaModificacion`, `UsuarioModificacion`.

### Estado activo/inactivo

Se detectan:

- `Articulos.Activo`.
- `Categorias.Activo`.
- `Ventas.Estado`.
- `AppointmentServices.Estado`.
- `DocumentosElectronicos.EstadoFiscal`.

No todas las tablas tienen estado o activo. En catalogos principales seria util tener una estrategia uniforme.

## 6. Relaciones entre tablas

### Uno a muchos

- `Customer` -> `Ventas`.
- `Usuarios` -> `Ventas`.
- `Categorias` -> `Articulos`.
- `Articulos` -> `VentaDetalles`.
- `Articulos` -> `StockMovimientos`.
- `Ventas` -> `VentaDetalles`.
- `Ventas` -> `DocumentosElectronicos`.
- `Customer` -> `Usuarios`.
- `Roles` -> `Usuarios`.
- `Vehicles` -> `Appointments`.
- `Customer` -> `Appointments` por `EmployeeID`.

### Muchos a muchos

- `Appointments` y `Services` mediante `AppointmentServices`.
- `Roles` y `Permisos` mediante `RolesPermisos`.
- `Usuarios` y `Roles` mediante `UsuariosRoles`, aunque tambien existe `Usuarios.RoleId`.

### Cabecera/detalle

- `Ventas` es cabecera y `VentaDetalles` es detalle.
- `Appointments` funciona como cabecera y `AppointmentServices` como detalle/puente.

### Catalogos

- `Services`
- `Categorias`
- `Roles`
- `Permisos`
- `ComponentsForm`
- `Vehicles`, dependiendo del dominio, puede ser catalogo o entidad operativa.

### Tablas transaccionales

- `Ventas`
- `VentaDetalles`
- `Appointments`
- `AppointmentServices`
- `StockMovimientos`
- `DocumentosElectronicos`

### Tablas historicas

- `StockMovimientos` funciona como historial de stock.
- No se detectaron tablas historicas especificas para auditoria de cambios.

## 7. Procedimientos almacenados

Los procedimientos existentes son pocos y estan centrados en usuarios/autenticacion, mas uno de prueba.

### Nombres

- `proc_ComparePass`: nombre entendible, aunque mezcla ingles y prefijo generico `proc_`.
- `proc_InsertUsuario`: claro.
- `PruebaMSSQL`: claramente de prueba.

### Parametros

Los parametros se usan por nombre desde ADO.NET. Esto es bueno porque no depende del orden.

Riesgos:

- `proc_ComparePass.@Correo` varchar(20) es demasiado corto para correos reales y no coincide con `Usuarios.correo` nvarchar(100).
- `proc_InsertUsuario` asigna una clave fija por defecto.

### Responsabilidades

- `proc_ComparePass` solo compara clave.
- `proc_InsertUsuario` inserta usuario y define password default.
- `PruebaMSSQL` solo consulta clientes.

No se detectaron procedimientos enormes ni con demasiadas responsabilidades.

### SQL dinamico

No se detecto SQL dinamico en las definiciones consultadas.

### Manejo de NULL

`proc_ComparePass` revisa existencia por correo. Si no existe, devuelve 0. No hay validaciones explicitas para parametros null.

`proc_InsertUsuario` no valida nulls antes de insertar. Depende de constraints de tabla y de FKs.

### Fechas

Los procedimientos encontrados no manejan fechas.

## 8. Consultas y performance

### Indices detectados

Se detectaron:

- Indices clustered de primary key en todas las tablas.
- Unique nonclustered en `Roles.RoleName`.
- Unique nonclustered en `Permisos.PermisoNombre`.

No se detectaron indices no clustered para foreign keys, busquedas por fecha, busquedas por texto o columnas frecuentes.

### Riesgos de performance

- `VentasController` consulta ventas y proyecta nombres con subconsultas a `Customer`, `Usuarios` y `Articulos`.
- `StockController` proyecta `NombreArticulo` con subconsulta por movimiento.
- `ArticulosController` proyecta `NombreCategoria` con subconsulta por articulo.
- `AppointmentRepository` usa subconsultas para obtener `EmployeeString` y `VehicleString`.
- `PedidosRepository` proyecta appointments con servicios asociados.

Con pocos datos funciona. Con volumen mayor convendria revisar planes de ejecucion, joins e indices.

### Filtros con LIKE

No se detectaron consultas SQL con `LIKE` en procedimientos ni consultas embebidas.

### Funciones sobre columnas filtradas

No se detectaron en procedimientos. En backend se usa LINQ, no SQL textual.

### Muchas uniones

No se detectaron procedimientos con muchas uniones. Las relaciones se resuelven desde EF Core.

### Busquedas por fecha

No se detectaron endpoints con filtros por fecha, pero tablas transaccionales tienen fechas:

- `Ventas.FechaVenta`
- `StockMovimientos.FechaMovimiento`
- `Appointments.AppointmentDate`
- `DocumentosElectronicos.FechaCreacion`
- `DocumentosElectronicos.FechaAprobacion`

Si se agregan reportes o filtros por fecha, conviene revisar indices.

### Paginacion y Count

No se detecto paginacion en endpoints principales. Listados como ventas, servicios, pedidos y stock devuelven listas completas. Esto puede ser suficiente en desarrollo, pero no escala bien.

## 9. Fechas y formatos

La base usa `datetime` en:

- `Appointments.AppointmentDate`
- `Ventas.FechaVenta`
- `StockMovimientos.FechaMovimiento`
- `DocumentosElectronicos.FechaAprobacion`
- `DocumentosElectronicos.FechaCreacion`

Defaults:

- `Ventas.FechaVenta` usa `getdate()`.
- `StockMovimientos.FechaMovimiento` usa `getdate()`.
- `DocumentosElectronicos.FechaCreacion` usa `getdate()`.

El backend usa `DateTime.UtcNow` en ventas y movimientos de stock. Esto puede generar diferencia entre fechas guardadas por backend y defaults SQL (`getdate()`, hora local del servidor).

Riesgos:

- Mezcla UTC en C# y hora local SQL Server.
- Filtros futuros con `dd/MM/yyyy` pueden fallar si se convierten strings en SQL.
- Filtros `BETWEEN fechaDesde AND fechaHasta` pueden excluir registros del ultimo dia si `fechaHasta` llega a medianoche.

Forma mas segura:

- Enviar fechas parametrizadas, nunca como strings concatenados.
- Usar formato ISO `yyyy-MM-dd` si se necesita texto.
- Para rangos, usar `>= @Desde AND < DATEADD(day, 1, @Hasta)` cuando `@Hasta` representa un dia completo.
- Definir una politica unica: UTC en toda la app o hora local controlada.

## 10. Contrato con backend

### EF Core

El contrato principal esta en `PracticaJWTcoreContext`. El backend espera tablas y columnas concretas:

- `Articulos.IdArticulo`, `NombreArticulo`, `PrecioVenta`, `StockActual`, `IdCategoria`.
- `Ventas.IdVenta`, `FechaVenta`, `IdCliente`, `IdUsuario`, `SubTotal`, `IvaTotal`, `Total`, `MetodoPago`, `Estado`.
- `VentaDetalles.IdVenta`, `IdArticulo`, `Cantidad`, `PrecioUnitario`, `PorcentajeIva`, `SubTotal`.
- `StockMovimientos.IdArticulo`, `FechaMovimiento`, `TipoMovimiento`, `Cantidad`, `StockAnterior`, `StockNuevo`, `Referencia`.
- `Usuarios.correo`, `clave`, `RoleId`, `CustomerID`.

Si se cambia un nombre de columna, falla el mapeo EF o las proyecciones.

### ADO.NET

Procedimientos usados:

- `proc_ComparePass`
- `proc_InsertUsuario`
- `proc_CambioClave`, llamado por backend pero no encontrado en la base actual.

Riesgos:

- Cambiar nombre de procedimiento rompe `SqlCommand`.
- Cambiar nombre de parametro rompe la ejecucion.
- Cambiar tipo de dato puede generar conversiones, truncamientos o errores.
- Cambiar columnas devueltas por procedimientos afecta consumidores si existieran. En los procedimientos actuales no se devuelven resultsets importantes, salvo `PruebaMSSQL`.

### Aliases y orden

El backend usa EF Core y objetos anonimos para respuestas, por lo que depende mas de nombres de propiedades C# que de aliases SQL. Pero en stored procedures, si se agregan futuros resultsets, conviene estabilizar aliases.

## 11. Contrato con reportes

No se detectaron archivos `.rdlc`, referencias a ReportViewer, SSRS, datasets o procedimientos de reportes en el repositorio inspeccionado.

No se detectaron procedimientos claramente orientados a reportes. `PruebaMSSQL` parece de prueba y hace `SELECT * FROM dbo.Customer`.

Riesgos futuros:

- Si se crean reportes sobre procedimientos, cambiar nombres de columnas rompe datasets.
- Si se usan `SELECT *`, agregar/quitar columnas puede alterar reportes y exportaciones.
- Para reportes convendria usar vistas o procedimientos con columnas explicitas y aliases estables.

## 12. Normalización

La base esta parcialmente normalizada.

Aspectos bien normalizados:

- `Ventas` y `VentaDetalles` separan cabecera/detalle.
- `Articulos` se relaciona con `Categorias`.
- `Roles`, `Permisos` y `ComponentsForm` estan separados.
- `RolesPermisos` modela asignaciones.
- `AppointmentServices` separa appointments de services.
- `DocumentosElectronicos` se separa de ventas.

Posibles redundancias o mezclas:

- `Usuarios.RoleId` y `UsuariosRoles` modelan roles de usuario en paralelo. Si ambos significan lo mismo, hay redundancia.
- `Customer` parece servir para clientes y empleados (`Appointments.EmployeeID`). Si el dominio distingue cliente, empleado y usuario, podria convenir separarlo o definir un tipo de persona.
- `Vehicles.OwnerName` guarda texto de propietario, pero tambien existen clientes. Si el propietario deberia ser un cliente, hay posible duplicacion.
- `Articulos.Precio`, `PrecioCosto` y `PrecioVenta` pueden ser validos, pero hay que definir claramente para que sirve cada uno.

No hay evidencia de tablas historicas generales.

## 13. Auditoría e historial

Campos detectados:

- `Ventas.FechaVenta`
- `StockMovimientos.FechaMovimiento`
- `DocumentosElectronicos.FechaCreacion`
- `DocumentosElectronicos.FechaAprobacion`
- `Articulos.Activo`
- `Categorias.Activo`
- `Ventas.Estado`
- `AppointmentServices.Estado`
- `DocumentosElectronicos.EstadoFiscal`

Campos no detectados de auditoria general:

- `UsuarioCreacion`
- `FechaModificacion`
- `UsuarioModificacion`
- `UsuarioAnulacion`
- `FechaAnulacion`
- `MotivoAnulacion`

La tabla con mejor perfil historico es `StockMovimientos`, porque registra movimiento, stock anterior, stock nuevo y referencia.

La base permite auditar parcialmente operaciones de stock y fechas transaccionales, pero no permite reconstruir de forma completa quien creo o modifico cada registro.

## 14. Seguridad

### SQL dinamico

No se detecto SQL dinamico en procedimientos almacenados.

### Parametros

Los procedimientos usan parametros. El backend llama procedimientos con `SqlParameter` o `AddWithValue`.

Esto reduce riesgo de SQL injection.

### Riesgos detectados

- `proc_InsertUsuario` crea usuarios con password inicial fija `'123'`.
- `Usuarios.clave` usa `PWDENCRYPT/PWDCOMPARE`, funciones antiguas. Para una arquitectura mas profesional conviene manejar hashing moderno en backend o esquema seguro definido.
- `proc_ComparePass.@Correo` varchar(20) puede truncar correos.
- `Usuarios.correo` no aparece con indice unique en la metadata consultada. Podria permitir duplicados si no hay otra restriccion no detectada.
- No se analizaron permisos SQL de usuarios/logins, solo estructura de base.

### Datos sensibles

Datos sensibles:

- `Usuarios.clave`.
- `Customer.Email`, `Phone`, `Address`.
- XML fiscal en `DocumentosElectronicos`.

Estos campos requieren cuidado en backups, logs, reportes y accesos.

## 15. Estado actual de la arquitectura SQL

Clasificacion: intermedia, transaccional parcialmente ordenada y acoplada a la aplicacion.

Evidencias positivas:

- Todas las tablas tienen primary key.
- Hay foreign keys reales.
- Hay separacion cabecera/detalle para ventas.
- Hay historial de stock.
- Hay catalogos para categorias, roles, permisos y servicios.
- Hay defaults utiles en estados, fechas y montos.

Evidencias de riesgo:

- Indices no clustered limitados.
- Auditoria incompleta.
- Nombres mezclados ingles/espanol.
- Redundancia potencial entre `Usuarios.RoleId` y `UsuariosRoles`.
- Stored procedure faltante `proc_CambioClave` respecto al backend.
- Password default fija en `proc_InsertUsuario`.
- Mezcla de reglas de negocio en backend con persistencia SQL.

No es una base basica sin relaciones. Tampoco parece todavia una arquitectura SQL profesional/escalable completa.

## 16. Problemas o riesgos detectados

- `proc_CambioClave` es llamado por backend pero no existe en la base consultada.
- `proc_ComparePass.@Correo` varchar(20) no coincide con `Usuarios.correo` nvarchar(100).
- `proc_InsertUsuario` genera clave default `'123'`.
- Uso de `PWDENCRYPT/PWDCOMPARE`, mecanismo antiguo para claves.
- Posible duplicidad de roles por `Usuarios.RoleId` y `UsuariosRoles`.
- No se detecto unique index en `Usuarios.correo`.
- No se detectaron indices no clustered para foreign keys frecuentes.
- No se detectaron indices para fechas transaccionales.
- No hay auditoria general de creacion/modificacion por usuario.
- Mezcla de nombres ingles/espanol.
- `Customer` se usa como cliente y empleado.
- `Vehicles.OwnerName` puede duplicar concepto de propietario/cliente.
- Listados backend no tienen paginacion.
- Fechas pueden mezclar UTC desde C# con `getdate()` SQL.
- `PruebaMSSQL` usa `SELECT *`.
- No hay vistas ni procedimientos dedicados para reportes.

## 17. Mejoras recomendadas

### Mejoras rápidas

1. Documentar procedimientos existentes.
   - Que cambiar: documentar objetivo, parametros y consumidores de `proc_ComparePass`, `proc_InsertUsuario`, `PruebaMSSQL`.
   - Por que: evita cambios accidentales.
   - Riesgo: bajo.
   - Como verificar: comparar docs con `INFORMATION_SCHEMA.PARAMETERS` y definiciones.
   - Aplicaciones afectadas: backend `PracticaJWTcore`.

2. Confirmar `proc_CambioClave`.
   - Que cambiar: verificar si debe existir en `ApiProyecto` o si el backend apunta a procedimiento obsoleto.
   - Por que: el endpoint de cambio de clave puede fallar.
   - Riesgo: bajo si solo se investiga.
   - Como verificar: buscar en todos los ambientes y probar endpoint en entorno controlado.
   - Aplicaciones afectadas: backend login/autenticacion.

3. Documentar contratos SQL-backend.
   - Que cambiar: listar tablas/columnas consumidas por EF y SPs.
   - Por que: evita renombres peligrosos.
   - Riesgo: bajo.
   - Como verificar: revisar `PracticaJWTcoreContext` y repositories.
   - Aplicaciones afectadas: backend y frontend indirectamente.

4. Revisar alias y evitar `SELECT *`.
   - Que cambiar: en futuros procedimientos/reportes, usar columnas explicitas.
   - Por que: contratos mas estables.
   - Riesgo: bajo si es solo criterio nuevo.
   - Como verificar: revisar definiciones SQL.
   - Aplicaciones afectadas: reportes y consumidores SQL.

### Mejoras medianas

1. Revisar indices para FKs y fechas.
   - Que cambiar: proponer indices en columnas como `Ventas.FechaVenta`, `VentaDetalles.IdVenta`, `StockMovimientos.IdArticulo`, `StockMovimientos.FechaMovimiento`, `Usuarios.correo`.
   - Por que: mejorar consultas y joins.
   - Riesgo: medio; indices aceleran lectura pero afectan escrituras y ocupan espacio.
   - Como verificar: planes de ejecucion, estadisticas IO/time y pruebas con volumen.
   - Aplicaciones afectadas: backend, reportes futuros.

2. Estandarizar seguridad de usuarios.
   - Que cambiar: revisar password default y hashing.
   - Por que: seguridad.
   - Riesgo: alto si afecta login existente.
   - Como verificar: pruebas de login, cambio de clave y creacion de usuario.
   - Aplicaciones afectadas: backend, frontend login.

3. Definir modelo de roles.
   - Que cambiar: decidir si se usa `Usuarios.RoleId` o `UsuariosRoles`.
   - Por que: evitar inconsistencia.
   - Riesgo: medio/alto por impacto en permisos.
   - Como verificar: revisar consultas de permisos y usuarios.
   - Aplicaciones afectadas: backend usuarios/permisos.

4. Crear estrategia de fechas.
   - Que cambiar: definir UTC o hora local y filtros seguros.
   - Por que: evitar errores por zona horaria y rangos.
   - Riesgo: medio.
   - Como verificar: pruebas con ventas al limite del dia.
   - Aplicaciones afectadas: ventas, stock, reportes.

### Mejoras grandes

1. Agregar auditoria transversal.
   - Que cambiar: campos `FechaCreacion`, `UsuarioCreacion`, `FechaModificacion`, `UsuarioModificacion` en tablas criticas.
   - Por que: trazabilidad profesional.
   - Riesgo: alto por cambios de schema y backend.
   - Como verificar: pruebas de insert/update por modulo.
   - Aplicaciones afectadas: backend, reportes, posibles migraciones.

2. Separar persona/cliente/empleado si el dominio lo requiere.
   - Que cambiar: redisenar `Customer` o agregar entidades especializadas.
   - Por que: evitar que una tabla tenga roles ambiguos.
   - Riesgo: alto.
   - Como verificar: mapa de dependencias, migracion de datos y pruebas completas.
   - Aplicaciones afectadas: appointments, ventas, usuarios.

3. Crear vistas o procedimientos para reportes.
   - Que cambiar: vistas/procs con contratos estables para ventas, stock y documentos.
   - Por que: separar reporteria del modelo transaccional.
   - Riesgo: medio.
   - Como verificar: comparar resultados con consultas base.
   - Aplicaciones afectadas: futuros reportes.

4. Normalizar historicos y documentos.
   - Que cambiar: definir tablas de eventos/historial por modulo.
   - Por que: trazabilidad y auditoria.
   - Riesgo: alto.
   - Como verificar: pruebas de flujo completo y conciliacion.
   - Aplicaciones afectadas: ventas, stock, documentos.

## 18. Arquitectura SQL recomendada

Una organizacion profesional adaptada al proyecto podria separar mentalmente las tablas por tipo:

### Tablas catalogo

- `Services`
- `Categorias`
- `Articulos`
- `Roles`
- `Permisos`
- `ComponentsForm`
- `Vehicles`, si se considera catalogo operativo.

### Tablas transaccionales

- `Appointments`
- `Ventas`
- `DocumentosElectronicos`

### Tablas detalle

- `AppointmentServices`
- `VentaDetalles`
- `RolesPermisos`
- `UsuariosRoles`

### Tablas historicas/auditoria

- `StockMovimientos` ya funciona como historico de stock.
- Futuras tablas recomendadas: auditoria de ventas, auditoria de usuarios, historial de documentos fiscales.

### Procedimientos por modulo

Convencion sugerida, sin ejecutar cambios:

- `seg.proc_ComparePass`
- `seg.proc_InsertUsuario`
- `seg.proc_CambioClave`
- `ventas.proc_ReporteVentas`
- `stock.proc_KardexArticulo`

Esto requeriria esquemas SQL por modulo y plan de migracion. No conviene hacerlo sin pruebas.

### Vistas para reportes

Podrian existir vistas de solo lectura:

- `vw_VentasDetalle`
- `vw_StockMovimientos`
- `vw_AppointmentsServicios`
- `vw_DocumentosElectronicos`

Serian utiles para reportes si se mantienen con columnas explicitas.

### Indices recomendados a revisar

No ejecutar automaticamente. Solo revisar con planes de ejecucion:

- `Usuarios(correo)` posiblemente unique.
- `Ventas(FechaVenta)`.
- `Ventas(IdCliente)`.
- `Ventas(IdUsuario)`.
- `VentaDetalles(IdVenta)`.
- `VentaDetalles(IdArticulo)`.
- `StockMovimientos(IdArticulo, FechaMovimiento)`.
- `Appointments(AppointmentDate)`.
- `AppointmentServices(AppointmentID)`.
- `RolesPermisos(RoleId, ComponentsId)`.

## 19. Cómo deberían trabajar los agentes con SQL Server

- No modificar tablas sin revisar backend, frontend y `PracticaJWTcoreContext`.
- No modificar procedimientos sin buscar referencias en todo el repositorio.
- No cambiar nombres de columnas sin buscar referencias en C#, DTOs, LINQ, frontend y reportes.
- No cambiar tipos de datos sin revisar modelos C#, validaciones y serializacion.
- No cambiar filtros de fecha sin probar inicio de dia, fin de dia, zona horaria y fechas null.
- No tocar reportes sin revisar datasets, parametros y campos devueltos.
- No ejecutar `ALTER`, `DROP`, `DELETE`, `UPDATE`, `INSERT`, migraciones ni scripts destructivos sin plan y aprobacion.
- Siempre entregar plan antes de cualquier `ALTER`.
- Para inspeccion, usar solo `SELECT` contra `sys.*`, `INFORMATION_SCHEMA.*` y definiciones.
- Antes de tocar usuarios/autenticacion, revisar `proc_ComparePass`, `proc_InsertUsuario`, posible `proc_CambioClave`, `Usuarios`, `Roles`, `Permisos`, backend y frontend login.
- Antes de tocar ventas/stock, revisar `Ventas`, `VentaDetalles`, `Articulos`, `StockMovimientos`, controllers y cualquier reporte.

## 20. Plan gradual de mejora

### Etapa 1: documentación

Objetivo:

- Documentar tablas, columnas, relaciones, SPs y contratos.

Riesgo:

- Bajo.

Beneficio:

- Reduce cambios a ciegas.

Verificación:

- Comparar documento con metadata real y backend.

### Etapa 2: mapa de dependencias

Objetivo:

- Mapear cada tabla/procedimiento con endpoints, repositories, frontend y reportes.

Riesgo:

- Bajo.

Beneficio:

- Permite estimar impacto antes de tocar SQL.

Verificación:

- Buscar referencias con `rg` y comparar con dependencias de SQL Server.

### Etapa 3: estandarización de nombres

Objetivo:

- Definir convenciones futuras sin romper lo existente.

Riesgo:

- Medio si se renombra; bajo si solo se documenta.

Beneficio:

- Facilita mantenimiento.

Verificación:

- Checklist de nombres nuevos y contratos existentes.

### Etapa 4: mejora de performance

Objetivo:

- Revisar indices, paginacion y consultas frecuentes.

Riesgo:

- Medio.

Beneficio:

- Mejor respuesta con mas datos.

Verificación:

- Planes de ejecucion, `SET STATISTICS IO/TIME`, pruebas con volumen.

### Etapa 5: auditoría e historial

Objetivo:

- Agregar trazabilidad profesional en tablas criticas.

Riesgo:

- Alto por cambios de schema/backend.

Beneficio:

- Saber quien creo/modifico/anulo datos.

Verificación:

- Pruebas de insert/update/delete logico por modulo.

### Etapa 6: testing de procedimientos

Objetivo:

- Tener pruebas para SPs y contratos SQL.

Riesgo:

- Medio si requiere datos controlados.

Beneficio:

- Cambios mas seguros.

Verificación:

- Casos de login valido/invalido, usuario duplicado, parametros null, cambio de clave, reportes futuros.

## 21. Glosario de conceptos

Tabla catalogo: tabla con datos de referencia. Ejemplo: `Categorias`, `Services`, `Roles`, `Permisos`.

Tabla transaccional: tabla que registra operaciones del negocio. Ejemplo: `Ventas`, `Appointments`, `DocumentosElectronicos`.

Cabecera/detalle: patron donde una tabla principal tiene varias lineas asociadas. Ejemplo: `Ventas` y `VentaDetalles`.

Primary key: columna o conjunto de columnas que identifica una fila. Ejemplo: `Ventas.IdVenta`.

Foreign key: restriccion que conecta una tabla con otra. Ejemplo: `VentaDetalles.IdVenta` hacia `Ventas.IdVenta`.

Indice: estructura para acelerar busquedas. Ejemplo: primary keys clustered e indices unique en `Roles.RoleName`.

Stored procedure: bloque SQL guardado y ejecutable por nombre. Ejemplo: `proc_ComparePass`.

Vista: consulta guardada que se usa como tabla virtual. No se detectaron vistas en esta base.

Funcion: objeto SQL que devuelve un valor o tabla. No se detectaron funciones en esta base.

Normalizacion: separar datos para evitar duplicacion. Ejemplo: `Articulos` separado de `Categorias`.

Desnormalizacion: duplicar o guardar datos derivados por performance o simplicidad. Ejemplo potencial: `Vehicles.OwnerName` si el propietario tambien existe como customer.

Auditoria: capacidad de saber quien hizo un cambio y cuando. Actualmente es parcial.

Historicos: tablas que guardan eventos pasados. Ejemplo: `StockMovimientos`.

Contrato SQL-backend: acuerdo implicito entre tablas/procedimientos y codigo C#. Ejemplo: `UsuarioRepository` espera `proc_InsertUsuario(@Correo, @CustomerID, @RoleId)`.

Performance: rendimiento de consultas y escrituras. Depende de indices, volumen, joins, filtros y planes.

Plan de ejecucion: estrategia que SQL Server usa para resolver una consulta. Sirve para diagnosticar lentitud.

CTE: expresion comun de tabla con `WITH`. No se detecto uso en procedimientos actuales.

SQL dinamico: SQL construido como texto y ejecutado. No se detecto en procedimientos actuales.

Refactor seguro en base de datos: cambio interno con compatibilidad hacia aplicaciones. Ejemplo: agregar un indice sin cambiar columnas ni resultados.

## Conclusion

La base de datos esta mejor organizada que una base inicial: tiene primary keys, foreign keys reales, tablas transaccionales, detalles, catalogos, historial de stock y procedimientos parametrizados para usuarios.

Los riesgos mas importantes son la inconsistencia entre backend y SQL por `proc_CambioClave`, el manejo de passwords con clave default y funciones antiguas, la auditoria limitada, la falta de indices no clustered visibles para consultas frecuentes y la redundancia potencial en roles de usuario.

Lo primero que conviene mejorar es documentar dependencias, confirmar/corregir el contrato de autenticacion y revisar indices con planes reales. No conviene tocar todavia nombres de tablas/columnas, modelo de roles, separacion de `Customer` o auditoria transversal sin un plan mayor, pruebas y revision completa de backend/frontend.
