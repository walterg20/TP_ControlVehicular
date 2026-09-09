using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Services;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AutenticarUsuarioHandler _autenticarUsuarioHandler;

        private string _nombreUsuario = string.Empty;
        public string NombreUsuario
        {
            get => _nombreUsuario;
            set => SetProperty(ref _nombreUsuario, value);
        }

        private string _contrasena = string.Empty;
        public string Contrasena
        {
            get => _contrasena;
            set => SetProperty(ref _contrasena, value);
        }

        private string _errorMessage = string.Empty;
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

        public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public event Action<UsuarioDto>? OnLoginSuccess;

        public ICommand IniciarSesionCommand { get; }

        public LoginViewModel(AutenticarUsuarioHandler autenticarUsuarioHandler)
        {
            _autenticarUsuarioHandler = autenticarUsuarioHandler;
            IniciarSesionCommand = new RelayCommand(IniciarSesionAsync, () => !IsLoading);
        }

        private async Task IniciarSesionAsync()
        {
            ErrorMessage = string.Empty;

            // 1. Validaciones de formulario (cliente)
            if (string.IsNullOrWhiteSpace(NombreUsuario) || string.IsNullOrWhiteSpace(Contrasena))
            {
                ErrorMessage = "Por favor, ingrese usuario y contraseña.";
                return;
            }

            IsLoading = true;
            ((RelayCommand)IniciarSesionCommand).RaiseCanExecuteChanged();

            try
            {
                // 2. Consulta y autenticación en capa de negocio / datos
                var response = await _autenticarUsuarioHandler.HandleAsync(NombreUsuario.Trim(), Contrasena);

                if (response.Resultado == ResultadoAutenticacion.Exitoso && response.Usuario != null)
                {
                    OnLoginSuccess?.Invoke(response.Usuario);
                }
                else
                {
                    // Captura errores específicos (Usuario no encontrado, contraseña incorrecta, error DB)
                    ErrorMessage = response.Mensaje;
                }
            }
            finally
            {
                IsLoading = false;
                ((RelayCommand)IniciarSesionCommand).RaiseCanExecuteChanged();
            }
        }

        public void LimpiarFormulario()
        {
            NombreUsuario = string.Empty;
            Contrasena = string.Empty;
            ErrorMessage = string.Empty;
            IsLoading = false;
        }
    }
}
