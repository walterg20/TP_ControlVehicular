using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.DTOs.Reportes;
using TP_ControlVehicular.Negocio.Services;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public class ReporteGerencialViewModel : BaseViewModel
    {
        private readonly ObtenerReporteIngresosHandler _obtenerReporteIngresosHandler;
        private readonly ObtenerReporteTiemposResolucionHandler _obtenerReporteTiemposResolucionHandler;
        private readonly ListarTallerHandler _listarTallerHandler;

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ObservableCollection<ReporteIngresosDto> ReportesIngresos { get; } = new();
        public ObservableCollection<ReporteTiemposResolucionDto> ReportesTiemposResolucion { get; } = new();
        public ObservableCollection<TallerDto> ListaTalleres { get; } = new();
        
        private DateTime _fechaDesde = DateTime.Today.AddDays(-30);
        public DateTime FechaDesde
        {
            get => _fechaDesde;
            set => SetProperty(ref _fechaDesde, value);
        }

        private DateTime _fechaHasta = DateTime.Today;
        public DateTime FechaHasta
        {
            get => _fechaHasta;
            set => SetProperty(ref _fechaHasta, value);
        }

        private int? _tallerIdSeleccionado;
        public int? TallerIdSeleccionado
        {
            get => _tallerIdSeleccionado;
            set => SetProperty(ref _tallerIdSeleccionado, value);
        }

        public ICommand GenerarReportesCommand { get; }

        public ReporteGerencialViewModel(
            ObtenerReporteIngresosHandler obtenerReporteIngresosHandler,
            ObtenerReporteTiemposResolucionHandler obtenerReporteTiemposResolucionHandler,
            ListarTallerHandler listarTallerHandler)
        {
            _obtenerReporteIngresosHandler = obtenerReporteIngresosHandler;
            _obtenerReporteTiemposResolucionHandler = obtenerReporteTiemposResolucionHandler;
            _listarTallerHandler = listarTallerHandler;
            
            GenerarReportesCommand = new RelayCommand(GenerarReportesAsync);
            
            _ = LoadTalleresAsync();
        }

        private async Task LoadTalleresAsync()
        {
            var talleres = await _listarTallerHandler.HandleAsync();
            ListaTalleres.Clear();
            ListaTalleres.Add(new TallerDto { IdTaller = 0, Nombre = "Todos" });
            foreach(var t in talleres)
            {
                ListaTalleres.Add(t);
            }
        }

        private async Task GenerarReportesAsync()
        {
            IsLoading = true;
            try
            {
                int? tallerId = TallerIdSeleccionado == 0 ? null : TallerIdSeleccionado;
                var revenueResult = await _obtenerReporteIngresosHandler.HandleAsync(FechaDesde, FechaHasta, tallerId);
                var resolutionResult = await _obtenerReporteTiemposResolucionHandler.HandleAsync(FechaDesde, FechaHasta, tallerId);

                ReportesIngresos.Clear();
                if (revenueResult != null)
                {
                    foreach(var r in revenueResult)
                        ReportesIngresos.Add(r);
                }

                ReportesTiemposResolucion.Clear();
                if (resolutionResult != null)
                {
                    foreach(var r in resolutionResult)
                        ReportesTiemposResolucion.Add(r);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
