using AutoMapper;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class RegistrarServicioHandler
    {
        private readonly IServicioRepository _servicioRepository;
        private readonly IMapper _mapper;

        public RegistrarServicioHandler(IServicioRepository servicioRepository, IMapper mapper)
        {
            _servicioRepository = servicioRepository;
            _mapper = mapper;
        }

        public async Task<ServicioDto> HandleAsync(Servicio servicio)
        {
            if (servicio.IdServicio > 0)
                await _servicioRepository.UpdateAsync(servicio);
            else
                await _servicioRepository.AddAsync(servicio);

            return _mapper.Map<ServicioDto>(servicio);
        }
    }
}
