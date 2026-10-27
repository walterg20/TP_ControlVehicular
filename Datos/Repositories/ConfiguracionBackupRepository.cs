// walterg20
using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Datos.Repositories
{
    public class ConfiguracionBackupRepository : Repository<ConfiguracionBackup>, IConfiguracionBackupRepository
    {
        private readonly CVDbContext _cvDbContext;

        public ConfiguracionBackupRepository(CVDbContext cvDbContext) : base(cvDbContext)
        {
            _cvDbContext = cvDbContext;
        }

        public async Task<ConfiguracionBackup?> ObtenerAsync()
        {
            return await _cvDbContext.ConfiguracionBackup
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == 1);
        }
    }
}
