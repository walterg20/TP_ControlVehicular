-- Script de generacion de datos

-- Datos para la tabla [dbo].[Cliente]
SET IDENTITY_INSERT [dbo].[Cliente] ON;
INSERT INTO [dbo].[Cliente] ([Id], [Nombre], [Apellido], [Dni], [FechaNacimiento], [Direccion], [Email], [Telefono], [Activo]) VALUES (1, 'Cliente', 'Demo', '20000001', '1990-01-01 00:00:00.000', 'Av. Siempre Viva 742', 'cliente@example.com', '3794000004', 1);
SET IDENTITY_INSERT [dbo].[Cliente] OFF;

-- Datos para la tabla [dbo].[Marca]
SET IDENTITY_INSERT [dbo].[Marca] ON;
INSERT INTO [dbo].[Marca] ([Id], [NombreMarca]) VALUES (1, 'Toyota');
INSERT INTO [dbo].[Marca] ([Id], [NombreMarca]) VALUES (2, 'Renault');
SET IDENTITY_INSERT [dbo].[Marca] OFF;

-- Datos para la tabla [dbo].[Modelo]
SET IDENTITY_INSERT [dbo].[Modelo] ON;
INSERT INTO [dbo].[Modelo] ([Id], [MarcaId], [NombreModelo]) VALUES (1, 1, 'Etios');
INSERT INTO [dbo].[Modelo] ([Id], [MarcaId], [NombreModelo]) VALUES (2, 2, 'Kwid');
SET IDENTITY_INSERT [dbo].[Modelo] OFF;

-- Datos para la tabla [dbo].[Vehiculo]
SET IDENTITY_INSERT [dbo].[Vehiculo] ON;
INSERT INTO [dbo].[Vehiculo] ([Id], [ClienteId], [ModeloId], [Anio], [Patente], [KmActual]) VALUES (1, 1, 1, 2020, 'AB 125 DC', 60000);
INSERT INTO [dbo].[Vehiculo] ([Id], [ClienteId], [ModeloId], [Anio], [Patente], [KmActual]) VALUES (2, 1, 2, 2025, 'AF 564 FG', 30000);
SET IDENTITY_INSERT [dbo].[Vehiculo] OFF;

-- Datos para la tabla [dbo].[Roles]
SET IDENTITY_INSERT [dbo].[Roles] ON;
INSERT INTO [dbo].[Roles] ([Id], [Nombre], [Descripcion], [Estado]) VALUES (1, 'Administrador', 'Este administrado tiene permiso para todo el uso del sistema', 1);
INSERT INTO [dbo].[Roles] ([Id], [Nombre], [Descripcion], [Estado]) VALUES (2, 'Recepcionista', 'La recepcionista puede crear orden de servicio, cliente y vehiculo', 1);
INSERT INTO [dbo].[Roles] ([Id], [Nombre], [Descripcion], [Estado]) VALUES (3, 'Mecánico', 'Mecanico puede agregar servicio a la orden de servicio y tildar lo que se hizo para que luego le facuten', 1);
SET IDENTITY_INSERT [dbo].[Roles] OFF;

-- Datos para la tabla [dbo].[Taller]
SET IDENTITY_INSERT [dbo].[Taller] ON;
INSERT INTO [dbo].[Taller] ([Id], [Nombre], [Direccion], [Telefono], [Activo]) VALUES (1, 'Taller PRO', 'Av. Siempre Viva 742', '3794000000', 1);
SET IDENTITY_INSERT [dbo].[Taller] OFF;

-- Datos para la tabla [dbo].[Usuarios]
SET IDENTITY_INSERT [dbo].[Usuarios] ON;
INSERT INTO [dbo].[Usuarios] ([Id], [RolId], [Nombre], [Contrasena], [Estado], [Apellido], [Dni], [Domicilio], [Email], [FechaNacimiento], [Telefono]) VALUES (1, 1, 'Admin', 'CHANGE_ME', 1, 'Sistema', '10000001', 'Av. Siempre Viva 742', 'admin@example.com', '2000-01-26 00:00:00.000', '3794000001');
INSERT INTO [dbo].[Usuarios] ([Id], [RolId], [Nombre], [Contrasena], [Estado], [Apellido], [Dni], [Domicilio], [Email], [FechaNacimiento], [Telefono]) VALUES (2, 3, 'Mecanico', 'CHANGE_ME', 1, 'Demo', '10000002', 'Av. Siempre Viva 742', 'mecanico@example.com', '2008-09-16 00:00:00.000', '3794000002');
INSERT INTO [dbo].[Usuarios] ([Id], [RolId], [Nombre], [Contrasena], [Estado], [Apellido], [Dni], [Domicilio], [Email], [FechaNacimiento], [Telefono]) VALUES (3, 2, 'Recepcion', 'CHANGE_ME', 1, 'Demo', '10000003', 'Av. Siempre Viva 742', 'recepcion@example.com', '2000-02-16 00:00:00.000', '3794000003');
SET IDENTITY_INSERT [dbo].[Usuarios] OFF;

-- Datos para la tabla [dbo].[RegistroServicio]
SET IDENTITY_INSERT [dbo].[RegistroServicio] ON;
INSERT INTO [dbo].[RegistroServicio] ([Id], [VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado]) VALUES (1, 1, 1, 3, '2026-09-27 19:31:53.396', 125000, 'Pendiente');
SET IDENTITY_INSERT [dbo].[RegistroServicio] OFF;

-- Datos para la tabla [dbo].[Servicio]
SET IDENTITY_INSERT [dbo].[Servicio] ON;
INSERT INTO [dbo].[Servicio] ([Id], [Nombre], [Precio], [Activo]) VALUES (1, 'Cambio de Filtro Aire', 25000.00, 1);
INSERT INTO [dbo].[Servicio] ([Id], [Nombre], [Precio], [Activo]) VALUES (2, 'Cambio de Correa de Distribucion', 15000.00, 1);
SET IDENTITY_INSERT [dbo].[Servicio] OFF;

-- Datos para la tabla [dbo].[DetalleServicio]
SET IDENTITY_INSERT [dbo].[DetalleServicio] ON;
INSERT INTO [dbo].[DetalleServicio] ([Id], [RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [Origen], [Estado], [Observaciones]) VALUES (1, 1, 2, 1, 1, 25000.00, 'Manual', 'Realizado', '');
INSERT INTO [dbo].[DetalleServicio] ([Id], [RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [Origen], [Estado], [Observaciones]) VALUES (3, 1, 2, 2, 1, 15000.00, 'Manual', 'Realizado', '');
SET IDENTITY_INSERT [dbo].[DetalleServicio] OFF;
