namespace TP_ControlVehicular.Entidad
{
    public class Marca
    {
        public int Id { get; set; }
        public string NombreMarca { get; set; } = string.Empty;

        public ICollection<Modelo> Modelos { get; set; } = new List<Modelo>();
    }
}
