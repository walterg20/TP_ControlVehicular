using AutoMapper;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    /// <summary>
    /// CASO DE USO: Registro de un Nuevo Usuario en el Sistema (Alta de Usuario).
    /// </summary>
    public class RegistrarUsuarioHandler
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;

        public RegistrarUsuarioHandler(IUsuarioRepository usuarioRepository, IMapper mapper)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Ejecuta el alta de la entidad Usuario en la base de datos y retorna el DTO mapeado.
        /// </summary>
        /// <param name="usuario">Entidad Usuario con los datos cargados desde el formulario.</param>
        /// <returns>UsuarioDto con el registro completo guardado e información de su Rol asociado.</returns>
        public async Task<UsuarioDto> HandleAsync(Usuario usuario)
        {
            // 1. Persistir la nueva entidad en la base de datos mediante EF Core
            await _usuarioRepository.AddAsync(usuario);

            // 2. Recargar la entidad cargando las propiedades de navegación (Rol)
            var reloaded = await _usuarioRepository.GetByNombreAsync(usuario.Nombre) ?? usuario;

            // 3. Mapear de Entidad Usuario a UsuarioDto
            return _mapper.Map<UsuarioDto>(reloaded);
        }
    }
}
