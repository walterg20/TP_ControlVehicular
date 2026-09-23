namespace TP_ControlVehicular.Entidad
{
    public class Rol
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool Estado { get; set; } = true;

        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
