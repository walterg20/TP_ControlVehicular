namespace TP_ControlVehicular.Entidad
{
    public class DetalleServicio
    {
        public const string EstadoPendiente = "Pendiente";
        public const string EstadoEnCurso = "En Curso";
        public const string EstadoFinalizada = "Finalizada";

        public int Id { get; set; }
        public int RegistroServicioId { get; set; }
        public int UsuarioId { get; set; } // Mecánico asignado que realiza el servicio
        public int ServicioId { get; set; }
        public int Cantidad { get; set; } = 1;
        public decimal Precio { get; set; }
        public int OrdenEjecucion { get; set; } = 1;
        public string Origen { get; set; } = "Taller";
        public string Estado { get; set; } = EstadoPendiente;
        public string Observaciones { get; set; } = string.Empty;

        public RegistroServicio RegistroServicio { get; set; } = null!;
        public Usuario Usuario { get; set; } = null!; // Mecánico
        public Servicio Servicio { get; set; } = null!;
    }
}
