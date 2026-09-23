using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class EliminarVehiculoHandler
    {
        private readonly IVehiculoRepository _vehiculoRepository;

        public EliminarVehiculoHandler(IVehiculoRepository vehiculoRepository)
        {
            _vehiculoRepository = vehiculoRepository;
        }

        public async Task HandleAsync(int id)
        {
            await _vehiculoRepository.DeleteAsync(id);
        }
    }
}
