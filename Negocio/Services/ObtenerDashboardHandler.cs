// walterg20
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

        public ObtenerDashboardHandler(
            IRegistroServicioRepository registroServicioRepository,
            IReporteGerencialRepository reporteGerencialRepository)
        {
            _registroServicioRepository = registroServicioRepository;
            _reporteGerencialRepository = reporteGerencialRepository;
        }

        /// <summary>Estados de una orden de servicio, en el orden en que se muestran.</summary>
        private static readonly string[] EstadosOrden =
        {
            RegistroServicio.EstadoAbierta,
            RegistroServicio.EstadoEnProceso,
            RegistroServicio.EstadoCompletada,
            RegistroServicio.EstadoPagada,
            RegistroServicio.EstadoCancelada
        };

        /// <summary>Estados de una tarea de un mecánico, en el orden en que se muestran.</summary>
        private static readonly string[] EstadosTarea = { "Pendiente", "En Curso", "Finalizada" };

        public async Task<DashboardMetricsDto> HandleAsync()
        {
            var currentUser = UserSession.CurrentUser;
            var esMecanico = currentUser != null && currentUser.IdRol == (int)RolesSistema.Mecanico;
            var esAdministrador = currentUser != null && currentUser.IdRol == (int)RolesSistema.Administrador;
            var esRecepcionista = currentUser != null && currentUser.IdRol == (int)RolesSistema.Recepcionista;

            // Scoping por rol: el Mecanico solo ve las ordenes donde participa.
            int? mecanicoId = esMecanico ? currentUser!.IdUsuario : (int?)null;

            var ordenes = await _registroServicioRepository.GetReporteCompletoAsync(mecanicoId);

            // Unidad de las metricas: la orden de trabajo (RegistroServicio).
            var ordenesActivas = ordenes
                .Where(o => o.Estado == RegistroServicio.EstadoAbierta
                         || o.Estado == RegistroServicio.EstadoEnProceso)
                .ToList();

            var hoy = DateTime.Today;
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);

            var entregadasHoy = ordenes.Count(o =>
                (o.Estado == RegistroServicio.EstadoCompletada || o.Estado == RegistroServicio.EstadoPagada)
                && o.Fecha.Date == hoy);

            // Vehiculos activos: vehiculos distintos con al menos una orden activa.
            var vehiculosActivosCount = ordenesActivas
                .GroupBy(o => o.VehiculoId)
                .Count();

            var metricas = new DashboardMetricsDto
            {
                VehiculosActivosCount = vehiculosActivosCount,
                EnProcesoCount = ordenesActivas.Count,
                EntregadasHoyCount = entregadasHoy,
                MostrarGraficosGerenciales = esAdministrador,
                MostrarGraficoOrdenes = esRecepcionista,
                MostrarGraficoMecanico = esMecanico
            };

            // Ingresos del mes: solo Administrador.
            if (esAdministrador)
            {
                var ingresos = await _reporteGerencialRepository.ObtenerIngresosPorFechaAsync(inicioMes, hoy);
                metricas.IngresosMesTotal = ingresos?.Sum(i => i.IngresosTotales) ?? 0m;
                metricas.MostrarIngresos = true;

                await CargarGraficosGerencialesAsync(metricas, inicioMes, hoy);
            }
            else if (esMecanico)
            {
                // El mecanico ve TODAS sus tareas (sin filtro de fecha): una tarea pendiente
                // de un mes anterior sigue siendo pendiente.
                var misTareas = ordenes
                    .SelectMany(o => o.Detalles ?? new List<DetalleServicio>())
                    .Where(d => d.UsuarioId == currentUser!.IdUsuario)
                    .ToList();

                metricas.TareasPorEstado = EstadosTarea
                    .Select(estado => new DashboardSerieDto
                    {
                        Etiqueta = estado,
                        Valor = misTareas.Count(d => d.Estado == estado)
                    })
                    .ToList();

                metricas.TareasAsignadas = misTareas.Count;
                metricas.TareasCompletadas = misTareas.Count(d => d.Estado == "Finalizada");
                metricas.PorcentajeAvance = metricas.TareasAsignadas == 0
                    ? 0
                    : Math.Round(metricas.TareasCompletadas * 100.0 / metricas.TareasAsignadas, 0);
            }
            else
            {
                // Recepcionista: ordenes del mes en curso agrupadas por estado.
                var ordenesDelMes = ordenes
                    .Where(o => o.Fecha >= inicioMes && o.Fecha < hoy.AddDays(1))
                    .ToList();

                metricas.OrdenesPorEstado = EstadosOrden
                    .Select(estado => new DashboardSerieDto
                    {
                        Etiqueta = estado,
                        Valor = ordenesDelMes.Count(o => o.Estado == estado)
                    })
                    .ToList();
            }

            return metricas;
        }

        /// <summary>
        /// Carga los tres gráficos del Reporte Gerencial para el Administrador.
        /// </summary>
        private async Task CargarGraficosGerencialesAsync(
            DashboardMetricsDto metricas,
            DateTime inicioMes,
            DateTime hoy)
        {
            // 1. Ingresos por mes: ultimos 6 meses (incluye el mes en curso).
            var desdeIngresos = inicioMes.AddMonths(-5);
            var mensuales = await _reporteGerencialRepository.ObtenerIngresosMensualesAsync(desdeIngresos, hoy);

            metricas.IngresosPorMes = mensuales
                .OrderBy(m => m.FechaMes)
                .Select(m => new DashboardSerieDto
                {
                    Etiqueta = FormatearPeriodo(m.PeriodoMes, m.FechaMes),
                    Valor = m.IngresosTotales
                })
                .ToList();

            // 2. Modelos mas reparados: ultimos 6 meses, top 8.
            var modelos = await _reporteGerencialRepository.ObtenerModelosMasReparadosPaginadoAsync(
                desdeIngresos, hoy, 1, 8);

            metricas.ModelosMasReparados = modelos.Items
                .Select(m => new DashboardSerieDto
                {
                    Etiqueta = m.MarcaModelo,
                    Valor = m.CantidadReparaciones
                })
                .ToList();

            // 3. Servicios por cantidad: ultimos 6 meses (el agrupamiento top N + Otros lo
            //    aplica el control de torta).
            var servicios = await _reporteGerencialRepository.ObtenerServiciosPorRangoPaginadoAsync(
                desdeIngresos, hoy, 1, 0);

            metricas.ServiciosPorRango = servicios.Items
                .Select(s => new DashboardSerieDto
                {
                    Etiqueta = s.Servicio,
                    Valor = s.Cantidad
                })
                .ToList();
        }

        /// <summary>
        /// Etiqueta de periodo mensual en formato "MM/aa".
        /// </summary>
        private static string FormatearPeriodo(string periodoMes, DateTime fechaMes)
        {
            if (fechaMes > DateTime.MinValue)
            {
                return fechaMes.ToString("MM/yy");
            }

            if (DateTime.TryParseExact(
                periodoMes, "yyyy-MM",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out var fecha))
            {
                return fecha.ToString("MM/yy");
            }

            return periodoMes;
        }
    }
}
