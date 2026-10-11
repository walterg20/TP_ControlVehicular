using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Services;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly ObtenerDashboardHandler _obtenerDashboardHandler;

        private int _vehiculosActivosCount;
        public int VehiculosActivosCount
        {
            get => _vehiculosActivosCount;
            set => SetProperty(ref _vehiculosActivosCount, value);
        }

        private int _enProcesoCount;
        public int EnProcesoCount
        {
            get => _enProcesoCount;
            set => SetProperty(ref _enProcesoCount, value);
        }

        private int _entregadasHoyCount;
        public int EntregadasHoyCount
        {
            get => _entregadasHoyCount;
            set => SetProperty(ref _entregadasHoyCount, value);
        }

        private decimal _ingresosMesTotal;
        public decimal IngresosMesTotal
        {
            get => _ingresosMesTotal;
            set => SetProperty(ref _ingresosMesTotal, value);
        }

        private bool _mostrarIngresos;
        public bool MostrarIngresos
        {
            get => _mostrarIngresos;
            set
            {
                if (SetProperty(ref _mostrarIngresos, value))
                {
                    OnPropertyChanged(nameof(TarjetasResumenVisible));
                }
            }
        }

        // Numero de tarjetas visibles en la fila de resumen (3 o 4 segun el rol).
        public int TarjetasResumenVisible => MostrarIngresos ? 4 : 3;

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ObservableCollection<VehiculoDto> VehiculosEnTaller { get; } = new();

        public ICommand CargarDashboardCommand { get; }

        public DashboardViewModel(ObtenerDashboardHandler obtenerDashboardHandler)
        {
            _obtenerDashboardHandler = obtenerDashboardHandler;
            
            CargarDashboardCommand = new RelayCommand(LoadAsync);
        }

        public async Task LoadAsync()
        {
            IsLoading = true;
            try
            {
                var result = await _obtenerDashboardHandler.HandleAsync();
                VehiculosActivosCount = result.VehiculosActivosCount;
                EnProcesoCount = result.EnProcesoCount;
                EntregadasHoyCount = result.EntregadasHoyCount;
                IngresosMesTotal = result.IngresosMesTotal;
                MostrarIngresos = result.MostrarIngresos;

                VehiculosEnTaller.Clear();
                foreach (var v in result.VehiculosEnTaller)
                {
                    VehiculosEnTaller.Add(v);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
