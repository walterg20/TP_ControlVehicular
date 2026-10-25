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
        f.id_factura AS FacturaId,
        ISNULL(f.Total, 0) AS TotalFactura,
        ISNULL((SELECT SUM(Monto) FROM Pago p WHERE p.id_factura = f.id_factura), 0) AS TotalPagado
        
    FROM RegistroServicio r
    INNER JOIN Vehiculo v ON r.VehiculoId = v.Id
    INNER JOIN Cliente c ON v.ClienteId = c.Id
    INNER JOIN Modelo mod ON v.ModeloId = mod.Id
    INNER JOIN Marca m ON mod.MarcaId = m.Id
    LEFT JOIN Taller t ON r.TallerId = t.Id
    LEFT JOIN DetalleServicio ds ON r.Id = ds.RegistroServicioId
    LEFT JOIN Servicio s ON ds.ServicioId = s.Id
    -- Incorporamos la Factura mediante LEFT JOIN
    LEFT JOIN Factura f ON r.Id = f.id_servicio
    WHERE r.Id = @OrdenId;
END
GO
