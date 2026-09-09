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
                    var vm = App.ServiceProvider.GetService(typeof(ReporteOrdenesViewModel)) as ReporteOrdenesViewModel;
                    if (vm != null)
                    {
                        DataContext = vm;
                        Loaded += async (s, e) => await vm.LoadAsync();
                    }
                }
            }
            catch
            {
                // Fallback handled gracefully
            }
        }

        public CtlReporteOrdenes(ReporteOrdenesViewModel viewModel) : this()
        {
            DataContext = viewModel;
            Loaded += async (s, e) => await viewModel.LoadAsync();
        }
    }
}
