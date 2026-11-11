// walterg20
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
            var vehiculo = await _vehiculoRepository.GetByIdAsync(id);
            if (vehiculo == null) return;
            vehiculo.Activo = false;
            await _vehiculoRepository.UpdateAsync(vehiculo);
        }

        public async Task ReactivarAsync(int id)
        {
            var vehiculo = await _vehiculoRepository.GetByIdAsync(id);
            if (vehiculo == null) return;
            vehiculo.Activo = true;
            await _vehiculoRepository.UpdateAsync(vehiculo);
        }
    }
}
