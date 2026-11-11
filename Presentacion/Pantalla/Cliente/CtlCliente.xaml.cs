// walterg20
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Presentacion.Pantalla.Compartido;
using TP_ControlVehicular.Presentacion.ViewModels;
using TP_ControlVehicular.Negocio.Services;

namespace TP_ControlVehicular.Presentacion.Cliente
{
    /// <summary>
    /// Lógica de interacción para CtlCliente.xaml
    /// </summary>
    public partial class CtlCliente : UserControl
    {
        public ClienteViewModel? ViewModel => DataContext as ClienteViewModel;

        public CtlCliente()
        {
            InitializeComponent();
            try
            {
                if (App.ServiceProvider != null)
                {
                    var vmCliente = App.ServiceProvider.GetService(typeof(ClienteViewModel)) as ClienteViewModel;
                    if (vmCliente != null)
                    {
                        DataContext = vmCliente;
                    }
                }
            }
            catch (Exception ex)
            {
                (App.ServiceProvider?.GetService(typeof(IErrorNotifier)) as IErrorNotifier)?.Notify(ex, "CtlCliente.xaml");
                // ignore DI resolution errors
            }

            Loaded += async (s, e) =>
            {
                if (ViewModel != null)
                {
                    ViewModel.NotificarPermisos();
                    await ViewModel.LoadAsync();
                }
            };
        }

        public CtlCliente(ClienteViewModel viewModel) : this()
        {
            DataContext = viewModel;
            ViewModel?.NotificarPermisos();
        }

        /// <summary>
        /// Ejecuta la busqueda en la base de datos (boton Buscar).
        /// </summary>
        private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ClienteViewModel vmCliente)
            {
                vmCliente.CancelarBusquedaProgramada();
                await vmCliente.BuscarAsync();
            }
        }

        /// <summary>
        /// Busca con retardo mientras el usuario escribe, para no consultar la base de datos
        /// en cada tecla.
        /// </summary>
        private void TxtBusqueda_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.DataContext is ClienteViewModel vmCliente)
            {
                vmCliente.ProgramarBusqueda();
            }
        }

        /// <summary>
        /// La tecla Enter ejecuta la busqueda de inmediato.
        /// </summary>
        private async void TxtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (this.DataContext is ClienteViewModel vmCliente)
            {
                vmCliente.CancelarBusquedaProgramada();
                await vmCliente.BuscarAsync();
            }
        }

        /// <summary>
        /// Cambiar el filtro de estado recarga el listado.
        /// </summary>
        private async void CmbFiltroEstado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.DataContext is ClienteViewModel vmCliente)
            {
                vmCliente.CancelarBusquedaProgramada();
                await vmCliente.BuscarAsync();
            }
        }

        private void DgClientes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel is ClienteViewModel vmCliente && !vmCliente.PuedeEditar)
            {
                return;
            }
            BtnEditar_Click(sender, e);
        }

        private async void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel is ClienteViewModel vmCliente)
            {
                if (!vmCliente.PuedeCrear)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para registrar nuevos clientes.", "Acceso denegado", Window.GetWindow(this));
                    return;
                }

                var frm = new FrmCliente();
                frm.Owner = Window.GetWindow(this);
                var resultado = frm.ShowDialog();
                if (resultado == true)
                {
                    await vmCliente.LoadAsync();
                }
            }
        }

        private async void BtnBorrar_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel is ClienteViewModel vmCliente)
            {
                if (!vmCliente.PuedeEliminar)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para eliminar clientes.", "Acceso denegado", Window.GetWindow(this));
                    return;
                }

                var selected = vmCliente.ClienteSeleccionado ?? dgClientes.SelectedItem as ClienteDto;
                if (selected is null)
                {
                    FrmConfirmacion.MostrarAviso("Por favor, selecciona primero un cliente.", "Aviso", Window.GetWindow(this));
                    return;
                }

                if (!FrmConfirmacion.Mostrar($"¿Seguro que querés marcar como inactivo al cliente {selected.Nombre} {selected.Apellido}?", "Confirmar baja", Window.GetWindow(this)))
                    return;

                await vmCliente.DeleteClienteAsync(selected.IdCliente);
            }
        }

        private async void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel is ClienteViewModel vmCliente)
            {
                if (!vmCliente.PuedeEditar)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para modificar clientes.", "Acceso denegado", Window.GetWindow(this));
                    return;
                }

                if (vmCliente.ClienteSeleccionado is null)
                {
                    FrmConfirmacion.MostrarAviso("Por favor, selecciona primero un cliente.", "Aviso", Window.GetWindow(this));
                    return;
                }

                var frm = new FrmCliente(vmCliente.ClienteSeleccionado);
                frm.Owner = Window.GetWindow(this);
                var ok = frm.ShowDialog();
                if (ok == true)
                {
                    await vmCliente.LoadAsync();
                }
            }
        }

        private async void BtnReactivar_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel is ClienteViewModel vmCliente)
            {
                if (!vmCliente.PuedeEditar)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para reactivar clientes.", "Acceso denegado", Window.GetWindow(this));
                    return;
                }

                var selected = vmCliente.ClienteSeleccionado ?? dgClientes.SelectedItem as ClienteDto;
                if (selected is null)
                {
                    FrmConfirmacion.MostrarAviso("Por favor, selecciona primero un cliente.", "Aviso", Window.GetWindow(this));
                    return;
                }

                var ok = await vmCliente.ReactivarClienteAsync(selected.IdCliente);
                if (ok)
                {
                    FrmConfirmacion.MostrarAviso("Cliente reactivado correctamente.", "Reactivar", Window.GetWindow(this));
                }
            }
        }
    }
}
