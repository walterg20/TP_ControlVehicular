using AutoMapper;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ListarRegistroServiciosHandler
    {
        private readonly IRegistroServicioRepository _repository;
        private readonly IMapper _mapper;

        public ListarRegistroServiciosHandler(IRegistroServicioRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RegistroServicioDto>> HandleAsync()
        {
            // Usamos GetReporteCompletoAsync para obtener las propiedades de navegación
            var registros = await _repository.GetReporteCompletoAsync();
            return _mapper.Map<IEnumerable<RegistroServicioDto>>(registros);
        }
    }
}
