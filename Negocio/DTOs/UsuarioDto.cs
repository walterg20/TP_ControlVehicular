namespace TP_ControlVehicular.Negocio.DTOs
{
    public class UsuarioDto
    {
        public int IdUsuario { get; set; }
        public int IdRol { get; set; }
        public string RolNombre { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Domicilio { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string Contrasena { get; set; } = string.Empty;
        public bool Estado { get; set; }
    }
}
