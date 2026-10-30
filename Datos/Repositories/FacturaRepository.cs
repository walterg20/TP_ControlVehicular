// walterg20
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Datos.Repositories
{
    public class FacturaRepository : IFacturaRepository
    {
        private readonly IDbContextFactory<CVDbContext> _contextFactory;

        public FacturaRepository(IDbContextFactory<CVDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task Create(Factura factura)
        {
            await using var ctx = _contextFactory.CreateDbContext();
            await ctx.Facturas.AddAsync(factura);
            await ctx.SaveChangesAsync();
        }

        public async Task<Factura?> GetById(int Id)
        {
            await using var ctx = _contextFactory.CreateDbContext();
            return await ctx.Facturas
                .Include(f => f.Pagos)
                .Include(f => f.RegistroServicio)
                .FirstOrDefaultAsync(f => f.Id == Id);
        }

        public async Task<Factura?> GetByRegistroServicio(int RegistroServicioId)
        {
            await using var ctx = _contextFactory.CreateDbContext();
            return await ctx.Facturas
                .Include(f => f.Pagos)
                .Include(f => f.RegistroServicio)
                .FirstOrDefaultAsync(f => f.RegistroServicioId == RegistroServicioId);
        }
    }
}
