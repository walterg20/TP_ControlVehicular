using AutoMapper;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ListarModelosHandler
    {
        private readonly IModeloRepository _modeloRepository;
        private readonly IMapper _mapper;

        public ListarModelosHandler(IModeloRepository modeloRepository, IMapper mapper)
        {
            _modeloRepository = modeloRepository;
            _mapper = mapper;
        }

        public async Task<List<ModeloDto>> HandleAsync()
        {
            var entities = await _modeloRepository.GetAllAsync();
            return _mapper.Map<List<ModeloDto>>(entities);
        }
    }
}
