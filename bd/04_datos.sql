-- ============================================================================
-- 04_datos.sql — Datos semilla del sistema de control vehicular.
-- ----------------------------------------------------------------------------
-- Autor: consolidación de scripts históricos (2026-10-12).
-- Contenido:
--   * Roles del sistema (Administrador = 1, Recepcionista = 2, Mecánico = 3).
--   * Catálogo completo de permisos (RBAC).
--   * Matriz Rol -> Permiso de los roles 1, 2 y 3.
--   * Taller, Cliente y Vehículos de demostración + PropietarioVehiculo.
--   * Catálogo ampliado de Marca/Modelo representativo del mercado argentino
--     (se preservan Toyota = Id 1 y Renault = Id 2).
--   * Usuarios semilla (Administrador "bootstrap", Mecánico y Recepción demo).
--   * Catálogo de Servicios y una orden de servicio de demostración
--     (RegistroServicio + DetalleServicio).
--   * ConfiguracionEmpresa y ConfiguracionBackup (filas singleton, Id = 1).
--   * Métodos de pago (Efectivo, Tarjeta).
-- Idempotencia:
--   * Marca/Modelo: tabla temporal #Marcas + MERGE por nombre (NombreMarca y
--     (MarcaId, NombreModelo)); no duplica filas ni reasigna Ids.
--   * Roles/Permisos/RolPermisos/Usuarios/Taller/Servicio/Cliente/Vehiculo y
--     las filas de demostración usan MERGE o IF NOT EXISTS / WHERE NOT EXISTS
--     por clave natural.
--   * SET IDENTITY_INSERT se usa solo donde un Id fijo es indispensable
--     (Roles 1/2/3, Cliente 1, Marca 1/2, Modelo 1/2) y se apaga de inmediato.
-- Requiere: 01_estructura.sql aplicado (y, para el comportamiento completo de
--   dominio, 03_triggers.sql). Compatible con SQL Server 2019 Express.
-- Codificación UTF-8. Orden de aplicación: 01 -> 02 -> 03 -> 04.
-- ============================================================================
USE [ControlVehicular];
GO

-- ============================================================================
-- 1. Roles (identidades fijas 1/2/3, requeridas por la matriz de permisos)
-- ============================================================================
SET IDENTITY_INSERT [dbo].[Roles] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [Id] = 1)
    INSERT INTO [dbo].[Roles] ([Id], [Nombre], [Descripcion], [Estado])
    VALUES (1, N'Administrador', N'Este administrado tiene permiso para todo el uso del sistema', 1);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [Id] = 2)
    INSERT INTO [dbo].[Roles] ([Id], [Nombre], [Descripcion], [Estado])
    VALUES (2, N'Recepcionista', N'La recepcionista puede crear orden de servicio, cliente y vehiculo', 1);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [Id] = 3)
    INSERT INTO [dbo].[Roles] ([Id], [Nombre], [Descripcion], [Estado])
    VALUES (3, N'Mecánico', N'Mecanico puede agregar servicio a la orden de servicio y tildar lo que se hizo para que luego le facuten', 1);

SET IDENTITY_INSERT [dbo].[Roles] OFF;
GO

-- ============================================================================
-- 2. Permisos (catálogo completo; se inserta por Nombre, sin Id fijo)
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
-- 3. RolPermisos — matriz final Rol -> Permiso (idempotente y autorreparable)
--    Se define el conjunto permitido por rol y luego:
--      a) se insertan los permisos faltantes;
--      b) se eliminan los permisos sobrantes (roles 1/2/3), reparando bases
--         que quedaron con concesiones heredadas de scripts históricos.
--    Administrador = todos los permisos EXCEPTO 'MisTrabajos.Ver'
--    (pantalla exclusiva del Mecánico).
-- ============================================================================
DECLARE @Permitidos TABLE ([IdRol] int NOT NULL, [IdPermiso] int NOT NULL);

-- Administrador (1): todos menos MisTrabajos.Ver (se deriva del catálogo).
INSERT INTO @Permitidos ([IdRol], [IdPermiso])
SELECT 1, p.[IdPermiso]
FROM [dbo].[Permisos] p
WHERE p.[Nombre] <> N'MisTrabajos.Ver';

-- Recepcionista (2).
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

-- Mecánico (3).
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

-- a) Insertar concesiones faltantes.
INSERT INTO [dbo].[RolPermisos] ([IdRol], [IdPermiso])
SELECT p.[IdRol], p.[IdPermiso]
FROM @Permitidos p
WHERE NOT EXISTS (
    SELECT 1 FROM [dbo].[RolPermisos] rp
    WHERE rp.[IdRol] = p.[IdRol] AND rp.[IdPermiso] = p.[IdPermiso]
);

-- b) Eliminar concesiones sobrantes (solo roles administrados 1/2/3).
DELETE rp
FROM [dbo].[RolPermisos] rp
WHERE rp.[IdRol] IN (1, 2, 3)
  AND NOT EXISTS (
    SELECT 1 FROM @Permitidos p
    WHERE p.[IdRol] = rp.[IdRol] AND p.[IdPermiso] = rp.[IdPermiso]
  );
GO

-- ============================================================================
-- 4. Taller (por clave natural Nombre)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Taller] WHERE [Nombre] = N'Taller PRO')
    INSERT INTO [dbo].[Taller] ([Nombre], [Direccion], [Telefono], [Activo])
    VALUES (N'Taller PRO', N'Av. Siempre Viva 742', N'3794000000', 1);
GO

-- ============================================================================
-- 5. Cliente demo (Id fijo 1)
-- ============================================================================
SET IDENTITY_INSERT [dbo].[Cliente] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Cliente] WHERE [Id] = 1 OR [Dni] = N'20000001')
    INSERT INTO [dbo].[Cliente] ([Id], [Nombre], [Apellido], [Dni], [FechaNacimiento], [Direccion], [Email], [Telefono], [Activo])
    VALUES (1, N'Cliente', N'Demo', N'20000001', CAST(N'1990-01-01T00:00:00' AS datetime2), N'Av. Siempre Viva 742', N'cliente@example.com', N'3794000004', 1);

SET IDENTITY_INSERT [dbo].[Cliente] OFF;
GO

-- ============================================================================
-- 6. Marca — se preservan Toyota = Id 1 y Renault = Id 2
-- ============================================================================
SET IDENTITY_INSERT [dbo].[Marca] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Marca] WHERE [NombreMarca] = N'Toyota')
    INSERT INTO [dbo].[Marca] ([Id], [NombreMarca])
    VALUES (1, N'Toyota');

IF NOT EXISTS (SELECT 1 FROM [dbo].[Marca] WHERE [NombreMarca] = N'Renault')
    INSERT INTO [dbo].[Marca] ([Id], [NombreMarca])
    VALUES (2, N'Renault');

SET IDENTITY_INSERT [dbo].[Marca] OFF;
GO

-- ============================================================================
-- 7. Modelo — se preservan los modelos demo Etios = Id 1 y Kwid = Id 2
--    (la marca se resuelve por nombre)
-- ============================================================================
SET IDENTITY_INSERT [dbo].[Modelo] ON;

IF NOT EXISTS (
    SELECT 1 FROM [dbo].[Modelo]
    WHERE [NombreModelo] = N'Etios'
      AND [MarcaId] = (SELECT [Id] FROM [dbo].[Marca] WHERE [NombreMarca] = N'Toyota')
)
    INSERT INTO [dbo].[Modelo] ([Id], [MarcaId], [NombreModelo])
    VALUES (1, (SELECT [Id] FROM [dbo].[Marca] WHERE [NombreMarca] = N'Toyota'), N'Etios');

IF NOT EXISTS (
    SELECT 1 FROM [dbo].[Modelo]
    WHERE [NombreModelo] = N'Kwid'
      AND [MarcaId] = (SELECT [Id] FROM [dbo].[Marca] WHERE [NombreMarca] = N'Renault')
)
    INSERT INTO [dbo].[Modelo] ([Id], [MarcaId], [NombreModelo])
    VALUES (2, (SELECT [Id] FROM [dbo].[Marca] WHERE [NombreMarca] = N'Renault'), N'Kwid');

SET IDENTITY_INSERT [dbo].[Modelo] OFF;
GO

-- ============================================================================
-- 8. Catálogo ampliado de Marca/Modelo (mercado argentino).
--    Se cargan en una tabla temporal y se combinan con MERGE por nombre, de
--    modo que re-ejecutar el script no duplica ni reasigna Ids. Toyota y
--    Renault se incluyen también: al existir por nombre, el MERGE no los toca.
-- ============================================================================
CREATE TABLE #Marcas ([Marca] nvarchar(100) NOT NULL, [Modelo] nvarchar(100) NOT NULL);

INSERT INTO #Marcas ([Marca], [Modelo]) VALUES
    -- Toyota
    (N'Toyota', N'Etios'), (N'Toyota', N'Corolla'), (N'Toyota', N'Hilux'),
    (N'Toyota', N'SW4'), (N'Toyota', N'Yaris'), (N'Toyota', N'RAV4'),
    -- Renault
    (N'Renault', N'Kwid'), (N'Renault', N'Sandero'), (N'Renault', N'Logan'),
    (N'Renault', N'Duster'), (N'Renault', N'Kangoo'), (N'Renault', N'Clio'),
    -- Volkswagen
    (N'Volkswagen', N'Gol'), (N'Volkswagen', N'Polo'), (N'Volkswagen', N'Amarok'),
    (N'Volkswagen', N'Vento'), (N'Volkswagen', N'T-Cross'), (N'Volkswagen', N'Saveiro'),
    -- Chevrolet
    (N'Chevrolet', N'Onix'), (N'Chevrolet', N'Prisma'), (N'Chevrolet', N'Cruze'),
    (N'Chevrolet', N'S10'), (N'Chevrolet', N'Tracker'), (N'Chevrolet', N'Spin'),
    -- Ford
    (N'Ford', N'Fiesta'), (N'Ford', N'Focus'), (N'Ford', N'Ranger'),
    (N'Ford', N'EcoSport'), (N'Ford', N'Ka'), (N'Ford', N'Territory'),
    -- Fiat
    (N'Fiat', N'Cronos'), (N'Fiat', N'Argo'), (N'Fiat', N'Toro'),
    (N'Fiat', N'Mobi'), (N'Fiat', N'Strada'), (N'Fiat', N'Pulse'),
    -- Peugeot
    (N'Peugeot', N'208'), (N'Peugeot', N'2008'), (N'Peugeot', N'308'),
    (N'Peugeot', N'3008'), (N'Peugeot', N'Partner'), (N'Peugeot', N'Boxer'),
    -- Citroën
    (N'Citroën', N'C3'), (N'Citroën', N'C4 Cactus'), (N'Citroën', N'Berlingo'),
    (N'Citroën', N'C-Elysée'), (N'Citroën', N'Jumpy'), (N'Citroën', N'C5 Aircross'),
    -- Honda
    (N'Honda', N'Fit'), (N'Honda', N'City'), (N'Honda', N'Civic'),
    (N'Honda', N'HR-V'), (N'Honda', N'CR-V'), (N'Honda', N'WR-V'),
    -- Nissan
    (N'Nissan', N'Versa'), (N'Nissan', N'Kicks'), (N'Nissan', N'Frontier'),
    (N'Nissan', N'March'), (N'Nissan', N'X-Trail'), (N'Nissan', N'Note'),
    -- Hyundai
    (N'Hyundai', N'HB20'), (N'Hyundai', N'Creta'), (N'Hyundai', N'Tucson'),
    (N'Hyundai', N'Elantra'), (N'Hyundai', N'i10'), (N'Hyundai', N'Santa Fe'),
    -- Kia
    (N'Kia', N'Rio'), (N'Kia', N'Cerato'), (N'Kia', N'Sportage'),
    (N'Kia', N'Seltos'), (N'Kia', N'Soul'), (N'Kia', N'Picanto'),
    -- Jeep
    (N'Jeep', N'Renegade'), (N'Jeep', N'Compass'), (N'Jeep', N'Cherokee'),
    (N'Jeep', N'Wrangler'), (N'Jeep', N'Grand Cherokee'),
    -- RAM
    (N'RAM', N'700'), (N'RAM', N'1500'), (N'RAM', N'2500'),
    -- Dodge
    (N'Dodge', N'Journey'), (N'Dodge', N'Durango'),
    -- Mercedes-Benz
    (N'Mercedes-Benz', N'Clase A'), (N'Mercedes-Benz', N'Clase C'), (N'Mercedes-Benz', N'Clase E'),
    (N'Mercedes-Benz', N'Sprinter'), (N'Mercedes-Benz', N'GLC'), (N'Mercedes-Benz', N'GLE'),
    -- BMW
    (N'BMW', N'Serie 1'), (N'BMW', N'Serie 3'), (N'BMW', N'Serie 5'),
    (N'BMW', N'X1'), (N'BMW', N'X3'), (N'BMW', N'X5'),
    -- Audi
    (N'Audi', N'A1'), (N'Audi', N'A3'), (N'Audi', N'A4'),
    (N'Audi', N'Q3'), (N'Audi', N'Q5'), (N'Audi', N'Q7'),
    -- Volvo
    (N'Volvo', N'S60'), (N'Volvo', N'XC40'), (N'Volvo', N'XC60'), (N'Volvo', N'XC90'),
    -- Mitsubishi
    (N'Mitsubishi', N'L200'), (N'Mitsubishi', N'Outlander'), (N'Mitsubishi', N'ASX'), (N'Mitsubishi', N'Montero'),
    -- Suzuki
    (N'Suzuki', N'Swift'), (N'Suzuki', N'Vitara'), (N'Suzuki', N'Jimny'),
    (N'Suzuki', N'S-Cross'), (N'Suzuki', N'Baleno'),
    -- Chery
    (N'Chery', N'Tiggo 2'), (N'Chery', N'Tiggo 4'), (N'Chery', N'Tiggo 5'),
    (N'Chery', N'Tiggo 7'), (N'Chery', N'Tiggo 8'), (N'Chery', N'Arrizo 5'),
    -- Lifan
    (N'Lifan', N'X60'), (N'Lifan', N'X50'), (N'Lifan', N'320'), (N'Lifan', N'Foison'),
    -- Geely
    (N'Geely', N'Emgrand'), (N'Geely', N'Coolray'), (N'Geely', N'Okavango'), (N'Geely', N'GS'),
    -- DFSK
    (N'DFSK', N'C31'), (N'DFSK', N'K01'), (N'DFSK', N'Glory 580'),
    -- Haval
    (N'Haval', N'H6'), (N'Haval', N'Jolion'), (N'Haval', N'H9'),
    -- BAIC
    (N'BAIC', N'X35'), (N'BAIC', N'X55'), (N'BAIC', N'BJ40'), (N'BAIC', N'D20'),
    -- JAC
    (N'JAC', N'S2'), (N'JAC', N'S3'), (N'JAC', N'T6'), (N'JAC', N'T8'), (N'JAC', N'J4'),
    -- Changan
    (N'Changan', N'CS15'), (N'Changan', N'CS35'), (N'Changan', N'CS75'), (N'Changan', N'Alsvin'),
    -- MG
    (N'MG', N'ZS'), (N'MG', N'HS'), (N'MG', N'RX5'), (N'MG', N'MG5'),
    -- Land Rover
    (N'Land Rover', N'Defender'), (N'Land Rover', N'Discovery'), (N'Land Rover', N'Range Rover Evoque'), (N'Land Rover', N'Range Rover Sport'),
    -- Jaguar
    (N'Jaguar', N'XE'), (N'Jaguar', N'XF'), (N'Jaguar', N'F-Pace'), (N'Jaguar', N'E-Pace'),
    -- Subaru
    (N'Subaru', N'Impreza'), (N'Subaru', N'Forester'), (N'Subaru', N'XV'), (N'Subaru', N'Outback'),
    -- Mazda
    (N'Mazda', N'Mazda 2'), (N'Mazda', N'Mazda 3'), (N'Mazda', N'CX-3'), (N'Mazda', N'CX-5'),
    -- Alfa Romeo
    (N'Alfa Romeo', N'Giulietta'), (N'Alfa Romeo', N'Giulia'), (N'Alfa Romeo', N'Stelvio'),
    -- Porsche
    (N'Porsche', N'Macan'), (N'Porsche', N'Cayenne'), (N'Porsche', N'911'), (N'Porsche', N'Panamera'),
    -- Mini
    (N'Mini', N'Cooper'), (N'Mini', N'Countryman'), (N'Mini', N'Clubman'),
    -- Lexus
    (N'Lexus', N'CT'), (N'Lexus', N'IS'), (N'Lexus', N'NX'), (N'Lexus', N'RX'),
    -- Isuzu
    (N'Isuzu', N'D-Max'), (N'Isuzu', N'MU-X'),
    -- Iveco
    (N'Iveco', N'Daily'), (N'Iveco', N'Tector'), (N'Iveco', N'Stralis'),
    -- Scania
    (N'Scania', N'P-Serie'), (N'Scania', N'R-Serie'), (N'Scania', N'G-Serie'),
    -- Seat
    (N'Seat', N'Ibiza'), (N'Seat', N'León'), (N'Seat', N'Arona'), (N'Seat', N'Ateca'),
    -- Skoda
    (N'Skoda', N'Fabia'), (N'Skoda', N'Octavia'), (N'Skoda', N'Karoq'), (N'Skoda', N'Kodiaq'),
    -- DS
    (N'DS', N'DS3'), (N'DS', N'DS4'), (N'DS', N'DS7'),
    -- Great Wall
    (N'Great Wall', N'Wingle'), (N'Great Wall', N'Poer'), (N'Great Wall', N'Haval H5');

-- 8.1 Marcas nuevas (MERGE por nombre).
MERGE INTO [dbo].[Marca] AS destino
USING (SELECT DISTINCT [Marca] FROM #Marcas) AS origen ([Marca])
    ON destino.[NombreMarca] = origen.[Marca]
WHEN NOT MATCHED THEN
    INSERT ([NombreMarca]) VALUES (origen.[Marca]);

-- 8.2 Modelos nuevos (MERGE por (MarcaId, NombreModelo); marca por nombre).
MERGE INTO [dbo].[Modelo] AS destino
USING (
    SELECT DISTINCT ma.[Id] AS [MarcaId], m.[Modelo] AS [NombreModelo]
    FROM #Marcas m
    INNER JOIN [dbo].[Marca] ma ON ma.[NombreMarca] = m.[Marca]
) AS origen
    ON destino.[MarcaId] = origen.[MarcaId] AND destino.[NombreModelo] = origen.[NombreModelo]
WHEN NOT MATCHED THEN
    INSERT ([MarcaId], [NombreModelo]) VALUES (origen.[MarcaId], origen.[NombreModelo]);

DROP TABLE #Marcas;
GO

-- ============================================================================
-- 9. Vehículos demo (el Modelo se resuelve por Marca + Modelo, nunca por Id)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Vehiculo] WHERE [Patente] = N'AB 125 DC')
    INSERT INTO [dbo].[Vehiculo] ([ModeloId], [Anio], [Patente], [KmActual])
    SELECT mo.[Id], 2020, N'AB 125 DC', 60000
    FROM [dbo].[Modelo] mo
    INNER JOIN [dbo].[Marca] ma ON ma.[Id] = mo.[MarcaId]
    WHERE ma.[NombreMarca] = N'Toyota' AND mo.[NombreModelo] = N'Etios';

IF NOT EXISTS (SELECT 1 FROM [dbo].[Vehiculo] WHERE [Patente] = N'AF 564 FG')
    INSERT INTO [dbo].[Vehiculo] ([ModeloId], [Anio], [Patente], [KmActual])
    SELECT mo.[Id], 2025, N'AF 564 FG', 30000
    FROM [dbo].[Modelo] mo
    INNER JOIN [dbo].[Marca] ma ON ma.[Id] = mo.[MarcaId]
    WHERE ma.[NombreMarca] = N'Renault' AND mo.[NombreModelo] = N'Kwid';
GO

-- ============================================================================
-- 10. PropietarioVehiculo — titularidad demo (Cliente demo sobre ambos)
-- ============================================================================
IF NOT EXISTS (
    SELECT 1
    FROM [dbo].[PropietarioVehiculo] pv
    INNER JOIN [dbo].[Vehiculo] v ON v.[Id] = pv.[VehiculoId]
    WHERE v.[Patente] = N'AB 125 DC' AND pv.[EsActual] = 1
)
    INSERT INTO [dbo].[PropietarioVehiculo] ([ClienteId], [VehiculoId], [FechaAdquisicion], [EsActual])
    SELECT c.[Id], v.[Id], GETDATE(), 1
    FROM [dbo].[Cliente] c
    CROSS JOIN [dbo].[Vehiculo] v
    WHERE c.[Dni] = N'20000001' AND v.[Patente] = N'AB 125 DC';

IF NOT EXISTS (
    SELECT 1
    FROM [dbo].[PropietarioVehiculo] pv
    INNER JOIN [dbo].[Vehiculo] v ON v.[Id] = pv.[VehiculoId]
    WHERE v.[Patente] = N'AF 564 FG' AND pv.[EsActual] = 1
)
    INSERT INTO [dbo].[PropietarioVehiculo] ([ClienteId], [VehiculoId], [FechaAdquisicion], [EsActual])
    SELECT c.[Id], v.[Id], GETDATE(), 1
    FROM [dbo].[Cliente] c
    CROSS JOIN [dbo].[Vehiculo] v
    WHERE c.[Dni] = N'20000001' AND v.[Patente] = N'AF 564 FG';
GO

-- ============================================================================
-- 11. Usuarios semilla (Administrador "bootstrap", Mecánico y Recepción demo).
--     El rol se resuelve por Id de rol (1/2/3) y se evita duplicar por Email.
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Usuarios] WHERE [Email] = N'admin@example.com')
    INSERT INTO [dbo].[Usuarios] ([RolId], [Nombre], [Apellido], [Dni], [Email], [Telefono], [Domicilio], [FechaNacimiento], [Contrasena], [Estado])
    SELECT r.[Id], N'Admin', N'Sistema', N'10000001', N'admin@example.com', N'3794000001', N'Av. Siempre Viva 742', CAST(N'2000-01-26T00:00:00' AS datetime2), N'CHANGE_ME', 1
    FROM [dbo].[Roles] r
    WHERE r.[Id] = 1;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Usuarios] WHERE [Email] = N'mecanico@example.com')
    INSERT INTO [dbo].[Usuarios] ([RolId], [Nombre], [Apellido], [Dni], [Email], [Telefono], [Domicilio], [FechaNacimiento], [Contrasena], [Estado])
    SELECT r.[Id], N'Mecanico', N'Demo', N'10000002', N'mecanico@example.com', N'3794000002', N'Av. Siempre Viva 742', CAST(N'2008-09-16T00:00:00' AS datetime2), N'CHANGE_ME', 1
    FROM [dbo].[Roles] r
    WHERE r.[Id] = 3;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Usuarios] WHERE [Email] = N'recepcion@example.com')
    INSERT INTO [dbo].[Usuarios] ([RolId], [Nombre], [Apellido], [Dni], [Email], [Telefono], [Domicilio], [FechaNacimiento], [Contrasena], [Estado])
    SELECT r.[Id], N'Recepcion', N'Demo', N'10000003', N'recepcion@example.com', N'3794000003', N'Av. Siempre Viva 742', CAST(N'2000-02-16T00:00:00' AS datetime2), N'CHANGE_ME', 1
    FROM [dbo].[Roles] r
    WHERE r.[Id] = 2;
GO

-- ============================================================================
-- 12. Servicio (catálogo demo, por clave natural Nombre)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Filtro Aire')
    INSERT INTO [dbo].[Servicio] ([Nombre], [Precio], [Activo])
    VALUES (N'Cambio de Filtro Aire', 25000.00, 1);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Servicio] WHERE [Nombre] = N'Cambio de Correa de Distribucion')
    INSERT INTO [dbo].[Servicio] ([Nombre], [Precio], [Activo])
    VALUES (N'Cambio de Correa de Distribucion', 15000.00, 1);
GO

-- ============================================================================
-- 13. RegistroServicio demo (orden de servicio sobre el vehículo AB 125 DC).
--     Estado canónico 'Completada' (ver CHK_RegistroServicio_Estado).
--     Los FKs se resuelven por clave natural (patente / nombre / email).
--     Nota: con 03_triggers.sql aplicado, el trigger de sincronización de KM
--     actualiza Vehiculo.KmActual a 125000 al insertar esta fila (comportamiento
--     de dominio esperado). La guarda NOT EXISTS evita que se re-dispare.
-- ============================================================================
IF NOT EXISTS (
    SELECT 1
    FROM [dbo].[RegistroServicio] rs
    INNER JOIN [dbo].[Vehiculo] v ON v.[Id] = rs.[VehiculoId]
    WHERE v.[Patente] = N'AB 125 DC'
      AND rs.[Fecha] = CAST(N'2026-09-27T19:31:53.396' AS datetime2)
)
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    SELECT v.[Id], t.[Id], u.[Id], CAST(N'2026-09-27T19:31:53.396' AS datetime2), 125000, N'Completada'
    FROM [dbo].[Vehiculo] v
    CROSS JOIN [dbo].[Taller] t
    CROSS JOIN [dbo].[Usuarios] u
    WHERE v.[Patente] = N'AB 125 DC'
      AND t.[Nombre] = N'Taller PRO'
      AND u.[Email] = N'recepcion@example.com';
GO

-- ============================================================================
-- 14. DetalleServicio demo (tareas de la orden anterior).
--     Estado canónico 'Finalizada' (ver CHK_DetalleServicio_Estado).
-- ============================================================================
IF NOT EXISTS (
    SELECT 1
    FROM [dbo].[DetalleServicio] ds
    INNER JOIN [dbo].[RegistroServicio] rs ON rs.[Id] = ds.[RegistroServicioId]
    INNER JOIN [dbo].[Vehiculo] v ON v.[Id] = rs.[VehiculoId]
    INNER JOIN [dbo].[Servicio] s ON s.[Id] = ds.[ServicioId]
    WHERE v.[Patente] = N'AB 125 DC'
      AND rs.[Fecha] = CAST(N'2026-09-27T19:31:53.396' AS datetime2)
      AND s.[Nombre] = N'Cambio de Filtro Aire'
)
    INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
    SELECT rs.[Id], u.[Id], s.[Id], 1, 25000.00, 1, N'Manual', N'Finalizada', N''
    FROM [dbo].[RegistroServicio] rs
    INNER JOIN [dbo].[Vehiculo] v ON v.[Id] = rs.[VehiculoId]
    CROSS JOIN [dbo].[Servicio] s
    CROSS JOIN [dbo].[Usuarios] u
    WHERE v.[Patente] = N'AB 125 DC'
      AND rs.[Fecha] = CAST(N'2026-09-27T19:31:53.396' AS datetime2)
      AND s.[Nombre] = N'Cambio de Filtro Aire'
      AND u.[Email] = N'mecanico@example.com';

IF NOT EXISTS (
    SELECT 1
    FROM [dbo].[DetalleServicio] ds
    INNER JOIN [dbo].[RegistroServicio] rs ON rs.[Id] = ds.[RegistroServicioId]
    INNER JOIN [dbo].[Vehiculo] v ON v.[Id] = rs.[VehiculoId]
    INNER JOIN [dbo].[Servicio] s ON s.[Id] = ds.[ServicioId]
    WHERE v.[Patente] = N'AB 125 DC'
      AND rs.[Fecha] = CAST(N'2026-09-27T19:31:53.396' AS datetime2)
      AND s.[Nombre] = N'Cambio de Correa de Distribucion'
)
    INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
    SELECT rs.[Id], u.[Id], s.[Id], 1, 15000.00, 1, N'Manual', N'Finalizada', N''
    FROM [dbo].[RegistroServicio] rs
    INNER JOIN [dbo].[Vehiculo] v ON v.[Id] = rs.[VehiculoId]
    CROSS JOIN [dbo].[Servicio] s
    CROSS JOIN [dbo].[Usuarios] u
    WHERE v.[Patente] = N'AB 125 DC'
      AND rs.[Fecha] = CAST(N'2026-09-27T19:31:53.396' AS datetime2)
      AND s.[Nombre] = N'Cambio de Correa de Distribucion'
      AND u.[Email] = N'mecanico@example.com';
GO

-- ============================================================================
-- 15. ConfiguracionEmpresa (fila singleton Id = 1)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[ConfiguracionEmpresa] WHERE [Id] = 1)
    INSERT INTO [dbo].[ConfiguracionEmpresa] ([Id], [Nombre], [Direccion], [Telefono], [Email], [Logo], [FechaActualizacion])
    VALUES (1, N'TALLER PRO', N'Ruta Nacional 11 Km 1005 - Resistencia, Chaco', N'+54 362 4123456', N'contacto@taller.com', NULL, SYSDATETIME());
GO

-- ============================================================================
-- 16. ConfiguracionBackup (fila singleton Id = 1; rutas vacías por defecto)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[ConfiguracionBackup] WHERE [Id] = 1)
    INSERT INTO [dbo].[ConfiguracionBackup] ([Id], [NombreBaseDatos], [RutaBackupServer], [RutaBackupUNC], [DriveFolderId], [UltimaSubidaArchivo], [FechaUltimaSubida], [FechaActualizacion])
    VALUES (1, N'ControlVehicular', N'', N'', NULL, NULL, NULL, SYSDATETIME());
GO

-- ============================================================================
-- 17. MetodoPago (por clave natural Nombre)
-- ============================================================================
INSERT INTO [dbo].[MetodoPago] ([Nombre])
SELECT v.[Nombre]
FROM (VALUES (N'Efectivo'), (N'Tarjeta')) AS v([Nombre])
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[MetodoPago] m WHERE m.[Nombre] = v.[Nombre]);
GO
