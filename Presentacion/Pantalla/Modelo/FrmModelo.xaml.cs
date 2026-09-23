using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Presentacion.Marca;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Modelo
{
    public partial class FrmModelo : Window
    {
        private readonly bool _esModificacion;

        public FrmModelo()
        {
            InitializeComponent();
            _esModificacion = false;
            this.Title = "Registrar Nuevo Modelo";
            ConfigurarViewModel(null);
            Loaded += (s, e) => txtNombreModelo.Focus();
            ConfigurarValidacionAlPerderFoco();
        }

        public FrmModelo(int idMarcaDefault)
        {
            InitializeComponent();
            _esModificacion = false;
            this.Title = "Registrar Nuevo Modelo";
            ConfigurarViewModel(null, idMarcaDefault);
            Loaded += (s, e) => txtNombreModelo.Focus();
            ConfigurarValidacionAlPerderFoco();
        }

        public FrmModelo(ModeloDto modelo)
        {
            InitializeComponent();
            _esModificacion = true;
            this.Title = "Modificar Modelo";
            ConfigurarViewModel(modelo);
            Loaded += (s, e) => txtNombreModelo.Focus();
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

        private async void ConfigurarViewModel(ModeloDto? modelo, int? idMarcaDefault = null)
        {
            try
            {
                if (App.ServiceProvider.GetService(typeof(ModeloViewModel)) is ModeloViewModel vmModelo)
                {
                    DataContext = vmModelo;
                    await vmModelo.CargarMarcasAsync(idMarcaDefault);

                    if (_esModificacion && modelo != null)
                    {
                        vmModelo.ModeloSeleccionado = modelo;
                        vmModelo.NombreModelo = modelo.NombreModelo;
                        vmModelo.IdMarca = modelo.IdMarca;
                    }
                    else
                    {
                        vmModelo.ModeloSeleccionado = null;
                        vmModelo.NombreModelo = string.Empty;
                        if (idMarcaDefault.HasValue && idMarcaDefault.Value > 0)
                        {
                            vmModelo.IdMarca = idMarcaDefault.Value;
                        }
                        else
                        {
                            vmModelo.IdMarca = 0;
                        }
                    }

                    vmModelo.ClearAllErrors();
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
            if (ok == true && DataContext is ModeloViewModel vmModelo)
            {
                await vmModelo.CargarMarcasAsync();
            }
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ModeloViewModel vmModelo)
            {
                var ok = await vmModelo.GuardarModeloAsync();
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
