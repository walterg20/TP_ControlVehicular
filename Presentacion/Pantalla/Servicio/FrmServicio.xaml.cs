using System.Windows;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Servicio
{
    public partial class FrmServicio : Window
    {
        public FrmServicio()
        {
            InitializeComponent();
            if (App.ServiceProvider.GetService(typeof(ServicioViewModel)) is ServicioViewModel vm)
            {
                DataContext = vm;
                vm.RegistrationCompleted += OnRegistrationCompleted;
                Closed += (_, _) => vm.RegistrationCompleted -= OnRegistrationCompleted;
            }
        }

        private void OnRegistrationCompleted(object? sender, bool ok) => Dispatcher.Invoke(() => DialogResult = ok);
        private void BtnCancelar_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
