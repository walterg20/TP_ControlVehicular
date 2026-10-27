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
    [Observaciones] nvarchar(max) NOT NULL,
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


