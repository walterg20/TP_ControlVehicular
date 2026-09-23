using AutoMapper;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    /// <summary>
    /// CASO DE USO: Autenticación de Usuarios (Login).
    /// Esta clase encapsula la lógica de negocio necesaria para verificar las credenciales de acceso de un usuario en el sistema.
    /// </summary>
    public class AutenticarUsuarioHandler
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor con inyección de dependencias para el Repositorio de Usuarios y el perfil de mapeo AutoMapper.
        /// </summary>
        public AutenticarUsuarioHandler(IUsuarioRepository usuarioRepository, IMapper mapper)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Ejecuta la autenticación verificando el DNI, estado activo del usuario y coincidencia de contraseña.
        /// </summary>
        /// <param name="dni">DNI ingresado en el login.</param>
        /// <param name="contrasena">Contraseña ingresada por el usuario.</param>
        /// <returns>Objeto AuthResponseDto con el resultado de la autenticación, mensajes explicativos y el UsuarioDto si fue exitoso.</returns>
        public async Task<AuthResponseDto> HandleAsync(string dni, string contrasena)
        {
            try
            {
                // 1. Consulta la base de datos por DNI e incluye la relación con Rol (.Include(u => u.Rol))
                var usuario = await _usuarioRepository.GetByDniAsync(dni);

                // 2. Validación de existencia del usuario
                if (usuario == null)
                {
                    return new AuthResponseDto
                    {
                        Resultado = ResultadoAutenticacion.UsuarioNoEncontrado,
                        Mensaje = "El DNI ingresado no se encuentra registrado."
                    };
                }

                // 3. Validación de Baja Lógica / Estado inactivo
                if (!usuario.Estado)
                {
                    return new AuthResponseDto
                    {
                        Resultado = ResultadoAutenticacion.UsuarioInactivo,
                        Mensaje = "El usuario se encuentra inactivo."
                    };
                }

                // 4. Verificación de la contraseña ingresada contra la almacenada
                if (usuario.Contrasena != contrasena)
                {
                    return new AuthResponseDto
                    {
                        Resultado = ResultadoAutenticacion.ContrasenaIncorrecta,
                        Mensaje = "Contraseña incorrecta."
                    };
                }

                // 5. Autenticación Exitosa: Mapeo de Entidad Usuario a UsuarioDto vía AutoMapper
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
                // Captura errores de red o fallo de conexión con el motor SQL Server
                return new AuthResponseDto
                {
                    Resultado = ResultadoAutenticacion.ErrorBaseDatos,
                    Mensaje = "Error de conexión con la base de datos."
                };
            }
        }
    }
}
