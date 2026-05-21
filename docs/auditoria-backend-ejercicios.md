# Auditoria Backend/API - Ejercicios Practicos

Inspeccion realizada sobre la parte backend/API del proyecto `PracticaJWTcore`, con revision de controllers, services, repositories, DTOs, models/entities, configuracion JWT, SQL Server, endpoints y contratos minimos consumidos por los frontends presentes en el repo.

Nota: en este repo no se encontro React como frontend activo; hay proyectos Blazor/Razor (`BlazorApp1`, `BlazorFrontend`, `ApiProyectoFrontend`). La revision de frontend se limito a detectar contratos API que podrian romperse.

## Prioridad Alta

## Ejercicio 1: Aplicar autorizacion real por rol o permiso

### Problema detectado

Muchos endpoints sensibles tienen `[Authorize]`, pero no validan rol ni permisos. Un usuario autenticado podria intentar crear roles, crear usuarios, borrar ventas, modificar stock o ver usuarios si tiene token valido.

### Ubicacion

- `PracticaJWTcore/Controllers/UsuariosController.cs`
- `PracticaJWTcore/Controllers/VentasController.cs`
- `PracticaJWTcore/Controllers/StockController.cs`
- `PracticaJWTcore/Controllers/ArticulosController.cs`
- Tablas: `Roles`, `Permisos`, `RolesPermisos`, `ComponentsForm`

### Por que es un problema

Autenticacion no es autorizacion. Tener token no deberia habilitar todas las acciones administrativas.

### Objetivo del ejercicio

Hacer que acciones sensibles validen rol o permisos por componente/pantalla.

### Tarea practica

Elegir un modulo, por ejemplo `Usuarios`. Proteger `CreateUsuario`, `CreateRole`, `UpdateRole`, `CreatePermisosRole` para que solo roles autorizados puedan usarlos. Usar claims del token o una policy/servicio de permisos.

### Pistas

El JWT ya agrega `ClaimTypes.Role` en `AutenticacionRespository.GenerateToken`. Empezar con `[Authorize(Roles = "Admin")]` antes de hacer permisos dinamicos.

### Criterios para saber si esta bien resuelto

Un usuario no admin recibe `403`. Un admin puede ejecutar la accion. Los endpoints de lectura siguen funcionando segun el permiso esperado.

### Nivel de dificultad

Avanzado

### Prioridad

Alta

### Tipo de mejora

Seguridad, Autorizacion, Controllers

## Ejercicio 2: Corregir respuesta de login invalido

### Problema detectado

El login siempre devuelve `200 OK` si el repository devuelve un objeto, incluso cuando el token viene vacio y `RolId = 0`.

### Ubicacion

- `PracticaJWTcore/Controllers/AutenticacionController.cs`
- `PracticaJWTcore/Repositorios/AutenticacionRespository.cs`

### Por que es un problema

El frontend puede interpretar un login fallido como exitoso. Tambien dificulta manejar errores correctamente.

### Objetivo del ejercicio

Devolver `401 Unauthorized` cuando correo/clave sean invalidos.

### Tarea practica

Ajustar el flujo `Repository -> Service -> Controller` para distinguir login valido e invalido sin depender implicitamente de `Token == ""`.

### Pistas

Se puede usar `ServiceResult<TokenRolDTO>` o validar `string.IsNullOrWhiteSpace(response.Token)` en controller como primer paso.

### Criterios para saber si esta bien resuelto

Credenciales invalidas devuelven `401`. Credenciales validas devuelven `200` con `tokenRol`.

### Nivel de dificultad

Intermedio

### Prioridad

Alta

### Tipo de mejora

Autenticacion, Manejo de errores, Integracion con frontend

## Ejercicio 3: Evitar cambio de clave de otro usuario

### Problema detectado

`CambioClave` recibe `correo` en el body. El endpoint tiene `[Authorize]`, pero no compara ese correo con el usuario autenticado del token.

### Ubicacion

- `PracticaJWTcore/Controllers/AutenticacionController.cs`
- `PracticaJWTcore/Models/CambioClave.cs`
- `PracticaJWTcore/Repositorios/AutenticacionRespository.cs`

### Por que es un problema

Un usuario autenticado podria intentar cambiar la clave de otro correo si conoce o consigue la clave actual.

### Objetivo del ejercicio

Usar la identidad del token como fuente de verdad.

### Tarea practica

Leer `User.FindFirst(ClaimTypes.NameIdentifier)` en el controller o service, y rechazar si no coincide con `cambio.correo`.

### Pistas

El token ya guarda `ClaimTypes.NameIdentifier` con el correo.

### Criterios para saber si esta bien resuelto

Si el correo del body no coincide con el token, devuelve `403` o `400`. Si coincide y la clave actual es valida, cambia la clave.

### Nivel de dificultad

Intermedio

### Prioridad

Alta

### Tipo de mejora

Seguridad, Autenticacion

## Ejercicio 4: No confiar en IdUsuario enviado desde ventas

### Problema detectado

`CreateVentaDto` permite enviar `IdUsuario`. El backend usa ese valor para registrar la venta.

### Ubicacion

- `PracticaJWTcore/Dtos/Ventas/CreateVentaDto.cs`
- `PracticaJWTcore/Services/VentasService.cs`

### Por que es un problema

Un usuario podria crear ventas a nombre de otro usuario.

### Objetivo del ejercicio

Registrar ventas con el usuario autenticado, no con un valor manipulable del body.

### Tarea practica

Obtener el usuario desde el token. Buscar su `IdUsuario` por correo y usarlo al crear la venta.

### Pistas

Se puede agregar un metodo en `IUsuariosRepository` o en `IVentasRepository` para resolver `IdUsuario` por correo.

### Criterios para saber si esta bien resuelto

Aunque el request mande otro `IdUsuario`, la venta queda asociada al usuario del token.

### Nivel de dificultad

Avanzado

### Prioridad

Alta

### Tipo de mejora

Seguridad, Autorizacion, Services

## Ejercicio 5: Proteger endpoints publicos que exponen catalogos

### Problema detectado

`PaginaBaseController` no tiene `[Authorize]`. `ServiceController.GetArticulos` tampoco tiene `[Authorize]`.

### Ubicacion

- `PracticaJWTcore/Controllers/PaginaBaseController.cs`
- `PracticaJWTcore/Controllers/ServiceController.cs`

### Por que es un problema

Cualquier cliente puede obtener clientes, vehiculos, servicios o articulos si conoce la URL.

### Objetivo del ejercicio

Definir que catalogos son publicos y proteger los privados.

### Tarea practica

Agregar `[Authorize]` donde corresponda y probar que el frontend siga pudiendo cargar combos luego del login.

### Pistas

Si algun combo se necesita antes del login, separarlo explicitamente como publico.

### Criterios para saber si esta bien resuelto

Sin token, endpoints privados devuelven `401`. Con token, devuelven datos correctamente.

### Nivel de dificultad

Basico

### Prioridad

Alta

### Tipo de mejora

Seguridad, Controllers

## Ejercicio 6: Evitar 500 por registros inexistentes

### Problema detectado

Varios repositories usan `FirstAsync`. Si no existe el registro, lanza excepcion y puede terminar como `500`.

### Ubicacion

- `PracticaJWTcore/Repositorios/CustomerRepository.cs`
- `PracticaJWTcore/Repositorios/ServicesRepository.cs`
- `PracticaJWTcore/Repositorios/AppointmentRepository.cs`

### Por que es un problema

Un "no encontrado" normal se convierte en error interno.

### Objetivo del ejercicio

Devolver `404 NotFound` cuando el recurso no existe.

### Tarea practica

Cambiar `FirstAsync` por `FirstOrDefaultAsync`, propagar `null` o `ServiceResult`, y ajustar controllers.

### Pistas

Usar como referencia el patron de `ArticulosController.GetById`.

### Criterios para saber si esta bien resuelto

`GET /api/customer/999999`, `GET /api/service/999999` y deletes inexistentes devuelven `404`, no `500`.

### Nivel de dificultad

Intermedio

### Prioridad

Alta

### Tipo de mejora

Manejo de errores, Repositories, Controllers

## Prioridad Media

## Ejercicio 7: Dejar de exponer entidades EF como contrato API

### Problema detectado

Algunos endpoints reciben o devuelven entidades EF directamente (`Customer`, `Service`, `ComponentsForm`, `Articulos`).

### Ubicacion

- `PracticaJWTcore/Controllers/CustomerController.cs`
- `PracticaJWTcore/Controllers/ServiceController.cs`
- `PracticaJWTcore/Repositorios/UsuarioRepository.cs`

### Por que es un problema

Acopla frontend a la base, puede exponer campos de mas y complica cambios futuros.

### Objetivo del ejercicio

Usar DTOs de request/response en esos endpoints.

### Tarea practica

Elegir primero `ServiceController`: crear `ServiceRequestDto` y `ServiceResponseDto`, mapearlos en service/repository.

### Pistas

No cambiar nombres JSON historicos sin verificar el consumidor Blazor (`api/service` se usa en `BlazorApp1`).

### Criterios para saber si esta bien resuelto

Swagger muestra DTOs, no entidades EF. El frontend sigue deserializando lo que espera.

### Nivel de dificultad

Intermedio

### Prioridad

Media

### Tipo de mejora

DTOs, Integracion con frontend, Arquitectura backend

## Ejercicio 8: Unificar manejo de respuestas HTTP

### Problema detectado

Algunos endpoints devuelven strings, otros objetos anonimos, otros `CreatedResult` con body `null`, otros `NoContent`.

### Ubicacion

- `PracticaJWTcore/Controllers/UsuariosController.cs`
- `PracticaJWTcore/Controllers/ServiceController.cs`
- `PracticaJWTcore/Controllers/VentasController.cs`
- `PracticaJWTcore/Controllers/StockController.cs`

### Por que es un problema

El frontend tiene que manejar demasiadas formas de error/exito.

### Objetivo del ejercicio

Definir una respuesta consistente para errores esperados.

### Tarea practica

Crear un formato simple: `{ code, message }` para errores. Aplicarlo primero a `UsuariosController`.

### Pistas

Ya existe `ServiceResult<T>`; se puede extender o traducir en controller.

### Criterios para saber si esta bien resuelto

Errores de validacion devuelven siempre `400` con `{ code, message }`.

### Nivel de dificultad

Intermedio

### Prioridad

Media

### Tipo de mejora

Manejo de errores, Controllers, Integracion con frontend

## Ejercicio 9: Agregar validaciones reales a DTOs

### Problema detectado

Muchos DTOs no tienen `[Required]`, `[Range]`, `[EmailAddress]` ni limites de longitud, aunque `[ApiController]` podria validarlos automaticamente.

### Ubicacion

- `PracticaJWTcore/Dtos/CreateUsuariosDTO.cs`
- `PracticaJWTcore/Dtos/CreateCustomerDto.cs`
- `PracticaJWTcore/Dtos/RolesPermisoDTO.cs`
- `PracticaJWTcore/Dtos/Stock/StockMovimientoRequestDto.cs`

### Por que es un problema

Requests incompletos llegan a services/repositories y pueden fallar tarde o guardar datos malos.

### Objetivo del ejercicio

Rechazar requests invalidos antes de ejecutar logica de negocio.

### Tarea practica

Agregar validaciones a `CreateUsuariosDTO` y `CreateRoleDTO`. Probar payload vacio, email invalido y `RoleId = 0`.

### Pistas

Con `[ApiController]`, `ModelState` invalido responde `400` automaticamente.

### Criterios para saber si esta bien resuelto

Payload invalido no llega al repository y Swagger muestra reglas basicas.

### Nivel de dificultad

Basico

### Prioridad

Media

### Tipo de mejora

Validaciones, DTOs

## Ejercicio 10: Recuperar el patron Repository en servicios auxiliares

### Problema detectado

`CustomerModalServices`, `VehicleModalServices` y `ServiciosModalServices` acceden directo a `PracticaJWTcoreContext`, aunque el patron esperado es Controller -> Service -> Repository.

### Ubicacion

- `PracticaJWTcore/Services/CustomerModalServices.cs`
- `PracticaJWTcore/Services/VehicleModalServices.cs`
- `PracticaJWTcore/Services/ServiciosModalServices.cs`

### Por que es un problema

La capa service mezcla negocio y acceso a datos, y queda mas dificil testear.

### Objetivo del ejercicio

Mover esas consultas a repositories.

### Tarea practica

Elegir `ServiciosModalServices`: crear o ajustar repository para obtener `ServiciosModal` y dejar el service solo delegando.

### Pistas

No cambiar el endpoint `api/PaginaBase/servicios`.

### Criterios para saber si esta bien resuelto

El service no inyecta `PracticaJWTcoreContext`; sigue devolviendo el mismo JSON.

### Nivel de dificultad

Intermedio

### Prioridad

Media

### Tipo de mejora

Services, Repositories, Arquitectura backend

## Ejercicio 11: Limpiar duplicados reales en RolesPermisos

### Problema detectado

SQL Server tiene duplicados en `RolesPermisos` para `RoleId=1`, `ComponentsId=4`, `PermisoId=1..4`.

### Ubicacion

- Tabla `RolesPermisos`
- `docs/sql/roles-usuarios-permisos.sql`

### Por que es un problema

La tabla permite permisos repetidos y bloquea crear un indice unico seguro.

### Objetivo del ejercicio

Dejar una sola fila por `(RoleId, ComponentsId, PermisoId)`.

### Tarea practica

Escribir primero un `SELECT` que muestre duplicados. Luego preparar un script que conserve el menor `RolePermisoId` y elimine el resto.

### Pistas

Usar `ROW_NUMBER() OVER (PARTITION BY RoleId, ComponentsId, PermisoId ORDER BY RolePermisoId)`.

### Criterios para saber si esta bien resuelto

La consulta de duplicados devuelve cero filas y se puede crear indice unico.

### Nivel de dificultad

Intermedio

### Prioridad

Media

### Tipo de mejora

SQL Server, Seguridad de datos

## Ejercicio 12: Revisar actualizacion de ventas y stock

### Problema detectado

`UpdateVenta` cambia cabecera (`IdCliente`, `IdUsuario`, `MetodoPago`, `Estado`) pero no recalcula detalles ni stock. `DeleteVenta` si restaura stock.

### Ubicacion

- `PracticaJWTcore/Services/VentasService.cs`

### Por que es un problema

Puede quedar una venta modificada parcialmente sin coherencia si mas adelante se espera editar items.

### Objetivo del ejercicio

Definir contrato claro: actualizacion solo de cabecera o actualizacion completa con items y stock.

### Tarea practica

Documentar y validar el comportamiento actual. Si el endpoint solo actualiza cabecera, renombrar DTO internamente o validar que no se envien items.

### Pistas

No cambiar la ruta sin revisar frontend/Swagger.

### Criterios para saber si esta bien resuelto

El endpoint deja claro que actualiza y no permite estados ambiguos.

### Nivel de dificultad

Avanzado

### Prioridad

Media

### Tipo de mejora

Services, SQL Server, Integracion con frontend

## Prioridad Baja

## Ejercicio 13: Ordenar nombres y archivos raros sin romper contratos

### Problema detectado

Hay nombres inconsistentes o con typos: `Appoitment`, `AutenticacionRespository`, y un archivo extraño `PracticaJWTcore/Dtos/PracticaJWTcore.sln`.

### Ubicacion

- `PracticaJWTcore/Services/AppoitmentServices.cs`
- `PracticaJWTcore/Repositorios/AutenticacionRespository.cs`
- `PracticaJWTcore/Dtos/PracticaJWTcore.sln`

### Por que es un problema

Dificulta busquedas, onboarding y mantenimiento.

### Objetivo del ejercicio

Ordenar sin romper namespaces ni referencias publicas.

### Tarea practica

Primero eliminar o reubicar solo el archivo claramente incorrecto si no se compila ni se usa. Despues proponer renombres internos con cuidado.

### Pistas

No renombrar rutas o endpoints publicos en este ejercicio.

### Criterios para saber si esta bien resuelto

`dotnet build` y `dotnet test` siguen pasando.

### Nivel de dificultad

Basico

### Prioridad

Baja

### Tipo de mejora

Limpieza de codigo

## Ejercicio 14: Eliminar stored procedures obsoletos o documentarlos

### Problema detectado

SQL Server conserva `proc_CambioClave` y `PruebaMSSQL`. El backend actual cambia clave con EF SQL interpolado, no con `proc_CambioClave`.

### Ubicacion

- SQL Server procedures: `proc_CambioClave`, `PruebaMSSQL`
- `PracticaJWTcore/Repositorios/AutenticacionRespository.cs`

### Por que es un problema

Objetos obsoletos confunden y pueden ser usados por error.

### Objetivo del ejercicio

Decidir si cada procedure se usa, se documenta o se elimina.

### Tarea practica

Buscar referencias en codigo y SQL. Preparar un script de diagnostico con `OBJECT_DEFINITION`.

### Pistas

No hacer `DROP` directo; primero listar consumidores.

### Criterios para saber si esta bien resuelto

Cada procedure tiene estado: usado, obsoleto documentado o candidato a eliminacion.

### Nivel de dificultad

Basico

### Prioridad

Baja

### Tipo de mejora

SQL Server, Limpieza de codigo

## Ejercicio 15: Mejorar configuracion sin cambiar comportamiento local

### Problema detectado

`JWT:Key` y connection string estan en `appsettings.json`; CORS permite cualquier origen; JWT no valida issuer/audience.

### Ubicacion

- `PracticaJWTcore/Program.cs`
- `PracticaJWTcore/appsettings.json`

### Por que es un problema

Esta bien para desarrollo, pero es riesgoso como base para produccion.

### Objetivo del ejercicio

Separar configuracion local de configuracion sensible.

### Tarea practica

Agregar lectura segura desde variables de entorno o user-secrets y dejar valores dev solo para local.

### Pistas

No cambiar la connection string actual sin plan. Empezar por validar que `JWT:Key` exista y tenga longitud minima.

### Criterios para saber si esta bien resuelto

Si falta `JWT:Key`, la API falla al iniciar con mensaje claro. En local sigue funcionando.

### Nivel de dificultad

Intermedio

### Prioridad

Baja

### Tipo de mejora

Configuracion, Seguridad

## Ruta recomendada para resolver los ejercicios

1. Ejercicio 2: login invalido con `401`.
2. Ejercicio 6: `404` en vez de `500`.
3. Ejercicio 9: validaciones de DTOs.
4. Ejercicio 1: autorizacion por rol/permisos.
5. Ejercicio 3: cambio de clave usando identidad del token.
6. Ejercicio 4: ventas con usuario autenticado.
7. Ejercicio 5: proteger catalogos publicos.
8. Ejercicio 7: DTOs en endpoints historicos.
9. Ejercicio 8: respuestas HTTP consistentes.
10. Ejercicio 10: recuperar patron repository.
11. Ejercicio 11: limpiar duplicados SQL.
12. Ejercicio 12: definir actualizacion de ventas/stock.
13. Ejercicios 13-15: limpieza y configuracion.

## Como voy a validar mis respuestas

Cuando resuelvas uno o varios ejercicios, mostrame:

- Archivos modificados.
- Endpoints afectados.
- DTOs nuevos o cambiados.
- Scripts SQL si los hay.
- Resultado de `dotnet build`.
- Resultado de `dotnet test`.
- Ejemplos de request/response antes y despues.
- Si tocaste o no frontend.
- Si cambiaste tablas, indices o procedures.

Voy a revisar si:

- Mantiene `Controller -> Service -> Repository -> SQL Server`.
- No rompe contratos que consume el frontend.
- Devuelve status HTTP coherentes.
- Valida requests antes de guardar.
- Respeta autenticacion/autorizacion.
- No expone entidades o datos de mas.
- No introduce cambios destructivos en SQL Server.
- El ejercicio quedo listo o necesita otra vuelta.
