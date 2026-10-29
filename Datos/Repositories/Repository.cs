// walterg20
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Negocio.Interfaces;


namespace TP_ControlVehicular.Datos.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly CVDbContext _cvDbContext;
        private readonly DbSet<T> _dbSet;

        public Repository(CVDbContext cvDbContext)
        {
            _cvDbContext = cvDbContext;
            _dbSet = _cvDbContext.Set<T>();
        }
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            var keyName = _cvDbContext.Model.FindEntityType(typeof(T))?.FindPrimaryKey()?.Properties.Select(x => x.Name).SingleOrDefault();
            if (keyName != null)
            {
                return await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => EF.Property<int>(e, keyName) == id);
            }
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.AsNoTracking().ToListAsync();

        public virtual async Task<IEnumerable<T>> GetTodosAsync() => await _dbSet.AsNoTracking().ToListAsync();

        /// <summary>
        /// Recupera una pagina de registros y el total de registros que cumplen el filtro.
        /// EF Core traduce <c>Skip</c>/<c>Take</c> a <c>OFFSET ... FETCH NEXT ... ROWS ONLY</c>.
        /// </summary>
        /// <param name="pagina">Pagina solicitada (base 1).</param>
        /// <param name="tamanoPagina">Cantidad de registros por pagina.</param>
        /// <param name="filtro">Predicado opcional aplicado antes de contar y paginar.</param>
        /// <param name="configurador">Permite aplicar <c>Include</c> u otras configuraciones sobre la consulta.</param>
        /// <param name="orden">Ordenamiento opcional; por defecto se conserva el orden natural de la tabla.</param>
        public virtual async Task<(IEnumerable<T> Items, int TotalRegistros)> GetPaginadoAsync(
            int pagina,
            int tamanoPagina,
            Expression<Func<T, bool>>? filtro = null,
            Func<IQueryable<T>, IQueryable<T>>? configurador = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orden = null)
        {
            var query = _dbSet.AsNoTracking().AsQueryable();

            if (configurador is not null)
            {
                query = configurador(query);
            }

            if (filtro is not null)
            {
                query = query.Where(filtro);
            }

            var totalRegistros = await query.CountAsync();

            var consultaOrdenada = orden is not null ? orden(query) : query;
            var items = await consultaOrdenada
                .Skip((Math.Max(pagina, 1) - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();

            return (items, totalRegistros);
        }

        /// <summary>
        /// Construye el patron <c>LIKE</c> para una busqueda de subcadena literal.
        /// Los caracteres especiales de <c>LIKE</c> (<c>%</c>, <c>_</c>, <c>[</c>) se escapan
        /// para que la busqueda se comporte como una busqueda de texto, no como un comodin.
        /// </summary>
        protected static string CrearPatronLike(string texto)
        {
            var escapado = texto
                .Replace("[", "[[]")
                .Replace("%", "[%]")
                .Replace("_", "[_]");
            return $"%{escapado}%";
        }

        public async Task AddAsync(T entity)
        {
            _cvDbContext.ChangeTracker.Clear();
            await _dbSet.AddAsync(entity);
            await _cvDbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _cvDbContext.ChangeTracker.Clear();
            _cvDbContext.Entry(entity).State = EntityState.Modified;
            await _cvDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _cvDbContext.SaveChangesAsync();
            }
        }
    }
}
