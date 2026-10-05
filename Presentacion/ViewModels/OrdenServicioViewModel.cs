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
    public class OrdenServicioViewModel : BaseViewModel, INotifyDataErrorInfo
    {
        private readonly ListarRegistroServiciosHandler _listarHandler;
        private readonly RegistrarRegistroServicioHandler _registrarHandler;
        private readonly ModificarRegistroServicioHandler _modificarHandler;
        private readonly IVehiculoRepository _vehiculoRepository;
        private readonly ITallerRepository _tallerRepository;
        private readonly IServicioRepository _servicioRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly TP_ControlVehicular.Negocio.Handlers.Reportes.GenerarComprobanteOrdenHandler _generarComprobanteOrdenHandler;
        private readonly TP_ControlVehicular.Negocio.Handlers.Reportes.GenerarComprobantePagoHandler _generarComprobantePagoHandler;
        private readonly IMapper _mapper;

        public OrdenServicioViewModel(
            ListarRegistroServiciosHandler listarHandler,
            RegistrarRegistroServicioHandler registrarHandler,
            ModificarRegistroServicioHandler modificarHandler,
            IVehiculoRepository vehiculoRepository,
            ITallerRepository tallerRepository,
            IServicioRepository servicioRepository,
            IUsuarioRepository usuarioRepository,
            IClienteRepository clienteRepository,
            TP_ControlVehicular.Negocio.Handlers.Reportes.GenerarComprobanteOrdenHandler generarComprobanteOrdenHandler,
            TP_ControlVehicular.Negocio.Handlers.Reportes.GenerarComprobantePagoHandler generarComprobantePagoHandler,
            IMapper mapper)
        {
            _listarHandler = listarHandler;
            _registrarHandler = registrarHandler;
            _modificarHandler = modificarHandler;
            _vehiculoRepository = vehiculoRepository;
            _tallerRepository = tallerRepository;
            _servicioRepository = servicioRepository;
            _usuarioRepository = usuarioRepository;
            _clienteRepository = clienteRepository;
            _generarComprobanteOrdenHandler = generarComprobanteOrdenHandler;
            _generarComprobantePagoHandler = generarComprobantePagoHandler;
            _mapper = mapper;

            Ordenes = new ObservableCollection<RegistroServicioDto>();
            ClientesDisponibles = new ObservableCollection<ClienteDto>();
            VehiculosDisponibles = new ObservableCollection<VehiculoDto>();
            TalleresDisponibles = new ObservableCollection<TallerDto>();
            EstadosDisponibles = new ObservableCollection<string> { "Pendiente", "En Proceso", "Completado", "Cancelado" };
            DetallesOrdenActual = new ObservableCollection<DetalleServicioDto>();
            DetallesOrdenActual.CollectionChanged += (s, e) => OnPropertyChanged(nameof(Total));
            ServiciosDisponibles = new ObservableCollection<ServicioDto>();
            MecanicosDisponibles = new ObservableCollection<UsuarioDto>();

            ((ObservableCollection<RegistroServicioDto>)Ordenes).CollectionChanged += (s, e) => OnPropertyChanged(nameof(ListadoOrdenesFiltered));
            
            Fecha = DateTime.Now;
            Estado = "En Proceso";
        }

        public ObservableCollection<RegistroServicioDto> Ordenes { get; set; }
        public ObservableCollection<ClienteDto> ClientesDisponibles { get; set; }
        public ObservableCollection<VehiculoDto> VehiculosDisponibles { get; set; }
        public ObservableCollection<TallerDto> TalleresDisponibles { get; set; }
        public ObservableCollection<string> EstadosDisponibles { get; set; }

        public ObservableCollection<DetalleServicioDto> DetallesOrdenActual { get; set; }
        public decimal Total => DetallesOrdenActual?.Sum(d => d.Precio * d.Cantidad) ?? 0;
        public ObservableCollection<ServicioDto> ServiciosDisponibles { get; set; }
        public ObservableCollection<UsuarioDto> MecanicosDisponibles { get; set; }

        private List<VehiculoDto> _todosLosVehiculos = new();

        private int _clienteId;
        public int ClienteId
        {
            get => _clienteId;
            set 
            { 
                _clienteId = value; 
                OnPropertyChanged(); 
                FiltrarVehiculosPorCliente(); 
            }
        }

        private void FiltrarVehiculosPorCliente()
        {
            VehiculosDisponibles.Clear();
            foreach (var v in _todosLosVehiculos.Where(v => _clienteId == 0 || v.IdCliente == _clienteId))
            {
                VehiculosDisponibles.Add(v);
            }
            if (!VehiculosDisponibles.Any(v => v.Id == VehiculoId))
            {
                VehiculoId = 0;
            }
        }

        public void SeleccionarClientePorVehiculo(int vehiculoId)
        {
            var v = _todosLosVehiculos.FirstOrDefault(x => x.Id == vehiculoId);
            if (v != null)
            {
                ClienteId = v.IdCliente;
            }
        }

        private ServicioDto? _servicioBusquedaSeleccionado;
        public ServicioDto? ServicioBusquedaSeleccionado
        {
            get => _servicioBusquedaSeleccionado;
            set { _servicioBusquedaSeleccionado = value; OnPropertyChanged(); }
        }

        private string _textoBusqueda = string.Empty;
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set { _textoBusqueda = value; OnPropertyChanged(); OnPropertyChanged(nameof(ListadoOrdenesFiltered)); }
        }

        public IEnumerable<RegistroServicioDto> ListadoOrdenesFiltered =>
            string.IsNullOrWhiteSpace(TextoBusqueda)
                ? Ordenes
                : Ordenes.Where(o => (o.VehiculoPatente ?? string.Empty).IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0
                                  || (o.VehiculoDetalle ?? string.Empty).IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0
                                  || (o.RecepcionistaNombre ?? string.Empty).IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0
                                  || (o.TallerNombre ?? string.Empty).IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0
                                  || (o.Estado ?? string.Empty).IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0);

        private int _vehiculoId;
        public int VehiculoId
        {
            get => _vehiculoId;
            set { _vehiculoId = value; OnPropertyChanged(); ValidateProperty(); }
        }

        private int _tallerId;
        public int TallerId
        {
            get => _tallerId;
            set { _tallerId = value; OnPropertyChanged(); ValidateProperty(); }
        }

        private int _usuarioId;
        public int UsuarioId
        {
            get => _usuarioId;
            set { _usuarioId = value; OnPropertyChanged(); }
        }

        private DateTime _fecha;
        public DateTime Fecha
        {
            get => _fecha;
            set { _fecha = value; OnPropertyChanged(); ValidateProperty(); }
        }

        private int _kmIngreso;
        public int KmIngreso
        {
            get => _kmIngreso;
            set { _kmIngreso = value; OnPropertyChanged(); ValidateProperty(); }
        }

        private string _estado = string.Empty;
        public string Estado
        {
            get => _estado;
            set { _estado = value; OnPropertyChanged(); ValidateProperty(); }
        }

        private bool EsMecanico => TP_ControlVehicular.Negocio.Context.UserSession.CurrentUser?.IdRol == 3;
        private bool EsRecepcionista => TP_ControlVehicular.Negocio.Context.UserSession.CurrentUser?.IdRol == 2;
        public bool PuedeCrear => !EsMecanico;
        public bool PuedeEliminar => !EsMecanico && OrdenSeleccionada != null;
        public bool PuedeImprimirRecepcion => !EsMecanico && OrdenSeleccionada != null;

        private RegistroServicioDto? _ordenSeleccionada;
        public RegistroServicioDto? OrdenSeleccionada
        {
            get => _ordenSeleccionada;
            set
            {
                _ordenSeleccionada = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PuedePagar));
                OnPropertyChanged(nameof(PuedeVerComprobantePago));
                OnPropertyChanged(nameof(PuedeEliminar));
                OnPropertyChanged(nameof(PuedeImprimirRecepcion));
            }
        }

        public bool PuedePagar => EsRecepcionista && OrdenSeleccionada?.Estado == "Finalizado";
        public bool PuedeVerComprobantePago => !EsMecanico && OrdenSeleccionada?.Estado == "Pagado";

        public async Task LoadCombosAsync()
        {
            int currentClienteId = ClienteId;
            int currentVehiculoId = VehiculoId;
            int currentTallerId = TallerId;

            ClientesDisponibles.Clear();
            _todosLosVehiculos.Clear();
            VehiculosDisponibles.Clear();
            TalleresDisponibles.Clear();
            ServiciosDisponibles.Clear();
            MecanicosDisponibles.Clear();
            
            try
            {
                var clientes = await _clienteRepository.GetAllAsync();
                foreach (var c in clientes)
                {
                    ClientesDisponibles.Add(_mapper.Map<ClienteDto>(c));
                }

                var vehiculos = await _vehiculoRepository.GetAllWithDetailsAsync(); // Use the specialized method to include Modelo and Marca
                foreach (var v in vehiculos)
                {
                    _todosLosVehiculos.Add(_mapper.Map<VehiculoDto>(v));
                }
                
                ClienteId = currentClienteId; // This triggers FiltrarVehiculosPorCliente()
                if (ClienteId == 0) FiltrarVehiculosPorCliente();
                VehiculoId = currentVehiculoId;

                var talleres = await _tallerRepository.GetAllAsync(); // Asumiendo GetAllAsync
                foreach (var t in talleres)
                {
                    TalleresDisponibles.Add(_mapper.Map<TallerDto>(t));
                }
                
                TallerId = currentTallerId;
                if (TallerId == 0 && TalleresDisponibles.Any())
                {
                    TallerId = TalleresDisponibles.First().IdTaller;
                }

                var servicios = await _servicioRepository.GetAllAsync();
                foreach (var s in servicios)
                {
                    ServiciosDisponibles.Add(_mapper.Map<ServicioDto>(s));
                }

                var usuarios = await _usuarioRepository.GetAllAsync();
                // Asumimos que RolId == 3 o similar es MecÃ¡nico, pero podemos cargar todos si no sabemos el ID, o filtrar por nombre
                foreach (var u in usuarios.Where(u => u.Rol?.Nombre?.IndexOf("Mec", StringComparison.OrdinalIgnoreCase) >= 0 || u.RolId == 3))
                {
                    MecanicosDisponibles.Add(_mapper.Map<UsuarioDto>(u));
                }
            }
            catch { }
        }

        public async Task LoadAsync()
        {
            OnPropertyChanged(nameof(PuedeCrear));
            OnPropertyChanged(nameof(PuedeEliminar));
            OnPropertyChanged(nameof(PuedeImprimirRecepcion));
            OnPropertyChanged(nameof(PuedePagar));
            OnPropertyChanged(nameof(PuedeVerComprobantePago));
            
            Ordenes.Clear();
            await LoadCombosAsync();

            try
            {
                var lista = await _listarHandler.HandleAsync();
                foreach (var orden in lista)
                {
                    Ordenes.Add(orden);
                }
            }
            catch { }
        }

        public async Task<bool> CancelarOrdenSeleccionadaAsync()
        {
            if (OrdenSeleccionada == null) return false;

            var orden = new Entidad.RegistroServicio
            {
                Id = OrdenSeleccionada.Id,
                VehiculoId = OrdenSeleccionada.VehiculoId,
                TallerId = OrdenSeleccionada.TallerId,
                UsuarioId = OrdenSeleccionada.UsuarioId,
                Fecha = OrdenSeleccionada.Fecha,
                KmIngreso = OrdenSeleccionada.KmIngreso,
                Estado = "Cancelado",
                Detalles = OrdenSeleccionada.Detalles?.Select(d => new Entidad.DetalleServicio
                {
                    Id = d.Id,
                    RegistroServicioId = d.RegistroServicioId,
                    ServicioId = d.ServicioId,
                    UsuarioId = d.UsuarioId,
                    Cantidad = d.Cantidad,
                    Precio = d.Precio,
                    Origen = d.Origen,
                    Estado = d.Estado,
                    
                    Observaciones = d.Observaciones
                }).ToList() ?? new List<Entidad.DetalleServicio>()
            };

            try
            {
                var dto = await _modificarHandler.HandleAsync(orden);
                var index = -1;
                for (int i = 0; i < Ordenes.Count; i++)
                {
                    if (Ordenes[i].Id == dto.Id)
                    {
                        index = i;
                        break;
                    }
                }
                if (index >= 0)
                {
                    Ordenes[index] = dto;
                }
                return true;
            }
            catch (Exception ex)
            {
                RegistrationFailed?.Invoke(this, "Error al cancelar la orden: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> GuardarOrdenAsync()
        {
            if (!ValidateAll()) return false;

            var orden = new Entidad.RegistroServicio
            {
                Id = OrdenSeleccionada?.Id ?? 0,
                VehiculoId = VehiculoId,
                TallerId = TallerId,
                UsuarioId = UsuarioId > 0 ? UsuarioId : 1, // Por seguridad si no se setea desde UI
                Fecha = Fecha,
                KmIngreso = KmIngreso,
                Estado = Estado,
                Detalles = DetallesOrdenActual.Select(d => new Entidad.DetalleServicio
                {
                    Id = d.Id,
                    ServicioId = d.ServicioId,
                    RegistroServicioId = OrdenSeleccionada?.Id ?? 0,
                    UsuarioId = d.UsuarioId,
                    Cantidad = d.Cantidad > 0 ? d.Cantidad : 1,
                    Precio = d.Precio,
                    Observaciones = d.Observaciones ?? string.Empty,
                    Origen = d.Origen ?? "Manual",
                    Estado = d.Realizado ? "Realizado" : "Pendiente"
                }).ToList()
            };

            try
            {
                if (orden.Id == 0)
                {
                    var dto = await _registrarHandler.HandleAsync(orden);
                    Ordenes.Add(dto);
                }
                else
                {
                    var dto = await _modificarHandler.HandleAsync(orden);
                    var index = -1;
                    for (int i = 0; i < Ordenes.Count; i++)
                    {
                        if (Ordenes[i].Id == dto.Id)
                        {
                            index = i;
                            break;
                        }
                    }
                    if (index >= 0) Ordenes[index] = dto;
                    else Ordenes.Add(dto);
                }

                return true;
            }
            catch (Exception ex)
            {
                RegistrationFailed?.Invoke(this, "Error al guardar la orden de servicio: " + ex.Message);
                return false;
            }
        }
        
        public event EventHandler<string>? RegistrationFailed;

        public override bool ValidateProperty([CallerMemberName] string? propertyName = null)
        {
            if (propertyName is null) return true;
            ClearErrors(propertyName);

            switch (propertyName)
            {
                case nameof(VehiculoId):
                    if (VehiculoId <= 0)
                        SetError(nameof(VehiculoId), "Debe seleccionar un VehÃ­culo.");
                    break;
                case nameof(TallerId):
                    if (TallerId <= 0)
                        SetError(nameof(TallerId), "Debe seleccionar un Taller.");
                    break;
                case nameof(KmIngreso):
                    if (KmIngreso <= 0)
                        SetError(nameof(KmIngreso), "El kilometraje debe ser mayor a 0.");
                    break;
                case nameof(Estado):
                    if (string.IsNullOrWhiteSpace(Estado))
                        SetError(nameof(Estado), "Debe seleccionar un Estado.");
                    break;
            }

            return !GetErrors(propertyName).Cast<object>().Any();
        }

        public bool ValidateAll()
        {
            
            ValidateProperty(nameof(VehiculoId));
            ValidateProperty(nameof(TallerId));
            ValidateProperty(nameof(KmIngreso));
            ValidateProperty(nameof(Estado));
            return !HasErrors;
        }

        public event EventHandler<Negocio.DTOs.Reportes.ComprobanteOrdenDto>? MostrarComprobanteOrdenRequested;
        public event EventHandler<Negocio.DTOs.Reportes.ComprobantePagoDto>? MostrarComprobantePagoRequested;

        private ICommand? _generarComprobanteOrdenCommand;
        public ICommand GenerarComprobanteOrdenCommand => _generarComprobanteOrdenCommand ??= new TP_ControlVehicular.Presentacion.RelayCommand(async () => await GenerarComprobanteOrdenAsync());

        private ICommand? _generarComprobantePagoCommand;
        public ICommand GenerarComprobantePagoCommand => _generarComprobantePagoCommand ??= new TP_ControlVehicular.Presentacion.RelayCommand(async () => await GenerarComprobantePagoAsync());

        private async Task GenerarComprobanteOrdenAsync()
        {
            if (OrdenSeleccionada == null || OrdenSeleccionada.Id <= 0) return;
            var comprobante = await _generarComprobanteOrdenHandler.HandleAsync(OrdenSeleccionada.Id);
            if (comprobante != null)
            {
                MostrarComprobanteOrdenRequested?.Invoke(this, comprobante);
            }
        }

        private async Task GenerarComprobantePagoAsync()
        {
            if (OrdenSeleccionada == null || OrdenSeleccionada.Id <= 0) return;
            var comprobante = await _generarComprobantePagoHandler.HandleAsync(OrdenSeleccionada.Id);
            if (comprobante != null)
            {
                MostrarComprobantePagoRequested?.Invoke(this, comprobante);
            }
        }
    }
}









