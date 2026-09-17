using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Presentacion.Pantalla.Compartido;
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
            Loaded += (s, e) => txtNombre.Focus();
            ConfigurarValidacionAlPerderFoco();
        }

        // Constructor para Modificar
        public FrmTaller(TallerDto taller)
        {
            InitializeComponent();
            _esModificacion = true;
            this.Title = "Modificar Taller";
            ConfigurarViewModel(taller);
            Loaded += (s, e) => txtNombre.Focus();
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

        private void ConfigurarViewModel(TallerDto? taller)
        {
            try
            {
                if (App.ServiceProvider?.GetService(typeof(TallerViewModel)) is TallerViewModel vmTaller)
                {
                    DataContext = vmTaller;
                    if (_esModificacion && taller != null)
                    {
                        vmTaller.TallerSeleccionado = taller;
                        vmTaller.Nombre = taller.Nombre ?? string.Empty;
                        vmTaller.Direccion = taller.Direccion ?? string.Empty;
                        vmTaller.Telefono = taller.Telefono ?? string.Empty;
                        vmTaller.Activo = taller.Activo;
                    }
                    else
                    {
                        vmTaller.LimpiarFormulario();
                    }

                    vmTaller.ClearAllErrors();
                }
            }
            catch
            {
                // Ignorar si DI no está disponible
            }
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TallerViewModel vmTaller)
            {
                var ok = await vmTaller.GuardarTallerAsync();
                if (ok)
                {
                    this.DialogResult = true;
                }
                else
                {
                    FrmConfirmacion.MostrarAviso("Por favor, corrija los errores del formulario antes de continuar.", "Validación", this);
                }
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