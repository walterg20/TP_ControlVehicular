-- =========================================================================
-- SCRIPT DE GENERACIÓN LIMPIO DE BASE DE DATOS TP_ControlVehicular
-- Sin dependencias de Migraciones (EF Core History)
-- Incluye: Creación de BD, Tablas, Relaciones y Datos Semilla
-- =========================================================================

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TP_ControlVehicular')
BEGIN
    CREATE DATABASE [TP_ControlVehicular];
END;
GO

USE [TP_ControlVehicular];
GO

-- --------------------------------------------------
-- 1. ESTRUCTURA DE TABLAS Y RELACIONES
-- --------------------------------------------------

CREATE TABLE [Cliente] (
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
GO

CREATE TABLE [Marca] (
    [Id] int NOT NULL IDENTITY,
    [NombreMarca] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_Marca] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Roles] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(max) NOT NULL,
    [Descripcion] nvarchar(max) NOT NULL,
    [Estado] bit NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Servicio] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(150) NOT NULL,
    [Precio] decimal(18,2) NOT NULL,
    [Activo] bit NOT NULL,
    CONSTRAINT [PK_Servicio] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Taller] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(100) NOT NULL,
    [Direccion] nvarchar(200) NOT NULL,
    [Telefono] nvarchar(20) NOT NULL,
    [Activo] bit NOT NULL,
    CONSTRAINT [PK_Taller] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Modelo] (
    [Id] int NOT NULL IDENTITY,
    [MarcaId] int NOT NULL,
    [NombreModelo] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_Modelo] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Modelo_Marca_MarcaId] FOREIGN KEY ([MarcaId]) REFERENCES [Marca] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Usuarios] (
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
    CONSTRAINT [FK_Usuarios_Roles_RolId] FOREIGN KEY ([RolId]) REFERENCES [Roles] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Vehiculo] (
    [Id] int NOT NULL IDENTITY,
    [ClienteId] int NOT NULL,
    [ModeloId] int NOT NULL,
    [Anio] int NOT NULL,
    [Patente] nvarchar(15) NOT NULL,
    [KmActual] int NOT NULL,
    CONSTRAINT [PK_Vehiculo] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Vehiculo_Cliente_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Cliente] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Vehiculo_Modelo_ModeloId] FOREIGN KEY ([ModeloId]) REFERENCES [Modelo] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [RegistroServicio] (
    [Id] int NOT NULL IDENTITY,
    [VehiculoId] int NOT NULL,
    [TallerId] int NOT NULL,
    [UsuarioId] int NOT NULL,
    [Fecha] datetime2 NOT NULL,
    [KmIngreso] int NOT NULL,
    [Estado] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_RegistroServicio] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RegistroServicio_Taller_TallerId] FOREIGN KEY ([TallerId]) REFERENCES [Taller] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_RegistroServicio_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_RegistroServicio_Vehiculo_VehiculoId] FOREIGN KEY ([VehiculoId]) REFERENCES [Vehiculo] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [DetalleServicio] (
    [Id] int NOT NULL IDENTITY,
    [RegistroServicioId] int NOT NULL,
    [UsuarioId] int NOT NULL,
    [ServicioId] int NOT NULL,
    [Cantidad] int NOT NULL,
    [Precio] decimal(18,2) NOT NULL,
    [Origen] nvarchar(100) NOT NULL,
    [Estado] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_DetalleServicio] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DetalleServicio_RegistroServicio_RegistroServicioId] FOREIGN KEY ([RegistroServicioId]) REFERENCES [RegistroServicio] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_DetalleServicio_Servicio_ServicioId] FOREIGN KEY ([ServicioId]) REFERENCES [Servicio] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_DetalleServicio_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_DetalleServicio_RegistroServicioId] ON [DetalleServicio] ([RegistroServicioId]);
GO
CREATE INDEX [IX_DetalleServicio_ServicioId] ON [DetalleServicio] ([ServicioId]);
GO
CREATE INDEX [IX_DetalleServicio_UsuarioId] ON [DetalleServicio] ([UsuarioId]);
GO
CREATE INDEX [IX_Modelo_MarcaId] ON [Modelo] ([MarcaId]);
GO
CREATE INDEX [IX_RegistroServicio_TallerId] ON [RegistroServicio] ([TallerId]);
GO
CREATE INDEX [IX_RegistroServicio_UsuarioId] ON [RegistroServicio] ([UsuarioId]);
GO
CREATE INDEX [IX_RegistroServicio_VehiculoId] ON [RegistroServicio] ([VehiculoId]);
GO
CREATE UNIQUE INDEX [IX_Usuarios_Dni] ON [Usuarios] ([Dni]);
GO
CREATE UNIQUE INDEX [IX_Usuarios_Email] ON [Usuarios] ([Email]);
GO
CREATE INDEX [IX_Usuarios_RolId] ON [Usuarios] ([RolId]);
GO
CREATE INDEX [IX_Vehiculo_ClienteId] ON [Vehiculo] ([ClienteId]);
GO
CREATE INDEX [IX_Vehiculo_ModeloId] ON [Vehiculo] ([ModeloId]);
GO


-- --------------------------------------------------
-- 2. INSERCIÓN DE DATOS INICIALES (ROLES Y USUARIOS)
-- --------------------------------------------------

-- Asegurarse de que no se dupliquen ejecutando solo si la tabla está vacía
IF NOT EXISTS (SELECT 1 FROM [Roles])
BEGIN
    SET IDENTITY_INSERT Roles ON;
    INSERT INTO Roles (Id, Nombre, Descripcion, Estado)
    VALUES 
    (1, 'Administrador', 'Control total del sistema (Usuarios, Roles, Talleres, etc.)', 1),
    (2, 'Recepcionista', 'Gestión de clientes, vehículos y órdenes', 1),
    (3, 'Mecanico', 'Gestión operativa de servicios', 1);
    SET IDENTITY_INSERT Roles OFF;
END
GO

IF NOT EXISTS (SELECT 1 FROM [Usuarios])
BEGIN
    SET IDENTITY_INSERT Usuarios ON;
    INSERT INTO Usuarios (Id, RolId, Nombre, Apellido, Dni, Email, Telefono, Domicilio, FechaNacimiento, Contrasena, Estado)
    VALUES 
    -- Admin (Login con DNI: 11111111, Clave: admin123)
    (1, 1, 'Admin', 'Sistema', '11111111', 'admin@taller.com', '3794000001', 'Calle Falsa 123', '1990-01-01', 'admin123', 1),
    -- Recepcionista (Login con DNI: 22222222, Clave: recep123)
    (2, 2, 'Ana', 'Lopez', '22222222', 'ana@taller.com', '3794000002', 'Avenida Siempre Viva 742', '1995-05-15', 'recep123', 1),
    -- Mecánico (Login con DNI: 33333333, Clave: meca123)
    (3, 3, 'Carlos', 'Gomez', '33333333', 'carlos@taller.com', '3794000003', 'Calle San Martin 450', '1985-11-20', 'meca123', 1);
    SET IDENTITY_INSERT Usuarios OFF;
END
GO