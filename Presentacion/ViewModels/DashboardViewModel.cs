using System.Collections.ObjectModel;
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
