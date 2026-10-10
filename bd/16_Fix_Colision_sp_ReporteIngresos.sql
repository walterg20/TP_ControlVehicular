-- =======================================================================
-- FIX: Colision de nombre en sp_ReporteIngresos
-- =======================================================================
-- Contexto:
--   bd/06_Migraciones_Manuales.sql define sp_ReporteIngresos con 3 parametros
--     (@StartDate, @EndDate, @WorkshopId) y columnas Date/WorkshopName/TotalRevenue.
--   bd/07_Reportes_Administrador.sql redefine el MISMO nombre con 2 parametros
--     (@FechaDesde, @FechaHasta) y columnas Fecha/CantidadFacturas/IngresosTotales.
--   Al compartir nombre, la ultima definicion aplicada (bd/07) pisa a la anterior,
--   rompiendo el reporte de ingresos general consumido por
--   ReporteRepository.ObtenerReporteIngresosAsync (3 parametros).
-- Solucion:
--   1) Renombrar la version administrativa (2 parametros) a sp_ReporteIngresosAdmin.
--   2) Restaurar sp_ReporteIngresos con la firma de 3 parametros, alineando los
--      alias de columna al DTO ReporteIngresosDto (Fecha, TallerNombre, TotalIngresos).
-- Idempotente (CREATE OR ALTER).
-- =======================================================================
USE [ControlVehicular];
GO

-- 1) Version administrativa (consumida por ReporteGerencialRepository.ObtenerIngresosPorFechaAsync)
CREATE OR ALTER PROCEDURE sp_ReporteIngresosAdmin
    @FechaDesde DATE = NULL,
    @FechaHasta DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CAST(f.Fecha AS DATE) AS Fecha,
        COUNT(f.Id) AS CantidadFacturas,
        SUM(f.Total) AS IngresosTotales
    FROM Factura f
    WHERE (@FechaDesde IS NULL OR CAST(f.Fecha AS DATE) >= @FechaDesde)
      AND (@FechaHasta IS NULL OR CAST(f.Fecha AS DATE) <= @FechaHasta)
    GROUP BY CAST(f.Fecha AS DATE)
    ORDER BY CAST(f.Fecha AS DATE) DESC;
END;
GO

-- 2) Version general (consumida por ReporteRepository.ObtenerReporteIngresosAsync)
CREATE OR ALTER PROCEDURE sp_ReporteIngresos
    @StartDate DATETIME,
    @EndDate DATETIME,
    @WorkshopId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CAST(f.Fecha AS DATE) AS Fecha,
        t.Nombre AS TallerNombre,
        SUM(f.Total) AS TotalIngresos
    FROM Factura f
    INNER JOIN RegistroServicio rs ON f.RegistroServicioId = rs.Id
    INNER JOIN Taller t ON rs.TallerId = t.Id
    WHERE f.Fecha >= @StartDate AND f.Fecha <= @EndDate
      AND (@WorkshopId IS NULL OR t.Id = @WorkshopId)
    GROUP BY CAST(f.Fecha AS DATE), t.Nombre
    ORDER BY CAST(f.Fecha AS DATE) DESC;
END;
GO
