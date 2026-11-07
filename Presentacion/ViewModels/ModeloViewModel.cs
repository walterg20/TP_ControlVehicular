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
    /// Listado de modelos con paginacion y busqueda en la base de datos.
    /// </summary>
    public class ModeloViewModel : BasePaginableViewModel
    {
        protected override string Modulo => "Modelo";

        private readonly ListarModelosHandler _listarModelosHandler;
        private readonly RegistrarModeloHandler _registrarModeloHandler;
        private readonly ModificarModeloHandler _modificarModeloHandler;
        private readonly EliminarModeloHandler _eliminarModeloHandler;
        private readonly ListarMarcaHandler _listarMarcaHandler;
        private readonly Debouncer _debouncer;

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
            _debouncer = new Debouncer();

            Modelos = new ObservableCollection<ModeloDto>();
            MarcasDisponibles = new ObservableCollection<MarcaDto>();
        }

        public ObservableCollection<ModeloDto> Modelos { get; set; }
        public ObservableCollection<MarcaDto> MarcasDisponibles { get; set; }

        private string _filtroEstado = "Todos";
        public string FiltroEstado
        {
            get => _filtroEstado;
            set
            {
                if (_filtroEstado == value) return;
                _filtroEstado = value;
                OnPropertyChanged();
                ReiniciarPagina();
                _ = CargarPaginaAsync();
            }
        }

        public bool PuedeVerFiltroEstado => PuedeEliminar;

        public List<string> EstadosDisponibles { get; } = new List<string> { "Todos", "Activos", "Inactivos" };

        public new void NotificarPermisos()
        {
            base.NotificarPermisos();
            OnPropertyChanged(nameof(PuedeVerFiltroEstado));
        }

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
            catch (Exception ex)
            {
                (App.ServiceProvider?.GetService(typeof(IErrorNotifier)) as IErrorNotifier)?.Notify(ex, "ModeloViewModel");
                // Silenciar error
            }
        }

        /// <summary>
        /// Carga la primera pagina del listado (usada al abrir la pantalla).
        /// </summary>
        public async Task LoadAsync()
        {
            await CargarMarcasAsync();
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
                var estado = PuedeVerFiltroEstado ? FiltroEstado : "Activos";
                var (items, total) = await _listarModelosHandler.HandlePaginadoAsync(
                    TextoBusqueda.Trim(),
                    estado,
                    PaginaActual,
                    TamanoPagina);

                Modelos.Clear();
                foreach (var modelo in items)
                {
                    Modelos.Add(modelo);
                }

                ActualizarTotales(total);
            }
            catch (Exception ex)
            {
                (App.ServiceProvider?.GetService(typeof(IErrorNotifier)) as IErrorNotifier)?.Notify(ex, "ModeloViewModel");
                // Silenciar error en carga inicial
                Modelos.Clear();
                ActualizarTotales(0);
            }
            finally
            {
                IsLoading = false;
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
                NombreModelo = NombreModelo.Trim(),
                Activo = modeloSeleccionado?.Activo ?? true
            };

            try
            {
                if (modelo.Id == 0)
                {
                    await _registrarModeloHandler.HandleAsync(modelo);
                }
                else
                {
                    await _modificarModeloHandler.HandleAsync(modelo);
                }

                // Recargar la pagina actual para que el listado refleje el alta o la edicion.
                await CargarPaginaAsync();
                return true;
            }
            catch (Exception ex)
            {
                (App.ServiceProvider?.GetService(typeof(IErrorNotifier)) as IErrorNotifier)?.Notify(ex, "ModeloViewModel");
                return false;
            }
        }

        public async Task<bool> EliminarModeloAsync(int id)
        {
            try
            {
                await _eliminarModeloHandler.HandleAsync(id);
                await CargarPaginaAsync();
                return true;
            }
            catch (Exception ex)
            {
                (App.ServiceProvider?.GetService(typeof(IErrorNotifier)) as IErrorNotifier)?.Notify(ex, "ModeloViewModel");
                return false;
            }
        }

        public async Task<bool> ReactivarModeloAsync(int id)
        {
            try
            {
                await _eliminarModeloHandler.ReactivarAsync(id);
                await CargarPaginaAsync();
                return true;
            }
            catch (Exception ex)
            {
                (App.ServiceProvider?.GetService(typeof(IErrorNotifier)) as IErrorNotifier)?.Notify(ex, "ModeloViewModel");
                return false;
            }
        }
    }
}
