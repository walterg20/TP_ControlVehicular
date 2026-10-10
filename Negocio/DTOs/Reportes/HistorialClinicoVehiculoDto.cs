namespace TP_ControlVehicular.Negocio.DTOs.Reportes
{
    public class HistorialClinicoVehiculoDto
    {
        public System.DateTime Fecha { get; set; }
        public int KmIngreso { get; set; }
        public string Servicio { get; set; } = string.Empty;
        public string Mecanico { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
    }
}
