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
        private readonly ListarTallerHandler _listarTallerHandler;
        private readonly RegistrarTallerHandler _registrarTallerHandler;
        private readonly ModificarTallerHandler _modificarTallerHandler;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, List<string>> _errors = new();

        public TallerViewModel(ListarTallerHandler listarTallerHandler, RegistrarTallerHandler registrarTallerHandler, ModificarTallerHandler modificarTallerHandler, IMapper mapper)
        {
            _listarTallerHandler = listarTallerHandler;
            _registrarTallerHandler = registrarTallerHandler;
            _modificarTallerHandler = modificarTallerHandler;
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
                : Talleres.Where(taller => (taller.Nombre ?? string.Empty).IndexOf(TextoBusqueda, System.StringComparison.OrdinalIgnoreCase) >= 0);

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

        public ICommand RegistrarCommand { get; }

        public async Task LoadAsync()
        {
            Talleres.Clear();
            try
            {
                var lista = await _listarTallerHandler.HandleAsync();
                foreach (var taller in lista)
                {
                    Talleres.Add(taller);
                }
            }
            catch
            {
                // Si falla, la colección se queda vacía
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
                    var dto = await _registrarTallerHandler.HandleAsync(taller);
                    Talleres.Add(dto);
                }
                else
                {
                    var dto = await _modificarTallerHandler.HandleAsync(taller);
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
                var dto = await _modificarTallerHandler.HandleAsync(taller);
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