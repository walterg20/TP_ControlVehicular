import io

sql = '''-- =======================================================================
-- REPORTES OPERATIVOS (Mecánico)
-- =======================================================================
USE [ControlVehicular];
GO

-- 1. Historial Clínico del Vehículo
CREATE OR ALTER PROCEDURE sp_ReporteHistorialVehiculo
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

-- 2. Hoja de Trabajo Diaria (Tareas Pendientes)
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
'''
with io.open('bd/11_SP_ReportesMecanico.sql', 'w', encoding='utf-8') as f:
    f.write(sql)