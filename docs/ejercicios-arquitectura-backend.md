# Ejercicios Para Aprender Arquitectura Backend

Guia practica basada en `arquitectura-backend-actualizada.pdf` y en el backend actual de `ApiProyectoLavanderia`.

Objetivo: ayudarte a entender la arquitectura del backend practicando con ejercicios manuales, sin romper contratos, endpoints, base de datos ni frontend.

> Recomendacion: antes de tocar codigo, lee el ejercicio completo, ubica los archivos mencionados y escribe en tus propias palabras que capa deberia resolver cada parte.

## Como usar esta guia

Para cada ejercicio:

1. Lee el objetivo.
2. Abre los archivos indicados.
3. Responde primero en papel o en tus notas.
4. Revisa las pistas.
5. Implementa solo si entendes que contrato estas preservando.
6. Ejecuta las verificaciones sugeridas.

Regla importante:

```text
Controller -> Service -> Repository -> DbContext / SQL Server
```

Si una solucion rompe ese flujo sin una razon clara, probablemente estas mezclando responsabilidades.

## Nivel 1: Lectura Arquitectonica

### Ejercicio 1: Identificar Responsabilidades Por Capa

Objetivo: reconocer que hace cada capa en un flujo real.

Archivos a leer:

- `PracticaJWTcore/Controllers/VentasController.cs`
- `PracticaJWTcore/Services/VentasService.cs`
- `PracticaJWTcore/Repositorios/VentasRepository.cs`
- `PracticaJWTcore/Dtos/Ventas/CreateVentaDto.cs`
- `PracticaJWTcore/Models/Venta.cs`

Problema del PDF que trabaja: entender el flujo `Controller -> Service -> Repository`.

Tarea manual:

- Escribi una tabla con 5 filas: controller, service, repository, DTO, model.
- Para cada fila, anota una responsabilidad real encontrada en el codigo.
- Marca que cosas NO deberia hacer esa capa.

Pistas:

- Si ves `BadRequest`, `Ok`, `Created` o `NotFound`, estas mirando responsabilidad HTTP.
- Si ves validaciones como stock suficiente o cantidad positiva, estas mirando negocio.
- Si ves `_context`, `DbSet`, `ToListAsync` o `SaveChangesAsync`, estas mirando acceso a datos.

Como te ayuda:

Te obliga a distinguir entre codigo que habla HTTP, codigo que decide reglas y codigo que consulta la base.

Verificacion:

- Tu tabla deberia poder explicar por que `VentasController` es mas liviano que antes.

Espacio para tus notas:

```text

```

### Ejercicio 2: Dibujar El Flujo De Crear Venta

Objetivo: entender paso a paso que pasa cuando el frontend llama a `POST /api/Ventas`.

Archivos a leer:

- `VentasController.cs`
- `VentasService.cs`
- `VentasRepository.cs`
- `CreateVentaDto.cs`
- `VentaResponseDto.cs`

Tarea manual:

- Dibuja el camino completo desde request hasta response.
- Inclui validacion de items, stock, transaccion, detalle, movimiento y response.

Pistas:

- Busca el metodo `CreateVenta`.
- Busca donde se llama `ExecuteInTransaction`.
- Busca donde se modifica `articulo.StockActual`.
- Busca donde se crea `StockMovimiento`.

Como te ayuda:

Ventas es el modulo mas completo para aprender arquitectura por capas porque combina validacion, calculos, transaccion y persistencia.

Verificacion:

- Tu dibujo deberia poder responder: "en que capa se calcula el total?" y "en que capa se guarda la venta?".

Espacio para tus notas:

```text

```

### Ejercicio 3: Detectar Entidades EF Expuestas

Objetivo: aprender por que no siempre conviene usar entidades como request/response.

Archivos a leer:

- `PracticaJWTcore/Controllers/ServiceController.cs`
- `PracticaJWTcore/Controllers/CustomerController.cs`
- `PracticaJWTcore/Controllers/ArticulosController.cs`
- `PracticaJWTcore/Models/Service.cs`
- `PracticaJWTcore/Dtos/Articulos/ArticuloRequestDto.cs`

Tarea manual:

- Hace una lista de endpoints que reciben o devuelven entidades.
- Hace otra lista de endpoints que usan DTOs.
- Escribi cual te parece mas seguro para mantener contratos.

Pistas:

- Si el controller recibe `[FromBody] Service`, esta usando una entidad como contrato.
- Si recibe `[FromBody] ArticuloRequestDto`, esta usando un DTO.
- Si devuelve `ArticuloResponseDto`, el response esta desacoplado del modelo EF.

Como te ayuda:

Te muestra por que los DTOs protegen al frontend de cambios internos en la base o entidades.

Verificacion:

- Deberias identificar que `ArticulosController` esta mas separado que `ServiceController`.

Espacio para tus notas:

```text

```

## Nivel 2: Refactor Seguro En Papel

### Ejercicio 4: Proponer Un DTO Para Service Sin Implementarlo

Objetivo: practicar diseno de contrato sin romper el frontend.

Archivos a leer:

- `PracticaJWTcore/Controllers/ServiceController.cs`
- `PracticaJWTcore/Models/Service.cs`
- `BlazorApp1/Pages/Services.razor`
- `BlazorApp1/Pages/ServicesCreacion.razor`
- `BlazorApp1/Modelos/Service.cs`

Tarea manual:

- Disena en tus notas un `ServiceRequestDto`.
- Disena un `ServiceResponseDto`.
- Compara sus propiedades con lo que consume `BlazorApp1`.
- Marca que nombres no deberias cambiar.

Pistas:

- Primero mira el modelo del frontend.
- No inventes nombres nuevos si el frontend ya espera otros.
- Preguntate si el frontend necesita todas las propiedades de la entidad.

Como te ayuda:

Practicas la regla mas importante de refactor seguro: antes de cambiar un DTO, revisa quien lo consume.

Verificacion:

- Tu DTO propuesto deberia permitir que `Services.razor` siga funcionando con cambios minimos o sin cambios.

Espacio para tus notas:

```text

```

### Ejercicio 5: Separar Error Esperado De Error Inesperado

Objetivo: entender el manejo de errores por capas.

Archivos a leer:

- `PracticaJWTcore/Services/ServiceResult.cs`
- `PracticaJWTcore/Services/ArticulosService.cs`
- `PracticaJWTcore/Controllers/ArticulosController.cs`
- `PracticaJWTcore/Controllers/UsuariosController.cs`

Tarea manual:

- Identifica tres errores esperados en `ArticulosService`.
- Identifica un error inesperado posible en `UsuariosController` o `UsuarioRepository`.
- Escribi que status HTTP corresponde a cada caso.

Pistas:

- `ARTICLE_NOT_FOUND` suele corresponder a `404`.
- `INVALID_PRICE` suele corresponder a `400`.
- Una excepcion no controlada suele terminar en `500`.

Como te ayuda:

Te muestra la diferencia entre reglas de negocio que podes responder prolijamente y fallas internas que deberian loguearse.

Verificacion:

- Deberias poder explicar por que `ServiceResult` ayuda a que el controller sea simple.

Espacio para tus notas:

```text

```

### Ejercicio 6: Revisar Un Controller Liviano

Objetivo: aprender a reconocer cuando un controller esta bien enfocado.

Archivos a leer:

- `PracticaJWTcore/Controllers/StockController.cs`
- `PracticaJWTcore/Services/StockService.cs`

Tarea manual:

- Marca todas las lineas del controller que tengan responsabilidad HTTP.
- Marca si hay alguna regla de negocio dentro del controller.
- Escribi que pasaria si la validacion de cantidad estuviera en el controller.

Pistas:

- Un controller liviano recibe, llama al service y traduce resultado a HTTP.
- La frase "cantidad debe ser mayor a cero" es negocio, no HTTP.

Como te ayuda:

Te entrena para detectar controllers que empiezan a crecer de mas.

Verificacion:

- Si casi todo el controller son llamadas al service y `Ok/BadRequest/NotFound`, vas bien.

Espacio para tus notas:

```text

```

## Nivel 3: Practica Con Cambios Pequeños

> Estos ejercicios implican tocar codigo si decidis hacerlos. Hacelos de a uno, ejecuta build despues de cada uno y no mezcles refactor con feature.

### Ejercicio 7: Agregar Un Caso De Validacion A Articulos

Objetivo: practicar donde poner una regla de negocio.

Idea de cambio:

- Validar que `StockMinimo` no sea mayor que `StockActual`, si esa regla tiene sentido para tu negocio.

Archivos a mirar:

- `ArticulosService.cs`
- `ArticuloRequestDto.cs`
- `ArticulosController.cs`
- `ArticulosServiceTests.cs`

Donde deberia vivir:

- La validacion deberia ir en `ArticulosService`, no en `ArticulosController`.

Pistas:

- Busca el metodo `ValidateArticulo`.
- Segui el estilo de `INVALID_PRICE` o `INVALID_STOCK`.
- Si agregas un codigo, mantenelo claro, por ejemplo `INVALID_MIN_STOCK`.

Como ayuda al codigo:

Centraliza la regla y evita que cualquier endpoint futuro cree articulos invalidos saltandose el controller actual.

Verificacion:

```text
dotnet build PracticaJWTcore.sln --no-restore
dotnet test PracticaJWTcore.sln --no-build
```

Espacio para tus notas:

```text

```

### Ejercicio 8: Crear Una Prueba Para Stock Manual

Objetivo: entender comportamiento actual antes de cambiarlo.

Idea:

- Escribir una prueba que confirme que `StockService.CreateMovimiento` crea un movimiento pero no actualiza `Articulos.StockActual`.

Archivos a mirar:

- `StockService.cs`
- `StockServiceTests.cs`
- `IStockRepository.cs`

Pistas:

- Usa mocks del repository.
- Verifica que se llame `AddMovimiento`.
- No esperes que se modifique el articulo salvo que decidas cambiar comportamiento.

Como ayuda al codigo:

Documenta con test una decision importante: movimiento manual no equivale automaticamente a ajuste de stock actual.

Verificacion:

- El test deberia fallar si alguien cambia esa regla sin querer.

Espacio para tus notas:

```text

```

### Ejercicio 9: Normalizar Un Error Sin Romper Contrato

Objetivo: practicar mejoras de errores con bajo riesgo.

Idea:

- Elegir un endpoint nuevo, por ejemplo `POST /api/Articulos`, y pensar como devolver `{ message, code }` sin romper frontend.

Archivos a mirar:

- `ArticulosController.cs`
- `ArticulosService.cs`
- `ServiceResult.cs`

Tarea manual antes de tocar:

- Buscar si el frontend consume `api/Articulos`.
- Si no lo consume, el riesgo es menor.
- Si lo consume, revisar si espera texto plano.

Pistas:

- No cambies todos los controllers de una vez.
- Podrias crear una respuesta de error local primero.
- El codigo ya tiene `result.Message` y `result.Code`.

Como ayuda al codigo:

Hace que los errores sean mas faciles de manejar desde frontend y mas faciles de testear.

Verificacion:

- Swagger deberia mostrar el endpoint igual.
- Build y tests deben pasar.

Espacio para tus notas:

```text

```

## Nivel 4: Seguridad Y Contratos

### Ejercicio 10: Mapa De Endpoints Publicos Y Privados

Objetivo: entender seguridad sin bloquear pantallas por accidente.

Archivos a leer:

- `Program.cs`
- `AutenticacionController.cs`
- `TestController.cs`
- `BlazorApp1/Pages/Login/Login.razor`
- `BlazorApp1/Pages/Services.razor`
- `BlazorApp1/Pages/PagesAppoiment/Pedidos.razor`

Tarea manual:

- Hace una tabla con endpoints publicos y privados.
- Marca cuales reciben token desde frontend.
- Marca donde podrias poner `[Authorize]` primero con menor riesgo.

Pistas:

- Login debe ser publico.
- Test puede ser publico.
- Si una pantalla ya envia token, probablemente tolera `[Authorize]`.
- Si una pantalla no envia token, bloquearla puede romper el flujo.

Como te ayuda:

Aprendes que seguridad no es solo agregar atributos: tambien es revisar consumidores.

Verificacion:

- Tu tabla deberia explicar por que no conviene poner `[Authorize]` global sin revisar Blazor.

Espacio para tus notas:

```text

```

### Ejercicio 11: Entender El Token

Objetivo: comprender que contiene el JWT y como se usa.

Archivos a leer:

- `AutenticacionRespository.cs`
- `TokenRolDTO.cs`
- `BlazorApp1/Pages/Login/Login.razor`
- `BlazorApp1/Modelos/TokenResponse.cs`

Tarea manual:

- Escribi que datos contiene el token.
- Escribi que datos devuelve el endpoint de login.
- Compara `tokenRol` del backend con `TokenResponse` del frontend.

Pistas:

- Busca `GenerateToken`.
- Busca `ClaimTypes.Role`.
- Busca `ClaimTypes.NameIdentifier`.
- Mira si el frontend lee `token`, `tokenRol` o una estructura diferente.

Como te ayuda:

Te muestra una posible zona de inconsistencia entre contrato backend y modelo frontend.

Verificacion:

- Deberias poder explicar que parte es JWT y que parte es wrapper del response.

Espacio para tus notas:

```text

```

## Nivel 5: SQL Server Y Persistencia

### Ejercicio 12: Relacionar DbSet Con Tabla

Objetivo: entender como EF Core conecta modelos con SQL Server.

Archivos a leer:

- `PracticaJWTcore/Models/PracticaJWTcoreContext.cs`
- `PracticaJWTcore/Models/Venta.cs`
- `PracticaJWTcore/Models/VentaDetalle.cs`
- `PracticaJWTcore/Models/StockMovimiento.cs`
- `PracticaJWTcore/Models/Articulos.cs`

Tarea manual:

- Hace una tabla con `DbSet`, entidad y tabla SQL.
- Marca que columnas se configuran con `HasColumnName`.
- Marca relaciones importantes.

Pistas:

- Busca `DbSet<`.
- Busca `ToTable`.
- Busca `HasOne`.
- Busca `HasColumnType`.

Como te ayuda:

Entendes que cambiar una entidad puede afectar el contrato SQL aunque no toques la base.

Verificacion:

- Tu tabla deberia incluir `Ventas`, `VentaDetalles`, `Articulos`, `StockMovimientos`.

Espacio para tus notas:

```text

```

### Ejercicio 13: ADO.NET Vs EF Core

Objetivo: distinguir dos formas de acceso a datos en el mismo proyecto.

Archivos a leer:

- `AutenticacionRespository.cs`
- `UsuarioRepository.cs`
- `VentasRepository.cs`

Tarea manual:

- Marca que metodos usan `SqlConnection`.
- Marca que metodos usan EF Core.
- Escribi por que puede existir mezcla de ambos.

Pistas:

- `SqlCommand` normalmente indica ADO.NET.
- `_context.Usuarios.Where(...)` indica EF Core.
- Un stored procedure suele estar detras de ADO.NET en este proyecto.

Como te ayuda:

Te prepara para no querer convertir todo automaticamente a una sola tecnologia sin entender impacto.

Verificacion:

- Deberias identificar `proc_ComparePass`, `proc_CambioClave` y `proc_InsertUsuario`.

Espacio para tus notas:

```text

```

## Nivel 6: Diseño De Mejoras Futuras

### Ejercicio 14: Planear Un Middleware Global De Errores

Objetivo: aprender a planificar antes de implementar.

No implementes todavia. Primero diseña.

Tarea manual:

- Escribi que errores deberia manejar el middleware.
- Escribi que errores NO deberia reemplazar.
- Escribi que deberia loguear.
- Escribi que formato de respuesta usarias.

Pistas:

- Errores esperados de negocio ya salen de `ServiceResult`.
- El middleware deberia cubrir errores inesperados.
- En produccion no conviene mostrar detalles internos.

Como ayuda al codigo:

Evita repetir try/catch en controllers y mejora la consistencia de respuestas.

Verificacion:

- Tu plan no deberia cambiar `BadRequest` esperados de ventas, stock y articulos.

Espacio para tus notas:

```text

```

### Ejercicio 15: Diseñar Repositories Para Modales

Objetivo: practicar consistencia de capas.

Archivos a leer:

- `PaginaBaseController.cs`
- `VehicleModalServices.cs`
- `CustomerModalServices.cs`
- `ServiciosModalServices.cs`

Tarea manual:

- Diseña interfaces de repository para vehicle/customer/servicios modal.
- Escribi que queries vivirian ahi.
- Escribi como quedaria el service despues.

Pistas:

- Hoy esos services acceden directo a `PracticaJWTcoreContext`.
- No hace falta cambiar endpoints.
- La ruta publica deberia quedar igual.

Como ayuda al codigo:

Hace mas consistente el flujo de capas y facilita testear services sin EF real.

Verificacion:

- Tu propuesta deberia mantener `PaginaBaseController` igual de liviano.

Espacio para tus notas:

```text

```

### Ejercicio 16: Reducir Objetos Anonimos

Objetivo: entender versionado de responses.

Archivos a leer:

- `AutenticacionController.cs`
- `UsuariosController.cs`
- `TokenRolDTO.cs`

Tarea manual:

- Lista las respuestas anonimas.
- Propone un DTO para cada una.
- Marca el riesgo de cambiar el JSON.

Pistas:

- `{ tokenRol = response }` es objeto anonimo.
- `{ mensaje = "..." }` es objeto anonimo.
- Aunque crear DTO sea mas limpio, cambiar nombres puede romper frontend.

Como ayuda al codigo:

Responses con DTOs son mas faciles de documentar, testear y evolucionar.

Verificacion:

- Tu propuesta debe conservar nombres JSON actuales si el frontend los consume.

Espacio para tus notas:

```text

```

## Checklist Antes De Implementar Cualquier Ejercicio

- Revise el controller.
- Revise el service.
- Revise el repository.
- Revise el DTO.
- Revise el model y `DbContext`.
- Revise si `BlazorApp1` consume el endpoint.
- No cambie rutas publicas.
- No cambie nombres de propiedades sin revisar frontend.
- No cambie tablas, columnas ni stored procedures.
- Ejecute build despues.
- Si hay tests relacionados, ejecutelos.

## Comandos De Verificacion

```powershell
dotnet build PracticaJWTcore.sln --no-restore
```

```powershell
dotnet test PracticaJWTcore.sln --no-build
```

Para buscar consumidores frontend:

```powershell
rg -n "api/NombreEndpoint|GetFromJsonAsync|PostAsJsonAsync|PutAsJsonAsync|DeleteAsync" BlazorApp1
```

Para buscar acceso directo a DbContext:

```powershell
rg -n "PracticaJWTcoreContext|_context|_db" PracticaJWTcore/Controllers PracticaJWTcore/Services
```

Para buscar entidades usadas como contrato:

```powershell
rg -n "\\[FromBody\\] (Service|Customer|Articulos|Venta|StockMovimiento)" PracticaJWTcore/Controllers
```

## Ruta De Aprendizaje Recomendada

1. Hace los ejercicios 1, 2 y 3 sin tocar codigo.
2. Hace los ejercicios 4, 5 y 6 escribiendo notas y mini diagramas.
3. Elegi solo uno entre los ejercicios 7, 8 o 9 para implementar.
4. Despues estudia seguridad con los ejercicios 10 y 11.
5. Termina con SQL y persistencia: ejercicios 12 y 13.
6. Usa los ejercicios 14, 15 y 16 como plan de refactor futuro.

## Autoevaluacion

Despues de completar varios ejercicios, deberias poder responder:

- Que responsabilidad tiene un controller?
- Que responsabilidad tiene un service?
- Que responsabilidad tiene un repository?
- Por que un DTO protege el contrato con frontend?
- Donde va una regla de negocio?
- Donde va una query EF Core?
- Por que no conviene exponer entidades EF en todos los endpoints?
- Que riesgo tiene agregar `[Authorize]` sin revisar Blazor?
- Que diferencia hay entre error esperado y excepcion inesperada?
- Que partes del backend actual estan mas ordenadas?
- Que partes siguen siendo deuda tecnica?

