using TP_ControlVehicular.Negocio.DTOs;

namespace TP_ControlVehicular.Negocio.Context
{
    public static class UserSession
    {
        public static UsuarioDto? CurrentUser { get; set; }
    }
}
