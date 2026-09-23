using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;
using TP_ControlVehicular.Negocio.Services;
using TP_ControlVehicular.Presentacion.Validaciones;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    using Usuario = TP_ControlVehicular.Entidad.Usuario;

    public class UsuarioViewModel : BaseViewModel, INotifyDataErrorInfo
    {
        private readonly ListarUsuariosHandler _listarUsuariosHandler;
        private readonly RegistrarUsuarioHandler _registrarUsuarioHandler;
        private readonly ModificarUsuarioHandler _modificarUsuarioHandler;
        private readonly IRolRepository _rolRepository;
        private readonly IMapper _mapper;

        public UsuarioViewModel(
            ListarUsuariosHandler listarUsuariosHandler,
            RegistrarUsuarioHandler registrarUsuarioHandler,
            ModificarUsuarioHandler modificarUsuarioHandler,
            IRolRepository rolRepository,
            IMapper mapper)
        {
            _listarUsuariosHandler = listarUsuariosHandler;
            _registrarUsuarioHandler = registrarUsuarioHandler;
            _modificarUsuarioHandler = modificarUsuarioHandler;
            _rolRepository = rolRepository;
            _mapper = mapper;

            Usuarios = new ObservableCollection<UsuarioDto>();
            RolesDisponibles = new ObservableCollection<RolDto>();

            ((ObservableCollection<UsuarioDto>)Usuarios).CollectionChanged += (s, e) => OnPropertyChanged(nameof(ListadoUsuariosFiltered));
            Estado = true;
            FechaNacimiento = DateTime.Today.AddYears(-20);
            RegistrarCommand = new RelayCommand(async () => await GuardarUsuarioAsync(), () => !HasErrors);
        }

        public ObservableCollection<UsuarioDto> Usuarios { get; set; }
        public ObservableCollection<RolDto> RolesDisponibles { get; set; }

        private string _textoBusqueda = string.Empty;
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set { _textoBusqueda = value; OnPropertyChanged(); OnPropertyChanged(nameof(ListadoUsuariosFiltered)); }
        }

        public IEnumerable<UsuarioDto> ListadoUsuariosFiltered =>
            string.IsNullOrWhiteSpace(TextoBusqueda)
                ? Usuarios
                : Usuarios.Where(usuario => (usuario.Nombre ?? string.Empty).IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0
                                   || (usuario.Apellido ?? string.Empty).IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0
                                   || (usuario.Dni ?? string.Empty).IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0
                                   || (usuario.RolNombre ?? string.Empty).IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0);

        private int _idRol;
        public int IdRol
        {
            get => _idRol;
            set { _idRol = value; OnPropertyChanged(); ValidateProperty(); }
        }

        private string _nombre = string.Empty;
        public string Nombre
        {
            get => _nombre;
            set { _nombre = value; OnPropertyChanged(); ValidateProperty(); }
        }

        private string _apellido = string.Empty;
        public string Apellido
        {
            get => _apellido;
            set { _apellido = value; OnPropertyChanged(); ValidateProperty(); }
        }

        private string _dni = string.Empty;
        public string Dni
        {
            get => _dni;
            set { _dni = value; OnPropertyChanged(); ValidateProperty(); }
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); ValidateProperty(); }
        }

        private string _telefono = string.Empty;
        public string Telefono
        {
            get => _telefono;
            set { _telefono = value; OnPropertyChanged(); ValidateProperty(); }
        }

        private string _domicilio = string.Empty;
        public string Domicilio
        {
            get => _domicilio;
            set { _domicilio = value; OnPropertyChanged(); ValidateProperty(); }
        }

        private DateTime _fechaNacimiento;
        public DateTime FechaNacimiento
        {
            get => _fechaNacimiento;
            set { _fechaNacimiento = value; OnPropertyChanged(); ValidateProperty(); }
        }

        private string _contrasena = string.Empty;
        public string Contrasena
        {
            get => _contrasena;
            set { _contrasena = value; OnPropertyChanged(); ValidateProperty(); }
        }

        private bool _estado;
        public bool Estado
        {
            get => _estado;
            set { _estado = value; OnPropertyChanged(); }
        }

        public UsuarioDto? UsuarioSeleccionado { get; set; }

        public ICommand RegistrarCommand { get; }

        public async Task CargarRolesAsync(int? seleccionarIdRol = null)
        {
            var idsAnteriores = RolesDisponibles.Select(rol => rol.IdRol).ToHashSet();
            RolesDisponibles.Clear();

            try
            {
                var roles = await _rolRepository.GetActivosAsync();
                int nuevoIdRol = 0;
                foreach (var rol in roles)
                {
                    var dto = _mapper.Map<RolDto>(rol);
                    RolesDisponibles.Add(dto);
                    if (!idsAnteriores.Contains(dto.IdRol))
                    {
                        nuevoIdRol = dto.IdRol;
                    }
                }

                if (seleccionarIdRol.HasValue && RolesDisponibles.Any(rol => rol.IdRol == seleccionarIdRol.Value))
                {
                    IdRol = seleccionarIdRol.Value;
                }
                else if (nuevoIdRol > 0)
                {
                    IdRol = nuevoIdRol;
                }
            }
            catch
            {
            }
        }

        public async Task LoadAsync()
        {
            Usuarios.Clear();
            await CargarRolesAsync();

            try
            {
                var lista = await _listarUsuariosHandler.HandleAsync();
                foreach (var usuario in lista)
                {
                    Usuarios.Add(usuario);
                }
            }
            catch
            {
            }
        }

        public async Task<bool> GuardarUsuarioAsync()
        {
            if (!ValidateAll()) return false;

            var usuarioSeleccionado = UsuarioSeleccionado;
            var usuario = new Usuario
            {
                Id = usuarioSeleccionado?.IdUsuario ?? 0,
                RolId = IdRol,
                Nombre = Nombre,
                Apellido = Apellido,
                Dni = Dni,
                Email = Email,
                Telefono = Telefono,
                Domicilio = Domicilio,
                FechaNacimiento = FechaNacimiento,
                Contrasena = Contrasena,
                Estado = Estado
            };

            try
            {
                if (usuario.Id == 0)
                {
                    var dto = await _registrarUsuarioHandler.HandleAsync(usuario);
                    Usuarios.Add(dto);
                }
                else
                {
                    var dto = await _modificarUsuarioHandler.HandleAsync(usuario);
                    var index = -1;
                    for (int i = 0; i < Usuarios.Count; i++)
                    {
                        if (Usuarios[i].IdUsuario == dto.IdUsuario)
                        {
                            index = i;
                            break;
                        }
                    }
                    if (index >= 0) Usuarios[index] = dto;
                    else Usuarios.Add(dto);
                }

                RegistrationCompleted?.Invoke(this, true);
                return true;
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.Message.Contains("IX_Usuarios_Dni"))
                        RegistrationFailed?.Invoke(this, "El DNI ingresado ya se encuentra registrado en el sistema.");
                    else if (ex.InnerException.Message.Contains("IX_Usuarios_Email"))
                        RegistrationFailed?.Invoke(this, "El Email ingresado ya se encuentra registrado en el sistema.");
                    else
                        RegistrationFailed?.Invoke(this, "Error de base de datos al guardar el usuario: " + ex.InnerException.Message);
                }
                else
                {
                    RegistrationFailed?.Invoke(this, "Error al guardar el usuario en la base de datos.");
                }
                return false;
            }
            catch (Exception ex)
            {
                RegistrationFailed?.Invoke(this, "Error al guardar el usuario: " + ex.Message);
                return false;
            }
        }

        public async Task ToggleEstadoAsync()
        {
            if (UsuarioSeleccionado == null) return;
            UsuarioSeleccionado.Estado = !UsuarioSeleccionado.Estado;

            var usuario = new Usuario
            {
                Id = UsuarioSeleccionado.IdUsuario,
                RolId = UsuarioSeleccionado.IdRol,
                Nombre = UsuarioSeleccionado.Nombre,
                Apellido = UsuarioSeleccionado.Apellido,
                Dni = UsuarioSeleccionado.Dni,
                Email = UsuarioSeleccionado.Email,
                Telefono = UsuarioSeleccionado.Telefono,
                Domicilio = UsuarioSeleccionado.Domicilio,
                FechaNacimiento = UsuarioSeleccionado.FechaNacimiento,
                Contrasena = UsuarioSeleccionado.Contrasena,
                Estado = UsuarioSeleccionado.Estado
            };

            try
            {
                var dto = await _modificarUsuarioHandler.HandleAsync(usuario);
                var index = -1;
                for (int i = 0; i < Usuarios.Count; i++)
                {
                    if (Usuarios[i].IdUsuario == dto.IdUsuario)
                    {
                        index = i;
                        break;
                    }
                }
                if (index >= 0) Usuarios[index] = dto;
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                UsuarioSeleccionado.Estado = !UsuarioSeleccionado.Estado; // Rollback visual
                RegistrationFailed?.Invoke(this, "Error de base de datos al actualizar estado del usuario: " + (ex.InnerException?.Message ?? ex.Message));
            }
            catch (Exception ex)
            {
                UsuarioSeleccionado.Estado = !UsuarioSeleccionado.Estado; // Rollback visual
                RegistrationFailed?.Invoke(this, "Error al actualizar estado del usuario: " + ex.Message);
            }
        }

        public event EventHandler<bool>? RegistrationCompleted;
        public event EventHandler<string>? RegistrationFailed;

        /// <summary>
        /// Realiza la validación en tiempo real de una propiedad individual del formulario al perder el foco (LostFocus).
        /// Utiliza la clase estática ValidadorGlobal para evaluar expresiones regulares (RegEx) y reglas de negocio.
        /// Reporta los errores a la interfaz mediante INotifyDataErrorInfo (Validation.Errors en WPF).
        /// </summary>
        /// <param name="propertyName">Nombre de la propiedad modificada.</param>
        /// <returns>True si la propiedad no tiene errores, False en caso contrario.</returns>
        public override bool ValidateProperty([CallerMemberName] string? propertyName = null)
        {
            if (propertyName is null) return true;
            ClearErrors(propertyName);

            switch (propertyName)
            {
                case nameof(Nombre):
                    if (!ValidadorGlobal.EsTextoValido(Nombre, 2))
                        SetError(nameof(Nombre), "Nombre requerido (mínimo 2 caracteres).");
                    break;
                case nameof(Apellido):
                    if (!ValidadorGlobal.EsRequerido(Apellido))
                        SetError(nameof(Apellido), "Apellido requerido.");
                    break;
                case nameof(Dni):
                    if (!ValidadorGlobal.EsRequerido(Dni))
                        SetError(nameof(Dni), "DNI requerido.");
                    else if (!ValidadorGlobal.EsDniValido(Dni))
                        SetError(nameof(Dni), "DNI inválido (debe contener entre 7 y 10 dígitos numéricos).");
                    break;
                case nameof(Email):
                    if (!ValidadorGlobal.EsRequerido(Email))
                        SetError(nameof(Email), "Email requerido.");
                    else if (!ValidadorGlobal.EsEmailValido(Email))
                        SetError(nameof(Email), "Formato de email inválido.");
                    break;
                case nameof(Telefono):
                    if (!ValidadorGlobal.EsRequerido(Telefono))
                        SetError(nameof(Telefono), "Teléfono requerido.");
                    else if (!ValidadorGlobal.EsTelefonoValido(Telefono))
                        SetError(nameof(Telefono), "Formato de teléfono inválido (ej: 362 4615825).");
                    break;
                case nameof(Domicilio):
                    if (!ValidadorGlobal.EsRequerido(Domicilio))
                        SetError(nameof(Domicilio), "Domicilio requerido.");
                    break;
                case nameof(FechaNacimiento):
                    if (!ValidadorGlobal.EsMayorDeEdad(FechaNacimiento, 18))
                        SetError(nameof(FechaNacimiento), "El usuario debe ser mayor de 18 años.");
                    break;
                case nameof(Contrasena):
                    if (!ValidadorGlobal.EsRequerido(Contrasena))
                        SetError(nameof(Contrasena), "Contraseña requerida.");
                    else if (!ValidadorGlobal.EsContrasenaValida(Contrasena))
                        SetError(nameof(Contrasena), "La contraseña debe tener mín. 6 caracteres, incluir al menos una mayúscula, un número y un carácter especial (#, @, etc.).");
                    break;
                case nameof(IdRol):
                    if (IdRol <= 0)
                        SetError(nameof(IdRol), "Debe seleccionar un Rol de la lista.");
                    break;
            }

            return !GetErrors(propertyName).Cast<object>().Any();
        }

        /// <summary>
        /// Evalúa la totalidad de los campos del formulario antes de intentar guardar el registro.
        /// </summary>
        /// <returns>True si todos los campos son válidos y no existen errores en la vista.</returns>
        public bool ValidateAll()
        {
            ValidateProperty(nameof(Nombre));
            ValidateProperty(nameof(Apellido));
            ValidateProperty(nameof(Dni));
            ValidateProperty(nameof(Email));
            ValidateProperty(nameof(Telefono));
            ValidateProperty(nameof(Domicilio));
            ValidateProperty(nameof(FechaNacimiento));
            ValidateProperty(nameof(Contrasena));
            ValidateProperty(nameof(IdRol));
            return !HasErrors;
        }
    }
}
