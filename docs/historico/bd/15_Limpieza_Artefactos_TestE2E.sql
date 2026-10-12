-- =======================================================================
-- LIMPIEZA DE ARTEFACTOS - Test E2E (EndToEndTallerTests)
-- -----------------------------------------------------------------------
-- Elimina los datos generados por las corridas del test end-to-end que
-- quedaron persistidos en la base (corridas previas que fallaron antes de
-- llegar a su limpieza). Los artefactos son:
--
--   * Vehiculos con patente TDD%: 3,4,5,6,7 (corridas antiguas) y
--     13,14,15 (corridas recientes). Todos con Anio 2024 / Km ~10k-15k.
--   * Ordenes de servicio que pertenecen a esos vehiculos:
--     2,3,4,5,6 (Pagada), 14 (Pagada), 17 (Completada),
--     18,19,20 (En Proceso).
--   * Detalles, Facturas y Pagos asociados a esas ordenes.
--   * Clientes 'Cliente TDD EndToEnd' (Ids 11,12,13).
--
-- NO se tocan los datos reales/demo:
--   * Clientes 1,3..10; Vehiculos 1,2,8..12; Ordenes 1,12,15.
--   * Los vehiculos TDD 3..7 estaban vinculados a clientes reales (4..8)
--     solo via PropietarioVehiculo: se elimina el vinculo, NO el cliente.
--
-- Orden de borrado respetando FKs:
--   Pago -> Factura -> DetalleServicio -> RegistroServicio
--        -> PropietarioVehiculo -> Vehiculo -> Cliente
-- (Se eliminan Pago/Factura antes de DetalleServicio porque el trigger
--  TR_DetalleServicio_BloquearModificacionFinalizada rechaza modificar
--  detalles de una orden ya facturada.)
--
-- Notas:
--   - El trigger de facturacion se deshabilita de forma temporal y se
--     re-habilita SIEMPRE (incluso ante error), dejandolo activo al final.
--   - Script idempotente: puede ejecutarse varias veces sin efectos adversos.
-- =======================================================================
USE [ControlVehicular];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

-- Deshabilitar temporalmente la proteccion de facturacion
DISABLE TRIGGER TR_DetalleServicio_BloquearModificacionFinalizada ON DetalleServicio;

BEGIN TRY
    BEGIN TRAN;

    -- Conjuntos objetivo (Ids explicitos, para precision e idempotencia)
    DECLARE @Vehiculos TABLE (Id INT PRIMARY KEY);
    INSERT INTO @Vehiculos (Id) VALUES (3),(4),(5),(6),(7),(13),(14),(15);

    DECLARE @Clientes TABLE (Id INT PRIMARY KEY);
    INSERT INTO @Clientes (Id) VALUES (11),(12),(13);

    -- Ordenes a borrar: todas las que pertenecen a los vehiculos de prueba
    DECLARE @Ordenes TABLE (Id INT PRIMARY KEY);
    INSERT INTO @Ordenes (Id)
    SELECT Id FROM RegistroServicio WHERE VehiculoId IN (SELECT Id FROM @Vehiculos);

    -- 1) Pagos de las facturas de las ordenes de prueba
    DELETE p
    FROM Pago p
    INNER JOIN Factura f ON f.Id = p.FacturaId
    WHERE f.RegistroServicioId IN (SELECT Id FROM @Ordenes);

    -- 2) Facturas de las ordenes de prueba
    DELETE f
    FROM Factura f
    WHERE f.RegistroServicioId IN (SELECT Id FROM @Ordenes);

    -- 3) Detalles de las ordenes de prueba
    DELETE d
    FROM DetalleServicio d
    WHERE d.RegistroServicioId IN (SELECT Id FROM @Ordenes);

    -- 4) Ordenes de prueba
    DELETE rs
    FROM RegistroServicio rs
    WHERE rs.Id IN (SELECT Id FROM @Ordenes);

    -- 5) Vinculos propietario-vehiculo de los vehiculos de prueba
    DELETE pv
    FROM PropietarioVehiculo pv
    WHERE pv.VehiculoId IN (SELECT Id FROM @Vehiculos);

    -- 6) Vehiculos de prueba
    DELETE v
    FROM Vehiculo v
    WHERE v.Id IN (SELECT Id FROM @Vehiculos);

    -- 7) Clientes generados por el test
    DELETE c
    FROM Cliente c
    WHERE c.Id IN (SELECT Id FROM @Clientes);

    COMMIT;

    -- Re-habilitar la proteccion de facturacion
    ENABLE TRIGGER TR_DetalleServicio_BloquearModificacionFinalizada ON DetalleServicio;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK;
    ENABLE TRIGGER TR_DetalleServicio_BloquearModificacionFinalizada ON DetalleServicio;
    THROW;
END CATCH
GO

-- -----------------------------------------------------------------------
-- Verificacion: todos los SELECT deben devolver 0 filas
-- -----------------------------------------------------------------------
PRINT '--- Vehiculos TDD restantes (esperado 0) ---';
SELECT Id, Patente FROM Vehiculo WHERE Patente LIKE 'TDD%' ORDER BY Id;

PRINT '--- Clientes EndToEnd restantes (esperado 0) ---';
SELECT Id, Nombre, Apellido FROM Cliente WHERE Apellido = 'EndToEnd' ORDER BY Id;

PRINT '--- Ordenes sobre vehiculos TDD restantes (esperado 0) ---';
SELECT rs.Id, rs.Estado, rs.VehiculoId
FROM RegistroServicio rs
INNER JOIN Vehiculo v ON v.Id = rs.VehiculoId
WHERE v.Patente LIKE 'TDD%'
ORDER BY rs.Id;

PRINT '--- Verificacion del trigger (debe estar habilitado: is_disabled = 0) ---';
SELECT name, is_disabled FROM sys.triggers
WHERE name = 'TR_DetalleServicio_BloquearModificacionFinalizada';

PRINT '--- Datos conservados: conteo de ordenes/clientes/vehiculos (informativo) ---';
SELECT
    (SELECT COUNT(*) FROM Cliente)          AS Clientes,
    (SELECT COUNT(*) FROM Vehiculo)         AS Vehiculos,
    (SELECT COUNT(*) FROM RegistroServicio) AS Ordenes,
    (SELECT COUNT(*) FROM Factura)          AS Facturas,
    (SELECT COUNT(*) FROM Pago)             AS Pagos;
GO
