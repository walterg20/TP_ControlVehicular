using AutoMapper;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class RegistrarRegistroServicioHandler
    {
        private readonly IRegistroServicioRepository _repository;
        private readonly IMapper _mapper;

        public RegistrarRegistroServicioHandler(IRegistroServicioRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<RegistroServicioDto> HandleAsync(RegistroServicio registro)
        {
            await _repository.AddAsync(registro);
            // Recargar con relaciones para el mapeo completo al DTO
            var list = await _repository.GetReporteCompletoAsync();
            var added = list.FirstOrDefault(r => r.Id == registro.Id) ?? registro;
            
            return _mapper.Map<RegistroServicioDto>(added);
        }
    }
}
