-- =======================================================================
-- REPORTE GERENCIAL (Administrador) - Cantidad de cada servicio en un rango
-- Agrega a nivel de TAREA (DetalleServicio): SUM(Cantidad) por servicio.
-- Excluye ordenes canceladas (no debe contarse trabajo de una orden cancelada).
-- Script idempotente: puede ejecutarse varias veces.
-- =======================================================================
USE [ControlVehicular];
GO

CREATE OR ALTER PROCEDURE sp_ReporteServiciosPorRango
    @FechaDesde DATE = NULL,
    @FechaHasta DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        s.Id AS ServicioId,
        s.Nombre AS Servicio,
        SUM(ds.Cantidad) AS Cantidad
    FROM DetalleServicio ds
    INNER JOIN RegistroServicio rs ON ds.RegistroServicioId = rs.Id
    INNER JOIN Servicio s ON ds.ServicioId = s.Id
    WHERE rs.Estado <> 'Cancelada'
      AND (@FechaDesde IS NULL OR CAST(rs.Fecha AS DATE) >= @FechaDesde)
      AND (@FechaHasta IS NULL OR CAST(rs.Fecha AS DATE) <= @FechaHasta)
    GROUP BY s.Id, s.Nombre
    HAVING SUM(ds.Cantidad) > 0
    ORDER BY Cantidad DESC, s.Nombre ASC;
END;
GO
