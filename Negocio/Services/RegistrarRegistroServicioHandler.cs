using AutoMapper;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class RegistrarRegistroServicioHandler
    {
        private readonly IRegistroServicioRepository _repository;
        private readonly IMapper _mapper;

        public RegistrarRegistroServicioHandler(IRegistroServicioRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<RegistroServicioDto> HandleAsync(RegistroServicio registro)
        {
            // Set order sequence and initial states for tasks
            if (registro.Detalles != null && registro.Detalles.Any())
            {
                int order = 1;
                foreach (var detalle in registro.Detalles)
                {
                    detalle.OrdenEjecucion = order;
                    detalle.Estado = (order == 1) ? DetalleServicio.EstadoEnCurso : DetalleServicio.EstadoPendiente;
                    order++;
                }
            }
            
            registro.Estado = RegistroServicio.EstadoEnProceso; // Al registrar, está en proceso de ejecución de tareas

            await _repository.AddAsync(registro);
            
            // Recargar con relaciones para el mapeo completo al DTO
            var list = await _repository.GetReporteCompletoAsync();
            var added = list.FirstOrDefault(r => r.Id == registro.Id) ?? registro;
            
            return _mapper.Map<RegistroServicioDto>(added);
        }
    }
}
