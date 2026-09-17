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

        public CtlDashboard(DashboardViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}
