using System.Windows;
using System.Windows.Controls;
using TP_ControlVehicular.Presentacion.Pantalla.Compartido;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Vehiculo
{
    public partial class CtlVehiculo : UserControl
    {
        public CtlVehiculo()
        {
            InitializeComponent();
            try
            {
                if (App.ServiceProvider.GetService(typeof(VehiculoViewModel)) is VehiculoViewModel vmVehiculo)
                {
                    this.DataContext = vmVehiculo;
                    this.Loaded += async (s, e) => { await vmVehiculo.LoadAsync(); };
                }
            }
            catch
            {
                // Silenciar si DI no está listo
            }
        }

        private async void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            var frm = new FrmVehiculo();
            frm.Owner = Window.GetWindow(this);
            var ok = frm.ShowDialog();
            if (ok == true && this.DataContext is VehiculoViewModel vmVehiculo)
            {
                await vmVehiculo.LoadAsync();
            }
        }

        private async void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is VehiculoViewModel vmVehiculo)
            {
                if (vmVehiculo.VehiculoSeleccionado == null)
                {
                    FrmConfirmacion.MostrarAviso("Por favor, selecciona primero un vehículo.", "Aviso", Window.GetWindow(this));
                    return;
                }

                var frm = new FrmVehiculo(vmVehiculo.VehiculoSeleccionado);
                frm.Owner = Window.GetWindow(this);
                var ok = frm.ShowDialog();
                if (ok == true)
                {
                    await vmVehiculo.LoadAsync();
                }
            }
        }

        private async void BtnBorrar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is VehiculoViewModel vmVehiculo)
            {
                if (vmVehiculo.VehiculoSeleccionado == null)
                {
                    FrmConfirmacion.MostrarAviso("Por favor, selecciona primero un vehículo para eliminar.", "Aviso", Window.GetWindow(this));
                    return;
                }

                if (!FrmConfirmacion.Mostrar($"¿Está seguro que desea eliminar el vehículo con patente '{vmVehiculo.VehiculoSeleccionado.Patente}'?", "Confirmar Eliminación", Window.GetWindow(this)))
                    return;

                var ok = await vmVehiculo.EliminarVehiculoAsync(vmVehiculo.VehiculoSeleccionado.Id);
                if (ok)
                {
                    FrmConfirmacion.MostrarAviso("Vehículo eliminado correctamente.", "Éxito", Window.GetWindow(this), icono: "✅");
                }
                else
                {
                    FrmConfirmacion.MostrarAviso("No se pudo eliminar el vehículo.", "Error", Window.GetWindow(this), icono: "❌");
                }
            }
        }
    }
}
