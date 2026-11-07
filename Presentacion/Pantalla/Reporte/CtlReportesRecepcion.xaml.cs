// walterg20
using System.Windows.Controls;
using TP_ControlVehicular.Presentacion.ViewModels;
using TP_ControlVehicular.Negocio.Services;

namespace TP_ControlVehicular.Presentacion.Pantalla.Reporte
{
    /// <summary>
    /// Reportes de recepcion: historial de vehiculo y vehiculos por cliente.
    /// Cada grilla tiene su propio estado de paginacion.
    /// </summary>
    public partial class CtlReportesRecepcion : UserControl
    {
        public ReportesViewModel? ViewModel => DataContext as ReportesViewModel;

        public CtlReportesRecepcion()
        {
            InitializeComponent();
            try
            {
                var vmReportes = App.ServiceProvider?.GetService(typeof(ReportesViewModel)) as ReportesViewModel;
                if (vmReportes is not null)
                {
                    DataContext = vmReportes;
                }
            }
            catch (Exception ex)
            {
                (App.ServiceProvider?.GetService(typeof(IErrorNotifier)) as IErrorNotifier)?.Notify(ex, "CtlReportesRecepcion.xaml");
                // Silenciar si DI no está listo
            }
        }

        /// <summary>Busca el historial con retardo mientras el usuario escribe.</summary>
        private void TxtPatente_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded || ViewModel is null) return;
            ViewModel.ProgramarBusqueda();
        }

        /// <summary>Busca los vehiculos del cliente con retardo mientras el usuario escribe.</summary>
        private void TxtDni_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded || ViewModel is null) return;
            ViewModel.ProgramarBusquedaVehiculos();
        }
    }
}
