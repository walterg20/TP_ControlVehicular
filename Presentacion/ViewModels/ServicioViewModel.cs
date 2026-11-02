// walterg20
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Services;
using TP_ControlVehicular.Presentacion.Helpers;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    /// <summary>
    /// Listado de servicios con paginacion y busqueda en la base de datos.
    /// </summary>
    public class ServicioViewModel : BasePaginableViewModel
    {
        protected override string Modulo => "Servicio";

        private readonly ListarServiciosHandler _listarServiciosHandler;
        private readonly RegistrarServicioHandler _registrarServicioHandler;
        private readonly ModificarServicioHandler _modificarServicioHandler;
        private readonly EliminarServicioHandler _eliminarServicioHandler;
        private readonly ReactivarServicioHandler _reactivarServicioHandler;
        private readonly Debouncer _debouncer;

        private ServicioDto? _servicioSeleccionado;
        public ServicioDto? ServicioSeleccionado
        {
            get => _servicioSeleccionado;
            set => SetProperty(ref _servicioSeleccionado, value);
        }

        private int _idServicio;
        public int IdServicio
        {
            get => _idServicio;
            set => SetProperty(ref _idServicio, value);
        }

        private string _nombre = string.Empty;
        public string Nombre
        {
            get => _nombre;
            set { SetProperty(ref _nombre, value); ValidateProperty(); }
        }

        private decimal _precio;
        public decimal Precio
        {
            get => _precio;
            set { SetProperty(ref _precio, value); ValidateProperty(); }
        }

        private bool _activo = true;
        public bool Activo
        {
            get => _activo;
            set { SetProperty(ref _activo, value); }
        }

        private bool _mostrarInactivos;
        public bool MostrarInactivos
        {
            get => _mostrarInactivos;
            set
            {
                if (SetProperty(ref _mostrarInactivos, value))
                {
                    // El filtro cambio: el listado vuelve a la primera pagina.
                    ReiniciarPagina();
                }
            }
        }

        private string _textoBusqueda = string.Empty;
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                if (SetProperty(ref _textoBusqueda, value))
                {
                    // El filtro cambio: el listado vuelve a la primera pagina.
                    ReiniciarPagina();
                }
            }
        }

        /// <summary>Registros de la pagina actual.</summary>
        public ObservableCollection<ServicioDto> Servicios { get; } = new();

        public ICommand CargarServiciosCommand { get; }

        public ServicioViewModel(
            ListarServiciosHandler listarServiciosHandler,
            RegistrarServicioHandler registrarServicioHandler,
            ModificarServicioHandler modificarServicioHandler,
            EliminarServicioHandler eliminarServicioHandler,
            ReactivarServicioHandler reactivarServicioHandler)
        {
            _listarServiciosHandler = listarServiciosHandler;
            _registrarServicioHandler = registrarServicioHandler;
            _modificarServicioHandler = modificarServicioHandler;
            _eliminarServicioHandler = eliminarServicioHandler;
            _reactivarServicioHandler = reactivarServicioHandler;
            _debouncer = new Debouncer();

            CargarServiciosCommand = new RelayCommand(LoadAsync);
        }

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
                var (items, total) = await _listarServiciosHandler.HandlePaginadoAsync(
                    TextoBusqueda.Trim(),
                    MostrarInactivos,
                    PaginaActual,
                    TamanoPagina);

                Servicios.Clear();
                foreach (var item in items)
                {
                    Servicios.Add(item);
                }

                ActualizarTotales(total);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public override bool ValidateProperty([CallerMemberName] string? propertyName = null)
        {
            if (propertyName is null) return true;
            ClearErrors(propertyName);

            switch (propertyName)
            {
                case nameof(Nombre):
                    if (string.IsNullOrWhiteSpace(Nombre))
                        SetError(nameof(Nombre), "El nombre del servicio es obligatorio.");
                    else if (Nombre.Trim().Length < 3)
                        SetError(nameof(Nombre), "El nombre del servicio debe tener al menos 3 caracteres.");
                    break;
                case nameof(Precio):
                    if (Precio <= 0)
                        SetError(nameof(Precio), "El precio del servicio debe ser mayor a 0.");
                    break;
            }

            return !GetErrors(propertyName).Cast<object>().Any();
        }

        public bool ValidateAll()
        {
            ValidateProperty(nameof(Nombre));
            ValidateProperty(nameof(Precio));
            return !HasErrors;
        }

        public async Task<bool> SaveAsync()
        {
            if (!ValidateAll()) return false;

            try
            {
                var entity = new Servicio
                {
                    Id = IdServicio,
                    Nombre = Nombre.Trim(),
                    Precio = Precio,
                    Activo = Activo
                };

                if (IdServicio == 0)
                {
                    await _registrarServicioHandler.HandleAsync(entity);
                }
                else
                {
                    await _modificarServicioHandler.HandleAsync(entity);
                }

                LimpiarFormulario();
                await LoadAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task DeleteAsync(int id)
        {
            await _eliminarServicioHandler.HandleAsync(id);
            await LoadAsync();
        }

        public async Task ReactivarAsync(int id)
        {
            await _reactivarServicioHandler.HandleAsync(id);
            await LoadAsync();
        }

        public void LimpiarFormulario()
        {
            IdServicio = 0;
            Nombre = string.Empty;
            Precio = 0;
            Activo = true;
            ServicioSeleccionado = null;
            ClearAllErrors();
        }
    }
}
