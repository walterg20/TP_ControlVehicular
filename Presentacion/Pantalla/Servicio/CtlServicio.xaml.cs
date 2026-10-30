// walterg20
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Presentacion.Pantalla.Compartido;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Pantalla.Servicio
{
    /// <summary>
    /// Interaction logic for CtlServicio.xaml
    /// </summary>
    public partial class CtlServicio : UserControl
    {
        public ServicioViewModel? ViewModel => DataContext as ServicioViewModel;

        public CtlServicio()
        {
            InitializeComponent();
            try
            {
                if (App.ServiceProvider != null)
                {
                    var vmServicio = App.ServiceProvider.GetService(typeof(ServicioViewModel)) as ServicioViewModel;
                    if (vmServicio != null)
                    {
                        DataContext = vmServicio;
                    }
                }
            }
            catch
            {
                // Fallback handled gracefully
            }

            Loaded += async (s, e) =>
            {
                if (ViewModel != null)
                {
                    await ViewModel.LoadAsync();
                }
            };
        }

        public CtlServicio(ServicioViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }

        /// <summary>
        /// Ejecuta la busqueda en la base de datos (boton Buscar).
        /// </summary>
        private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ServicioViewModel vmServicio)
            {
                vmServicio.CancelarBusquedaProgramada();
                await vmServicio.BuscarAsync();
            }
        }

        /// <summary>
        /// Busca con retardo mientras el usuario escribe, para no consultar la base de datos
        /// en cada tecla.
        /// </summary>
        private void TxtBusqueda_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.DataContext is ServicioViewModel vmServicio)
            {
                vmServicio.ProgramarBusqueda();
            }
        }

        /// <summary>
        /// La tecla Enter ejecuta la busqueda de inmediato.
        /// </summary>
        private async void TxtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (this.DataContext is ServicioViewModel vmServicio)
            {
                vmServicio.CancelarBusquedaProgramada();
                await vmServicio.BuscarAsync();
            }
        }

        /// <summary>
        /// Cambiar la visibilidad de los registros inactivos recarga el listado.
        /// </summary>
        private async void ChkMostrarInactivos_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ServicioViewModel vmServicio)
            {
                vmServicio.CancelarBusquedaProgramada();
                await vmServicio.BuscarAsync();
            }
        }

        private async void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.LimpiarFormulario();
                var modal = new FrmServicio(ViewModel);
                modal.Owner = Window.GetWindow(this);
                if (modal.ShowDialog() == true)
                {
                    await ViewModel.LoadAsync();
                }
            }
        }

        private async void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgServicios.SelectedItem as ServicioDto;
            if (selected == null)
            {
                FrmConfirmacion.MostrarAviso("Por favor, seleccione un servicio de la lista.", "Aviso", Window.GetWindow(this));
                return;
            }

            if (ViewModel != null)
            {
                ViewModel.IdServicio = selected.IdServicio;
                ViewModel.Nombre = selected.Nombre;
                ViewModel.Precio = selected.Precio;
                ViewModel.Activo = selected.Activo;

                var modal = new FrmServicio(ViewModel);
                modal.Owner = Window.GetWindow(this);
                if (modal.ShowDialog() == true)
                {
                    await ViewModel.LoadAsync();
                }
            }
        }

        private async void BtnBorrar_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgServicios.SelectedItem as ServicioDto;
            if (selected == null)
            {
                FrmConfirmacion.MostrarAviso("Por favor, seleccione un servicio de la lista.", "Aviso", Window.GetWindow(this));
                return;
            }

            if (!FrmConfirmacion.Mostrar($"¿Está seguro de desactivar el servicio '{selected.Nombre}'?", "Confirmar eliminación", Window.GetWindow(this)))
                return;

            if (ViewModel != null)
            {
                await ViewModel.DeleteAsync(selected.IdServicio);
            }
        }

        private async void BtnReactivar_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgServicios.SelectedItem as ServicioDto;
            if (selected == null)
            {
                FrmConfirmacion.MostrarAviso("Por favor, seleccione un servicio de la lista.", "Aviso", Window.GetWindow(this));
                return;
            }

            if (ViewModel != null)
            {
                await ViewModel.ReactivarAsync(selected.IdServicio);
            }
        }
    }
}
