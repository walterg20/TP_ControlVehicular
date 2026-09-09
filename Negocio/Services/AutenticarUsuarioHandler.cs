using AutoMapper;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class AutenticarUsuarioHandler
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;

        public AutenticarUsuarioHandler(IUsuarioRepository usuarioRepository, IMapper mapper)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
        }

        public async Task<AuthResponseDto> HandleAsync(string nombre, string contrasena)
        {
            try
            {
                var usuario = await _usuarioRepository.GetByNombreAsync(nombre);

                if (usuario == null)
                {
                    return new AuthResponseDto
                    {
                        Resultado = ResultadoAutenticacion.UsuarioNoEncontrado,
                        Mensaje = "El usuario ingresado no existe."
                    };
                }

                if (!usuario.Estado)
                {
                    return new AuthResponseDto
                    {
                        Resultado = ResultadoAutenticacion.UsuarioInactivo,
                        Mensaje = "El usuario se encuentra inactivo."
                    };
                }

                if (usuario.Contrasena != contrasena)
                {
                    return new AuthResponseDto
                    {
                        Resultado = ResultadoAutenticacion.ContrasenaIncorrecta,
                        Mensaje = "Contraseña incorrecta."
                    };
                }

                var usuarioDto = _mapper.Map<UsuarioDto>(usuario);
                return new AuthResponseDto
                {
                    Resultado = ResultadoAutenticacion.Exitoso,
                    Usuario = usuarioDto,
                    Mensaje = "Autenticación exitosa."
                };
            }
            catch (Exception)
            {
                return new AuthResponseDto
                {
                    Resultado = ResultadoAutenticacion.ErrorBaseDatos,
                    Mensaje = "Error de conexión con la base de datos."
                };
            }
        }
    }
}
