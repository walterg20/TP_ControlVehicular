// walterg20
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TP_ControlVehicular.Presentacion.Pantalla.Compartido;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Usuario
{
    public partial class CtlUsuario : UserControl
    {
        public CtlUsuario()
        {
            InitializeComponent();
            try
            {
                var vmUsuario = App.ServiceProvider.GetService(typeof(UsuarioViewModel)) as UsuarioViewModel;
                if (vmUsuario is not null)
                {
                    this.DataContext = vmUsuario;
                    this.Loaded += async (s, e) => { await vmUsuario.LoadAsync(); };
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
        /// Cambiar la visibilidad de los registros inactivos recarga el listado.
        /// </summary>
        private async void ChkMostrarInactivos_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is UsuarioViewModel vmUsuario)
            {
                vmUsuario.CancelarBusquedaProgramada();
                await vmUsuario.BuscarAsync();
            }
        }

        private async void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            var frm = new FrmUsuario();
            frm.Owner = Window.GetWindow(this);
            var ok = frm.ShowDialog();
            if (ok == true && this.DataContext is UsuarioViewModel vmUsuario)
            {
                await vmUsuario.LoadAsync();
            }
        }

        private async void BtnModificar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is UsuarioViewModel vmUsuario)
            {
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

        private async void BtnBaja_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is UsuarioViewModel vmUsuario)
            {
                if (vmUsuario.UsuarioSeleccionado == null)
                {
                    FrmConfirmacion.MostrarAviso("Por favor, selecciona primero un usuario.", "Aviso", Window.GetWindow(this));
                    return;
                }

                if (!FrmConfirmacion.Mostrar("¿Cambiar el estado del usuario seleccionado?", "Confirmar cambio", Window.GetWindow(this)))
                    return;

                await vmUsuario.ToggleActivoAsync();
                await vmUsuario.LoadAsync();
            }
        }
    }
}
