namespace TP_ControlVehicular.Negocio.DTOs
{
    public class VehiculoDto
    {
        public int Id { get; set; }
        public string Patente { get; set; } = string.Empty;
        public int Anio { get; set; }
        public int KmActual { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;
        public string ModeloNombre { get; set; } = string.Empty;
        public string MarcaNombre { get; set; } = string.Empty;
    }
}
