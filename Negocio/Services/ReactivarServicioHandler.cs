// walterg20
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ReactivarServicioHandler
    {
        private readonly IServicioRepository _servicioRepository;

        public ReactivarServicioHandler(IServicioRepository servicioRepository)
        {
            _servicioRepository = servicioRepository;
        }

        public async Task HandleAsync(int id)
        {
            var entity = await _servicioRepository.GetByIdAsync(id);
            if (entity != null)
            {
                entity.Activo = true;
                await _servicioRepository.UpdateAsync(entity);
            }
        }
    }
}