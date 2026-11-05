// walterg20
using AutoMapper;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ListarVehiculosHandler
    {
        private readonly IVehiculoRepository _vehiculoRepository;
        private readonly IMapper _mapper;

        public ListarVehiculosHandler(IVehiculoRepository vehiculoRepository, IMapper mapper)
        {
            _vehiculoRepository = vehiculoRepository;
            _mapper = mapper;
        }

        public async Task<List<VehiculoDto>> HandleAsync()
        {
            var entities = await _vehiculoRepository.GetAllWithDetailsAsync();
            return _mapper.Map<List<VehiculoDto>>(entities);
        }

        /// <summary>
        /// Recupera una pagina de vehiculos con la busqueda aplicada en la base de datos.
        /// </summary>
        public async Task<(IEnumerable<VehiculoDto> Items, int TotalRegistros)> HandlePaginadoAsync(
            string? busqueda,
            string? filtroEstado,
            int pagina,
            int tamanoPagina)
        {
            var (entities, total) = await _vehiculoRepository.GetPaginadoAsync(busqueda, filtroEstado, pagina, tamanoPagina);
            return (_mapper.Map<List<VehiculoDto>>(entities), total);
        }
    }
}
