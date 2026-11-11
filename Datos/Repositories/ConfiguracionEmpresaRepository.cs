// walterg20
using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Datos.Repositories
{
    public class ConfiguracionEmpresaRepository : Repository<ConfiguracionEmpresa>, IConfiguracionEmpresaRepository
    {
        private readonly IDbContextFactory<CVDbContext> _contextFactory;

        public ConfiguracionEmpresaRepository(IDbContextFactory<CVDbContext> contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<ConfiguracionEmpresa?> ObtenerAsync()
        {
            await using var ctx = CrearContexto();
            return await ctx.ConfiguracionEmpresa
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == 1);
        }
    }
}
