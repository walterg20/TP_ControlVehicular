using AutoMapper;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ModificarModeloHandler
    {
        private readonly IModeloRepository _modeloRepository;
        private readonly IMapper _mapper;

        public ModificarModeloHandler(IModeloRepository modeloRepository, IMapper mapper)
        {
            _modeloRepository = modeloRepository;
            _mapper = mapper;
        }

        public async Task<ModeloDto> HandleAsync(Modelo entity)
        {
            await _modeloRepository.UpdateAsync(entity);
            var reloaded = await _modeloRepository.GetByIdAsync(entity.Id);
            return _mapper.Map<ModeloDto>(reloaded ?? entity);
        }
    }
}
