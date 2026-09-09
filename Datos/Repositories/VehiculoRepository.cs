using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Datos.Repositories
{
    public class VehiculoRepository : Repository<Vehiculo>, IVehiculoRepository
    {
        private readonly CVDbContext _cvDbContext;

        public VehiculoRepository(CVDbContext cvDbContext) : base(cvDbContext)
        {
            _cvDbContext = cvDbContext;
        }

        public async Task<IEnumerable<Vehiculo>> GetByClienteAsync(int idCliente) =>
            await _cvDbContext.Vehiculos
                .Include(v => v.Cliente)
                .Include(v => v.Modelo)
                .ThenInclude(m => m.Marca)
                .Where(v => v.ClienteId == idCliente)
                .ToListAsync();

        public async Task<IEnumerable<Vehiculo>> GetAllWithDetailsAsync() =>
            await _cvDbContext.Vehiculos
                .Include(v => v.Cliente)
                .Include(v => v.Modelo)
                .ThenInclude(m => m.Marca)
                .ToListAsync();
    }
}
