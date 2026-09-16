using System.Windows;
using System.Windows.Controls;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Servicio
{
    public partial class CtlServicio : UserControl
    {
        public CtlServicio()
        {
            InitializeComponent();
            if (App.ServiceProvider.GetService(typeof(ServicioViewModel)) is ServicioViewModel vm)
            {
                DataContext = vm;
                Loaded += async (_, _) => await vm.LoadAsync();
            }
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ServicioViewModel vm)
                dgServicios.ItemsSource = vm.ListadoServiciosFiltered;
        }

        private async void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not ServicioViewModel vm) return;
            vm.LimpiarFormulario();
            var modal = new FrmServicio { Owner = Window.GetWindow(this) };
            if (modal.ShowDialog() == true) await vm.LoadAsync();
        }

        private async void BtnBorrar_Click(object sender, RoutedEventArgs e)
        {
            if (dgServicios.SelectedItem is not ServicioDto selected)
            {
                MessageBox.Show("Por favor, selecciona primero un servicio.", "Aviso");
                return;
            }
            if (MessageBox.Show($"¿Seguro que querés marcar como inactivo al servicio {selected.Nombre}?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes && DataContext is ServicioViewModel vm)
                await vm.DeleteServicioAsync(selected.IdServicio);
        }

        private async void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgServicios.SelectedItem is not ServicioDto selected)
            {
                MessageBox.Show("Por favor, selecciona primero un servicio.", "Aviso");
                return;
            }
            if (DataContext is not ServicioViewModel vm) return;
            vm.CargarServicio(selected);
            var modal = new FrmServicio { Owner = Window.GetWindow(this) };
            if (modal.ShowDialog() == true) await vm.LoadAsync();
            else vm.LimpiarFormulario();
        }
    }
}
