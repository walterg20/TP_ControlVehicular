// walterg20
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Services;
using TP_ControlVehicular.Presentacion.Helpers;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    /// <summary>
    /// Listado de clientes con paginacion y busqueda en la base de datos.
    /// </summary>
    public class ClienteViewModel : BasePaginableViewModel
    {
        protected override string Modulo => "Cliente";

        private readonly RegistrarClienteHandler _registrarClienteHandler;
        private readonly TP_ControlVehicular.Negocio.Interfaces.IClienteRepository _clienteRepository;
        private readonly AutoMapper.IMapper _mapper;
        private readonly Debouncer _debouncer;
        private readonly Dictionary<string, List<string>> _errors = new();

        public ClienteViewModel(RegistrarClienteHandler registrarClienteHandler, TP_ControlVehicular.Negocio.Interfaces.IClienteRepository clienteRepository, AutoMapper.IMapper mapper)
        {
            _registrarClienteHandler = registrarClienteHandler;
            _clienteRepository = clienteRepository;
            _mapper = mapper;
            _debouncer = new Debouncer();
            Clientes = new ObservableCollection<ClienteDto>();
            Activo = true; // por defecto
            RegistrarCommand = new RelayCommand(async () => await RegistrarClienteAsync(), () => !HasErrors);
        }

        public ObservableCollection<ClienteDto> Clientes { get; set; }

        private string _filtroEstado = "Todos";
        public string FiltroEstado
        {
            get => _filtroEstado;
            set
            {
                if (_filtroEstado == value) return;
                _filtroEstado = value;
                OnPropertyChanged();
                ReiniciarPagina();
                _ = CargarPaginaAsync();
            }
        }

        public bool PuedeVerFiltroEstado => PuedeEliminar;

        public List<string> EstadosDisponibles { get; } = new List<string> { "Todos", "Activos", "Inactivos" };

        public bool MostrarInactivos
        {
            get => FiltroEstado != "Activos";
            set => FiltroEstado = value ? "Todos" : "Activos";
        }

        public new void NotificarPermisos()
        {
            base.NotificarPermisos();
            OnPropertyChanged(nameof(PuedeVerFiltroEstado));
        }

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

        public int IdCliente { get; set; }

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

        private DateTime? _fechaNacimiento = null;
        public DateTime? FechaNacimiento
        {
            get => _fechaNacimiento;
            set { _fechaNacimiento = value; OnPropertyChanged(); }
        }

        private string _direccion = string.Empty;
        public string Direccion
        {
            get => _direccion;
            set { _direccion = value; OnPropertyChanged(); ValidateProperty(); }
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

        private bool _activo;
        public bool Activo
        {
            get => _activo;
            set { _activo = value; OnPropertyChanged(); }
        }

        public ClienteDto? ClienteSeleccionado { get; set; }
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
        /// Es la accion que ejecutan el boton Buscar y la tecla Enter.
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

        /// <summary>
        /// Cancela una busqueda programada que todavia no se ejecuto (por ejemplo, cuando
        /// el usuario presiona Enter y la busqueda se ejecuta de inmediato).
        /// </summary>
        public void CancelarBusquedaProgramada() => _debouncer.Cancel();

        /// <summary>
        /// Recupera de la base de datos los registros de la pagina actual.
        /// </summary>
        protected override async Task CargarPaginaAsync()
        {
            IsLoading = true;
            try
            {
                var estado = PuedeVerFiltroEstado ? FiltroEstado : "Activos";
                var (items, total) = await _clienteRepository.GetPaginadoAsync(
                    TextoBusqueda.Trim(),
                    estado,
                    PaginaActual,
                    TamanoPagina);

                Clientes.Clear();
                foreach (var cliente in items)
                {
                    Clientes.Add(_mapper.Map<ClienteDto>(cliente));
                }

                ActualizarTotales(total);
            }
            catch (Exception ex)
            {
                (App.ServiceProvider?.GetService(typeof(IErrorNotifier)) as IErrorNotifier)?.Notify(ex, "ClienteViewModel");
                Clientes.Clear();
                ActualizarTotales(0);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task<bool> DeleteClienteAsync(int id)
        {
            try
            {
                var entity = await _clienteRepository.GetByIdAsync(id);
                if (entity == null) return false;
                entity.Activo = false;
                await _clienteRepository.UpdateAsync(entity);
                await CargarPaginaAsync();
                return true;
            }
            catch (Exception ex)
            {
                (App.ServiceProvider?.GetService(typeof(IErrorNotifier)) as IErrorNotifier)?.Notify(ex, "ClienteViewModel");
                return false;
            }
        }

        public async Task<bool> ReactivarClienteAsync(int id)
        {
            try
            {
                var entity = await _clienteRepository.GetByIdAsync(id);
                if (entity == null) return false;
                entity.Activo = true;
                await _clienteRepository.UpdateAsync(entity);
                await CargarPaginaAsync();
                return true;
            }
            catch (Exception ex)
            {
                (App.ServiceProvider?.GetService(typeof(IErrorNotifier)) as IErrorNotifier)?.Notify(ex, "ClienteViewModel");
                return false;
            }
        }

        public async Task<bool> RegistrarClienteAsync()
        {
            if (!ValidateAll()) return false;

            var cliente = new TP_ControlVehicular.Entidad.Cliente
            {
                Nombre = Nombre,
                Apellido = Apellido,
                Dni = Dni,
                FechaNacimiento = FechaNacimiento ?? DateTime.MinValue,
                Direccion = Direccion,
                Email = Email,
                Telefono = Telefono,
                Activo = Activo
            };

            try
            {
                var dto = await _registrarClienteHandler.HandleAsync(cliente);
                // Recargar la pagina actual para que el listado refleje el alta o la edicion.
                await CargarPaginaAsync();
                RegistrationCompleted?.Invoke(this, true);
                return true;
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                // intentar detectar violaciones de unique constraint en SQL Server
                if (dbEx.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
                {
                    // error de clave duplicada (posible DNI repetido)
                    _errors[nameof(Dni)] = new List<string> { "Ya existe un cliente con ese DNI." };
                    OnErrorsChanged(nameof(Dni));
                    RegistrationFailed?.Invoke(this, "Ya existe un cliente con ese DNI.");
                    return false;
                }

                RegistrationFailed?.Invoke(this, "Error al guardar cliente: " + dbEx.Message);
                return false;
            }
            catch (Exception ex)
            {
                RegistrationFailed?.Invoke(this, "Error al guardar cliente: " + ex.Message);
                return false;
            }
        }

        // Evento para notificar a la vista que el registro se completó (éxito/fallo)
        public event EventHandler<bool>? RegistrationCompleted;
        // Evento para notificar errores de registro con mensaje (p.ej. DNI duplicado)
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
                case nameof(Apellido):
                    if (string.IsNullOrWhiteSpace(Apellido) || Apellido.Trim().Length < 5)
                        SetError(nameof(Apellido), "Apellido requerido (mínimo 5 caracteres).");
                    break;
                case nameof(Dni):
                    if (string.IsNullOrWhiteSpace(Dni))
                        SetError(nameof(Dni), "DNI requerido.");
                    else if (!Regex.IsMatch(Dni, @"^\d{7,8}$"))
                        SetError(nameof(Dni), "DNI debe ser numérico (7-8 dígitos).");
                    break;
                case nameof(Email):
                    if (!string.IsNullOrWhiteSpace(Email) && !Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                        SetError(nameof(Email), "Formato de correo electrónico inválido.");
                    break;
            }

            return !GetErrors(propertyName).Cast<object>().Any();
        }

        public bool ValidateAll()
        {
            ValidateProperty(nameof(Nombre));
            ValidateProperty(nameof(Apellido));
            ValidateProperty(nameof(Dni));
            ValidateProperty(nameof(Email));
            return !HasErrors;
        }
    }
}
