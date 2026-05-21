/*
Script de diagnostico y hardening para usuarios, roles y permisos.

Ejecutado en esta tarea:
- DROP PROCEDURE dbo.proc_InsertUsuario, porque el backend crea usuarios con EF Core
  y guarda la clave con Microsoft.AspNetCore.Identity.PasswordHasher.

No ejecutar constraints/indices si los diagnosticos devuelven filas.
*/

SET NOCOUNT ON;

/* Diagnostico: roles duplicados */
SELECT RoleName, COUNT(*) AS Cantidad
FROM dbo.Roles
GROUP BY RoleName
HAVING COUNT(*) > 1;

/* Diagnostico: permisos duplicados por rol/componente */
SELECT RoleId, ComponentsId, PermisoId, COUNT(*) AS Cantidad
FROM dbo.RolesPermisos
GROUP BY RoleId, ComponentsId, PermisoId
HAVING COUNT(*) > 1;

/* Diagnostico: usuarios con RoleId invalido */
SELECT u.*
FROM dbo.Usuarios u
LEFT JOIN dbo.Roles r ON r.RoleId = u.RoleId
WHERE u.RoleId IS NOT NULL
  AND r.RoleId IS NULL;

/* Diagnostico: confirmar si existe doble modelo UsuariosRoles */
SELECT s.name AS SchemaName, t.name AS TableName
FROM sys.tables t
JOIN sys.schemas s ON s.schema_id = t.schema_id
WHERE t.name = 'UsuariosRoles';

/* Ejecutado: eliminar procedure obsoleto */
IF OBJECT_ID('dbo.proc_InsertUsuario', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.proc_InsertUsuario;
END;

/* Hardening seguro: crear indice unico solo si no hay duplicados */
IF NOT EXISTS (
    SELECT 1
    FROM dbo.RolesPermisos
    GROUP BY RoleId, ComponentsId, PermisoId
    HAVING COUNT(*) > 1
)
AND NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_RolesPermisos_Role_Component_Permiso'
      AND object_id = OBJECT_ID('dbo.RolesPermisos')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_RolesPermisos_Role_Component_Permiso
    ON dbo.RolesPermisos(RoleId, ComponentsId, PermisoId);
END;

/* Indices no unicos de apoyo */
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_Usuarios_RoleId'
      AND object_id = OBJECT_ID('dbo.Usuarios')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_Usuarios_RoleId ON dbo.Usuarios(RoleId);
END;

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_RolesPermisos_Role_Component'
      AND object_id = OBJECT_ID('dbo.RolesPermisos')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_RolesPermisos_Role_Component
    ON dbo.RolesPermisos(RoleId, ComponentsId);
END;
