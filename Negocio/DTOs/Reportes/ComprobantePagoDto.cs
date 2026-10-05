namespace TP_ControlVehicular.Negocio.DTOs.Reportes
{
    public class ComprobantePagoRowDto
    {
        public int OrdenId { get; set; }
        public DateTime FechaPago { get; set; }
        public int Kilometraje { get; set; }
        
        public string TallerNombre { get; set; } = string.Empty;
        public string TallerDireccion { get; set; } = string.Empty;
        public string TallerTelefono { get; set; } = string.Empty;
        
        public string ClienteNombre { get; set; } = string.Empty;
        public string ClienteDocumento { get; set; } = string.Empty;
        public string ClienteTelefono { get; set; } = string.Empty;
        public string ClienteDireccion { get; set; } = string.Empty;
        public string ClienteEmail { get; set; } = string.Empty;
        
        public string VehiculoPatente { get; set; } = string.Empty;
        public string VehiculoDescripcion { get; set; } = string.Empty;
        
        // Detalle (puede ser nulo si la orden no tiene detalles an)
        public int? DetalleId { get; set; }
        public string? ProductoOServicio { get; set; }
        public decimal? Precio { get; set; }
        public int? Cantidad { get; set; }
        public decimal? SubtotalDetalle { get; set; }
        public string? Origen { get; set; }
    }

    public class ComprobantePagoDto
    {
        public int OrdenId { get; set; }
        public DateTime FechaPago { get; set; }
        public int Kilometraje { get; set; }
        
        public string TallerNombre { get; set; } = string.Empty;
        public string TallerDireccion { get; set; } = string.Empty;
        public string TallerTelefono { get; set; } = string.Empty;
        
        public string ClienteNombre { get; set; } = string.Empty;
        public string ClienteDocumento { get; set; } = string.Empty;
        public string ClienteTelefono { get; set; } = string.Empty;
        public string ClienteDireccion { get; set; } = string.Empty;
        public string ClienteEmail { get; set; } = string.Empty;
        
        public string VehiculoPatente { get; set; } = string.Empty;
        public string VehiculoDescripcion { get; set; } = string.Empty;
        
        public decimal TotalManoObra { get; set; }
        public decimal TotalRepuestos { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal TotalGeneral { get; set; }

        public List<DetalleComprobanteDto> Detalles { get; set; } = new List<DetalleComprobanteDto>();
    }

    public class DetalleComprobanteDto
    {
        public string ProductoOServicio { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
    }
}
