// walterg20
using AutoMapper;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Services;
using TP_ControlVehicular.Presentacion.Helpers;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    /// <summary>
    /// Listado de roles con paginacion y busqueda en la base de datos.
    /// </summary>
    public class RolViewModel : BasePaginableViewModel, INotifyDataErrorInfo
    {
        private readonly ListarRolHandler _listarRolHandler;
        private readonly RegistrarRolHandler _registrarRolHandler;
        private readonly ModificarRolHandler _modificarRolHandler;
        private readonly ReactivarRolHandler _reactivarRolHandler;
        private readonly IMapper _mapper;
        private readonly Debouncer _debouncer;
        private readonly Dictionary<string, List<string>> _errors = new();

        public RolViewModel(
            ListarRolHandler listarRolHandler,
            RegistrarRolHandler registrarRolHandler,
            ModificarRolHandler modificarRolHandler,
            ReactivarRolHandler reactivarRolHandler,
            IMapper mapper)
        {
            _listarRolHandler = listarRolHandler;
            _registrarRolHandler = registrarRolHandler;
            _modificarRolHandler = modificarRolHandler;
            _reactivarRolHandler = reactivarRolHandler;
            _mapper = mapper;
            _debouncer = new Debouncer();
            Roles = new ObservableCollection<RolDto>();
            Activo = true;
            RegistrarCommand = new RelayCommand(async () => await GuardarRolAsync(), () => !HasErrors);
        }

        public ObservableCollection<RolDto> Roles { get; set; }

        private string _textoBusqueda = string.Empty;
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                _textoBusqueda = value;
                OnPropertyChanged();
                // El filtro cambio: el listado vuelve a la primera pagina.
                ReiniciarPagina();
            }
        }

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

        private bool _activo;
        public bool Activo
        {
            get => _activo;
            set { _activo = value; OnPropertyChanged(); }
        }

        private bool _mostrarInactivos;
        public bool MostrarInactivos
        {
            get => _mostrarInactivos;
            set
            {
                if (_mostrarInactivos != value)
                {
                    _mostrarInactivos = value;
                    OnPropertyChanged(nameof(MostrarInactivos));
                    // El filtro cambio: el listado vuelve a la primera pagina.
                    ReiniciarPagina();
                }
            }
        }

        public RolDto? RolSeleccionado { get; set; }

        public ICommand RegistrarCommand { get; }

        /// <summary>
        /// Carga la primera pagina del listado (usada al abrir la pantalla).
        /// </summary>
        public async Task LoadAsync()
        {
            ReiniciarPagina();
            await CargarPaginaAsync();
        }

        /// <summary>
        /// Aplica los filtros actuales y recarga el listado desde la primera pagina.
        /// </summary>
        public async Task BuscarAsync()
        {
            ReiniciarPagina();
            await CargarPaginaAsync();
        }

        /// <summary>
        /// Programa la busqueda con un retardo breve, para no consultar la base de datos
        /// en cada tecla escrita.
        /// </summary>
        public void ProgramarBusqueda() => _debouncer.Debounce(BuscarAsync);

        /// <summary>Cancela una busqueda programada que todavia no se ejecuto.</summary>
        public void CancelarBusquedaProgramada() => _debouncer.Cancel();

        protected override async Task CargarPaginaAsync()
        {
            IsLoading = true;
            try
            {
                var (items, total) = await _listarRolHandler.HandlePaginadoAsync(
                    TextoBusqueda.Trim(),
                    MostrarInactivos,
                    PaginaActual,
                    TamanoPagina);

                Roles.Clear();
                foreach (var rol in items)
                {
                    Roles.Add(rol);
                }

                ActualizarTotales(total);
            }
            catch
            {
                // Silenciar error en carga inicial
                Roles.Clear();
                ActualizarTotales(0);
            }
            finally
            {
                IsLoading = false;
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
                Activo = Activo
            };

            try
            {
                if (rol.Id == 0)
                {
                    await _registrarRolHandler.HandleAsync(rol);
                }
                else
                {
                    await _modificarRolHandler.HandleAsync(rol);
                }

                // Recargar la pagina actual para que el listado refleje el alta o la edicion.
                await CargarPaginaAsync();

                RegistrationCompleted?.Invoke(this, true);
                return true;
            }
            catch (Exception ex)
            {
                RegistrationFailed?.Invoke(this, "Error al guardar el rol: " + ex.Message);
                return false;
            }
        }

        public async Task ToggleActivoAsync()
        {
            if (RolSeleccionado == null) return;
            RolSeleccionado.Activo = !RolSeleccionado.Activo;

            var rol = new Entidad.Rol
            {
                Id = RolSeleccionado.IdRol,
                Nombre = RolSeleccionado.Nombre,
                Descripcion = RolSeleccionado.Descripcion,
                Activo = RolSeleccionado.Activo
            };

            try
            {
                await _modificarRolHandler.HandleAsync(rol);
                await CargarPaginaAsync();
            }
            catch (Exception ex)
            {
                RegistrationFailed?.Invoke(this, "Error al actualizar estado del rol: " + ex.Message);
            }
        }

        public async Task ReactivarAsync(int id)
        {
            await _reactivarRolHandler.HandleAsync(id);
            await LoadAsync();
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
