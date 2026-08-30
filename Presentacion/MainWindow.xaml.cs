using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TP_ControlVehicular.Presentacion.Cliente;

namespace TP_ControlVehicular
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void MenuItem_Click_Cliente(object sender, RoutedEventArgs e)
        {
            //var ctlCliente = App.ServiceProvider.GetRequiredService<CtlCliente>();
            //ctlCliente.DataContext = App.ServiceProvider.GetRequiredService<ClienteViewModel>();
            AgregarPagina(new CtlCliente());
        }
        private void MenuItem_Click_Vehiculo(object sender, RoutedEventArgs e)
        {
            /*var ctlVehiculo = App.ServiceProvider.GetRequiredService<CtlVehiculo>();
            ctlVehiculo.DataContext = App.ServiceProvider.GetRequiredService<VehiculoViewModel>();
            AgregarPagina(ctlVehiculo);*/
        }

        // Menú Modelo
        private void MenuItem_Click_Modelo(object sender, RoutedEventArgs e)
        {
            /*var ctlModelo = App.ServiceProvider.GetRequiredService<CtlModelo>();
            ctlModelo.DataContext = App.ServiceProvider.GetRequiredService<ModeloViewModel>();
            AgregarPagina(ctlModelo);*/
        }

        // Menú Marca
        private void MenuItem_Click_Marca(object sender, RoutedEventArgs e)
        {
            /*var ctlMarca = App.ServiceProvider.GetRequiredService<CtlMarca>();
            ctlMarca.DataContext = App.ServiceProvider.GetRequiredService<MarcaViewModel>();
            AgregarPagina(ctlMarca);*/
        }
        private void AgregarPagina(UserControl userControl)
        {
            // Reemplaza el contenido actual por el UserControl proporcionado
            this.grdContenido.Children.Clear();
            // Asegurar que el control ocupe todas las columnas del grid
            Grid.SetColumn(userControl, 0);
            // ColumnSpan must be >= 1. Si no hay columnas definidas, usar 1.
            var span = Math.Max(1, this.grdContenido.ColumnDefinitions.Count);
            Grid.SetColumnSpan(userControl, span);
            this.grdContenido.Children.Add(userControl);
        }
    }
}