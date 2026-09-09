namespace TP_ControlVehicular.Entidad
{
    public class Usuario
    {
        public int Id { get; set; }
        public int RolId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public bool Estado { get; set; } = true;

        public Rol Rol { get; set; } = null!;
    }
}
