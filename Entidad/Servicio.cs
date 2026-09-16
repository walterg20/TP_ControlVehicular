namespace TP_ControlVehicular.Entidad
{
    public class Servicio
    {
        public int IdServicio { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public bool Activo { get; set; }
    }
}
