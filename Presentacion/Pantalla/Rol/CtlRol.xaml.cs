// walterg20
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TP_ControlVehicular.Presentacion.Pantalla.Compartido;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Rol
{
    public partial class CtlRol : UserControl
    {
        public RolViewModel? ViewModel => DataContext as RolViewModel;

        public CtlRol()
        {
            InitializeComponent();
            try
            {
                var vmRol = App.ServiceProvider.GetService(typeof(RolViewModel)) as RolViewModel;
                if (vmRol is not null)
                {
                    this.DataContext = vmRol;
                    this.Loaded += async (s, e) =>
                    {
                        vmRol.NotificarPermisos();
                        await vmRol.LoadAsync();
                    };
                }
            }
            catch
            {
                // Ignorar si DI no está listo
            }
        }

        public CtlRol(RolViewModel viewModel) : this()
        {
            DataContext = viewModel;
            ViewModel?.NotificarPermisos();
        }

        /// <summary>
        /// Ejecuta la busqueda en la base de datos (boton Buscar).
        /// </summary>
        private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is RolViewModel vmRol)
            {
                vmRol.CancelarBusquedaProgramada();
                await vmRol.BuscarAsync();
            }
        }

        /// <summary>
        /// Busca con retardo mientras el usuario escribe, para no consultar la base de datos
        /// en cada tecla.
        /// </summary>
        private void TxtBusqueda_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.DataContext is RolViewModel vmRol)
            {
                vmRol.ProgramarBusqueda();
            }
        }

        /// <summary>
        /// La tecla Enter ejecuta la busqueda de inmediato.
        /// </summary>
        private async void TxtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (this.DataContext is RolViewModel vmRol)
            {
                vmRol.CancelarBusquedaProgramada();
                await vmRol.BuscarAsync();
            }
        }

        /// <summary>
        /// Cambiar el filtro de estado recarga el listado.
        /// </summary>
        private async void CmbFiltroEstado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.DataContext is RolViewModel vmRol)
            {
                vmRol.CancelarBusquedaProgramada();
                await vmRol.BuscarAsync();
            }
        }

        private void DgRoles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel is RolViewModel vmRol && !vmRol.PuedeEditar)
            {
                return;
            }
            BtnEditar_Click(sender, e);
        }

        private async void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel is RolViewModel vmRol)
            {
                if (!vmRol.PuedeCrear)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para registrar nuevos roles.", "Acceso denegado", Window.GetWindow(this));
                    return;
                }

                var frm = new FrmRol();
                frm.Owner = Window.GetWindow(this);
                var ok = frm.ShowDialog();
                if (ok == true)
                {
                    await vmRol.LoadAsync();
                }
            }
        }

        private async void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is RolViewModel vmRol)
            {
                if (!vmRol.PuedeEditar)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para modificar roles.", "Acceso denegado", Window.GetWindow(this));
                    return;
                }

                if (vmRol.RolSeleccionado == null)
                {
                    FrmConfirmacion.MostrarAviso("Por favor, selecciona primero un rol.", "Aviso", Window.GetWindow(this));
                    return;
                }

                var frm = new FrmRol(vmRol.RolSeleccionado);
                frm.Owner = Window.GetWindow(this);
                var ok = frm.ShowDialog();
                if (ok == true)
                {
                    await vmRol.LoadAsync();
                }
            }
        }

        private void BtnAdministrarPermisos_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is RolViewModel vmRol)
            {
                var rolSeleccionado = vmRol.RolSeleccionado;
                if (rolSeleccionado == null)
                {
                    FrmConfirmacion.MostrarAviso("Por favor seleccione un rol para administrar sus permisos.", "Aviso", Window.GetWindow(this));
                    return;
                }

                var frm = new TP_ControlVehicular.Presentacion.Pantalla.Rol.FrmRolPermiso(rolSeleccionado);
                if (frm.ShowDialog() == true)
                {
                    _ = vmRol.LoadAsync();
                }
            }
        }

        private async void BtnBorrar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is RolViewModel vmRol)
            {
                if (!vmRol.PuedeEliminar)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para eliminar roles.", "Acceso denegado", Window.GetWindow(this));
                    return;
                }

                if (vmRol.RolSeleccionado == null)
                {
                    FrmConfirmacion.MostrarAviso("Por favor, selecciona primero un rol.", "Aviso", Window.GetWindow(this));
                    return;
                }

                if (!FrmConfirmacion.Mostrar($"¿Está seguro de marcar como inactivo al rol '{vmRol.RolSeleccionado.Nombre}'?", "Confirmar baja", Window.GetWindow(this)))
                    return;

                await vmRol.EliminarRolAsync(vmRol.RolSeleccionado.IdRol);
            }
        }

        private async void BtnReactivar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is RolViewModel vmRol)
            {
                if (!vmRol.PuedeEditar)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para reactivar roles.", "Acceso denegado", Window.GetWindow(this));
                    return;
                }

                if (vmRol.RolSeleccionado == null)
                {
                    FrmConfirmacion.MostrarAviso("Por favor, selecciona primero un rol.", "Aviso", Window.GetWindow(this));
                    return;
                }

                var ok = await vmRol.ReactivarAsync(vmRol.RolSeleccionado.IdRol);
                if (ok)
                {
                    FrmConfirmacion.MostrarAviso("Rol reactivado correctamente.", "Reactivar", Window.GetWindow(this));
                }
            }
        }
    }
}
