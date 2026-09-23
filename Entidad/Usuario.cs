namespace TP_ControlVehicular.Entidad
{
    public class Usuario
    {
        public int Id { get; set; }
        public int RolId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Domicilio { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string Contrasena { get; set; } = string.Empty;
        public bool Estado { get; set; } = true;

        public Rol Rol { get; set; } = null!;
    }
}
