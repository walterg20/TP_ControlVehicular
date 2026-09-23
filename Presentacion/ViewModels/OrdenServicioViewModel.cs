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
        private readonly IMapper _mapper;

        public OrdenServicioViewModel(
            ListarRegistroServiciosHandler listarHandler,
            RegistrarRegistroServicioHandler registrarHandler,
            ModificarRegistroServicioHandler modificarHandler,
            IVehiculoRepository vehiculoRepository,
            ITallerRepository tallerRepository,
            IMapper mapper)
        {
            _listarHandler = listarHandler;
            _registrarHandler = registrarHandler;
            _modificarHandler = modificarHandler;
            _vehiculoRepository = vehiculoRepository;
            _tallerRepository = tallerRepository;
            _mapper = mapper;

            Ordenes = new ObservableCollection<RegistroServicioDto>();
            VehiculosDisponibles = new ObservableCollection<VehiculoDto>();
            TalleresDisponibles = new ObservableCollection<TallerDto>();
            EstadosDisponibles = new ObservableCollection<string> { "Pendiente", "En Proceso", "Completado", "Cancelado" };

            ((ObservableCollection<RegistroServicioDto>)Ordenes).CollectionChanged += (s, e) => OnPropertyChanged(nameof(ListadoOrdenesFiltered));
            
            Fecha = DateTime.Now;
            Estado = "En Proceso";
        }

        public ObservableCollection<RegistroServicioDto> Ordenes { get; set; }
        public ObservableCollection<VehiculoDto> VehiculosDisponibles { get; set; }
        public ObservableCollection<TallerDto> TalleresDisponibles { get; set; }
        public ObservableCollection<string> EstadosDisponibles { get; set; }

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

        public RegistroServicioDto? OrdenSeleccionada { get; set; }

        public async Task LoadCombosAsync()
        {
            VehiculosDisponibles.Clear();
            TalleresDisponibles.Clear();
            
            try
            {
                var vehiculos = await _vehiculoRepository.GetAllAsync(); // Asumiendo GetAllAsync
                foreach (var v in vehiculos)
                {
                    VehiculosDisponibles.Add(_mapper.Map<VehiculoDto>(v));
                }

                var talleres = await _tallerRepository.GetAllAsync(); // Asumiendo GetAllAsync
                foreach (var t in talleres)
                {
                    TalleresDisponibles.Add(_mapper.Map<TallerDto>(t));
                }
            }
            catch { }
        }

        public async Task LoadAsync()
        {
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
                Estado = Estado
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
                        SetError(nameof(VehiculoId), "Debe seleccionar un Vehículo.");
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
    }
}
