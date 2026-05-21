# AGENTS.md

## Regla Obligatoria De Trabajo Seguro

Antes de editar archivos, crear archivos, borrar archivos, cambiar configuracion, modificar base de datos, ejecutar migraciones, instalar paquetes, hacer cambios destructivos o cambiar arquitectura, primero se debe:

1. Inspeccionar el contexto.
2. Mostrar un plan.
3. Esperar aprobacion explicita del usuario.

La unica aprobacion valida debe ser exactamente:

```text
y
```

No aplicar cambios hasta que el usuario responda exactamente:

```text
y
```

## Contenido Obligatorio Del Plan

El plan debe incluir:

1. Resumen del problema.
2. Hallazgos.
3. Cambios propuestos.
4. Archivos, tablas, procedimientos, endpoints, componentes o reportes afectados.
5. Riesgos.
6. Plan de verificacion.
7. Pregunta final de aprobacion.

## Respeto De Tecnologia Existente

Si el usuario esta trabajando con una tecnologia especifica, se debe respetar esa tecnologia:

- Si es WebForms, no convertir a MVC, Razor, Blazor, React ni ASP.NET Core.
- Si es ADO.NET, no convertir a EF Core, Dapper ni otro ORM.
- Si es VB.NET, no convertir a C#.
- Si es ReportViewer/SSRS/RDLC, revisar diseno del reporte y dataset antes de tocar SQL.
- Si es React, revisar componentes, hooks y services antes de cambiar contratos de API.
