using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ListarTareasPorEspecialistaHandler
    {
        private readonly CVDbContext _context;
        private readonly IMapper _mapper;

        public ListarTareasPorEspecialistaHandler(CVDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DetalleServicioDto>> HandleAsync(int usuarioMecanicoId)
        {
            // Solo devolvemos tareas que pertenezcan a este mecánico y que NO estén finalizadas.
            // Opcionalmente se podría devolver solo las "En Curso", 
            // pero mostrar "Pendiente" también le permite ver qué tiene en cola.
            var tareas = await _context.DetalleServicios
                .Include(d => d.Servicio)
                .Include(d => d.RegistroServicio)
                    .ThenInclude(r => r.Vehiculo)
                .Where(t => t.UsuarioId == usuarioMecanicoId && t.Estado != DetalleServicio.EstadoFinalizada)
                .OrderBy(t => t.RegistroServicioId).ThenBy(t => t.OrdenEjecucion)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<DetalleServicioDto>>(tareas);
        }
    }
}
