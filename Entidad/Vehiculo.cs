
namespace TP_ControlVehicular.Entidad
{
    public class Vehiculo
    {
        public int Id{ get; set; }
        public int ClienteId { get; set; }
        public int ModeloId { get; set; }
        public int Anio { get; set; }
        public string Patente { get; set; } = string.Empty;
        public int KmActual { get; set; }

        public Cliente Cliente { get; set; } = null!;
        public Modelo Modelo { get; set; } = null!;
    }
}
