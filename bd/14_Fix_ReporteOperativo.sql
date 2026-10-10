-- =======================================================================
-- FIX DE DATOS - Reporte Operativo (Productividad por Mecanico)
-- -----------------------------------------------------------------------
-- Corrige datos historicos que quedaron con el usuario equivocado por rol:
--   1) DetalleServicio.UsuarioId debe ser SIEMPRE un Mecanico (RolId = 3).
--      Cualquier detalle asignado a un no-mecanico (ej. Recepcionista) se
--      reasigna a Pedro Canoero (Id 2), mecanico de referencia.
--   2) Anomalia: una orden Cancelada no debe tener tareas activas.
--      Las tareas 'En Curso' de ordenes 'Cancelada' se cierran ('Finalizada'),
--      ya que DetalleServicio no posee un estado 'Cancelada'.
--
-- Notas:
--   - RegistroServicio NO se modifica: se respeta quien registro la orden
--     (puede ser Administrador o Recepcionista).
--   - Reglas de rol por columna UsuarioId:
--       RegistroServicio.UsuarioId  -> quien recibe el vehiculo (Adm/Recep)
--       DetalleServicio.UsuarioId   -> Mecanico asignado (RolId = 3)
--   - Los detalles a corregir pertenecen a ordenes ya FACTURADAS, por lo que
--     el trigger TR_DetalleServicio_BloquearModificacionFinalizada los protege.
--     Al ser una correccion administrativa de datos se deshabilita el trigger
--     de forma temporal y se re-habilita SIEMPRE (incluso ante error),
--     dejandolo activo al finalizar.
--   - Script idempotente: puede ejecutarse varias veces sin efectos adversos.
-- =======================================================================
USE [ControlVehicular];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

-- Deshabilitar temporalmente la proteccion de facturacion
DISABLE TRIGGER TR_DetalleServicio_BloquearModificacionFinalizada ON DetalleServicio;

BEGIN TRY
    DECLARE @IdMecanico INT = (
        SELECT TOP (1) Id FROM Usuarios WHERE RolId = 3 ORDER BY Id
    );   -- Pedro Canoero (Id 2)

    IF @IdMecanico IS NULL
        THROW 50000, 'No existe ningun usuario Mecanico (RolId = 3). Se aborta el fix.', 1;

    BEGIN TRAN;

    -- 1) DetalleServicio: no-mecanicos -> mecanico de referencia
    UPDATE DetalleServicio
    SET UsuarioId = @IdMecanico
    WHERE UsuarioId NOT IN (SELECT Id FROM Usuarios WHERE RolId = 3);

    -- 2) Anomalia: tareas 'En Curso' de ordenes 'Cancelada' -> 'Finalizada'
    UPDATE DetalleServicio
    SET Estado = 'Finalizada'
    WHERE Estado = 'En Curso'
      AND RegistroServicioId IN (SELECT Id FROM RegistroServicio WHERE Estado = 'Cancelada');

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
-- Verificacion: los dos primeros SELECT deben devolver 0 filas
-- -----------------------------------------------------------------------
PRINT '--- Detalles asignados a un no-mecanico (esperado 0) ---';
SELECT ds.Id, ds.RegistroServicioId, ds.UsuarioId, u.RolId, u.Nombre, ds.Estado
FROM DetalleServicio ds
INNER JOIN Usuarios u ON u.Id = ds.UsuarioId
WHERE u.RolId <> 3;

PRINT '--- Tareas activas en ordenes Cancelada (esperado 0) ---';
SELECT ds.Id, ds.RegistroServicioId, ds.Estado
FROM DetalleServicio ds
INNER JOIN RegistroServicio rs ON rs.Id = ds.RegistroServicioId
WHERE rs.Estado = 'Cancelada'
  AND ds.Estado = 'En Curso';

PRINT '--- Verificacion del trigger (debe estar habilitado: is_disabled = 0) ---';
SELECT name, is_disabled FROM sys.triggers
WHERE name = 'TR_DetalleServicio_BloquearModificacionFinalizada';

PRINT '--- Productividad resultante (mecanicos) ---';
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
GROUP BY u.Id, u.Nombre, u.Apellido
ORDER BY TareasCompletadas DESC, TareasAsignadas DESC, u.Apellido ASC;
GO
