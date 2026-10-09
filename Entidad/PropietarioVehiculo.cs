namespace TP_ControlVehicular.Entidad
{
    public class PropietarioVehiculo
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int VehiculoId { get; set; }
        public DateTime FechaAdquisicion { get; set; }
        public DateTime? FechaVenta { get; set; }
        public bool EsActual { get; set; }

        public Cliente Cliente { get; set; } = null!;
        public Vehiculo Vehiculo { get; set; } = null!;
    }
}
