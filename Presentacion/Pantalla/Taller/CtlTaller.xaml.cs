using System.Windows;
using System.Windows.Controls;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Taller
{
    public partial class CtlTaller : UserControl
    {
        public CtlTaller()
        {
            InitializeComponent();
            // Asignar ViewModel desde DI para que la vista tenga DataContext y podamos cargar datos
            try
            {
                var vm = App.ServiceProvider.GetService(typeof(TallerViewModel)) as TallerViewModel;
                if (vm is not null)
                {
                    this.DataContext = vm;
                    this.Loaded += async (s, e) => { await vm.LoadAsync(); };
                }
            }
            catch
            {
                // ignore DI resolution errors
            }
        }

        // El filtro de búsqueda se aplica al listado calculado, sin recargar de la BD
        private void TxtBusqueda_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.DataContext is TallerViewModel vm)
            {
                dgTalleres.ItemsSource = vm.ListadoTalleresFiltered;
            }
        }

        private async void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            var frm = new FrmTaller();
            frm.Owner = Window.GetWindow(this);
            var ok = frm.ShowDialog();
            if (ok == true && this.DataContext is TallerViewModel vm)
            {
                await vm.LoadAsync();
            }
        }

        private async void BtnModificar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is TallerViewModel vm)
            {
                if (vm.TallerSeleccionado == null)
                {
                    MessageBox.Show("Por favor, selecciona primero un taller.", "Aviso");
                    return;
                }

                var frm = new FrmTaller(vm.TallerSeleccionado);
                frm.Owner = Window.GetWindow(this);
                var ok = frm.ShowDialog();
                if (ok == true)
                {
                    await vm.LoadAsync();
                }
            }
        }

        // Baja lógica: marca Activo = false usando ModificarTallerHandler
        private async void BtnBaja_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is TallerViewModel vm)
            {
                if (vm.TallerSeleccionado == null)
                {
                    MessageBox.Show("Por favor, selecciona primero un taller.", "Aviso");
                    return;
                }

                var msg = MessageBox.Show("¿Dar de baja (lógico) este taller?\nSetear Activo = false?", "Confirmar baja", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (msg != MessageBoxResult.Yes) return;

                await vm.ToggleActivoAsync();
                await vm.LoadAsync();
            }
        }
    }
}