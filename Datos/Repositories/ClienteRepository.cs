using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;


namespace TP_ControlVehicular.Datos.Repositories
{
    public class ClienteRepository: Repository<Cliente>, IClienteRepository
    {
        private readonly CVDbContext _context;

        public ClienteRepository(CVDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cliente>> GetActivosAsync() =>
            await _context.Clientes.Where(c => c.Activo).ToListAsync();
    }
}
