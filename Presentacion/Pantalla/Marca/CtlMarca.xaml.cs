using System.Windows;
using System.Windows.Controls;
using TP_ControlVehicular.Presentacion.Pantalla.Compartido;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Marca
{
    public partial class CtlMarca : UserControl
    {
        public CtlMarca()
        {
            InitializeComponent();
            try
            {
                if (App.ServiceProvider.GetService(typeof(MarcaViewModel)) is MarcaViewModel vmMarca)
                {
                    this.DataContext = vmMarca;
                    this.Loaded += async (s, e) => { await vmMarca.LoadAsync(); };
                }
            }
            catch
            {
                // Silenciar si DI no está listo
            }
        }

        private async void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            var frm = new FrmMarca();
            frm.Owner = Window.GetWindow(this);
            var ok = frm.ShowDialog();
            if (ok == true && this.DataContext is MarcaViewModel vmMarca)
            {
                await vmMarca.LoadAsync();
            }
        }

        private async void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is MarcaViewModel vmMarca)
            {
                if (vmMarca.MarcaSeleccionada == null)
                {
                    FrmConfirmacion.MostrarAviso("Por favor, selecciona primero una marca.", "Aviso", Window.GetWindow(this));
                    return;
                }

                var frm = new FrmMarca(vmMarca.MarcaSeleccionada);
                frm.Owner = Window.GetWindow(this);
                var ok = frm.ShowDialog();
                if (ok == true)
                {
                    await vmMarca.LoadAsync();
                }
            }
        }

        private async void BtnBorrar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is MarcaViewModel vmMarca)
            {
                if (vmMarca.MarcaSeleccionada == null)
                {
                    FrmConfirmacion.MostrarAviso("Por favor, selecciona primero una marca para eliminar.", "Aviso", Window.GetWindow(this));
                    return;
                }

                if (!FrmConfirmacion.Mostrar($"¿Está seguro que desea eliminar la marca '{vmMarca.MarcaSeleccionada.NombreMarca}'?", "Confirmar Eliminación", Window.GetWindow(this)))
                    return;

                var ok = await vmMarca.EliminarMarcaAsync(vmMarca.MarcaSeleccionada.Id);
                if (ok)
                {
                    FrmConfirmacion.MostrarAviso("Marca eliminada correctamente.", "Éxito", Window.GetWindow(this), icono: "✅");
                }
                else
                {
                    FrmConfirmacion.MostrarAviso("No se pudo eliminar la marca (puede tener modelos asociados).", "Error", Window.GetWindow(this), icono: "❌");
                }
            }
        }
    }
}
