using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class EliminarServicioHandler
    {
        private readonly IServicioRepository _servicioRepository;

        public EliminarServicioHandler(IServicioRepository servicioRepository)
        {
            _servicioRepository = servicioRepository;
        }

        public async Task HandleAsync(int id)
        {
            var entity = await _servicioRepository.GetByIdAsync(id);
            if (entity != null)
            {
                entity.Activo = false;
                await _servicioRepository.UpdateAsync(entity);
            }
        }
    }
}
