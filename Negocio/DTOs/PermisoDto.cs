namespace TP_ControlVehicular.Negocio.DTOs
{
    public class PermisoDto
    {
        public int IdPermiso { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Asignado { get; set; }
    }
}
