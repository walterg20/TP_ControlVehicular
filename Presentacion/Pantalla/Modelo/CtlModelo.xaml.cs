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
        public ModeloViewModel? ViewModel => DataContext as ModeloViewModel;

        public CtlModelo()
        {
            InitializeComponent();
            try
            {
                if (App.ServiceProvider != null)
                {
                    var vmModelo = App.ServiceProvider.GetService(typeof(ModeloViewModel)) as ModeloViewModel;
                    if (vmModelo != null)
                    {
                        DataContext = vmModelo;
                    }
                }
            }
            catch
            {
                // Silenciar si DI no está listo
            }

            Loaded += async (s, e) =>
            {
                if (ViewModel != null)
                {
                    ViewModel.NotificarPermisos();
                    await ViewModel.LoadAsync();
                }
            };
        }

        public CtlModelo(ModeloViewModel viewModel) : this()
        {
            DataContext = viewModel;
            ViewModel?.NotificarPermisos();
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
        private async void CmbFiltroEstado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.DataContext is ModeloViewModel vmModelo)
            {
                vmModelo.CancelarBusquedaProgramada();
                await vmModelo.BuscarAsync();
            }
        }

        private async void BtnReactivar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ModeloViewModel vmModelo && vmModelo.ModeloSeleccionado != null)
            {
                var ok = await vmModelo.ReactivarModeloAsync(vmModelo.ModeloSeleccionado.Id);
                if (ok)
                {
                    Presentacion.Pantalla.Compartido.FrmConfirmacion.MostrarAviso("Modelo reactivado correctamente.", "Reactivar", Window.GetWindow(this));
                }
            }
            else
            {
                Presentacion.Pantalla.Compartido.FrmConfirmacion.MostrarAviso("Por favor, seleccione un modelo.", "Aviso", Window.GetWindow(this));
            }
        }

        private void DgModelos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel is ModeloViewModel vmModelo && !vmModelo.PuedeEditar)
            {
                return;
            }
            BtnEditar_Click(sender, e);
        }

        private async void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel is ModeloViewModel vmModelo)
            {
                if (!vmModelo.PuedeCrear)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para registrar nuevos modelos.", "Acceso denegado", Window.GetWindow(this));
                    return;
                }

                var frm = new FrmModelo();
                frm.Owner = Window.GetWindow(this);
                var ok = frm.ShowDialog();
                if (ok == true)
                {
                    await vmModelo.LoadAsync();
                }
            }
        }

        private async void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel is ModeloViewModel vmModelo)
            {
                if (!vmModelo.PuedeEditar)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para modificar modelos.", "Acceso denegado", Window.GetWindow(this));
                    return;
                }

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
            if (ViewModel is ModeloViewModel vmModelo)
            {
                if (!vmModelo.PuedeEliminar)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para eliminar modelos.", "Acceso denegado", Window.GetWindow(this));
                    return;
                }

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
