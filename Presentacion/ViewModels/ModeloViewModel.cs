using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Services;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public class ModeloViewModel : BaseViewModel
    {
        private readonly ListarModelosHandler _listarModelosHandler;
        private readonly RegistrarModeloHandler _registrarModeloHandler;
        private readonly ModificarModeloHandler _modificarModeloHandler;
        private readonly EliminarModeloHandler _eliminarModeloHandler;
        private readonly ListarMarcaHandler _listarMarcaHandler;

        public ModeloViewModel(
            ListarModelosHandler listarModelosHandler,
            RegistrarModeloHandler registrarModeloHandler,
            ModificarModeloHandler modificarModeloHandler,
            EliminarModeloHandler eliminarModeloHandler,
            ListarMarcaHandler listarMarcaHandler)
        {
            _listarModelosHandler = listarModelosHandler;
            _registrarModeloHandler = registrarModeloHandler;
            _modificarModeloHandler = modificarModeloHandler;
            _eliminarModeloHandler = eliminarModeloHandler;
            _listarMarcaHandler = listarMarcaHandler;

            Modelos = new ObservableCollection<ModeloDto>();
            MarcasDisponibles = new ObservableCollection<MarcaDto>();
            ((ObservableCollection<ModeloDto>)Modelos).CollectionChanged += (s, e) => OnPropertyChanged(nameof(ListadoModelosFiltered));
        }

        public ObservableCollection<ModeloDto> Modelos { get; set; }
        public ObservableCollection<MarcaDto> MarcasDisponibles { get; set; }

        private string _textoBusqueda = string.Empty;
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                _textoBusqueda = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ListadoModelosFiltered));
            }
        }

        public IEnumerable<ModeloDto> ListadoModelosFiltered =>
            string.IsNullOrWhiteSpace(TextoBusqueda)
                ? Modelos
                : Modelos.Where(modelo => (modelo.NombreModelo ?? string.Empty).IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0
                                  || (modelo.MarcaNombre ?? string.Empty).IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0);

        private string _nombreModelo = string.Empty;
        public string NombreModelo
        {
            get => _nombreModelo;
            set { _nombreModelo = value; OnPropertyChanged(); ValidateProperty(); }
        }

        private int _idMarca;
        public int IdMarca
        {
            get => _idMarca;
            set { _idMarca = value; OnPropertyChanged(); ValidateProperty(); }
        }

        public override bool ValidateProperty([CallerMemberName] string? propertyName = null)
        {
            if (propertyName is null) return true;
            ClearErrors(propertyName);

            switch (propertyName)
            {
                case nameof(NombreModelo):
                    if (string.IsNullOrWhiteSpace(NombreModelo) || NombreModelo.Trim().Length < 4)
                        SetError(nameof(NombreModelo), "Nombre del Modelo requerido (mínimo 4 caracteres).");
                    break;
                case nameof(IdMarca):
                    if (IdMarca <= 0)
                        SetError(nameof(IdMarca), "Debe seleccionar una Marca de la lista.");
                    break;
            }

            return !GetErrors(propertyName).Cast<object>().Any();
        }

        public bool ValidateAll()
        {
            ValidateProperty(nameof(NombreModelo));
            ValidateProperty(nameof(IdMarca));
            return !HasErrors;
        }

        public ModeloDto? ModeloSeleccionado { get; set; }

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
                else if (nuevoId > 0)
                {
                    IdMarca = nuevoId;
                }
            }
            catch
            {
                // Silenciar error
            }
        }

        public async Task LoadAsync()
        {
            Modelos.Clear();
            await CargarMarcasAsync();
            try
            {
                var lista = await _listarModelosHandler.HandleAsync();
                foreach (var modelo in lista)
                {
                    Modelos.Add(modelo);
                }
            }
            catch
            {
                // Silenciar error en carga inicial
            }
        }

        public async Task<bool> GuardarModeloAsync()
        {
            if (!ValidateAll()) return false;

            var modeloSeleccionado = ModeloSeleccionado;
            var modelo = new Entidad.Modelo
            {
                Id = modeloSeleccionado?.Id ?? 0,
                MarcaId = IdMarca,
                NombreModelo = NombreModelo.Trim()
            };

            try
            {
                if (modelo.Id == 0)
                {
                    var dto = await _registrarModeloHandler.HandleAsync(modelo);
                    Modelos.Add(dto);
                }
                else
                {
                    var dto = await _modificarModeloHandler.HandleAsync(modelo);
                    var exist = Modelos.FirstOrDefault(modelo => modelo.Id == dto.Id);
                    if (exist != null)
                    {
                        exist.NombreModelo = dto.NombreModelo;
                        exist.IdMarca = dto.IdMarca;
                        exist.MarcaNombre = dto.MarcaNombre;
                        OnPropertyChanged(nameof(ListadoModelosFiltered));
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> EliminarModeloAsync(int id)
        {
            try
            {
                await _eliminarModeloHandler.HandleAsync(id);
                var exist = Modelos.FirstOrDefault(modelo => modelo.Id == id);
                if (exist != null)
                {
                    Modelos.Remove(exist);
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
