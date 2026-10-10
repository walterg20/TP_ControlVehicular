-- =============================================
-- 09_PermisosAvanzados.sql
-- Creación de tablas para el sistema RBAC
-- =============================================
USE [ControlVehicular_DB];
GO

-- 1. Crear tabla Permisos
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Permisos]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Permisos](
        [IdPermiso] [int] IDENTITY(1,1) NOT NULL,
        [Nombre] [nvarchar](100) NOT NULL,
        CONSTRAINT [PK_Permisos] PRIMARY KEY CLUSTERED ([IdPermiso] ASC)
    );
END
GO

-- 2. Crear tabla intermedia RolPermisos
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RolPermisos]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[RolPermisos](
        [IdRol] [int] NOT NULL,
        [IdPermiso] [int] NOT NULL,
        CONSTRAINT [PK_RolPermisos] PRIMARY KEY CLUSTERED ([IdRol] ASC, [IdPermiso] ASC),
        CONSTRAINT [FK_RolPermisos_Roles] FOREIGN KEY([IdRol]) REFERENCES [dbo].[Roles] ([IdRol]) ON DELETE CASCADE,
        CONSTRAINT [FK_RolPermisos_Permisos] FOREIGN KEY([IdPermiso]) REFERENCES [dbo].[Permisos] ([IdPermiso]) ON DELETE CASCADE
    );
END
GO

-- 3. Insertar Permisos Semilla
SET IDENTITY_INSERT [dbo].[Permisos] ON;
INSERT INTO [dbo].[Permisos] ([IdPermiso], [Nombre])
SELECT IdPermiso, Nombre FROM (
    VALUES 
    (1, 'Cliente.Ver'), (2, 'Cliente.Crear'), (3, 'Cliente.Editar'), (4, 'Cliente.Eliminar'),
    (5, 'Vehiculo.Ver'), (6, 'Vehiculo.Crear'), (7, 'Vehiculo.Editar'), (8, 'Vehiculo.Eliminar'),
    (9, 'Servicio.Ver'), (10, 'Servicio.Crear'), (11, 'Servicio.Editar'), (12, 'Servicio.Eliminar'),
    (13, 'Usuario.Ver'), (14, 'Usuario.Crear'), (15, 'Usuario.Editar'), (16, 'Usuario.Eliminar'),
    (17, 'OrdenServicio.Ver'), (18, 'OrdenServicio.Crear'), (19, 'OrdenServicio.Editar'), (20, 'OrdenServicio.Eliminar'),
    (21, 'Taller.Ver'), (22, 'Taller.Editar'),
    (23, 'Reporte.Ver')
) AS Source(IdPermiso, Nombre)
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Permisos] P WHERE P.IdPermiso = Source.IdPermiso);
SET IDENTITY_INSERT [dbo].[Permisos] OFF;
GO

-- 4. Asignar todos los permisos al rol de Administrador (Asumiendo IdRol = 1)
INSERT INTO [dbo].[RolPermisos] ([IdRol], [IdPermiso])
SELECT 1, IdPermiso 
FROM [dbo].[Permisos]
WHERE NOT EXISTS (
    SELECT 1 FROM [dbo].[RolPermisos] RP 
    WHERE RP.IdRol = 1 AND RP.IdPermiso = [dbo].[Permisos].IdPermiso
);
GO