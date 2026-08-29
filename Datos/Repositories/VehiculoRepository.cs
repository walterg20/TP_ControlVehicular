using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;


namespace TP_ControlVehicular.Datos.Repositories
{
    public class VehiculoRepository : Repository<Vehiculo>, IVehiculoRepository
    {
        private readonly CVDbContext _context;

        public VehiculoRepository(CVDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Vehiculo>> GetByClienteAsync(int idCliente) =>
            await _context.Vehiculos
                .Include(v => v.Modelo)
                .ThenInclude(m => m.Marca)
                .Where(v => v.ClienteId == idCliente)
                .ToListAsync();
    }
}
