using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Context;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;
using TP_ControlVehicular.Negocio.Reportes;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ObtenerDashboardHandler
    {
        private readonly IRegistroServicioRepository _registroServicioRepository;
        private readonly IReporteGerencialRepository _reporteGerencialRepository;
        private readonly IMapper _mapper;

        public ObtenerDashboardHandler(
            IRegistroServicioRepository registroServicioRepository,
            IReporteGerencialRepository reporteGerencialRepository,
            IMapper mapper)
        {
            _registroServicioRepository = registroServicioRepository;
            _reporteGerencialRepository = reporteGerencialRepository;
            _mapper = mapper;
        }

        public async Task<DashboardMetricsDto> HandleAsync()
        {
            var currentUser = UserSession.CurrentUser;
            var esMecanico = currentUser != null && currentUser.IdRol == (int)RolesSistema.Mecanico;
            var esAdministrador = currentUser != null && currentUser.IdRol == (int)RolesSistema.Administrador;

            // Scoping por rol: el Mecanico solo ve las ordenes donde participa.
            int? mecanicoId = esMecanico ? currentUser!.IdUsuario : (int?)null;

            var ordenes = await _registroServicioRepository.GetReporteCompletoAsync(mecanicoId);

            // Unidad de las metricas: la orden de trabajo (RegistroServicio).
            var ordenesActivas = ordenes
                .Where(o => o.Estado == RegistroServicio.EstadoAbierta
                         || o.Estado == RegistroServicio.EstadoEnProceso)
                .ToList();

            var hoy = DateTime.Today;
            var entregadasHoy = ordenes.Count(o =>
                (o.Estado == RegistroServicio.EstadoCompletada || o.Estado == RegistroServicio.EstadoPagada)
                && o.Fecha.Date == hoy);

            // Vehiculos activos: vehiculos distintos con al menos una orden activa.
            var vehiculosActivos = ordenesActivas
                .GroupBy(o => o.VehiculoId)
                .Select(g => g.First().Vehiculo)
                .ToList();

            var metricas = new DashboardMetricsDto
            {
                VehiculosActivosCount = vehiculosActivos.Count,
                EnProcesoCount = ordenesActivas.Count,
                EntregadasHoyCount = entregadasHoy,
                VehiculosEnTaller = _mapper.Map<List<VehiculoDto>>(vehiculosActivos)
            };

            // Ingresos del mes: solo Administrador.
            if (esAdministrador)
            {
                var primerDiaDelMes = new DateTime(hoy.Year, hoy.Month, 1);
                var ingresos = await _reporteGerencialRepository.ObtenerIngresosPorFechaAsync(primerDiaDelMes, hoy);
                metricas.IngresosMesTotal = ingresos?.Sum(i => i.IngresosTotales) ?? 0m;
                metricas.MostrarIngresos = true;
            }

            return metricas;
        }
    }
}
