using AutoMapper;
using System.Collections.ObjectModel;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;
using TP_ControlVehicular.Negocio.Services;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public class VehiculoViewModel : BaseViewModel
    {
        private readonly ListarVehiculosHandler _listarVehiculosHandler;
        private readonly RegistrarVehiculoHandler _registrarVehiculoHandler;
        private readonly ModificarVehiculoHandler _modificarVehiculoHandler;
        private readonly EliminarVehiculoHandler _eliminarVehiculoHandler;
        private readonly IClienteRepository _clienteRepository;
        private readonly ListarModelosHandler _listarModelosHandler;
        private readonly ListarMarcaHandler _listarMarcaHandler;
        private readonly IMapper _mapper;

        public VehiculoViewModel(
            ListarVehiculosHandler listarVehiculosHandler,
            RegistrarVehiculoHandler registrarVehiculoHandler,
            ModificarVehiculoHandler modificarVehiculoHandler,
            EliminarVehiculoHandler eliminarVehiculoHandler,
            IClienteRepository clienteRepository,
            ListarModelosHandler listarModelosHandler,
            ListarMarcaHandler listarMarcaHandler,
            IMapper mapper)
        {
            _listarVehiculosHandler = listarVehiculosHandler;
            _registrarVehiculoHandler = registrarVehiculoHandler;
            _modificarVehiculoHandler = modificarVehiculoHandler;
            _eliminarVehiculoHandler = eliminarVehiculoHandler;
            _clienteRepository = clienteRepository;
            _listarModelosHandler = listarModelosHandler;
            _listarMarcaHandler = listarMarcaHandler;
            _mapper = mapper;

            Vehiculos = new ObservableCollection<VehiculoDto>();
            ClientesDisponibles = new ObservableCollection<ClienteDto>();
            MarcasDisponibles = new ObservableCollection<MarcaDto>();
            ModelosDisponibles = new ObservableCollection<ModeloDto>();
            ((ObservableCollection<VehiculoDto>)Vehiculos).CollectionChanged += (s, e) => OnPropertyChanged(nameof(ListadoVehiculosFiltered));
        }

        public ObservableCollection<VehiculoDto> Vehiculos { get; set; }
        public ObservableCollection<ClienteDto> ClientesDisponibles { get; set; }
        public ObservableCollection<MarcaDto> MarcasDisponibles { get; set; }
        public ObservableCollection<ModeloDto> ModelosDisponibles { get; set; }
        private List<ModeloDto> _todosLosModelos = new();

        private string _textoBusqueda = string.Empty;
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                _textoBusqueda = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ListadoVehiculosFiltered));
            }
        }

        public IEnumerable<VehiculoDto> ListadoVehiculosFiltered =>
            string.IsNullOrWhiteSpace(TextoBusqueda)
                ? Vehiculos
                : Vehiculos.Where(vehiculo => (vehiculo.Patente ?? string.Empty).IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0
                                    || (vehiculo.ClienteNombre ?? string.Empty).IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0
                                    || (vehiculo.MarcaNombre ?? string.Empty).IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0
                                    || (vehiculo.ModeloNombre ?? string.Empty).IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0);

        private int _idCliente;
        public int IdCliente
        {
            get => _idCliente;
            set { _idCliente = value; OnPropertyChanged(); ValidateProperty(); }
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

        private string _patente = string.Empty;
        public string Patente
        {
            get => _patente;
            set { _patente = value; OnPropertyChanged(); ValidateProperty(); }
        }

        private int _anio;
        public int Anio
        {
            get => _anio;
            set { _anio = value; OnPropertyChanged(); ValidateProperty(); }
        }

        private int _kmActual;
        public int KmActual
        {
            get => _kmActual;
            set { _kmActual = value; OnPropertyChanged(); ValidateProperty(); }
        }

        private void FiltrarModelosPorMarca()
        {
            var modelAnterior = _idModelo;
            ModelosDisponibles.Clear();

            if (_idMarca > 0)
            {
                var modelosDeMarca = _todosLosModelos.Where(modelo => modelo.IdMarca == _idMarca).ToList();
                foreach (var modelo in modelosDeMarca)
                {
                    ModelosDisponibles.Add(modelo);
                }

                if (modelosDeMarca.Any(modelo => modelo.Id == modelAnterior))
                {
                    IdModelo = modelAnterior;
                }
                else
                {
                    IdModelo = 0;
                }
            }
            else
            {
                foreach (var modelo in _todosLosModelos)
                {
                    ModelosDisponibles.Add(modelo);
                }
            }
        }

        public override bool ValidateProperty([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            if (propertyName is null) return true;
            ClearErrors(propertyName);

            switch (propertyName)
            {
                case nameof(Patente):
                    if (string.IsNullOrWhiteSpace(Patente) || Patente.Trim().Length < 5)
                        SetError(nameof(Patente), "Patente requerida (mínimo 5 caracteres).");
                    break;
                case nameof(IdCliente):
                    if (IdCliente <= 0)
                        SetError(nameof(IdCliente), "Debe seleccionar un Cliente.");
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
                case nameof(KmActual):
                    if (KmActual < 0)
                        SetError(nameof(KmActual), "El kilometraje no puede ser negativo.");
                    break;
            }

            return !GetErrors(propertyName).Cast<object>().Any();
        }

        public bool ValidateAll()
        {
            ValidateProperty(nameof(Patente));
            ValidateProperty(nameof(IdCliente));
            ValidateProperty(nameof(IdMarca));
            ValidateProperty(nameof(IdModelo));
            ValidateProperty(nameof(Anio));
            ValidateProperty(nameof(KmActual));
            return !HasErrors;
        }

        public VehiculoDto? VehiculoSeleccionado { get; set; }

        public async Task CargarMarcasAsync(int? seleccionarIdMarca = null)
        {
            var idsAnteriores = MarcasDisponibles.Select(marca => marca.Id).ToHashSet();
            MarcasDisponibles.Clear();

            try
            {
                var marcas = await _listarMarcaHandler.HandleAsync();
                int nuevoId = 0;
                foreach (var marca in marcas)
                {
                    MarcasDisponibles.Add(marca);
                    if (!idsAnteriores.Contains(marca.Id))
                    {
                        nuevoId = marca.Id;
                    }
                }

                if (seleccionarIdMarca.HasValue && MarcasDisponibles.Any(marca => marca.Id == seleccionarIdMarca.Value))
                {
                    IdMarca = seleccionarIdMarca.Value;
                }
                else if (nuevoId > 0 && idsAnteriores.Count > 0)
                {
                    IdMarca = nuevoId;
                }
            }
            catch
            {
                // Silenciar error en carga de marcas
            }
        }

        public async Task CargarModelosAsync(int? seleccionarIdModelo = null)
        {
            var idsAnteriores = _todosLosModelos.Select(modelo => modelo.Id).ToHashSet();

            try
            {
                var modelos = await _listarModelosHandler.HandleAsync();
                _todosLosModelos = modelos;

                int nuevoId = 0;
                foreach (var modelo in _todosLosModelos)
                {
                    if (!idsAnteriores.Contains(modelo.Id))
                    {
                        nuevoId = modelo.Id;
                    }
                }

                int? targetModeloId = seleccionarIdModelo ?? (nuevoId > 0 && idsAnteriores.Count > 0 ? nuevoId : null);

                if (targetModeloId.HasValue)
                {
                    var target = _todosLosModelos.FirstOrDefault(modelo => modelo.Id == targetModeloId.Value);
                    if (target != null && target.IdMarca > 0)
                    {
                        _idMarca = target.IdMarca;
                        OnPropertyChanged(nameof(IdMarca));
                    }
                }

                FiltrarModelosPorMarca();

                if (targetModeloId.HasValue && ModelosDisponibles.Any(modelo => modelo.Id == targetModeloId.Value))
                {
                    IdModelo = targetModeloId.Value;
                }
            }
            catch
            {
                // Silenciar error en carga de modelos
            }
        }

        public async Task CargarCombosAsync(int? seleccionarIdModelo = null)
        {
            ClientesDisponibles.Clear();

            try
            {
                var clientes = await _clienteRepository.GetAllAsync();
                foreach (var cliente in clientes)
                {
                    ClientesDisponibles.Add(_mapper.Map<ClienteDto>(cliente));
                }

                await CargarMarcasAsync();
                await CargarModelosAsync(seleccionarIdModelo);
            }
            catch
            {
                // Silenciar error en carga de combos
            }
        }

        public async Task LoadAsync()
        {
            Vehiculos.Clear();
            await CargarCombosAsync();

            try
            {
                var lista = await _listarVehiculosHandler.HandleAsync();
                foreach (var vehiculo in lista)
                {
                    Vehiculos.Add(vehiculo);
                }
            }
            catch
            {
                // Silenciar error en carga inicial
            }
        }

        public async Task<bool> GuardarVehiculoAsync()
        {
            if (!ValidateAll()) return false;

            var seleccion = VehiculoSeleccionado;
            var vehiculo = new Entidad.Vehiculo
            {
                Id = seleccion?.Id ?? 0,
                ClienteId = IdCliente,
                ModeloId = IdModelo,
                Patente = Patente.Trim().ToUpper(),
                Anio = Anio,
                KmActual = KmActual
            };

            try
            {
                if (vehiculo.Id == 0)
                {
                    var dto = await _registrarVehiculoHandler.HandleAsync(vehiculo);
                    Vehiculos.Add(dto);
                }
                else
                {
                    var dto = await _modificarVehiculoHandler.HandleAsync(vehiculo);
                    var exist = Vehiculos.FirstOrDefault(vehiculo => vehiculo.Id == dto.Id);
                    if (exist != null)
                    {
                        exist.Patente = dto.Patente;
                        exist.Anio = dto.Anio;
                        exist.KmActual = dto.KmActual;
                        exist.IdCliente = dto.IdCliente;
                        exist.ClienteNombre = dto.ClienteNombre;
                        exist.IdModelo = dto.IdModelo;
                        exist.ModeloNombre = dto.ModeloNombre;
                        exist.MarcaNombre = dto.MarcaNombre;
                        OnPropertyChanged(nameof(ListadoVehiculosFiltered));
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> EliminarVehiculoAsync(int id)
        {
            try
            {
                await _eliminarVehiculoHandler.HandleAsync(id);
                var exist = Vehiculos.FirstOrDefault(vehiculo => vehiculo.Id == id);
                if (exist != null)
                {
                    Vehiculos.Remove(exist);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
