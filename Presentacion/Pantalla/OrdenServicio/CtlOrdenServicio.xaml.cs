// walterg20
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
                if (App.ServiceProvider?.GetService(typeof(OrdenServicioViewModel)) is OrdenServicioViewModel vmOrdenServicio)
                {
                    this.DataContext = vmOrdenServicio;
                    vmOrdenServicio.MostrarComprobanteOrdenRequested -= Vm_MostrarComprobanteOrdenRequested;
                    vmOrdenServicio.MostrarComprobanteOrdenRequested += Vm_MostrarComprobanteOrdenRequested;
                    vmOrdenServicio.MostrarComprobantePagoRequested -= Vm_MostrarComprobantePagoRequested;
                    vmOrdenServicio.MostrarComprobantePagoRequested += Vm_MostrarComprobantePagoRequested;
                    vmOrdenServicio.NotificarPermisos();
                    await vmOrdenServicio.LoadAsync();
                }
            }
            catch
            {
                // Manejar error inicial de carga si ocurre
            }
        }

        private void Vm_MostrarComprobanteOrdenRequested(object? sender, TP_ControlVehicular.Negocio.DTOs.Reportes.ComprobanteOrdenDto e)
        {
            var reporteService = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<TP_ControlVehicular.Negocio.Reportes.IReporteService>(App.ServiceProvider);
            var pdfBytes = reporteService.GenerarComprobanteRecepcion(e);
            string tempFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"ComprobanteRecepcion_{e.OrdenId}_{DateTime.Now.Ticks}.pdf");
            System.IO.File.WriteAllBytes(tempFile, pdfBytes);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(tempFile) { UseShellExecute = true });
        }

        private void Vm_MostrarComprobantePagoRequested(object? sender, TP_ControlVehicular.Negocio.DTOs.Reportes.ComprobantePagoDto e)
        {
            var reporteService = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<TP_ControlVehicular.Negocio.Reportes.IReporteService>(App.ServiceProvider);
            var pdfBytes = reporteService.GenerarComprobantePago(e);
            string tempFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"ComprobantePago_{e.OrdenId}_{DateTime.Now.Ticks}.pdf");
            System.IO.File.WriteAllBytes(tempFile, pdfBytes);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(tempFile) { UseShellExecute = true });
        }

        private void BtnNuevaOrden_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OrdenServicioViewModel vmOrdenServicio)
            {
                if (!vmOrdenServicio.PuedeCrear) { Compartido.FrmConfirmacion.MostrarAviso("No tiene permisos para crear órdenes.", "Acceso Denegado", Window.GetWindow(this)); return; }
                // Limpiar selección actual si la hubiera
                vmOrdenServicio.OrdenSeleccionada = null;

                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.AgregarPagina(new CtlOrdenServicioForm());
                }
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OrdenServicioViewModel vmOrdenServicio)
            {
                if (vmOrdenServicio.OrdenSeleccionada == null)
                {
                    Compartido.FrmConfirmacion.MostrarAviso("Por favor, seleccione una orden de servicio para editar.", "Aviso", Window.GetWindow(this));
                    return;
                }

                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.AgregarPagina(new CtlOrdenServicioForm(vmOrdenServicio.OrdenSeleccionada));
                }
            }
        }

        private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OrdenServicioViewModel vmOrdenServicio)
            {
                if (!vmOrdenServicio.PuedeEliminar) { Compartido.FrmConfirmacion.MostrarAviso("No tiene permisos para eliminar órdenes.", "Acceso Denegado", Window.GetWindow(this)); return; }
                if (vmOrdenServicio.OrdenSeleccionada == null)
                {
                    Compartido.FrmConfirmacion.MostrarAviso("Por favor, seleccione una orden de servicio.", "Aviso", Window.GetWindow(this));
                    return;
                }

                var confirmacion = Compartido.FrmConfirmacion.Mostrar(
                    $"¿Está seguro de querer cancelar la orden #{vmOrdenServicio.OrdenSeleccionada.Id} del vehículo {vmOrdenServicio.OrdenSeleccionada.VehiculoPatente}?",
                    "Confirmación", Window.GetWindow(this));

                if (confirmacion)
                {
                    var ok = await vmOrdenServicio.CancelarOrdenSeleccionadaAsync();
                }
            }
        }

        private void BtnPagar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OrdenServicioViewModel vmOrdenServicio)
            {
                if (vmOrdenServicio.OrdenSeleccionada == null)
                {
                    Compartido.FrmConfirmacion.MostrarAviso("Por favor, seleccione una orden de servicio para pagar.", "Aviso", Window.GetWindow(this));
                    return;
                }

                decimal total = System.Linq.Enumerable.Sum(vmOrdenServicio.OrdenSeleccionada.Detalles ?? new System.Collections.Generic.List<Negocio.DTOs.DetalleServicioDto>(), d => d.Precio * d.Cantidad);

                var facturacionService = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<Negocio.Interfaces.IFacturacionService>(App.ServiceProvider);
                var frmPago = new FrmPago(facturacionService, vmOrdenServicio.OrdenSeleccionada.Id, total);
                frmPago.Owner = Window.GetWindow(this);
                if (frmPago.ShowDialog() == true)
                {
                    _ = vmOrdenServicio.LoadAsync();
                }
            }
        }

        private void BtnComprobanteOrden_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OrdenServicioViewModel vmOrdenServicio)
            {
                if (vmOrdenServicio.OrdenSeleccionada == null)
                {
                    Compartido.FrmConfirmacion.MostrarAviso("Por favor, seleccione una orden de servicio.", "Aviso", Window.GetWindow(this));
                    return;
                }
                vmOrdenServicio.GenerarComprobanteOrdenCommand.Execute(null);
            }
        }

        private void BtnComprobantePago_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OrdenServicioViewModel vmOrdenServicio)
            {
                if (vmOrdenServicio.OrdenSeleccionada == null)
                {
                    Compartido.FrmConfirmacion.MostrarAviso("Por favor, seleccione una orden de servicio.", "Aviso", Window.GetWindow(this));
                    return;
                }

                vmOrdenServicio.GenerarComprobantePagoCommand.Execute(null);
            }
        }

        /// <summary>Ejecuta la consulta con los filtros aplicados.</summary>
        private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OrdenServicioViewModel vmOrdenServicio)
            {
                vmOrdenServicio.CancelarBusquedaProgramada();
                await vmOrdenServicio.BuscarAsync();
            }
        }

        /// <summary>Busca con retardo mientras el usuario escribe.</summary>
        private void TxtBusqueda_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded || DataContext is not OrdenServicioViewModel vmOrdenServicio) return;
            vmOrdenServicio.ProgramarBusqueda();
        }

        /// <summary>La tecla Enter ejecuta la busqueda de inmediato.</summary>
        private async void TxtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter || DataContext is not OrdenServicioViewModel vmOrdenServicio) return;
            vmOrdenServicio.CancelarBusquedaProgramada();
            await vmOrdenServicio.BuscarAsync();
        }

        /// <summary>Recarga el listado al cambiar el rango de fechas.</summary>
        private void DtpFechaDesde_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded || DataContext is not OrdenServicioViewModel vmOrdenServicio) return;
            vmOrdenServicio.ProgramarBusqueda();
        }

        /// <summary>Recarga el listado al cambiar el rango de fechas.</summary>
        private void DtpFechaHasta_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded || DataContext is not OrdenServicioViewModel vmOrdenServicio) return;
            vmOrdenServicio.ProgramarBusqueda();
        }

        /// <summary>Recarga el listado al cambiar el filtro de estado.</summary>
        private void CboEstado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded || DataContext is not OrdenServicioViewModel vmOrdenServicio) return;
            vmOrdenServicio.ProgramarBusqueda();
        }

        private void CmbFiltroActivo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // El setter de FiltroActivo ya recarga el listado; nada mas que hacer.
        }
    }
}
