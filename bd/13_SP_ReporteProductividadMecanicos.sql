-- =======================================================================
-- REPORTE OPERATIVO (Recepcionista) - Productividad por Mecanico
-- Agrega a nivel de TAREA (DetalleServicio), no de orden.
-- Script idempotente: puede ejecutarse varias veces.
-- =======================================================================
USE [ControlVehicular];
GO

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
    WHERE (@FechaDesde IS NULL OR CAST(rs.Fecha AS DATE) >= @FechaDesde)
      AND (@FechaHasta IS NULL OR CAST(rs.Fecha AS DATE) <= @FechaHasta)
    GROUP BY u.Id, u.Nombre, u.Apellido
    ORDER BY TareasCompletadas DESC, TareasAsignadas DESC, u.Apellido ASC;
END;
GO
