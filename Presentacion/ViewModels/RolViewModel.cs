using AutoMapper;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Services;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public class RolViewModel : BaseViewModel, INotifyDataErrorInfo
    {
        private readonly ListarRolHandler _listarRolHandler;
        private readonly RegistrarRolHandler _registrarRolHandler;
        private readonly ModificarRolHandler _modificarRolHandler;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, List<string>> _errors = new();

        public RolViewModel(
            ListarRolHandler listarRolHandler,
            RegistrarRolHandler registrarRolHandler,
            ModificarRolHandler modificarRolHandler,
            IMapper mapper)
        {
            _listarRolHandler = listarRolHandler;
            _registrarRolHandler = registrarRolHandler;
            _modificarRolHandler = modificarRolHandler;
            _mapper = mapper;
            Roles = new ObservableCollection<RolDto>();
            ((ObservableCollection<RolDto>)Roles).CollectionChanged += (s, e) => OnPropertyChanged(nameof(ListadoRolesFiltered));
            Estado = true;
            RegistrarCommand = new RelayCommand(async () => await GuardarRolAsync(), () => !HasErrors);
        }

        public ObservableCollection<RolDto> Roles { get; set; }

        private string _textoBusqueda = string.Empty;
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set { _textoBusqueda = value; OnPropertyChanged(); OnPropertyChanged(nameof(ListadoRolesFiltered)); }
        }

        public IEnumerable<RolDto> ListadoRolesFiltered =>
            string.IsNullOrWhiteSpace(TextoBusqueda)
                ? Roles
                : Roles.Where(rol => (rol.Nombre ?? string.Empty).IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0);

        private string _nombre = string.Empty;
        public string Nombre
        {
            get => _nombre;
            set { _nombre = value; OnPropertyChanged(); ValidateProperty(); }
        }

        private string _descripcion = string.Empty;
        public string Descripcion
        {
            get => _descripcion;
            set { _descripcion = value; OnPropertyChanged(); }
        }

        private bool _estado;
        public bool Estado
        {
            get => _estado;
            set { _estado = value; OnPropertyChanged(); }
        }

        public RolDto? RolSeleccionado { get; set; }

        public ICommand RegistrarCommand { get; }

        public async Task LoadAsync()
        {
            Roles.Clear();
            try
            {
                var lista = await _listarRolHandler.HandleAsync();
                foreach (var rol in lista)
                {
                    Roles.Add(rol);
                }
            }
            catch
            {
                // Silenciar error en carga inicial
            }
        }

        public async Task<bool> GuardarRolAsync()
        {
            if (!ValidateAll()) return false;

            var rolSeleccionado = RolSeleccionado;
            var rol = new Entidad.Rol
            {
                Id = rolSeleccionado?.IdRol ?? 0,
                Nombre = Nombre,
                Descripcion = Descripcion,
                Estado = Estado
            };

            try
            {
                if (rol.Id == 0)
                {
                    var dto = await _registrarRolHandler.HandleAsync(rol);
                    Roles.Add(dto);
                }
                else
                {
                    var dto = await _modificarRolHandler.HandleAsync(rol);
                    var index = -1;
                    for (int i = 0; i < Roles.Count; i++)
                    {
                        if (Roles[i].IdRol == dto.IdRol)
                        {
                            index = i;
                            break;
                        }
                    }
                    if (index >= 0) Roles[index] = dto;
                    else Roles.Add(dto);
                }

                RegistrationCompleted?.Invoke(this, true);
                return true;
            }
            catch (Exception ex)
            {
                RegistrationFailed?.Invoke(this, "Error al guardar el rol: " + ex.Message);
                return false;
            }
        }

        public async Task ToggleEstadoAsync()
        {
            if (RolSeleccionado == null) return;
            RolSeleccionado.Estado = !RolSeleccionado.Estado;

            var rol = new Entidad.Rol
            {
                Id = RolSeleccionado.IdRol,
                Nombre = RolSeleccionado.Nombre,
                Descripcion = RolSeleccionado.Descripcion,
                Estado = RolSeleccionado.Estado
            };

            try
            {
                var dto = await _modificarRolHandler.HandleAsync(rol);
                var index = -1;
                for (int i = 0; i < Roles.Count; i++)
                {
                    if (Roles[i].IdRol == dto.IdRol)
                    {
                        index = i;
                        break;
                    }
                }
                if (index >= 0) Roles[index] = dto;
            }
            catch (Exception ex)
            {
                RegistrationFailed?.Invoke(this, "Error al actualizar estado del rol: " + ex.Message);
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
                        SetError(nameof(Nombre), "Nombre de Rol requerido (mínimo 5 caracteres).");
                    break;
            }

            return !GetErrors(propertyName).Cast<object>().Any();
        }

        public bool ValidateAll()
        {
            ValidateProperty(nameof(Nombre));
            return !HasErrors;
        }
    }
}
