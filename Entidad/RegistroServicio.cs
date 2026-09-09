namespace TP_ControlVehicular.Entidad
{
    public class RegistroServicio
    {
        public int Id { get; set; }
        public int VehiculoId { get; set; }
        public int TallerId { get; set; }
        public int UsuarioId { get; set; } // Recepcionista que recibe el vehículo
        public DateTime Fecha { get; set; } = DateTime.Now;
        public int KmIngreso { get; set; }
        public string Estado { get; set; } = "En Proceso";

        public Vehiculo Vehiculo { get; set; } = null!;
        public Taller Taller { get; set; } = null!;
        public Usuario Usuario { get; set; } = null!; // Recepcionista
        public ICollection<DetalleServicio> Detalles { get; set; } = new List<DetalleServicio>();
    }
}
