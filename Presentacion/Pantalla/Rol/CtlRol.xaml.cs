using System.Windows;
using System.Windows.Controls;
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

                await vmRol.ToggleEstadoAsync();
                await vmRol.LoadAsync();
            }
        }
    }
}
