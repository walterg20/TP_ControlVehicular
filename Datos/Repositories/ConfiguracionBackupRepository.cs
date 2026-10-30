// walterg20
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Datos.Repositories
{
    public class ConfiguracionBackupRepository : Repository<ConfiguracionBackup>, IConfiguracionBackupRepository
    {
        private readonly IDbContextFactory<CVDbContext> _contextFactory;

        public ConfiguracionBackupRepository(IDbContextFactory<CVDbContext> contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<ConfiguracionBackup?> ObtenerAsync()
        {
            await using var ctx = CrearContexto();
            return await ctx.ConfiguracionBackup
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == 1);
        }
    }
}
