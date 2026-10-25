using System.Windows;
using System.Windows.Controls;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Pantalla.Configuracion
{
    public partial class CtlConfiguracion : UserControl
    {
        public CtlConfiguracion()
        {
            InitializeComponent();
            Loaded += CtlConfiguracion_Loaded;
        }

        private async void CtlConfiguracion_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is EmpresaConfiguracionViewModel vm)
            {
                await vm.LoadAsync();
            }
        }

        private void BtnSeleccionarLogo_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is EmpresaConfiguracionViewModel vm)
            {
                vm.SeleccionarLogo(Window.GetWindow(this));
            }
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is EmpresaConfiguracionViewModel vm)
            {
                await vm.GuardarAsync();
            }
        }
    }
}