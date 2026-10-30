// walterg20
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TP_ControlVehicular.Presentacion.Pantalla.Compartido;
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

        /// <summary>
        /// Ejecuta la busqueda en la base de datos (boton Buscar).
        /// </summary>
        private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ModeloViewModel vmModelo)
            {
                vmModelo.CancelarBusquedaProgramada();
                await vmModelo.BuscarAsync();
            }
        }

        /// <summary>
        /// Busca con retardo mientras el usuario escribe, para no consultar la base de datos
        /// en cada tecla.
        /// </summary>
        private void TxtBusqueda_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.DataContext is ModeloViewModel vmModelo)
            {
                vmModelo.ProgramarBusqueda();
            }
        }

        /// <summary>
        /// La tecla Enter ejecuta la busqueda de inmediato.
        /// </summary>
        private async void TxtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (this.DataContext is ModeloViewModel vmModelo)
            {
                vmModelo.CancelarBusquedaProgramada();
                await vmModelo.BuscarAsync();
            }
        }

        /// <summary>
        /// Cambiar la visibilidad de los registros inactivos recarga el listado.
        /// </summary>
        private async void ChkMostrarInactivos_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ModeloViewModel vmModelo)
            {
                vmModelo.CancelarBusquedaProgramada();
                await vmModelo.BuscarAsync();
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
                    FrmConfirmacion.MostrarAviso("Por favor, selecciona primero un modelo.", "Aviso", Window.GetWindow(this));
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
                    FrmConfirmacion.MostrarAviso("Por favor, selecciona primero un modelo para eliminar.", "Aviso", Window.GetWindow(this));
                    return;
                }

                if (!FrmConfirmacion.Mostrar($"¿Está seguro que desea eliminar el modelo '{vmModelo.ModeloSeleccionado.NombreModelo}'?", "Confirmar Eliminación", Window.GetWindow(this)))
                    return;

                var ok = await vmModelo.EliminarModeloAsync(vmModelo.ModeloSeleccionado.Id);
                if (ok)
                {
                    FrmConfirmacion.MostrarAviso("Modelo eliminado correctamente.", "Éxito", Window.GetWindow(this), icono: "✅");
                }
                else
                {
                    FrmConfirmacion.MostrarAviso("No se pudo eliminar el modelo (puede tener vehículos asociados).", "Error", Window.GetWindow(this), icono: "❌");
                }
            }
        }
    }
}
