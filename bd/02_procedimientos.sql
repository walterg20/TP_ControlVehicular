-- ============================================================================
-- 02_procedimientos.sql — Procedimientos almacenados (reportes y comprobantes).
-- ----------------------------------------------------------------------------
-- Autor: consolidación de scripts históricos (2026-10-12).
-- Contenido: 14 procedimientos almacenados, cada uno con CREATE OR ALTER
--            (idempotente: puede re-ejecutarse sobre una base existente).
-- Requiere: esquema creado por 01_estructura.sql.
-- Compatible con SQL Server 2019 Express. Codificación UTF-8.
-- Orden de aplicación: 01 -> 02 -> 03 -> 04.
-- ----------------------------------------------------------------------------
-- NOTAS DE COLISIÓN DE NOMBRES (origen histórico):
--   * sp_ReporteIngresos: definido en 06_ (3 parámetros) y redefinido en 07_
--     (2 parámetros) con el MISMO nombre; la última definición aplicada pisaba
--     a la anterior. 16_ renombró la versión administrativa a
--     sp_ReporteIngresosAdmin y restauró sp_ReporteIngresos (3 parámetros).
--     Aquí se conserva esa resolución: ambas versiones canónicas provienen de 16_.
--   * sp_ReporteTiemposResolucion, sp_HistorialVehiculo, sp_VehiculosPorCliente
--     y sp_ServiciosPorMecanico: definidos en 02_StoredProcedures_Reportes.sql y
--     redefinidos (versión definitiva) en 06_Migraciones_Manuales.sql, posterior
--     en el orden de aplicación. Se conserva la versión de 06_.
-- ============================================================================
USE [ControlVehicular];
GO

-- ----------------------------------------------------------------------------
-- sp_ReporteIngresos — ingresos por fecha y taller (3 parámetros).
-- Consumido por ReporteRepository.ObtenerReporteIngresosAsync.
-- ----------------------------------------------------------------------------
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

-- ----------------------------------------------------------------------------
-- sp_ReporteIngresosAdmin — ingresos por fecha, consolidado (2 parámetros).
-- Consumido por ReporteGerencialRepository.ObtenerIngresosPorFechaAsync.
-- ----------------------------------------------------------------------------
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

-- ----------------------------------------------------------------------------
-- sp_ReporteTiemposResolucion — tiempo promedio de resolución por taller.
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_ReporteTiemposResolucion
    @StartDate DATETIME,
    @EndDate DATETIME,
    @WorkshopId INT = NULL
AS
BEGIN
    SELECT 
        t.Nombre AS WorkshopName,
        AVG(CAST(DATEDIFF(HOUR, rs.Fecha, f.Fecha) AS FLOAT) / 24.0) AS AverageResolutionTimeDays
    FROM RegistroServicio rs
    INNER JOIN Factura f ON rs.Id = f.RegistroServicioId
    INNER JOIN Taller t ON rs.TallerId = t.Id
    WHERE rs.Fecha >= @StartDate AND rs.Fecha <= @EndDate
      AND (@WorkshopId IS NULL OR t.Id = @WorkshopId)
    GROUP BY t.Nombre
    ORDER BY t.Nombre;
END;
GO

-- ----------------------------------------------------------------------------
-- sp_HistorialVehiculo — historial de servicios de un vehículo (por Id o patente).
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_HistorialVehiculo
    @VehiculoId INT = NULL,
    @Patente NVARCHAR(50) = NULL
AS
BEGIN
    SELECT 
        v.Id AS VehiculoId,
        v.Patente,
        ma.NombreMarca AS Marca,
        mo.NombreModelo AS Modelo,
        rs.Fecha AS FechaIngreso,
        f.Fecha AS FechaEgreso,
        rs.Estado,
        
        ISNULL(f.Total, 0) AS MontoTotal
    FROM Vehiculo v
    INNER JOIN Modelo mo ON v.ModeloId = mo.Id
    INNER JOIN Marca ma ON mo.MarcaId = ma.Id
    INNER JOIN RegistroServicio rs ON v.Id = rs.VehiculoId
    LEFT JOIN Factura f ON rs.Id = f.RegistroServicioId
    WHERE (@VehiculoId IS NULL OR v.Id = @VehiculoId)
      AND (@Patente IS NULL OR v.Patente = @Patente)
    ORDER BY rs.Fecha DESC;
END;
GO

-- ----------------------------------------------------------------------------
-- sp_VehiculosPorCliente — vehículos (actuales) de un cliente con conteo de servicios.
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_VehiculosPorCliente
    @ClienteId INT = NULL,
    @DNI NVARCHAR(50) = NULL
AS
BEGIN
    SELECT 
        c.Id AS ClienteId,
        c.Nombre AS NombreCliente,
        c.Apellido AS ApellidoCliente,
        c.Dni,
        v.Id AS VehiculoId,
        v.Patente,
        ma.NombreMarca AS Marca,
        mo.NombreModelo AS Modelo,
        COUNT(rs.Id) AS CantidadServicio
    FROM Cliente c
    INNER JOIN PropietarioVehiculo pv ON c.Id = pv.ClienteId AND pv.EsActual = 1
    INNER JOIN Vehiculo v ON pv.VehiculoId = v.Id
    INNER JOIN Modelo mo ON v.ModeloId = mo.Id
    INNER JOIN Marca ma ON mo.MarcaId = ma.Id
    LEFT JOIN RegistroServicio rs ON v.Id = rs.VehiculoId
    WHERE (@ClienteId IS NULL OR c.Id = @ClienteId)
      AND (@DNI IS NULL OR c.Dni = @DNI)
    GROUP BY c.Id, c.Nombre, c.Apellido, c.Dni, v.Id, v.Patente, ma.NombreMarca, mo.NombreModelo
    ORDER BY c.Apellido, c.Nombre;
END;
GO

-- ----------------------------------------------------------------------------
-- sp_ServiciosPorMecanico — tareas realizadas por un mecánico en un rango.
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_ServiciosPorMecanico
    @FechaDesde DATETIME,
    @FechaHasta DATETIME,
    @MecanicoId INT
AS
BEGIN
    SELECT 
        rs.Fecha AS Fecha,
        v.Patente AS Patente,
        s.Nombre AS Servicio,
        ds.Cantidad AS Cantidad,
        (ds.Precio * ds.Cantidad) AS Importe
    FROM DetalleServicio ds
    INNER JOIN RegistroServicio rs ON ds.RegistroServicioId = rs.Id
    INNER JOIN Vehiculo v ON rs.VehiculoId = v.Id
    INNER JOIN Servicio s ON ds.ServicioId = s.Id
    WHERE rs.Fecha >= @FechaDesde AND rs.Fecha <= @FechaHasta
      AND ds.UsuarioId = @MecanicoId
    ORDER BY rs.Fecha DESC;
END;
GO

-- ----------------------------------------------------------------------------
-- sp_ReporteTopClientes — clientes con mayor cantidad de servicios y gasto.
-- ----------------------------------------------------------------------------
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

-- ----------------------------------------------------------------------------
-- sp_ReporteModelosReparados — modelos de vehículos más reparados.
-- ----------------------------------------------------------------------------
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

-- ----------------------------------------------------------------------------
-- sp_HistorialClinicoVehiculo — detalle de servicios por vehículo (reporte mecánico).
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_HistorialClinicoVehiculo
    @VehiculoId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        CAST(rs.Fecha AS DATE) AS Fecha,
        rs.KmIngreso,
        s.Nombre AS Servicio,
        u.Nombre + ' ' + u.Apellido AS Mecanico,
        ds.Estado,
        ds.Observaciones
    FROM RegistroServicio rs
    INNER JOIN DetalleServicio ds ON rs.Id = ds.RegistroServicioId
    INNER JOIN Servicio s ON ds.ServicioId = s.Id
    LEFT JOIN Usuarios u ON ds.UsuarioId = u.Id
    WHERE rs.VehiculoId = @VehiculoId
    ORDER BY rs.Fecha DESC, ds.Id ASC;
END;
GO

-- ----------------------------------------------------------------------------
-- sp_ReporteHojaTrabajoDiaria — tareas pendientes/en curso de un mecánico.
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_ReporteHojaTrabajoDiaria
    @MecanicoId INT,
    @Fecha DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @Fecha IS NULL SET @Fecha = CAST(GETDATE() AS DATE);

    SELECT 
        v.Patente,
        ma.NombreMarca + ' ' + mo.NombreModelo AS Vehiculo,
        c.Nombre + ' ' + c.Apellido AS Cliente,
        s.Nombre AS Servicio,
        ds.OrdenEjecucion,
        ds.Estado,
        ds.Observaciones
    FROM DetalleServicio ds
    INNER JOIN RegistroServicio rs ON ds.RegistroServicioId = rs.Id
    INNER JOIN Vehiculo v ON rs.VehiculoId = v.Id
    INNER JOIN Modelo mo ON v.ModeloId = mo.Id
    INNER JOIN Marca ma ON mo.MarcaId = ma.Id
    INNER JOIN PropietarioVehiculo pv ON v.Id = pv.VehiculoId AND pv.EsActual = 1
    INNER JOIN Cliente c ON pv.ClienteId = c.Id
    INNER JOIN Servicio s ON ds.ServicioId = s.Id
    WHERE ds.UsuarioId = @MecanicoId
      AND ds.Estado IN ('Pendiente', 'En Curso')
      AND rs.Estado NOT IN ('Cancelada', 'Pagada')
    ORDER BY ds.OrdenEjecucion ASC, rs.Fecha ASC;
END;
GO

-- ----------------------------------------------------------------------------
-- sp_ReporteProductividadMecanicos — productividad por mecánico (a nivel de tarea).
-- Solo cuenta tareas cuyo UsuarioId es un Mecánico (RolId = 3).
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_ReporteProductividadMecanicos
    @FechaDesde DATE = NULL,
    @FechaHasta DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        u.Id AS MecanicoId,
        u.Nombre + ' ' + u.Apellido AS MecanicoNombre,
        COUNT(*) AS TareasAsignadas,
        SUM(CASE WHEN ds.Estado = 'Pendiente'  THEN 1 ELSE 0 END) AS Pendientes,
        SUM(CASE WHEN ds.Estado = 'En Curso'   THEN 1 ELSE 0 END) AS EnCurso,
        SUM(CASE WHEN ds.Estado = 'Finalizada' THEN 1 ELSE 0 END) AS TareasCompletadas
    FROM DetalleServicio ds
    INNER JOIN RegistroServicio rs ON ds.RegistroServicioId = rs.Id
    INNER JOIN Usuarios u ON ds.UsuarioId = u.Id
    WHERE u.RolId = 3
      AND rs.Estado <> 'Cancelada'
      AND (@FechaDesde IS NULL OR CAST(rs.Fecha AS DATE) >= @FechaDesde)
      AND (@FechaHasta IS NULL OR CAST(rs.Fecha AS DATE) <= @FechaHasta)
    GROUP BY u.Id, u.Nombre, u.Apellido
    ORDER BY TareasCompletadas DESC, TareasAsignadas DESC, u.Apellido ASC;
END;
GO

-- ----------------------------------------------------------------------------
-- sp_ReporteServiciosPorRango — cantidad de cada servicio en un rango (a nivel de tarea).
-- Excluye órdenes canceladas.
-- ----------------------------------------------------------------------------
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

-- ----------------------------------------------------------------------------
-- sp_GenerarComprobanteOrden — datos de la orden de servicio para el comprobante.
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GenerarComprobanteOrden
    @OrdenId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        r.Id AS OrdenId,
        c.Nombre + ' ' + c.Apellido AS ClienteNombre,
        c.Dni AS ClienteDocumento,
        v.Patente AS VehiculoPatente,
        m.NombreMarca AS VehiculoMarca,
        mod.NombreModelo AS VehiculoModelo,
        r.Fecha AS FechaRecepcion,
        NULL AS FechaEstimadaEntrega,
        '' AS Observaciones,
        
        ISNULL(t.Nombre, 'Taller Pro') AS TallerNombre,
        ISNULL(t.Direccion, 'mz121') AS TallerDireccion,
        ISNULL(t.Telefono, '362456578') AS TallerTelefono,
        
        ds.Id AS DetalleId,
        s.Nombre AS ProductoOServicio,
        ds.Observaciones AS ObservacionDetalle
    FROM RegistroServicio r
    INNER JOIN Vehiculo v ON r.VehiculoId = v.Id
    INNER JOIN PropietarioVehiculo pv ON v.Id = pv.VehiculoId AND pv.EsActual = 1
    INNER JOIN Cliente c ON pv.ClienteId = c.Id
    INNER JOIN Modelo mod ON v.ModeloId = mod.Id
    INNER JOIN Marca m ON mod.MarcaId = m.Id
    LEFT JOIN Taller t ON r.TallerId = t.Id
    LEFT JOIN DetalleServicio ds ON r.Id = ds.RegistroServicioId
    LEFT JOIN Servicio s ON ds.ServicioId = s.Id
    WHERE r.Id = @OrdenId;
END
GO

-- ----------------------------------------------------------------------------
-- sp_GenerarComprobantePago — datos de la orden, factura y pagos para el comprobante.
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GenerarComprobantePago
    @OrdenId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        r.Id AS OrdenId,
        r.Fecha AS FechaPago,
        r.KmIngreso AS Kilometraje,
        
        ISNULL(t.Nombre, 'Taller Pro') AS TallerNombre,
        ISNULL(t.Direccion, 'mz121') AS TallerDireccion,
        ISNULL(t.Telefono, '362456578') AS TallerTelefono,

        c.Nombre + ' ' + c.Apellido AS ClienteNombre,
        c.Dni AS ClienteDocumento,
        ISNULL(c.Telefono, '') AS ClienteTelefono,
        ISNULL(c.Direccion, '') AS ClienteDireccion,
        ISNULL(c.Email, '') AS ClienteEmail,

        v.Patente AS VehiculoPatente,
        m.NombreMarca + ' ' + mod.NombreModelo + ' ' + CAST(v.Anio AS VARCHAR) AS VehiculoDescripcion,
        
        ds.Id AS DetalleId,
        s.Nombre AS ProductoOServicio,
        ds.Precio AS Precio,
        ds.Cantidad AS Cantidad,
        (ds.Precio * ds.Cantidad) AS SubtotalDetalle,
        ds.Origen,
        
        -- Datos de la factura y el total pagado hasta el momento (con manejo de NULL por si no está pagada)
        f.Id AS FacturaId,
        ISNULL(f.Total, 0) AS TotalFactura,
        ISNULL((SELECT SUM(Monto) FROM Pago p WHERE p.FacturaId = f.Id), 0) AS TotalPagado
        
    FROM RegistroServicio r
    INNER JOIN Vehiculo v ON r.VehiculoId = v.Id
    INNER JOIN PropietarioVehiculo pv ON v.Id = pv.VehiculoId AND pv.EsActual = 1
    INNER JOIN Cliente c ON pv.ClienteId = c.Id
    INNER JOIN Modelo mod ON v.ModeloId = mod.Id
    INNER JOIN Marca m ON mod.MarcaId = m.Id
    LEFT JOIN Taller t ON r.TallerId = t.Id
    LEFT JOIN DetalleServicio ds ON r.Id = ds.RegistroServicioId
    LEFT JOIN Servicio s ON ds.ServicioId = s.Id
    -- Incorporamos la Factura mediante LEFT JOIN
    LEFT JOIN Factura f ON r.Id = f.RegistroServicioId
    WHERE r.Id = @OrdenId;
END
GO
