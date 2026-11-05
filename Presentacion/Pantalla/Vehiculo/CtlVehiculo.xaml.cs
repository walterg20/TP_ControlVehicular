// walterg20
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TP_ControlVehicular.Presentacion.Pantalla.Compartido;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Vehiculo
{
    public partial class CtlVehiculo : UserControl
    {
        public VehiculoViewModel? ViewModel => DataContext as VehiculoViewModel;

        public CtlVehiculo()
        {
            InitializeComponent();
            try
            {
                if (App.ServiceProvider != null)
                {
                    var vmVehiculo = App.ServiceProvider.GetService(typeof(VehiculoViewModel)) as VehiculoViewModel;
                    if (vmVehiculo != null)
                    {
                        DataContext = vmVehiculo;
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

        public CtlVehiculo(VehiculoViewModel viewModel) : this()
        {
            DataContext = viewModel;
            ViewModel?.NotificarPermisos();
        }

        /// <summary>
        /// Ejecuta la busqueda en la base de datos (boton Buscar).
        /// </summary>
        private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is VehiculoViewModel vmVehiculo)
            {
                vmVehiculo.CancelarBusquedaProgramada();
                await vmVehiculo.BuscarAsync();
            }
        }

        /// <summary>
        /// Busca con retardo mientras el usuario escribe, para no consultar la base de datos
        /// en cada tecla.
        /// </summary>
        private void TxtBusqueda_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.DataContext is VehiculoViewModel vmVehiculo)
            {
                vmVehiculo.ProgramarBusqueda();
            }
        }

        /// <summary>
        /// La tecla Enter ejecuta la busqueda de inmediato.
        /// </summary>
        private async void TxtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (this.DataContext is VehiculoViewModel vmVehiculo)
            {
                vmVehiculo.CancelarBusquedaProgramada();
                await vmVehiculo.BuscarAsync();
            }
        }

        /// <summary>
        /// Cambiar la visibilidad de los registros inactivos recarga el listado.
        /// </summary>
        private async void CmbFiltroEstado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.DataContext is VehiculoViewModel vmVehiculo)
            {
                vmVehiculo.CancelarBusquedaProgramada();
                await vmVehiculo.BuscarAsync();
            }
        }

        private async void BtnReactivar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is VehiculoViewModel vmVehiculo && vmVehiculo.VehiculoSeleccionado != null)
            {
                var ok = await vmVehiculo.ReactivarVehiculoAsync(vmVehiculo.VehiculoSeleccionado.Id);
                if (ok)
                {
                    Presentacion.Pantalla.Compartido.FrmConfirmacion.MostrarAviso("Vehículo reactivado correctamente.", "Reactivar", Window.GetWindow(this));
                }
            }
            else
            {
                Presentacion.Pantalla.Compartido.FrmConfirmacion.MostrarAviso("Por favor, seleccione un vehículo.", "Aviso", Window.GetWindow(this));
            }
        }

        private void DgVehiculos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel is VehiculoViewModel vmVehiculo && !vmVehiculo.PuedeEditar)
            {
                return;
            }
            BtnEditar_Click(sender, e);
        }

        private async void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel is VehiculoViewModel vmVehiculo)
            {
                if (!vmVehiculo.PuedeCrear)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para registrar nuevos vehículos.", "Acceso denegado", Window.GetWindow(this));
                    return;
                }

                var frm = new FrmVehiculo();
                frm.Owner = Window.GetWindow(this);
                var ok = frm.ShowDialog();
                if (ok == true)
                {
                    if (Application.Current.MainWindow is MainWindow win) win.MostrarToast("Vehículo guardado con éxito.");
                    await vmVehiculo.LoadAsync();
                }
            }
        }

        private async void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel is VehiculoViewModel vmVehiculo)
            {
                if (!vmVehiculo.PuedeEditar)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para modificar vehículos.", "Acceso denegado", Window.GetWindow(this));
                    return;
                }

                if (vmVehiculo.VehiculoSeleccionado == null)
                {
                    FrmConfirmacion.MostrarAviso("Por favor, selecciona primero un vehículo.", "Aviso", Window.GetWindow(this));
                    return;
                }

                var frm = new FrmVehiculo(vmVehiculo.VehiculoSeleccionado);
                frm.Owner = Window.GetWindow(this);
                var ok = frm.ShowDialog();
                if (ok == true)
                {
                    if (Application.Current.MainWindow is MainWindow win) win.MostrarToast("Vehículo actualizado con éxito.");
                    await vmVehiculo.LoadAsync();
                }
            }
        }

        private async void BtnBorrar_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel is VehiculoViewModel vmVehiculo)
            {
                if (!vmVehiculo.PuedeEliminar)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para eliminar vehículos.", "Acceso denegado", Window.GetWindow(this));
                    return;
                }

                if (vmVehiculo.VehiculoSeleccionado == null)
                {
                    FrmConfirmacion.MostrarAviso("Por favor, selecciona primero un vehículo para eliminar.", "Aviso", Window.GetWindow(this));
                    return;
                }

                if (!FrmConfirmacion.Mostrar($"¿Está seguro que desea eliminar el vehículo con patente '{vmVehiculo.VehiculoSeleccionado.Patente}'?", "Confirmar Eliminación", Window.GetWindow(this)))
                    return;

                var ok = await vmVehiculo.EliminarVehiculoAsync(vmVehiculo.VehiculoSeleccionado.Id);
                if (ok)
                {
                    FrmConfirmacion.MostrarAviso("Vehículo eliminado correctamente.", "Éxito", Window.GetWindow(this), icono: "✅");
                }
                else
                {
                    FrmConfirmacion.MostrarAviso("No se pudo eliminar el vehículo.", "Error", Window.GetWindow(this), icono: "❌");
                }
            }
        }
    }
}
