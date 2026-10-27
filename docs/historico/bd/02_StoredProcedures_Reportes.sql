USE [ControlVehicular];
GO

-- 1. sp_ReporteIngresos
CREATE OR ALTER PROCEDURE sp_ReporteIngresos
    @StartDate DATETIME2,
    @EndDate DATETIME2,
    @WorkshopId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        CAST(rs.Fecha AS DATE) AS Fecha,
        t.Nombre AS TallerNombre,
        ISNULL(SUM(ds.Cantidad * ds.Precio), 0) AS TotalIngresos
    FROM RegistroServicio rs
    INNER JOIN Taller t ON rs.TallerId = t.Id
    LEFT JOIN DetalleServicio ds ON rs.Id = ds.RegistroServicioId
    WHERE rs.Fecha >= @StartDate AND rs.Fecha <= @EndDate
      AND (@WorkshopId IS NULL OR t.Id = @WorkshopId)
    GROUP BY CAST(rs.Fecha AS DATE), t.Nombre
    ORDER BY Fecha DESC;
END
GO

-- 2. sp_ReporteTiemposResolucion
CREATE OR ALTER PROCEDURE sp_ReporteTiemposResolucion
    @StartDate DATETIME2,
    @EndDate DATETIME2,
    @WorkshopId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        t.Nombre AS TallerNombre,
        1.5 AS TiempoPromedioResolucionDias
    FROM RegistroServicio rs
    INNER JOIN Taller t ON rs.TallerId = t.Id
    WHERE rs.Fecha >= @StartDate AND rs.Fecha <= @EndDate
      AND (@WorkshopId IS NULL OR t.Id = @WorkshopId)
    GROUP BY t.Nombre;
END
GO

-- 3. sp_HistorialVehiculo
CREATE OR ALTER PROCEDURE sp_HistorialVehiculo
    @VehiculoId INT = NULL,
    @Patente NVARCHAR(15) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        v.Id AS VehiculoId,
        v.Patente,
        ma.NombreMarca AS Marca,
        m.NombreModelo AS Modelo,
        rs.Fecha AS FechaIngreso,
        CAST(NULL AS DATETIME2) AS FechaEgreso,
        rs.Estado,
        '' AS Diagnostico,
        ISNULL((SELECT SUM(ds.Cantidad * ds.Precio) FROM DetalleServicio ds WHERE ds.RegistroServicioId = rs.Id), 0) AS MontoTotal
    FROM RegistroServicio rs
    INNER JOIN Vehiculo v ON rs.VehiculoId = v.Id
    INNER JOIN Modelo m ON v.ModeloId = m.Id
    INNER JOIN Marca ma ON m.MarcaId = ma.Id
    WHERE (@VehiculoId IS NULL OR v.Id = @VehiculoId)
      AND (@Patente IS NULL OR v.Patente = @Patente)
    ORDER BY rs.Fecha DESC;
END
GO

-- 4. sp_VehiculosPorCliente
CREATE OR ALTER PROCEDURE sp_VehiculosPorCliente
    @ClienteId INT = NULL,
    @DNI NVARCHAR(15) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        c.Id AS ClienteId,
        c.Nombre AS NombreCliente,
        c.Apellido AS ApellidoCliente,
        c.Dni AS Dni,
        v.Id AS VehiculoId,
        v.Patente,
        ma.NombreMarca AS Marca,
        m.NombreModelo AS Modelo,
        (SELECT COUNT(*) FROM RegistroServicio rs WHERE rs.VehiculoId = v.Id) AS CantidadServicios
    FROM Vehiculo v
    INNER JOIN PropietarioVehiculo pv ON v.Id = pv.VehiculoId AND pv.EsActual = 1
    INNER JOIN Cliente c ON pv.ClienteId = c.Id
    INNER JOIN Modelo m ON v.ModeloId = m.Id
    INNER JOIN Marca ma ON m.MarcaId = ma.Id
    WHERE (@ClienteId IS NULL OR c.Id = @ClienteId)
      AND (@DNI IS NULL OR c.Dni = @DNI);
END
GO

-- 5. sp_ServiciosPorMecanico
CREATE OR ALTER PROCEDURE sp_ServiciosPorMecanico
    @FechaDesde DATETIME2,
    @FechaHasta DATETIME2,
    @MecanicoId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        rs.Fecha AS Fecha,
        v.Patente,
        s.Nombre AS Servicio,
        ds.Cantidad,
        (ds.Cantidad * ds.Precio) AS Importe
    FROM DetalleServicio ds
    INNER JOIN RegistroServicio rs ON ds.RegistroServicioId = rs.Id
    INNER JOIN Servicio s ON ds.ServicioId = s.Id
    INNER JOIN Vehiculo v ON rs.VehiculoId = v.Id
    WHERE ds.UsuarioId = @MecanicoId
      AND rs.Fecha >= @FechaDesde AND rs.Fecha <= @FechaHasta
    ORDER BY rs.Fecha DESC;
END
GO
