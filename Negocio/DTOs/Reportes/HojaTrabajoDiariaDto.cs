namespace TP_ControlVehicular.Negocio.DTOs.Reportes
{
    public class HojaTrabajoDiariaDto
    {
        public string Patente { get; set; } = string.Empty;
        public string Vehiculo { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public string Servicio { get; set; } = string.Empty;
        public int OrdenEjecucion { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
    }
}
