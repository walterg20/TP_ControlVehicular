using AutoMapper;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;
using TP_ControlVehicular.Negocio.Services;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public class UsuarioViewModel : BaseViewModel, INotifyDataErrorInfo
    {
        private readonly ListarUsuariosHandler _listarUsuariosHandler;
        private readonly RegistrarUsuarioHandler _registrarUsuarioHandler;
        private readonly ModificarUsuarioHandler _modificarUsuarioHandler;
        private readonly IRolRepository _rolRepository;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, List<string>> _errors = new();

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
                // Silenciar error en carga de roles
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
                // Silenciar error en carga inicial
            }
        }

        public async Task<bool> GuardarUsuarioAsync()
        {
            if (!ValidateAll()) return false;

            var usuarioSeleccionado = UsuarioSeleccionado;
            var usuario = new Entidad.Usuario
            {
                Id = usuarioSeleccionado?.IdUsuario ?? 0,
                RolId = IdRol,
                Nombre = Nombre,
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

            var usuario = new Entidad.Usuario
            {
                Id = UsuarioSeleccionado.IdUsuario,
                RolId = UsuarioSeleccionado.IdRol,
                Nombre = UsuarioSeleccionado.Nombre,
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
            catch (Exception ex)
            {
                RegistrationFailed?.Invoke(this, "Error al actualizar estado del usuario: " + ex.Message);
            }
        }

        public event EventHandler<bool>? RegistrationCompleted;
        public event EventHandler<string>? RegistrationFailed;

        public override bool ValidateProperty([CallerMemberName] string? propertyName = null)
        {
            if (propertyName is null) return true;
            ClearErrors(propertyName);

            switch (propertyName)
            {
                case nameof(Nombre):
                    if (string.IsNullOrWhiteSpace(Nombre) || Nombre.Trim().Length < 5)
                        SetError(nameof(Nombre), "Nombre requerido (mínimo 5 caracteres).");
                    break;
                case nameof(Contrasena):
                    if (string.IsNullOrWhiteSpace(Contrasena))
                        SetError(nameof(Contrasena), "Contraseña requerida.");
                    else if (!System.Text.RegularExpressions.Regex.IsMatch(Contrasena, @"^(?=.*[A-Z])(?=.*\d)(?=.*[#@!$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]).{6,}$"))
                        SetError(nameof(Contrasena), "La contraseña debe tener mín. 6 caracteres, incluir al menos una mayúscula, un número y un carácter especial (#, @, etc.).");
                    break;
                case nameof(IdRol):
                    if (IdRol <= 0)
                        SetError(nameof(IdRol), "Debe seleccionar un Rol de la lista.");
                    break;
            }

            return !GetErrors(propertyName).Cast<object>().Any();
        }

        public bool ValidateAll()
        {
            ValidateProperty(nameof(Nombre));
            ValidateProperty(nameof(Contrasena));
            ValidateProperty(nameof(IdRol));
            return !HasErrors;
        }
    }
}
