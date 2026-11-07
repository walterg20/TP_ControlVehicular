// walterg20
using AutoMapper;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;
using TP_ControlVehicular.Negocio.Services;
using TP_ControlVehicular.Presentacion.Helpers;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TP_ControlVehicular.Negocio.Reportes.Documentos;
using System.Diagnostics;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public class OrdenServicioViewModel : BasePaginableViewModel, INotifyDataErrorInfo
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
        private readonly AsignarVehiculoPorPatenteHandler _asignarVehiculoPorPatenteHandler;
        private readonly IMapper _mapper;
        private readonly Debouncer _debouncer;
        private readonly IAyudaService? _ayudaService;
        private int _idRolActual;

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
            AsignarVehiculoPorPatenteHandler asignarVehiculoPorPatenteHandler,
            IMapper mapper,
            IAyudaService? ayudaService = null)
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
            _asignarVehiculoPorPatenteHandler = asignarVehiculoPorPatenteHandler;
            _mapper = mapper;
            _debouncer = new Debouncer();
            _ayudaService = ayudaService;

            Ordenes = new ObservableCollection<RegistroServicioDto>();
            ClientesDisponibles = new ObservableCollection<ClienteDto>();
            VehiculosDisponibles = new ObservableCollection<VehiculoDto>();
            ResultadosBusquedaUniversal = new ObservableCollection<BusquedaClienteVehiculoDto>();
            TalleresDisponibles = new ObservableCollection<TallerDto>();
            EstadosDisponibles = new ObservableCollection<string> { "Abierta", "En Proceso", "Completada", "Cancelada" };
            DetallesOrdenActual = new ObservableCollection<DetalleServicioDto>();
            DetallesOrdenActual.CollectionChanged += (s, e) => OnPropertyChanged(nameof(Total));
            ServiciosDisponibles = new ObservableCollection<ServicioDto>();
            MecanicosDisponibles = new ObservableCollection<UsuarioDto>();

            ((ObservableCollection<RegistroServicioDto>)Ordenes).CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(IsListEmpty));
            };

            Fecha = DateTime.Now;
            Estado = "Abierta";
        }

        public ObservableCollection<RegistroServicioDto> Ordenes { get; set; }
        public ObservableCollection<ClienteDto> ClientesDisponibles { get; set; }
        public ObservableCollection<VehiculoDto> VehiculosDisponibles { get; set; }

        /// <summary>
        /// Filas del buscador universal (cliente + vehiculo) mostradas en el desplegable
        /// de la pantalla de orden de servicio.
        /// </summary>
        public ObservableCollection<BusquedaClienteVehiculoDto> ResultadosBusquedaUniversal { get; set; }
        public ObservableCollection<TallerDto> TalleresDisponibles { get; set; }
        public ObservableCollection<string> EstadosDisponibles { get; set; }

        public ObservableCollection<DetalleServicioDto> DetallesOrdenActual { get; set; }
        public decimal Total => DetallesOrdenActual?.Sum(d => d.Precio * d.Cantidad) ?? 0;
        public ObservableCollection<ServicioDto> ServiciosDisponibles { get; set; }
        public ObservableCollection<UsuarioDto> MecanicosDisponibles { get; set; }

        private List<VehiculoDto> _todosLosVehiculos = new();
        private List<ClienteDto> _todosLosClientes = new();
        private List<ServicioDto> _todosLosServicios = new();

        public IReadOnlyList<ClienteDto> TodosLosClientes => _todosLosClientes;
        public IReadOnlyList<ServicioDto> TodosLosServicios => _todosLosServicios;

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

        /// <summary>
        /// Filtra la lista de clientes disponibles según el término ingresado.
        /// Se activa únicamente cuando el término contiene 3 o más caracteres (coincidiendo por Nombre, Apellido o DNI).
        /// Con menos de 3 caracteres se preserva o restaura la lista completa sin filtrar agresivamente.
        /// </summary>
        public void FiltrarClientes(string? texto)
        {
            if (!_todosLosClientes.Any() && ClientesDisponibles.Any())
            {
                _todosLosClientes.AddRange(ClientesDisponibles);
            }

            var term = texto?.Trim() ?? string.Empty;

            if (term.Length < 3)
            {
                if (ClientesDisponibles.Count != _todosLosClientes.Count)
                {
                    ClientesDisponibles.Clear();
                    foreach (var c in _todosLosClientes)
                    {
                        ClientesDisponibles.Add(c);
                    }
                }
                return;
            }

            var filtrados = _todosLosClientes.Where(c =>
                (!string.IsNullOrEmpty(c.Nombre) && c.Nombre.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(c.Apellido) && c.Apellido.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(c.Dni) && c.Dni.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(c.NombreCompletoConDni) && c.NombreCompletoConDni.Contains(term, StringComparison.OrdinalIgnoreCase))
            ).ToList();

            ClientesDisponibles.Clear();
            foreach (var c in filtrados)
            {
                ClientesDisponibles.Add(c);
            }
        }

        /// <summary>
        /// Catalogo completo de vehiculos cargado en memoria. Se expone para las pruebas
        /// y para consultas que necesiten el total de vehiculos del taller.
        /// </summary>
        public IReadOnlyList<VehiculoDto> TodosLosVehiculos => _todosLosVehiculos;

        /// <summary>
        /// Texto tipeado actualmente en el buscador universal. Se conserva para poder
        /// reutilizarlo (por ejemplo, para prellenar la patente en el modal de alta).
        /// </summary>
        private string _textoBusquedaUniversal = string.Empty;
        public string TextoBusquedaUniversal
        {
            get => _textoBusquedaUniversal;
            set { _textoBusquedaUniversal = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Fila seleccionada en el desplegable del buscador universal. La asignacion de
        /// cliente y vehiculo la realiza <see cref="SeleccionarResultadoUniversal"/>, que
        /// invoca el code-behind de forma explicita para no disparar efectos colaterales
        /// al prellenar una orden en modo edicion.
        /// </summary>
        private BusquedaClienteVehiculoDto? _resultadoBusquedaSeleccionado;
        public BusquedaClienteVehiculoDto? ResultadoBusquedaSeleccionado
        {
            get => _resultadoBusquedaSeleccionado;
            set { _resultadoBusquedaSeleccionado = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Secciones de ayuda contextual para mostrar en el panel de ayuda.
        /// Se actualiza dinámicamente cuando cambia el foco entre controles.
        /// </summary>
        private IEnumerable<AyudaSeccionDto>? _ayudaActual;
        public IEnumerable<AyudaSeccionDto>? AyudaActual
        {
            get => _ayudaActual;
            set { _ayudaActual = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Se dispara cuando el operador elige la fila "Registrar Nuevo Cliente y Vehiculo".
        /// El parametro es el texto tipeado en el buscador.
        /// </summary>
        public event EventHandler<string>? AbrirModalNuevoClienteVehiculoRequested;

        /// <summary>Se dispara cuando hay que elegir el vehiculo en el desplegable.</summary>
        public event EventHandler? FocoVehiculoRequested;

        /// <summary>Se dispara cuando el cliente y el vehiculo ya estan completos.</summary>
        public event EventHandler? FocoKilometrajeRequested;

        /// <summary>Cantidad maxima de filas mostradas en el desplegable del buscador.</summary>
        public const int MaximoResultadosBusquedaUniversal = 25;

        /// <summary>
        /// Busca clientes y vehiculos de forma simultanea por DNI, apellido, nombre y patente.
        /// El filtro predictivo se activa desde 3 caracteres (igual que el resto de los combos
        /// de la pantalla). Cada fila se formatea como
        /// <c>[Patente] - Apellido, Nombre (DNI) - Vehiculo</c>.
        /// </summary>
        public void FiltrarBusquedaUniversal(string? texto)
        {
            if (!_todosLosClientes.Any() && ClientesDisponibles.Any())
            {
                _todosLosClientes.AddRange(ClientesDisponibles);
            }

            var term = texto?.Trim() ?? string.Empty;
            TextoBusquedaUniversal = term;

            if (term.Length < 3)
            {
                ResultadosBusquedaUniversal.Clear();
                return;
            }

            var filas = ConstruirFilasBusquedaUniversal(term);

            ResultadosBusquedaUniversal.Clear();
            foreach (var fila in filas)
            {
                ResultadosBusquedaUniversal.Add(fila);
            }
        }

        /// <summary>
        /// Construye las filas del desplegable a partir de los catalogos en memoria.
        /// Un cliente con un solo vehiculo produce una fila normal; un cliente con dos o mas
        /// vehiculos produce una fila agrupada que deriva la eleccion al desplegable de vehiculos.
        /// </summary>
        private List<BusquedaClienteVehiculoDto> ConstruirFilasBusquedaUniversal(string term)
        {
            var filas = new List<BusquedaClienteVehiculoDto>();

            // Un vehiculo puede no tener propietario actual: se indexa aparte para no
            // contaminar los grupos por cliente.
            var vehiculosSinCliente = new List<VehiculoDto>();

            // Cliente -> vehiculos cuyo texto coincide con los datos del cliente.
            var coincidenciasPorCliente = new Dictionary<int, List<VehiculoDto>>();

            // Clientes que ya tienen una fila propia por coincidencia de patente.
            var clientesConPatente = new HashSet<int>();

            foreach (var vehiculo in _todosLosVehiculos)
            {
                var cliente = BuscarCliente(vehiculo.IdCliente);
                bool coincidePatente = Coincide(vehiculo.Patente, term);
                bool coincideCliente = cliente != null && CoincideCliente(cliente, term);

                if (!coincidePatente && !coincideCliente) continue;

                if (vehiculo.IdCliente <= 0 || cliente == null)
                {
                    if (coincidePatente) vehiculosSinCliente.Add(vehiculo);
                    continue;
                }

                if (coincidePatente)
                {
                    // Coincidencia directa por patente: se muestra el vehiculo concreto
                    // aunque el cliente tenga varios autos.
                    filas.Add(CrearFilaVehiculo(vehiculo, cliente));
                    clientesConPatente.Add(cliente.IdCliente);
                    continue;
                }

                if (!coincidenciasPorCliente.TryGetValue(cliente.IdCliente, out var lista))
                {
                    lista = new List<VehiculoDto>();
                    coincidenciasPorCliente[cliente.IdCliente] = lista;
                }
                lista.Add(vehiculo);
            }

            // Clientes que coinciden por nombre, apellido o DNI.
            foreach (var par in coincidenciasPorCliente)
            {
                var cliente = BuscarCliente(par.Key);
                if (cliente == null) continue;

                // Si el cliente ya tiene una fila propia por coincidencia de patente,
                // no se agrega la fila agrupada para evitar duplicar al cliente.
                if (clientesConPatente.Contains(cliente.IdCliente)) continue;

                int totalVehiculos = _todosLosVehiculos.Count(v => v.IdCliente == cliente.IdCliente);

                if (totalVehiculos <= 1)
                {
                    filas.Add(CrearFilaVehiculo(par.Value[0], cliente));
                }
                else
                {
                    filas.Add(CrearFilaAgrupada(cliente, totalVehiculos));
                }
            }

            // Vehiculos sin propietario actual que coinciden por patente.
            foreach (var vehiculo in vehiculosSinCliente)
            {
                filas.Add(CrearFilaVehiculo(vehiculo, null));
            }

            if (filas.Count == 0)
            {
                filas.Add(CrearFilaAccionRegistrar());
            }

            return filas.Take(MaximoResultadosBusquedaUniversal).ToList();
        }

        /// <summary>
        /// Aplica la seleccion de una fila del buscador universal: completa cliente y vehiculo
        /// de la orden y avisa al code-behind donde debe ir el foco.
        /// </summary>
        public void SeleccionarResultadoUniversal(BusquedaClienteVehiculoDto resultado)
        {
            if (resultado == null) return;

            if (resultado.EsAccionRegistrar)
            {
                AbrirModalNuevoClienteVehiculoRequested?.Invoke(this, TextoBusquedaUniversal);
                return;
            }

            ClienteId = resultado.ClienteId;

            if (resultado.EsAgrupado || resultado.VehiculoId <= 0)
            {
                // Cliente con varios vehiculos (o sin vehiculo): el operador elige en el desplegable.
                VehiculoId = 0;
                FocoVehiculoRequested?.Invoke(this, EventArgs.Empty);
                return;
            }

            VehiculoId = resultado.VehiculoId;
            FocoKilometrajeRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Indica si el texto tipeado tiene forma de patente y, por lo tanto, conviene
        /// prellenarlo en el modal de alta unificada. Una patente combina letras y numeros
        /// (por ejemplo "AG 987 ZK"); un nombre o un DNI puro no cumplen esa forma.
        /// </summary>
        public bool ParecePatente(string? texto)
        {
            var term = texto?.Trim() ?? string.Empty;
            if (term.Length < 5) return false;

            bool tieneLetras = term.Any(char.IsLetter);
            bool tieneNumeros = term.Any(char.IsDigit);

            return tieneLetras && tieneNumeros;
        }

        private ClienteDto? BuscarCliente(int idCliente)
        {
            if (idCliente <= 0) return null;
            return _todosLosClientes.FirstOrDefault(c => c.IdCliente == idCliente);
        }

        private static bool Coincide(string? valor, string term)
        {
            return !string.IsNullOrEmpty(valor) && valor.Contains(term, StringComparison.OrdinalIgnoreCase);
        }

        private static bool CoincideCliente(ClienteDto cliente, string term)
        {
            return Coincide(cliente.Dni, term)
                || Coincide(cliente.Apellido, term)
                || Coincide(cliente.Nombre, term);
        }

        private static BusquedaClienteVehiculoDto CrearFilaVehiculo(VehiculoDto vehiculo, ClienteDto? cliente)
        {
            var fila = new BusquedaClienteVehiculoDto
            {
                ClienteId = cliente?.IdCliente ?? 0,
                VehiculoId = vehiculo.Id,
                Patente = vehiculo.Patente,
                Apellido = cliente?.Apellido ?? string.Empty,
                Nombre = cliente?.Nombre ?? string.Empty,
                Dni = cliente?.Dni ?? string.Empty,
                VehiculoDescripcion = $"{vehiculo.MarcaNombre} {vehiculo.ModeloNombre}".Trim()
            };

            fila.DisplayText = cliente == null
                ? $"[{fila.Patente}] - (Sin Cliente) - {fila.VehiculoDescripcion}"
                : $"[{fila.Patente}] - {fila.Apellido}, {fila.Nombre} ({fila.Dni}) - {fila.VehiculoDescripcion}";

            fila.TextMostrar = cliente == null
                ? $"[{fila.Patente}] - (Sin Cliente)"
                : $"{fila.Apellido}, {fila.Nombre}";

            return fila;
        }

        private static BusquedaClienteVehiculoDto CrearFilaAgrupada(ClienteDto cliente, int cantidadVehiculos)
        {
            return new BusquedaClienteVehiculoDto
            {
                ClienteId = cliente.IdCliente,
                VehiculoId = 0,
                Apellido = cliente.Apellido,
                Nombre = cliente.Nombre,
                Dni = cliente.Dni,
                EsAgrupado = true,
                CantidadVehiculos = cantidadVehiculos,
                DisplayText = $"{cliente.Apellido}, {cliente.Nombre} (DNI: {cliente.Dni}) - [Ver {cantidadVehiculos} vehiculos]",
                TextMostrar = $"{cliente.Apellido}, {cliente.Nombre}"
            };
        }

        private static BusquedaClienteVehiculoDto CrearFilaAccionRegistrar()
        {
            return new BusquedaClienteVehiculoDto
            {
                EsAccionRegistrar = true,
                DisplayText = "🔍 No se encontraron resultados. + Registrar Nuevo Cliente y Vehiculo",
                TextMostrar = "🔍 No se encontraron resultados. + Registrar Nuevo Cliente y Vehiculo"
            };
        }

        /// <summary>
        /// Soporte dual para servicios: cuando el texto no tiene filtro o tiene menos de 3 caracteres,
        /// expone el catálogo completo de servicios; cuando tiene 3 o más caracteres, filtra
        /// dinámicamente por el nombre del servicio.
        /// </summary>
        public void FiltrarServicios(string? texto)
        {
            if (!_todosLosServicios.Any() && ServiciosDisponibles.Any())
            {
                _todosLosServicios.AddRange(ServiciosDisponibles);
            }

            var term = texto?.Trim() ?? string.Empty;

            if (term.Length < 3)
            {
                if (ServiciosDisponibles.Count != _todosLosServicios.Count)
                {
                    ServiciosDisponibles.Clear();
                    foreach (var s in _todosLosServicios)
                    {
                        ServiciosDisponibles.Add(s);
                    }
                }
                return;
            }

            var filtrados = _todosLosServicios.Where(s =>
                !string.IsNullOrEmpty(s.Nombre) && s.Nombre.Contains(term, StringComparison.OrdinalIgnoreCase)
            ).ToList();

            ServiciosDisponibles.Clear();
            foreach (var s in filtrados)
            {
                ServiciosDisponibles.Add(s);
            }
        }

        /// <summary>
        /// Carga dinámicamente la ayuda contextual para un control específico.
        /// Se llama cuando el usuario cambia el foco a un control en la pantalla.
        /// </summary>
        /// <param name="nombreControl">Nombre del control (ej: "TxtCliente", "CmbServicio").</param>
        public async void CargarAyudaPorControl(string nombreControl)
        {
            if (_ayudaService == null) return;

            try
            {
                var ayuda = await _ayudaService.ObtenerAyudaAsync(_idRolActual, "OrdenServicio", nombreControl);

                if (ayuda != null)
                {
                    AyudaActual = ayuda.Secciones.OrderBy(s => s.Orden).ToList();
                }
                else
                {
                    AyudaActual = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar ayuda: {ex.Message}");
                AyudaActual = null;
            }
        }

        /// <summary>
        /// Establece el rol actual del usuario (requerido para filtrar ayudas).
        /// Debe llamarse al inicializar la pantalla.
        /// </summary>
        public void EstablecerRolActual(int idRol)
        {
            _idRolActual = idRol;
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
            set { _textoBusqueda = value; OnPropertyChanged(); ReiniciarPagina(); }
        }

        private DateTime? _fechaDesde = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        public DateTime? FechaDesde
        {
            get => _fechaDesde;
            set { _fechaDesde = value; OnPropertyChanged(); ReiniciarPagina(); }
        }

        private DateTime? _fechaHasta = DateTime.Today;
        public DateTime? FechaHasta
        {
            get => _fechaHasta;
            set { _fechaHasta = value; OnPropertyChanged(); ReiniciarPagina(); }
        }

        private string _estadoFiltro = "Todos";
        public string EstadoFiltro
        {
            get => _estadoFiltro;
            set { _estadoFiltro = value; OnPropertyChanged(); ReiniciarPagina(); }
        }

        private string _filtroActivo = "Todos";
        public string FiltroActivo
        {
            get => _filtroActivo;
            set
            {
                if (_filtroActivo == value) return;
                _filtroActivo = value;
                OnPropertyChanged();
                ReiniciarPagina();
                _ = CargarPaginaAsync();
            }
        }

        public bool PuedeVerFiltroActivo => PuedeEliminar;

        public List<string> EstadosActivosDisponibles { get; } = new List<string> { "Todos", "Activos", "Inactivos" };

        private string _mensajeValidacionFechas = string.Empty;
        public string MensajeValidacionFechas
        {
            get => _mensajeValidacionFechas;
            set { _mensajeValidacionFechas = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Valida el rango de fechas y devuelve true si es correcto.
        /// </summary>
        private bool ValidarFechas()
        {
            if (FechaDesde.HasValue && FechaHasta.HasValue && FechaDesde.Value.Date > FechaHasta.Value.Date)
            {
                MensajeValidacionFechas = "La fecha 'Desde' no puede ser posterior a 'Hasta'.";
                return false;
            }

            MensajeValidacionFechas = string.Empty;
            return true;
        }

        public bool IsListEmpty => !Ordenes.Any();

        private ICommand? _limpiarFiltrosCommand;
        public ICommand LimpiarFiltrosCommand => _limpiarFiltrosCommand ??= new RelayCommand(async () =>
        {
            FechaDesde = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            FechaHasta = DateTime.Today;
            EstadoFiltro = "Todos";
            TextoBusqueda = string.Empty;
            await BuscarAsync();
        });

        private ICommand? _imprimirListadoFiltradoCommand;
        public ICommand ImprimirListadoFiltradoCommand => _imprimirListadoFiltradoCommand ??= new RelayCommand(async () => await ImprimirListadoFiltradoAsync());

        public async Task ImprimirListadoFiltradoAsync()
        {
            // El PDF exporta el conjunto completo de ordenes que cumplen los filtros,
            // no solo la pagina visible.
            var ordenes = (await _listarHandler.HandleCompletoAsync(
                TextoBusqueda.Trim(),
                EstadoFiltro,
                FechaDesde,
                FechaHasta,
                PuedeVerFiltroActivo ? FiltroActivo : "Activos")).ToList();

            var filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"ListadoOrdenes_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

            var filtrosTexto = $"Estado: {EstadoFiltro}";
            if (FechaDesde.HasValue) filtrosTexto += $" | Desde: {FechaDesde.Value:dd/MM/yyyy}";
            if (FechaHasta.HasValue) filtrosTexto += $" | Hasta: {FechaHasta.Value:dd/MM/yyyy}";
            if (!string.IsNullOrWhiteSpace(TextoBusqueda)) filtrosTexto += $" | Búsqueda: '{TextoBusqueda}'";

            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(QuestPDF.Helpers.PageSizes.A4.Landscape());
                    page.Margin(1, QuestPDF.Infrastructure.Unit.Centimetre);
                    page.PageColor(QuestPDF.Helpers.Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Element(c => c.ComposeEncabezadoTaller("Listado de Órdenes de Trabajo"));
                    page.Content().PaddingVertical(1, QuestPDF.Infrastructure.Unit.Centimetre).Column(col =>
                    {
                        col.Item().PaddingBottom(10).Text($"Filtros aplicados: {filtrosTexto} (Total: {ordenes.Count} órdenes)").FontSize(11).FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(45);   // N°
                                columns.ConstantColumn(95);   // Fecha
                                columns.RelativeColumn(2);    // Cliente
                                columns.ConstantColumn(70);   // Patente
                                columns.RelativeColumn(2);    // Vehículo
                                columns.RelativeColumn(1.5f); // Recepción
                                columns.ConstantColumn(55);   // Km
                                columns.ConstantColumn(80);   // Estado
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("N°");
                                header.Cell().Element(CellStyle).Text("Fecha");
                                header.Cell().Element(CellStyle).Text("Cliente");
                                header.Cell().Element(CellStyle).Text("Patente");
                                header.Cell().Element(CellStyle).Text("Vehículo");
                                header.Cell().Element(CellStyle).Text("Recepción");
                                header.Cell().Element(CellStyle).Text("Km");
                                header.Cell().Element(CellStyle).Text("Estado");

                                static QuestPDF.Infrastructure.IContainer CellStyle(QuestPDF.Infrastructure.IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Black);
                                }
                            });

                            foreach (var ord in ordenes)
                            {
                                table.Cell().Element(Block).Text(ord.Id.ToString());
                                table.Cell().Element(Block).Text(ord.Fecha.ToString("dd/MM/yyyy HH:mm"));
                                table.Cell().Element(Block).Text(ord.ClienteDetalle ?? "-");
                                table.Cell().Element(Block).Text(ord.VehiculoPatente ?? "-");
                                table.Cell().Element(Block).Text(ord.VehiculoDetalle ?? "-");
                                table.Cell().Element(Block).Text(ord.RecepcionistaNombre ?? "-");
                                table.Cell().Element(Block).Text(ord.KmIngreso.ToString());
                                table.Cell().Element(Block).Text(ord.Estado ?? "-");

                                static QuestPDF.Infrastructure.IContainer Block(QuestPDF.Infrastructure.IContainer container)
                                {
                                    return container.PaddingVertical(2).PaddingHorizontal(2);
                                }
                            }
                        });
                    });
                    page.Footer().Element(c => c.ComposePieDePagina());
                });
            });

            await Task.Run(() => document.GeneratePdf(filePath));

            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al abrir PDF: {ex.Message}");
            }
        }

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


        private bool EsMecanico
        {
            get { return !TienePermiso("OrdenServicio.Crear"); }
        }
        public bool EsRecepcionista
        {
            get { return TienePermiso("OrdenServicio.Crear"); }
        }

        protected override string Modulo => "OrdenServicio";
        public new bool PuedeEliminar => TienePermiso("OrdenServicio.Eliminar") && OrdenSeleccionada != null;
        public bool PuedeImprimirRecepcion => !EsMecanico && OrdenSeleccionada != null;

        public new void NotificarPermisos()
        {
            base.NotificarPermisos();
            OnPropertyChanged(nameof(PuedeVerFiltroActivo));
        }

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

        public bool PuedePagar => EsRecepcionista && OrdenSeleccionada?.Estado == "Completada";
        public bool PuedeVerComprobantePago => !EsMecanico && OrdenSeleccionada?.Estado == "Pagada";

        public async Task LoadCombosAsync()
        {
            int currentClienteId = ClienteId;
            int currentVehiculoId = VehiculoId;
            int currentTallerId = TallerId;

            ClientesDisponibles.Clear();
            _todosLosClientes.Clear();
            _todosLosVehiculos.Clear();
            VehiculosDisponibles.Clear();
            TalleresDisponibles.Clear();
            ServiciosDisponibles.Clear();
            _todosLosServicios.Clear();
            MecanicosDisponibles.Clear();

            try
            {
                var clientes = await _clienteRepository.GetAllAsync();
                foreach (var c in clientes)
                {
                    var dto = _mapper.Map<ClienteDto>(c);
                    _todosLosClientes.Add(dto);
                    ClientesDisponibles.Add(dto);
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
                    var dto = _mapper.Map<ServicioDto>(s);
                    _todosLosServicios.Add(dto);
                    ServiciosDisponibles.Add(dto);
                }

                var usuarios = await _usuarioRepository.GetActivosAsync();
                if (usuarios != null)
                {
                    // Asumimos que RolId == 3 o similar es Mecánico, pero podemos cargar todos si no sabemos el ID, o filtrar por nombre
                    foreach (var u in usuarios.Where(u => u.RolId == 3 || (u.Rol?.Nombre != null && u.Rol.Nombre.IndexOf("Mec", StringComparison.OrdinalIgnoreCase) >= 0)))
                    {
                        MecanicosDisponibles.Add(_mapper.Map<UsuarioDto>(u));
                    }
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

            await LoadCombosAsync();

            ReiniciarPagina();
            await CargarPaginaAsync();
        }

        /// <summary>
        /// Aplica los filtros actuales y recarga el listado desde la primera pagina.
        /// </summary>
        public async Task BuscarAsync()
        {
            if (!ValidarFechas()) return;

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

        /// <summary>
        /// Carga la pagina actual de ordenes con los filtros aplicados en la base de datos.
        /// </summary>
        protected override async Task CargarPaginaAsync()
        {
            if (!ValidarFechas()) return;

            IsLoading = true;
            try
            {
                var (items, total) = await _listarHandler.HandlePaginadoAsync(
                    TextoBusqueda.Trim(),
                    EstadoFiltro,
                    FechaDesde,
                    FechaHasta,
                    PaginaActual,
                    TamanoPagina,
                    PuedeVerFiltroActivo ? FiltroActivo : "Activos");

                Ordenes.Clear();
                foreach (var orden in items)
                {
                    Ordenes.Add(orden);
                }

                ActualizarTotales(total);
            }
            catch
            {
                Ordenes.Clear();
                ActualizarTotales(0);
            }
            finally
            {
                IsLoading = false;
            }
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
                Estado = Entidad.RegistroServicio.EstadoCancelada,
                Activo = false,
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
                    OrdenEjecucion = d.OrdenEjecucion,

                    Observaciones = d.Observaciones
                }).ToList() ?? new List<Entidad.DetalleServicio>()
            };

            try
            {
                await _modificarHandler.HandleAsync(orden);
                // Recargar la pagina actual para que el listado refleje el cambio de estado.
                await CargarPaginaAsync();
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

            if (DetallesOrdenActual.Any(d => d.UsuarioId <= 0))
            {
                RegistrationFailed?.Invoke(this, "Debe asignar un mecánico a cada servicio de la orden.");
                return false;
            }

            if (IsLoading) return false;
            IsLoading = true;

            var orden = new Entidad.RegistroServicio
            {
                Id = OrdenSeleccionada?.Id ?? 0,
                VehiculoId = VehiculoId,
                TallerId = TallerId > 0 ? TallerId : 1, // Por defecto al taller 1 si no está seteado
                UsuarioId = UsuarioId > 0 ? UsuarioId : 1, // Por seguridad si no se setea desde UI
                Fecha = Fecha,
                KmIngreso = KmIngreso,
                Estado = Estado,
                Activo = OrdenSeleccionada?.Activo ?? true,
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
                    Estado = d.Realizado ? "Finalizada" : (!string.IsNullOrEmpty(d.Estado) ? d.Estado : "Pendiente"),
                    OrdenEjecucion = d.OrdenEjecucion
                }).ToList()
            };

            try
            {
                if (orden.Id == 0)
                {
                    await _registrarHandler.HandleAsync(orden);
                }
                else
                {
                    await _modificarHandler.HandleAsync(orden);
                }

                // Recargar la pagina actual para que el listado refleje el alta o la edicion.
                await CargarPaginaAsync();

                return true;
            }
            catch (Exception ex)
            {
                Exception current = ex;
                while (current.InnerException != null)
                {
                    current = current.InnerException;
                }
                RegistrationFailed?.Invoke(this, "Error al guardar la orden de servicio:\n" + current.Message);
                return false;
            }
            finally
            {
                IsLoading = false;
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
                        SetError(nameof(VehiculoId), "Debe seleccionar un Vehículo.");
                    break;
                case nameof(TallerId):
                    // Se quita validación de TallerId porque la UI lo tiene oculto y se asigna por defecto
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

        private ICommand? _agregarVehiculoRapidoCommand;
        public ICommand AgregarVehiculoRapidoCommand => _agregarVehiculoRapidoCommand ??= new TP_ControlVehicular.Presentacion.RelayCommand(async () => await AgregarVehiculoRapidoAsync());

        private async Task AgregarVehiculoRapidoAsync()
        {
            if (ClienteId <= 0)
            {
                System.Windows.MessageBox.Show("Primero debe seleccionar un Cliente.");
                return;
            }

            var patente = Microsoft.VisualBasic.Interaction.InputBox("Ingrese la Patente del vehículo a asignar al cliente:", "Agregar Vehículo Rápido", "");
            if (!string.IsNullOrWhiteSpace(patente))
            {
                var dto = await _asignarVehiculoPorPatenteHandler.HandleAsync(patente, ClienteId);
                if (dto != null)
                {
                    // Reload combos to update vehicles list
                    await LoadCombosAsync();
                    VehiculoId = dto.Id;
                }
                else
                {
                    System.Windows.MessageBox.Show("No se encontró ningún vehículo con esa patente.");
                }
            }
        }

        public void EvaluarEstadoGeneral()
        {
            if (DetallesOrdenActual == null || !DetallesOrdenActual.Any())
            {
                Estado = "Pendiente";
                return;
            }

            bool todosRealizados = DetallesOrdenActual.All(d => d.Realizado);
            bool algunRealizado = DetallesOrdenActual.Any(d => d.Realizado);

            if (todosRealizados)
            {
                Estado = "Completada";
            }
            else if (algunRealizado)
            {
                Estado = "En Proceso";
            }
            else
            {
                Estado = "Abierta";
            }
        }
    }
}









