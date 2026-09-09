using System.Windows;
using System.Windows.Controls;
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
                    MessageBox.Show("Por favor, selecciona primero un usuario.", "Aviso");
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
                    MessageBox.Show("Por favor, selecciona primero un usuario.", "Aviso");
                    return;
                }

                var msg = MessageBox.Show("¿Cambiar el estado del usuario seleccionado?", "Confirmar cambio", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (msg != MessageBoxResult.Yes) return;

                await vmUsuario.ToggleEstadoAsync();
                await vmUsuario.LoadAsync();
            }
        }
    }
}
