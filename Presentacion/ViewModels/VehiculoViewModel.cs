using System.Collections.ObjectModel;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Services;


namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public class VehiculoViewModel
    {
        private readonly RegistrarVehiculoHandler _registrarVehiculoHandler;
        private readonly ListarVehiculosPorClienteHandler _listarVehiculosHandler;

        public VehiculoViewModel(
            RegistrarVehiculoHandler registrarVehiculoHandler,
            ListarVehiculosPorClienteHandler listarVehiculosHandler)
        {
            _registrarVehiculoHandler = registrarVehiculoHandler;
            _listarVehiculosHandler = listarVehiculosHandler;

            Vehiculos = new ObservableCollection<VehiculoDto>();
            RegistrarCommand = new RelayCommand(async () => await RegistrarVehiculo());
            ListarCommand = new RelayCommand(async () => await ListarVehiculos());
        }

        public ObservableCollection<VehiculoDto> Vehiculos { get; set; }

        public int IdCliente { get; set; }
        public string Patente { get; set; } = string.Empty;
        public int Anio { get; set; }
        public int KmActual { get; set; }
        public int IdModelo { get; set; }

        public ICommand RegistrarCommand { get; }
        public ICommand ListarCommand { get; }

        private async Task RegistrarVehiculo()
        {
            var vehiculo = new TP_ControlVehicular.Entidad.Vehiculo
            {
                ClienteId = IdCliente,
                Patente = Patente,
                Anio = Anio,
                KmActual = KmActual,
                ModeloId = IdModelo
            };

            var dto = await _registrarVehiculoHandler.HandleAsync(vehiculo);
            Vehiculos.Add(dto);
        }

        private async Task ListarVehiculos()
        {
            Vehiculos.Clear();
            var lista = await _listarVehiculosHandler.HandleAsync(IdCliente);
            foreach (var v in lista)
                Vehiculos.Add(v);
        }
    }
}
