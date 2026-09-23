using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;


namespace TP_ControlVehicular.Datos.Repositories
{
    public class ClienteRepository: Repository<Cliente>, IClienteRepository
    {
        private readonly CVDbContext _cvDbContext;

        public ClienteRepository(CVDbContext cvDbContext) : base(cvDbContext)
        {
            _cvDbContext = cvDbContext;
        }

        public async Task<IEnumerable<Cliente>> GetActivosAsync() =>
            await _cvDbContext.Clientes.Where(c => c.Activo).ToListAsync();
    }
}
