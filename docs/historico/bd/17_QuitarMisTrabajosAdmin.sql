-- =======================================================================
-- FIX: Quitar permiso MisTrabajos.Ver al Administrador (IdRol = 1)
-- =======================================================================
-- Contexto:
--   bd/10_ConfiguracionPermisosRecepcionMecanico.sql otorgaba MisTrabajos.Ver
--   al Admin (IdRol = 1) "para completitud". La pantalla Mis Trabajos es
--   exclusiva del Mecanico (IdRol = 3): el Admin no la ve ni la necesita.
-- Solucion:
--   1) Se elimino el grant al Admin en bd/10 (fuente del seed).
--   2) Este script limpia de forma idempotente las bases que ya lo aplicaron.
-- Idempotente: si no existe la fila, no borra nada.
-- =======================================================================
USE [ControlVehicular];
GO

DELETE rp
FROM RolPermisos rp
INNER JOIN Permisos p ON rp.IdPermiso = p.IdPermiso
WHERE rp.IdRol = 1          -- Administrador
  AND p.Nombre = 'MisTrabajos.Ver';
GO
