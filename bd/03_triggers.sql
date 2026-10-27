-- ============================================================================
-- 03_triggers.sql — Disparadores (triggers) de integridad del workflow.
-- ----------------------------------------------------------------------------
-- Autor: consolidación de scripts históricos (2026-10-12).
-- Contenido: 3 triggers, cada uno con CREATE OR ALTER
--            (idempotente: puede re-ejecutarse sobre una base existente).
-- Requiere: esquema creado por 01_estructura.sql.
-- Compatible con SQL Server 2019 Express. Codificación UTF-8.
-- Orden de aplicación: 01 -> 02 -> 03 -> 04.
-- ----------------------------------------------------------------------------
-- NOTA: las restricciones CHECK definidas junto a estos triggers en
-- 06_Migraciones_Manuales.sql (CK_Servicio_Precio, CK_DetalleServicio_Precio,
-- CK_DetalleServicio_Cantidad, CK_RegistroServicio_KmIngreso, CK_Vehiculo_KmActual)
-- y las de estado (CHK_DetalleServicio_Estado, CHK_RegistroServicio_Estado, estas
-- últimas de 05_WorkflowOrdenesServicio.sql) se consolidaron como constraints
-- en línea dentro de 01_estructura.sql; aquí solo viven los triggers.
-- ============================================================================
USE [ControlVehicular];
GO

-- ----------------------------------------------------------------------------
-- TR_DetalleServicio_ValidarPrecioYCantidad
-- Valida que el precio no sea negativo y la cantidad sea positiva en cada
-- inserción/actualización de DetalleServicio.
-- ----------------------------------------------------------------------------
CREATE OR ALTER TRIGGER TR_DetalleServicio_ValidarPrecioYCantidad
ON DetalleServicio
AFTER INSERT, UPDATE
AS
BEGIN
    IF EXISTS (SELECT 1 FROM inserted WHERE Precio < 0 OR Cantidad <= 0)
    BEGIN
        RAISERROR('Precio y cantidad deben ser válidos', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO

-- ----------------------------------------------------------------------------
-- TR_RegistroServicio_SincronizarKm
-- Sincroniza el kilometraje del vehículo al registrar una orden (si el ingreso
-- es mayor o igual al actual) y bloquea el ingreso si es menor al actual.
-- ----------------------------------------------------------------------------
CREATE OR ALTER TRIGGER TR_RegistroServicio_SincronizarKm
ON RegistroServicio
AFTER INSERT
AS
BEGIN
    UPDATE v
    SET v.KmActual = i.KmIngreso
    FROM Vehiculo v
    INNER JOIN inserted i ON v.Id = i.VehiculoId
    WHERE i.KmIngreso >= v.KmActual;

    DECLARE @ErrorMsg NVARCHAR(200);
    DECLARE @KmIngreso INT;
    DECLARE @KmActual INT;

    SELECT TOP 1 @KmIngreso = i.KmIngreso, @KmActual = v.KmActual
    FROM inserted i
    INNER JOIN Vehiculo v ON v.Id = i.VehiculoId
    WHERE i.KmIngreso < v.KmActual;

    IF @KmActual IS NOT NULL
    BEGIN
        SET @ErrorMsg = CONCAT('El kilometraje de ingreso (', @KmIngreso, ') no puede ser menor al actual del vehículo (', @KmActual, ').');
        RAISERROR(@ErrorMsg, 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO

-- ----------------------------------------------------------------------------
-- TR_DetalleServicio_BloquearModificacionFinalizada
-- Impide modificar detalles de una orden de servicio que ya fue facturada.
-- ----------------------------------------------------------------------------
CREATE OR ALTER TRIGGER TR_DetalleServicio_BloquearModificacionFinalizada
ON DetalleServicio
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    IF EXISTS (
        SELECT 1 
        FROM RegistroServicio rs
        INNER JOIN Factura f ON rs.Id = f.RegistroServicioId
        WHERE rs.Id IN (SELECT RegistroServicioId FROM inserted UNION SELECT RegistroServicioId FROM deleted)
    )
    BEGIN
        RAISERROR('No se pueden modificar detalles de una orden ya facturada.', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO
