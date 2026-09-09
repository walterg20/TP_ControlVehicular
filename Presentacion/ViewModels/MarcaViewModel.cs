using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Services;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public class MarcaViewModel : BaseViewModel
    {
        private readonly ListarMarcaHandler _listarMarcaHandler;
        private readonly RegistrarMarcaHandler _registrarMarcaHandler;
        private readonly ModificarMarcaHandler _modificarMarcaHandler;
        private readonly EliminarMarcaHandler _eliminarMarcaHandler;

        public MarcaViewModel(
            ListarMarcaHandler listarMarcaHandler,
            RegistrarMarcaHandler registrarMarcaHandler,
            ModificarMarcaHandler modificarMarcaHandler,
            EliminarMarcaHandler eliminarMarcaHandler)
        {
            _listarMarcaHandler = listarMarcaHandler;
            _registrarMarcaHandler = registrarMarcaHandler;
            _modificarMarcaHandler = modificarMarcaHandler;
            _eliminarMarcaHandler = eliminarMarcaHandler;

            Marcas = new ObservableCollection<MarcaDto>();
            ((ObservableCollection<MarcaDto>)Marcas).CollectionChanged += (s, e) => OnPropertyChanged(nameof(ListadoMarcasFiltered));
        }

        public ObservableCollection<MarcaDto> Marcas { get; set; }

        private string _textoBusqueda = string.Empty;
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                _textoBusqueda = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ListadoMarcasFiltered));
            }
        }

        public IEnumerable<MarcaDto> ListadoMarcasFiltered =>
            string.IsNullOrWhiteSpace(TextoBusqueda)
                ? Marcas
                : Marcas.Where(m => (m.NombreMarca ?? string.Empty).IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0);

        private string _nombreMarca = string.Empty;
        public string NombreMarca
        {
            get => _nombreMarca;
            set { _nombreMarca = value; OnPropertyChanged(); ValidateProperty(); }
        }

        public override bool ValidateProperty([CallerMemberName] string? propertyName = null)
        {
            if (propertyName is null) return true;
            ClearErrors(propertyName);

            switch (propertyName)
            {
                case nameof(NombreMarca):
                    if (string.IsNullOrWhiteSpace(NombreMarca) || NombreMarca.Trim().Length < 5)
                        SetError(nameof(NombreMarca), "Nombre de la Marca requerido (mínimo 5 caracteres).");
                    break;
            }

            return !GetErrors(propertyName).Cast<object>().Any();
        }

        public bool ValidateAll()
        {
            ValidateProperty(nameof(NombreMarca));
            return !HasErrors;
        }

        public MarcaDto? MarcaSeleccionada { get; set; }

        public async Task LoadAsync()
        {
            Marcas.Clear();
            try
            {
                var lista = await _listarMarcaHandler.HandleAsync();
                foreach (var marca in lista)
                {
                    Marcas.Add(marca);
                }
            }
            catch
            {
                // Silenciar error en carga inicial
            }
        }

        public async Task<bool> GuardarMarcaAsync()
        {
            if (!ValidateAll()) return false;

            var marcaSeleccionada = MarcaSeleccionada;
            var marca = new Entidad.Marca
            {
                Id = marcaSeleccionada?.Id ?? 0,
                NombreMarca = NombreMarca.Trim()
            };

            try
            {
                if (marca.Id == 0)
                {
                    var dto = await _registrarMarcaHandler.HandleAsync(marca);
                    Marcas.Add(dto);
                }
                else
                {
                    var dto = await _modificarMarcaHandler.HandleAsync(marca);
                    var exist = Marcas.FirstOrDefault(m => m.Id == dto.Id);
                    if (exist != null)
                    {
                        exist.NombreMarca = dto.NombreMarca;
                        OnPropertyChanged(nameof(ListadoMarcasFiltered));
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> EliminarMarcaAsync(int id)
        {
            try
            {
                await _eliminarMarcaHandler.HandleAsync(id);
                var exist = Marcas.FirstOrDefault(m => m.Id == id);
                if (exist != null)
                {
                    Marcas.Remove(exist);
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
