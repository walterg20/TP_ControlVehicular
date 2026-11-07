// walterg20
using System.Windows.Controls;
using TP_ControlVehicular.Presentacion.ViewModels;
using TP_ControlVehicular.Negocio.Services;

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
                    var vmDashboard = App.ServiceProvider.GetService(typeof(DashboardViewModel)) as DashboardViewModel;
                    if (vmDashboard != null)
                    {
                        DataContext = vmDashboard;
                    }
                }
            }
            catch (Exception ex)
            {
                (App.ServiceProvider?.GetService(typeof(IErrorNotifier)) as IErrorNotifier)?.Notify(ex, "CtlDashboard.xaml");
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
