// walterg20
using AutoMapper;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;
using TP_ControlVehicular.Negocio.Services;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    /// <summary>
    /// ViewModel del modal unificado de alta (Cliente + Vehiculo) que se abre desde el
    /// buscador universal de la orden de servicio. Registra el cliente, el vehiculo y el
    /// vinculo de propiedad en un solo paso.
    /// </summary>
    public class ClienteVehiculoViewModel : BaseViewModel
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IVehiculoRepository _vehiculoRepository;
        private readonly ListarModelosHandler _listarModelosHandler;
        private readonly ListarMarcaHandler _listarMarcaHandler;
        private readonly IRepository<PropietarioVehiculo> _propietarioRepository;
        private readonly IMapper _mapper;

        private List<ModeloDto> _todosLosModelos = new();

        public ClienteVehiculoViewModel(
            IClienteRepository clienteRepository,
            IVehiculoRepository vehiculoRepository,
            ListarModelosHandler listarModelosHandler,
            ListarMarcaHandler listarMarcaHandler,
            IRepository<PropietarioVehiculo> propietarioRepository,
            IMapper mapper)
        {
            _clienteRepository = clienteRepository;
            _vehiculoRepository = vehiculoRepository;
            _listarModelosHandler = listarModelosHandler;
            _listarMarcaHandler = listarMarcaHandler;
            _propietarioRepository = propietarioRepository;
            _mapper = mapper;

            MarcasDisponibles = new ObservableCollection<MarcaDto>();
            ModelosDisponibles = new ObservableCollection<ModeloDto>();
        }

        /// <summary>Marcas disponibles para el vehiculo.</summary>
        public ObservableCollection<MarcaDto> MarcasDisponibles { get; set; }

        /// <summary>Modelos disponibles, filtrados por la marca seleccionada.</summary>
        public ObservableCollection<ModeloDto> ModelosDisponibles { get; set; }


        /// <summary>
        /// Cliente ya existente al que se le quiere agregar un vehiculo. Cuando tiene valor,
        /// el modal no crea un cliente nuevo: solo el vehiculo y su vinculo.
        /// </summary>
        private ClienteDto? _clienteExistente;
        public ClienteDto? ClienteExistente
        {
            get => _clienteExistente;
            set
            {
                _clienteExistente = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(UsaClienteExistente));
                OnPropertyChanged(nameof(PuedeEditarCliente));
            }
        }

        /// <summary>Indica que el vehiculo se agregara a un cliente ya registrado.</summary>
        public bool UsaClienteExistente => ClienteExistente != null;

        /// <summary>
        /// Indica si los datos del cliente pueden editarse. Cuando el vehiculo se suma a un
        /// cliente ya registrado, los campos del cliente van en solo lectura.
        /// </summary>
        public bool PuedeEditarCliente => !UsaClienteExistente;

        /// <summary>
        /// Reinicia el estado del ViewModel. Necesario porque en WPF el servicio es Scoped
        /// y vive toda la vida de la aplicacion: el modal puede abrirse varias veces.
        /// </summary>
        public void Reiniciar(ClienteDto? clienteExistente = null)
        {
            ClienteExistente = clienteExistente;
            ClienteCreado = null;
            VehiculoCreado = null;

            Dni = clienteExistente?.Dni ?? string.Empty;
            Apellido = clienteExistente?.Apellido ?? string.Empty;
            Nombre = clienteExistente?.Nombre ?? string.Empty;
            Telefono = clienteExistente?.Telefono ?? string.Empty;

            Patente = string.Empty;
            Anio = DateTime.Now.Year;
            IdMarca = 0;
            IdModelo = 0;

            ClearAllErrors();
        }

        /// <summary>Cliente recien creado (se completa al guardar con exito).</summary>
        public ClienteDto? ClienteCreado { get; private set; }

        /// <summary>Vehiculo recien creado (se completa al guardar con exito).</summary>
        public VehiculoDto? VehiculoCreado { get; private set; }

        #region Datos del cliente

        private string _dni = string.Empty;
        public string Dni
        {
            get => _dni;
            set { _dni = value; OnPropertyChanged(); ValidateProperty(); }
        }

        private string _apellido = string.Empty;
        public string Apellido
        {
            get => _apellido;
            set { _apellido = value; OnPropertyChanged(); ValidateProperty(); }
        }

        private string _nombre = string.Empty;
        public string Nombre
        {
            get => _nombre;
            set { _nombre = value; OnPropertyChanged(); ValidateProperty(); }
        }

        private string _telefono = string.Empty;
        public string Telefono
        {
            get => _telefono;
            set { _telefono = value; OnPropertyChanged(); ValidateProperty(); }
        }

        #endregion

        #region Datos del vehiculo

        private string _patente = string.Empty;
        public string Patente
        {
            get => _patente;
            set { _patente = value; OnPropertyChanged(); ValidateProperty(); }
        }

        private int _idMarca;
        public int IdMarca
        {
            get => _idMarca;
            set
            {
                _idMarca = value;
                OnPropertyChanged();
                ValidateProperty();
                FiltrarModelosPorMarca();
            }
        }

        private int _idModelo;
        public int IdModelo
        {
            get => _idModelo;
            set { _idModelo = value; OnPropertyChanged(); ValidateProperty(); }
        }

        private int _anio = DateTime.Now.Year;
        public int Anio
        {
            get => _anio;
            set { _anio = value; OnPropertyChanged(); ValidateProperty(); }
        }



        #endregion

        /// <summary>Carga las marcas y los modelos disponibles.</summary>
        public async Task CargarCombosAsync()
        {
            MarcasDisponibles.Clear();
            ModelosDisponibles.Clear();

            try
            {
                var marcas = await _listarMarcaHandler.HandleAsync();
                foreach (var marca in marcas)
                {
                    MarcasDisponibles.Add(marca);
                }

                var modelos = await _listarModelosHandler.HandleAsync();
                _todosLosModelos = modelos;

                FiltrarModelosPorMarca();
            }
            catch
            {
                // Silenciar error en carga de combos
            }
        }

        private void FiltrarModelosPorMarca()
        {
            var modeloAnterior = _idModelo;
            ModelosDisponibles.Clear();

            if (_idMarca > 0)
            {
                var modelosDeMarca = _todosLosModelos.Where(modelo => modelo.IdMarca == _idMarca).ToList();
                foreach (var modelo in modelosDeMarca)
                {
                    ModelosDisponibles.Add(modelo);
                }

                IdModelo = modelosDeMarca.Any(modelo => modelo.Id == modeloAnterior) ? modeloAnterior : 0;
            }
            else
            {
                foreach (var modelo in _todosLosModelos)
                {
                    ModelosDisponibles.Add(modelo);
                }
            }
        }

        public override bool ValidateProperty([CallerMemberName] string? propertyName = null)
        {
            if (propertyName is null) return true;
            ClearErrors(propertyName);

            switch (propertyName)
            {
                case nameof(Dni):
                    if (string.IsNullOrWhiteSpace(Dni) || Dni.Trim().Length < 6)
                        SetError(nameof(Dni), "DNI requerido (mínimo 6 caracteres).");
                    break;
                case nameof(Apellido):
                    if (string.IsNullOrWhiteSpace(Apellido))
                        SetError(nameof(Apellido), "El Apellido es obligatorio.");
                    break;
                case nameof(Nombre):
                    if (string.IsNullOrWhiteSpace(Nombre))
                        SetError(nameof(Nombre), "El Nombre es obligatorio.");
                    break;
                case nameof(Telefono):
                    if (string.IsNullOrWhiteSpace(Telefono))
                        SetError(nameof(Telefono), "El Teléfono es obligatorio (para avisar cuando el auto esté listo).");
                    break;
                case nameof(Patente):
                    if (string.IsNullOrWhiteSpace(Patente) || Patente.Trim().Length < 5)
                        SetError(nameof(Patente), "Patente requerida (mínimo 5 caracteres).");
                    break;
                case nameof(IdMarca):
                    if (IdMarca <= 0)
                        SetError(nameof(IdMarca), "Debe seleccionar una Marca.");
                    break;
                case nameof(IdModelo):
                    if (IdModelo <= 0)
                        SetError(nameof(IdModelo), "Debe seleccionar un Modelo.");
                    break;
                case nameof(Anio):
                    if (Anio < 1900 || Anio > DateTime.Now.Year + 1)
                        SetError(nameof(Anio), $"Año inválido (debe ser entre 1900 y {DateTime.Now.Year + 1}).");
                    break;
            }

            return !GetErrors(propertyName).Cast<object>().Any();
        }

        public bool ValidateAll()
        {
            // Los datos del cliente solo se validan cuando se crea uno nuevo.
            if (!UsaClienteExistente)
            {
                ValidateProperty(nameof(Dni));
                ValidateProperty(nameof(Apellido));
                ValidateProperty(nameof(Nombre));
                ValidateProperty(nameof(Telefono));
            }
            ValidateProperty(nameof(Patente));
            ValidateProperty(nameof(IdMarca));
            ValidateProperty(nameof(IdModelo));
            ValidateProperty(nameof(Anio));
            return !HasErrors;
        }

        /// <summary>
        /// Registra el cliente, el vehiculo y el vinculo de propiedad, en ese orden.
        /// Si hay un cliente ya seleccionado, solo se crean el vehiculo y su vinculo.
        /// Devuelve true solo si todas las operaciones quedaron persistidas.
        /// </summary>
        public async Task<bool> RegistrarAsync()
        {
            if (!ValidateAll()) return false;

            if (IsLoading) return false;
            IsLoading = true;

            var clienteExistente = ClienteExistente;

            Entidad.Cliente? cliente = null;
            if (clienteExistente == null)
            {
                cliente = new Entidad.Cliente
                {
                    Nombre = Nombre.Trim(),
                    Apellido = Apellido.Trim(),
                    Dni = Dni.Trim(),
                    Telefono = Telefono.Trim(),
                    Email = string.Empty,
                    Direccion = string.Empty,
                    FechaNacimiento = DateTime.MinValue,
                    Activo = true
                };
            }

            var vehiculo = new Entidad.Vehiculo
            {
                ModeloId = IdModelo,
                Patente = Patente.Trim().ToUpper(),
                Anio = Anio,
                KmActual = 0
            };

            try
            {
                // 1) Cliente (solo si es nuevo)
                if (cliente != null)
                {
                    await _clienteRepository.AddAsync(cliente);
                }

                // 2) Vehiculo
                await _vehiculoRepository.AddAsync(vehiculo);

                // 3) Vinculo de propiedad (propietario actual)
                await _propietarioRepository.AddAsync(new PropietarioVehiculo
                {
                    ClienteId = cliente?.Id ?? clienteExistente!.IdCliente,
                    VehiculoId = vehiculo.Id,
                    FechaAdquisicion = DateTime.Now,
                    EsActual = true
                });

                ClienteCreado = cliente != null ? _mapper.Map<ClienteDto>(cliente) : clienteExistente;
                VehiculoCreado = _mapper.Map<VehiculoDto>(vehiculo);

                return true;
            }
            catch (Exception ex)
            {
                RegistrationFailed?.Invoke(this, ex.Message);
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>Se dispara cuando el registro falla.</summary>
        public event EventHandler<string>? RegistrationFailed;
    }
}
