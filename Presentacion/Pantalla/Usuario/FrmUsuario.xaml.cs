using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Presentacion.Rol;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Usuario
{
    public partial class FrmUsuario : Window
    {
        private readonly bool _esModificacion;

        public FrmUsuario()
        {
            InitializeComponent();
            _esModificacion = false;
            this.Title = "Registrar Nuevo Usuario";
            
            // Configurar límites del DatePicker (min: 80 años atrás, max: 18 años atrás)
            dtpFechaNacimiento.DisplayDateStart = DateTime.Today.AddYears(-80);
            dtpFechaNacimiento.DisplayDateEnd = DateTime.Today.AddYears(-18);

            ConfigurarViewModel(null);
            Loaded += (s, e) => txtNombre.Focus();
            ConfigurarValidacionAlPerderFoco();
        }

        public FrmUsuario(UsuarioDto usuario)
        {
            InitializeComponent();
            _esModificacion = true;
            this.Title = "Modificar Usuario";

            dtpFechaNacimiento.DisplayDateStart = DateTime.Today.AddYears(-80);
            dtpFechaNacimiento.DisplayDateEnd = DateTime.Today.AddYears(-18);

            ConfigurarViewModel(usuario);
            Loaded += (s, e) => txtNombre.Focus();
            ConfigurarValidacionAlPerderFoco();
        }

        private void ConfigurarValidacionAlPerderFoco()
        {
            AddHandler(UIElement.LostFocusEvent, new RoutedEventHandler((s, e) =>
            {
                if (e.OriginalSource is FrameworkElement element && DataContext is BaseViewModel vm)
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

        private async void ConfigurarViewModel(UsuarioDto? usuario)
        {
            try
            {
                if (App.ServiceProvider.GetService(typeof(UsuarioViewModel)) is UsuarioViewModel vmUsuario)
                {
                    DataContext = vmUsuario;
                    await vmUsuario.LoadAsync();

                    if (_esModificacion && usuario != null)
                    {
                        vmUsuario.UsuarioSeleccionado = usuario;
                        vmUsuario.Nombre = usuario.Nombre;
                        vmUsuario.Apellido = usuario.Apellido;
                        vmUsuario.Dni = usuario.Dni;
                        vmUsuario.Email = usuario.Email;
                        vmUsuario.Telefono = usuario.Telefono;
                        vmUsuario.Domicilio = usuario.Domicilio;
                        
                        if (usuario.FechaNacimiento.Year < 1900)
                            vmUsuario.FechaNacimiento = DateTime.Today.AddYears(-18);
                        else
                            vmUsuario.FechaNacimiento = usuario.FechaNacimiento;
                            
                        vmUsuario.Contrasena = usuario.Contrasena;
                        vmUsuario.IdRol = usuario.IdRol;
                        vmUsuario.Estado = usuario.Estado;

                        // En modo edición, ocultar el input por defecto y mostrar el botón "Setear Clave"
                        btnSetearClave.Visibility = Visibility.Visible;
                        pnlContrasenaInput.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        vmUsuario.UsuarioSeleccionado = null;
                        vmUsuario.Nombre = string.Empty;
                        vmUsuario.Apellido = string.Empty;
                        vmUsuario.Dni = string.Empty;
                        vmUsuario.Email = string.Empty;
                        vmUsuario.Telefono = string.Empty;
                        vmUsuario.Domicilio = string.Empty;
                        vmUsuario.FechaNacimiento = DateTime.Today.AddYears(-18);
                        vmUsuario.Contrasena = string.Empty;
                        vmUsuario.IdRol = 0;
                        vmUsuario.Estado = true;

                        // En modo creación, mostrar el input de contraseña directamente
                        btnSetearClave.Visibility = Visibility.Collapsed;
                        pnlContrasenaInput.Visibility = Visibility.Visible;
                    }

                    vmUsuario.ClearAllErrors();
                    
                    vmUsuario.RegistrationFailed -= VmUsuario_RegistrationFailed;
                    vmUsuario.RegistrationFailed += VmUsuario_RegistrationFailed;
                }
            }
            catch
            {
                // Ignorar si DI no está disponible
            }
        }

        private void VmUsuario_RegistrationFailed(object? sender, string errorMessage)
        {
            Presentacion.Pantalla.Compartido.FrmConfirmacion.MostrarAviso(errorMessage, "Error de Guardado", Window.GetWindow(this));
        }

        private void BtnSetearClave_Click(object sender, RoutedEventArgs e)
        {
            btnSetearClave.Visibility = Visibility.Collapsed;
            pnlContrasenaInput.Visibility = Visibility.Visible;
            BtnGenerarClave_Click(sender, e);
        }

        private void BtnGenerarClave_Click(object sender, RoutedEventArgs e)
        {
            string claveRandom = Validaciones.ValidadorGlobal.GenerarContrasenaRandom();
            if (DataContext is UsuarioViewModel vmUsuario)
            {
                vmUsuario.Contrasena = claveRandom;
                vmUsuario.ValidateProperty(nameof(vmUsuario.Contrasena));
            }
            txtContrasena.Text = claveRandom;
            pwdContrasena.Password = claveRandom;

            // Asegurar que la clave sea visible al generarla
            txtContrasena.Visibility = Visibility.Visible;
            pwdContrasena.Visibility = Visibility.Collapsed;
            btnToggleVerClave.Content = "👁️";
        }

        private void BtnToggleVerClave_Click(object sender, RoutedEventArgs e)
        {
            if (txtContrasena.Visibility == Visibility.Visible)
            {
                pwdContrasena.Password = txtContrasena.Text;
                txtContrasena.Visibility = Visibility.Collapsed;
                pwdContrasena.Visibility = Visibility.Visible;
                btnToggleVerClave.Content = "🙈";
                pwdContrasena.Focus();
            }
            else
            {
                txtContrasena.Text = pwdContrasena.Password;
                pwdContrasena.Visibility = Visibility.Collapsed;
                txtContrasena.Visibility = Visibility.Visible;
                btnToggleVerClave.Content = "👁️";
                txtContrasena.Focus();
                txtContrasena.SelectionStart = txtContrasena.Text.Length;
            }
        }

        private void PwdContrasena_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (pwdContrasena.Visibility == Visibility.Visible)
            {
                txtContrasena.Text = pwdContrasena.Password;
                if (DataContext is UsuarioViewModel vmUsuario)
                {
                    vmUsuario.Contrasena = pwdContrasena.Password;
                }
            }
        }

        private async void BtnAgregarRol_Click(object sender, RoutedEventArgs e)
        {
            var frmRol = new FrmRol();
            frmRol.Owner = this;
            var ok = frmRol.ShowDialog();
            if (ok == true && DataContext is UsuarioViewModel vmUsuario)
            {
                await vmUsuario.CargarRolesAsync();
            }
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is UsuarioViewModel vmUsuario)
            {
                if (!vmUsuario.ValidateAll())
                {
                    Presentacion.Pantalla.Compartido.FrmConfirmacion.MostrarAviso("Por favor, revise los campos marcados en rojo. Asegúrese de que la contraseña cumpla con los requisitos.", "Validación", Window.GetWindow(this));
                    return;
                }

                var ok = await vmUsuario.GuardarUsuarioAsync();
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
    }
}
