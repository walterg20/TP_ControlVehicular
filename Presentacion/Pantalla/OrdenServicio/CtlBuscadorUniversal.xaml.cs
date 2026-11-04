using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Presentacion.Helpers;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Pantalla.OrdenServicio
{
    /// <summary>
    /// Control de busqueda predictiva universal (DNI, apellido, nombre y patente).
    /// Se implementa con TextBox + Popup/ListBox para tener control determinista sobre
    /// el texto tipeado, el desplegable de resultados y el foco.
    /// </summary>
    public partial class CtlBuscadorUniversal : UserControl
    {
        private readonly Debouncer _debouncer = new(TimeSpan.FromMilliseconds(250));
        private bool _omitirFiltro;
        private string _textoAplicado = string.Empty;

        public CtlBuscadorUniversal()
        {
            InitializeComponent();
            Loaded += (s, e) => txtBusqueda.Focus();
            Unloaded += (s, e) => _debouncer.Cancel();
        }

        /// <summary>ViewModel de la orden de servicio al que se enlaza el control.</summary>
        private OrdenServicioViewModel? ViewModel => DataContext as OrdenServicioViewModel;

        /// <summary>Texto tipeado por el operador.</summary>
        public string TextoBusqueda => txtBusqueda.Text;

        private void TxtBusqueda_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_omitirFiltro) return;

            var vm = ViewModel;
            if (vm == null) return;

            string texto = txtBusqueda.Text;

            // El texto vigente queda siempre sincronizado con el ViewModel: el modal
            // unificado lo usa para prellenar la patente.
            vm.TextoBusquedaUniversal = texto;

            // El texto aplicado tras una seleccion no dispara una nueva busqueda.
            if (texto == _textoAplicado) return;

            if (texto.Trim().Length < 3)
            {
                _debouncer.Cancel();
                vm.FiltrarBusquedaUniversal(texto);
                CerrarDesplegable();
                return;
            }

            _debouncer.Debounce(() =>
            {
                if (ViewModel is OrdenServicioViewModel vmActual)
                {
                    _omitirFiltro = true;
                    try
                    {
                        vmActual.FiltrarBusquedaUniversal(texto);
                        if (vmActual.ResultadosBusquedaUniversal.Any())
                        {
                            AbrirDesplegable();
                        }
                        else
                        {
                            CerrarDesplegable();
                        }
                    }
                    finally
                    {
                        _omitirFiltro = false;
                    }
                }
                return Task.CompletedTask;
            });
        }

        private void TxtBusqueda_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // Si hay texto aplicado de una seleccion previa, se selecciona completo para
            // que al escribir se reemplace en lugar de concatenarse.
            if (!string.IsNullOrEmpty(_textoAplicado) && txtBusqueda.Text == _textoAplicado)
            {
                txtBusqueda.SelectAll();
            }
        }

        private void TxtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            var vm = ViewModel;

            switch (e.Key)
            {
                case Key.Escape:
                    CerrarDesplegable();
                    e.Handled = true;
                    return;

                case Key.Enter:
                    var resultado = lstResultados.SelectedItem as BusquedaClienteVehiculoDto
                                    ?? vm?.ResultadosBusquedaUniversal.FirstOrDefault();
                    if (resultado != null)
                    {
                        AplicarResultado(resultado);
                        e.Handled = true;
                    }
                    return;

                case Key.Down:
                    if (vm != null && vm.ResultadosBusquedaUniversal.Any())
                    {
                        if (!popupDesplegable.IsOpen) AbrirDesplegable();
                        lstResultados.SelectedIndex = lstResultados.SelectedIndex < 0
                            ? 0
                            : Math.Min(lstResultados.SelectedIndex + 1, vm.ResultadosBusquedaUniversal.Count - 1);
                        lstResultados.ScrollIntoView(lstResultados.SelectedItem);
                        e.Handled = true;
                    }
                    return;

                case Key.Up:
                    if (lstResultados.SelectedIndex > 0)
                    {
                        lstResultados.SelectedIndex = lstResultados.SelectedIndex - 1;
                        lstResultados.ScrollIntoView(lstResultados.SelectedItem);
                        e.Handled = true;
                    }
                    return;
            }
        }

        /// <summary>
        /// El Popup con StaysOpen="False" se cierra antes de procesar el click del mouse.
        /// Por eso la fila se aplica en PreviewMouseLeftButtonDown.
        /// </summary>
        private void LstResultados_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement elemento
                && elemento.DataContext is BusquedaClienteVehiculoDto resultado)
            {
                AplicarResultado(resultado);
                e.Handled = true;
            }
        }

        private void LstResultados_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // La seleccion por teclado (flechas) solo resalta la fila: se aplica con Enter.
        }

        private void AplicarResultado(BusquedaClienteVehiculoDto resultado)
        {
            var vm = ViewModel;
            if (vm == null) return;

            // El desplegable se cierra antes de aplicar: si no, el Popup abierto roba el
            // foco que el ViewModel pide (kilometraje o vehiculo).
            CerrarDesplegable();

            _omitirFiltro = true;
            try
            {
                if (resultado.EsAccionRegistrar)
                {
                    // El modal unificado se abre desde el ViewModel; se conserva el texto
                    // tipeado para que el operador vea que termino buscando.
                    _textoAplicado = string.Empty;
                    txtBusqueda.Text = vm.TextoBusquedaUniversal;
                }
                else
                {
                    // El campo muestra el nombre del dueño: la fila completa ya fue aplicada.
                    _textoAplicado = resultado.TextMostrar;
                    txtBusqueda.Text = _textoAplicado;
                    txtBusqueda.SelectAll();
                }

                // La seleccion se aplica al final: el foco pedido por el ViewModel
                // (kilometraje o vehiculo) queda como ultimo movimiento.
                vm.SeleccionarResultadoUniversal(resultado);
            }
            finally
            {
                _omitirFiltro = false;
            }
        }

        private void AbrirDesplegable()
        {
            popupDesplegable.IsOpen = true;
            lstResultados.SelectedIndex = -1;
        }

        private void CerrarDesplegable()
        {
            popupDesplegable.IsOpen = false;
            lstResultados.SelectedIndex = -1;
        }

        /// <summary>
        /// Muestra en el campo el nombre del cliente con el vehiculo ya vinculado.
        /// </summary>
        public void MostrarClienteSeleccionado(string texto)
        {
            _omitirFiltro = true;
            try
            {
                _textoAplicado = texto;
                txtBusqueda.Text = texto;
            }
            finally
            {
                _omitirFiltro = false;
            }
        }

        /// <summary>Limpia el campo de busqueda.</summary>
        public void LimpiarBusqueda()
        {
            _omitirFiltro = true;
            try
            {
                _textoAplicado = string.Empty;
                txtBusqueda.Clear();
            }
            finally
            {
                _omitirFiltro = false;
            }
        }
    }
}
