namespace TP_ControlVehicular.Negocio.DTOs.Reportes
{
    public class ComprobanteOrdenRowDto
    {
        public int OrdenId { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;
        public string ClienteDocumento { get; set; } = string.Empty;
        public string VehiculoPatente { get; set; } = string.Empty;
        public string VehiculoMarca { get; set; } = string.Empty;
        public string VehiculoModelo { get; set; } = string.Empty;
        public DateTime FechaRecepcion { get; set; }
        public DateTime? FechaEstimadaEntrega { get; set; }
        public string Observaciones { get; set; } = string.Empty;
        
        public string TallerNombre { get; set; } = string.Empty;
        public string TallerDireccion { get; set; } = string.Empty;
        public string TallerTelefono { get; set; } = string.Empty;
        
        public int? DetalleId { get; set; }
        public string? ProductoOServicio { get; set; }
        public string? ObservacionDetalle { get; set; }
    }

    public class ComprobanteOrdenDto
    {
        public int OrdenId { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;
        public string ClienteDocumento { get; set; } = string.Empty;
        public string VehiculoPatente { get; set; } = string.Empty;
        public string VehiculoMarca { get; set; } = string.Empty;
        public string VehiculoModelo { get; set; } = string.Empty;
        public DateTime FechaRecepcion { get; set; }
        public DateTime? FechaEstimadaEntrega { get; set; }
        public string Observaciones { get; set; } = string.Empty;

        public string TallerNombre { get; set; } = string.Empty;
        public string TallerDireccion { get; set; } = string.Empty;
        public string TallerTelefono { get; set; } = string.Empty;

        public List<DetalleOrdenDto> Detalles { get; set; } = new List<DetalleOrdenDto>();
    }

    public class DetalleOrdenDto
    {
        public string ProductoOServicio { get; set; } = string.Empty;
        public string Observacion { get; set; } = string.Empty;
    }
}
