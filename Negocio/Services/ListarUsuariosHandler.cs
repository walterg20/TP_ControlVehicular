// walterg20
using AutoMapper;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    /// <summary>
    /// CASO DE USO: Consulta y Listado General de Usuarios.
    /// Recupera los usuarios registrados con su rol asociado para ser mostrados en la interfaz.
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
            var entities = await _usuarioRepository.GetWithRolAsync();
            return _mapper.Map<List<UsuarioDto>>(entities);
        }

        /// <summary>
        /// Recupera una pagina de usuarios con la busqueda y el filtro de estado aplicados en la base de datos.
        /// </summary>
        public async Task<(IEnumerable<UsuarioDto> Items, int TotalRegistros)> HandlePaginadoAsync(
            string? busqueda,
            string? filtroEstado,
            int pagina,
            int tamanoPagina)
        {
            var (entities, total) = await _usuarioRepository.GetPaginadoAsync(busqueda, filtroEstado, pagina, tamanoPagina);
            return (_mapper.Map<List<UsuarioDto>>(entities), total);
        }
    }
}
