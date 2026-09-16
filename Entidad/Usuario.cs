namespace TP_ControlVehicular.Entidad;

public class Usuario
{
    public int IdUsuario { get; set; }
    public int IdRol { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
    public bool Estado { get; set; }
    public Rol Rol { get; set; } = null!;
}
