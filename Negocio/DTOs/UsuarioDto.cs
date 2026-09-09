namespace TP_ControlVehicular.Negocio.DTOs
{
    public class UsuarioDto
    {
        public int IdUsuario { get; set; }
        public int IdRol { get; set; }
        public string RolNombre { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public bool Estado { get; set; }
    }
}
