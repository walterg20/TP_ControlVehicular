using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class EliminarModeloHandler
    {
        private readonly IModeloRepository _modeloRepository;

        public EliminarModeloHandler(IModeloRepository modeloRepository)
        {
            _modeloRepository = modeloRepository;
        }

        public async Task HandleAsync(int id)
        {
            await _modeloRepository.DeleteAsync(id);
        }
    }
}
