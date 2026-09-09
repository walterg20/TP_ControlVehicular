using System.Windows.Controls;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Pantalla.Dashboard
{
    /// <summary>
    /// Interaction logic for CtlDashboard.xaml
    /// </summary>
    public partial class CtlDashboard : UserControl
    {
        public DashboardViewModel? ViewModel => DataContext as DashboardViewModel;

        public CtlDashboard()
        {
            InitializeComponent();
            try
            {
                if (App.ServiceProvider != null)
                {
                    var vm = App.ServiceProvider.GetService(typeof(DashboardViewModel)) as DashboardViewModel;
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

        public CtlDashboard(DashboardViewModel viewModel) : this()
        {
            DataContext = viewModel;
            Loaded += async (s, e) => await viewModel.LoadAsync();
        }
    }
}
