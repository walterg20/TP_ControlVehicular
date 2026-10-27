// walterg20
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Cliente
{
    /// <summary>
    /// Lógica de interacción para FrmCliente.xaml
    /// </summary>
    public partial class FrmCliente : Window
    {
        private bool _esModificacion = false;

        // Constructor para Nuevo
        public FrmCliente()
        {
            InitializeComponent();
            lblTituloFormulario.Text = "Registrar Nuevo Cliente";
            _esModificacion = false;
            this.Title = "Registrar Nuevo Cliente";

            this.configurarViewModel(null);

            Loaded += (s, e) => txtNombre.Focus();
            ConfigurarValidacionAlPerderFoco();
        }
        public FrmCliente(ClienteDto clienteDto)
        {
            InitializeComponent();
            lblTituloFormulario.Text = "Modificar Cliente";
            _esModificacion = true;
            this.Title = "Modificar Cliente";

            this.configurarViewModel(clienteDto);
            Loaded += (s, e) => txtNombre.Focus();
            this.ConfigurarValidacionAlPerderFoco();
        }

        private void ConfigurarValidacionAlPerderFoco()
        {
            AddHandler(UIElement.LostFocusEvent, new RoutedEventHandler((s, e) =>
            {
                if (e.OriginalSource is FrameworkElement element && DataContext is ViewModels.BaseViewModel vmCliente)
                {
                    DependencyProperty? dp = null;
                    if (element is TextBox)
                        dp = TextBox.TextProperty;
                    else if (element is ComboBox)
                        dp = ComboBox.SelectedValueProperty;
                    else if (element is DatePicker)
                        dp = DatePicker.SelectedDateProperty;

                    if (dp != null)
                    {
                        var binding = BindingOperations.GetBinding(element, dp);
                        var be = element.GetBindingExpression(dp);
                        if (binding != null && binding.Path != null && !string.IsNullOrEmpty(binding.Path.Path))
                        {
                            be?.UpdateSource();
                            vmCliente.ValidateProperty(binding.Path.Path);
                        }
                    }
                }
            }));
        }


        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is Presentacion.ViewModels.ClienteViewModel vmCliente)
            {
                var ok = await vmCliente.RegistrarClienteAsync();
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
            this.DialogResult = false; // Cierra sin hacer nada
        }


        private void configurarViewModel(ClienteDto? cliente)
        {
            try
            {
                if (App.ServiceProvider.GetService(typeof(ClienteViewModel)) is Presentacion.ViewModels.ClienteViewModel vmCliente)
                {
                    DataContext = vmCliente;
                    if (_esModificacion && cliente != null)
                    {
                        vmCliente.IdCliente = cliente.IdCliente;
                        vmCliente.Nombre = cliente.Nombre;
                        vmCliente.Apellido = cliente.Apellido;
                        vmCliente.Dni = cliente.Dni;
                        vmCliente.FechaNacimiento = cliente.FechaNac;
                        vmCliente.Direccion = cliente.Direccion;
                        vmCliente.Email = cliente.Email;
                        vmCliente.Telefono = cliente.Telefono;
                        vmCliente.Activo = cliente.Activo;
                    }
                    else
                    {
                        // Inicializar propiedades para un nuevo cliente
                        vmCliente.IdCliente = 0; // o algún valor predeterminado
                        vmCliente.Nombre = string.Empty;
                        vmCliente.Apellido = string.Empty;
                        vmCliente.Dni = string.Empty;
                        vmCliente.FechaNacimiento = null; // o DateTime.MinValue según tu lógica
                        vmCliente.Direccion = string.Empty;
                        vmCliente.Email = string.Empty;
                        vmCliente.Telefono = string.Empty;
                        vmCliente.Activo = true; // o false según tu lógica
                    }
                    vmCliente.ClearAllErrors();
                }
            }
            catch
            {
                // Silenciar si DI no está disponible
            }
        }
    }
}
