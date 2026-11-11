// walterg20
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class EliminarMarcaHandler
    {
        private readonly IMarcaRepository _marcaRepository;

        public EliminarMarcaHandler(IMarcaRepository marcaRepository)
        {
            _marcaRepository = marcaRepository;
        }

        public async Task HandleAsync(int id)
        {
            var marca = await _marcaRepository.GetByIdAsync(id);
            if (marca == null) return;
            marca.Activo = false;
            await _marcaRepository.UpdateAsync(marca);
        }

        public async Task ReactivarAsync(int id)
        {
            var marca = await _marcaRepository.GetByIdAsync(id);
            if (marca == null) return;
            marca.Activo = true;
            await _marcaRepository.UpdateAsync(marca);
        }
    }
}
