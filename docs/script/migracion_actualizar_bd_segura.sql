-- ============================================================================
-- migracion_actualizar_bd_segura.sql
-- Actualización segura e idempotente de la base de datos ControlVehicular.
-- NO BORRA NINGÚN DATO EXISTENTE (preserva Clientes, Vehículos, Historial, etc.)
-- ============================================================================
USE [ControlVehicular];
GO

SET NOCOUNT ON;
BEGIN TRANSACTION;

BEGIN TRY
    PRINT '>>> 1. Verificando y renombrando Estado a Activo (baja logica)...';
    
    -- Roles: renombrar Estado -> Activo si corresponde
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Roles]') AND name = 'Estado')
       AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Roles]') AND name = 'Activo')
    BEGIN
        EXEC sp_rename '[dbo].[Roles].[Estado]', 'Activo', 'COLUMN';
        PRINT '  * [Roles]: Columna [Estado] renombrada a [Activo].';
    END
    ELSE
    BEGIN
        PRINT '  * [Roles]: Columna [Activo] ya existe o esta al dia.';
    END

    -- Usuarios: renombrar Estado -> Activo si corresponde
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Usuarios]') AND name = 'Estado')
       AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Usuarios]') AND name = 'Activo')
    BEGIN
        EXEC sp_rename '[dbo].[Usuarios].[Estado]', 'Activo', 'COLUMN';
        PRINT '  * [Usuarios]: Columna [Estado] renombrada a [Activo].';
    END
    ELSE
    BEGIN
        PRINT '  * [Usuarios]: Columna [Activo] ya existe o esta al dia.';
    END

    -- Cliente: asegurar Activo
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Cliente]') AND name = 'Activo')
    BEGIN
        ALTER TABLE [dbo].[Cliente] ADD [Activo] bit NOT NULL CONSTRAINT [DF_Cliente_Activo] DEFAULT (1);
        PRINT '  * [Cliente]: Columna [Activo] agregada con default (1).';
    END

    -- Taller: asegurar Activo
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Taller]') AND name = 'Activo')
    BEGIN
        ALTER TABLE [dbo].[Taller] ADD [Activo] bit NOT NULL CONSTRAINT [DF_Taller_Activo] DEFAULT (1);
        PRINT '  * [Taller]: Columna [Activo] agregada con default (1).';
    END

    -- Servicio: asegurar Activo
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Servicio]') AND name = 'Activo')
    BEGIN
        ALTER TABLE [dbo].[Servicio] ADD [Activo] bit NOT NULL CONSTRAINT [DF_Servicio_Activo] DEFAULT (1);
        PRINT '  * [Servicio]: Columna [Activo] agregada con default (1).';
    END

    PRINT '>>> 2. Actualizando catalogo de Servicios (15 servicios)...';
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

    PRINT '>>> 3. Actualizando catalogo de Marcas y Modelos (45 marcas, 203 modelos)...';
    IF OBJECT_ID('tempdb..#MarcasCatalogo') IS NOT NULL DROP TABLE #MarcasCatalogo;
    CREATE TABLE #MarcasCatalogo ([Marca] nvarchar(100) NOT NULL, [Modelo] nvarchar(100) NOT NULL);

    INSERT INTO #MarcasCatalogo ([Marca], [Modelo]) VALUES
        (N'Toyota', N'Etios'), (N'Toyota', N'Corolla'), (N'Toyota', N'Hilux'),
        (N'Toyota', N'SW4'), (N'Toyota', N'Yaris'), (N'Toyota', N'RAV4'),
        (N'Renault', N'Kwid'), (N'Renault', N'Sandero'), (N'Renault', N'Logan'),
        (N'Renault', N'Duster'), (N'Renault', N'Kangoo'), (N'Renault', N'Clio'),
        (N'Volkswagen', N'Gol'), (N'Volkswagen', N'Polo'), (N'Volkswagen', N'Amarok'),
        (N'Volkswagen', N'Vento'), (N'Volkswagen', N'T-Cross'), (N'Volkswagen', N'Saveiro'),
        (N'Chevrolet', N'Onix'), (N'Chevrolet', N'Prisma'), (N'Chevrolet', N'Cruze'),
        (N'Chevrolet', N'S10'), (N'Chevrolet', N'Tracker'), (N'Chevrolet', N'Spin'),
        (N'Ford', N'Fiesta'), (N'Ford', N'Focus'), (N'Ford', N'Ranger'),
        (N'Ford', N'EcoSport'), (N'Ford', N'Ka'), (N'Ford', N'Territory'),
        (N'Fiat', N'Cronos'), (N'Fiat', N'Argo'), (N'Fiat', N'Toro'),
        (N'Fiat', N'Mobi'), (N'Fiat', N'Strada'), (N'Fiat', N'Pulse'),
        (N'Peugeot', N'208'), (N'Peugeot', N'2008'), (N'Peugeot', N'308'),
        (N'Peugeot', N'3008'), (N'Peugeot', N'Partner'), (N'Peugeot', N'Boxer'),
        (N'Citroën', N'C3'), (N'Citroën', N'C4 Cactus'), (N'Citroën', N'Berlingo'),
        (N'Citroën', N'C-Elysée'), (N'Citroën', N'Jumpy'), (N'Citroën', N'C5 Aircross'),
        (N'Honda', N'Fit'), (N'Honda', N'City'), (N'Honda', N'Civic'),
        (N'Honda', N'HR-V'), (N'Honda', N'CR-V'), (N'Honda', N'WR-V'),
        (N'Nissan', N'Versa'), (N'Nissan', N'Kicks'), (N'Nissan', N'Frontier'),
        (N'Nissan', N'March'), (N'Nissan', N'X-Trail'), (N'Nissan', N'Note'),
        (N'Hyundai', N'HB20'), (N'Hyundai', N'Creta'), (N'Hyundai', N'Tucson'),
        (N'Hyundai', N'Elantra'), (N'Hyundai', N'i10'), (N'Hyundai', N'Santa Fe'),
        (N'Kia', N'Rio'), (N'Kia', N'Cerato'), (N'Kia', N'Sportage'),
        (N'Kia', N'Seltos'), (N'Kia', N'Soul'), (N'Kia', N'Picanto'),
        (N'Jeep', N'Renegade'), (N'Jeep', N'Compass'), (N'Jeep', N'Cherokee'),
        (N'Jeep', N'Wrangler'), (N'Jeep', N'Grand Cherokee'),
        (N'RAM', N'700'), (N'RAM', N'1500'), (N'RAM', N'2500'),
        (N'Dodge', N'Journey'), (N'Dodge', N'Durango'),
        (N'Mercedes-Benz', N'Clase A'), (N'Mercedes-Benz', N'Clase C'), (N'Mercedes-Benz', N'Clase E'),
        (N'Mercedes-Benz', N'Sprinter'), (N'Mercedes-Benz', N'GLC'), (N'Mercedes-Benz', N'GLE'),
        (N'BMW', N'Serie 1'), (N'BMW', N'Serie 3'), (N'BMW', N'Serie 5'),
        (N'BMW', N'X1'), (N'BMW', N'X3'), (N'BMW', N'X5'),
        (N'Audi', N'A1'), (N'Audi', N'A3'), (N'Audi', N'A4'),
        (N'Audi', N'Q3'), (N'Audi', N'Q5'), (N'Audi', N'Q7'),
        (N'Volvo', N'S60'), (N'Volvo', N'XC40'), (N'Volvo', N'XC60'), (N'Volvo', N'XC90'),
        (N'Mitsubishi', N'L200'), (N'Mitsubishi', N'Outlander'), (N'Mitsubishi', N'Eclipse Cross'),
        (N'Subaru', N'Forester'), (N'Subaru', N'Outback'), (N'Subaru', N'XV'),
        (N'Suzuki', N'Swift'), (N'Suzuki', N'Vitara'), (N'Suzuki', N'Jimny'),
        (N'Chery', N'Tiggo 2'), (N'Chery', N'Tiggo 4'), (N'Chery', N'QQ'),
        (N'Haval', N'H2'), (N'Haval', N'H6'), (N'Haval', N'Jolion'),
        (N'Geely', N'Emgrand'), (N'Geely', N'Coolray'), (N'Geely', N'Azkarra'),
        (N'BAIC', N'X25'), (N'BAIC', N'X35'), (N'BAIC', N'X55'),
        (N'JAC', N'S2'), (N'JAC', N'S3'), (N'JAC', N'T6'),
        (N'DFSK', N'Glory 560'), (N'DFSK', N'Glory 580'), (N'DFSK', N'C31'),
        (N'Changan', N'CS15'), (N'Changan', N'CS35'), (N'Changan', N'CS75'),
        (N'Great Wall', N'Wingle 5'), (N'Great Wall', N'Wingle 6'), (N'Great Wall', N'Poer'),
        (N'Lifan', N'X50'), (N'Lifan', N'X60'), (N'Lifan', N'Myway'),
        (N'Alfa Romeo', N'Giulia'), (N'Alfa Romeo', N'Stelvio'), (N'Alfa Romeo', N'Giulietta'),
        (N'DS', N'DS 3 Crossback'), (N'DS', N'DS 7 Crossback'), (N'DS', N'DS 4'),
        (N'Mini', N'Cooper'), (N'Mini', N'Countryman'), (N'Mini', N'Clubman'),
        (N'Lexus', N'IS'), (N'Lexus', N'NX'), (N'Lexus', N'RX'),
        (N'Porsche', N'Macan'), (N'Porsche', N'Cayenne'), (N'Porsche', N'911'),
        (N'Land Rover', N'Defender'), (N'Land Rover', N'Discovery'), (N'Land Rover', N'Range Rover Evoque'),
        (N'Jaguar', N'XE'), (N'Jaguar', N'F-Pace'), (N'Jaguar', N'E-Pace'),
        (N'Seat', N'Ibiza'), (N'Seat', N'León'), (N'Seat', N'Ateca'),
        (N'Skoda', N'Fabia'), (N'Skoda', N'Octavia'), (N'Skoda', N'Kodiaq'),
        (N'Mazda', N'Mazda 3'), (N'Mazda', N'CX-30'), (N'Mazda', N'CX-5'),
        (N'MG', N'MG ZS'), (N'MG', N'MG 5'), (N'MG', N'MG HS'),
        (N'Iveco', N'Daily'), (N'Iveco', N'Tector'), (N'Iveco', N'Stralis'),
        (N'Scania', N'Serie R'), (N'Scania', N'Serie S'), (N'Scania', N'Serie G'),
        (N'Isuzu', N'D-Max'), (N'Isuzu', N'NPR'), (N'Isuzu', N'FVR');

    -- Insertar marcas faltantes sin tocar las existentes
    MERGE [dbo].[Marca] AS target
    USING (SELECT DISTINCT [Marca] FROM #MarcasCatalogo) AS source
    ON target.[NombreMarca] = source.[Marca]
    WHEN NOT MATCHED THEN
        INSERT ([NombreMarca]) VALUES (source.[Marca]);

    -- Insertar modelos faltantes vinculados a su marca
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

    PRINT '>>> 4. Verificando Metodos de Pago...';
    IF NOT EXISTS (SELECT 1 FROM [dbo].[MetodoPago] WHERE [Nombre] = N'Efectivo')
        INSERT INTO [dbo].[MetodoPago] ([Nombre]) VALUES (N'Efectivo');
    IF NOT EXISTS (SELECT 1 FROM [dbo].[MetodoPago] WHERE [Nombre] = N'Tarjeta')
        INSERT INTO [dbo].[MetodoPago] ([Nombre]) VALUES (N'Tarjeta');

    PRINT '>>> 5. Verificando Permisos RBAC...';
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

    COMMIT TRANSACTION;
    PRINT '=======================================================';
    PRINT '>> ACTUALIZACION COMPLETADA CON EXITO SIN PERDIDA DE DATOS <<';
    PRINT '=======================================================';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    PRINT 'ERROR DURANTE LA ACTUALIZACION: ' + ERROR_MESSAGE();
    THROW;
END CATCH;
GO
