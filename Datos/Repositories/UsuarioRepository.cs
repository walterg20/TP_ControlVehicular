// walterg20
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Datos.Repositories
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        private readonly IDbContextFactory<CVDbContext> _contextFactory;

        public UsuarioRepository(IDbContextFactory<CVDbContext> contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Usuario>> GetActivosAsync()
        {
            await using var ctx = CrearContexto();
            return await ctx.Usuarios.AsNoTracking()
                .Include(u => u.Rol).ThenInclude(r => r.Permisos)
                .Where(u => u.Activo)
                .ToListAsync();
        }

        public async Task<Usuario?> GetByNombreAsync(string nombre)
        {
            await using var ctx = CrearContexto();
            return await ctx.Usuarios.AsNoTracking()
                .Include(u => u.Rol).ThenInclude(r => r.Permisos)
                .FirstOrDefaultAsync(u => u.Nombre == nombre);
        }

        public async Task<Usuario?> GetByDniAsync(string dni)
        {
            await using var ctx = CrearContexto();
            return await ctx.Usuarios.AsNoTracking()
                .Include(u => u.Rol).ThenInclude(r => r.Permisos)
                .FirstOrDefaultAsync(u => u.Dni == dni);
        }

        public async Task<List<Usuario>> GetWithRolAsync()
        {
            await using var ctx = CrearContexto();
            return await ctx.Usuarios.AsNoTracking()
                .Include(u => u.Rol).ThenInclude(r => r.Permisos)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Usuario> Items, int TotalRegistros)> GetPaginadoAsync(
            string? busqueda,
            int pagina,
            int tamanoPagina)
        {
            Expression<Func<Usuario, bool>>? filtro = null;

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var patron = CrearPatronLike(busqueda.Trim());
                filtro = u => EF.Functions.Like(u.Nombre, patron)
                          || EF.Functions.Like(u.Apellido, patron)
                          || EF.Functions.Like(u.Dni, patron)
                          || EF.Functions.Like(u.Rol.Nombre, patron);
            }

            return await GetPaginadoAsync(
                pagina,
                tamanoPagina,
                filtro,
                q => q.Include(u => u.Rol).ThenInclude(r => r.Permisos).AsSplitQuery(),
                q => q.OrderBy(u => u.Id));
        }
    }
}
