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

        // Propiedades de navegación aplanadas para la grilla
        public string VehiculoPatente { get; set; } = string.Empty;
        public string VehiculoDetalle { get; set; } = string.Empty; // e.g. "Toyota Corolla"
        public string TallerNombre { get; set; } = string.Empty;
        public string RecepcionistaNombre { get; set; } = string.Empty;
    }
}
