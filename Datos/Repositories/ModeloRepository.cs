// walterg20
using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;


namespace TP_ControlVehicular.Datos.Repositories
{
    public class ModeloRepository : Repository<Modelo>, IModeloRepository
    {
        private readonly CVDbContext _cvDbContext;

        public ModeloRepository(CVDbContext cvDbContext) : base(cvDbContext)
        {
            _cvDbContext = cvDbContext;
        }

        public override async Task<IEnumerable<Modelo>> GetAllAsync() =>
            await _cvDbContext.Modelos
                .Include(m => m.Marca)
                .AsNoTracking()
                .ToListAsync();

        public override async Task<IEnumerable<Modelo>> GetTodosAsync() =>
            await _cvDbContext.Modelos
                .Include(m => m.Marca)
                .AsNoTracking()
                .ToListAsync();

        public override async Task<Modelo?> GetByIdAsync(int id) =>
            await _cvDbContext.Modelos
                .Include(m => m.Marca)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

        public async Task<IEnumerable<Modelo>> GetByMarcaAsync(int idMarca) =>
            await _cvDbContext.Modelos
                .Include(m => m.Marca)
                .Where(m => m.MarcaId == idMarca)
                .ToListAsync();
    }
}
