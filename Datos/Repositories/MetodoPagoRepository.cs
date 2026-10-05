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
        private readonly CVDbContext _context;

        public MetodoPagoRepository(CVDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MetodoPago>> GetAll()
        {
            return await _context.MetodosPago.ToListAsync();
        }
    }
}
