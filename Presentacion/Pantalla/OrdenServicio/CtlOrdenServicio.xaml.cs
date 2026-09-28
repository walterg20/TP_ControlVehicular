using System.Windows;
using System.Windows.Controls;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Pantalla.OrdenServicio
{
    public partial class CtlOrdenServicio : UserControl
    {
        public CtlOrdenServicio()
        {
            InitializeComponent();
            this.Loaded -= CtlOrdenServicio_Loaded;
            this.Loaded += CtlOrdenServicio_Loaded;
        }

        private async void CtlOrdenServicio_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (App.ServiceProvider?.GetService(typeof(OrdenServicioViewModel)) is OrdenServicioViewModel vm)
                {
                    this.DataContext = vm;
                    await vm.LoadAsync();
                }
            }
            catch
            {
                // Manejar error inicial de carga si ocurre
            }
        }

                private void BtnNuevaOrden_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OrdenServicioViewModel vm)
            {
                if (!vm.PuedeCrear) { Compartido.FrmConfirmacion.MostrarAviso("No tiene permisos para crear órdenes.", "Acceso Denegado", Window.GetWindow(this)); return; }
                // Limpiar selección actual si la hubiera
                vm.OrdenSeleccionada = null;
                
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.AgregarPagina(new CtlOrdenServicioForm());
                }
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OrdenServicioViewModel vm)
            {
                if (vm.OrdenSeleccionada == null)
                {
                    Compartido.FrmConfirmacion.MostrarAviso("Por favor, seleccione una orden de servicio para editar.", "Aviso", Window.GetWindow(this));
                    return;
                }

                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.AgregarPagina(new CtlOrdenServicioForm(vm.OrdenSeleccionada));
                }
            }
        }

                private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OrdenServicioViewModel vm)
            {
                if (!vm.PuedeEliminar) { Compartido.FrmConfirmacion.MostrarAviso("No tiene permisos para eliminar órdenes.", "Acceso Denegado", Window.GetWindow(this)); return; }
                if (vm.OrdenSeleccionada == null)
                {
                    Compartido.FrmConfirmacion.MostrarAviso("Por favor, seleccione una orden de servicio.", "Aviso", Window.GetWindow(this));
                    return;
                }

                var confirmacion = Compartido.FrmConfirmacion.Mostrar(
                    $"¿Está seguro de querer cancelar la orden #{vm.OrdenSeleccionada.Id} del vehículo {vm.OrdenSeleccionada.VehiculoPatente}?", 
                    "Confirmación", Window.GetWindow(this));

                if (confirmacion)
                {
                    var ok = await vm.CancelarOrdenSeleccionadaAsync();
                }
            }
        }
    }
}




