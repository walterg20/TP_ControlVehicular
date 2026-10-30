// walterg20
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Pantalla.MisTrabajos
{
    public partial class CtlMisTrabajos : UserControl
    {
        public CtlMisTrabajos()
        {
            InitializeComponent();
            ConfigurarViewModel();
        }

        private async void ConfigurarViewModel()
        {
            if (App.ServiceProvider?.GetService(typeof(MisTrabajosViewModel)) is MisTrabajosViewModel vmMisTrabajos)
            {
                DataContext = vmMisTrabajos;
                await vmMisTrabajos.LoadAsync();
            }
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MisTrabajosViewModel vmMisTrabajos)
            {
                var ok = await vmMisTrabajos.GuardarCambiosAsync();
                if (ok && Application.Current.MainWindow is MainWindow win)
                {
                    win.MostrarToast("Cambios guardados con éxito.");
                }
            }
        }

        private void BtnVerOrden_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MisTrabajosViewModel vmMisTrabajos)
            {
                AbrirOrdenCompleta(vmMisTrabajos.OrdenSeleccionada?.Orden);
            }
            else if (sender is Button btn && btn.CommandParameter is RegistroServicioDto orden)
            {
                AbrirOrdenCompleta(orden);
            }
        }

        private void DgOrdenes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Solo abrir si el doble clic ocurrió sobre una fila de la grilla maestra.
            var origen = e.OriginalSource as DependencyObject;
            if (BuscarAncestro<DataGridRow>(origen) == null) return;

            if (DataContext is MisTrabajosViewModel vmMisTrabajos)
            {
                AbrirOrdenCompleta(vmMisTrabajos.OrdenSeleccionada?.Orden);
            }
        }

        private static T? BuscarAncestro<T>(DependencyObject? actual) where T : DependencyObject
        {
            while (actual != null)
            {
                if (actual is T encontrado) return encontrado;
                actual = actual is Visual || actual is System.Windows.Media.Media3D.Visual3D
                    ? VisualTreeHelper.GetParent(actual)
                    : LogicalTreeHelper.GetParent(actual);
            }
            return null;
        }

        private void AbrirOrdenCompleta(RegistroServicioDto? orden)
        {
            if (orden == null) return;
            if (Application.Current.MainWindow is MainWindow win)
            {
                // Abre el formulario de la orden conservando el origen para que la vuelta regrese aquí.
                win.AgregarPagina(new Pantalla.OrdenServicio.CtlOrdenServicioForm(orden, "MisTrabajos"));
            }
        }

        /// <summary>Busca con retardo mientras el usuario escribe.</summary>
        private void TxtBusqueda_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded || DataContext is not MisTrabajosViewModel vmMisTrabajos) return;
            vmMisTrabajos.ProgramarBusqueda();
        }

        /// <summary>La tecla Enter ejecuta la busqueda de inmediato.</summary>
        private async void TxtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter || DataContext is not MisTrabajosViewModel vmMisTrabajos) return;
            vmMisTrabajos.CancelarBusquedaProgramada();
            await vmMisTrabajos.BuscarAsync();
        }

        /// <summary>Recarga el listado al cambiar el rango de fechas.</summary>
        private void DtpFechaDesde_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded || DataContext is not MisTrabajosViewModel vmMisTrabajos) return;
            vmMisTrabajos.ProgramarBusqueda();
        }

        /// <summary>Recarga el listado al cambiar el rango de fechas.</summary>
        private void DtpFechaHasta_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded || DataContext is not MisTrabajosViewModel vmMisTrabajos) return;
            vmMisTrabajos.ProgramarBusqueda();
        }

        /// <summary>Recarga el listado al cambiar el filtro de tareas.</summary>
        private void CboFiltroEstado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded || DataContext is not MisTrabajosViewModel vmMisTrabajos) return;
            vmMisTrabajos.ProgramarBusqueda();
        }
    }
}
