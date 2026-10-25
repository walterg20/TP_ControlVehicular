-- =============================================
-- 12_PermisosReportesPorRol.sql
-- Permisos granulares por reporte (spec: openspec/specs/reportes-rbac)
-- Matriz:
--   Admin (1)          -> todos los reportes
--   Recepcionista (2)  -> Ordenes, Operativo
--   Mecanico (3)       -> Reportes del mecanico (Historial / Tareas del dia)
-- Script idempotente: puede ejecutarse varias veces sin duplicar filas.
-- =============================================
USE [ControlVehicular];
GO

-- 1. Nuevos permisos
INSERT INTO Permisos (Nombre)
SELECT v.Nombre
FROM (VALUES ('Reporte.Ordenes.Ver'),
             ('Reporte.Operativo.Ver'),
             ('Reporte.Gerencial.Ver'),
             ('Reporte.Mecanico.Ver')) v(Nombre)
WHERE NOT EXISTS (SELECT 1 FROM Permisos p WHERE p.Nombre = v.Nombre);

-- 2. Administrador (IdRol = 1): todos los reportes
INSERT INTO RolPermisos (IdRol, IdPermiso)
SELECT 1, p.IdPermiso
FROM Permisos p
WHERE p.Nombre IN ('Reporte.Ordenes.Ver', 'Reporte.Operativo.Ver', 'Reporte.Gerencial.Ver', 'Reporte.Mecanico.Ver')
  AND NOT EXISTS (SELECT 1 FROM RolPermisos rp WHERE rp.IdRol = 1 AND rp.IdPermiso = p.IdPermiso);

-- 3. Recepcionista (IdRol = 2): Ordenes + Operativo; se quita el permiso generico Reporte.Ver
DELETE rp
FROM RolPermisos rp
INNER JOIN Permisos p ON p.IdPermiso = rp.IdPermiso
WHERE rp.IdRol = 2 AND p.Nombre = 'Reporte.Ver';

INSERT INTO RolPermisos (IdRol, IdPermiso)
SELECT 2, p.IdPermiso
FROM Permisos p
WHERE p.Nombre IN ('Reporte.Ordenes.Ver', 'Reporte.Operativo.Ver')
  AND NOT EXISTS (SELECT 1 FROM RolPermisos rp WHERE rp.IdRol = 2 AND rp.IdPermiso = p.IdPermiso);

-- 4. Mecanico (IdRol = 3): reportes del mecanico
INSERT INTO RolPermisos (IdRol, IdPermiso)
SELECT 3, p.IdPermiso
FROM Permisos p
WHERE p.Nombre = 'Reporte.Mecanico.Ver'
  AND NOT EXISTS (SELECT 1 FROM RolPermisos rp WHERE rp.IdRol = 3 AND rp.IdPermiso = p.IdPermiso);
GO
