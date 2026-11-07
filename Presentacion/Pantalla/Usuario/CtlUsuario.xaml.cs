// walterg20
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TP_ControlVehicular.Presentacion.Pantalla.Compartido;
using TP_ControlVehicular.Presentacion.ViewModels;
using TP_ControlVehicular.Negocio.Services;

namespace TP_ControlVehicular.Presentacion.Usuario
{
    public partial class CtlUsuario : UserControl
    {
        public UsuarioViewModel? ViewModel => DataContext as UsuarioViewModel;

        public CtlUsuario()
        {
            InitializeComponent();
            try
            {
                var vmUsuario = App.ServiceProvider.GetService(typeof(UsuarioViewModel)) as UsuarioViewModel;
                if (vmUsuario is not null)
                {
                    this.DataContext = vmUsuario;
                    this.Loaded += async (s, e) =>
                    {
                        vmUsuario.NotificarPermisos();
                        await vmUsuario.LoadAsync();
                    };
                }
            }
            catch (Exception ex)
            {
                (App.ServiceProvider?.GetService(typeof(IErrorNotifier)) as IErrorNotifier)?.Notify(ex, "CtlUsuario.xaml");
                // Ignorar si DI no está listo
            }
        }

        public CtlUsuario(UsuarioViewModel viewModel) : this()
        {
            DataContext = viewModel;
            ViewModel?.NotificarPermisos();
        }

        /// <summary>
        /// Ejecuta la busqueda en la base de datos (boton Buscar).
        /// </summary>
        private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is UsuarioViewModel vmUsuario)
            {
                vmUsuario.CancelarBusquedaProgramada();
                await vmUsuario.BuscarAsync();
            }
        }

        /// <summary>
        /// Busca con retardo mientras el usuario escribe, para no consultar la base de datos
        /// en cada tecla.
        /// </summary>
        private void TxtBusqueda_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.DataContext is UsuarioViewModel vmUsuario)
            {
                vmUsuario.ProgramarBusqueda();
            }
        }

        /// <summary>
        /// La tecla Enter ejecuta la busqueda de inmediato.
        /// </summary>
        private async void TxtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (this.DataContext is UsuarioViewModel vmUsuario)
            {
                vmUsuario.CancelarBusquedaProgramada();
                await vmUsuario.BuscarAsync();
            }
        }

        /// <summary>
        /// Cambiar el filtro de estado recarga el listado.
        /// </summary>
        private async void CmbFiltroEstado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.DataContext is UsuarioViewModel vmUsuario)
            {
                vmUsuario.CancelarBusquedaProgramada();
                await vmUsuario.BuscarAsync();
            }
        }

        private void DgUsuarios_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel is UsuarioViewModel vmUsuario && !vmUsuario.PuedeEditar)
            {
                return;
            }
            BtnEditar_Click(sender, e);
        }

        private async void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel is UsuarioViewModel vmUsuario)
            {
                if (!vmUsuario.PuedeCrear)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para registrar nuevos usuarios.", "Acceso denegado", Window.GetWindow(this));
                    return;
                }

                var frm = new FrmUsuario();
                frm.Owner = Window.GetWindow(this);
                var ok = frm.ShowDialog();
                if (ok == true)
                {
                    await vmUsuario.LoadAsync();
                }
            }
        }

        private async void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is UsuarioViewModel vmUsuario)
            {
                if (!vmUsuario.PuedeEditar)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para modificar usuarios.", "Acceso denegado", Window.GetWindow(this));
                    return;
                }

                if (vmUsuario.UsuarioSeleccionado == null)
                {
                    FrmConfirmacion.MostrarAviso("Por favor, selecciona primero un usuario.", "Aviso", Window.GetWindow(this));
                    return;
                }

                var frm = new FrmUsuario(vmUsuario.UsuarioSeleccionado);
                frm.Owner = Window.GetWindow(this);
                var ok = frm.ShowDialog();
                if (ok == true)
                {
                    await vmUsuario.LoadAsync();
                }
            }
        }

        private async void BtnBorrar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is UsuarioViewModel vmUsuario)
            {
                if (!vmUsuario.PuedeEliminar)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para eliminar usuarios.", "Acceso denegado", Window.GetWindow(this));
                    return;
                }

                if (vmUsuario.UsuarioSeleccionado == null)
                {
                    FrmConfirmacion.MostrarAviso("Por favor, selecciona primero un usuario.", "Aviso", Window.GetWindow(this));
                    return;
                }

                if (!FrmConfirmacion.Mostrar($"¿Está seguro de marcar como inactivo al usuario '{vmUsuario.UsuarioSeleccionado.Nombre} {vmUsuario.UsuarioSeleccionado.Apellido}'?", "Confirmar baja", Window.GetWindow(this)))
                    return;

                await vmUsuario.EliminarUsuarioAsync(vmUsuario.UsuarioSeleccionado.IdUsuario);
            }
        }

        private async void BtnReactivar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is UsuarioViewModel vmUsuario)
            {
                if (!vmUsuario.PuedeEditar)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para reactivar usuarios.", "Acceso denegado", Window.GetWindow(this));
                    return;
                }

                if (vmUsuario.UsuarioSeleccionado == null)
                {
                    FrmConfirmacion.MostrarAviso("Por favor, selecciona primero un usuario.", "Aviso", Window.GetWindow(this));
                    return;
                }

                var ok = await vmUsuario.ReactivarUsuarioAsync(vmUsuario.UsuarioSeleccionado.IdUsuario);
                if (ok)
                {
                    FrmConfirmacion.MostrarAviso("Usuario reactivado correctamente.", "Reactivar", Window.GetWindow(this));
                }
            }
        }
    }
}
