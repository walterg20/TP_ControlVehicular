using System;
using System.Collections.Generic;
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
                var vm = App.ServiceProvider.GetService(typeof(TP_ControlVehicular.Presentacion.ViewModels.ClienteViewModel)) as TP_ControlVehicular.Presentacion.ViewModels.ClienteViewModel;
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

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is TP_ControlVehicular.Presentacion.ViewModels.ClienteViewModel vm)
            {
                // Asignar ItemsSource al filtro calculado
                dgClientes.ItemsSource = vm.ListadoClientesFiltered;
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
            MessageBox.Show("Abrir formulario de creación vacío.", "Control Vehicular");
            // 1. Creamos la ventana modal
            FrmCliente modal = new FrmCliente();

            // 2. Opcional: Centrar el modal respecto a la ventana principal de la app
            modal.Owner = Window.GetWindow(this);

            // 3. Abrimos el modal. El código se "detiene" aquí hasta que el usuario guarde o cierre
            bool? resultado = modal.ShowDialog();

            // 4. Si guardó correctamente (DialogResult = true), refrescamos
            if (resultado == true)
            {
                // Intentar recargar usando el DataContext (si es ClienteViewModel)
                if (this.DataContext is TP_ControlVehicular.Presentacion.ViewModels.ClienteViewModel vm)
                {
                    await vm.LoadAsync();
                }
                else if (this.DataContext is null && this.Parent is FrameworkElement parent)
                {
                    // Buscar en el control padre
                    if (parent.DataContext is TP_ControlVehicular.Presentacion.ViewModels.ClienteViewModel vm2)
                        await vm2.LoadAsync();
                }
            }
        }

        private async void BtnBorrar_Click(object sender, RoutedEventArgs e)
        {
            // Eliminación lógica: marcar Activo = false
            var selected = dgClientes.SelectedItem as TP_ControlVehicular.Negocio.DTOs.ClienteDto;
            if (selected is null)
            {
                MessageBox.Show("Por favor, selecciona primero un cliente.", "Aviso");
                return;
            }

            var result = MessageBox.Show($"¿Seguro que querés marcar como inactivo al cliente {selected.Nombre} {selected.Apellido}?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;

            if (this.DataContext is TP_ControlVehicular.Presentacion.ViewModels.ClienteViewModel vm)
            {
                await vm.DeleteClienteAsync(selected.IdCliente);
            }
        }
        private async void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgClientes.SelectedItem as TP_ControlVehicular.Negocio.DTOs.ClienteDto;
            if (selected is null)
            {
                MessageBox.Show("Por favor, selecciona primero un cliente.", "Aviso");
                return;
            }

            if (this.DataContext is TP_ControlVehicular.Presentacion.ViewModels.ClienteViewModel vm)
            {
                // cargar datos en el mismo ViewModel usado por el formulario
                vm.IdCliente = selected.IdCliente;
                vm.Nombre = selected.Nombre;
                vm.Apellido = selected.Apellido;
                vm.Dni = selected.Dni;
                vm.FechaNacimiento = selected.FechaNac;
                vm.Direccion = selected.Direccion;
                vm.Email = selected.Email;
                vm.Telefono = selected.Telefono;
                vm.Activo = selected.Activo;

                // Abrir modal (usa el mismo VM desde DI en FrmCliente)
                var modal = new FrmCliente();
                modal.Owner = Window.GetWindow(this);
                var ok = modal.ShowDialog();
                if (ok == true)
                {
                    await vm.LoadAsync();
                }
                else
                {
                    // limpiar IdCliente si canceló
                    vm.IdCliente = 0;
                }
            }
        }

        private void dgClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
