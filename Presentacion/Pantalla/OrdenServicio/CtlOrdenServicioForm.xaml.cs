// walterg20
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Presentacion.Helpers;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Pantalla.OrdenServicio
{
    public partial class CtlOrdenServicioForm : UserControl
    {
        private readonly bool _esModificacion;
        private readonly Debouncer _debouncerClientes = new(TimeSpan.FromMilliseconds(250));
        private readonly Debouncer _debouncerServicios = new(TimeSpan.FromMilliseconds(250));
        private bool _omitirFiltroClientes = false;
        private bool _omitirFiltroServicios = false;

        public CtlOrdenServicioForm()
        {
            InitializeComponent();
            _origen = "General";
            _esModificacion = false;
            lblTitulo.Text = "Registrar Nueva Orden";
            ConfigurarViewModel(null);
            ConfigurarValidacionAlPerderFoco();
            ConfigurarBusquedaCombos();
            this.Unloaded += (s, e) =>
            {
                _debouncerClientes.Cancel();
                _debouncerServicios.Cancel();
                if (DataContext is OrdenServicioViewModel vmOrdenServicioForm)
                {
                    vmOrdenServicioForm.RegistrationFailed -= Vm_RegistrationFailed;
                }
            };
        }

        private readonly string _origen;

        public CtlOrdenServicioForm(RegistroServicioDto orden, string origen = "General")
        {
            InitializeComponent();
            _origen = origen;
            _esModificacion = true;
            lblTitulo.Text = $"Modificar Orden #{orden.Id}";
            ConfigurarViewModel(orden);
            ConfigurarValidacionAlPerderFoco();
            ConfigurarBusquedaCombos();
            this.Unloaded += (s, e) =>
            {
                _debouncerClientes.Cancel();
                _debouncerServicios.Cancel();
                if (DataContext is OrdenServicioViewModel vmOrdenServicioForm)
                {
                    vmOrdenServicioForm.RegistrationFailed -= Vm_RegistrationFailed;
                }
            };
        }

        private void ConfigurarValidacionAlPerderFoco()
        {
            AddHandler(UIElement.LostFocusEvent, new RoutedEventHandler((s, e) =>
            {
                if (e.OriginalSource is FrameworkElement element && DataContext is BaseViewModel vmOrdenServicioForm)
                {
                    DependencyProperty? dp = null;
                    if (element is TextBox) dp = TextBox.TextProperty;
                    else if (element is ComboBox) dp = ComboBox.SelectedValueProperty; // O SelectedItemProperty según bind

                    if (dp != null)
                    {
                        var binding = BindingOperations.GetBinding(element, dp);
                        if (binding == null && element is ComboBox)
                            binding = BindingOperations.GetBinding(element, ComboBox.SelectedItemProperty);

                        if (binding != null && binding.Path != null && !string.IsNullOrEmpty(binding.Path.Path))
                        {
                            var be = element.GetBindingExpression(binding.Path.Path == "Estado" ? ComboBox.SelectedItemProperty : dp);
                            be?.UpdateSource();
                            vmOrdenServicioForm.ValidateProperty(binding.Path.Path);
                        }
                    }
                }
            }));
        }

        private async void ConfigurarViewModel(RegistroServicioDto? orden)
        {
            try
            {
                if (App.ServiceProvider?.GetService(typeof(OrdenServicioViewModel)) is OrdenServicioViewModel vmOrdenServicioForm)
                {
                    DataContext = vmOrdenServicioForm;

                    if (vmOrdenServicioForm.VehiculosDisponibles.Count == 0 || vmOrdenServicioForm.TalleresDisponibles.Count == 0 || vmOrdenServicioForm.ServiciosDisponibles.Count == 0)
                        await vmOrdenServicioForm.LoadCombosAsync();

                    vmOrdenServicioForm.DetallesOrdenActual.Clear();

                    if (_esModificacion && orden != null)
                    {
                        vmOrdenServicioForm.OrdenSeleccionada = orden;
                        vmOrdenServicioForm.SeleccionarClientePorVehiculo(orden.VehiculoId);
                        vmOrdenServicioForm.VehiculoId = orden.VehiculoId;
                        vmOrdenServicioForm.TallerId = orden.TallerId;
                        vmOrdenServicioForm.UsuarioId = orden.UsuarioId;
                        vmOrdenServicioForm.Fecha = orden.Fecha;
                        vmOrdenServicioForm.KmIngreso = orden.KmIngreso;
                        vmOrdenServicioForm.Estado = orden.Estado;

                        if (orden.Detalles != null)
                        {
                            foreach (var det in orden.Detalles)
                            {
                                vmOrdenServicioForm.DetallesOrdenActual.Add(new DetalleServicioDto
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
                                    Estado = det.Estado,
                                    OrdenEjecucion = det.OrdenEjecucion
                                });
                            }
                        }
                    }
                    else
                    {
                        vmOrdenServicioForm.OrdenSeleccionada = null;
                        vmOrdenServicioForm.VehiculoId = 0;
                        vmOrdenServicioForm.TallerId = 0;
                        vmOrdenServicioForm.KmIngreso = 0;
                        vmOrdenServicioForm.Fecha = DateTime.Now;
                        vmOrdenServicioForm.Estado = "Abierta";

                        if (Application.Current.MainWindow is MainWindow mainWin && mainWin.UsuarioSesionActual != null)
                        {
                            vmOrdenServicioForm.UsuarioId = mainWin.UsuarioSesionActual.IdUsuario;
                        }
                    }

                    vmOrdenServicioForm.ClearAllErrors();
                    vmOrdenServicioForm.RegistrationFailed -= Vm_RegistrationFailed;
                    vmOrdenServicioForm.RegistrationFailed += Vm_RegistrationFailed;

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

                            var view = System.Windows.Data.CollectionViewSource.GetDefaultView(vmOrdenServicioForm.DetallesOrdenActual);
                            if (view != null)
                            {
                                int currentUserId = mWin.UsuarioSesionActual.IdUsuario;
                                view.Filter = item =>
                                {
                                    if (item is TP_ControlVehicular.Negocio.DTOs.DetalleServicioDto det)
                                    {
                                        return det.UsuarioId == currentUserId;
                                    }
                                    return true;
                                };
                            }
                        }
                    }
                }
            }
            catch
            {
                // Ignorar en tiempo de diseño o si falla DI
            }
        }

        private void ConfigurarBusquedaCombos()
        {
            cmbClientes.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent, new TextChangedEventHandler(CmbClientes_TextChanged));
            cmbClientes.SelectionChanged += CmbClientes_SelectionChanged;

            cmbServicios.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent, new TextChangedEventHandler(CmbServicios_TextChanged));
            cmbServicios.SelectionChanged += CmbServicios_SelectionChanged;
            cmbServicios.DropDownOpened += CmbServicios_DropDownOpened;
        }

        private void CmbClientes_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_omitirFiltroClientes) return;

            if (e.OriginalSource is TextBox textBox && textBox.IsKeyboardFocused)
            {
                string texto = textBox.Text;
                int caret = textBox.SelectionStart;

                if (texto.Trim().Length < 3)
                {
                    _debouncerClientes.Cancel();
                    if (DataContext is OrdenServicioViewModel vmOrdenServicio)
                    {
                        vmOrdenServicio.FiltrarClientes(texto);
                    }
                    cmbClientes.IsDropDownOpen = false;
                    return;
                }

                _debouncerClientes.Debounce(() =>
                {
                    if (DataContext is OrdenServicioViewModel vmOrdenServicio)
                    {
                        _omitirFiltroClientes = true;
                        try
                        {
                            vmOrdenServicio.FiltrarClientes(texto);
                            cmbClientes.IsDropDownOpen = vmOrdenServicio.ClientesDisponibles.Any();
                            textBox.SelectionStart = Math.Min(caret, textBox.Text.Length);
                            textBox.SelectionLength = 0;
                        }
                        finally
                        {
                            _omitirFiltroClientes = false;
                        }
                    }
                    return Task.CompletedTask;
                });
            }
        }

        private void CmbClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_omitirFiltroClientes) return;

            if (cmbClientes.SelectedItem is ClienteDto cliente)
            {
                _debouncerClientes.Cancel();
                _omitirFiltroClientes = true;
                try
                {
                    if (DataContext is OrdenServicioViewModel vmOrdenServicio)
                    {
                        vmOrdenServicio.ClienteId = cliente.IdCliente;
                    }
                    cmbClientes.Text = cliente.NombreCompletoConDni;
                    cmbClientes.IsDropDownOpen = false;
                }
                finally
                {
                    _omitirFiltroClientes = false;
                }
            }
        }

        private void CmbServicios_DropDownOpened(object? sender, EventArgs e)
        {
            if (DataContext is OrdenServicioViewModel vmOrdenServicio)
            {
                if (string.IsNullOrWhiteSpace(cmbServicios.Text) || cmbServicios.Text.Trim().Length < 3)
                {
                    _omitirFiltroServicios = true;
                    try
                    {
                        vmOrdenServicio.FiltrarServicios(string.Empty);
                    }
                    finally
                    {
                        _omitirFiltroServicios = false;
                    }
                }
            }
        }

        private void CmbServicios_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_omitirFiltroServicios) return;

            if (e.OriginalSource is TextBox textBox && textBox.IsKeyboardFocused)
            {
                string texto = textBox.Text;
                int caret = textBox.SelectionStart;

                if (texto.Trim().Length < 3)
                {
                    _debouncerServicios.Cancel();
                    if (DataContext is OrdenServicioViewModel vmOrdenServicio)
                    {
                        vmOrdenServicio.FiltrarServicios(texto);
                    }
                    return;
                }

                _debouncerServicios.Debounce(() =>
                {
                    if (DataContext is OrdenServicioViewModel vmOrdenServicio)
                    {
                        _omitirFiltroServicios = true;
                        try
                        {
                            vmOrdenServicio.FiltrarServicios(texto);
                            cmbServicios.IsDropDownOpen = vmOrdenServicio.ServiciosDisponibles.Any();
                            textBox.SelectionStart = Math.Min(caret, textBox.Text.Length);
                            textBox.SelectionLength = 0;
                        }
                        finally
                        {
                            _omitirFiltroServicios = false;
                        }
                    }
                    return Task.CompletedTask;
                });
            }
        }

        private void CmbServicios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_omitirFiltroServicios) return;

            if (cmbServicios.SelectedItem is ServicioDto servicio)
            {
                _debouncerServicios.Cancel();
                _omitirFiltroServicios = true;
                try
                {
                    if (DataContext is OrdenServicioViewModel vmOrdenServicio)
                    {
                        vmOrdenServicio.ServicioBusquedaSeleccionado = servicio;
                    }
                    cmbServicios.Text = servicio.Nombre;
                    cmbServicios.IsDropDownOpen = false;
                }
                finally
                {
                    _omitirFiltroServicios = false;
                }
            }
        }

        private void BtnAgregarItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OrdenServicioViewModel vmOrdenServicioForm)
            {
                if (vmOrdenServicioForm.ServicioBusquedaSeleccionado == null && !string.IsNullOrWhiteSpace(cmbServicios.Text))
                {
                    vmOrdenServicioForm.ServicioBusquedaSeleccionado = vmOrdenServicioForm.ServiciosDisponibles
                        .FirstOrDefault(s => s.Nombre.Equals(cmbServicios.Text.Trim(), StringComparison.OrdinalIgnoreCase));
                }

                if (vmOrdenServicioForm.ServicioBusquedaSeleccionado != null)
                {
                    if (vmOrdenServicioForm.DetallesOrdenActual.Any(d => d.ServicioId == vmOrdenServicioForm.ServicioBusquedaSeleccionado.IdServicio))
                    {
                        Compartido.FrmConfirmacion.MostrarAviso("El servicio ya se encuentra asignado a un mecánico en esta orden.", "Aviso", Window.GetWindow(this));
                        return;
                    }

                    int idMecanicoAuto = 0;
                    if (Application.Current.MainWindow is MainWindow mWin && mWin.UsuarioSesionActual != null)
                    {
                        bool esMecanico = mWin.UsuarioSesionActual.RolNombre?.IndexOf("Mec", StringComparison.OrdinalIgnoreCase) >= 0 || mWin.UsuarioSesionActual.IdRol == 3;
                        if (esMecanico)
                            idMecanicoAuto = mWin.UsuarioSesionActual.IdUsuario;
                    }

                    vmOrdenServicioForm.DetallesOrdenActual.Add(new DetalleServicioDto
                    {
                        ServicioId = vmOrdenServicioForm.ServicioBusquedaSeleccionado.IdServicio,
                        ServicioNombre = vmOrdenServicioForm.ServicioBusquedaSeleccionado.Nombre,
                        UsuarioId = idMecanicoAuto,
                        Realizado = false,
                        Observaciones = string.Empty,
                        Cantidad = 1,
                        Precio = vmOrdenServicioForm.ServicioBusquedaSeleccionado.Precio,
                        Estado = "Pendiente",
                        Origen = "Manual"
                    });

                    _debouncerServicios.Cancel();
                    _omitirFiltroServicios = true;
                    try
                    {
                        vmOrdenServicioForm.ServicioBusquedaSeleccionado = null;
                        cmbServicios.Text = string.Empty;
                        vmOrdenServicioForm.FiltrarServicios(string.Empty);
                        cmbServicios.IsDropDownOpen = false;
                    }
                    finally
                    {
                        _omitirFiltroServicios = false;
                    }
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
                if (DataContext is OrdenServicioViewModel vmOrdenServicioForm)
                {
                    await vmOrdenServicioForm.LoadCombosAsync();
                }
            }
        }

        private async void BtnNuevoVehiculo_Click(object sender, RoutedEventArgs e)
        {
            var frm = new Vehiculo.FrmVehiculo();
            frm.Owner = Window.GetWindow(this);
            if (frm.ShowDialog() == true)
            {
                if (DataContext is OrdenServicioViewModel vmOrdenServicioForm)
                {
                    await vmOrdenServicioForm.LoadCombosAsync();
                }
            }
        }

        private async void BtnNuevoServicio_Click(object sender, RoutedEventArgs e)
        {
            var frm = new Servicio.FrmServicio();
            frm.Owner = Window.GetWindow(this);
            if (frm.ShowDialog() == true)
            {
                if (DataContext is OrdenServicioViewModel vmOrdenServicioForm)
                {
                    await vmOrdenServicioForm.LoadCombosAsync();
                }
            }
        }

        private void BtnQuitarItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is DetalleServicioDto detalle)
            {
                if (DataContext is OrdenServicioViewModel vmOrdenServicioForm)
                {
                    vmOrdenServicioForm.DetallesOrdenActual.Remove(detalle);
                    vmOrdenServicioForm.EvaluarEstadoGeneral();
                }
            }
        }

        private void Vm_RegistrationFailed(object? sender, string errorMessage)
        {
            Compartido.FrmConfirmacion.MostrarAviso(errorMessage, "Error", Window.GetWindow(this));
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OrdenServicioViewModel vmOrdenServicioForm)
            {
                if (!vmOrdenServicioForm.ValidateAll())
                {
                    Compartido.FrmConfirmacion.MostrarAviso("Por favor, revise los campos marcados en rojo.", "Validación", Window.GetWindow(this));
                    return;
                }
                if (vmOrdenServicioForm.DetallesOrdenActual.Count == 0)
                {
                    Compartido.FrmConfirmacion.MostrarAviso("Debe agregar al menos un servicio a la orden antes de guardar.", "Validación", Window.GetWindow(this));
                    return;
                }

                var ok = await vmOrdenServicioForm.GuardarOrdenAsync();
                if (ok)
                {
                    VolverAlListado();
                    if (Application.Current.MainWindow is MainWindow win)
                    {
                        win.MostrarToast("Orden de servicio guardada con éxito.");
                    }
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
                if (_origen == "MisTrabajos")
                {
                    mainWindow.AgregarPagina(new Pantalla.MisTrabajos.CtlMisTrabajos());
                }
                else
                {
                    mainWindow.AgregarPagina(new CtlOrdenServicio());
                }
            }
        }

        private void CheckBoxRevisado_Changed(object sender, RoutedEventArgs e)
        {
            if (DataContext is OrdenServicioViewModel vmOrdenServicioForm)
            {
                vmOrdenServicioForm.EvaluarEstadoGeneral();
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







