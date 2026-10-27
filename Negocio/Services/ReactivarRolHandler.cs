// walterg20
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ReactivarRolHandler
    {
        private readonly IRolRepository _rolRepository;

        public ReactivarRolHandler(IRolRepository rolRepository)
        {
            _rolRepository = rolRepository;
        }

        public async Task HandleAsync(int id)
        {
            var entity = await _rolRepository.GetByIdAsync(id);
            if (entity != null)
            {
                entity.Activo = true;
                await _rolRepository.UpdateAsync(entity);
            }
        }
    }
}