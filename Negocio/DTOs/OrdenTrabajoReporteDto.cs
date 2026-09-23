namespace TP_ControlVehicular.Negocio.DTOs
{
    public class OrdenTrabajoReporteDto
    {
        public int IdOrden { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string Patente { get; set; } = string.Empty;
        public string MarcaModelo { get; set; } = string.Empty;
        public string ClienteNombre { get; set; } = string.Empty;
        public string ClienteDni { get; set; } = string.Empty;
        public int KmIngresado { get; set; }
        public string ServiciosAplicados { get; set; } = string.Empty;
        public string MecanicoNombre { get; set; } = string.Empty;
        public string RecepcionistaNombre { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }
}
