using AutoMapper;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ModificarServicioHandler
    {
        private readonly IServicioRepository _servicioRepository;
        private readonly IMapper _mapper;

        public ModificarServicioHandler(IServicioRepository servicioRepository, IMapper mapper)
        {
            _servicioRepository = servicioRepository;
            _mapper = mapper;
        }

        public async Task<ServicioDto> HandleAsync(Servicio servicio)
        {
            await _servicioRepository.UpdateAsync(servicio);
            var reloaded = await _servicioRepository.GetByIdAsync(servicio.Id) ?? servicio;
            return _mapper.Map<ServicioDto>(reloaded);
        }
    }
}
