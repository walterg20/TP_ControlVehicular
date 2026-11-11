// walterg20
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs.Reportes;
using TP_ControlVehicular.Negocio.Handlers.Reportes;
using System.Collections.Generic;
using TP_ControlVehicular.Presentacion.Helpers;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    /// <summary>
    /// Reportes de recepcion con paginacion en la base de datos.
    /// La pantalla tiene dos grillas independientes: el historial del vehiculo (que usa el
    /// estado de paginacion de esta clase) y los vehiculos del cliente (que usa
    /// <see cref="PaginacionVehiculos"/>).
    /// </summary>
    public class ReportesViewModel : BasePaginableViewModel
    {
        private readonly ObtenerHistorialVehiculoHandler _historialHandler;
        private readonly ObtenerVehiculosPorClienteHandler _vehiculosClienteHandler;
        private readonly Debouncer _debouncer;
        private readonly Debouncer _debouncerVehiculos;

        /// <summary>Estado de paginacion de la grilla de vehiculos por cliente.</summary>
        public PaginacionViewModel PaginacionVehiculos { get; }

        public ReportesViewModel(
            ObtenerHistorialVehiculoHandler historialHandler,
            ObtenerVehiculosPorClienteHandler vehiculosClienteHandler)
        {
            _historialHandler = historialHandler;
            _vehiculosClienteHandler = vehiculosClienteHandler;
            _debouncer = new Debouncer();
            _debouncerVehiculos = new Debouncer();

            PaginacionVehiculos = new PaginacionViewModel(
                async (pagina, tamano) => await CargarVehiculosPaginaAsync(pagina, tamano));

            GenerarHistorialCommand = new RelayCommand(CargarHistorialAsync);
            GenerarVehiculosClienteCommand = new RelayCommand(CargarVehiculosClienteAsync);
        }

        private string _patente = string.Empty;
        public string Patente
        {
            get => _patente;
            set => SetProperty(ref _patente, value);
        }

        private string _dni = string.Empty;
        public string DNI
        {
            get => _dni;
            set => SetProperty(ref _dni, value);
        }

        private ObservableCollection<HistorialVehiculoDto> _resultadosHistorial = new();
        public ObservableCollection<HistorialVehiculoDto> ResultadosHistorial
        {
            get => _resultadosHistorial;
            set => SetProperty(ref _resultadosHistorial, value);
        }

        private ObservableCollection<VehiculoClienteDto> _resultadosVehiculos = new();
        public ObservableCollection<VehiculoClienteDto> ResultadosVehiculos
        {
            get => _resultadosVehiculos;
            set => SetProperty(ref _resultadosVehiculos, value);
        }

        public ICommand GenerarHistorialCommand { get; }
        public ICommand GenerarVehiculosClienteCommand { get; }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        /// <summary>
        /// Genera el historial del vehiculo indicado, desde la primera pagina.
        /// </summary>
        private async Task CargarHistorialAsync()
        {
            await BuscarAsync();
        }

        /// <summary>
        /// Genera el reporte de vehiculos del cliente indicado, desde la primera pagina.
        /// </summary>
        private async Task CargarVehiculosClienteAsync()
        {
            ErrorMessage = string.Empty;
            PaginacionVehiculos.ReiniciarPagina();
            await PaginacionVehiculos.RecargarAsync();
        }

        /// <summary>
        /// Programa la carga del historial con un retardo breve.
        /// </summary>
        public void ProgramarBusqueda() => _debouncer.Debounce(BuscarAsync);

        /// <summary>Cancela una busqueda programada que todavia no se ejecuto.</summary>
        public void CancelarBusquedaProgramada() => _debouncer.Cancel();

        /// <summary>
        /// Programa la carga de los vehiculos del cliente con un retardo breve.
        /// </summary>
        public void ProgramarBusquedaVehiculos() =>
            _debouncerVehiculos.Debounce(async () =>
            {
                PaginacionVehiculos.ReiniciarPagina();
                await PaginacionVehiculos.RecargarAsync();
            });

        /// <summary>
        /// Aplica los filtros actuales y recarga el historial desde la primera pagina.
        /// </summary>
        public async Task BuscarAsync()
        {
            ErrorMessage = string.Empty;
            ReiniciarPagina();
            await CargarPaginaAsync();
        }

        /// <summary>
        /// Carga la pagina actual del historial de vehiculo.
        /// </summary>
        protected override async Task CargarPaginaAsync()
        {
            try
            {
                var (items, total) = await _historialHandler.HandlePaginadoAsync(
                    null,
                    string.IsNullOrWhiteSpace(Patente) ? null : Patente.Trim(),
                    PaginaActual,
                    TamanoPagina);

                ResultadosHistorial = new ObservableCollection<HistorialVehiculoDto>(items);
                ActualizarTotales(total);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al cargar el historial: {ex.Message}";
                ResultadosHistorial = new ObservableCollection<HistorialVehiculoDto>();
                ActualizarTotales(0);
            }
        }

        /// <summary>
        /// Carga la pagina actual de los vehiculos del cliente.
        /// </summary>
        private async Task CargarVehiculosPaginaAsync(int pagina, int tamanoPagina)
        {
            try
            {
                var (items, total) = await _vehiculosClienteHandler.HandlePaginadoAsync(
                    null,
                    string.IsNullOrWhiteSpace(DNI) ? null : DNI.Trim(),
                    pagina,
                    tamanoPagina);

                ResultadosVehiculos = new ObservableCollection<VehiculoClienteDto>(items);
                PaginacionVehiculos.ActualizarTotales(total);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al cargar los vehículos: {ex.Message}";
                ResultadosVehiculos = new ObservableCollection<VehiculoClienteDto>();
                PaginacionVehiculos.ActualizarTotales(0);
            }
        }
    }
}
