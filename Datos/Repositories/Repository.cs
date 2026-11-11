// walterg20
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Negocio.Interfaces;


namespace TP_ControlVehicular.Datos.Repositories
{
    /// <summary>
    /// Repositorio base. Cada operacion usa su propio <see cref="CVDbContext"/> (creado por
    /// <see cref="IDbContextFactory{TContext}"/>) para que dos operaciones concurrentes nunca
    /// compartan la misma instancia de contexto.
    /// </summary>
    /// <remarks>
    /// En WPF no hay un ambito de request: los servicios Scoped viven toda la vida de la
    /// aplicacion. Si el repositorio guardara un unico contexto, dos pantallas cargando a la
    /// vez (o el evento Loaded disparandose de nuevo sobre un control reutilizado) fallarian
    /// con "A second operation was started on this context instance".
    /// </remarks>
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly IDbContextFactory<CVDbContext> _contextFactory;

        public Repository(IDbContextFactory<CVDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        /// <summary>Crea un contexto nuevo, exclusivo de la operacion que lo va a usar.</summary>
        protected CVDbContext CrearContexto() => _contextFactory.CreateDbContext();

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            await using var ctx = CrearContexto();
            var keyName = ctx.Model.FindEntityType(typeof(T))?.FindPrimaryKey()?.Properties.Select(x => x.Name).SingleOrDefault();
            if (keyName != null)
            {
                return await ctx.Set<T>().AsNoTracking().FirstOrDefaultAsync(e => EF.Property<int>(e, keyName) == id);
            }
            return await ctx.Set<T>().FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            await using var ctx = CrearContexto();
            return await ctx.Set<T>().AsNoTracking().ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetTodosAsync()
        {
            await using var ctx = CrearContexto();
            return await ctx.Set<T>().AsNoTracking().ToListAsync();
        }

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
            await using var ctx = CrearContexto();

            var query = ctx.Set<T>().AsNoTracking().AsQueryable();

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
            await using var ctx = CrearContexto();
            ctx.ChangeTracker.Clear();
            await ctx.Set<T>().AddAsync(entity);
            await ctx.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            await using var ctx = CrearContexto();
            ctx.ChangeTracker.Clear();
            ctx.Entry(entity).State = EntityState.Modified;
            await ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await using var ctx = CrearContexto();
            var entity = await ctx.Set<T>().FindAsync(id);
            if (entity != null)
            {
                ctx.Set<T>().Remove(entity);
                await ctx.SaveChangesAsync();
            }
        }
    }
}
