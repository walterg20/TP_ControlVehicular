// walterg20
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Services;
using TP_ControlVehicular.Negocio.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using TP_ControlVehicular.Presentacion.Helpers;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    /// <summary>
    /// Listado de talleres con paginacion y busqueda en la base de datos.
    /// </summary>
    public class TallerViewModel : BasePaginableViewModel, INotifyDataErrorInfo
    {
        private readonly ListarTallerHandler _listarTallerHandler;
        private readonly RegistrarTallerHandler _registrarTallerHandler;
        private readonly ModificarTallerHandler _modificarTallerHandler;
        private readonly ReactivarTallerHandler _reactivarTallerHandler;
        private readonly IMapper _mapper;
        private readonly Debouncer _debouncer;
        private readonly Dictionary<string, List<string>> _errors = new();

        public TallerViewModel(ListarTallerHandler listarTallerHandler, RegistrarTallerHandler registrarTallerHandler, ModificarTallerHandler modificarTallerHandler, ReactivarTallerHandler reactivarTallerHandler, IMapper mapper)
        {
            _listarTallerHandler = listarTallerHandler;
            _registrarTallerHandler = registrarTallerHandler;
            _modificarTallerHandler = modificarTallerHandler;
            _reactivarTallerHandler = reactivarTallerHandler;
            _mapper = mapper;
            _debouncer = new Debouncer();
            Talleres = new ObservableCollection<TallerDto>();
            Activo = true;
            RegistrarCommand = new RelayCommand(async () => await RegistrarAsync(), () => !HasErrors);
        }

        public ObservableCollection<TallerDto> Talleres { get; set; }

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

        private string _direccion = string.Empty;
        public string Direccion
        {
            get => _direccion;
            set { _direccion = value; OnPropertyChanged(); ValidateProperty(); }
        }

        private string _telefono = string.Empty;
        public string Telefono
        {
            get => _telefono;
            set { _telefono = value; OnPropertyChanged(); }
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
                var (items, total) = await _listarTallerHandler.HandlePaginadoAsync(
                    TextoBusqueda.Trim(),
                    MostrarInactivos,
                    PaginaActual,
                    TamanoPagina);

                Talleres.Clear();
                foreach (var taller in items)
                {
                    Talleres.Add(taller);
                }

                ActualizarTotales(total);
            }
            catch
            {
                // Si falla, la colección se queda vacía
                Talleres.Clear();
                ActualizarTotales(0);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void LimpiarFormulario()
        {
            TallerSeleccionado = null;
            Nombre = string.Empty;
            Direccion = string.Empty;
            Telefono = string.Empty;
            Activo = true;
            ClearAllErrors();
        }

        public async Task<bool> RegistrarAsync()
        {
            return await GuardarTallerAsync();
        }

        // Guarda un taller: si es nuevo (id == 0) usa RegistrarTallerHandler,
        // si existe (id > 0) usa ModificarTallerHandler. Retorna true si tuvo éxito.
        public async Task<bool> GuardarTallerAsync()
        {
            if (!ValidateAll()) return false;

            int id = TallerSeleccionado?.IdTaller ?? 0;

            var taller = new TP_ControlVehicular.Entidad.Taller
            {
                Id = id,
                Nombre = Nombre.Trim(),
                Direccion = Direccion.Trim(),
                Telefono = (Telefono ?? string.Empty).Trim(),
                Activo = Activo
            };

            try
            {
                if (id == 0)
                {
                    await _registrarTallerHandler.HandleAsync(taller);
                }
                else
                {
                    await _modificarTallerHandler.HandleAsync(taller);
                }

                // Recargar la pagina actual para que el listado refleje el alta o la edicion.
                await CargarPaginaAsync();

                RegistrationCompleted?.Invoke(this, true);
                return true;
            }
            catch (Exception ex)
            {
                RegistrationFailed?.Invoke(this, "Error al guardar taller: " + ex.Message);
                return false;
            }
        }

        // Evento: éxito/fracaso del registro
        public event EventHandler<bool>? RegistrationCompleted;
        // Evento: error de registro con mensaje
        public event EventHandler<string>? RegistrationFailed;

        public override bool ValidateProperty([CallerMemberName] string? propertyName = null)
        {
            if (propertyName is null) return true;
            ClearErrors(propertyName);

            switch (propertyName)
            {
                case nameof(Nombre):
                    if (string.IsNullOrWhiteSpace(Nombre) || Nombre.Trim().Length < 5)
                        SetError(nameof(Nombre), "Nombre del Taller requerido (mínimo 5 caracteres).");
                    break;
                case nameof(Direccion):
                    if (string.IsNullOrWhiteSpace(Direccion) || Direccion.Trim().Length < 5)
                        SetError(nameof(Direccion), "Dirección del Taller requerida (mínimo 5 caracteres).");
                    break;
            }

            return !GetErrors(propertyName).Cast<object>().Any();
        }

        public bool ValidateAll()
        {
            ValidateProperty(nameof(Nombre));
            ValidateProperty(nameof(Direccion));
            return !HasErrors;
        }

        // Para modificar un taller existente (cargar datos en el form)
        public TallerDto? TallerSeleccionado { get; set; }

        // Para baja lógica (toggle Activo): usa ModificarTallerHandler para persistir
        public async Task ToggleActivoAsync()
        {
            if (TallerSeleccionado == null) return;

            TallerSeleccionado.Activo = !TallerSeleccionado.Activo;

            var taller = new TP_ControlVehicular.Entidad.Taller
            {
                Id = TallerSeleccionado.IdTaller,
                Nombre = TallerSeleccionado.Nombre,
                Direccion = TallerSeleccionado.Direccion,
                Telefono = TallerSeleccionado.Telefono,
                Activo = TallerSeleccionado.Activo
            };

            try
            {
                await _modificarTallerHandler.HandleAsync(taller);
                await CargarPaginaAsync();
            }
            catch (Exception ex)
            {
                RegistrationFailed?.Invoke(this, "Error al actualizar taller: " + ex.Message);
            }
        }
        // Reactiva un taller previamente dado de baja (Activo = true)
        public async Task ReactivarAsync(int id)
        {
            await _reactivarTallerHandler.HandleAsync(id);
            await LoadAsync();
        }
    }
}
