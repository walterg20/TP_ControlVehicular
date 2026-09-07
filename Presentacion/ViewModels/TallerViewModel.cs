using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Services;
using TP_ControlVehicular.Negocio.Interfaces;
using AutoMapper;
using System.Collections.Generic;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public class TallerViewModel : BaseViewModel, INotifyDataErrorInfo
    {
        private readonly ListarTallerHandler _listarHandler;
        private readonly RegistrarTallerHandler _registrarHandler;
        private readonly ModificarTallerHandler _modificarHandler;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, List<string>> _errors = new();

        public TallerViewModel(ListarTallerHandler listarHandler, RegistrarTallerHandler registrarHandler, ModificarTallerHandler modificarHandler, IMapper mapper)
        {
            _listarHandler = listarHandler;
            _registrarHandler = registrarHandler;
            _modificarHandler = modificarHandler;
            _mapper = mapper;
            Talleres = new ObservableCollection<TallerDto>();
            ((ObservableCollection<TallerDto>)Talleres).CollectionChanged += (s, e) => OnPropertyChanged(nameof(ListadoTalleresFiltered));
            Activo = true;
            RegistrarCommand = new RelayCommand(async () => await RegistrarAsync(), () => !HasErrors);
        }

        public ObservableCollection<TallerDto> Talleres { get; set; }

        private string _textoBusqueda = string.Empty;
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set { _textoBusqueda = value; OnPropertyChanged(); OnPropertyChanged(nameof(ListadoTalleresFiltered)); }
        }

        public IEnumerable<TallerDto> ListadoTalleresFiltered =>
            string.IsNullOrWhiteSpace(TextoBusqueda)
                ? Talleres
                : Talleres.Where(t => (t.Nombre ?? string.Empty).IndexOf(TextoBusqueda, System.StringComparison.OrdinalIgnoreCase) >= 0);

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
            set { _direccion = value; OnPropertyChanged(); }
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

        public ICommand RegistrarCommand { get; }

        public async Task LoadAsync()
        {
            Talleres.Clear();
            try
            {
                var lista = await _listarHandler.HandleAsync();
                foreach (var t in lista)
                {
                    Talleres.Add(t);
                }
            }
            catch
            {
                // Si falla, la colección se queda vacía
            }
        }

        public async Task<bool> RegistrarAsync()
        {
            return await GuardarTallerAsync();
        }

        // Guarda un taller: si es nuevo (IdTaller == 0) usa RegistrarTallerHandler,
        // si existe (IdTaller > 0) usa ModificarTallerHandler. Retorna true si tuvo éxito.
        public async Task<bool> GuardarTallerAsync()
        {
            ValidateProperty(nameof(Nombre));
            if (HasErrors) return false;

            var tallerSeleccionado = TallerSeleccionado;
            if (tallerSeleccionado is null) return false;

            var taller = new TP_ControlVehicular.Entidad.Taller
            {
                Id = tallerSeleccionado.IdTaller,
                Nombre = tallerSeleccionado.Nombre,
                Direccion = tallerSeleccionado.Direccion,
                Telefono = tallerSeleccionado.Telefono,
                Activo = tallerSeleccionado.Activo
            };

            try
            {
                if (tallerSeleccionado.IdTaller == 0)
                {
                    var dto = await _registrarHandler.HandleAsync(taller);
                    Talleres.Add(dto);
                }
                else
                {
                    var dto = await _modificarHandler.HandleAsync(taller);
                    var index = -1;
                    for (int i = 0; i < Talleres.Count; i++)
                    {
                        if (Talleres[i].IdTaller == dto.IdTaller)
                        {
                            index = i;
                            break;
                        }
                    }
                    if (index >= 0) Talleres[index] = dto;
                    else Talleres.Add(dto);
                }

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

        private void ValidateProperty([CallerMemberName] string? propertyName = null)
        {
            if (propertyName is null) return;
            if (_errors.ContainsKey(propertyName)) _errors.Remove(propertyName);
            var list = new List<string>();

            switch (propertyName)
            {
                case nameof(Nombre):
                    if (string.IsNullOrWhiteSpace(Nombre) || Nombre.Length < 2) list.Add("Nombre requerido (mín. 2 caracteres).");
                    break;
            }

            if (list.Any()) _errors[propertyName] = list;
            OnErrorsChanged(propertyName);
        }

        public bool HasErrors => _errors.Any();

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public System.Collections.IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return _errors.SelectMany(kv => kv.Value);
            return _errors.TryGetValue(propertyName, out var list) ? list : Enumerable.Empty<string>();
        }

        protected void OnErrorsChanged(string? propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            if (RegistrarCommand is RelayCommand rc) rc.RaiseCanExecuteChanged();
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
                var dto = await _modificarHandler.HandleAsync(taller);
                var index = -1;
                for (int i = 0; i < Talleres.Count; i++)
                {
                    if (Talleres[i].IdTaller == dto.IdTaller)
                    {
                        index = i;
                        break;
                    }
                }
                if (index >= 0) Talleres[index] = dto;
                else Talleres.Add(dto);
            }
            catch (Exception ex)
            {
                RegistrationFailed?.Invoke(this, "Error al actualizar taller: " + ex.Message);
            }
        }
    }
}