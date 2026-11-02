// walterg20
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Services;
using TP_ControlVehicular.Presentacion.Helpers;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    /// <summary>
    /// Listado de marcas con paginacion y busqueda en la base de datos.
    /// </summary>
    public class MarcaViewModel : BasePaginableViewModel
    {
        protected override string Modulo => "Marca";

        private readonly ListarMarcaHandler _listarMarcaHandler;
        private readonly RegistrarMarcaHandler _registrarMarcaHandler;
        private readonly ModificarMarcaHandler _modificarMarcaHandler;
        private readonly EliminarMarcaHandler _eliminarMarcaHandler;
        private readonly Debouncer _debouncer;

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
            _debouncer = new Debouncer();

            Marcas = new ObservableCollection<MarcaDto>();
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
                // El filtro cambio: el listado vuelve a la primera pagina.
                ReiniciarPagina();
            }
        }

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

        /// <summary>
        /// Carga la primera pagina del listado (usada al abrir la pantalla).
        /// </summary>
        public async Task LoadAsync()
        {
            ReiniciarPagina();
            await CargarPaginaAsync();
        }

        /// <summary>
        /// Aplica los filtros actuales y recarga el listado desde la primera pagina.
        /// </summary>
        public async Task BuscarAsync()
        {
            ReiniciarPagina();
            await CargarPaginaAsync();
        }

        /// <summary>
        /// Programa la busqueda con un retardo breve, para no consultar la base de datos
        /// en cada tecla escrita.
        /// </summary>
        public void ProgramarBusqueda() => _debouncer.Debounce(BuscarAsync);

        /// <summary>Cancela una busqueda programada que todavia no se ejecuto.</summary>
        public void CancelarBusquedaProgramada() => _debouncer.Cancel();

        protected override async Task CargarPaginaAsync()
        {
            IsLoading = true;
            try
            {
                var (items, total) = await _listarMarcaHandler.HandlePaginadoAsync(
                    TextoBusqueda.Trim(),
                    PaginaActual,
                    TamanoPagina);

                Marcas.Clear();
                foreach (var marca in items)
                {
                    Marcas.Add(marca);
                }

                ActualizarTotales(total);
            }
            catch
            {
                // Silenciar error en carga inicial
                Marcas.Clear();
                ActualizarTotales(0);
            }
            finally
            {
                IsLoading = false;
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
                    await _registrarMarcaHandler.HandleAsync(marca);
                }
                else
                {
                    await _modificarMarcaHandler.HandleAsync(marca);
                }

                // Recargar la pagina actual para que el listado refleje el alta o la edicion.
                await CargarPaginaAsync();
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
                await CargarPaginaAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
