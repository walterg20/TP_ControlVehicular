// walterg20
using AutoMapper;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ListarServiciosHandler
    {
        private readonly IServicioRepository _servicioRepository;
        private readonly IMapper _mapper;

        public ListarServiciosHandler(IServicioRepository servicioRepository, IMapper mapper)
        {
            _servicioRepository = servicioRepository;
            _mapper = mapper;
        }

        public async Task<List<ServicioDto>> HandleAsync(bool incluirInactivos = false)
        {
            var entities = incluirInactivos
                ? await _servicioRepository.GetTodosAsync()
                : await _servicioRepository.GetActivosAsync();
            return _mapper.Map<List<ServicioDto>>(entities);
        }
    }
}
