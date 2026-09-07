using AutoMapper;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ModificarTallerHandler
    {
        private readonly ITallerRepository _tallerRepository;
        private readonly IMapper _mapper;

        public ModificarTallerHandler(ITallerRepository tallerRepository, IMapper mapper)
        {
            _tallerRepository = tallerRepository;
            _mapper = mapper;
        }

        public async Task<TallerDto> HandleAsync(Taller taller)
        {
            await _tallerRepository.UpdateAsync(taller);
            return _mapper.Map<TallerDto>(taller);
        }
    }
}