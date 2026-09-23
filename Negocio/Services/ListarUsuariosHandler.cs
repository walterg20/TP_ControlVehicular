using AutoMapper;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    /// <summary>
    /// CASO DE USO: Consulta y Listado General de Usuarios.
    /// Recupera la totalidad de los usuarios registrados (activos e inactivos) con su rol asociado para ser mostrados en la interfaz.
    /// </summary>
    public class ListarUsuariosHandler
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;

        public ListarUsuariosHandler(IUsuarioRepository usuarioRepository, IMapper mapper)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene la lista completa de usuarios desde el repositorio y los transforma a DTOs.
        /// </summary>
        /// <returns>Lista de UsuarioDto para el DataGrid del módulo de usuarios.</returns>
        public async Task<List<UsuarioDto>> HandleAsync()
        {
            // 1. Consulta el repositorio pidiendo todos los usuarios e incluyendo su Rol (GetWithRolAsync)
            var entities = await _usuarioRepository.GetWithRolAsync();

            // 2. Mapea la colección de entidades a la colección de DTOs
            return _mapper.Map<List<UsuarioDto>>(entities);
        }
    }
}
