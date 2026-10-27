// walterg20
namespace TP_ControlVehicular.Negocio.DTOs
{
    public class RolDto
    {
        public int IdRol { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public bool Estado { get; set; }
        public System.Collections.Generic.List<string> Permisos { get; set; } = new();
        public override string ToString() => Nombre;
    }
}
