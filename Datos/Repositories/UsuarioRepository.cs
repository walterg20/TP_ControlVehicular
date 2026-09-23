using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Datos.Repositories
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        private readonly CVDbContext _cvDbContext;

        public UsuarioRepository(CVDbContext cvDbContext) : base(cvDbContext)
        {
            _cvDbContext = cvDbContext;
        }

        public async Task<List<Usuario>> GetActivosAsync() =>
            await _cvDbContext.Usuarios.AsNoTracking()
                .Include(u => u.Rol)
                .Where(u => u.Estado)
                .ToListAsync();

        public async Task<Usuario?> GetByNombreAsync(string nombre) =>
            await _cvDbContext.Usuarios.AsNoTracking()
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Nombre == nombre);

        public async Task<Usuario?> GetByDniAsync(string dni) =>
            await _cvDbContext.Usuarios.AsNoTracking()
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Dni == dni);

        public async Task<List<Usuario>> GetWithRolAsync() =>
            await _cvDbContext.Usuarios.AsNoTracking()
                .Include(u => u.Rol)
                .ToListAsync();
    }
}
