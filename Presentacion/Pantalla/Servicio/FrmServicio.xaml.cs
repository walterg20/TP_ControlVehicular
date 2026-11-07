// walterg20
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Presentacion.Pantalla.Compartido;
using TP_ControlVehicular.Presentacion.ViewModels;
using TP_ControlVehicular.Negocio.Services;

namespace TP_ControlVehicular.Presentacion.Pantalla.Servicio
{
    /// <summary>
    /// Interaction logic for FrmServicio.xaml
    /// </summary>
    public partial class FrmServicio : Window
    {
        private bool _esModificacion = false;

        // Constructor para Nuevo
        public FrmServicio()
        {
            InitializeComponent();
            lblTituloFormulario.Text = "Registrar Nuevo Servicio";
            _esModificacion = false;
            this.Title = "Registrar Nuevo Servicio";

            this.ConfigurarViewModel(null);

            Loaded += (s, e) => txtNombre.Focus();
            ConfigurarValidacionAlPerderFoco();
        }

        // Constructor para Modificar
        public FrmServicio(ServicioDto servicioDto)
        {
            InitializeComponent();
            lblTituloFormulario.Text = "Modificar Servicio";
            _esModificacion = true;
            this.Title = "Modificar Servicio";

            this.ConfigurarViewModel(servicioDto);
            Loaded += (s, e) => txtNombre.Focus();
            ConfigurarValidacionAlPerderFoco();
        }

        private void ConfigurarValidacionAlPerderFoco()
        {
            AddHandler(UIElement.LostFocusEvent, new RoutedEventHandler((s, e) =>
            {
                if (e.OriginalSource is FrameworkElement element && DataContext is BaseViewModel vmServicio)
                {
                    DependencyProperty? dp = null;
                    if (element is TextBox)
                        dp = TextBox.TextProperty;

                    if (dp != null)
                    {
                        var binding = BindingOperations.GetBinding(element, dp);
                        var be = element.GetBindingExpression(dp);
                        if (binding != null && binding.Path != null && !string.IsNullOrEmpty(binding.Path.Path))
                        {
                            be?.UpdateSource();
                            vmServicio.ValidateProperty(binding.Path.Path);
                        }
                    }
                }
            }));
        }

        private void ConfigurarViewModel(ServicioDto? servicio)
        {
            try
            {
                if (App.ServiceProvider?.GetService(typeof(ServicioViewModel)) is ServicioViewModel vmServicio)
                {
                    DataContext = vmServicio;
                    vmServicio.NotificarPermisos();

                    if (_esModificacion && servicio != null)
                    {
                        vmServicio.IdServicio = servicio.IdServicio;
                        vmServicio.Nombre = servicio.Nombre;
                        vmServicio.Precio = servicio.Precio;
                        vmServicio.Activo = servicio.Activo;
                    }
                    else
                    {
                        vmServicio.LimpiarFormulario();
                    }
                    vmServicio.ClearAllErrors();

                    bool puedeOperar = _esModificacion ? vmServicio.PuedeEditar : vmServicio.PuedeCrear;
                    if (!puedeOperar)
                    {
                        btnGuardar.IsEnabled = false;
                        txtNombre.IsReadOnly = true;
                        txtPrecio.IsReadOnly = true;
                        chkActivo.IsEnabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                (App.ServiceProvider?.GetService(typeof(IErrorNotifier)) as IErrorNotifier)?.Notify(ex, "FrmServicio.xaml");
                // Silenciar si DI no está disponible
            }
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (FocusManager.GetFocusedElement(this) is FrameworkElement fe)
            {
                BindingOperations.GetBindingExpression(fe, TextBox.TextProperty)?.UpdateSource();
            }

            if (DataContext is ServicioViewModel vmServicio)
            {
                bool puedeOperar = _esModificacion ? vmServicio.PuedeEditar : vmServicio.PuedeCrear;
                if (!puedeOperar)
                {
                    FrmConfirmacion.MostrarAviso("No tiene permisos para guardar cambios en este servicio.", "Acceso denegado", this);
                    return;
                }

                if (!vmServicio.ValidateAll())
                {
                    FrmConfirmacion.MostrarAviso("Por favor, corrija los errores del formulario antes de continuar.", "Validación", this);
                    return;
                }

                var ok = await vmServicio.SaveAsync();
                if (ok)
                {
                    this.DialogResult = true;
                    Close();
                }
                else
                {
                    FrmConfirmacion.MostrarAviso("No se pudo guardar el servicio. Por favor, verifique los datos.", "Error", this, icono: "❌");
                }
            }
            else
            {
                this.DialogResult = false;
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
