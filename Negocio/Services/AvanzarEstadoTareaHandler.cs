using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Negocio.Services
{
    public class AvanzarEstadoTareaHandler
    {
        private readonly CVDbContext _context;

        public AvanzarEstadoTareaHandler(CVDbContext context)
        {
            _context = context;
        }

        public async Task HandleAsync(int detalleId, int usuarioAutenticadoId)
        {
            var tarea = await _context.DetalleServicios
                .Include(d => d.RegistroServicio)
                    .ThenInclude(r => r.Detalles)
                .FirstOrDefaultAsync(d => d.Id == detalleId);

            if (tarea == null) 
                throw new Exception("Tarea no encontrada.");
            
            // 1. Filtro de Seguridad
            if (tarea.UsuarioId != usuarioAutenticadoId)
                throw new UnauthorizedAccessException("No tienes permisos para modificar esta tarea.");

            if (tarea.Estado == DetalleServicio.EstadoFinalizada)
                return; // Ya está finalizada

            // 2. Avanzar estado de la tarea actual
            tarea.Estado = DetalleServicio.EstadoFinalizada;

            // 3. Obtener todas las tareas de la orden para verificar el estado global
            var orden = tarea.RegistroServicio;
            var todasLasTareas = orden.Detalles.OrderBy(d => d.OrdenEjecucion).ToList();

            // 4. Lógica Secuencial / Cierre de circuito
            var tareasPendientes = todasLasTareas.Where(t => t.Estado != DetalleServicio.EstadoFinalizada).ToList();

            if (!tareasPendientes.Any())
            {
                // Si no hay pendientes, la orden completa pasa a "Completada"
                orden.Estado = RegistroServicio.EstadoCompletada;
            }
            else
            {
                // Habilitamos la siguiente tarea en la secuencia
                var siguienteTarea = tareasPendientes.First();
                if (siguienteTarea.Estado == DetalleServicio.EstadoPendiente)
                {
                    siguienteTarea.Estado = DetalleServicio.EstadoEnCurso;
                }
                
                // Asegurarse de que la orden esté "En Proceso"
                if (orden.Estado == RegistroServicio.EstadoAbierta)
                {
                    orden.Estado = RegistroServicio.EstadoEnProceso;
                }
            }

            // Guardar cambios transaccionalmente (Ef Core maneja esto automáticamente al llamar SaveChanges)
            await _context.SaveChangesAsync();
        }
    }
}
