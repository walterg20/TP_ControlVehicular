using AutoMapper;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ModificarRegistroServicioHandler
    {
        private readonly IRegistroServicioRepository _repository;
        private readonly IMapper _mapper;

        public ModificarRegistroServicioHandler(IRegistroServicioRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<RegistroServicioDto> HandleAsync(RegistroServicio registro)
        {
            await _repository.UpdateAsync(registro);
            
            // Recargar con relaciones
            var list = await _repository.GetReporteCompletoAsync();
            var updated = list.FirstOrDefault(r => r.Id == registro.Id) ?? registro;
            
            return _mapper.Map<RegistroServicioDto>(updated);
        }
    }
}
