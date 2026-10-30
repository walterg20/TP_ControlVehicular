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
    public class ClienteRepository : Repository<Cliente>, IClienteRepository
    {
        private readonly IDbContextFactory<CVDbContext> _contextFactory;

        public ClienteRepository(IDbContextFactory<CVDbContext> contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<Cliente>> GetActivosAsync()
        {
            await using var ctx = CrearContexto();
            return await ctx.Clientes.Where(c => c.Activo).ToListAsync();
        }

        public async Task<(IEnumerable<Cliente> Items, int TotalRegistros)> GetPaginadoAsync(
            string? busqueda,
            bool incluirInactivos,
            int pagina,
            int tamanoPagina)
        {
            Expression<Func<Cliente, bool>>? filtro = null;

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var patron = CrearPatronLike(busqueda.Trim());
                filtro = c => EF.Functions.Like(c.Nombre, patron)
                          || EF.Functions.Like(c.Apellido, patron)
                          || EF.Functions.Like(c.Dni, patron);
            }

            Expression<Func<Cliente, bool>>? soloActivos = null;
            if (!incluirInactivos)
            {
                soloActivos = c => c.Activo;
            }

            return await GetPaginadoAsync(
                pagina,
                tamanoPagina,
                filtro,
                q => soloActivos is null ? q : q.Where(soloActivos),
                q => q.OrderBy(c => c.Id));
        }
    }
}
