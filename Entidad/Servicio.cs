namespace TP_ControlVehicular.Entidad
{
    public class Servicio
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public bool Activo { get; set; } = true;

        public ICollection<DetalleServicio> Detalles { get; set; } = new List<DetalleServicio>();
    }
}
