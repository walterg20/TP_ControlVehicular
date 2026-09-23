using System.Windows;
using System.Windows.Controls;
using TP_ControlVehicular.Presentacion.Pantalla.Compartido;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Taller
{
    public partial class CtlTaller : UserControl
    {
        public CtlTaller()
        {
            InitializeComponent();
            try
            {
                var vmTaller = App.ServiceProvider?.GetService(typeof(TallerViewModel)) as TallerViewModel;
                if (vmTaller is not null)
                {
                    this.DataContext = vmTaller;
                    this.Loaded += async (s, e) => { await vmTaller.LoadAsync(); };
                }
            }
            catch
            {
                // Silenciar si DI no está listo
            }
        }

        private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is TallerViewModel vmTaller)
            {
                await vmTaller.LoadAsync();
            }
        }

        private async void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is TallerViewModel vmTaller)
            {
                vmTaller.LimpiarFormulario();
                var frm = new FrmTaller();
                frm.Owner = Window.GetWindow(this);
                var ok = frm.ShowDialog();
                if (ok == true)
                {
                    await vmTaller.LoadAsync();
                }
            }
        }

        private async void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is TallerViewModel vmTaller)
            {
                if (vmTaller.TallerSeleccionado == null)
                {
                    FrmConfirmacion.MostrarAviso("Por favor, selecciona primero un taller de la lista.", "Aviso", Window.GetWindow(this));
                    return;
                }

                var frm = new FrmTaller(vmTaller.TallerSeleccionado);
                frm.Owner = Window.GetWindow(this);
                var ok = frm.ShowDialog();
                if (ok == true)
                {
                    await vmTaller.LoadAsync();
                }
            }
        }

        private async void BtnBorrar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is TallerViewModel vmTaller)
            {
                if (vmTaller.TallerSeleccionado == null)
                {
                    FrmConfirmacion.MostrarAviso("Por favor, selecciona primero un taller de la lista.", "Aviso", Window.GetWindow(this));
                    return;
                }

                if (!FrmConfirmacion.Mostrar($"¿Está seguro de dar de baja (lógica) al taller '{vmTaller.TallerSeleccionado.Nombre}'?", "Confirmar baja", Window.GetWindow(this)))
                    return;

                await vmTaller.ToggleActivoAsync();
                await vmTaller.LoadAsync();
            }
        }
    }
}