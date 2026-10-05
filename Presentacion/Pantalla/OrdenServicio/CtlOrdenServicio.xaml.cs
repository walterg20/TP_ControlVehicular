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
                    vm.MostrarComprobanteOrdenRequested -= Vm_MostrarComprobanteOrdenRequested;
                    vm.MostrarComprobanteOrdenRequested += Vm_MostrarComprobanteOrdenRequested;
                    vm.MostrarComprobantePagoRequested -= Vm_MostrarComprobantePagoRequested;
                    vm.MostrarComprobantePagoRequested += Vm_MostrarComprobantePagoRequested;
                    await vm.LoadAsync();
                }
            }
            catch
            {
                // Manejar error inicial de carga si ocurre
            }
        }

        private void Vm_MostrarComprobanteOrdenRequested(object? sender, TP_ControlVehicular.Negocio.DTOs.Reportes.ComprobanteOrdenDto e)
        {
            var reporteService = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<TP_ControlVehicular.Negocio.Servicios.Reportes.IReporteService>(App.ServiceProvider);
            var pdfBytes = reporteService.GenerarComprobanteRecepcion(e);
            string tempFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"ComprobanteRecepcion_{e.OrdenId}.pdf");
            System.IO.File.WriteAllBytes(tempFile, pdfBytes);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(tempFile) { UseShellExecute = true });
        }

        private void Vm_MostrarComprobantePagoRequested(object? sender, TP_ControlVehicular.Negocio.DTOs.Reportes.ComprobantePagoDto e)
        {
            var reporteService = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<TP_ControlVehicular.Negocio.Servicios.Reportes.IReporteService>(App.ServiceProvider);
            var pdfBytes = reporteService.GenerarComprobantePago(e);
            string tempFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"ComprobantePago_{e.OrdenId}.pdf");
            System.IO.File.WriteAllBytes(tempFile, pdfBytes);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(tempFile) { UseShellExecute = true });
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

        private void BtnPagar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OrdenServicioViewModel vm)
            {
                if (vm.OrdenSeleccionada == null)
                {
                    Compartido.FrmConfirmacion.MostrarAviso("Por favor, seleccione una orden de servicio para pagar.", "Aviso", Window.GetWindow(this));
                    return;
                }

                decimal total = System.Linq.Enumerable.Sum(vm.OrdenSeleccionada.Detalles ?? new System.Collections.Generic.List<Negocio.DTOs.DetalleServicioDto>(), d => d.Precio * d.Cantidad);

                var billingService = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<Negocio.Interfaces.IBillingService>(App.ServiceProvider);
                var frmPago = new FrmPago(billingService, vm.OrdenSeleccionada.Id, total);
                frmPago.Owner = Window.GetWindow(this);
                if (frmPago.ShowDialog() == true)
                {
                    _ = vm.LoadAsync();
                }
            }
        }

        private void BtnComprobanteOrden_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OrdenServicioViewModel vm)
            {
                if (vm.OrdenSeleccionada == null)
                {
                    Compartido.FrmConfirmacion.MostrarAviso("Por favor, seleccione una orden de servicio.", "Aviso", Window.GetWindow(this));
                    return;
                }
                vm.GenerarComprobanteOrdenCommand.Execute(null);
            }
        }

        private void BtnComprobantePago_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OrdenServicioViewModel vm)
            {
                if (vm.OrdenSeleccionada == null)
                {
                    Compartido.FrmConfirmacion.MostrarAviso("Por favor, seleccione una orden de servicio.", "Aviso", Window.GetWindow(this));
                    return;
                }

                vm.GenerarComprobantePagoCommand.Execute(null);
            }
        }
    }
}
