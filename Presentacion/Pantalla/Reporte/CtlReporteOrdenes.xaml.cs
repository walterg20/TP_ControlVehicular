using System.Windows.Controls;
using TP_ControlVehicular.Presentacion.ViewModels;

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
            catch
            {
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
    }
}
