// walterg20
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TP_ControlVehicular.Presentacion.Pantalla.Compartido;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Taller
{
    public partial class CtlTaller : UserControl
    {
        public TallerViewModel? ViewModel => DataContext as TallerViewModel;

        public CtlTaller()
        {
            InitializeComponent();
            try
            {
                var vmTaller = App.ServiceProvider?.GetService(typeof(TallerViewModel)) as TallerViewModel;
                if (vmTaller is not null)
                {
                    this.DataContext = vmTaller;
                    this.Loaded += async (s, e) =>
                    {
                        vmTaller.NotificarPermisos();
                        await vmTaller.LoadAsync();
                    };
                }
            }
            catch
            {
                // Silenciar si DI no está listo
            }
        }

        public CtlTaller(TallerViewModel viewModel) : this()
        {
            DataContext = viewModel;
            ViewModel?.NotificarPermisos();
        }

        /// <summary>
        /// Ejecuta la busqueda en la base de datos (boton Buscar).
        /// </summary>
        private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is TallerViewModel vmTaller)
            {
                vmTaller.CancelarBusquedaProgramada();
                await vmTaller.BuscarAsync();
            }
        }

        /// <summary>
        /// Busca con retardo mientras el usuario escribe, para no consultar la base de datos
        /// en cada tecla.
        /// </summary>
        private void TxtBusqueda_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.DataContext is TallerViewModel vmTaller)
            {
                vmTaller.ProgramarBusqueda();
            }
        }

        /// <summary>
        /// La tecla Enter ejecuta la busqueda de inmediato.
        /// </summary>
        private async void TxtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (this.DataContext is TallerViewModel vmTaller)
            {
                vmTaller.CancelarBusquedaProgramada();
                await vmTaller.BuscarAsync();
            }
        }

        /// <summary>
        /// Cambiar el filtro de estado recarga el listado.
        /// </summary>
        private async void CmbFiltroEstado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.DataContext is TallerViewModel vmTaller)
            {
                vmTaller.CancelarBusquedaProgramada();
                await vmTaller.BuscarAsync();
            }
        }

        private void DgTalleres_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel is TallerViewModel vmTaller && !vmTaller.PuedeEditar)
            {
                return;
            }
            BtnEditar_Click(sender, e);
        }

        private async void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is TallerViewModel vmTaller)
            {
                if (!vmTaller.PuedeCrear)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para registrar nuevos talleres.", "Acceso denegado", Window.GetWindow(this));
                    return;
                }

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
                if (!vmTaller.PuedeEditar)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para modificar talleres.", "Acceso denegado", Window.GetWindow(this));
                    return;
                }

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
                if (!vmTaller.PuedeEliminar)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para eliminar talleres.", "Acceso denegado", Window.GetWindow(this));
                    return;
                }

                if (vmTaller.TallerSeleccionado == null)
                {
                    FrmConfirmacion.MostrarAviso("Por favor, selecciona primero un taller de la lista.", "Aviso", Window.GetWindow(this));
                    return;
                }

                if (!FrmConfirmacion.Mostrar($"¿Está seguro de dar de baja (lógica) al taller '{vmTaller.TallerSeleccionado.Nombre}'?", "Confirmar baja", Window.GetWindow(this)))
                    return;

                await vmTaller.EliminarTallerAsync(vmTaller.TallerSeleccionado.IdTaller);
            }
        }

        private async void BtnReactivar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is TallerViewModel vmTaller)
            {
                if (!vmTaller.PuedeEditar)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para reactivar talleres.", "Acceso denegado", Window.GetWindow(this));
                    return;
                }

                if (vmTaller.TallerSeleccionado == null)
                {
                    FrmConfirmacion.MostrarAviso("Por favor, selecciona primero un taller de la lista.", "Aviso", Window.GetWindow(this));
                    return;
                }

                var ok = await vmTaller.ReactivarAsync(vmTaller.TallerSeleccionado.IdTaller);
                if (ok)
                {
                    FrmConfirmacion.MostrarAviso("Taller reactivado correctamente.", "Reactivar", Window.GetWindow(this));
                }
            }
        }
    }
}
