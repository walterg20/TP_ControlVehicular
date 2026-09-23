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

        public async Task<IEnumerable<Modelo>> GetByMarcaAsync(int idMarca) =>
            await _cvDbContext.Modelos.Where(m => m.Id == idMarca).ToListAsync();
    }
}
