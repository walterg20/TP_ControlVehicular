using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Services;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public class ServicioViewModel : BaseViewModel
    {
        private readonly ListarServiciosHandler _listarServiciosHandler;
        private readonly RegistrarServicioHandler _registrarServicioHandler;
        private readonly ModificarServicioHandler _modificarServicioHandler;
        private readonly EliminarServicioHandler _eliminarServicioHandler;

        private int _idServicio;
        public int IdServicio
        {
            get => _idServicio;
            set => SetProperty(ref _idServicio, value);
        }

        private string _nombre = string.Empty;
        public string Nombre
        {
            get => _nombre;
            set { SetProperty(ref _nombre, value); ValidateProperty(); }
        }

        private decimal _precio;
        public decimal Precio
        {
            get => _precio;
            set { SetProperty(ref _precio, value); ValidateProperty(); }
        }

        private bool _activo = true;
        public bool Activo
        {
            get => _activo;
            set => SetProperty(ref _activo, value);
        }

        private string _textoBusqueda = string.Empty;
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                if (SetProperty(ref _textoBusqueda, value))
                {
                    AplicarFiltro();
                }
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private readonly List<ServicioDto> _todosLosServicios = new();
        public ObservableCollection<ServicioDto> ListadoServiciosFiltered { get; } = new();

        public ICommand CargarServiciosCommand { get; }

        public ServicioViewModel(
            ListarServiciosHandler listarServiciosHandler,
            RegistrarServicioHandler registrarServicioHandler,
            ModificarServicioHandler modificarServicioHandler,
            EliminarServicioHandler eliminarServicioHandler)
        {
            _listarServiciosHandler = listarServiciosHandler;
            _registrarServicioHandler = registrarServicioHandler;
            _modificarServicioHandler = modificarServicioHandler;
            _eliminarServicioHandler = eliminarServicioHandler;

            CargarServiciosCommand = new RelayCommand(LoadAsync);
        }

        public async Task LoadAsync()
        {
            IsLoading = true;
            try
            {
                var list = await _listarServiciosHandler.HandleAsync();
                _todosLosServicios.Clear();
                _todosLosServicios.AddRange(list);
                AplicarFiltro();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void AplicarFiltro()
        {
            ListadoServiciosFiltered.Clear();
            var query = _todosLosServicios.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                var txt = TextoBusqueda.Trim().ToLower();
                query = query.Where(s => s.Nombre.ToLower().Contains(txt));
            }

            foreach (var item in query)
            {
                ListadoServiciosFiltered.Add(item);
            }
        }

        public override bool ValidateProperty([CallerMemberName] string? propertyName = null)
        {
            if (propertyName is null) return true;
            ClearErrors(propertyName);

            switch (propertyName)
            {
                case nameof(Nombre):
                    if (string.IsNullOrWhiteSpace(Nombre))
                        SetError(nameof(Nombre), "El nombre del servicio es obligatorio.");
                    else if (Nombre.Trim().Length < 3)
                        SetError(nameof(Nombre), "El nombre del servicio debe tener al menos 3 caracteres.");
                    break;
                case nameof(Precio):
                    if (Precio <= 0)
                        SetError(nameof(Precio), "El precio del servicio debe ser mayor a 0.");
                    break;
            }

            return !GetErrors(propertyName).Cast<object>().Any();
        }

        public bool ValidateAll()
        {
            ValidateProperty(nameof(Nombre));
            ValidateProperty(nameof(Precio));
            return !HasErrors;
        }

        public async Task<bool> SaveAsync()
        {
            if (!ValidateAll()) return false;

            var entity = new Servicio
            {
                Id = IdServicio,
                Nombre = Nombre.Trim(),
                Precio = Precio,
                Activo = Activo
            };

            if (IdServicio == 0)
            {
                await _registrarServicioHandler.HandleAsync(entity);
            }
            else
            {
                await _modificarServicioHandler.HandleAsync(entity);
            }

            LimpiarFormulario();
            await LoadAsync();
            return true;
        }

        public async Task DeleteAsync(int id)
        {
            await _eliminarServicioHandler.HandleAsync(id);
            await LoadAsync();
        }

        public void LimpiarFormulario()
        {
            IdServicio = 0;
            Nombre = string.Empty;
            Precio = 0;
            Activo = true;
            ClearAllErrors();
        }
    }
}
