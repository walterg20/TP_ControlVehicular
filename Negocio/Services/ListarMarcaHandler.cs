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
    }
}
