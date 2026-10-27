// walterg20
using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Datos.Repositories
{
    public class RegistroServicioRepository : Repository<RegistroServicio>, IRegistroServicioRepository
    {
        private readonly CVDbContext _cvDbContext;

        public RegistroServicioRepository(CVDbContext cvDbContext) : base(cvDbContext)
        {
            _cvDbContext = cvDbContext;
        }

        public new async Task UpdateAsync(RegistroServicio entity)
        {
            _cvDbContext.ChangeTracker.Clear();

            var existente = await _cvDbContext.RegistroServicios
                .Include(r => r.Detalles)
                .FirstOrDefaultAsync(r => r.Id == entity.Id);

            if (existente != null)
            {
                _cvDbContext.Entry(existente).CurrentValues.SetValues(entity);

                // Eliminar detalles que ya no estn
                foreach (var detalleExistente in existente.Detalles.ToList())
                {
                    if (!entity.Detalles.Any(d => d.Id == detalleExistente.Id))
                    {
                        _cvDbContext.DetalleServicios.Remove(detalleExistente);
                    }
                }

                // Agregar o actualizar detalles
                foreach (var detalleModel in entity.Detalles)
                {
                    var detalleExistente = existente.Detalles.FirstOrDefault(d => d.Id == detalleModel.Id);
                    if (detalleExistente != null)
                    {
                        _cvDbContext.Entry(detalleExistente).CurrentValues.SetValues(detalleModel);
                    }
                    else
                    {
                        // Asegurarse de que el ID del registro de servicio coincida
                        detalleModel.RegistroServicioId = existente.Id;
                        existente.Detalles.Add(detalleModel);
                    }
                }

                await _cvDbContext.SaveChangesAsync();
            }
            else
            {
                _cvDbContext.RegistroServicios.Update(entity);
                await _cvDbContext.SaveChangesAsync();
            }
        }

        public async Task<List<RegistroServicio>> GetReporteCompletoAsync(int? mecanicoId = null)
        {
            var query = _cvDbContext.RegistroServicios
                .Include(r => r.Vehiculo)
                    .ThenInclude(v => v.Propietarios).ThenInclude(p => p.Cliente)
                .Include(r => r.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                        .ThenInclude(m => m.Marca)
                .Include(r => r.Usuario) // Recepcionista
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.Servicio)
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.Usuario) // Mecánico
                .AsNoTracking().AsQueryable();

            if (mecanicoId.HasValue)
            {
                query = query.Where(r => r.Detalles.Any(d => d.UsuarioId == mecanicoId.Value));
            }

            return await query.OrderByDescending(r => r.Fecha).ToListAsync();
        }
    }
}
