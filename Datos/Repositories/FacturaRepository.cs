using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Datos.Repositories
{
    public class FacturaRepository : IFacturaRepository
    {
        private readonly CVDbContext _context;

        public FacturaRepository(CVDbContext context)
        {
            _context = context;
        }

        public async Task Create(Factura factura)
        {
            await _context.Facturas.AddAsync(factura);
            await _context.SaveChangesAsync();
        }

        public async Task<Factura?> GetById(int Id)
        {
            return await _context.Facturas
                .Include(f => f.Pagos)
                .Include(f => f.RegistroServicio)
                .FirstOrDefaultAsync(f => f.Id == Id);
        }

        public async Task<Factura?> GetByRegistroServicio(int RegistroServicioId)
        {
            return await _context.Facturas
                .Include(f => f.Pagos)
                .Include(f => f.RegistroServicio)
                .FirstOrDefaultAsync(f => f.RegistroServicioId == RegistroServicioId);
        }
    }
}
