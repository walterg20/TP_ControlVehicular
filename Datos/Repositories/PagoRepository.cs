// walterg20
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Datos.Repositories
{
    public class PagoRepository : IPagoRepository
    {
        private readonly IDbContextFactory<CVDbContext> _contextFactory;

        public PagoRepository(IDbContextFactory<CVDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task Create(Pago pago)
        {
            await using var ctx = _contextFactory.CreateDbContext();
            await ctx.Pagos.AddAsync(pago);
            await ctx.SaveChangesAsync();
        }

        public async Task<IEnumerable<Pago>> GetPagosByFactura(int Id)
        {
            await using var ctx = _contextFactory.CreateDbContext();
            return await ctx.Pagos
                .Include(p => p.MetodoPago)
                .Where(p => p.FacturaId == Id)
                .ToListAsync();
        }
    }
}
