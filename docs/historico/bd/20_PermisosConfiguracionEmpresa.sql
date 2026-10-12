-- =============================================
-- 20_PermisosConfiguracionEmpresa.sql
-- Permiso de gestion de la identidad de empresa (logo + datos).
-- Spec: openspec/changes/2026-10-10-logo-empresa
-- Matriz:
--   Admin (IdRol = 1)          -> Configuracion.Empresa.Gestionar
--   Recepcionista (2), Mecanico (3) -> sin acceso
-- Script idempotente: puede ejecutarse varias veces sin duplicar filas.
-- =============================================
USE [ControlVehicular];
GO

-- 1. Nuevo permiso
INSERT INTO Permisos (Nombre)
SELECT v.Nombre
FROM (VALUES ('Configuracion.Empresa.Gestionar')) v(Nombre)
WHERE NOT EXISTS (SELECT 1 FROM Permisos p WHERE p.Nombre = v.Nombre);
GO

-- 2. Administrador (IdRol = 1)
INSERT INTO RolPermisos (IdRol, IdPermiso)
SELECT 1, p.IdPermiso
FROM Permisos p
WHERE p.Nombre = 'Configuracion.Empresa.Gestionar'
  AND NOT EXISTS (SELECT 1 FROM RolPermisos rp WHERE rp.IdRol = 1 AND rp.IdPermiso = p.IdPermiso);
GO
