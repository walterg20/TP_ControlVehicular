using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.DTOs.Reportes;
using TP_ControlVehicular.Negocio.Handlers.Reportes;
using TP_ControlVehicular.Negocio.Context;
using TP_ControlVehicular.Presentacion;
using System.Linq;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public class ReporteServiciosViewModel : BaseViewModel
    {
        private readonly ReporteTallerHandler _handler;
        // In a real app, you might inject a handler to get Mechanics. We assume something similar for now.
        // Assuming there is a way to get users with Role 3. 
        // But for simplicity, we can just bind to a mock list if we don't have the service.

        private DateTime _fechaDesde = DateTime.Now.AddMonths(-1);
        private DateTime _fechaHasta = DateTime.Now;
        private ObservableCollection<ReporteServicioDto> _reporte = new();
        private bool _isMecanicoSelectorVisible;
        private int _mecanicoSeleccionadoId;
        private string _mensajeError = string.Empty;

        public ReporteServiciosViewModel(ReporteTallerHandler handler)
        {
            _handler = handler;
            CmdGenerarReporte = new RelayCommand(GenerarReporteAsync);
            
            // Validar rol de usuario para la visibilidad del selector de mecánicos
            if (UserSession.CurrentUser != null && UserSession.CurrentUser.IdRol == 3)
            {
                IsMecanicoSelectorVisible = false;
                MecanicoSeleccionadoId = UserSession.CurrentUser.IdUsuario;
            }
            else
            {
                IsMecanicoSelectorVisible = true;
            }
        }

        public DateTime FechaDesde
        {
            get => _fechaDesde;
            set => SetProperty(ref _fechaDesde, value);
        }

        public DateTime FechaHasta
        {
            get => _fechaHasta;
            set => SetProperty(ref _fechaHasta, value);
        }

        public bool IsMecanicoSelectorVisible
        {
            get => _isMecanicoSelectorVisible;
            set => SetProperty(ref _isMecanicoSelectorVisible, value);
        }

        public int MecanicoSeleccionadoId
        {
            get => _mecanicoSeleccionadoId;
            set => SetProperty(ref _mecanicoSeleccionadoId, value);
        }

        public ObservableCollection<ReporteServicioDto> Reporte
        {
            get => _reporte;
            set => SetProperty(ref _reporte, value);
        }

        public string MensajeError
        {
            get => _mensajeError;
            set => SetProperty(ref _mensajeError, value);
        }

        public ICommand CmdGenerarReporte { get; }

        private async Task GenerarReporteAsync()
        {
            try
            {
                MensajeError = string.Empty;
                var resultado = await _handler.ObtenerServiciosPorMecanicoAsync(FechaDesde, FechaHasta, MecanicoSeleccionadoId);
                Reporte = new ObservableCollection<ReporteServicioDto>(resultado);
            }
            catch (Exception ex)
            {
                MensajeError = "Error al generar el reporte: " + ex.Message;
            }
        }
    }
}
