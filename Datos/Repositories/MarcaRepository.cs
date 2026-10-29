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
    public class MarcaRepository : Repository<Marca>, IMarcaRepository
    {
        private readonly CVDbContext _cvDbContext;

        public MarcaRepository(CVDbContext cvDbContext) : base(cvDbContext)
        {
            _cvDbContext = cvDbContext;
        }

        public async Task<Marca?> GetByNombreAsync(string nombre)
        {
            return await _cvDbContext.Marcas
                .Include(m => m.Modelos)
                .FirstOrDefaultAsync(m => m.NombreMarca == nombre);
        }

        public async Task<(IEnumerable<Marca> Items, int TotalRegistros)> GetPaginadoAsync(
            string? busqueda,
            int pagina,
            int tamanoPagina)
        {
            Expression<Func<Marca, bool>>? filtro = null;

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var patron = CrearPatronLike(busqueda.Trim());
                filtro = m => EF.Functions.Like(m.NombreMarca, patron);
            }

            return await GetPaginadoAsync(pagina, tamanoPagina, filtro, null, q => q.OrderBy(m => m.Id));
        }
    }
}
