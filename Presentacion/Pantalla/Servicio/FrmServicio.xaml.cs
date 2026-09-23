using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Presentacion.Pantalla.Compartido;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Pantalla.Servicio
{
    /// <summary>
    /// Interaction logic for FrmServicio.xaml
    /// </summary>
    public partial class FrmServicio : Window
    {
        private readonly ServicioViewModel _viewModel;

        public FrmServicio()
        {
            InitializeComponent();
            _viewModel = (App.ServiceProvider?.GetService(typeof(ServicioViewModel)) as ServicioViewModel)!;
            DataContext = _viewModel;
            ConfigurarValidacionAlPerderFoco();
        }

        public FrmServicio(ServicioViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            ConfigurarValidacionAlPerderFoco();
        }

        public void PrepararParaNuevo()
        {
            Title = "Registrar Nuevo Servicio";
            _viewModel.LimpiarFormulario();
        }

        public void PrepararParaEdicion(ServicioDto servicio)
        {
            Title = "Modificar Servicio";
            _viewModel.IdServicio = servicio.IdServicio;
            _viewModel.Nombre = servicio.Nombre;
            _viewModel.Precio = servicio.Precio;
            _viewModel.Activo = servicio.Activo;
            _viewModel.ClearAllErrors();
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

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!_viewModel.ValidateAll())
            {
                FrmConfirmacion.MostrarAviso("Por favor, corrija los errores del formulario antes de continuar.", "Validación", this);
                return;
            }

            var ok = await _viewModel.SaveAsync();
            if (ok)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                FrmConfirmacion.MostrarAviso("No se pudo guardar el servicio. Por favor, verifique los datos.", "Error", this, icono: "❌");
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
