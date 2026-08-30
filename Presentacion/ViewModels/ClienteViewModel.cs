
using System.Collections.ObjectModel;
using System.Windows.Input;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Services;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public class ClienteViewModel: BaseViewModel
    {
        private readonly RegistrarClienteHandler _registrarClienteHandler;

        public ClienteViewModel(RegistrarClienteHandler registrarClienteHandler)
        {
            _registrarClienteHandler = registrarClienteHandler;
            Clientes = new ObservableCollection<ClienteDto>();
            RegistrarCommand = new RelayCommand(async () => await RegistrarCliente());
        }

        public ObservableCollection<ClienteDto> Clientes { get; set; }

        private string _nombre;
        public string Nombre
        {
            get => _nombre;
            set { _nombre = value; OnPropertyChanged(); }
        }

        private string _apellido;
        public string Apellido
        {
            get => _apellido;
            set { _apellido = value; OnPropertyChanged(); }
        }

        private string _dni;
        public string Dni
        {
            get => _dni;
            set { _dni = value; OnPropertyChanged(); }
        }

        public ICommand RegistrarCommand { get; }

        private async Task RegistrarCliente()
        {
            var cliente = new TP_ControlVehicular.Entidad.Cliente
            {
                Nombre = Nombre,
                Apellido = Apellido,
                Dni = Dni,
                Activo = true
            };

            var dto = await _registrarClienteHandler.HandleAsync(cliente);
            Clientes.Add(dto);
        }
    }
}
