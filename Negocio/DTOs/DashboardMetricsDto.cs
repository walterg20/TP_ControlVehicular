namespace TP_ControlVehicular.Negocio.DTOs
{
    public class DashboardMetricsDto
    {
        public int VehiculosActivosCount { get; set; }
        public int EnProcesoCount { get; set; }
        public int EntregadasHoyCount { get; set; }
        public List<VehiculoDto> VehiculosEnTaller { get; set; } = new();
    }
}
