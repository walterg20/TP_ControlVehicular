// walterg20
using System.Collections.Generic;

namespace TP_ControlVehicular.Negocio.DTOs
{
    public class DashboardMetricsDto
    {
        public int VehiculosActivosCount { get; set; }
        public int EnProcesoCount { get; set; }
        public int EntregadasHoyCount { get; set; }
        public decimal IngresosMesTotal { get; set; }
        public bool MostrarIngresos { get; set; }

        // Gráficos del dashboard, según el rol del usuario autenticado.

        /// <summary>Administrador: ingresos agrupados por mes (últimos 6 meses).</summary>
        public List<DashboardSerieDto> IngresosPorMes { get; set; } = new();

        /// <summary>Administrador: modelos más reparados (mes en curso).</summary>
        public List<DashboardSerieDto> ModelosMasReparados { get; set; } = new();

        /// <summary>Administrador: cantidad por servicio (mes en curso).</summary>
        public List<DashboardSerieDto> ServiciosPorRango { get; set; } = new();

        /// <summary>Recepcionista: órdenes agrupadas por estado (mes en curso).</summary>
        public List<DashboardSerieDto> OrdenesPorEstado { get; set; } = new();

        /// <summary>Mecánico: sus tareas agrupadas por estado (sin filtro de fecha).</summary>
        public List<DashboardSerieDto> TareasPorEstado { get; set; } = new();

        /// <summary>Mecánico: total de tareas asignadas.</summary>
        public int TareasAsignadas { get; set; }

        /// <summary>Mecánico: total de tareas completadas.</summary>
        public int TareasCompletadas { get; set; }

        /// <summary>Mecánico: porcentaje de avance (0..100).</summary>
        public double PorcentajeAvance { get; set; }

        // Qué conjunto de gráficos corresponde al rol del usuario autenticado.

        /// <summary>Verdadero si el usuario es Administrador (los tres gráficos gerenciales).</summary>
        public bool MostrarGraficosGerenciales { get; set; }

        /// <summary>Verdadero si el usuario es Recepcionista (órdenes por estado).</summary>
        public bool MostrarGraficoOrdenes { get; set; }

        /// <summary>Verdadero si el usuario es Mecánico (tareas por estado y avance).</summary>
        public bool MostrarGraficoMecanico { get; set; }
    }
}
