using System.Windows;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Taller
{
    public partial class FrmTaller : Window
    {
        private readonly bool _esModificacion;

        // Constructor para Nuevo
        public FrmTaller()
        {
            InitializeComponent();
            _esModificacion = false;
            this.Title = "Registrar Nuevo Taller";
            ConfigurarViewModel(null);
        }

        // Constructor para Modificar
        public FrmTaller(TallerDto taller)
        {
            InitializeComponent();
            _esModificacion = true;
            this.Title = "Modificar Taller";
            ConfigurarViewModel(taller);
        }

        private void ConfigurarViewModel(TallerDto? taller)
        {
            try
            {
                if (App.ServiceProvider.GetService(typeof(TallerViewModel)) is TallerViewModel vm)
                {
                    DataContext = vm;
                    if (_esModificacion && taller != null)
                    {
                        vm.TallerSeleccionado = taller;
                        vm.Nombre = taller.Nombre;
                        vm.Direccion = taller.Direccion;
                        vm.Telefono = taller.Telefono;
                        vm.Activo = taller.Activo;
                    }
                    vm.RegistrationCompleted += (s, ok) =>
                    {
                        // Cerrar la ventana en el hilo de la UI
                        Dispatcher.Invoke(() => this.DialogResult = ok);
                    };
                }
            }
            catch
            {
                // Ignorar si DI no está disponible
            }
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TallerViewModel vm)
            {
                var ok = await vm.GuardarTallerAsync();
                this.DialogResult = ok;
            }
            else
            {
                this.DialogResult = false;
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false; // Cierra sin hacer nada
        }
    }
}