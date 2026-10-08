namespace TP_ControlVehicular.Negocio.DTOs
{
    public class RegistroServicioDto
    {
        public int Id { get; set; }
        public int VehiculoId { get; set; }
        public int TallerId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime Fecha { get; set; }
        public int KmIngreso { get; set; }
        public string Estado { get; set; } = string.Empty;

        // Propiedades de navegaciÃ³n aplanadas para la grilla
        public string ClienteDetalle { get; set; } = string.Empty;
        public string VehiculoPatente { get; set; } = string.Empty;
        public string VehiculoDetalle { get; set; } = string.Empty; // e.g. "Toyota Corolla"
        public string TallerNombre { get; set; } = string.Empty;
        public string RecepcionistaNombre { get; set; } = string.Empty;

        public string EstaPagadoStr => Estado == "Pagada" ? "SÃ­" : "No";

        // Lista de detalles para la grilla de checkout (inspecciÃ³n)
        public List<DetalleServicioDto> Detalles { get; set; } = new List<DetalleServicioDto>();
    }
}
