-- =============================================
-- 22_PermisosBackup.sql
-- Permiso de gestion de respaldos de base de datos y resguardo en Google Drive.
-- Spec: openspec/changes/2026-10-11-backup-drive
-- Matriz:
--   Admin (IdRol = 1)          -> Configuracion.Backup.Gestionar
--   Recepcionista (2), Mecanico (3) -> sin acceso
-- Script idempotente: puede ejecutarse varias veces sin duplicar filas.
-- =============================================
USE [ControlVehicular];
GO

-- 1. Nuevo permiso
INSERT INTO Permisos (Nombre)
SELECT v.Nombre
FROM (VALUES ('Configuracion.Backup.Gestionar')) v(Nombre)
WHERE NOT EXISTS (SELECT 1 FROM Permisos p WHERE p.Nombre = v.Nombre);
GO

-- 2. Administrador (IdRol = 1)
INSERT INTO RolPermisos (IdRol, IdPermiso)
SELECT 1, p.IdPermiso
FROM Permisos p
WHERE p.Nombre = 'Configuracion.Backup.Gestionar'
  AND NOT EXISTS (SELECT 1 FROM RolPermisos rp WHERE rp.IdRol = 1 AND rp.IdPermiso = p.IdPermiso);
GO
