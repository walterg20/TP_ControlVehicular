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
            await _marcaRepository.DeleteAsync(id);
        }
    }
}
