using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Marca
{
    public partial class FrmMarca : Window
    {
        private readonly bool _esModificacion;

        public FrmMarca()
        {
            InitializeComponent();
            _esModificacion = false;
            this.Title = "Registrar Nueva Marca";
            ConfigurarViewModel(null);
            Loaded += (s, e) => txtNombreMarca.Focus();
            ConfigurarValidacionAlPerderFoco();
        }

        public FrmMarca(MarcaDto marca)
        {
            InitializeComponent();
            _esModificacion = true;
            this.Title = "Modificar Marca";
            ConfigurarViewModel(marca);
            Loaded += (s, e) => txtNombreMarca.Focus();
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

        private void ConfigurarViewModel(MarcaDto? marca)
        {
            try
            {
                if (App.ServiceProvider.GetService(typeof(MarcaViewModel)) is MarcaViewModel vmMarca)
                {
                    DataContext = vmMarca;
                    if (_esModificacion && marca != null)
                    {
                        vmMarca.MarcaSeleccionada = marca;
                        vmMarca.NombreMarca = marca.NombreMarca;
                    }
                    else
                    {
                        vmMarca.MarcaSeleccionada = null;
                        vmMarca.NombreMarca = string.Empty;
                    }

                    vmMarca.ClearAllErrors();
                }
            }
            catch
            {
                // Silenciar si DI no está disponible
            }
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MarcaViewModel vmMarca)
            {
                var ok = await vmMarca.GuardarMarcaAsync();
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
