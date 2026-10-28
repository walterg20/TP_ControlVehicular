-- ============================================================================
-- 02_datos.sql — Datos semilla canónicos del sistema de control vehicular.
-- ----------------------------------------------------------------------------
-- Contenido canónico oficial:
--   * Roles del sistema (Administrador = 1, Recepcionista = 2, Mecánico = 3).
--   * Catálogo completo de permisos RBAC (38 permisos).
--   * Matriz de asignación Rol -> Permiso (roles 1, 2 y 3).
--   * Usuario inicial: Únicamente el usuario Administrador bootstrap.
--   * Taller base (Id = 1, Taller PRO).
--   * ConfiguracionEmpresa y ConfiguracionBackup (filas singleton, Id = 1).
--   * Métodos de pago (Efectivo, Tarjeta).
--   * Catálogo completo de Servicios oficiales (15 servicios).
--   * Catálogo ampliado de Marca/Modelo representativo del mercado argentino
--     (45 marcas, 203 modelos; se preservan Toyota = Id 1 y Renault = Id 2).
--   * Usuarios del equipo: Recepcionista y Mecánicos de taller.
--   * Clientes (55 clientes locales con datos de contacto completos).
--   * Vehículos (55 vehículos vinculados a modelos oficiales con kilometraje real).
--   * PropietarioVehiculo (55 vinculaciones de titularidad vigentes).
--   * Órdenes de Servicio (61 órdenes entre Junio y Octubre 2026 en distintos estados).
--   * Detalles de Servicio (110+ tareas asignadas a mecánicos).
--   * Facturación y Pagos para órdenes finalizadas y cobradas.
-- Idempotencia:
--   * Cada sección usa MERGE o IF NOT EXISTS para re-ejecución sin duplicados.
-- Compatible con SQL Server 2019 Express. Codificación UTF-8.
-- Orden de aplicación: 01_estructura -> 02_datos -> 03_procedimientos -> 04_triggers -> 05_utilidades.
-- ============================================================================

USE [ControlVehicular];
GO

-- ============================================================================
-- 1. Roles (identidades fijas 1/2/3, requeridas por la matriz de permisos)
-- ============================================================================
SET IDENTITY_INSERT [dbo].[Roles] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [Id] = 1)
    INSERT INTO [dbo].[Roles] ([Id], [Nombre], [Descripcion], [Activo])
    VALUES (1, N'Administrador', N'Este administrador tiene permiso para todo el uso del sistema', 1);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [Id] = 2)
    INSERT INTO [dbo].[Roles] ([Id], [Nombre], [Descripcion], [Activo])
    VALUES (2, N'Recepcionista', N'La recepcionista puede crear orden de servicio, cliente y vehiculo', 1);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [Id] = 3)
    INSERT INTO [dbo].[Roles] ([Id], [Nombre], [Descripcion], [Activo])
    VALUES (3, N'Mecánico', N'Mecanico puede agregar servicio a la orden de servicio y tildar lo que se hizo para que luego le facturen', 1);

SET IDENTITY_INSERT [dbo].[Roles] OFF;
GO

-- ============================================================================
-- 2. Permisos (catálogo completo de 38 permisos RBAC)
-- ============================================================================
INSERT INTO [dbo].[Permisos] ([Nombre])
SELECT v.[Nombre]
FROM (VALUES
    (N'Cliente.Ver'), (N'Cliente.Crear'), (N'Cliente.Editar'), (N'Cliente.Eliminar'),
    (N'Vehiculo.Ver'), (N'Vehiculo.Crear'), (N'Vehiculo.Editar'), (N'Vehiculo.Eliminar'),
    (N'Marca.Ver'), (N'Marca.Crear'), (N'Marca.Editar'), (N'Marca.Eliminar'),
    (N'Modelo.Ver'), (N'Modelo.Crear'), (N'Modelo.Editar'), (N'Modelo.Eliminar'),
    (N'Servicio.Ver'), (N'Servicio.Crear'), (N'Servicio.Editar'), (N'Servicio.Eliminar'),
    (N'Usuario.Ver'), (N'Usuario.Crear'), (N'Usuario.Editar'), (N'Usuario.Eliminar'),
    (N'OrdenServicio.Ver'), (N'OrdenServicio.Crear'), (N'OrdenServicio.Editar'), (N'OrdenServicio.Eliminar'),
    (N'Taller.Ver'), (N'Taller.Editar'),
    (N'Reporte.Ver'),
    (N'MisTrabajos.Ver'),
    (N'Reporte.Ordenes.Ver'), (N'Reporte.Operativo.Ver'), (N'Reporte.Gerencial.Ver'), (N'Reporte.Mecanico.Ver'),
    (N'Configuracion.Empresa.Gestionar'), (N'Configuracion.Backup.Gestionar')
) AS v([Nombre])
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Permisos] p WHERE p.[Nombre] = v.[Nombre]);
GO

-- ============================================================================
-- 3. RolPermisos — matriz Rol -> Permiso (idempotente y autorreparable)
--    Administrador = todos los permisos EXCEPTO 'MisTrabajos.Ver'.
-- ============================================================================
DECLARE @Permitidos TABLE ([IdRol] int NOT NULL, [IdPermiso] int NOT NULL);

-- Administrador (1): todos menos MisTrabajos.Ver
INSERT INTO @Permitidos ([IdRol], [IdPermiso])
SELECT 1, p.[IdPermiso]
FROM [dbo].[Permisos] p
WHERE p.[Nombre] <> N'MisTrabajos.Ver';

-- Recepcionista (2)
INSERT INTO @Permitidos ([IdRol], [IdPermiso])
SELECT 2, p.[IdPermiso]
FROM [dbo].[Permisos] p
WHERE p.[Nombre] IN (
    N'Cliente.Ver', N'Cliente.Crear', N'Cliente.Editar', N'Cliente.Eliminar',
    N'Vehiculo.Ver', N'Vehiculo.Crear', N'Vehiculo.Editar', N'Vehiculo.Eliminar',
    N'Marca.Ver', N'Marca.Crear', N'Marca.Editar', N'Marca.Eliminar',
    N'Modelo.Ver', N'Modelo.Crear', N'Modelo.Editar', N'Modelo.Eliminar',
    N'OrdenServicio.Ver', N'OrdenServicio.Crear', N'OrdenServicio.Editar', N'OrdenServicio.Eliminar',
    N'Servicio.Ver',
    N'Reporte.Ordenes.Ver', N'Reporte.Operativo.Ver'
);

-- Mecánico (3)
INSERT INTO @Permitidos ([IdRol], [IdPermiso])
SELECT 3, p.[IdPermiso]
FROM [dbo].[Permisos] p
WHERE p.[Nombre] IN (
    N'Cliente.Ver', N'Vehiculo.Ver', N'Marca.Ver', N'Modelo.Ver',
    N'OrdenServicio.Ver', N'OrdenServicio.Editar',
    N'Servicio.Ver',
    N'MisTrabajos.Ver',
    N'Reporte.Mecanico.Ver'
);

-- a) Insertar concesiones faltantes
INSERT INTO [dbo].[RolPermisos] ([IdRol], [IdPermiso])
SELECT p.[IdRol], p.[IdPermiso]
FROM @Permitidos p
WHERE NOT EXISTS (
    SELECT 1 FROM [dbo].[RolPermisos] rp
    WHERE rp.[IdRol] = p.[IdRol] AND rp.[IdPermiso] = p.[IdPermiso]
);

-- b) Eliminar concesiones sobrantes (roles 1, 2 y 3)
DELETE rp
FROM [dbo].[RolPermisos] rp
WHERE rp.[IdRol] IN (1, 2, 3)
  AND NOT EXISTS (
    SELECT 1 FROM @Permitidos p
    WHERE p.[IdRol] = rp.[IdRol] AND p.[IdPermiso] = rp.[IdPermiso]
  );
GO

-- ============================================================================
-- 4. Taller (fila base por clave natural Nombre)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Taller] WHERE [Nombre] = N'Taller PRO')
    INSERT INTO [dbo].[Taller] ([Nombre], [Direccion], [Telefono], [Activo])
    VALUES (N'Taller PRO', N'Av. Siempre Viva 742', N'3794000000', 1);
GO

-- ============================================================================
-- 5. ConfiguracionEmpresa (fila singleton Id = 1)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[ConfiguracionEmpresa] WHERE [Id] = 1)
    INSERT INTO [dbo].[ConfiguracionEmpresa] ([Id], [Nombre], [Direccion], [Telefono], [Email], [Logo], [FechaActualizacion])
    VALUES (1, N'TALLER PRO', N'Ruta Nacional 11 Km 1005 - Resistencia, Chaco', N'+54 362 4123456', N'contacto@taller.com', NULL, SYSDATETIME());
GO

-- ============================================================================
-- 6. ConfiguracionBackup (fila singleton Id = 1; rutas configurables)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[ConfiguracionBackup] WHERE [Id] = 1)
    INSERT INTO [dbo].[ConfiguracionBackup] ([Id], [NombreBaseDatos], [RutaBackupServer], [RutaBackupUNC], [DriveFolderId], [UltimaSubidaArchivo], [FechaUltimaSubida], [FechaActualizacion])
    VALUES (1, N'ControlVehicular', N'', N'', NULL, NULL, NULL, SYSDATETIME());
GO

-- ============================================================================
-- 7. MetodoPago (Efectivo y Tarjeta)
-- ============================================================================
INSERT INTO [dbo].[MetodoPago] ([Nombre])
SELECT v.[Nombre]
FROM (VALUES (N'Efectivo'), (N'Tarjeta')) AS v([Nombre])
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[MetodoPago] m WHERE m.[Nombre] = v.[Nombre]);
GO

-- ============================================================================
-- 8. Usuario inicial (Únicamente Administrador bootstrap)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Usuarios] WHERE [Email] = N'admin@example.com')
    INSERT INTO [dbo].[Usuarios] ([RolId], [Nombre], [Apellido], [Dni], [Email], [Telefono], [Domicilio], [FechaNacimiento], [Contrasena], [Activo])
    SELECT r.[Id], N'Admin', N'Sistema', N'10000001', N'admin@example.com', N'3794000001', N'Av. Siempre Viva 742', CAST(N'2000-01-26T00:00:00' AS datetime2), N'CHANGE_ME', 1
    FROM [dbo].[Roles] r
    WHERE r.[Id] = 1;
GO

-- ============================================================================
-- 9. Catálogo de Servicios oficiales (15 servicios)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Filtro Aire')
    INSERT INTO [dbo].[Servicio] ([Nombre], [Precio], [Activo]) VALUES (N'Cambio de Filtro Aire', 25000.00, 1);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Correa de Distribucion')
    INSERT INTO [dbo].[Servicio] ([Nombre], [Precio], [Activo]) VALUES (N'Cambio de Correa de Distribucion', 15000.00, 1);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Filtro de Aceite')
    INSERT INTO [dbo].[Servicio] ([Nombre], [Precio], [Activo]) VALUES (N'Cambio de Filtro de Aceite', 34000.00, 1);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Aceite de Motor')
    INSERT INTO [dbo].[Servicio] ([Nombre], [Precio], [Activo]) VALUES (N'Cambio de Aceite de Motor', 65000.00, 1);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Servicio] WHERE [Nombre] = N'Alineación y Balanceo')
    INSERT INTO [dbo].[Servicio] ([Nombre], [Precio], [Activo]) VALUES (N'Alineación y Balanceo', 38000.00, 1);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Pastillas de Freno')
    INSERT INTO [dbo].[Servicio] ([Nombre], [Precio], [Activo]) VALUES (N'Cambio de Pastillas de Freno', 45000.00, 1);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Bujías')
    INSERT INTO [dbo].[Servicio] ([Nombre], [Precio], [Activo]) VALUES (N'Cambio de Bujías', 28000.00, 1);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Servicio] WHERE [Nombre] = N'Recarga de Aire Acondicionado')
    INSERT INTO [dbo].[Servicio] ([Nombre], [Precio], [Activo]) VALUES (N'Recarga de Aire Acondicionado', 55000.00, 1);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Batería')
    INSERT INTO [dbo].[Servicio] ([Nombre], [Precio], [Activo]) VALUES (N'Cambio de Batería', 95000.00, 1);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Servicio] WHERE [Nombre] = N'Diagnóstico Computarizado OBD2')
    INSERT INTO [dbo].[Servicio] ([Nombre], [Precio], [Activo]) VALUES (N'Diagnóstico Computarizado OBD2', 20000.00, 1);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Servicio] WHERE [Nombre] = N'Limpieza de Inyectores')
    INSERT INTO [dbo].[Servicio] ([Nombre], [Precio], [Activo]) VALUES (N'Limpieza de Inyectores', 48000.00, 1);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Líquido de Frenos')
    INSERT INTO [dbo].[Servicio] ([Nombre], [Precio], [Activo]) VALUES (N'Cambio de Líquido de Frenos', 22000.00, 1);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Servicio] WHERE [Nombre] = N'Rotación de Neumáticos')
    INSERT INTO [dbo].[Servicio] ([Nombre], [Precio], [Activo]) VALUES (N'Rotación de Neumáticos', 12000.00, 1);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Filtro de Habitáculo')
    INSERT INTO [dbo].[Servicio] ([Nombre], [Precio], [Activo]) VALUES (N'Cambio de Filtro de Habitáculo', 18000.00, 1);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Servicio] WHERE [Nombre] = N'Lavado de Motor')
    INSERT INTO [dbo].[Servicio] ([Nombre], [Precio], [Activo]) VALUES (N'Lavado de Motor', 16000.00, 1);
GO

-- ============================================================================
-- 10. Catálogo completo de Marcas y Modelos (45 marcas, 203 modelos)
-- ============================================================================
SET IDENTITY_INSERT [dbo].[Marca] ON;
IF NOT EXISTS (SELECT 1 FROM [dbo].[Marca] WHERE [NombreMarca] = N'Toyota')
    INSERT INTO [dbo].[Marca] ([Id], [NombreMarca]) VALUES (1, N'Toyota');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Marca] WHERE [NombreMarca] = N'Renault')
    INSERT INTO [dbo].[Marca] ([Id], [NombreMarca]) VALUES (2, N'Renault');
SET IDENTITY_INSERT [dbo].[Marca] OFF;
GO

SET IDENTITY_INSERT [dbo].[Modelo] ON;
IF NOT EXISTS (SELECT 1 FROM [dbo].[Modelo] WHERE [NombreModelo] = N'Etios' AND [MarcaId] = (SELECT [Id] FROM [dbo].[Marca] WHERE [NombreMarca] = N'Toyota'))
    INSERT INTO [dbo].[Modelo] ([Id], [MarcaId], [NombreModelo]) VALUES (1, (SELECT [Id] FROM [dbo].[Marca] WHERE [NombreMarca] = N'Toyota'), N'Etios');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Modelo] WHERE [NombreModelo] = N'Kwid' AND [MarcaId] = (SELECT [Id] FROM [dbo].[Marca] WHERE [NombreMarca] = N'Renault'))
    INSERT INTO [dbo].[Modelo] ([Id], [MarcaId], [NombreModelo]) VALUES (2, (SELECT [Id] FROM [dbo].[Marca] WHERE [NombreMarca] = N'Renault'), N'Kwid');
SET IDENTITY_INSERT [dbo].[Modelo] OFF;
GO

IF OBJECT_ID('tempdb..#MarcasCatalogo') IS NOT NULL DROP TABLE #MarcasCatalogo;
CREATE TABLE #MarcasCatalogo ([Marca] nvarchar(100) NOT NULL, [Modelo] nvarchar(100) NOT NULL);

INSERT INTO #MarcasCatalogo ([Marca], [Modelo]) VALUES
    (N'Toyota', N'Etios'),
    (N'Renault', N'Kwid'),
    (N'Renault', N'Stepway'),
    (N'Toyota', N'Corolla'),
    (N'Toyota', N'Hilux'),
    (N'Toyota', N'RAV4'),
    (N'Toyota', N'SW4'),
    (N'Toyota', N'Yaris'),
    (N'Renault', N'Clio'),
    (N'Renault', N'Duster'),
    (N'Renault', N'Kangoo'),
    (N'Renault', N'Logan'),
    (N'Renault', N'Sandero'),
    (N'Alfa Romeo', N'Giulia'),
    (N'Alfa Romeo', N'Giulietta'),
    (N'Alfa Romeo', N'Stelvio'),
    (N'Audi', N'A1'),
    (N'Audi', N'A3'),
    (N'Audi', N'A4'),
    (N'Audi', N'Q3'),
    (N'Audi', N'Q5'),
    (N'Audi', N'Q7'),
    (N'BAIC', N'BJ40'),
    (N'BAIC', N'D20'),
    (N'BAIC', N'X35'),
    (N'BAIC', N'X55'),
    (N'BMW', N'Serie 1'),
    (N'BMW', N'Serie 3'),
    (N'BMW', N'Serie 5'),
    (N'BMW', N'X1'),
    (N'BMW', N'X3'),
    (N'BMW', N'X5'),
    (N'Changan', N'Alsvin'),
    (N'Changan', N'CS15'),
    (N'Changan', N'CS35'),
    (N'Changan', N'CS75'),
    (N'Chery', N'Arrizo 5'),
    (N'Chery', N'Tiggo 2'),
    (N'Chery', N'Tiggo 4'),
    (N'Chery', N'Tiggo 5'),
    (N'Chery', N'Tiggo 7'),
    (N'Chery', N'Tiggo 8'),
    (N'Chevrolet', N'Cruze'),
    (N'Chevrolet', N'Onix'),
    (N'Chevrolet', N'Prisma'),
    (N'Chevrolet', N'S10'),
    (N'Chevrolet', N'Spin'),
    (N'Chevrolet', N'Tracker'),
    (N'Citroën', N'Berlingo'),
    (N'Citroën', N'C3'),
    (N'Citroën', N'C4 Cactus'),
    (N'Citroën', N'C5 Aircross'),
    (N'Citroën', N'C-Elysée'),
    (N'Citroën', N'Jumpy'),
    (N'DFSK', N'C31'),
    (N'DFSK', N'Glory 580'),
    (N'DFSK', N'K01'),
    (N'Dodge', N'Durango'),
    (N'Dodge', N'Journey'),
    (N'DS', N'DS3'),
    (N'DS', N'DS4'),
    (N'DS', N'DS7'),
    (N'Fiat', N'Argo'),
    (N'Fiat', N'Cronos'),
    (N'Fiat', N'Mobi'),
    (N'Fiat', N'Pulse'),
    (N'Fiat', N'Strada'),
    (N'Fiat', N'Toro'),
    (N'Ford', N'EcoSport'),
    (N'Ford', N'Fiesta'),
    (N'Ford', N'Focus'),
    (N'Ford', N'Ka'),
    (N'Ford', N'Ranger'),
    (N'Ford', N'Territory'),
    (N'Geely', N'Coolray'),
    (N'Geely', N'Emgrand'),
    (N'Geely', N'GS'),
    (N'Geely', N'Okavango'),
    (N'Great Wall', N'Haval H5'),
    (N'Great Wall', N'Poer'),
    (N'Great Wall', N'Wingle'),
    (N'Haval', N'H6'),
    (N'Haval', N'H9'),
    (N'Haval', N'Jolion'),
    (N'Honda', N'City'),
    (N'Honda', N'Civic'),
    (N'Honda', N'CR-V'),
    (N'Honda', N'Fit'),
    (N'Honda', N'HR-V'),
    (N'Honda', N'WR-V'),
    (N'Hyundai', N'Creta'),
    (N'Hyundai', N'Elantra'),
    (N'Hyundai', N'HB20'),
    (N'Hyundai', N'i10'),
    (N'Hyundai', N'Santa Fe'),
    (N'Hyundai', N'Tucson'),
    (N'Isuzu', N'D-Max'),
    (N'Isuzu', N'MU-X'),
    (N'Iveco', N'Daily'),
    (N'Iveco', N'Stralis'),
    (N'Iveco', N'Tector'),
    (N'JAC', N'J4'),
    (N'JAC', N'S2'),
    (N'JAC', N'S3'),
    (N'JAC', N'T6'),
    (N'JAC', N'T8'),
    (N'Jaguar', N'E-Pace'),
    (N'Jaguar', N'F-Pace'),
    (N'Jaguar', N'XE'),
    (N'Jaguar', N'XF'),
    (N'Jeep', N'Cherokee'),
    (N'Jeep', N'Compass'),
    (N'Jeep', N'Grand Cherokee'),
    (N'Jeep', N'Renegade'),
    (N'Jeep', N'Wrangler'),
    (N'Kia', N'Cerato'),
    (N'Kia', N'Picanto'),
    (N'Kia', N'Rio'),
    (N'Kia', N'Seltos'),
    (N'Kia', N'Soul'),
    (N'Kia', N'Sportage'),
    (N'Land Rover', N'Defender'),
    (N'Land Rover', N'Discovery'),
    (N'Land Rover', N'Range Rover Evoque'),
    (N'Land Rover', N'Range Rover Sport'),
    (N'Lexus', N'CT'),
    (N'Lexus', N'IS'),
    (N'Lexus', N'NX'),
    (N'Lexus', N'RX'),
    (N'Lifan', N'320'),
    (N'Lifan', N'Foison'),
    (N'Lifan', N'X50'),
    (N'Lifan', N'X60'),
    (N'Mazda', N'CX-3'),
    (N'Mazda', N'CX-5'),
    (N'Mazda', N'Mazda 2'),
    (N'Mazda', N'Mazda 3'),
    (N'Mercedes-Benz', N'Clase A'),
    (N'Mercedes-Benz', N'Clase C'),
    (N'Mercedes-Benz', N'Clase E'),
    (N'Mercedes-Benz', N'GLC'),
    (N'Mercedes-Benz', N'GLE'),
    (N'Mercedes-Benz', N'Sprinter'),
    (N'MG', N'HS'),
    (N'MG', N'MG5'),
    (N'MG', N'RX5'),
    (N'MG', N'ZS'),
    (N'Mini', N'Clubman'),
    (N'Mini', N'Cooper'),
    (N'Mini', N'Countryman'),
    (N'Mitsubishi', N'ASX'),
    (N'Mitsubishi', N'L200'),
    (N'Mitsubishi', N'Montero'),
    (N'Mitsubishi', N'Outlander'),
    (N'Nissan', N'Frontier'),
    (N'Nissan', N'Kicks'),
    (N'Nissan', N'March'),
    (N'Nissan', N'Note'),
    (N'Nissan', N'Versa'),
    (N'Nissan', N'X-Trail'),
    (N'Peugeot', N'2008'),
    (N'Peugeot', N'208'),
    (N'Peugeot', N'3008'),
    (N'Peugeot', N'308'),
    (N'Peugeot', N'Boxer'),
    (N'Peugeot', N'Partner'),
    (N'Porsche', N'911'),
    (N'Porsche', N'Cayenne'),
    (N'Porsche', N'Macan'),
    (N'Porsche', N'Panamera'),
    (N'RAM', N'1500'),
    (N'RAM', N'2500'),
    (N'RAM', N'700'),
    (N'Scania', N'G-Serie'),
    (N'Scania', N'P-Serie'),
    (N'Scania', N'R-Serie'),
    (N'Seat', N'Arona'),
    (N'Seat', N'Ateca'),
    (N'Seat', N'Ibiza'),
    (N'Seat', N'León'),
    (N'Skoda', N'Fabia'),
    (N'Skoda', N'Karoq'),
    (N'Skoda', N'Kodiaq'),
    (N'Skoda', N'Octavia'),
    (N'Subaru', N'Forester'),
    (N'Subaru', N'Impreza'),
    (N'Subaru', N'Outback'),
    (N'Subaru', N'XV'),
    (N'Suzuki', N'Baleno'),
    (N'Suzuki', N'Jimny'),
    (N'Suzuki', N'S-Cross'),
    (N'Suzuki', N'Swift'),
    (N'Suzuki', N'Vitara'),
    (N'Volkswagen', N'Amarok'),
    (N'Volkswagen', N'Gol'),
    (N'Volkswagen', N'Polo'),
    (N'Volkswagen', N'Saveiro'),
    (N'Volkswagen', N'T-Cross'),
    (N'Volkswagen', N'Vento'),
    (N'Volvo', N'S60'),
    (N'Volvo', N'XC40'),
    (N'Volvo', N'XC60'),
    (N'Volvo', N'XC90');

-- 1. Insertar marcas faltantes preservando las existentes
MERGE [dbo].[Marca] AS target
USING (SELECT DISTINCT [Marca] FROM #MarcasCatalogo) AS source
ON target.[NombreMarca] = source.[Marca]
WHEN NOT MATCHED THEN
    INSERT ([NombreMarca]) VALUES (source.[Marca]);

-- 2. Insertar modelos faltantes vinculados a su marca
MERGE [dbo].[Modelo] AS target
USING (
    SELECT m.[Id] AS [MarcaId], t.[Modelo] AS [NombreModelo]
    FROM #MarcasCatalogo t
    INNER JOIN [dbo].[Marca] m ON m.[NombreMarca] = t.[Marca]
) AS source
ON target.[MarcaId] = source.[MarcaId] AND target.[NombreModelo] = source.[NombreModelo]
WHEN NOT MATCHED THEN
    INSERT ([MarcaId], [NombreModelo]) VALUES (source.[MarcaId], source.[NombreModelo]);

DROP TABLE #MarcasCatalogo;
GO

-- ============================================================================
-- 11. Usuarios del equipo de trabajo (Recepcionista y Mecánicos)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Usuarios] WHERE [Email] = N'recepcion@taller.com')
    INSERT INTO [dbo].[Usuarios] ([RolId], [Nombre], [Apellido], [Dni], [Email], [Telefono], [Domicilio], [FechaNacimiento], [Contrasena], [Activo])
    VALUES (2, N'Laura', N'Fernández', N'28111222', N'recepcion@taller.com', N'3624000101', N'Calle Mitre 350', CAST(N'1985-04-12T00:00:00' AS datetime2), N'CHANGE_ME', 1);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Usuarios] WHERE [Email] = N'carlos.mecanico@taller.com')
    INSERT INTO [dbo].[Usuarios] ([RolId], [Nombre], [Apellido], [Dni], [Email], [Telefono], [Domicilio], [FechaNacimiento], [Contrasena], [Activo])
    VALUES (3, N'Carlos', N'Benítez', N'32444555', N'carlos.mecanico@taller.com', N'3624000102', N'Av. Sarmiento 1200', CAST(N'1987-08-20T00:00:00' AS datetime2), N'CHANGE_ME', 1);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Usuarios] WHERE [Email] = N'marcos.mecanico@taller.com')
    INSERT INTO [dbo].[Usuarios] ([RolId], [Nombre], [Apellido], [Dni], [Email], [Telefono], [Domicilio], [FechaNacimiento], [Contrasena], [Activo])
    VALUES (3, N'Marcos', N'Romero', N'35666777', N'marcos.mecanico@taller.com', N'3624000103', N'Calle Vedia 560', CAST(N'1990-11-15T00:00:00' AS datetime2), N'CHANGE_ME', 1);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Usuarios] WHERE [Email] = N'lucas.mecanico@taller.com')
    INSERT INTO [dbo].[Usuarios] ([RolId], [Nombre], [Apellido], [Dni], [Email], [Telefono], [Domicilio], [FechaNacimiento], [Contrasena], [Activo])
    VALUES (3, N'Lucas', N'Almirón', N'38888999', N'lucas.mecanico@taller.com', N'3624000104', N'Av. Castelli 980', CAST(N'1994-02-28T00:00:00' AS datetime2), N'CHANGE_ME', 1);
GO
-- ============================================================================
-- 12. Clientes (55 clientes del mercado local argentino)
-- ============================================================================
CREATE TABLE #ClientesSeed (
    [Nombre] NVARCHAR(100),
    [Apellido] NVARCHAR(100),
    [Dni] NVARCHAR(8),
    [FechaNacimiento] DATETIME2,
    [Direccion] NVARCHAR(200),
    [Email] NVARCHAR(MAX),
    [Telefono] NVARCHAR(MAX)
);

INSERT INTO #ClientesSeed ([Nombre], [Apellido], [Dni], [FechaNacimiento], [Direccion], [Email], [Telefono])
VALUES    (N'Juan', N'Pérez', N'28341901', CAST(N'1981-03-15T00:00:00' AS DATETIME2), N'Av. Castelli 1240', N'juan.perez@gmail.com', N'3624112233'),
    (N'María', N'González', N'30452812', CAST(N'1983-07-22T00:00:00' AS DATETIME2), N'Calle Alvear 450', N'maria.gonzalez@hotmail.com', N'3624223344'),
    (N'Carlos', N'Rodríguez', N'25619304', CAST(N'1977-11-04T00:00:00' AS DATETIME2), N'Av. 9 de Julio 1820', N'carlos.rodriguez@yahoo.com.ar', N'3624334455'),
    (N'Ana', N'Martínez', N'33789451', CAST(N'1988-02-18T00:00:00' AS DATETIME2), N'Santa María de Oro 320', N'ana.martinez@gmail.com', N'3624445566'),
    (N'Lucas', N'López', N'35123984', CAST(N'1990-09-30T00:00:00' AS DATETIME2), N'Av. Italia 890', N'lucas.lopez@outlook.com', N'3624556677'),
    (N'Florencia', N'Gómez', N'32876123', CAST(N'1987-05-12T00:00:00' AS DATETIME2), N'Julio A. Roca 215', N'flor.gomez@gmail.com', N'3624667788'),
    (N'Matías', N'Díaz', N'29431876', CAST(N'1982-12-08T00:00:00' AS DATETIME2), N'Av. Sarmiento 1540', N'matias.diaz@gmail.com', N'3624778899'),
    (N'Sofía', N'Alvarez', N'36987452', CAST(N'1992-04-25T00:00:00' AS DATETIME2), N'Marcelo T. de Alvear 620', N'sofia.alvarez@gmail.com', N'3624889900'),
    (N'Martín', N'Romero', N'27845129', CAST(N'1980-08-14T00:00:00' AS DATETIME2), N'Av. Belgrano 1120', N'martin.romero@hotmail.com', N'3624990011'),
    (N'Camila', N'Sosa', N'38124963', CAST(N'1994-01-19T00:00:00' AS DATETIME2), N'Pueyrredón 780', N'camila.sosa@gmail.com', N'3624001122'),
    (N'Nicolás', N'Torres', N'34562187', CAST(N'1989-10-03T00:00:00' AS DATETIME2), N'Av. San Martín 1350', N'nicolas.torres@outlook.com', N'3624123456'),
    (N'Valentina', N'Ruiz', N'37458912', CAST(N'1993-06-28T00:00:00' AS DATETIME2), N'Obligado 340', N'valentina.ruiz@gmail.com', N'3624234567'),
    (N'Gonzalo', N'Benítez', N'26987413', CAST(N'1978-03-09T00:00:00' AS DATETIME2), N'Av. Las Heras 920', N'gonzalo.benitez@yahoo.com.ar', N'3624345678'),
    (N'Lucía', N'Flores', N'39125874', CAST(N'1995-11-17T00:00:00' AS DATETIME2), N'Güemes 560', N'lucia.flores@gmail.com', N'3624456789'),
    (N'Tomás', N'Acosta', N'31784523', CAST(N'1985-08-21T00:00:00' AS DATETIME2), N'Av. Mac Lean 1740', N'tomas.acosta@gmail.com', N'3624567890'),
    (N'Julieta', N'Medina', N'35896321', CAST(N'1991-02-14T00:00:00' AS DATETIME2), N'Don Bosco 430', N'julieta.medina@gmail.com', N'3624678901'),
    (N'Diego', N'Herrera', N'24896325', CAST(N'1975-09-05T00:00:00' AS DATETIME2), N'Av. Hernandarias 850', N'diego.herrera@hotmail.com', N'3624789012'),
    (N'Paula', N'Aguirre', N'33458962', CAST(N'1988-04-11T00:00:00' AS DATETIME2), N'Vedia 290', N'paula.aguirre@gmail.com', N'3624890123'),
    (N'Agustín', N'Pereira', N'36125478', CAST(N'1991-12-02T00:00:00' AS DATETIME2), N'Av. 25 de Mayo 1680', N'agustin.pereira@outlook.com', N'3624901234'),
    (N'Micaela', N'Castro', N'38745219', CAST(N'1994-07-07T00:00:00' AS DATETIME2), N'Brown 610', N'micaela.castro@gmail.com', N'3624012345'),
    (N'Federico', N'Giménez', N'28965412', CAST(N'1981-10-29T00:00:00' AS DATETIME2), N'Av. Alvear 1920', N'fede.gimenez@gmail.com', N'3624113355'),
    (N'Daniela', N'Silva', N'31258964', CAST(N'1985-05-16T00:00:00' AS DATETIME2), N'Mitre 480', N'daniela.silva@hotmail.com', N'3624224466'),
    (N'Gabriel', N'Morales', N'27458963', CAST(N'1979-01-23T00:00:00' AS DATETIME2), N'Av. Chaco 1140', N'gabriel.morales@yahoo.com.ar', N'3624335577'),
    (N'Victoria', N'Cabrera', N'37896541', CAST(N'1993-09-12T00:00:00' AS DATETIME2), N'Yrigoyen 730', N'victoria.cabrera@gmail.com', N'3624446688'),
    (N'Esteban', N'Vega', N'30125896', CAST(N'1983-06-04T00:00:00' AS DATETIME2), N'Av. Rivadavia 1410', N'esteban.vega@gmail.com', N'3624557799'),
    (N'Mariana', N'Ríos', N'34896521', CAST(N'1989-11-20T00:00:00' AS DATETIME2), N'Liniers 350', N'mariana.rios@outlook.com', N'3624668800'),
    (N'Facundo', N'Duarte', N'36541289', CAST(N'1992-03-31T00:00:00' AS DATETIME2), N'Av. Edison 1830', N'facundo.duarte@gmail.com', N'3624779911'),
    (N'Antonella', N'Molina', N'39458712', CAST(N'1996-08-15T00:00:00' AS DATETIME2), N'Salta 520', N'antonella.molina@gmail.com', N'3624880022'),
    (N'Rodrigo', N'Ortiz', N'29874512', CAST(N'1982-04-09T00:00:00' AS DATETIME2), N'Av. Avalos 960', N'rodrigo.ortiz@hotmail.com', N'3624991133'),
    (N'Belén', N'Rivas', N'35412896', CAST(N'1990-10-24T00:00:00' AS DATETIME2), N'Frondizi 280', N'belen.rivas@gmail.com', N'3624002244'),
    (N'Cristian', N'Cardozo', N'32145896', CAST(N'1986-07-18T00:00:00' AS DATETIME2), N'Av. Lavalle 1520', N'cristian.cardozo@gmail.com', N'3624114477'),
    (N'Natalia', N'Núñez', N'26541289', CAST(N'1978-12-05T00:00:00' AS DATETIME2), N'Perón 410', N'natalia.nunez@yahoo.com.ar', N'3624225588'),
    (N'Ignacio', N'Vera', N'38451296', CAST(N'1994-02-27T00:00:00' AS DATETIME2), N'Av. Paraguay 1210', N'ignacio.vera@outlook.com', N'3624336699'),
    (N'Melina', N'Luna', N'33985412', CAST(N'1988-09-08T00:00:00' AS DATETIME2), N'Corrientes 640', N'melina.luna@gmail.com', N'3624447700'),
    (N'Lautaro', N'Juárez', N'37125896', CAST(N'1993-01-14T00:00:00' AS DATETIME2), N'Av. Vélez Sarsfield 1650', N'lautaro.juarez@gmail.com', N'3624558811'),
    (N'Daiana', N'Maidana', N'34125896', CAST(N'1989-06-19T00:00:00' AS DATETIME2), N'Entre Ríos 390', N'daiana.maidana@hotmail.com', N'3624669922'),
    (N'Maximiliano', N'Godoy', N'28451296', CAST(N'1981-08-30T00:00:00' AS DATETIME2), N'Av. Moreno 1180', N'maxi.godoy@gmail.com', N'3624770033'),
    (N'Sabrina', N'Barrientos', N'36854129', CAST(N'1992-11-11T00:00:00' AS DATETIME2), N'Jujuy 510', N'sabrina.barrientos@gmail.com', N'3624881144'),
    (N'Leandro', N'Ferreyra', N'30985412', CAST(N'1984-05-03T00:00:00' AS DATETIME2), N'Av. Wilde 1430', N'leandro.ferreyra@outlook.com', N'3624992255'),
    (N'Rocío', N'Ibarra', N'39854126', CAST(N'1996-03-22T00:00:00' AS DATETIME2), N'Catamarca 270', N'rocio.ibarra@gmail.com', N'3624003366'),
    (N'Santiago', N'Coronel', N'27125896', CAST(N'1979-10-15T00:00:00' AS DATETIME2), N'Av. Sabin 1790', N'santiago.coronel@gmail.com', N'3624115588'),
    (N'Romina', N'Leguizamón', N'35985412', CAST(N'1991-07-09T00:00:00' AS DATETIME2), N'La Rioja 460', N'romina.leguizamon@yahoo.com.ar', N'3624226699'),
    (N'Damián', N'Ojeda', N'32985412', CAST(N'1987-04-01T00:00:00' AS DATETIME2), N'Av. Martina 1250', N'damian.ojeda@gmail.com', N'3624337700'),
    (N'Yamila', N'Mansilla', N'37985412', CAST(N'1993-12-26T00:00:00' AS DATETIME2), N'San Juan 580', N'yamila.mansilla@hotmail.com', N'3624448811'),
    (N'Franco', N'Galeano', N'31985412', CAST(N'1986-02-07T00:00:00' AS DATETIME2), N'Av. Alvear 2100', N'franco.galeano@gmail.com', N'3624559922'),
    (N'Gisela', N'Valdez', N'34985412', CAST(N'1990-08-16T00:00:00' AS DATETIME2), N'Mendoza 310', N'gisela.valdez@outlook.com', N'3624660033'),
    (N'Hernán', N'Escalante', N'29125896', CAST(N'1982-11-28T00:00:00' AS DATETIME2), N'Av. Castelli 1890', N'hernan.escalante@gmail.com', N'3624771144'),
    (N'Noelia', N'Cardozo', N'36451298', CAST(N'1992-06-10T00:00:00' AS DATETIME2), N'Posadas 490', N'noelia.cardozo@gmail.com', N'3624882255'),
    (N'Bruno', N'Peralta', N'38125896', CAST(N'1994-09-04T00:00:00' AS DATETIME2), N'Av. 9 de Julio 2450', N'bruno.peralta@gmail.com', N'3624993366'),
    (N'Andrea', N'Bogado', N'33125896', CAST(N'1987-01-21T00:00:00' AS DATETIME2), N'Necochea 370', N'andrea.bogado@yahoo.com.ar', N'3624004477'),
    (N'Sebastián', N'Villalba', N'30784512', CAST(N'1984-08-17T00:00:00' AS DATETIME2), N'Av. Belgrano 1650', N'sebastian.villalba@gmail.com', N'3624116699'),
    (N'Lorena', N'Aquino', N'35784512', CAST(N'1991-04-12T00:00:00' AS DATETIME2), N'Arturo Frondizi 820', N'lorena.aquino@hotmail.com', N'3624227700'),
    (N'Emiliano', N'Gauto', N'27985412', CAST(N'1980-11-06T00:00:00' AS DATETIME2), N'Av. Italia 1490', N'emiliano.gauto@gmail.com', N'3624338811'),
    (N'Tamara', N'Salinas', N'37412589', CAST(N'1993-05-29T00:00:00' AS DATETIME2), N'Ameghino 540', N'tamara.salinas@outlook.com', N'3624449922'),
    (N'Joaquín', N'Paz', N'39145896', CAST(N'1995-10-14T00:00:00' AS DATETIME2), N'Av. San Martín 1980', N'joaquin.paz@gmail.com', N'3624550033');
MERGE [dbo].[Cliente] AS target
USING #ClientesSeed AS source
ON target.[Dni] = source.[Dni]
WHEN NOT MATCHED THEN
    INSERT ([Nombre], [Apellido], [Dni], [FechaNacimiento], [Direccion], [Email], [Telefono], [Activo])
    VALUES (source.[Nombre], source.[Apellido], source.[Dni], source.[FechaNacimiento], source.[Direccion], source.[Email], source.[Telefono], 1);

DROP TABLE #ClientesSeed;
GO
-- ============================================================================
-- 13. Vehículos (55 vehículos vinculados a modelos oficiales)
-- ============================================================================
CREATE TABLE #VehiculosSeed (
    [Marca] NVARCHAR(100),
    [Modelo] NVARCHAR(100),
    [Anio] INT,
    [Patente] NVARCHAR(15),
    [KmActual] INT
);

INSERT INTO #VehiculosSeed ([Marca], [Modelo], [Anio], [Patente], [KmActual])
VALUES    (N'Toyota', N'Corolla', 2019, N'AD123AA', 54000),
    (N'Toyota', N'Hilux', 2021, N'AE456BB', 78000),
    (N'Toyota', N'Yaris', 2020, N'AD789CC', 41000),
    (N'Toyota', N'Etios', 2018, N'AC321DD', 75000),
    (N'Toyota', N'Corolla Cross', 2022, N'AF654EE', 32000),
    (N'Volkswagen', N'Gol', 2017, N'AB987FF', 89000),
    (N'Volkswagen', N'Amarok', 2020, N'AD159GG', 92000),
    (N'Volkswagen', N'Polo', 2021, N'AE357HH', 37000),
    (N'Volkswagen', N'Saveiro', 2016, N'AA753II', 115000),
    (N'Volkswagen', N'T-Cross', 2022, N'AF951JJ', 28000),
    (N'Ford', N'Ranger', 2019, N'AD852KK', 84000),
    (N'Ford', N'Focus', 2018, N'AC147LL', 73000),
    (N'Ford', N'Fiesta', 2017, N'AB258MM', 81000),
    (N'Ford', N'Ka', 2019, N'AD369NN', 59000),
    (N'Ford', N'EcoSport', 2016, N'AA963OO', 102000),
    (N'Chevrolet', N'Cruze', 2020, N'AD741PP', 49000),
    (N'Chevrolet', N'Onix', 2021, N'AE852QQ', 35000),
    (N'Chevrolet', N'Tracker', 2022, N'AF963RR', 26000),
    (N'Chevrolet', N'S10', 2018, N'AC159SS', 98000),
    (N'Chevrolet', N'Spin', 2017, N'AB357TT', 87000),
    (N'Fiat', N'Cronos', 2022, N'AF753UU', 31000),
    (N'Fiat', N'Toro', 2020, N'AD951VV', 64000),
    (N'Fiat', N'Strada', 2021, N'AE159WW', 47000),
    (N'Fiat', N'Argo', 2019, N'AD357XX', 58000),
    (N'Fiat', N'Mobi', 2018, N'AC753YY', 62000),
    (N'Peugeot', N'208', 2022, N'AF159ZZ', 29000),
    (N'Peugeot', N'2008', 2020, N'AD357AB', 51000),
    (N'Peugeot', N'Partner', 2017, N'AB753CD', 108000),
    (N'Peugeot', N'308', 2016, N'AA951EF', 94000),
    (N'Renault', N'Sandero', 2019, N'AD159GH', 63000),
    (N'Renault', N'Kangoo', 2021, N'AE357IJ', 44000),
    (N'Renault', N'Duster', 2018, N'AC753KL', 79000),
    (N'Renault', N'Logan', 2017, N'AB951MN', 86000),
    (N'Renault', N'Stepway', 2020, N'AD159OP', 52000),
    (N'Renault', N'Kwid', 2019, N'AD357QR', 48000),
    (N'Nissan', N'Frontier', 2021, N'AE753ST', 67000),
    (N'Nissan', N'Kicks', 2020, N'AD951UV', 43000),
    (N'Nissan', N'Versa', 2022, N'AF159WX', 25000),
    (N'Honda', N'Civic', 2018, N'AC357YZ', 69000),
    (N'Honda', N'HR-V', 2021, N'AE753AA', 38000),
    (N'Citroën', N'C3', 2022, N'AF951BB', 22000),
    (N'Citroën', N'C4 Cactus', 2020, N'AD159CC', 53000),
    (N'Citroën', N'Berlingo', 2018, N'AC357DD', 91000),
    (N'Jeep', N'Renegade', 2021, N'AE753EE', 39000),
    (N'Jeep', N'Compass', 2020, N'AD951FF', 56000),
    (N'Toyota', N'Corolla', 2021, N'AE159GG', 42000),
    (N'Toyota', N'Hilux', 2019, N'AD357HH', 88000),
    (N'Volkswagen', N'Gol', 2018, N'AC753II', 76000),
    (N'Fiat', N'Cronos', 2021, N'AE951JJ', 36000),
    (N'Ford', N'Ranger', 2022, N'AF159KK', 31000),
    (N'Peugeot', N'208', 2021, N'AE357LL', 33000),
    (N'Chevrolet', N'Cruze', 2019, N'AD753MM', 61000),
    (N'Renault', N'Kangoo', 2020, N'AD951NN', 54000),
    (N'Volkswagen', N'Amarok', 2022, N'AF159OO', 35000),
    (N'Toyota', N'Yaris', 2022, N'AF357PP', 24000);
MERGE [dbo].[Vehiculo] AS target
USING (
    SELECT mo.[Id] AS [ModeloId], s.[Anio], s.[Patente], s.[KmActual]
    FROM #VehiculosSeed s
    INNER JOIN [dbo].[Marca] ma ON ma.[NombreMarca] = s.[Marca]
    INNER JOIN [dbo].[Modelo] mo ON mo.[MarcaId] = ma.[Id] AND mo.[NombreModelo] = s.[Modelo]
) AS source
ON target.[Patente] = source.[Patente]
WHEN NOT MATCHED THEN
    INSERT ([ModeloId], [Anio], [Patente], [KmActual])
    VALUES (source.[ModeloId], source.[Anio], source.[Patente], source.[KmActual]);

DROP TABLE #VehiculosSeed;
GO
-- ============================================================================
-- 14. PropietarioVehiculo (vinculación de titularidad Cliente <-> Vehículo)
-- ============================================================================
CREATE TABLE #PropietariosSeed (
    [DniCliente] NVARCHAR(8),
    [PatenteVehiculo] NVARCHAR(15),
    [FechaAdquisicion] DATETIME2
);

INSERT INTO #PropietariosSeed ([DniCliente], [PatenteVehiculo], [FechaAdquisicion])
VALUES    (N'28341901', N'AD123AA', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'30452812', N'AE456BB', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'25619304', N'AD789CC', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'33789451', N'AC321DD', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'35123984', N'AF654EE', CAST(N'2022-03-01T00:00:00' AS DATETIME2)),
    (N'32876123', N'AB987FF', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'29431876', N'AD159GG', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'36987452', N'AE357HH', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'27845129', N'AA753II', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'38124963', N'AF951JJ', CAST(N'2022-03-01T00:00:00' AS DATETIME2)),
    (N'34562187', N'AD852KK', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'37458912', N'AC147LL', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'26987413', N'AB258MM', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'39125874', N'AD369NN', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'31784523', N'AA963OO', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'35896321', N'AD741PP', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'24896325', N'AE852QQ', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'33458962', N'AF963RR', CAST(N'2022-03-01T00:00:00' AS DATETIME2)),
    (N'36125478', N'AC159SS', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'38745219', N'AB357TT', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'28965412', N'AF753UU', CAST(N'2022-03-01T00:00:00' AS DATETIME2)),
    (N'31258964', N'AD951VV', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'27458963', N'AE159WW', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'37896541', N'AD357XX', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'30125896', N'AC753YY', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'34896521', N'AF159ZZ', CAST(N'2022-03-01T00:00:00' AS DATETIME2)),
    (N'36541289', N'AD357AB', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'39458712', N'AB753CD', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'29874512', N'AA951EF', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'35412896', N'AD159GH', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'32145896', N'AE357IJ', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'26541289', N'AC753KL', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'38451296', N'AB951MN', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'33985412', N'AD159OP', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'37125896', N'AD357QR', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'34125896', N'AE753ST', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'28451296', N'AD951UV', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'36854129', N'AF159WX', CAST(N'2022-03-01T00:00:00' AS DATETIME2)),
    (N'30985412', N'AC357YZ', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'39854126', N'AE753AA', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'27125896', N'AF951BB', CAST(N'2022-03-01T00:00:00' AS DATETIME2)),
    (N'35985412', N'AD159CC', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'32985412', N'AC357DD', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'37985412', N'AE753EE', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'31985412', N'AD951FF', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'34985412', N'AE159GG', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'29125896', N'AD357HH', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'36451298', N'AC753II', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'38125896', N'AE951JJ', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'33125896', N'AF159KK', CAST(N'2022-03-01T00:00:00' AS DATETIME2)),
    (N'30784512', N'AE357LL', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'35784512', N'AD753MM', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'27985412', N'AD951NN', CAST(N'2021-03-01T00:00:00' AS DATETIME2)),
    (N'37412589', N'AF159OO', CAST(N'2022-03-01T00:00:00' AS DATETIME2)),
    (N'39145896', N'AF357PP', CAST(N'2022-03-01T00:00:00' AS DATETIME2));
MERGE [dbo].[PropietarioVehiculo] AS target
USING (
    SELECT c.[Id] AS [ClienteId], v.[Id] AS [VehiculoId], s.[FechaAdquisicion]
    FROM #PropietariosSeed s
    INNER JOIN [dbo].[Cliente] c ON c.[Dni] = s.[DniCliente]
    INNER JOIN [dbo].[Vehiculo] v ON v.[Patente] = s.[PatenteVehiculo]
) AS source
ON target.[ClienteId] = source.[ClienteId] AND target.[VehiculoId] = source.[VehiculoId]
WHEN NOT MATCHED THEN
    INSERT ([ClienteId], [VehiculoId], [FechaAdquisicion], [FechaVenta], [EsActual])
    VALUES (source.[ClienteId], source.[VehiculoId], source.[FechaAdquisicion], NULL, 1);

DROP TABLE #PropietariosSeed;
GO
-- ============================================================================
-- 15. Órdenes de Servicio (RegistroServicio), Detalles, Facturas y Pagos
--     (Desde Junio 2026 a Octubre 2026, con estados realistas)
-- ============================================================================
SET NOCOUNT ON;

DECLARE @TallerId INT = (SELECT TOP 1 [Id] FROM [dbo].[Taller] WHERE [Nombre] = N'Taller PRO');
IF @TallerId IS NULL SET @TallerId = 1;

DECLARE @UsuarioRecepId INT = (SELECT TOP 1 [Id] FROM [dbo].[Usuarios] WHERE [Email] = N'recepcion@taller.com');
IF @UsuarioRecepId IS NULL SET @UsuarioRecepId = (SELECT TOP 1 [Id] FROM [dbo].[Usuarios] WHERE [RolId] = 1);

DECLARE @MecCarlosId INT = (SELECT TOP 1 [Id] FROM [dbo].[Usuarios] WHERE [Email] = N'carlos.mecanico@taller.com');
DECLARE @MecMarcosId INT = (SELECT TOP 1 [Id] FROM [dbo].[Usuarios] WHERE [Email] = N'marcos.mecanico@taller.com');
DECLARE @MecLucasId INT  = (SELECT TOP 1 [Id] FROM [dbo].[Usuarios] WHERE [Email] = N'lucas.mecanico@taller.com');

IF @MecCarlosId IS NULL SET @MecCarlosId = @UsuarioRecepId;
IF @MecMarcosId IS NULL SET @MecMarcosId = @UsuarioRecepId;
IF @MecLucasId IS NULL  SET @MecLucasId  = @UsuarioRecepId;

DECLARE @MetodoEfectivoId INT = (SELECT TOP 1 [Id] FROM [dbo].[MetodoPago] WHERE [Nombre] = N'Efectivo');
DECLARE @MetodoTarjetaId INT  = (SELECT TOP 1 [Id] FROM [dbo].[MetodoPago] WHERE [Nombre] = N'Tarjeta');
IF @MetodoEfectivoId IS NULL SET @MetodoEfectivoId = 1;
IF @MetodoTarjetaId IS NULL  SET @MetodoTarjetaId  = 2;

DECLARE @VehiculoId INT;
DECLARE @RegistroId INT;
DECLARE @FacturaId INT;
DECLARE @ServicioId INT;
DECLARE @PrecioServicio DECIMAL(18,2);
DECLARE @KmCalculado INT;
DECLARE @TotalOrden DECIMAL(10,2);
DECLARE @MetodoPagoId INT;
DECLARE @ContadorFactura INT = 1;

-- --- Orden 01 (AD123AA - 2026-06-02 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AD123AA';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-06-02 08:30:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-06-02 08:30:00' AS DATETIME2), 54000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Aceite de Motor';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 65000.00, 1, N'Presencial', N'Finalizada', N'Aceite sintético 5W-30');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Filtro de Aceite';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 34000.00, 2, N'Presencial', N'Finalizada', N'Filtro original');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Filtro Aire';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 25000.00, 3, N'Presencial', N'Finalizada', N'Reemplazo de elemento filtrante');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-06-02 08:30:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-06-02 08:30:00' AS DATETIME2));
    END
END;

-- --- Orden 02 (AE456BB - 2026-06-04 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AE456BB';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-06-04 09:15:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-06-04 09:15:00' AS DATETIME2), 78000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Alineación y Balanceo';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 1, 38000.00, 1, N'Presencial', N'Finalizada', N'Control en las 4 ruedas');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Rotación de Neumáticos';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 1, 22000.00, 2, N'Presencial', N'Finalizada', N'Ajuste de presión según manual');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-06-04 09:15:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-06-04 09:15:00' AS DATETIME2));
    END
END;

-- --- Orden 03 (AD789CC - 2026-06-07 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AD789CC';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-06-07 10:00:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-06-07 10:00:00' AS DATETIME2), 41000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Pastillas de Freno';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 1, 45000.00, 1, N'Presencial', N'Finalizada', N'Pastillas delanteras reemplazadas');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Líquido de Frenos';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 1, 30000.00, 2, N'Presencial', N'Finalizada', N'Purga de circuito completo');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-06-07 10:00:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-06-07 10:00:00' AS DATETIME2));
    END
END;

-- --- Orden 04 (AC321DD - 2026-06-09 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AC321DD';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-06-09 11:20:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-06-09 11:20:00' AS DATETIME2), 75000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Batería';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 95000.00, 1, N'Presencial', N'Finalizada', N'Batería Moura 12V 65Ah');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Diagnóstico Computarizado OBD2';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 20000.00, 2, N'Presencial', N'Finalizada', N'Borrado de errores de baja tensión');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-06-09 11:20:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-06-09 11:20:00' AS DATETIME2));
    END
END;

-- --- Orden 05 (AF654EE - 2026-06-12 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AF654EE';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-06-12 14:00:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-06-12 14:00:00' AS DATETIME2), 69000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Bujías';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 4, 7000.00, 1, N'Presencial', N'Finalizada', N'Bujías NGK Iridium');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Limpieza de Inyectores';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 1, 48000.00, 2, N'Presencial', N'Finalizada', N'Limpieza ultrasónica en banco');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-06-12 14:00:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-06-12 14:00:00' AS DATETIME2));
    END
END;

-- --- Orden 06 (AB987FF - 2026-06-15 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AB987FF';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-06-15 08:45:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-06-15 08:45:00' AS DATETIME2), 89000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Aceite de Motor';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 1, 65000.00, 1, N'Presencial', N'Finalizada', N'Servicio preventivo');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Filtro de Aceite';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 1, 34000.00, 2, N'Presencial', N'Finalizada', N'Filtro sustituido');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-06-15 08:45:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-06-15 08:45:00' AS DATETIME2));
    END
END;

-- --- Orden 07 (AD159GG - 2026-06-18 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AD159GG';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-06-18 10:30:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-06-18 10:30:00' AS DATETIME2), 92000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Recarga de Aire Acondicionado';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 55000.00, 1, N'Presencial', N'Finalizada', N'Prueba de vacío y recarga R134a');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-06-18 10:30:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-06-18 10:30:00' AS DATETIME2));
    END
END;

-- --- Orden 08 (AE357HH - 2026-06-21 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AE357HH';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-06-21 15:10:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-06-21 15:10:00' AS DATETIME2), 37000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Correa de Distribucion';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 1, 15000.00, 1, N'Presencial', N'Finalizada', N'Kit distribución y bomba de agua');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Servicio Completo';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 1, 120000.00, 2, N'Presencial', N'Finalizada', N'Inspección de 25 puntos de seguridad');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-06-21 15:10:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-06-21 15:10:00' AS DATETIME2));
    END
END;

-- --- Orden 09 (AA753II - 2026-06-24 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AA753II';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-06-24 09:00:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-06-24 09:00:00' AS DATETIME2), 115000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Alineación y Balanceo';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 1, 38000.00, 1, N'Presencial', N'Finalizada', N'Corrección de comba delantera');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-06-24 09:00:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-06-24 09:00:00' AS DATETIME2));
    END
END;

-- --- Orden 10 (AF951JJ - 2026-06-26 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AF951JJ';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-06-26 11:40:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-06-26 11:40:00' AS DATETIME2), 28000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Pastillas de Freno';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 45000.00, 1, N'Presencial', N'Finalizada', N'Discos en buen estado');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Escaneo General';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 25000.00, 2, N'Presencial', N'Finalizada', N'Sin fallas en centralita');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-06-26 11:40:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-06-26 11:40:00' AS DATETIME2));
    END
END;

-- --- Orden 11 (AD852KK - 2026-06-28 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AD852KK';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-06-28 16:00:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-06-28 16:00:00' AS DATETIME2), 84000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Aceite de Motor';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 1, 65000.00, 1, N'Presencial', N'Finalizada', N'Aceite semi-sintético 10W-40');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Filtro Aire';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 1, 25000.00, 2, N'Presencial', N'Finalizada', N'Filtro nuevo colocado');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-06-28 16:00:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-06-28 16:00:00' AS DATETIME2));
    END
END;

-- --- Orden 12 (AC147LL - 2026-06-30 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AC147LL';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-06-30 17:15:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-06-30 17:15:00' AS DATETIME2), 73000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Limpieza de Inyectores';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 1, 48000.00, 1, N'Presencial', N'Finalizada', N'Mejora de consumo y ralentí');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-06-30 17:15:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-06-30 17:15:00' AS DATETIME2));
    END
END;

-- --- Orden 13 (AB258MM - 2026-07-02 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AB258MM';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-07-02 08:30:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-07-02 08:30:00' AS DATETIME2), 81000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Aceite de Motor';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 65000.00, 1, N'Presencial', N'Finalizada', N'Aceite 5W-30 sintético');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Filtro de Aceite';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 34000.00, 2, N'Presencial', N'Finalizada', N'Filtro reemplazado');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Alineación y Balanceo';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 1, 38000.00, 3, N'Presencial', N'Finalizada', N'Balanceo dinámico');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-07-02 08:30:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-07-02 08:30:00' AS DATETIME2));
    END
END;

-- --- Orden 14 (AD369NN - 2026-07-05 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AD369NN';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-07-05 10:15:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-07-05 10:15:00' AS DATETIME2), 59000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Bujías';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 4, 7000.00, 1, N'Presencial', N'Finalizada', N'Juego de bujías nuevas');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Diagnóstico Computarizado OBD2';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 1, 20000.00, 2, N'Presencial', N'Finalizada', N'Sensor MAF chequeado');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-07-05 10:15:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-07-05 10:15:00' AS DATETIME2));
    END
END;

-- --- Orden 15 (AA963OO - 2026-07-08 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AA963OO';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-07-08 14:00:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-07-08 14:00:00' AS DATETIME2), 102000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Pastillas de Freno';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 45000.00, 1, N'Presencial', N'Finalizada', N'Pastillas cerámicas');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Líquido de Frenos';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 30000.00, 2, N'Presencial', N'Finalizada', N'Líquido DOT 4');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-07-08 14:00:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-07-08 14:00:00' AS DATETIME2));
    END
END;

-- --- Orden 16 (AD741PP - 2026-07-11 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AD741PP';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-07-11 09:20:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-07-11 09:20:00' AS DATETIME2), 49000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Recarga de Aire Acondicionado';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 1, 55000.00, 1, N'Presencial', N'Finalizada', N'Filtro de polen sustituido');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-07-11 09:20:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-07-11 09:20:00' AS DATETIME2));
    END
END;

-- --- Orden 17 (AE852QQ - 2026-07-14 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AE852QQ';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-07-14 11:30:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-07-14 11:30:00' AS DATETIME2), 35000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Batería';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 1, 95000.00, 1, N'Presencial', N'Finalizada', N'Batería Willard 12V 75Ah');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-07-14 11:30:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-07-14 11:30:00' AS DATETIME2));
    END
END;

-- --- Orden 18 (AF963RR - 2026-07-17 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AF963RR';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-07-17 15:45:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-07-17 15:45:00' AS DATETIME2), 26000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Aceite de Motor';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 65000.00, 1, N'Presencial', N'Finalizada', N'Mantenimiento periódico');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Filtro Aire';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 25000.00, 2, N'Presencial', N'Finalizada', N'Caja filtrera higienizada');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-07-17 15:45:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-07-17 15:45:00' AS DATETIME2));
    END
END;

-- --- Orden 19 (AC159SS - 2026-07-20 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AC159SS';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-07-20 08:50:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-07-20 08:50:00' AS DATETIME2), 98000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Servicio Completo';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 1, 120000.00, 1, N'Presencial', N'Finalizada', N'Revisión preventiva antes de viaje');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-07-20 08:50:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-07-20 08:50:00' AS DATETIME2));
    END
END;

-- --- Orden 20 (AB357TT - 2026-07-22 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AB357TT';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-07-22 10:40:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-07-22 10:40:00' AS DATETIME2), 87000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Alineación y Balanceo';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 1, 38000.00, 1, N'Presencial', N'Finalizada', N'Alineado láser');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Rotación de Neumáticos';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 1, 22000.00, 2, N'Presencial', N'Finalizada', N'Desgaste uniforme');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-07-22 10:40:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-07-22 10:40:00' AS DATETIME2));
    END
END;

-- --- Orden 21 (AF753UU - 2026-07-25 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AF753UU';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-07-25 12:10:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-07-25 12:10:00' AS DATETIME2), 31000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Limpieza de Inyectores';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 48000.00, 1, N'Presencial', N'Finalizada', N'Desarme y reemplazo de o-rings');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-07-25 12:10:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-07-25 12:10:00' AS DATETIME2));
    END
END;

-- --- Orden 22 (AD951VV - 2026-07-27 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AD951VV';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-07-27 14:30:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-07-27 14:30:00' AS DATETIME2), 64000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Pastillas de Freno';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 1, 45000.00, 1, N'Presencial', N'Finalizada', N'Revisión de campanas traseras');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-07-27 14:30:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-07-27 14:30:00' AS DATETIME2));
    END
END;

-- --- Orden 23 (AE159WW - 2026-07-29 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AE159WW';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-07-29 16:20:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-07-29 16:20:00' AS DATETIME2), 47000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Correa de Distribucion';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 1, 15000.00, 1, N'Presencial', N'Finalizada', N'Tensor y rodillo guía nuevos');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-07-29 16:20:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-07-29 16:20:00' AS DATETIME2));
    END
END;

-- --- Orden 24 (AD357XX - 2026-07-31 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AD357XX';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-07-31 17:50:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-07-31 17:50:00' AS DATETIME2), 58000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Diagnóstico Computarizado OBD2';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 20000.00, 1, N'Presencial', N'Finalizada', N'Luz de Check Engine apagada');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-07-31 17:50:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-07-31 17:50:00' AS DATETIME2));
    END
END;

-- --- Orden 25 (AC753YY - 2026-08-03 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AC753YY';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-08-03 08:30:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-08-03 08:30:00' AS DATETIME2), 62000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Aceite de Motor';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 1, 65000.00, 1, N'Presencial', N'Finalizada', N'Aceite Castrol 5W-40');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Filtro de Aceite';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 1, 34000.00, 2, N'Presencial', N'Finalizada', N'Filtro Mann');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-08-03 08:30:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-08-03 08:30:00' AS DATETIME2));
    END
END;

-- --- Orden 26 (AF159ZZ - 2026-08-06 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AF159ZZ';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-08-06 09:40:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-08-06 09:40:00' AS DATETIME2), 29000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Alineación y Balanceo';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 1, 38000.00, 1, N'Presencial', N'Finalizada', N'Calibración de tren delantero');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-08-06 09:40:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-08-06 09:40:00' AS DATETIME2));
    END
END;

-- --- Orden 27 (AD357AB - 2026-08-09 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AD357AB';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-08-09 11:15:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-08-09 11:15:00' AS DATETIME2), 51000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Pastillas de Freno';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 45000.00, 1, N'Presencial', N'Finalizada', N'Cambio de líquido DOT 4');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Líquido de Frenos';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 30000.00, 2, N'Presencial', N'Finalizada', N'Sin pérdidas en caliper');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-08-09 11:15:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-08-09 11:15:00' AS DATETIME2));
    END
END;

-- --- Orden 28 (AB753CD - 2026-08-12 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AB753CD';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-08-12 14:00:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-08-12 14:00:00' AS DATETIME2), 108000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Batería';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 1, 95000.00, 1, N'Presencial', N'Finalizada', N'Revisión de alternador OK');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-08-12 14:00:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-08-12 14:00:00' AS DATETIME2));
    END
END;

-- --- Orden 29 (AA951EF - 2026-08-14 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AA951EF';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-08-14 15:30:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-08-14 15:30:00' AS DATETIME2), 94000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Recarga de Aire Acondicionado';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 1, 55000.00, 1, N'Presencial', N'Finalizada', N'Compresor verificado');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Filtro Aire';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 1, 25000.00, 2, N'Presencial', N'Finalizada', N'Habitáculo desinfectado');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-08-14 15:30:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-08-14 15:30:00' AS DATETIME2));
    END
END;

-- --- Orden 30 (AD159GH - 2026-08-17 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AD159GH';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-08-17 09:10:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-08-17 09:10:00' AS DATETIME2), 63000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Limpieza de Inyectores';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 48000.00, 1, N'Presencial', N'Finalizada', N'Caudal equilibrado');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-08-17 09:10:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-08-17 09:10:00' AS DATETIME2));
    END
END;

-- --- Orden 31 (AE357IJ - 2026-08-19 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AE357IJ';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-08-19 10:45:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-08-19 10:45:00' AS DATETIME2), 44000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Bujías';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 4, 7000.00, 1, N'Presencial', N'Finalizada', N'Cables de bujía testeados');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-08-19 10:45:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-08-19 10:45:00' AS DATETIME2));
    END
END;

-- --- Orden 32 (AC753KL - 2026-08-22 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AC753KL';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-08-22 13:20:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-08-22 13:20:00' AS DATETIME2), 79000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Servicio Completo';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 1, 120000.00, 1, N'Presencial', N'Finalizada', N'Control de suspensión y frenos');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-08-22 13:20:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-08-22 13:20:00' AS DATETIME2));
    END
END;

-- --- Orden 33 (AB951MN - 2026-08-25 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AB951MN';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-08-25 15:00:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-08-25 15:00:00' AS DATETIME2), 86000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Aceite de Motor';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 65000.00, 1, N'Presencial', N'Finalizada', N'Aceite Shell Helix');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Filtro de Aceite';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 34000.00, 2, N'Presencial', N'Finalizada', N'Arandela de tapón nueva');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Alineación y Balanceo';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 1, 38000.00, 3, N'Presencial', N'Finalizada', N'Alineado integral');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-08-25 15:00:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-08-25 15:00:00' AS DATETIME2));
    END
END;

-- --- Orden 34 (AD159OP - 2026-08-27 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AD159OP';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-08-27 16:40:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-08-27 16:40:00' AS DATETIME2), 52000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Escaneo General';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 1, 25000.00, 1, N'Presencial', N'Finalizada', N'Diagnóstico de inyección y ABS');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-08-27 16:40:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-08-27 16:40:00' AS DATETIME2));
    END
END;

-- --- Orden 35 (AD357QR - 2026-08-29 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AD357QR';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-08-29 11:30:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-08-29 11:30:00' AS DATETIME2), 48000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Pastillas de Freno';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 1, 45000.00, 1, N'Presencial', N'Finalizada', N'Limpieza de mordazas');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-08-29 11:30:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-08-29 11:30:00' AS DATETIME2));
    END
END;

-- --- Orden 36 (AE753ST - 2026-08-31 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AE753ST';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-08-31 17:00:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-08-31 17:00:00' AS DATETIME2), 67000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Rotación de Neumáticos';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 22000.00, 1, N'Presencial', N'Finalizada', N'Neumáticos traseros a posición delantera');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-08-31 17:00:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-08-31 17:00:00' AS DATETIME2));
    END
END;

-- --- Orden 37 (AD951UV - 2026-09-02 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AD951UV';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-09-02 08:30:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-09-02 08:30:00' AS DATETIME2), 43000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Aceite de Motor';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 1, 65000.00, 1, N'Presencial', N'Finalizada', N'Mantenimiento programado');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Filtro Aire';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 1, 25000.00, 2, N'Presencial', N'Finalizada', N'Filtro aire motor');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-09-02 08:30:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-09-02 08:30:00' AS DATETIME2));
    END
END;

-- --- Orden 38 (AF159WX - 2026-09-04 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AF159WX';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-09-04 10:15:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-09-04 10:15:00' AS DATETIME2), 25000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Alineación y Balanceo';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 1, 38000.00, 1, N'Presencial', N'Finalizada', N'Corrección de desvío a la derecha');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-09-04 10:15:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-09-04 10:15:00' AS DATETIME2));
    END
END;

-- --- Orden 39 (AC357YZ - 2026-09-07 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AC357YZ';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-09-07 14:20:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-09-07 14:20:00' AS DATETIME2), 69000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Pastillas de Freno';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 45000.00, 1, N'Presencial', N'Finalizada', N'Pastillas Fras-le');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-09-07 14:20:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-09-07 14:20:00' AS DATETIME2));
    END
END;

-- --- Orden 40 (AE753AA - 2026-09-10 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AE753AA';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-09-10 09:00:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-09-10 09:00:00' AS DATETIME2), 38000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Batería';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 1, 95000.00, 1, N'Presencial', N'Finalizada', N'Prueba de carga del alternador exitosa');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-09-10 09:00:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-09-10 09:00:00' AS DATETIME2));
    END
END;

-- --- Orden 41 (AF951BB - 2026-09-13 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AF951BB';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-09-13 11:45:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-09-13 11:45:00' AS DATETIME2), 39000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Recarga de Aire Acondicionado';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 1, 55000.00, 1, N'Presencial', N'Finalizada', N'Control estanqueidad');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-09-13 11:45:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-09-13 11:45:00' AS DATETIME2));
    END
END;

-- --- Orden 42 (AD159CC - 2026-09-16 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AD159CC';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-09-16 15:10:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-09-16 15:10:00' AS DATETIME2), 53000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Correa de Distribucion';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 15000.00, 1, N'Presencial', N'Finalizada', N'Cambio a los 80.000 km');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Servicio Completo';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 120000.00, 2, N'Presencial', N'Finalizada', N'Revisión integral de fluidos');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-09-16 15:10:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-09-16 15:10:00' AS DATETIME2));
    END
END;

-- --- Orden 43 (AC357DD - 2026-09-19 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AC357DD';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-09-19 09:30:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-09-19 09:30:00' AS DATETIME2), 91000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Limpieza de Inyectores';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 1, 48000.00, 1, N'Presencial', N'Finalizada', N'Pulverización y estanqueidad ensayadas');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-09-19 09:30:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-09-19 09:30:00' AS DATETIME2));
    END
END;

-- --- Orden 44 (AE753EE - 2026-09-22 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AE753EE';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-09-22 12:00:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-09-22 12:00:00' AS DATETIME2), 39000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Bujías';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 4, 7000.00, 1, N'Presencial', N'Finalizada', N'Bujías Bosch 4 electrodos');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Diagnóstico Computarizado OBD2';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 1, 20000.00, 2, N'Presencial', N'Finalizada', N'Lectura de parámetros en vivo');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-09-22 12:00:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-09-22 12:00:00' AS DATETIME2));
    END
END;

-- --- Orden 45 (AD951FF - 2026-09-25 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AD951FF';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-09-25 14:40:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-09-25 14:40:00' AS DATETIME2), 56000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Alineación y Balanceo';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 38000.00, 1, N'Presencial', N'Finalizada', N'Plomadas adhesivas en llantas de aleación');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-09-25 14:40:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-09-25 14:40:00' AS DATETIME2));
    END
END;

-- --- Orden 46 (AE159GG - 2026-09-27 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AE159GG';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-09-27 16:15:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-09-27 16:15:00' AS DATETIME2), 42000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Aceite de Motor';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 1, 65000.00, 1, N'Presencial', N'Finalizada', N'Aceite Total Quartz 7000');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Filtro de Aceite';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 1, 34000.00, 2, N'Presencial', N'Finalizada', N'Filtro cambiado');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-09-27 16:15:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-09-27 16:15:00' AS DATETIME2));
    END
END;

-- --- Orden 47 (AD357HH - 2026-09-29 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AD357HH';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-09-29 10:20:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-09-29 10:20:00' AS DATETIME2), 88000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Pastillas de Freno';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 1, 45000.00, 1, N'Presencial', N'Finalizada', N'Sensibilidad del pedal restablecida');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Líquido de Frenos';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 1, 30000.00, 2, N'Presencial', N'Finalizada', N'Líquido de frenos reemplazado');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-09-29 10:20:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-09-29 10:20:00' AS DATETIME2));
    END
END;

-- --- Orden 48 (AC753II - 2026-09-30 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AC753II';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-09-30 17:30:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-09-30 17:30:00' AS DATETIME2), 76000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Escaneo General';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 25000.00, 1, N'Presencial', N'Finalizada', N'Escaneo completo sin novedades');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-09-30 17:30:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-09-30 17:30:00' AS DATETIME2));
    END
END;

-- --- Orden 49 (AE951JJ - 2026-10-02 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AE951JJ';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-10-02 08:30:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-10-02 08:30:00' AS DATETIME2), 36000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Aceite de Motor';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 1, 65000.00, 1, N'Presencial', N'Finalizada', N'Servicio preventivo');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Filtro Aire';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 1, 25000.00, 2, N'Presencial', N'Finalizada', N'Filtro sustituido');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-10-02 08:30:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-10-02 08:30:00' AS DATETIME2));
    END
END;

-- --- Orden 50 (AF159KK - 2026-10-05 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AF159KK';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-10-05 10:00:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-10-05 10:00:00' AS DATETIME2), 31000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Alineación y Balanceo';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 1, 38000.00, 1, N'Presencial', N'Finalizada', N'Control de convergencia');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Rotación de Neumáticos';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 1, 22000.00, 2, N'Presencial', N'Finalizada', N'Rotación en X');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-10-05 10:00:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-10-05 10:00:00' AS DATETIME2));
    END
END;

-- --- Orden 51 (AE357LL - 2026-10-08 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AE357LL';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-10-08 14:15:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-10-08 14:15:00' AS DATETIME2), 33000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Pastillas de Freno';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 45000.00, 1, N'Presencial', N'Finalizada', N'Revisión general de frenos');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-10-08 14:15:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-10-08 14:15:00' AS DATETIME2));
    END
END;

-- --- Orden 52 (AD753MM - 2026-10-11 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AD753MM';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-10-11 09:10:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-10-11 09:10:00' AS DATETIME2), 61000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Batería';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 1, 95000.00, 1, N'Presencial', N'Finalizada', N'Borne limpiado y engrasado');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-10-11 09:10:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-10-11 09:10:00' AS DATETIME2));
    END
END;

-- --- Orden 53 (AD951NN - 2026-10-14 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AD951NN';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-10-14 11:30:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-10-14 11:30:00' AS DATETIME2), 54000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Recarga de Aire Acondicionado';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 1, 55000.00, 1, N'Presencial', N'Finalizada', N'Carga completa de gas');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-10-14 11:30:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-10-14 11:30:00' AS DATETIME2));
    END
END;

-- --- Orden 54 (AF159OO - 2026-10-16 - Pagada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AF159OO';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-10-16 15:45:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-10-16 15:45:00' AS DATETIME2), 35000, N'Pagada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Limpieza de Inyectores';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 48000.00, 1, N'Presencial', N'Finalizada', N'Excelente respuesta');
    END
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'2026-10-16 15:45:00' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'2026-10-16 15:45:00' AS DATETIME2));
    END
END;

-- --- Orden 55 (AF357PP - 2026-10-19 - Completada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AF357PP';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-10-19 09:00:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-10-19 09:00:00' AS DATETIME2), 24000, N'Completada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Aceite de Motor';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 1, 65000.00, 1, N'Presencial', N'Finalizada', N'Aceite sintético nuevo');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Filtro de Aceite';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 1, 34000.00, 2, N'Presencial', N'Finalizada', N'Filtro instalado');
    END
END;

-- --- Orden 56 (AD123AA - 2026-10-21 - Completada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AD123AA';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-10-21 11:15:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-10-21 11:15:00' AS DATETIME2), 56500, N'Completada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Alineación y Balanceo';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 1, 38000.00, 1, N'Presencial', N'Finalizada', N'Comprobación final terminada');
    END
END;

-- --- Orden 57 (AE456BB - 2026-10-22 - Completada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AE456BB';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-10-22 14:00:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-10-22 14:00:00' AS DATETIME2), 81000, N'Completada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Pastillas de Freno';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 45000.00, 1, N'Presencial', N'Finalizada', N'Trabajo finalizado');
    END
END;

-- --- Orden 58 (AD789CC - 2026-10-24 - En Proceso) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AD789CC';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-10-24 08:30:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-10-24 08:30:00' AS DATETIME2), 42500, N'En Proceso');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Correa de Distribucion';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 1, 15000.00, 1, N'Presencial', N'En Curso', N'Desarmando distribución');
    END
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Servicio Completo';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 1, 120000.00, 2, N'Presencial', N'Pendiente', N'Esperando finalización de correa');
    END
END;

-- --- Orden 59 (AC321DD - 2026-10-25 - En Proceso) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AC321DD';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-10-25 10:20:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-10-25 10:20:00' AS DATETIME2), 69200, N'En Proceso');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Diagnóstico Computarizado OBD2';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecCarlosId, @ServicioId, 1, 20000.00, 1, N'Presencial', N'En Curso', N'Escaneando módulos de carrocería');
    END
END;

-- --- Orden 60 (AF654EE - 2026-10-26 - Abierta) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AF654EE';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-10-26 09:10:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-10-26 09:10:00' AS DATETIME2), 69300, N'Abierta');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Alineación y Balanceo';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecMarcosId, @ServicioId, 1, 38000.00, 1, N'Presencial', N'Pendiente', N'Vehículo ingresado a la espera de turno');
    END
END;

-- --- Orden 61 (AB987FF - 2026-10-27 - Cancelada) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'AB987FF';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'2026-10-27 11:00:00' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'2026-10-27 11:00:00' AS DATETIME2), 89500, N'Cancelada');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Batería';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, @MecLucasId, @ServicioId, 1, 95000.00, 1, N'Presencial', N'Pendiente', N'Cliente desestimó presupuesto');
    END
END;
GO
