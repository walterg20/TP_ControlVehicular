using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Services;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    /// <summary>
    /// ViewModel para la pantalla de inicio de sesión (CtlLogin).
    /// Controla la vinculación de datos (DataBinding), estado del formulario, mensajes de error y la ejecución del comando de autenticación.
    /// </summary>
    public class LoginViewModel : BaseViewModel
    {
        private readonly AutenticarUsuarioHandler _autenticarUsuarioHandler;

        private string _dni = string.Empty;
        /// <summary>
        /// DNI ingresado por el usuario.
        /// </summary>
        public string Dni
        {
            get => _dni;
            set => SetProperty(ref _dni, value);
        }

        private string _contrasena = string.Empty;
        /// <summary>
        /// Contraseña ingresada por el usuario.
        /// </summary>
        public string Contrasena
        {
            get => _contrasena;
            set => SetProperty(ref _contrasena, value);
        }

        private string _errorMessage = string.Empty;
        /// <summary>
        /// Mensaje de error a mostrar en la vista en caso de datos inválidos o fallos de login.
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (SetProperty(ref _errorMessage, value))
                {
                    OnPropertyChanged(nameof(HasErrorMessage));
                }
            }
        }

        /// <summary>
        /// Indica si existe un mensaje de error activo para controlar la visibilidad del aviso en la UI.
        /// </summary>
        public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

        private bool _isLoading;
        /// <summary>
        /// Indica si la autenticación se encuentra en progreso para deshabilitar botones y mostrar indicadores de carga.
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        /// <summary>
        /// Evento notificador hacia MainWindow cuando el login resulta exitoso.
        /// </summary>
        public event Action<UsuarioDto>? OnLoginSuccess;

        /// <summary>
        /// Comando WPF enlazado al botón "Iniciar Sesión".
        /// </summary>
        public ICommand IniciarSesionCommand { get; }

        public LoginViewModel(AutenticarUsuarioHandler autenticarUsuarioHandler)
        {
            _autenticarUsuarioHandler = autenticarUsuarioHandler;
            IniciarSesionCommand = new RelayCommand(IniciarSesionAsync, () => !IsLoading);
        }

        /// <summary>
        /// Método de ejecución asíncrono del comando de inicio de sesión.
        /// </summary>
        private async Task IniciarSesionAsync()
        {
            ErrorMessage = string.Empty;

            // 1. Validaciones básicas de presencia de campos obligatorios en el cliente
            if (string.IsNullOrWhiteSpace(Dni) || string.IsNullOrWhiteSpace(Contrasena))
            {
                ErrorMessage = "Por favor, ingrese DNI y contraseña.";
                return;
            }

            IsLoading = true;
            ((RelayCommand)IniciarSesionCommand).RaiseCanExecuteChanged();

            try
            {
                // 2. Invocación al Caso de Uso de Autenticación en la Capa de Negocio
                var response = await _autenticarUsuarioHandler.HandleAsync(Dni.Trim(), Contrasena);

                if (response.Resultado == ResultadoAutenticacion.Exitoso && response.Usuario != null)
                {
                    // 3. Notifica éxito pasando los datos del usuario autenticado
                    OnLoginSuccess?.Invoke(response.Usuario);
                }
                else
                {
                    // Captura y muestra en la interfaz el error retornado por la capa de negocio
                    ErrorMessage = response.Mensaje;
                }
            }
            finally
            {
                IsLoading = false;
                ((RelayCommand)IniciarSesionCommand).RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Resetea los campos del formulario de login al cerrar sesión o volver a la pantalla.
        /// </summary>
        public void LimpiarFormulario()
        {
            Dni = string.Empty;
            Contrasena = string.Empty;
            ErrorMessage = string.Empty;
            IsLoading = false;
        }
    }
}
