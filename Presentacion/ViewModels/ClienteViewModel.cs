
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Services;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public class ClienteViewModel: BaseViewModel, INotifyDataErrorInfo
    {
        private readonly RegistrarClienteHandler _registrarClienteHandler;
        private readonly TP_ControlVehicular.Negocio.Interfaces.IClienteRepository _clienteRepository;
        private readonly AutoMapper.IMapper _mapper;
        private readonly Dictionary<string, List<string>> _errors = new();

        public ClienteViewModel(RegistrarClienteHandler registrarClienteHandler, TP_ControlVehicular.Negocio.Interfaces.IClienteRepository clienteRepository, AutoMapper.IMapper mapper)
        {
            _registrarClienteHandler = registrarClienteHandler;
            _clienteRepository = clienteRepository;
            _mapper = mapper;
            Clientes = new ObservableCollection<ClienteDto>();
            // escuchar cambios para invalidar ListadoClientesFiltered
            ((ObservableCollection<ClienteDto>)Clientes).CollectionChanged += (s, e) => OnPropertyChanged(nameof(ListadoClientesFiltered));
            Activo = true; // por defecto
            RegistrarCommand = new RelayCommand(async () => await RegistrarClienteAsync(), () => !HasErrors);
        }

        public ObservableCollection<ClienteDto> Clientes { get; set; }

        private string _textoBusqueda = string.Empty;
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set { _textoBusqueda = value; OnPropertyChanged(); OnPropertyChanged(nameof(ListadoClientesFiltered)); }
        }

        public IEnumerable<ClienteDto> ListadoClientesFiltered =>
            string.IsNullOrWhiteSpace(TextoBusqueda)
                ? Clientes
                : Clientes.Where(c => (c.Nombre ?? string.Empty).IndexOf(TextoBusqueda, System.StringComparison.OrdinalIgnoreCase) >= 0
                                  || (c.Apellido ?? string.Empty).IndexOf(TextoBusqueda, System.StringComparison.OrdinalIgnoreCase) >= 0
                                  || (c.Dni ?? string.Empty).IndexOf(TextoBusqueda, System.StringComparison.OrdinalIgnoreCase) >= 0);

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
            set { _direccion = value; OnPropertyChanged(); }
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
            Clientes.Clear();
            IEnumerable<TP_ControlVehicular.Entidad.Cliente> lista;
            try
            {
                lista = await _clienteRepository.GetActivosAsync();
            }
            catch
            {
                lista = Enumerable.Empty<TP_ControlVehicular.Entidad.Cliente>();
            }

            foreach (var c in lista)
            {
                Clientes.Add(_mapper.Map<ClienteDto>(c));
            }
        }

        public async Task DeleteClienteAsync(int id)
        {
            var entity = await _clienteRepository.GetByIdAsync(id);
            if (entity == null) return;
            entity.Activo = false;
            await _clienteRepository.UpdateAsync(entity);
            await LoadAsync();
        }

        public async Task<bool> RegistrarClienteAsync()
        {
            // Validar antes de intentar registrar
            ValidateProperty(nameof(Nombre));
            ValidateProperty(nameof(Apellido));
            ValidateProperty(nameof(Dni));
            ValidateProperty(nameof(Email));

            if (HasErrors) return false;

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
                // si fue actualización, reemplazar el elemento en la colección
                if (cliente.Id > 0)
                {
                    var existing = Clientes.FirstOrDefault(c => c.IdCliente == cliente.Id);
                    if (existing != null)
                    {
                        var idx = Clientes.IndexOf(existing);
                        Clientes[idx] = dto;
                    }
                    else
                    {
                        Clientes.Add(dto);
                    }
                }
                else
                {
                    Clientes.Add(dto);
                }
                RegistrationCompleted?.Invoke(this, true);
                return true;
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                // intentar detectar violación de unique constraint en SQL Server
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

        // Evento para notificar a la vista que el registro se completó (éxito/fracaso)
        public event EventHandler<bool>? RegistrationCompleted;
        // Evento para notificar errores de registro con mensaje (p.ej. DNI duplicado)
        public event EventHandler<string>? RegistrationFailed;

        private void ValidateProperty([CallerMemberName] string? propertyName = null)
        {
            if (propertyName is null) return;
            if (_errors.ContainsKey(propertyName)) _errors.Remove(propertyName);
            var list = new List<string>();

            switch (propertyName)
            {
                case nameof(Nombre):
                    if (string.IsNullOrWhiteSpace(Nombre) || Nombre.Length < 2) list.Add("Nombre requerido (min 2 caracteres).");
                    break;
                case nameof(Apellido):
                    if (string.IsNullOrWhiteSpace(Apellido) || Apellido.Length < 2) list.Add("Apellido requerido (min 2 caracteres).");
                    break;
                case nameof(Dni):
                    if (string.IsNullOrWhiteSpace(Dni)) list.Add("DNI requerido.");
                    else if (!Regex.IsMatch(Dni, "^\\d{7,8}$")) list.Add("DNI debe ser numérico (7-8 dígitos).");
                    break;
                case nameof(Email):
                    if (!string.IsNullOrWhiteSpace(Email) && !Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) list.Add("Email inválido.");
                    break;
            }

            if (list.Any()) _errors[propertyName] = list;
            OnErrorsChanged(propertyName);
        }

        public bool HasErrors => _errors.Any();

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return _errors.SelectMany(kv => kv.Value);
            return _errors.TryGetValue(propertyName, out var list) ? list : Enumerable.Empty<string>();
        }

        protected void OnErrorsChanged(string? propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            // Actualizar disponibilidad del comando
            if (RegistrarCommand is RelayCommand rc) rc.RaiseCanExecuteChanged();
        }
    }
}
