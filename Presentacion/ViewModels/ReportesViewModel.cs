using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using TP_ControlVehicular.Negocio.DTOs.Reportes;
using TP_ControlVehicular.Negocio.Handlers.Reportes;
using System.Collections.Generic;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public class ReportesViewModel : BaseViewModel
    {
        private readonly ObtenerHistorialVehiculoHandler _historialHandler;
        private readonly ObtenerVehiculosPorClienteHandler _vehiculosClienteHandler;

        public ReportesViewModel(
            ObtenerHistorialVehiculoHandler historialHandler,
            ObtenerVehiculosPorClienteHandler vehiculosClienteHandler)
        {
            _historialHandler = historialHandler;
            _vehiculosClienteHandler = vehiculosClienteHandler;

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

        private async Task CargarHistorialAsync()
        {
            try
            {
                ErrorMessage = string.Empty;
                var resultados = await _historialHandler.HandleAsync(null, Patente);
                ResultadosHistorial = new ObservableCollection<HistorialVehiculoDto>(resultados);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al cargar el historial: {ex.Message}";
            }
        }

        private async Task CargarVehiculosClienteAsync()
        {
            try
            {
                ErrorMessage = string.Empty;
                var resultados = await _vehiculosClienteHandler.HandleAsync(null, DNI);
                ResultadosVehiculos = new ObservableCollection<VehiculoClienteDto>(resultados);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al cargar los vehículos: {ex.Message}";
            }
        }
    }
}
