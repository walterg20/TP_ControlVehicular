// walterg20
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Services;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    /// <summary>
    /// Dashboard general. Ademas de las tarjetas de resumen, muestra un conjunto de
    /// graficos que dependen del rol del usuario autenticado.
    /// </summary>
    public class DashboardViewModel : BaseViewModel
    {
        private readonly ObtenerDashboardHandler _obtenerDashboardHandler;

        private int _vehiculosActivosCount;
        public int VehiculosActivosCount
        {
            get => _vehiculosActivosCount;
            set => SetProperty(ref _vehiculosActivosCount, value);
        }

        private int _enProcesoCount;
        public int EnProcesoCount
        {
            get => _enProcesoCount;
            set => SetProperty(ref _enProcesoCount, value);
        }

        private int _entregadasHoyCount;
        public int EntregadasHoyCount
        {
            get => _entregadasHoyCount;
            set => SetProperty(ref _entregadasHoyCount, value);
        }

        private decimal _ingresosMesTotal;
        public decimal IngresosMesTotal
        {
            get => _ingresosMesTotal;
            set => SetProperty(ref _ingresosMesTotal, value);
        }

        private bool _mostrarIngresos;
        public bool MostrarIngresos
        {
            get => _mostrarIngresos;
            set
            {
                if (SetProperty(ref _mostrarIngresos, value))
                {
                    OnPropertyChanged(nameof(TarjetasResumenVisible));
                }
            }
        }

        // Numero de tarjetas visibles en la fila de resumen (3 o 4 segun el rol).
        public int TarjetasResumenVisible => MostrarIngresos ? 4 : 3;

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // ------------------------------------------------------------------
        // Gráficos por rol
        // ------------------------------------------------------------------

        /// <summary>Administrador: ingresos agrupados por mes.</summary>
        public ObservableCollection<DashboardSerieDto> IngresosPorMes { get; } = new();

        /// <summary>Administrador: modelos más reparados.</summary>
        public ObservableCollection<DashboardSerieDto> ModelosMasReparados { get; } = new();

        /// <summary>Administrador: cantidad por servicio.</summary>
        public ObservableCollection<DashboardSerieDto> ServiciosPorRango { get; } = new();

        /// <summary>Recepcionista: órdenes agrupadas por estado.</summary>
        public ObservableCollection<DashboardSerieDto> OrdenesPorEstado { get; } = new();

        /// <summary>Mecánico: sus tareas agrupadas por estado.</summary>
        public ObservableCollection<DashboardSerieDto> TareasPorEstado { get; } = new();

        private int _tareasAsignadas;
        public int TareasAsignadas
        {
            get => _tareasAsignadas;
            set => SetProperty(ref _tareasAsignadas, value);
        }

        private int _tareasCompletadas;
        public int TareasCompletadas
        {
            get => _tareasCompletadas;
            set => SetProperty(ref _tareasCompletadas, value);
        }

        private double _porcentajeAvance;
        public double PorcentajeAvance
        {
            get => _porcentajeAvance;
            set => SetProperty(ref _porcentajeAvance, value);
        }

        private bool _mostrarGraficosGerenciales;
        public bool MostrarGraficosGerenciales
        {
            get => _mostrarGraficosGerenciales;
            set => SetProperty(ref _mostrarGraficosGerenciales, value);
        }

        private bool _mostrarGraficoOrdenes;
        public bool MostrarGraficoOrdenes
        {
            get => _mostrarGraficoOrdenes;
            set => SetProperty(ref _mostrarGraficoOrdenes, value);
        }

        private bool _mostrarGraficoMecanico;
        public bool MostrarGraficoMecanico
        {
            get => _mostrarGraficoMecanico;
            set => SetProperty(ref _mostrarGraficoMecanico, value);
        }

        private string _tituloSeccion = string.Empty;
        /// <summary>Título del bloque inferior, según el rol.</summary>
        public string TituloSeccion
        {
            get => _tituloSeccion;
            set => SetProperty(ref _tituloSeccion, value);
        }

        public ICommand CargarDashboardCommand { get; }

        public DashboardViewModel(ObtenerDashboardHandler obtenerDashboardHandler)
        {
            _obtenerDashboardHandler = obtenerDashboardHandler;

            CargarDashboardCommand = new RelayCommand(LoadAsync);
        }

        public async Task LoadAsync()
        {
            IsLoading = true;
            try
            {
                var result = await _obtenerDashboardHandler.HandleAsync();
                VehiculosActivosCount = result.VehiculosActivosCount;
                EnProcesoCount = result.EnProcesoCount;
                EntregadasHoyCount = result.EntregadasHoyCount;
                IngresosMesTotal = result.IngresosMesTotal;
                MostrarIngresos = result.MostrarIngresos;

                ReemplazarSerie(IngresosPorMes, result.IngresosPorMes);
                ReemplazarSerie(ModelosMasReparados, result.ModelosMasReparados);
                ReemplazarSerie(ServiciosPorRango, result.ServiciosPorRango);
                ReemplazarSerie(OrdenesPorEstado, result.OrdenesPorEstado);
                ReemplazarSerie(TareasPorEstado, result.TareasPorEstado);

                TareasAsignadas = result.TareasAsignadas;
                TareasCompletadas = result.TareasCompletadas;
                PorcentajeAvance = result.PorcentajeAvance;

                var esAdministrador = result.MostrarGraficosGerenciales;
                var esMecanico = result.MostrarGraficoMecanico;

                MostrarGraficosGerenciales = esAdministrador;
                MostrarGraficoOrdenes = result.MostrarGraficoOrdenes;
                MostrarGraficoMecanico = esMecanico;

                TituloSeccion = MostrarGraficosGerenciales
                    ? "📊 Visiones gerenciales"
                    : MostrarGraficoMecanico
                        ? "🔧 Mi rendimiento"
                        : "📋 Estado de órdenes";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private static void ReemplazarSerie(
            ObservableCollection<DashboardSerieDto> destino,
            System.Collections.Generic.IEnumerable<DashboardSerieDto> origen)
        {
            destino.Clear();
            foreach (var item in origen)
            {
                destino.Add(item);
            }
        }
    }
}
