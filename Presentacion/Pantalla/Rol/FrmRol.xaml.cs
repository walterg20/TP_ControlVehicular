using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Rol
{
    public partial class FrmRol : Window
    {
        private readonly bool _esModificacion;

        public FrmRol()
        {
            InitializeComponent();
            _esModificacion = false;
            this.Title = "Registrar Nuevo Rol";
            ConfigurarViewModel(null);
            Loaded += (s, e) => txtNombre.Focus();
            ConfigurarValidacionAlPerderFoco();
        }

        public FrmRol(RolDto rol)
        {
            InitializeComponent();
            _esModificacion = true;
            this.Title = "Modificar Rol";
            ConfigurarViewModel(rol);
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

        private void ConfigurarViewModel(RolDto? rol)
        {
            try
            {
                if (App.ServiceProvider.GetService(typeof(RolViewModel)) is RolViewModel vmRol)
                {
                    DataContext = vmRol;
                    if (_esModificacion && rol != null)
                    {
                        vmRol.RolSeleccionado = rol;
                        vmRol.Nombre = rol.Nombre;
                        vmRol.Descripcion = rol.Descripcion;
                        vmRol.Estado = rol.Estado;
                    }
                    else
                    {
                        vmRol.RolSeleccionado = null;
                        vmRol.Nombre = string.Empty;
                        vmRol.Descripcion = string.Empty;
                        vmRol.Estado = true;
                    }

                    vmRol.ClearAllErrors();
                }
            }
            catch
            {
                // Ignorar si DI no está disponible
            }
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RolViewModel vmRol)
            {
                var ok = await vmRol.GuardarRolAsync();
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
