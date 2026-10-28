USE [ControlVehicular];
GO
SET NOCOUNT ON;
BEGIN TRANSACTION;
BEGIN TRY
    -- 1. Asegurar columna Activo en Marca, Modelo, Vehiculo si se desea paridad con 06
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Marca]') AND name = 'Activo')
        ALTER TABLE [dbo].[Marca] ADD [Activo] bit NOT NULL CONSTRAINT [DF_Marca_Activo] DEFAULT (1);
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Modelo]') AND name = 'Activo')
        ALTER TABLE [dbo].[Modelo] ADD [Activo] bit NOT NULL CONSTRAINT [DF_Modelo_Activo] DEFAULT (1);
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Vehiculo]') AND name = 'Activo')
        ALTER TABLE [dbo].[Vehiculo] ADD [Activo] bit NOT NULL CONSTRAINT [DF_Vehiculo_Activo] DEFAULT (1);
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[MetodoPago]') AND name = 'Activo')
        ALTER TABLE [dbo].[MetodoPago] ADD [Activo] bit NOT NULL CONSTRAINT [DF_MetodoPago_Activo] DEFAULT (1);
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Factura]') AND name = 'Activo')
        ALTER TABLE [dbo].[Factura] ADD [Activo] bit NOT NULL CONSTRAINT [DF_Factura_Activo] DEFAULT (1);
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Pago]') AND name = 'Activo')
        ALTER TABLE [dbo].[Pago] ADD [Activo] bit NOT NULL CONSTRAINT [DF_Pago_Activo] DEFAULT (1);
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[RegistroServicio]') AND name = 'Activo')
        ALTER TABLE [dbo].[RegistroServicio] ADD [Activo] bit NOT NULL CONSTRAINT [DF_RegistroServicio_Activo] DEFAULT (1);
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[DetalleServicio]') AND name = 'Activo')
        ALTER TABLE [dbo].[DetalleServicio] ADD [Activo] bit NOT NULL CONSTRAINT [DF_DetalleServicio_Activo] DEFAULT (1);
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PropietarioVehiculo]') AND name = 'Activo')
        ALTER TABLE [dbo].[PropietarioVehiculo] ADD [Activo] bit NOT NULL CONSTRAINT [DF_PropietarioVehiculo_Activo] DEFAULT (1);
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ConfiguracionBackup]') AND name = 'Activo')
        ALTER TABLE [dbo].[ConfiguracionBackup] ADD [Activo] bit NOT NULL CONSTRAINT [DF_ConfiguracionBackup_Activo] DEFAULT (1);

    -- 2. Asegurar todas las Marcas de 06
    IF OBJECT_ID('tempdb..#TodasMarcas') IS NOT NULL DROP TABLE #TodasMarcas;
    CREATE TABLE #TodasMarcas ([NombreMarca] nvarchar(100) NOT NULL);
    INSERT INTO #TodasMarcas VALUES (N'Alfa Romeo');
    INSERT INTO #TodasMarcas VALUES (N'Audi');
    INSERT INTO #TodasMarcas VALUES (N'BAIC');
    INSERT INTO #TodasMarcas VALUES (N'BMW');
    INSERT INTO #TodasMarcas VALUES (N'Changan');
    INSERT INTO #TodasMarcas VALUES (N'Chery');
    INSERT INTO #TodasMarcas VALUES (N'Chevrolet');
    INSERT INTO #TodasMarcas VALUES (N'Citroën');
    INSERT INTO #TodasMarcas VALUES (N'DFSK');
    INSERT INTO #TodasMarcas VALUES (N'DS');
    INSERT INTO #TodasMarcas VALUES (N'Dodge');
    INSERT INTO #TodasMarcas VALUES (N'Fiat');
    INSERT INTO #TodasMarcas VALUES (N'Ford');
    INSERT INTO #TodasMarcas VALUES (N'Geely');
    INSERT INTO #TodasMarcas VALUES (N'Great Wall');
    INSERT INTO #TodasMarcas VALUES (N'Haval');
    INSERT INTO #TodasMarcas VALUES (N'Honda');
    INSERT INTO #TodasMarcas VALUES (N'Hyundai');
    INSERT INTO #TodasMarcas VALUES (N'Isuzu');
    INSERT INTO #TodasMarcas VALUES (N'Iveco');
    INSERT INTO #TodasMarcas VALUES (N'JAC');
    INSERT INTO #TodasMarcas VALUES (N'Jaguar');
    INSERT INTO #TodasMarcas VALUES (N'Jeep');
    INSERT INTO #TodasMarcas VALUES (N'Kia');
    INSERT INTO #TodasMarcas VALUES (N'Land Rover');
    INSERT INTO #TodasMarcas VALUES (N'Lexus');
    INSERT INTO #TodasMarcas VALUES (N'Lifan');
    INSERT INTO #TodasMarcas VALUES (N'MG');
    INSERT INTO #TodasMarcas VALUES (N'Mazda');
    INSERT INTO #TodasMarcas VALUES (N'Mercedes-Benz');
    INSERT INTO #TodasMarcas VALUES (N'Mini');
    INSERT INTO #TodasMarcas VALUES (N'Mitsubishi');
    INSERT INTO #TodasMarcas VALUES (N'Nissan');
    INSERT INTO #TodasMarcas VALUES (N'Peugeot');
    INSERT INTO #TodasMarcas VALUES (N'Porsche');
    INSERT INTO #TodasMarcas VALUES (N'RAM');
    INSERT INTO #TodasMarcas VALUES (N'Renault');
    INSERT INTO #TodasMarcas VALUES (N'Scania');
    INSERT INTO #TodasMarcas VALUES (N'Seat');
    INSERT INTO #TodasMarcas VALUES (N'Skoda');
    INSERT INTO #TodasMarcas VALUES (N'Subaru');
    INSERT INTO #TodasMarcas VALUES (N'Suzuki');
    INSERT INTO #TodasMarcas VALUES (N'Toyota');
    INSERT INTO #TodasMarcas VALUES (N'Volkswagen');
    INSERT INTO #TodasMarcas VALUES (N'Volvo');
    MERGE [dbo].[Marca] AS target
    USING #TodasMarcas AS source ON target.[NombreMarca] = source.[NombreMarca]
    WHEN NOT MATCHED THEN INSERT ([NombreMarca]) VALUES (source.[NombreMarca]);
    DROP TABLE #TodasMarcas;

    -- 3. Asegurar todos los 203 Modelos de 06
    IF OBJECT_ID('tempdb..#TodosModelos') IS NOT NULL DROP TABLE #TodosModelos;
    CREATE TABLE #TodosModelos ([Marca] nvarchar(100) NOT NULL, [Modelo] nvarchar(100) NOT NULL);
    INSERT INTO #TodosModelos VALUES (N'Toyota', N'Etios');
    INSERT INTO #TodosModelos VALUES (N'Renault', N'Kwid');
    INSERT INTO #TodosModelos VALUES (N'Renault', N'Stepway');
    INSERT INTO #TodosModelos VALUES (N'Toyota', N'Corolla');
    INSERT INTO #TodosModelos VALUES (N'Toyota', N'Hilux');
    INSERT INTO #TodosModelos VALUES (N'Toyota', N'RAV4');
    INSERT INTO #TodosModelos VALUES (N'Toyota', N'SW4');
    INSERT INTO #TodosModelos VALUES (N'Toyota', N'Yaris');
    INSERT INTO #TodosModelos VALUES (N'Renault', N'Clio');
    INSERT INTO #TodosModelos VALUES (N'Renault', N'Duster');
    INSERT INTO #TodosModelos VALUES (N'Renault', N'Kangoo');
    INSERT INTO #TodosModelos VALUES (N'Renault', N'Logan');
    INSERT INTO #TodosModelos VALUES (N'Renault', N'Sandero');
    INSERT INTO #TodosModelos VALUES (N'Alfa Romeo', N'Giulia');
    INSERT INTO #TodosModelos VALUES (N'Alfa Romeo', N'Giulietta');
    INSERT INTO #TodosModelos VALUES (N'Alfa Romeo', N'Stelvio');
    INSERT INTO #TodosModelos VALUES (N'Audi', N'A1');
    INSERT INTO #TodosModelos VALUES (N'Audi', N'A3');
    INSERT INTO #TodosModelos VALUES (N'Audi', N'A4');
    INSERT INTO #TodosModelos VALUES (N'Audi', N'Q3');
    INSERT INTO #TodosModelos VALUES (N'Audi', N'Q5');
    INSERT INTO #TodosModelos VALUES (N'Audi', N'Q7');
    INSERT INTO #TodosModelos VALUES (N'BAIC', N'BJ40');
    INSERT INTO #TodosModelos VALUES (N'BAIC', N'D20');
    INSERT INTO #TodosModelos VALUES (N'BAIC', N'X35');
    INSERT INTO #TodosModelos VALUES (N'BAIC', N'X55');
    INSERT INTO #TodosModelos VALUES (N'BMW', N'Serie 1');
    INSERT INTO #TodosModelos VALUES (N'BMW', N'Serie 3');
    INSERT INTO #TodosModelos VALUES (N'BMW', N'Serie 5');
    INSERT INTO #TodosModelos VALUES (N'BMW', N'X1');
    INSERT INTO #TodosModelos VALUES (N'BMW', N'X3');
    INSERT INTO #TodosModelos VALUES (N'BMW', N'X5');
    INSERT INTO #TodosModelos VALUES (N'Changan', N'Alsvin');
    INSERT INTO #TodosModelos VALUES (N'Changan', N'CS15');
    INSERT INTO #TodosModelos VALUES (N'Changan', N'CS35');
    INSERT INTO #TodosModelos VALUES (N'Changan', N'CS75');
    INSERT INTO #TodosModelos VALUES (N'Chery', N'Arrizo 5');
    INSERT INTO #TodosModelos VALUES (N'Chery', N'Tiggo 2');
    INSERT INTO #TodosModelos VALUES (N'Chery', N'Tiggo 4');
    INSERT INTO #TodosModelos VALUES (N'Chery', N'Tiggo 5');
    INSERT INTO #TodosModelos VALUES (N'Chery', N'Tiggo 7');
    INSERT INTO #TodosModelos VALUES (N'Chery', N'Tiggo 8');
    INSERT INTO #TodosModelos VALUES (N'Chevrolet', N'Cruze');
    INSERT INTO #TodosModelos VALUES (N'Chevrolet', N'Onix');
    INSERT INTO #TodosModelos VALUES (N'Chevrolet', N'Prisma');
    INSERT INTO #TodosModelos VALUES (N'Chevrolet', N'S10');
    INSERT INTO #TodosModelos VALUES (N'Chevrolet', N'Spin');
    INSERT INTO #TodosModelos VALUES (N'Chevrolet', N'Tracker');
    INSERT INTO #TodosModelos VALUES (N'Citroën', N'Berlingo');
    INSERT INTO #TodosModelos VALUES (N'Citroën', N'C3');
    INSERT INTO #TodosModelos VALUES (N'Citroën', N'C4 Cactus');
    INSERT INTO #TodosModelos VALUES (N'Citroën', N'C5 Aircross');
    INSERT INTO #TodosModelos VALUES (N'Citroën', N'C-Elysée');
    INSERT INTO #TodosModelos VALUES (N'Citroën', N'Jumpy');
    INSERT INTO #TodosModelos VALUES (N'DFSK', N'C31');
    INSERT INTO #TodosModelos VALUES (N'DFSK', N'Glory 580');
    INSERT INTO #TodosModelos VALUES (N'DFSK', N'K01');
    INSERT INTO #TodosModelos VALUES (N'Dodge', N'Durango');
    INSERT INTO #TodosModelos VALUES (N'Dodge', N'Journey');
    INSERT INTO #TodosModelos VALUES (N'DS', N'DS3');
    INSERT INTO #TodosModelos VALUES (N'DS', N'DS4');
    INSERT INTO #TodosModelos VALUES (N'DS', N'DS7');
    INSERT INTO #TodosModelos VALUES (N'Fiat', N'Argo');
    INSERT INTO #TodosModelos VALUES (N'Fiat', N'Cronos');
    INSERT INTO #TodosModelos VALUES (N'Fiat', N'Mobi');
    INSERT INTO #TodosModelos VALUES (N'Fiat', N'Pulse');
    INSERT INTO #TodosModelos VALUES (N'Fiat', N'Strada');
    INSERT INTO #TodosModelos VALUES (N'Fiat', N'Toro');
    INSERT INTO #TodosModelos VALUES (N'Ford', N'EcoSport');
    INSERT INTO #TodosModelos VALUES (N'Ford', N'Fiesta');
    INSERT INTO #TodosModelos VALUES (N'Ford', N'Focus');
    INSERT INTO #TodosModelos VALUES (N'Ford', N'Ka');
    INSERT INTO #TodosModelos VALUES (N'Ford', N'Ranger');
    INSERT INTO #TodosModelos VALUES (N'Ford', N'Territory');
    INSERT INTO #TodosModelos VALUES (N'Geely', N'Coolray');
    INSERT INTO #TodosModelos VALUES (N'Geely', N'Emgrand');
    INSERT INTO #TodosModelos VALUES (N'Geely', N'GS');
    INSERT INTO #TodosModelos VALUES (N'Geely', N'Okavango');
    INSERT INTO #TodosModelos VALUES (N'Great Wall', N'Haval H5');
    INSERT INTO #TodosModelos VALUES (N'Great Wall', N'Poer');
    INSERT INTO #TodosModelos VALUES (N'Great Wall', N'Wingle');
    INSERT INTO #TodosModelos VALUES (N'Haval', N'H6');
    INSERT INTO #TodosModelos VALUES (N'Haval', N'H9');
    INSERT INTO #TodosModelos VALUES (N'Haval', N'Jolion');
    INSERT INTO #TodosModelos VALUES (N'Honda', N'City');
    INSERT INTO #TodosModelos VALUES (N'Honda', N'Civic');
    INSERT INTO #TodosModelos VALUES (N'Honda', N'CR-V');
    INSERT INTO #TodosModelos VALUES (N'Honda', N'Fit');
    INSERT INTO #TodosModelos VALUES (N'Honda', N'HR-V');
    INSERT INTO #TodosModelos VALUES (N'Honda', N'WR-V');
    INSERT INTO #TodosModelos VALUES (N'Hyundai', N'Creta');
    INSERT INTO #TodosModelos VALUES (N'Hyundai', N'Elantra');
    INSERT INTO #TodosModelos VALUES (N'Hyundai', N'HB20');
    INSERT INTO #TodosModelos VALUES (N'Hyundai', N'i10');
    INSERT INTO #TodosModelos VALUES (N'Hyundai', N'Santa Fe');
    INSERT INTO #TodosModelos VALUES (N'Hyundai', N'Tucson');
    INSERT INTO #TodosModelos VALUES (N'Isuzu', N'D-Max');
    INSERT INTO #TodosModelos VALUES (N'Isuzu', N'MU-X');
    INSERT INTO #TodosModelos VALUES (N'Iveco', N'Daily');
    INSERT INTO #TodosModelos VALUES (N'Iveco', N'Stralis');
    INSERT INTO #TodosModelos VALUES (N'Iveco', N'Tector');
    INSERT INTO #TodosModelos VALUES (N'JAC', N'J4');
    INSERT INTO #TodosModelos VALUES (N'JAC', N'S2');
    INSERT INTO #TodosModelos VALUES (N'JAC', N'S3');
    INSERT INTO #TodosModelos VALUES (N'JAC', N'T6');
    INSERT INTO #TodosModelos VALUES (N'JAC', N'T8');
    INSERT INTO #TodosModelos VALUES (N'Jaguar', N'E-Pace');
    INSERT INTO #TodosModelos VALUES (N'Jaguar', N'F-Pace');
    INSERT INTO #TodosModelos VALUES (N'Jaguar', N'XE');
    INSERT INTO #TodosModelos VALUES (N'Jaguar', N'XF');
    INSERT INTO #TodosModelos VALUES (N'Jeep', N'Cherokee');
    INSERT INTO #TodosModelos VALUES (N'Jeep', N'Compass');
    INSERT INTO #TodosModelos VALUES (N'Jeep', N'Grand Cherokee');
    INSERT INTO #TodosModelos VALUES (N'Jeep', N'Renegade');
    INSERT INTO #TodosModelos VALUES (N'Jeep', N'Wrangler');
    INSERT INTO #TodosModelos VALUES (N'Kia', N'Cerato');
    INSERT INTO #TodosModelos VALUES (N'Kia', N'Picanto');
    INSERT INTO #TodosModelos VALUES (N'Kia', N'Rio');
    INSERT INTO #TodosModelos VALUES (N'Kia', N'Seltos');
    INSERT INTO #TodosModelos VALUES (N'Kia', N'Soul');
    INSERT INTO #TodosModelos VALUES (N'Kia', N'Sportage');
    INSERT INTO #TodosModelos VALUES (N'Land Rover', N'Defender');
    INSERT INTO #TodosModelos VALUES (N'Land Rover', N'Discovery');
    INSERT INTO #TodosModelos VALUES (N'Land Rover', N'Range Rover Evoque');
    INSERT INTO #TodosModelos VALUES (N'Land Rover', N'Range Rover Sport');
    INSERT INTO #TodosModelos VALUES (N'Lexus', N'CT');
    INSERT INTO #TodosModelos VALUES (N'Lexus', N'IS');
    INSERT INTO #TodosModelos VALUES (N'Lexus', N'NX');
    INSERT INTO #TodosModelos VALUES (N'Lexus', N'RX');
    INSERT INTO #TodosModelos VALUES (N'Lifan', N'320');
    INSERT INTO #TodosModelos VALUES (N'Lifan', N'Foison');
    INSERT INTO #TodosModelos VALUES (N'Lifan', N'X50');
    INSERT INTO #TodosModelos VALUES (N'Lifan', N'X60');
    INSERT INTO #TodosModelos VALUES (N'Mazda', N'CX-3');
    INSERT INTO #TodosModelos VALUES (N'Mazda', N'CX-5');
    INSERT INTO #TodosModelos VALUES (N'Mazda', N'Mazda 2');
    INSERT INTO #TodosModelos VALUES (N'Mazda', N'Mazda 3');
    INSERT INTO #TodosModelos VALUES (N'Mercedes-Benz', N'Clase A');
    INSERT INTO #TodosModelos VALUES (N'Mercedes-Benz', N'Clase C');
    INSERT INTO #TodosModelos VALUES (N'Mercedes-Benz', N'Clase E');
    INSERT INTO #TodosModelos VALUES (N'Mercedes-Benz', N'GLC');
    INSERT INTO #TodosModelos VALUES (N'Mercedes-Benz', N'GLE');
    INSERT INTO #TodosModelos VALUES (N'Mercedes-Benz', N'Sprinter');
    INSERT INTO #TodosModelos VALUES (N'MG', N'HS');
    INSERT INTO #TodosModelos VALUES (N'MG', N'MG5');
    INSERT INTO #TodosModelos VALUES (N'MG', N'RX5');
    INSERT INTO #TodosModelos VALUES (N'MG', N'ZS');
    INSERT INTO #TodosModelos VALUES (N'Mini', N'Clubman');
    INSERT INTO #TodosModelos VALUES (N'Mini', N'Cooper');
    INSERT INTO #TodosModelos VALUES (N'Mini', N'Countryman');
    INSERT INTO #TodosModelos VALUES (N'Mitsubishi', N'ASX');
    INSERT INTO #TodosModelos VALUES (N'Mitsubishi', N'L200');
    INSERT INTO #TodosModelos VALUES (N'Mitsubishi', N'Montero');
    INSERT INTO #TodosModelos VALUES (N'Mitsubishi', N'Outlander');
    INSERT INTO #TodosModelos VALUES (N'Nissan', N'Frontier');
    INSERT INTO #TodosModelos VALUES (N'Nissan', N'Kicks');
    INSERT INTO #TodosModelos VALUES (N'Nissan', N'March');
    INSERT INTO #TodosModelos VALUES (N'Nissan', N'Note');
    INSERT INTO #TodosModelos VALUES (N'Nissan', N'Versa');
    INSERT INTO #TodosModelos VALUES (N'Nissan', N'X-Trail');
    INSERT INTO #TodosModelos VALUES (N'Peugeot', N'2008');
    INSERT INTO #TodosModelos VALUES (N'Peugeot', N'208');
    INSERT INTO #TodosModelos VALUES (N'Peugeot', N'3008');
    INSERT INTO #TodosModelos VALUES (N'Peugeot', N'308');
    INSERT INTO #TodosModelos VALUES (N'Peugeot', N'Boxer');
    INSERT INTO #TodosModelos VALUES (N'Peugeot', N'Partner');
    INSERT INTO #TodosModelos VALUES (N'Porsche', N'911');
    INSERT INTO #TodosModelos VALUES (N'Porsche', N'Cayenne');
    INSERT INTO #TodosModelos VALUES (N'Porsche', N'Macan');
    INSERT INTO #TodosModelos VALUES (N'Porsche', N'Panamera');
    INSERT INTO #TodosModelos VALUES (N'RAM', N'1500');
    INSERT INTO #TodosModelos VALUES (N'RAM', N'2500');
    INSERT INTO #TodosModelos VALUES (N'RAM', N'700');
    INSERT INTO #TodosModelos VALUES (N'Scania', N'G-Serie');
    INSERT INTO #TodosModelos VALUES (N'Scania', N'P-Serie');
    INSERT INTO #TodosModelos VALUES (N'Scania', N'R-Serie');
    INSERT INTO #TodosModelos VALUES (N'Seat', N'Arona');
    INSERT INTO #TodosModelos VALUES (N'Seat', N'Ateca');
    INSERT INTO #TodosModelos VALUES (N'Seat', N'Ibiza');
    INSERT INTO #TodosModelos VALUES (N'Seat', N'León');
    INSERT INTO #TodosModelos VALUES (N'Skoda', N'Fabia');
    INSERT INTO #TodosModelos VALUES (N'Skoda', N'Karoq');
    INSERT INTO #TodosModelos VALUES (N'Skoda', N'Kodiaq');
    INSERT INTO #TodosModelos VALUES (N'Skoda', N'Octavia');
    INSERT INTO #TodosModelos VALUES (N'Subaru', N'Forester');
    INSERT INTO #TodosModelos VALUES (N'Subaru', N'Impreza');
    INSERT INTO #TodosModelos VALUES (N'Subaru', N'Outback');
    INSERT INTO #TodosModelos VALUES (N'Subaru', N'XV');
    INSERT INTO #TodosModelos VALUES (N'Suzuki', N'Baleno');
    INSERT INTO #TodosModelos VALUES (N'Suzuki', N'Jimny');
    INSERT INTO #TodosModelos VALUES (N'Suzuki', N'S-Cross');
    INSERT INTO #TodosModelos VALUES (N'Suzuki', N'Swift');
    INSERT INTO #TodosModelos VALUES (N'Suzuki', N'Vitara');
    INSERT INTO #TodosModelos VALUES (N'Volkswagen', N'Amarok');
    INSERT INTO #TodosModelos VALUES (N'Volkswagen', N'Gol');
    INSERT INTO #TodosModelos VALUES (N'Volkswagen', N'Polo');
    INSERT INTO #TodosModelos VALUES (N'Volkswagen', N'Saveiro');
    INSERT INTO #TodosModelos VALUES (N'Volkswagen', N'T-Cross');
    INSERT INTO #TodosModelos VALUES (N'Volkswagen', N'Vento');
    INSERT INTO #TodosModelos VALUES (N'Volvo', N'S60');
    INSERT INTO #TodosModelos VALUES (N'Volvo', N'XC40');
    INSERT INTO #TodosModelos VALUES (N'Volvo', N'XC60');
    INSERT INTO #TodosModelos VALUES (N'Volvo', N'XC90');
    MERGE [dbo].[Modelo] AS target
    USING (
        SELECT m.[Id] AS [MarcaId], t.[Modelo] AS [NombreModelo]
        FROM #TodosModelos t
        INNER JOIN [dbo].[Marca] m ON m.[NombreMarca] = t.[Marca]
    ) AS source
    ON target.[MarcaId] = source.[MarcaId] AND target.[NombreModelo] = source.[NombreModelo]
    WHEN NOT MATCHED THEN INSERT ([MarcaId], [NombreModelo]) VALUES (source.[MarcaId], source.[NombreModelo]);
    DROP TABLE #TodosModelos;

    COMMIT TRANSACTION;
    PRINT 'TODOS LOS MODELOS, MARCAS Y COLUMNAS ACTIVO SINCRONIZADOS EXITOSAMENTE.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    PRINT 'ERROR: ' + ERROR_MESSAGE();
    THROW;
END CATCH;
GO