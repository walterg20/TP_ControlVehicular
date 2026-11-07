// walterg20
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Presentacion.ViewModels;
using TP_ControlVehicular.Negocio.Services;

namespace TP_ControlVehicular.Presentacion.Pantalla.OrdenServicio
{
    /// <summary>
    /// Modal unificado de alta: cliente + vehiculo + vinculo de propiedad en un solo paso.
    /// Se abre desde el buscador universal de la orden de servicio.
    /// </summary>
    public partial class FrmClienteVehiculo : Window
    {
        private readonly string _patenteSugerida;
        private readonly ClienteDto? _clienteExistente;

        /// <summary>
        /// Constructor sin parametros: WPF lo exige para instanciar la ventana desde el
        /// XAML compilado (Application.LoadComponent).
        /// </summary>
        public FrmClienteVehiculo() : this("", null)
        {
        }

        public FrmClienteVehiculo(string patenteSugerida, ClienteDto? clienteExistente = null)
        {
            InitializeComponent();
            _patenteSugerida = patenteSugerida;
            _clienteExistente = clienteExistente;
            ConfigurarViewModel();
            Loaded += (s, e) => txtPatente.Focus();
            ConfigurarValidacionAlPerderFoco();
        }

        /// <summary>Cliente creado en el modal (disponible tras un guardado exitoso).</summary>
        public ClienteDto? ClienteCreado => (DataContext as ClienteVehiculoViewModel)?.ClienteCreado;

        /// <summary>Vehiculo creado en el modal (disponible tras un guardado exitoso).</summary>
        public VehiculoDto? VehiculoCreado => (DataContext as ClienteVehiculoViewModel)?.VehiculoCreado;

        private void ConfigurarViewModel()
        {
            try
            {
                if (App.ServiceProvider?.GetService(typeof(ClienteVehiculoViewModel)) is ClienteVehiculoViewModel vmClienteVehiculo)
                {
                    DataContext = vmClienteVehiculo;

                    // El servicio es Scoped (vive toda la app): se reinicia su estado.
                    vmClienteVehiculo.Reiniciar(_clienteExistente);

                    vmClienteVehiculo.Patente = _patenteSugerida ?? string.Empty;

                    if (_clienteExistente != null)
                    {
                        lblTituloFormulario.Text = "Registrar Vehículo para Cliente Existente";
                    }

                    vmClienteVehiculo.RegistrationFailed -= Vm_RegistrationFailed;
                    vmClienteVehiculo.RegistrationFailed += Vm_RegistrationFailed;

                    _ = vmClienteVehiculo.CargarCombosAsync();
                }
            }
            catch (Exception ex)
            {
                (App.ServiceProvider?.GetService(typeof(IErrorNotifier)) as IErrorNotifier)?.Notify(ex, "FrmClienteVehiculo.xaml");
                // Silenciar si DI no esta disponible
            }
        }

        private void ConfigurarValidacionAlPerderFoco()
        {
            AddHandler(UIElement.LostFocusEvent, new RoutedEventHandler((s, e) =>
            {
                if (e.OriginalSource is FrameworkElement element && DataContext is BaseViewModel vmClienteVehiculo)
                {
                    DependencyProperty? dp = null;
                    if (element is TextBox)
                        dp = TextBox.TextProperty;
                    else if (element is ComboBox)
                        dp = ComboBox.SelectedValueProperty;

                    if (dp != null)
                    {
                        var binding = BindingOperations.GetBinding(element, dp);
                        if (binding != null && binding.Path != null && !string.IsNullOrEmpty(binding.Path.Path))
                        {
                            var be = element.GetBindingExpression(dp);
                            be?.UpdateSource();
                            vmClienteVehiculo.ValidateProperty(binding.Path.Path);
                        }
                    }
                }
            }));
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ClienteVehiculoViewModel vmClienteVehiculo)
            {
                bool ok = await vmClienteVehiculo.RegistrarAsync();
                if (ok)
                {
                    this.DialogResult = true;
                }
            }
            else
            {
                this.DialogResult = false;
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Vm_RegistrationFailed(object? sender, string errorMessage)
        {
            Compartido.FrmConfirmacion.MostrarAviso(
                "No se pudo registrar el cliente y el vehículo:\n" + errorMessage,
                "Error",
                this);
        }
    }
}
