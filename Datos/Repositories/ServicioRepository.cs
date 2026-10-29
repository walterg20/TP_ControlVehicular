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
    public class ServicioRepository : Repository<Servicio>, IServicioRepository
    {
        private readonly CVDbContext _cvDbContext;

        public ServicioRepository(CVDbContext cvDbContext) : base(cvDbContext)
        {
            _cvDbContext = cvDbContext;
        }

        public async Task<List<Servicio>> GetActivosAsync() =>
            await _cvDbContext.Servicios
                .Where(s => s.Activo)
                .ToListAsync();

        public async Task<Servicio?> GetByNombreAsync(string nombre) =>
            await _cvDbContext.Servicios
                .FirstOrDefaultAsync(s => s.Nombre == nombre);

        public async Task<(IEnumerable<Servicio> Items, int TotalRegistros)> GetPaginadoAsync(
            string? busqueda,
            bool incluirInactivos,
            int pagina,
            int tamanoPagina)
        {
            Expression<Func<Servicio, bool>>? filtro = null;

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var patron = CrearPatronLike(busqueda.Trim());
                filtro = s => EF.Functions.Like(s.Nombre, patron);
            }

            Expression<Func<Servicio, bool>>? soloActivos = null;
            if (!incluirInactivos)
            {
                soloActivos = s => s.Activo;
            }

            return await GetPaginadoAsync(
                pagina,
                tamanoPagina,
                filtro,
                q => soloActivos is null ? q : q.Where(soloActivos),
                q => q.OrderBy(s => s.Id));
        }
    }
}
