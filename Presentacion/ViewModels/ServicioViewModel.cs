using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;
using TP_ControlVehicular.Negocio.Services;
using ServicioEntidad = TP_ControlVehicular.Entidad.Servicio;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public class ServicioViewModel : BaseViewModel, INotifyDataErrorInfo
    {
        private readonly RegistrarServicioHandler _registrarServicioHandler;
        private readonly IServicioRepository _servicioRepository;
        private readonly AutoMapper.IMapper _mapper;
        private readonly Dictionary<string, List<string>> _errors = new();

        public ServicioViewModel(RegistrarServicioHandler registrarServicioHandler, IServicioRepository servicioRepository, AutoMapper.IMapper mapper)
        {
            _registrarServicioHandler = registrarServicioHandler;
            _servicioRepository = servicioRepository;
            _mapper = mapper;
            Servicios = new ObservableCollection<ServicioDto>();
            Servicios.CollectionChanged += (_, _) => OnPropertyChanged(nameof(ListadoServiciosFiltered));
            Activo = true;
            RegistrarCommand = new RelayCommand(async () => await RegistrarServicioAsync(), () => !HasErrors);
        }

        public ObservableCollection<ServicioDto> Servicios { get; }

        private string _textoBusqueda = string.Empty;
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set { _textoBusqueda = value; OnPropertyChanged(); OnPropertyChanged(nameof(ListadoServiciosFiltered)); }
        }

        public IEnumerable<ServicioDto> ListadoServiciosFiltered =>
            string.IsNullOrWhiteSpace(TextoBusqueda)
                ? Servicios
                : Servicios.Where(s => s.Nombre.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase));

        public int IdServicio { get; set; }

        private string _nombre = string.Empty;
        public string Nombre { get => _nombre; set { _nombre = value; OnPropertyChanged(); ValidateProperty(); } }
        private decimal _precio;
        public decimal Precio { get => _precio; set { _precio = value; OnPropertyChanged(); } }
        private bool _activo;
        public bool Activo { get => _activo; set { _activo = value; OnPropertyChanged(); } }

        public ICommand RegistrarCommand { get; }
        public event EventHandler<bool>? RegistrationCompleted;
        public event EventHandler<string>? RegistrationFailed;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public async Task LoadAsync()
        {
            Servicios.Clear();
            IEnumerable<ServicioEntidad> lista;
            try { lista = await _servicioRepository.GetActivosAsync(); }
            catch { lista = Enumerable.Empty<ServicioEntidad>(); }
            foreach (var servicio in lista)
                Servicios.Add(_mapper.Map<ServicioDto>(servicio));
        }

        public async Task DeleteServicioAsync(int id)
        {
            var servicio = await _servicioRepository.GetByIdAsync(id);
            if (servicio is null) return;
            servicio.Activo = false;
            await _servicioRepository.UpdateAsync(servicio);
            await LoadAsync();
        }

        public void CargarServicio(ServicioDto servicio)
        {
            IdServicio = servicio.IdServicio;
            Nombre = servicio.Nombre;
            Precio = servicio.Precio;
            Activo = servicio.Activo;
        }

        public void LimpiarFormulario()
        {
            IdServicio = 0;
            Nombre = string.Empty;
            Precio = 0;
            Activo = true;
            _errors.Clear();
            OnErrorsChanged(null);
        }

        public async Task<bool> RegistrarServicioAsync()
        {
            ValidateProperty(nameof(Nombre));
            if (HasErrors) return false;

            var servicio = new ServicioEntidad
            {
                IdServicio = this.IdServicio,
                Nombre = Nombre,
                Precio = Precio,
                Activo = Activo
            };

            try
            {
                var dto = await _registrarServicioHandler.HandleAsync(servicio);
                var existente = Servicios.FirstOrDefault(s => s.IdServicio == servicio.IdServicio);
                if (existente is null) Servicios.Add(dto);
                else Servicios[Servicios.IndexOf(existente)] = dto;
                RegistrationCompleted?.Invoke(this, true);
                return true;
            }
            catch (Exception ex)
            {
                RegistrationFailed?.Invoke(this, "Error al guardar servicio: " + ex.Message);
                return false;
            }
        }

        private void ValidateProperty([CallerMemberName] string? propertyName = null)
        {
            if (propertyName is null) return;
            _errors.Remove(propertyName);
            var errors = new List<string>();
            switch (propertyName)
            {
                case nameof(Nombre) when string.IsNullOrWhiteSpace(Nombre) || Nombre.Length < 2:
                    errors.Add("Nombre requerido (min 2 caracteres)."); break;
            }
            if (errors.Any()) _errors[propertyName] = errors;
            OnErrorsChanged(propertyName);
        }

        public bool HasErrors => _errors.Any();
        public IEnumerable GetErrors(string? propertyName) =>
            string.IsNullOrEmpty(propertyName)
                ? _errors.SelectMany(entry => entry.Value)
                : _errors.TryGetValue(propertyName, out var errors) ? errors : Enumerable.Empty<string>();
        private void OnErrorsChanged(string? propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            if (RegistrarCommand is RelayCommand command) command.RaiseCanExecuteChanged();
        }
    }
}
