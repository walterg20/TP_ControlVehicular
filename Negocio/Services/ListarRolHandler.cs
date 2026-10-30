// walterg20
using AutoMapper;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ListarRolHandler
    {
        private readonly IRolRepository _rolRepository;
        private readonly IMapper _mapper;

        public ListarRolHandler(IRolRepository rolRepository, IMapper mapper)
        {
            _rolRepository = rolRepository;
            _mapper = mapper;
        }

        public async Task<List<RolDto>> HandleAsync(bool incluirInactivos = false)
        {
            var entities = incluirInactivos
                ? await _rolRepository.GetTodosAsync()
                : await _rolRepository.GetActivosAsync();
            return _mapper.Map<List<RolDto>>(entities);
        }

        /// <summary>
        /// Recupera una pagina de roles con la busqueda aplicada en la base de datos.
        /// </summary>
        public async Task<(IEnumerable<RolDto> Items, int TotalRegistros)> HandlePaginadoAsync(
            string? busqueda,
            bool incluirInactivos,
            int pagina,
            int tamanoPagina)
        {
            var (entities, total) = await _rolRepository.GetPaginadoAsync(busqueda, incluirInactivos, pagina, tamanoPagina);
            return (_mapper.Map<List<RolDto>>(entities), total);
        }
    }
}
