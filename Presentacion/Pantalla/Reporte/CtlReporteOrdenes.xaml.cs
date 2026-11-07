// walterg20
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TP_ControlVehicular.Presentacion.ViewModels;
using TP_ControlVehicular.Negocio.Services;

namespace TP_ControlVehicular.Presentacion.Pantalla.Reporte
{
    /// <summary>
    /// Interaction logic for CtlReporteOrdenes.xaml
    /// </summary>
    public partial class CtlReporteOrdenes : UserControl
    {
        public ReporteOrdenesViewModel? ViewModel => DataContext as ReporteOrdenesViewModel;

        public CtlReporteOrdenes()
        {
            InitializeComponent();
            try
            {
                if (App.ServiceProvider != null)
                {
                    var vmReporteOrdenes = App.ServiceProvider.GetService(typeof(ReporteOrdenesViewModel)) as ReporteOrdenesViewModel;
                    if (vmReporteOrdenes != null)
                    {
                        DataContext = vmReporteOrdenes;
                    }
                }
            }
            catch (Exception ex)
            {
                (App.ServiceProvider?.GetService(typeof(IErrorNotifier)) as IErrorNotifier)?.Notify(ex, "CtlReporteOrdenes.xaml");
                // Fallback handled gracefully
            }

            Loaded += async (s, e) =>
            {
                if (ViewModel != null)
                {
                    await ViewModel.LoadAsync();
                }
            };
        }

        public CtlReporteOrdenes(ReporteOrdenesViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }

        /// <summary>Ejecuta la consulta con los filtros aplicados.</summary>
        private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel is null) return;
            ViewModel.CancelarBusquedaProgramada();
            await ViewModel.BuscarAsync();
        }

        /// <summary>Busca con retardo mientras el usuario escribe.</summary>
        private void TxtBusqueda_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded || ViewModel is null) return;
            ViewModel.ProgramarBusqueda();
        }

        /// <summary>La tecla Enter ejecuta la busqueda de inmediato.</summary>
        private async void TxtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter || ViewModel is null) return;
            ViewModel.CancelarBusquedaProgramada();
            await ViewModel.BuscarAsync();
        }

        /// <summary>Recarga el reporte al cambiar el rango de fechas.</summary>
        private void DtpFechaDesde_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded || ViewModel is null) return;
            ViewModel.ProgramarBusqueda();
        }

        /// <summary>Recarga el reporte al cambiar el rango de fechas.</summary>
        private void DtpFechaHasta_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded || ViewModel is null) return;
            ViewModel.ProgramarBusqueda();
        }

        /// <summary>Recarga el reporte al cambiar el filtro de estado.</summary>
        private void CboEstado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded || ViewModel is null) return;
            ViewModel.ProgramarBusqueda();
        }
    }
}
