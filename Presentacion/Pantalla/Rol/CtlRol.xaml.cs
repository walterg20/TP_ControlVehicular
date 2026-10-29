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
        public CtlRol()
        {
            InitializeComponent();
            try
            {
                var vmRol = App.ServiceProvider.GetService(typeof(RolViewModel)) as RolViewModel;
                if (vmRol is not null)
                {
                    this.DataContext = vmRol;
                    this.Loaded += async (s, e) => { await vmRol.LoadAsync(); };
                }
            }
            catch
            {
                // Ignorar si DI no está listo
            }
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
        /// Cambiar la visibilidad de los registros inactivos recarga el listado.
        /// </summary>
        private async void ChkMostrarInactivos_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is RolViewModel vmRol)
            {
                vmRol.CancelarBusquedaProgramada();
                await vmRol.BuscarAsync();
            }
        }

        private async void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            var frm = new FrmRol();
            frm.Owner = Window.GetWindow(this);
            var ok = frm.ShowDialog();
            if (ok == true && this.DataContext is RolViewModel vmRol)
            {
                await vmRol.LoadAsync();
            }
        }

        private async void BtnModificar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is RolViewModel vmRol)
            {
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

        private async void BtnBaja_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is RolViewModel vmRol)
            {
                if (vmRol.RolSeleccionado == null)
                {
                    FrmConfirmacion.MostrarAviso("Por favor, selecciona primero un rol.", "Aviso", Window.GetWindow(this));
                    return;
                }

                if (!FrmConfirmacion.Mostrar("¿Cambiar el estado del rol seleccionado?", "Confirmar cambio", Window.GetWindow(this)))
                    return;

                await vmRol.ToggleActivoAsync();
                await vmRol.LoadAsync();
            }
        }

        private async void BtnReactivar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is RolViewModel vmRol)
            {
                if (vmRol.RolSeleccionado == null)
                {
                    FrmConfirmacion.MostrarAviso("Por favor, selecciona primero un rol.", "Aviso", Window.GetWindow(this));
                    return;
                }

                await vmRol.ReactivarAsync(vmRol.RolSeleccionado.IdRol);
            }
        }
    }
}
