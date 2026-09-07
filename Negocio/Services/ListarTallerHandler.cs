using AutoMapper;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ListarTallerHandler
    {
        private readonly ITallerRepository _tallerRepository;
        private readonly IMapper _mapper;

        public ListarTallerHandler(ITallerRepository tallerRepository, IMapper mapper)
        {
            _tallerRepository = tallerRepository;
            _mapper = mapper;
        }

        public async Task<List<TallerDto>> HandleAsync()
        {
            var entities = await _tallerRepository.GetActivosAsync();
            return _mapper.Map<List<TallerDto>>(entities);
        }
    }
}
