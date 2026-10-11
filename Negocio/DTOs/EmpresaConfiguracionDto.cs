namespace TP_ControlVehicular.Negocio.DTOs
{
    public class EmpresaConfiguracionDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string? Email { get; set; }
        public bool TieneLogo { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}