using AutoMapper;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    /// <summary>
    /// CASO DE USO: Modificación y Cambio de Estado de Usuario.
    /// Encapsula las operaciones de actualización de datos personales, rol o estado activo/inactivo (baja lógica).
    /// </summary>
    public class ModificarUsuarioHandler
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;

        public ModificarUsuarioHandler(IUsuarioRepository usuarioRepository, IMapper mapper)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Actualiza un usuario existente en la base de datos y retorna el DTO actualizado.
        /// </summary>
        /// <param name="usuario">Entidad Usuario con las modificaciones aplicadas.</param>
        /// <returns>UsuarioDto actualizado con los datos procesados.</returns>
        public async Task<UsuarioDto> HandleAsync(Usuario usuario)
        {
            // 1. Actualiza el registro en la base de datos usando EF Core
            await _usuarioRepository.UpdateAsync(usuario);

            // 2. Recarga el objeto para asegurar las relaciones (Include Rol)
            var reloaded = await _usuarioRepository.GetByNombreAsync(usuario.Nombre) ?? usuario;

            // 3. Mapea la entidad actualizada a DTO
            return _mapper.Map<UsuarioDto>(reloaded);
        }
    }
}
