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
            var entities = await _marcaRepository.GetAllAsync();
            return _mapper.Map<List<MarcaDto>>(entities);
        }

        /// <summary>
        /// Recupera una pagina de marcas con la busqueda aplicada en la base de datos.
        /// </summary>
        public async Task<(IEnumerable<MarcaDto> Items, int TotalRegistros)> HandlePaginadoAsync(
            string? busqueda,
            int pagina,
            int tamanoPagina)
        {
            var (entities, total) = await _marcaRepository.GetPaginadoAsync(busqueda, pagina, tamanoPagina);
            return (_mapper.Map<List<MarcaDto>>(entities), total);
        }
    }
}
