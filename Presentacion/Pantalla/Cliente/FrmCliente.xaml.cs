
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TP_ControlVehicular.Negocio.DTOs;

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

            // Obtener el ViewModel desde el contenedor DI y suscribirse al evento
            try
            {
                if (App.ServiceProvider.GetService(typeof(Presentacion.ViewModels.ClienteViewModel)) is Presentacion.ViewModels.ClienteViewModel vmCliente)
                {
                    DataContext = vmCliente;
                    vmCliente.ClearAllErrors();
                }
            }
            catch
            {
                // Ignorar si DI no está disponible
            }

            Loaded += (s, e) => txtNombre.Focus();
            ConfigurarValidacionAlPerderFoco();
        }

        private void ConfigurarValidacionAlPerderFoco()
        {
            AddHandler(UIElement.LostFocusEvent, new RoutedEventHandler((s, e) =>
            {
                if (e.OriginalSource is FrameworkElement element && DataContext is ViewModels.BaseViewModel vm)
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
                            vm.ValidateProperty(binding.Path.Path);
                        }
                    }
                }
            }));
        }

        // Constructor para Editar
        /*public FrmCliente(UsuarioModel usuario)
        {
            InitializeComponent();
            lblTituloFormulario.Text = "Modificar Usuario";
            _usuarioEdicion = usuario;
            _esModificacion = true;

            txtNombre.Text = usuario.Nombre;
            txtContrasena.Password = usuario.Contrasena;
            cmbRol.SelectedIndex = usuario.IdRol - 1;
            chkEstado.IsChecked = usuario.Estado;
        }*/

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
    }
}
