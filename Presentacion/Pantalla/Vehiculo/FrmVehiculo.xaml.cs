using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Presentacion.Marca;
using TP_ControlVehicular.Presentacion.Modelo;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Vehiculo
{
    public partial class FrmVehiculo : Window
    {
        private readonly bool _esModificacion;

        public FrmVehiculo()
        {
            InitializeComponent();
            _esModificacion = false;
            this.Title = "Registrar Nuevo Vehículo";
            ConfigurarViewModel(null);
            Loaded += (object sender, System.Windows.RoutedEventArgs e) => txtPatente.Focus();
            ConfigurarValidacionAlPerderFoco();
        }

        public FrmVehiculo(VehiculoDto vehiculo)
        {
            InitializeComponent();
            _esModificacion = true;
            this.Title = "Modificar Vehículo";
            ConfigurarViewModel(vehiculo);
            Loaded += (object sender, System.Windows.RoutedEventArgs e) => txtPatente.Focus();
            ConfigurarValidacionAlPerderFoco();
        }

        private void ConfigurarValidacionAlPerderFoco()
        {
            AddHandler(UIElement.LostFocusEvent, new RoutedEventHandler((s, e) =>
            {
                if (e.OriginalSource is FrameworkElement element && DataContext is BaseViewModel vm)
                {
                    DependencyProperty? dp = null;
                    if (element is TextBox)
                        dp = TextBox.TextProperty;
                    else if (element is ComboBox)
                        dp = ComboBox.SelectedValueProperty;
                    else if (element is DatePicker)
                        dp = DatePicker.SelectedDateProperty;

                    if (dp != null)
                    {
                        var binding = BindingOperations.GetBinding(element, dp);
                        var be = element.GetBindingExpression(dp);
                        if (binding != null && binding.Path != null && !string.IsNullOrEmpty(binding.Path.Path))
                        {
                            be?.UpdateSource();
                            vm.ValidateProperty(binding.Path.Path);
                        }
                    }
                }
            }));
        }

        private async void ConfigurarViewModel(VehiculoDto? vehiculo)
        {
            try
            {
                if (App.ServiceProvider.GetService(typeof(VehiculoViewModel)) is VehiculoViewModel vmVehiculo)
                {
                    DataContext = vmVehiculo;
                    await vmVehiculo.CargarCombosAsync(vehiculo?.IdModelo);

                    if (_esModificacion && vehiculo != null)
                    {
                        vmVehiculo.VehiculoSeleccionado = vehiculo;
                        vmVehiculo.Patente = vehiculo.Patente;
                        vmVehiculo.Anio = vehiculo.Anio;
                        vmVehiculo.KmActual = vehiculo.KmActual;
                        vmVehiculo.IdCliente = vehiculo.IdCliente;
                    }
                    else
                    {
                        vmVehiculo.VehiculoSeleccionado = null;
                        vmVehiculo.Patente = string.Empty;
                        vmVehiculo.Anio = DateTime.Now.Year;
                        vmVehiculo.KmActual = 0;
                        vmVehiculo.IdCliente = 0;
                        vmVehiculo.IdMarca = 0;
                        vmVehiculo.IdModelo = 0;
                    }

                    vmVehiculo.ClearAllErrors();
                }
            }
            catch
            {
                // Silenciar si DI no está disponible
            }
        }

        private async void BtnAgregarMarca_Click(object sender, RoutedEventArgs e)
        {
            var frmMarca = new FrmMarca();
            frmMarca.Owner = this;
            var ok = frmMarca.ShowDialog();
            if (ok == true && DataContext is VehiculoViewModel vmVehiculo)
            {
                await vmVehiculo.CargarMarcasAsync();
            }
        }

        private async void BtnAgregarModelo_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is VehiculoViewModel vmVehiculo)
            {
                var frmModelo = vmVehiculo.IdMarca > 0 ? new FrmModelo(vmVehiculo.IdMarca) : new FrmModelo();
                frmModelo.Owner = this;
                var ok = frmModelo.ShowDialog();
                if (ok == true)
                {
                    await vmVehiculo.CargarModelosAsync();
                }
            }
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is VehiculoViewModel vmVehiculo)
            {
                var ok = await vmVehiculo.GuardarVehiculoAsync();
                if (ok)
                {
                    this.DialogResult = true;
                }
            }
            else
            {
                this.DialogResult = false;
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
