// walterg20
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Presentacion.Pantalla.Compartido;
using TP_ControlVehicular.Presentacion.ViewModels;
using TP_ControlVehicular.Negocio.Services;

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
            catch (Exception ex)
            {
                (App.ServiceProvider?.GetService(typeof(IErrorNotifier)) as IErrorNotifier)?.Notify(ex, "CtlServicio.xaml");
                // Fallback handled gracefully
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

        public CtlServicio(ServicioViewModel viewModel) : this()
        {
            DataContext = viewModel;
            ViewModel?.NotificarPermisos();
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
        /// Cambiar el filtro de estado recarga el listado.
        /// </summary>
        private async void CmbFiltroEstado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.DataContext is ServicioViewModel vmServicio)
            {
                vmServicio.CancelarBusquedaProgramada();
                await vmServicio.BuscarAsync();
            }
        }

        private async void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel is ServicioViewModel vmServicio)
            {
                if (!vmServicio.PuedeCrear)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para registrar nuevos servicios.", "Acceso denegado", Window.GetWindow(this));
                    return;
                }

                var frm = new FrmServicio();
                frm.Owner = Window.GetWindow(this);
                var resultado = frm.ShowDialog();
                if (resultado == true)
                {
                    await vmServicio.LoadAsync();
                }
            }
        }

        private async void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel is ServicioViewModel vmServicio)
            {
                if (!vmServicio.PuedeEditar)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para modificar servicios.", "Acceso denegado", Window.GetWindow(this));
                    return;
                }

                if (vmServicio.ServicioSeleccionado is null)
                {
                    FrmConfirmacion.MostrarAviso("Por favor, selecciona primero un servicio.", "Aviso", Window.GetWindow(this));
                    return;
                }

                var frm = new FrmServicio(vmServicio.ServicioSeleccionado);
                frm.Owner = Window.GetWindow(this);
                var ok = frm.ShowDialog();
                if (ok == true)
                {
                    await vmServicio.LoadAsync();
                }
            }
        }

        private void DgServicios_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel is ServicioViewModel vmServicio && !vmServicio.PuedeEditar)
            {
                return;
            }
            BtnEditar_Click(sender, e);
        }

        private async void BtnBorrar_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel is ServicioViewModel vmServicio)
            {
                if (!vmServicio.PuedeEliminar)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para eliminar servicios.", "Acceso denegado", Window.GetWindow(this));
                    return;
                }

                var selected = vmServicio.ServicioSeleccionado;
                if (selected is null)
                {
                    FrmConfirmacion.MostrarAviso("Por favor, selecciona primero un servicio.", "Aviso", Window.GetWindow(this));
                    return;
                }

                if (!FrmConfirmacion.Mostrar($"¿Está seguro de desactivar el servicio '{selected.Nombre}'?", "Confirmar eliminación", Window.GetWindow(this)))
                    return;

                await vmServicio.DeleteAsync(selected.IdServicio);
            }
        }

        private async void BtnReactivar_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel is ServicioViewModel vmServicio)
            {
                if (!vmServicio.PuedeEditar)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para reactivar servicios.", "Acceso denegado", Window.GetWindow(this));
                    return;
                }

                var selected = vmServicio.ServicioSeleccionado;
                if (selected is null)
                {
                    FrmConfirmacion.MostrarAviso("Por favor, selecciona primero un servicio.", "Aviso", Window.GetWindow(this));
                    return;
                }

                await vmServicio.ReactivarAsync(selected.IdServicio);
                FrmConfirmacion.MostrarAviso("Servicio reactivado correctamente.", "Reactivar", Window.GetWindow(this));
            }
        }
    }
}
