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
        private readonly CVDbContext _context;

        public PagoRepository(CVDbContext context)
        {
            _context = context;
        }

        public async Task Create(Pago pago)
        {
            await _context.Pagos.AddAsync(pago);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Pago>> GetPagosByFactura(int Id)
        {
            return await _context.Pagos
                .Include(p => p.MetodoPago)
                .Where(p => p.FacturaId == Id)
                .ToListAsync();
        }
    }
}
