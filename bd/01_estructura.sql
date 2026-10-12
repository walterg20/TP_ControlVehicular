-- ============================================================================
-- 01_estructura.sql — Esquema base (DDL) del sistema de control vehicular.
-- ----------------------------------------------------------------------------
-- Autor: consolidación de scripts históricos (2026-10-12).
-- Contenido: secuencia de facturación + 18 tablas + índices + CK/UNIQUE/FK.
-- Idempotente: cada objeto se crea solo si no existe (puede re-ejecutarse).
-- Requiere: base de datos [ControlVehicular] ya creada.
-- Compatible con SQL Server 2019 Express. Codificación UTF-8.
-- Orden de aplicación: 01 -> 02 -> 03 -> 04.
-- ============================================================================
USE [ControlVehicular];
GO

-- ----------------------------------------------------------------------------
-- Secuencia para el número consecutivo de factura (usada por [Factura].[Numero]).
-- Debe existir antes de crear la tabla [Factura].
-- ----------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.sequences WHERE name = N'Seq_Factura_Numero' AND schema_id = SCHEMA_ID(N'dbo'))
BEGIN
    CREATE SEQUENCE [dbo].[Seq_Factura_Numero] AS INT START WITH 1 INCREMENT BY 1;
END
GO

-- ============================================================================
-- 1. Tablas
-- ============================================================================

-- ----------------------------------------------------------------------------
-- Cliente
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[Cliente]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Cliente] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(100) NOT NULL,
        [Apellido] nvarchar(100) NOT NULL,
        [Dni] nvarchar(8) NOT NULL,
        [FechaNacimiento] datetime2 NOT NULL,
        [Direccion] nvarchar(200) NOT NULL,
        [Email] nvarchar(max) NOT NULL,
        [Telefono] nvarchar(max) NOT NULL,
        [Activo] bit NOT NULL,
        CONSTRAINT [PK_Cliente] PRIMARY KEY ([Id])
    );
END
GO

-- ----------------------------------------------------------------------------
-- Marca
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[Marca]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Marca] (
        [Id] int NOT NULL IDENTITY,
        [NombreMarca] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_Marca] PRIMARY KEY ([Id])
    );
END
GO

-- ----------------------------------------------------------------------------
-- Roles
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[Roles]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Roles] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(max) NOT NULL,
        [Descripcion] nvarchar(max) NOT NULL,
        [Estado] bit NOT NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
    );
END
GO

-- ----------------------------------------------------------------------------
-- Servicio
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[Servicio]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Servicio] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(150) NOT NULL,
        [Precio] decimal(18,2) NOT NULL,
        [Activo] bit NOT NULL,
        CONSTRAINT [PK_Servicio] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Servicio_Precio] CHECK ([Precio] > 0)
    );
END
GO

-- ----------------------------------------------------------------------------
-- Taller
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[Taller]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Taller] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(100) NOT NULL,
        [Direccion] nvarchar(200) NOT NULL,
        [Telefono] nvarchar(20) NOT NULL,
        [Activo] bit NOT NULL,
        CONSTRAINT [PK_Taller] PRIMARY KEY ([Id])
    );
END
GO

-- ----------------------------------------------------------------------------
-- MetodoPago
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[MetodoPago]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[MetodoPago] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_MetodoPago] PRIMARY KEY ([Id])
    );
END
GO

-- ----------------------------------------------------------------------------
-- Modelo
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[Modelo]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Modelo] (
        [Id] int NOT NULL IDENTITY,
        [MarcaId] int NOT NULL,
        [NombreModelo] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_Modelo] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Modelo_Marca_MarcaId] FOREIGN KEY ([MarcaId]) REFERENCES [dbo].[Marca] ([Id]) ON DELETE CASCADE
    );
END
GO

-- ----------------------------------------------------------------------------
-- Permisos (RBAC)
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[Permisos]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Permisos] (
        [IdPermiso] int NOT NULL IDENTITY,
        [Nombre] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_Permisos] PRIMARY KEY CLUSTERED ([IdPermiso] ASC)
    );
END
GO

-- ----------------------------------------------------------------------------
-- Usuarios
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[Usuarios]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Usuarios] (
        [Id] int NOT NULL IDENTITY,
        [RolId] int NOT NULL,
        [Nombre] nvarchar(100) NOT NULL,
        [Apellido] nvarchar(100) NOT NULL,
        [Dni] nvarchar(15) NOT NULL,
        [Email] nvarchar(100) NOT NULL,
        [Telefono] nvarchar(20) NOT NULL,
        [Domicilio] nvarchar(200) NOT NULL,
        [FechaNacimiento] datetime2 NOT NULL,
        [Contrasena] nvarchar(max) NOT NULL,
        [Estado] bit NOT NULL,
        CONSTRAINT [PK_Usuarios] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Usuarios_Roles_RolId] FOREIGN KEY ([RolId]) REFERENCES [dbo].[Roles] ([Id]) ON DELETE NO ACTION
    );
END
GO

-- ----------------------------------------------------------------------------
-- Vehiculo
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[Vehiculo]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Vehiculo] (
        [Id] int NOT NULL IDENTITY,
        [ModeloId] int NOT NULL,
        [Anio] int NOT NULL,
        [Patente] nvarchar(15) NOT NULL,
        [KmActual] int NOT NULL,
        CONSTRAINT [PK_Vehiculo] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Vehiculo_Modelo_ModeloId] FOREIGN KEY ([ModeloId]) REFERENCES [dbo].[Modelo] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [CK_Vehiculo_KmActual] CHECK ([KmActual] >= 0)
    );
END
GO

-- ----------------------------------------------------------------------------
-- PropietarioVehiculo (relación histórica Cliente <-> Vehiculo)
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[PropietarioVehiculo]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[PropietarioVehiculo] (
        [Id] int NOT NULL IDENTITY,
        [ClienteId] int NOT NULL,
        [VehiculoId] int NOT NULL,
        [FechaAdquisicion] datetime2 NOT NULL DEFAULT GETDATE(),
        [FechaVenta] datetime2 NULL,
        [EsActual] bit NOT NULL DEFAULT 1,
        CONSTRAINT [PK_PropietarioVehiculo] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PropietarioVehiculo_Cliente] FOREIGN KEY ([ClienteId]) REFERENCES [dbo].[Cliente] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PropietarioVehiculo_Vehiculo] FOREIGN KEY ([VehiculoId]) REFERENCES [dbo].[Vehiculo] ([Id]) ON DELETE CASCADE
    );
END
GO

-- ----------------------------------------------------------------------------
-- RolPermisos (tabla intermedia Rol <-> Permiso)
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[RolPermisos]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[RolPermisos] (
        [IdRol] int NOT NULL,
        [IdPermiso] int NOT NULL,
        CONSTRAINT [PK_RolPermisos] PRIMARY KEY CLUSTERED ([IdRol] ASC, [IdPermiso] ASC),
        CONSTRAINT [FK_RolPermisos_Roles] FOREIGN KEY ([IdRol]) REFERENCES [dbo].[Roles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RolPermisos_Permisos] FOREIGN KEY ([IdPermiso]) REFERENCES [dbo].[Permisos] ([IdPermiso]) ON DELETE CASCADE
    );
END
GO

-- ----------------------------------------------------------------------------
-- RegistroServicio (orden de servicio / ingreso al taller)
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[RegistroServicio]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[RegistroServicio] (
        [Id] int NOT NULL IDENTITY,
        [VehiculoId] int NOT NULL,
        [TallerId] int NOT NULL,
        [UsuarioId] int NOT NULL,
        [Fecha] datetime2 NOT NULL,
        [KmIngreso] int NOT NULL,
        [Estado] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_RegistroServicio] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RegistroServicio_Taller_TallerId] FOREIGN KEY ([TallerId]) REFERENCES [dbo].[Taller] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_RegistroServicio_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Usuarios] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_RegistroServicio_Vehiculo_VehiculoId] FOREIGN KEY ([VehiculoId]) REFERENCES [dbo].[Vehiculo] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [CK_RegistroServicio_KmIngreso] CHECK ([KmIngreso] >= 0),
        CONSTRAINT [CHK_RegistroServicio_Estado] CHECK ([Estado] IN ('Abierta', 'En Proceso', 'Completada', 'Pagada', 'Cancelada'))
    );
END
GO

-- ----------------------------------------------------------------------------
-- DetalleServicio (tareas de una orden de servicio)
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[DetalleServicio]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[DetalleServicio] (
        [Id] int NOT NULL IDENTITY,
        [RegistroServicioId] int NOT NULL,
        [UsuarioId] int NOT NULL,
        [ServicioId] int NOT NULL,
        [Cantidad] int NOT NULL,
        [Precio] decimal(18,2) NOT NULL,
        [OrdenEjecucion] int NOT NULL DEFAULT 1,
        [Origen] nvarchar(100) NOT NULL,
        [Estado] nvarchar(50) NOT NULL,
        [Observaciones] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_DetalleServicio] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DetalleServicio_RegistroServicio_RegistroServicioId] FOREIGN KEY ([RegistroServicioId]) REFERENCES [dbo].[RegistroServicio] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_DetalleServicio_Servicio_ServicioId] FOREIGN KEY ([ServicioId]) REFERENCES [dbo].[Servicio] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_DetalleServicio_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Usuarios] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [CK_DetalleServicio_Precio] CHECK ([Precio] >= 0),
        CONSTRAINT [CK_DetalleServicio_Cantidad] CHECK ([Cantidad] > 0),
        CONSTRAINT [CHK_DetalleServicio_Estado] CHECK ([Estado] IN ('Pendiente', 'En Curso', 'Finalizada'))
    );
END
GO

-- ----------------------------------------------------------------------------
-- Factura (una factura por orden de servicio)
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[Factura]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Factura] (
        [Id] int NOT NULL IDENTITY,
        [RegistroServicioId] int NOT NULL,
        [Numero] int NOT NULL DEFAULT (NEXT VALUE FOR [dbo].[Seq_Factura_Numero]),
        [Fecha] datetime2 NOT NULL,
        [Total] decimal(10,2) NOT NULL,
        CONSTRAINT [PK_Factura] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Factura_RegistroServicio_RegistroServicioId] FOREIGN KEY ([RegistroServicioId]) REFERENCES [dbo].[RegistroServicio] ([Id]) ON DELETE NO ACTION
    );
END
GO

-- ----------------------------------------------------------------------------
-- Pago (pagos parciales o totales de una factura)
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[Pago]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Pago] (
        [Id] int NOT NULL IDENTITY,
        [FacturaId] int NOT NULL,
        [MetodoPagoId] int NOT NULL,
        [Monto] decimal(10,2) NOT NULL,
        [FechaPago] datetime2 NOT NULL,
        CONSTRAINT [PK_Pago] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Pago_Factura_FacturaId] FOREIGN KEY ([FacturaId]) REFERENCES [dbo].[Factura] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Pago_MetodoPago_MetodoPagoId] FOREIGN KEY ([MetodoPagoId]) REFERENCES [dbo].[MetodoPago] ([Id]) ON DELETE NO ACTION
    );
END
GO

-- ----------------------------------------------------------------------------
-- ConfiguracionEmpresa (tabla singleton, Id = 1; la fila semilla va en 04_datos)
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[ConfiguracionEmpresa]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ConfiguracionEmpresa] (
        [Id] int NOT NULL,
        [Nombre] nvarchar(100) NOT NULL,
        [Direccion] nvarchar(200) NOT NULL,
        [Telefono] nvarchar(20) NOT NULL,
        [Email] nvarchar(150) NULL,
        [Logo] varbinary(max) NULL,
        [FechaActualizacion] datetime2 NOT NULL,
        CONSTRAINT [PK_ConfiguracionEmpresa] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

-- ----------------------------------------------------------------------------
-- ConfiguracionBackup (tabla singleton, Id = 1; la fila semilla va en 04_datos)
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[ConfiguracionBackup]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ConfiguracionBackup] (
        [Id] int NOT NULL,
        [NombreBaseDatos] nvarchar(128) NOT NULL,
        [RutaBackupServer] nvarchar(260) NOT NULL,
        [RutaBackupUNC] nvarchar(260) NOT NULL,
        [DriveFolderId] nvarchar(260) NULL,
        [UltimaSubidaArchivo] nvarchar(260) NULL,
        [FechaUltimaSubida] datetime2 NULL,
        [FechaActualizacion] datetime2 NOT NULL,
        CONSTRAINT [PK_ConfiguracionBackup] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

-- ============================================================================
-- 2. Índices
-- ============================================================================

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_DetalleServicio_RegistroServicioId' AND object_id = OBJECT_ID(N'[dbo].[DetalleServicio]'))
BEGIN
    CREATE INDEX [IX_DetalleServicio_RegistroServicioId] ON [dbo].[DetalleServicio] ([RegistroServicioId]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_DetalleServicio_ServicioId' AND object_id = OBJECT_ID(N'[dbo].[DetalleServicio]'))
BEGIN
    CREATE INDEX [IX_DetalleServicio_ServicioId] ON [dbo].[DetalleServicio] ([ServicioId]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_DetalleServicio_UsuarioId' AND object_id = OBJECT_ID(N'[dbo].[DetalleServicio]'))
BEGIN
    CREATE INDEX [IX_DetalleServicio_UsuarioId] ON [dbo].[DetalleServicio] ([UsuarioId]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Modelo_MarcaId' AND object_id = OBJECT_ID(N'[dbo].[Modelo]'))
BEGIN
    CREATE INDEX [IX_Modelo_MarcaId] ON [dbo].[Modelo] ([MarcaId]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_RegistroServicio_TallerId' AND object_id = OBJECT_ID(N'[dbo].[RegistroServicio]'))
BEGIN
    CREATE INDEX [IX_RegistroServicio_TallerId] ON [dbo].[RegistroServicio] ([TallerId]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_RegistroServicio_UsuarioId' AND object_id = OBJECT_ID(N'[dbo].[RegistroServicio]'))
BEGIN
    CREATE INDEX [IX_RegistroServicio_UsuarioId] ON [dbo].[RegistroServicio] ([UsuarioId]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_RegistroServicio_VehiculoId' AND object_id = OBJECT_ID(N'[dbo].[RegistroServicio]'))
BEGIN
    CREATE INDEX [IX_RegistroServicio_VehiculoId] ON [dbo].[RegistroServicio] ([VehiculoId]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Usuarios_Dni' AND object_id = OBJECT_ID(N'[dbo].[Usuarios]'))
BEGIN
    CREATE UNIQUE INDEX [IX_Usuarios_Dni] ON [dbo].[Usuarios] ([Dni]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Usuarios_Email' AND object_id = OBJECT_ID(N'[dbo].[Usuarios]'))
BEGIN
    CREATE UNIQUE INDEX [IX_Usuarios_Email] ON [dbo].[Usuarios] ([Email]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Usuarios_RolId' AND object_id = OBJECT_ID(N'[dbo].[Usuarios]'))
BEGIN
    CREATE INDEX [IX_Usuarios_RolId] ON [dbo].[Usuarios] ([RolId]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Vehiculo_ModeloId' AND object_id = OBJECT_ID(N'[dbo].[Vehiculo]'))
BEGIN
    CREATE INDEX [IX_Vehiculo_ModeloId] ON [dbo].[Vehiculo] ([ModeloId]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Factura_RegistroServicioId' AND object_id = OBJECT_ID(N'[dbo].[Factura]'))
BEGIN
    CREATE UNIQUE INDEX [IX_Factura_RegistroServicioId] ON [dbo].[Factura] ([RegistroServicioId]);
END
GO
