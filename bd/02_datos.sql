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
