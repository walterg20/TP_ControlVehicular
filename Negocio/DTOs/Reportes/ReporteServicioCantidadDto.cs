namespace TP_ControlVehicular.Negocio.DTOs.Reportes
{
    public class ReporteServicioCantidadDto
    {
        public int ServicioId { get; set; }
        public string Servicio { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }
}
