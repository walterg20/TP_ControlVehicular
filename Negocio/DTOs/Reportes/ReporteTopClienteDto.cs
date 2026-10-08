namespace TP_ControlVehicular.Negocio.DTOs.Reportes
{
    public class ReporteTopClienteDto
    {
        public int ClienteId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public int CantidadServicios { get; set; }
        public decimal TotalGastado { get; set; }
        
        public string NombreCompleto => $"{Nombre} {Apellido}";
    }
}
