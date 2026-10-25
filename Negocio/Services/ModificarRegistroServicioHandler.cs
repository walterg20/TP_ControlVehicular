using AutoMapper;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ModificarRegistroServicioHandler
    {
        private readonly IRegistroServicioRepository _repository;
        private readonly IMapper _mapper;

        public ModificarRegistroServicioHandler(IRegistroServicioRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<RegistroServicioDto> HandleAsync(RegistroServicio registro)
        {
            if (registro.Detalles != null && registro.Detalles.Any())
            {
                var todasLasTareas = registro.Detalles.OrderBy(d => d.OrdenEjecucion).ToList();
                var tareasPendientes = todasLasTareas.Where(t => t.Estado != DetalleServicio.EstadoFinalizada).ToList();

                if (registro.Estado != RegistroServicio.EstadoPagada && registro.Estado != "Cancelada")
                {
                    if (!tareasPendientes.Any())
                    {
                        registro.Estado = RegistroServicio.EstadoCompletada;
                    }
                    else
                    {
                        var siguienteTarea = tareasPendientes.First();
                        if (siguienteTarea.Estado == DetalleServicio.EstadoPendiente)
                        {
                            siguienteTarea.Estado = DetalleServicio.EstadoEnCurso;
                        }

                        if (registro.Estado == RegistroServicio.EstadoAbierta || registro.Estado == RegistroServicio.EstadoCompletada)
                        {
                            registro.Estado = RegistroServicio.EstadoEnProceso;
                        }
                    }
                }
            }

            await _repository.UpdateAsync(registro);
            
            // Recargar con relaciones
            var list = await _repository.GetReporteCompletoAsync();
            var updated = list.FirstOrDefault(r => r.Id == registro.Id) ?? registro;
            
            return _mapper.Map<RegistroServicioDto>(updated);
        }
    }
}
