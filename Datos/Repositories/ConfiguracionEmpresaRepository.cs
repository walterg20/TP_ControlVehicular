// walterg20
using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Datos.Repositories
{
    public class ConfiguracionEmpresaRepository : Repository<ConfiguracionEmpresa>, IConfiguracionEmpresaRepository
    {
        private readonly CVDbContext _cvDbContext;

        public ConfiguracionEmpresaRepository(CVDbContext cvDbContext) : base(cvDbContext)
        {
            _cvDbContext = cvDbContext;
        }

        public async Task<ConfiguracionEmpresa?> ObtenerAsync()
        {
            return await _cvDbContext.ConfiguracionEmpresa
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == 1);
        }
    }
}