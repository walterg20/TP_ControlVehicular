// walterg20
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Datos.Repositories
{
    public class MetodoPagoRepository : IMetodoPagoRepository
    {
        private readonly IDbContextFactory<CVDbContext> _contextFactory;

        public MetodoPagoRepository(IDbContextFactory<CVDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<MetodoPago>> GetAll()
        {
            await using var ctx = _contextFactory.CreateDbContext();
            return await ctx.MetodosPago.ToListAsync();
        }
    }
}
