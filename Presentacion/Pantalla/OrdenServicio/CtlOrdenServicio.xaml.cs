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
                // Limpiar selección actual si la hubiera
                vm.OrdenSeleccionada = null;
                
                var frm = new FrmOrdenServicio
                {
                    Owner = Window.GetWindow(this)
                };
                
                if (frm.ShowDialog() == true)
                {
                    // La recarga ya se realiza al guardar o al cerrar
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

                var frm = new FrmOrdenServicio(vm.OrdenSeleccionada)
                {
                    Owner = Window.GetWindow(this)
                };
                frm.ShowDialog();
            }
        }

        private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OrdenServicioViewModel vm)
            {
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
                    var estadoAnterior = vm.OrdenSeleccionada.Estado;
                    vm.Estado = "Cancelado";
                    var ok = await vm.GuardarOrdenAsync();
                    if(!ok)
                    {
                        vm.Estado = estadoAnterior;
                    }
                }
            }
        }
    }
}
