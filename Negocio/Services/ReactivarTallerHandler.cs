// walterg20
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ReactivarTallerHandler
    {
        private readonly ITallerRepository _tallerRepository;

        public ReactivarTallerHandler(ITallerRepository tallerRepository)
        {
            _tallerRepository = tallerRepository;
        }

        public async Task HandleAsync(int id)
        {
            var entity = await _tallerRepository.GetByIdAsync(id);
            if (entity != null)
            {
                entity.Activo = true;
                await _tallerRepository.UpdateAsync(entity);
            }
        }
    }
}