using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Pantalla.OrdenServicio
{
    public partial class CtlOrdenServicioForm : UserControl
    {
        private readonly bool _esModificacion;

        public CtlOrdenServicioForm()
        {
            InitializeComponent();
            _esModificacion = false;
            lblTitulo.Text = "Registrar Nueva Orden";
            ConfigurarViewModel(null);
            ConfigurarValidacionAlPerderFoco();
            this.Unloaded += (s, e) => {
                if (DataContext is OrdenServicioViewModel vm)
                {
                    vm.RegistrationFailed -= Vm_RegistrationFailed;
                }
            };
        }

        public CtlOrdenServicioForm(RegistroServicioDto orden)
        {
            InitializeComponent();
            _esModificacion = true;
            lblTitulo.Text = $"Modificar Orden #{orden.Id}";
            ConfigurarViewModel(orden);
            ConfigurarValidacionAlPerderFoco();
            this.Unloaded += (s, e) => {
                if (DataContext is OrdenServicioViewModel vm)
                {
                    vm.RegistrationFailed -= Vm_RegistrationFailed;
                }
            };
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
                    
                    if (vm.VehiculosDisponibles.Count == 0 || vm.TalleresDisponibles.Count == 0 || vm.ServiciosDisponibles.Count == 0)
                        await vm.LoadCombosAsync();

                    vm.DetallesOrdenActual.Clear();

                    if (_esModificacion && orden != null)
                    {
                        vm.OrdenSeleccionada = orden;
                        vm.SeleccionarClientePorVehiculo(orden.VehiculoId);
                        vm.VehiculoId = orden.VehiculoId;
                        vm.TallerId = orden.TallerId;
                        vm.UsuarioId = orden.UsuarioId;
                        vm.Fecha = orden.Fecha;
                        vm.KmIngreso = orden.KmIngreso;
                        vm.Estado = orden.Estado;

                        if (orden.Detalles != null)
                        {
                            foreach (var det in orden.Detalles)
                            {
                                vm.DetallesOrdenActual.Add(new DetalleServicioDto
                                {
                                    Id = det.Id,
                                    RegistroServicioId = det.RegistroServicioId,
                                    ServicioId = det.ServicioId,
                                    ServicioNombre = det.ServicioNombre,
                                    UsuarioId = det.UsuarioId,
                                    Realizado = det.Realizado,
                                    Observaciones = det.Observaciones ?? string.Empty,
                                    Cantidad = det.Cantidad,
                                    Precio = det.Precio,
                                    Origen = det.Origen,
                                    Estado = det.Estado
                                });
                            }
                        }
                    }
                    else
                    {
                        vm.OrdenSeleccionada = null;
                        vm.VehiculoId = 0;
                        vm.TallerId = 0;
                        vm.KmIngreso = 0;
                        vm.Fecha = DateTime.Now;
                        vm.Estado = "Pendiente";
                        
                        if (Application.Current.MainWindow is MainWindow mainWin && mainWin.UsuarioSesionActual != null)
                        {
                            vm.UsuarioId = mainWin.UsuarioSesionActual.IdUsuario;
                        }
                    }

                    vm.ClearAllErrors();
                    vm.RegistrationFailed -= Vm_RegistrationFailed;
                    vm.RegistrationFailed += Vm_RegistrationFailed;

                    // Restricciones por Rol
                    if (Application.Current.MainWindow is MainWindow mWin && mWin.UsuarioSesionActual != null)
                    {
                        bool esMecanico = mWin.UsuarioSesionActual.RolNombre?.IndexOf("Mec", StringComparison.OrdinalIgnoreCase) >= 0 || mWin.UsuarioSesionActual.IdRol == 3;
                        if (esMecanico)
                        {
                            cmbClientes.IsEnabled = false;
                            btnNuevoCliente.IsEnabled = false;
                            btnNuevoVehiculo.IsEnabled = false;
                            btnNuevoServicio.IsEnabled = false;
                            cmbVehiculos.IsEnabled = false;
                            cmbTalleres.IsEnabled = false;
                            cmbEstado.IsEnabled = false;
                            txtKm.IsEnabled = false;
                        }
                    }
                }
            }
            catch
            {
                // Ignorar en tiempo de diseño o si falla DI
            }
        }

        private void BtnAgregarItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OrdenServicioViewModel vm)
            {
                if (vm.ServicioBusquedaSeleccionado != null)
                {
                    if (vm.DetallesOrdenActual.Any(d => d.ServicioId == vm.ServicioBusquedaSeleccionado.IdServicio))
                    {
                        Compartido.FrmConfirmacion.MostrarAviso("El servicio ya se encuentra en el checklist.", "Aviso", Window.GetWindow(this));
                        return;
                    }

                    int idMecanicoAuto = 0;
                    if (Application.Current.MainWindow is MainWindow mWin && mWin.UsuarioSesionActual != null)
                    {
                        bool esMecanico = mWin.UsuarioSesionActual.RolNombre?.IndexOf("Mec", StringComparison.OrdinalIgnoreCase) >= 0 || mWin.UsuarioSesionActual.IdRol == 3;
                        if (esMecanico)
                            idMecanicoAuto = mWin.UsuarioSesionActual.IdUsuario;
                    }

                    vm.DetallesOrdenActual.Add(new DetalleServicioDto
                    {
                        ServicioId = vm.ServicioBusquedaSeleccionado.IdServicio,
                        ServicioNombre = vm.ServicioBusquedaSeleccionado.Nombre,
                        UsuarioId = idMecanicoAuto,
                        Realizado = true,
                        Observaciones = string.Empty,
                        Cantidad = 1,
                        Precio = vm.ServicioBusquedaSeleccionado.Precio,
                        Estado = "Realizado",
                        Origen = "Manual"
                    });
                    vm.ServicioBusquedaSeleccionado = null;
                }
                else
                {
                    Compartido.FrmConfirmacion.MostrarAviso("Seleccione un servicio para agregar.", "Aviso", Window.GetWindow(this));
                }
            }
        }

        private async void BtnNuevoCliente_Click(object sender, RoutedEventArgs e)
        {
            var frm = new Cliente.FrmCliente();
            frm.Owner = Window.GetWindow(this);
            if (frm.ShowDialog() == true)
            {
                if (DataContext is OrdenServicioViewModel vm)
                {
                    await vm.LoadCombosAsync();
                }
            }
        }

        private async void BtnNuevoVehiculo_Click(object sender, RoutedEventArgs e)
        {
            var frm = new Vehiculo.FrmVehiculo();
            frm.Owner = Window.GetWindow(this);
            if (frm.ShowDialog() == true)
            {
                if (DataContext is OrdenServicioViewModel vm)
                {
                    await vm.LoadCombosAsync();
                }
            }
        }

                private async void BtnNuevoServicio_Click(object sender, RoutedEventArgs e)
        {
            var frm = new Servicio.FrmServicio();
            frm.Owner = Window.GetWindow(this);
            if (frm.ShowDialog() == true)
            {
                if (DataContext is OrdenServicioViewModel vm)
                {
                    await vm.LoadCombosAsync();
                }
            }
        }

        private void BtnQuitarItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is DetalleServicioDto detalle)
            {
                if (DataContext is OrdenServicioViewModel vm)
                {
                    vm.DetallesOrdenActual.Remove(detalle);
                }
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
                if (vm.DetallesOrdenActual.Count == 0)
                {
                    Compartido.FrmConfirmacion.MostrarAviso("Debe agregar al menos un servicio a la orden antes de guardar.", "Validación", Window.GetWindow(this));
                    return;
                }

                var ok = await vm.GuardarOrdenAsync();
                if (ok)
                {
                    VolverAlListado();
                }
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            VolverAlListado();
        }

        private void VolverAlListado()
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                // Instantiate the list control, which will load the updated data upon loaded
                mainWindow.AgregarPagina(new CtlOrdenServicio());
            }
        }
    
        private void dgChecklist_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (e.Column.Header?.ToString() == "Mec�nico")
            {
                if (Application.Current.MainWindow is MainWindow mWin && mWin.UsuarioSesionActual != null)
                {
                    bool esMecanico = mWin.UsuarioSesionActual.RolNombre?.IndexOf("Mec", StringComparison.OrdinalIgnoreCase) >= 0 || mWin.UsuarioSesionActual.IdRol == 3;
                    if (esMecanico)
                    {
                        e.Cancel = true;
                    }
                }
            }
        }
    }
}







