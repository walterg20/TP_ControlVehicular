
namespace TP_ControlVehicular.Entidad
{
    public class Modelo
    {
        public int Id { get; set; }
        public int MarcaId { get; set; }
        public string NombreModelo { get; set; } = string.Empty;

        public Marca Marca { get; set; } = null!;
        public ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
    }
}
