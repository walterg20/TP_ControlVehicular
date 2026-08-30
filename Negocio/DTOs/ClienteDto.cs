
namespace TP_ControlVehicular.Negocio.DTOs
{
    public class ClienteDto
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public bool Activo { get; set; }
    }
}
