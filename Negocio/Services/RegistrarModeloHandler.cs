using AutoMapper;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class RegistrarModeloHandler
    {
        private readonly IModeloRepository _modeloRepository;
        private readonly IMapper _mapper;

        public RegistrarModeloHandler(IModeloRepository modeloRepository, IMapper mapper)
        {
            _modeloRepository = modeloRepository;
            _mapper = mapper;
        }

        public async Task<ModeloDto> HandleAsync(Modelo entity)
        {
            await _modeloRepository.AddAsync(entity);
            var reloaded = await _modeloRepository.GetByIdAsync(entity.Id);
            return _mapper.Map<ModeloDto>(reloaded ?? entity);
        }
    }
}
