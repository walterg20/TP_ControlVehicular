using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Pantalla.OrdenServicio
{
    public partial class FrmOrdenServicio : Window
    {
        private readonly bool _esModificacion;

        public FrmOrdenServicio()
        {
            InitializeComponent();
            _esModificacion = false;
            lblTitulo.Text = "Registrar Nueva Orden";
            ConfigurarViewModel(null);
            ConfigurarValidacionAlPerderFoco();
        }

        public FrmOrdenServicio(RegistroServicioDto orden)
        {
            InitializeComponent();
            _esModificacion = true;
            lblTitulo.Text = $"Modificar Orden #{orden.Id}";
            ConfigurarViewModel(orden);
            ConfigurarValidacionAlPerderFoco();
        }

        private void ConfigurarValidacionAlPerderFoco()
        {
            AddHandler(UIElement.LostFocusEvent, new RoutedEventHandler((s, e) =>
            {
                if (e.OriginalSource is FrameworkElement element && DataContext is BaseViewModel vm)
                {
                    DependencyProperty? dp = null;
                    if (element is TextBox) dp = TextBox.TextProperty;
                    else if (element is ComboBox) dp = ComboBox.SelectedValueProperty; // O SelectedItemProperty según bind

                    if (dp != null)
                    {
                        var binding = BindingOperations.GetBinding(element, dp);
                        if(binding == null && element is ComboBox)
                            binding = BindingOperations.GetBinding(element, ComboBox.SelectedItemProperty);

                        if (binding != null && binding.Path != null && !string.IsNullOrEmpty(binding.Path.Path))
                        {
                            var be = element.GetBindingExpression(binding.Path.Path == "Estado" ? ComboBox.SelectedItemProperty : dp);
                            be?.UpdateSource();
                            vm.ValidateProperty(binding.Path.Path);
                        }
                    }
                }
            }));
        }

        private async void ConfigurarViewModel(RegistroServicioDto? orden)
        {
            try
            {
                if (App.ServiceProvider?.GetService(typeof(OrdenServicioViewModel)) is OrdenServicioViewModel vm)
                {
                    DataContext = vm;
                    
                    if (vm.VehiculosDisponibles.Count == 0 || vm.TalleresDisponibles.Count == 0)
                        await vm.LoadCombosAsync();

                    if (_esModificacion && orden != null)
                    {
                        vm.OrdenSeleccionada = orden;
                        vm.VehiculoId = orden.VehiculoId;
                        vm.TallerId = orden.TallerId;
                        vm.UsuarioId = orden.UsuarioId;
                        vm.Fecha = orden.Fecha;
                        vm.KmIngreso = orden.KmIngreso;
                        vm.Estado = orden.Estado;
                    }
                    else
                    {
                        vm.OrdenSeleccionada = null;
                        vm.VehiculoId = 0;
                        vm.TallerId = 0;
                        vm.KmIngreso = 0;
                        vm.Fecha = DateTime.Now;
                        vm.Estado = "Pendiente";
                        
                        // Si hay un usuario en sesión, sería ideal setear su ID, pero
                        // como requeriría conocerlo, asumimos que se seteará 1 por defecto 
                        // si no se pasa, o se puede setear acá accediendo a MainWindow.
                        if (Application.Current.MainWindow is MainWindow mainWin && mainWin.UsuarioSesionActual != null)
                        {
                            vm.UsuarioId = mainWin.UsuarioSesionActual.IdUsuario;
                        }
                    }

                    vm.ClearAllErrors();
                    vm.RegistrationFailed -= Vm_RegistrationFailed;
                    vm.RegistrationFailed += Vm_RegistrationFailed;
                }
            }
            catch
            {
                // Ignorar en tiempo de diseño o si falla DI
            }
        }

        private void Vm_RegistrationFailed(object? sender, string errorMessage)
        {
            Compartido.FrmConfirmacion.MostrarAviso(errorMessage, "Error", Window.GetWindow(this));
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OrdenServicioViewModel vm)
            {
                if (!vm.ValidateAll())
                {
                    Compartido.FrmConfirmacion.MostrarAviso("Por favor, revise los campos marcados en rojo.", "Validación", Window.GetWindow(this));
                    return;
                }

                var ok = await vm.GuardarOrdenAsync();
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

        protected override void OnClosed(EventArgs e)
        {
            if (DataContext is OrdenServicioViewModel vm)
            {
                vm.RegistrationFailed -= Vm_RegistrationFailed;
            }
            base.OnClosed(e);
        }
    }
}
