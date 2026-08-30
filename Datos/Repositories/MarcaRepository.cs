using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;


namespace TP_ControlVehicular.Datos.Repositories
{
    public class MarcaRepository : Repository<Marca>, IMarcaRepository
    {
        private readonly CVDbContext _context;

        public MarcaRepository(CVDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Marca?> GetByNombreAsync(string nombre)
        {
            return await _context.Marcas
                .Include(m => m.Modelos)
                .FirstOrDefaultAsync(m => m.NombreMarca == nombre);
        }

    }
}
