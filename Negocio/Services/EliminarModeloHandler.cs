// walterg20
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
            var modelo = await _modeloRepository.GetByIdAsync(id);
            if (modelo == null) return;
            modelo.Activo = false;
            await _modeloRepository.UpdateAsync(modelo);
        }

        public async Task ReactivarAsync(int id)
        {
            var modelo = await _modeloRepository.GetByIdAsync(id);
            if (modelo == null) return;
            modelo.Activo = true;
            await _modeloRepository.UpdateAsync(modelo);
        }
    }
}
