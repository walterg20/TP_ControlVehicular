using System.Windows;
using System.Windows.Controls;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Modelo
{
    public partial class CtlModelo : UserControl
    {
        public CtlModelo()
        {
            InitializeComponent();
            try
            {
                if (App.ServiceProvider.GetService(typeof(ModeloViewModel)) is ModeloViewModel vmModelo)
                {
                    this.DataContext = vmModelo;
                    this.Loaded += async (s, e) => { await vmModelo.LoadAsync(); };
                }
            }
            catch
            {
                // Silenciar si DI no está listo
            }
        }

        private async void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            var frm = new FrmModelo();
            frm.Owner = Window.GetWindow(this);
            var ok = frm.ShowDialog();
            if (ok == true && this.DataContext is ModeloViewModel vmModelo)
            {
                await vmModelo.LoadAsync();
            }
        }

        private async void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ModeloViewModel vmModelo)
            {
                if (vmModelo.ModeloSeleccionado == null)
                {
                    MessageBox.Show("Por favor, selecciona primero un modelo.", "Aviso");
                    return;
                }

                var frm = new FrmModelo(vmModelo.ModeloSeleccionado);
                frm.Owner = Window.GetWindow(this);
                var ok = frm.ShowDialog();
                if (ok == true)
                {
                    await vmModelo.LoadAsync();
                }
            }
        }

        private async void BtnBorrar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ModeloViewModel vmModelo)
            {
                if (vmModelo.ModeloSeleccionado == null)
                {
                    MessageBox.Show("Por favor, selecciona primero un modelo para eliminar.", "Aviso");
                    return;
                }

                var res = MessageBox.Show($"¿Está seguro que desea eliminar el modelo '{vmModelo.ModeloSeleccionado.NombreModelo}'?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (res == MessageBoxResult.Yes)
                {
                    var ok = await vmModelo.EliminarModeloAsync(vmModelo.ModeloSeleccionado.Id);
                    if (ok)
                    {
                        MessageBox.Show("Modelo eliminado correctamente.", "Éxito");
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar el modelo (puede tener vehículos asociados).", "Error");
                    }
                }
            }
        }
    }
}
