-- =========================================================================================
-- MIGRACIÓN: Separar posesión de Vehículo a tabla PropietarioVehiculo (Sin pérdida de datos)
-- =========================================================================================

-- 1. Eliminar la Foreign Key vieja que causa el ciclo de borrado en cascada
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Vehiculo_Cliente_ClienteId')
BEGIN
    ALTER TABLE [Vehiculo] DROP CONSTRAINT [FK_Vehiculo_Cliente_ClienteId];
END
GO

-- 2. Crear la nueva tabla intermedia
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PropietarioVehiculo')
BEGIN
    CREATE TABLE [PropietarioVehiculo] (
        [Id] int NOT NULL IDENTITY,
        [ClienteId] int NOT NULL,
        [VehiculoId] int NOT NULL,
        [FechaAdquisicion] datetime2 NOT NULL DEFAULT GETDATE(),
        [FechaVenta] datetime2 NULL,
        [EsActual] bit NOT NULL DEFAULT 1,
        CONSTRAINT [PK_PropietarioVehiculo] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PropietarioVehiculo_Cliente] FOREIGN KEY ([ClienteId]) REFERENCES [Cliente] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_PropietarioVehiculo_Vehiculo] FOREIGN KEY ([VehiculoId]) REFERENCES [Vehiculo] ([Id]) ON DELETE CASCADE
    );
END
GO

-- 3. Migrar los datos existentes: copiar dueños actuales de Vehiculo a la nueva tabla
INSERT INTO [PropietarioVehiculo] ([ClienteId], [VehiculoId], [FechaAdquisicion], [EsActual])
SELECT [ClienteId], [Id], GETDATE(), 1
FROM [Vehiculo]
WHERE [ClienteId] IS NOT NULL;
GO

-- 4. Opcional: Eliminar los índices creados por EF Core sobre esa columna
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Vehiculo_ClienteId' AND object_id = OBJECT_ID('Vehiculo'))
BEGIN
    DROP INDEX [IX_Vehiculo_ClienteId] ON [Vehiculo];
END
GO

-- 5. Finalmente, eliminar la columna ClienteId de Vehiculo
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'ClienteId' AND Object_ID = Object_ID(N'Vehiculo'))
BEGIN
    ALTER TABLE [Vehiculo] DROP COLUMN [ClienteId];
END
GO

PRINT 'Migración completada exitosamente sin pérdida de datos.';
