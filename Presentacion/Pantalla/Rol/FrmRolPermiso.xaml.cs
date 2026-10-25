using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Handlers;

namespace TP_ControlVehicular.Presentacion.Pantalla.Rol
{
    public partial class FrmRolPermiso : Window
    {
        private readonly int _rolId;
        private readonly RolDto _rolDto;
        private List<PermisoDto> _permisosDisponibles = new();

        public FrmRolPermiso(RolDto rol)
        {
            InitializeComponent();
            _rolId = rol.IdRol;
            _rolDto = rol;
            lblTitulo.Text = $"Permisos de: {rol.Nombre}";
            Owner = Application.Current.MainWindow;
            Loaded += FrmRolPermiso_Loaded;
        }

        private async void FrmRolPermiso_Loaded(object sender, RoutedEventArgs e)
        {
            var handler = App.ServiceProvider.GetRequiredService<ObtenerPermisosHandler>();
            _permisosDisponibles = await handler.HandleAsync();

            // Marcar los que el rol ya tiene
            foreach (var p in _permisosDisponibles)
            {
                if (_rolDto.Permisos != null && _rolDto.Permisos.Contains(p.Nombre))
                {
                    p.Asignado = true;
                }
            }

            icPermisos.ItemsSource = _permisosDisponibles;
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            var seleccionados = _permisosDisponibles.Where(p => p.Asignado).Select(p => p.IdPermiso).ToList();
            var handler = App.ServiceProvider.GetRequiredService<ActualizarPermisosRolHandler>();
            
            bool result = await handler.HandleAsync(_rolId, seleccionados);
            if (result)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Ocurrió un error al guardar los permisos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
