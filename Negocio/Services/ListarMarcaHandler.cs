// walterg20
using AutoMapper;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ListarMarcaHandler
    {
        private readonly IMarcaRepository _marcaRepository;
        private readonly IMapper _mapper;

        public ListarMarcaHandler(IMarcaRepository marcaRepository, IMapper mapper)
        {
            _marcaRepository = marcaRepository;
            _mapper = mapper;
        }

        public async Task<List<MarcaDto>> HandleAsync()
        {
            var entities = await _marcaRepository.GetActivosAsync();
            return _mapper.Map<List<MarcaDto>>(entities);
        }

        /// <summary>
        /// Recupera una pagina de marcas con la busqueda y el filtro de estado aplicados en la base de datos.
        /// </summary>
        public async Task<(IEnumerable<MarcaDto> Items, int TotalRegistros)> HandlePaginadoAsync(
            string? busqueda,
            string? filtroEstado,
            int pagina,
            int tamanoPagina)
        {
            var (entities, total) = await _marcaRepository.GetPaginadoAsync(busqueda, filtroEstado, pagina, tamanoPagina);
            return (_mapper.Map<List<MarcaDto>>(entities), total);
        }
    }
}
