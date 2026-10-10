-- =======================================================================
-- REPORTES GERENCIALES (Administrador)
-- =======================================================================

-- 1. Ingresos Totales por Fecha (versión administrativa; renombrada desde
--    sp_ReporteIngresos a sp_ReporteIngresosAdmin para evitar la colisión con la
--    versión de 3 parámetros consumida por ReporteRepository — ver bd/16)
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

-- 2. Top Clientes Frecuentes
CREATE OR ALTER PROCEDURE sp_ReporteTopClientes
    @FechaDesde DATE = NULL,
    @FechaHasta DATE = NULL,
    @TopN INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@TopN)
        c.Id AS ClienteId,
        c.Nombre,
        c.Apellido,
        c.Dni,
        COUNT(rs.Id) AS CantidadServicios,
        ISNULL(SUM(f.Total), 0) AS TotalGastado
    FROM Cliente c
    INNER JOIN PropietarioVehiculo pv ON c.Id = pv.ClienteId AND pv.EsActual = 1
    INNER JOIN Vehiculo v ON pv.VehiculoId = v.Id
    INNER JOIN RegistroServicio rs ON v.Id = rs.VehiculoId
    LEFT JOIN Factura f ON rs.Id = f.RegistroServicioId
    WHERE (@FechaDesde IS NULL OR CAST(rs.Fecha AS DATE) >= @FechaDesde)
      AND (@FechaHasta IS NULL OR CAST(rs.Fecha AS DATE) <= @FechaHasta)
    GROUP BY c.Id, c.Nombre, c.Apellido, c.Dni
    ORDER BY CantidadServicios DESC, TotalGastado DESC;
END;
GO

-- 3. Modelos de Vehículos Más Reparados
CREATE OR ALTER PROCEDURE sp_ReporteModelosReparados
    @FechaDesde DATE = NULL,
    @FechaHasta DATE = NULL,
    @TopN INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@TopN)
        ma.NombreMarca AS Marca,
        mo.NombreModelo AS Modelo,
        COUNT(rs.Id) AS CantidadReparaciones
    FROM RegistroServicio rs
    INNER JOIN Vehiculo v ON rs.VehiculoId = v.Id
    INNER JOIN Modelo mo ON v.ModeloId = mo.Id
    INNER JOIN Marca ma ON mo.MarcaId = ma.Id
    WHERE (@FechaDesde IS NULL OR CAST(rs.Fecha AS DATE) >= @FechaDesde)
      AND (@FechaHasta IS NULL OR CAST(rs.Fecha AS DATE) <= @FechaHasta)
    GROUP BY ma.NombreMarca, mo.NombreModelo
    ORDER BY CantidadReparaciones DESC;
END;
GO
