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

    }
}
