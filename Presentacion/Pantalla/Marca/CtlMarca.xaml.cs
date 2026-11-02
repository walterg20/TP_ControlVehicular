// walterg20
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TP_ControlVehicular.Presentacion.Pantalla.Compartido;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Marca
{
    public partial class CtlMarca : UserControl
    {
        public MarcaViewModel? ViewModel => DataContext as MarcaViewModel;

        public CtlMarca()
        {
            InitializeComponent();
            try
            {
                if (App.ServiceProvider != null)
                {
                    var vmMarca = App.ServiceProvider.GetService(typeof(MarcaViewModel)) as MarcaViewModel;
                    if (vmMarca != null)
                    {
                        DataContext = vmMarca;
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

        public CtlMarca(MarcaViewModel viewModel) : this()
        {
            DataContext = viewModel;
            ViewModel?.NotificarPermisos();
        }

        /// <summary>
        /// Ejecuta la busqueda en la base de datos (boton Buscar).
        /// </summary>
        private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is MarcaViewModel vmMarca)
            {
                vmMarca.CancelarBusquedaProgramada();
                await vmMarca.BuscarAsync();
            }
        }

        /// <summary>
        /// Busca con retardo mientras el usuario escribe, para no consultar la base de datos
        /// en cada tecla.
        /// </summary>
        private void TxtBusqueda_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.DataContext is MarcaViewModel vmMarca)
            {
                vmMarca.ProgramarBusqueda();
            }
        }

        /// <summary>
        /// La tecla Enter ejecuta la busqueda de inmediato.
        /// </summary>
        private async void TxtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (this.DataContext is MarcaViewModel vmMarca)
            {
                vmMarca.CancelarBusquedaProgramada();
                await vmMarca.BuscarAsync();
            }
        }

        /// <summary>
        /// Cambiar la visibilidad de los registros inactivos recarga el listado.
        /// </summary>
        private async void ChkMostrarInactivos_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is MarcaViewModel vmMarca)
            {
                vmMarca.CancelarBusquedaProgramada();
                await vmMarca.BuscarAsync();
            }
        }

        private void DgMarcas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel is MarcaViewModel vmMarca && !vmMarca.PuedeEditar)
            {
                return;
            }
            BtnEditar_Click(sender, e);
        }

        private async void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel is MarcaViewModel vmMarca)
            {
                if (!vmMarca.PuedeCrear)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para registrar nuevas marcas.", "Acceso denegado", Window.GetWindow(this));
                    return;
                }

                var frm = new FrmMarca();
                frm.Owner = Window.GetWindow(this);
                var ok = frm.ShowDialog();
                if (ok == true)
                {
                    await vmMarca.LoadAsync();
                }
            }
        }

        private async void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel is MarcaViewModel vmMarca)
            {
                if (!vmMarca.PuedeEditar)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para modificar marcas.", "Acceso denegado", Window.GetWindow(this));
                    return;
                }

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
            if (ViewModel is MarcaViewModel vmMarca)
            {
                if (!vmMarca.PuedeEliminar)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para eliminar marcas.", "Acceso denegado", Window.GetWindow(this));
                    return;
                }

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
