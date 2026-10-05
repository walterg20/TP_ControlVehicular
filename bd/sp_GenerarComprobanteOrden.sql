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
    INNER JOIN Cliente c ON v.ClienteId = c.Id
    INNER JOIN Modelo mod ON v.ModeloId = mod.Id
    INNER JOIN Marca m ON mod.MarcaId = m.Id
    LEFT JOIN Taller t ON r.TallerId = t.Id
    LEFT JOIN DetalleServicio ds ON r.Id = ds.RegistroServicioId
    LEFT JOIN Servicio s ON ds.ServicioId = s.Id
    WHERE r.Id = @OrdenId;
END
GO
