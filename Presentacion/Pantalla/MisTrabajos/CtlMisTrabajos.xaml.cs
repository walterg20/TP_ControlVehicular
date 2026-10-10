using System.Windows;
using System.Windows.Controls;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Pantalla.MisTrabajos
{
    public partial class CtlMisTrabajos : UserControl
    {
        public CtlMisTrabajos()
        {
            InitializeComponent();
            ConfigurarViewModel();
        }

        private async void ConfigurarViewModel()
        {
            if (App.ServiceProvider?.GetService(typeof(MisTrabajosViewModel)) is MisTrabajosViewModel vm)
            {
                DataContext = vm;
                await vm.LoadAsync();
            }
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MisTrabajosViewModel vm)
            {
                var ok = await vm.GuardarCambiosAsync();
                if (ok && Application.Current.MainWindow is MainWindow win)
                {
                    win.MostrarToast("Cambios guardados con éxito.");
                }
            }
        }

        private void BtnVerOrden_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is RegistroServicioDto orden)
            {
                if (Application.Current.MainWindow is MainWindow win)
                {
                    // Call the CtlOrdenServicioForm passing "MisTrabajos" as origin
                    win.AgregarPagina(new Pantalla.OrdenServicio.CtlOrdenServicioForm(orden, "MisTrabajos"));
                }
            }
        }
    }
}
