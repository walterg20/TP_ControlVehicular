// walterg20
using System.Windows;
using System.Windows.Controls;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Presentacion.Pantalla.Compartido;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Cliente
{
    /// <summary>
    /// Lógica de interacción para CtlCliente.xaml
    /// </summary>
    public partial class CtlCliente : UserControl
    {
        public CtlCliente()
        {
            InitializeComponent();
            // Asignar ViewModel desde DI para que la vista tenga DataContext y podamos cargar datos
            try
            {
                var vmCliente = App.ServiceProvider.GetService(typeof(ClienteViewModel)) as ClienteViewModel;
                if (vmCliente is not null)
                {
                    this.DataContext = vmCliente;
                    this.Loaded += async (s, e) => { await vmCliente.LoadAsync(); };
                }
            }
            catch
            {
                // ignore DI resolution errors
            }
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is TP_ControlVehicular.Presentacion.ViewModels.ClienteViewModel vmCliente)
            {
                // Asignar ItemsSource al filtro calculado
                dgClientes.ItemsSource = vmCliente.ListadoClientesFiltered;
            }
        }
        // Evento para simular la acción de "Editar" haciendo doble clic en la fila
        private void DgUsuarios_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            /*var usuarioSeleccionado; //= dgUsuarios.SelectedItem as UsuarioModel;

            if (usuarioSeleccionado != null)
            {
                MessageBox.Show($"Abriendo edición para: {usuarioSeleccionado.Nombre}", "Sistema MDI");
                // Acá ponés la lógica para mandar este 'usuarioSeleccionado' a tu formulario
            }*/
        }

        private async void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            var frm = new FrmCliente();
            frm.Owner = Window.GetWindow(this);
            var resultado = frm.ShowDialog();
            if (resultado == true && this.DataContext is ClienteViewModel clienteViewModel)
            {
                // Intentar recargar usando el DataContext (si es ClienteViewModel)
                if (this.DataContext is ClienteViewModel vmCliente)
                {
                    await vmCliente.LoadAsync();
                }

            }
        }

        private async void BtnBorrar_Click(object sender, RoutedEventArgs e)
        {
            // Eliminación lógica: marcar Activo = false
            var selected = dgClientes.SelectedItem as ClienteDto;
            if (selected is null)
            {
                FrmConfirmacion.MostrarAviso("Por favor, selecciona primero un cliente.", "Aviso", Window.GetWindow(this));
                return;
            }

            if (!FrmConfirmacion.Mostrar($"¿Seguro que querés marcar como inactivo al cliente {selected.Nombre} {selected.Apellido}?", "Confirmar baja", Window.GetWindow(this)))
                return;

            if (this.DataContext is ClienteViewModel vmCliente)
            {
                await vmCliente.DeleteClienteAsync(selected.IdCliente);
            }
        }
        private async void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ClienteViewModel vmCliente)
            {

                if (vmCliente.ClienteSeleccionado is null)
                {
                    FrmConfirmacion.MostrarAviso("Por favor, selecciona primero un cliente.", "Aviso", Window.GetWindow(this));
                    return;
                }
                var frm = new FrmCliente(vmCliente.ClienteSeleccionado);
                frm.Owner = Window.GetWindow(this);
                var ok = frm.ShowDialog();
                if (ok == true)
                {
                    await vmCliente.LoadAsync();
                }
            }


        }

    }
}
