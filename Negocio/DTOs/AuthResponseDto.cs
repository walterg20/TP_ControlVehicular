namespace TP_ControlVehicular.Negocio.DTOs
{
    public class AuthResponseDto
    {
        public ResultadoAutenticacion Resultado { get; set; }
        public UsuarioDto? Usuario { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }
}
