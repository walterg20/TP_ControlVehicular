namespace TP_ControlVehicular.Entidad
{
    public class DetalleServicio
    {
        public int Id { get; set; }
        public int RegistroServicioId { get; set; }
        public int UsuarioId { get; set; } // Mecánico asignado que realiza el servicio
        public int ServicioId { get; set; }
        public int Cantidad { get; set; } = 1;
        public decimal Precio { get; set; }
        public string Origen { get; set; } = "Taller";
        public string Estado { get; set; } = "Realizado";

        public RegistroServicio RegistroServicio { get; set; } = null!;
        public Usuario Usuario { get; set; } = null!; // Mecánico
        public Servicio Servicio { get; set; } = null!;
    }
}
