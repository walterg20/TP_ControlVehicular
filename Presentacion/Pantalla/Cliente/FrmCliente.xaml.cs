
using System.Windows;
using TP_ControlVehicular.Negocio.DTOs;

namespace TP_ControlVehicular.Presentacion.Cliente
{
    /// <summary>
    /// Lógica de interacción para FrmCliente.xaml
    /// </summary>
    public partial class FrmCliente : Window
    {
        private bool _esModificacion = false;

        // Constructor para Nuevo
        public FrmCliente()
        {
            InitializeComponent();
            lblTituloFormulario.Text = "Registrar Nuevo Cliente";
            _esModificacion = false;
            this.Title = "Registrar Nuevo Cliente";

            // Obtener el ViewModel desde el contenedor DI y suscribirse al evento
            try
            {
                if (App.ServiceProvider.GetService(typeof(Presentacion.ViewModels.ClienteViewModel)) is Presentacion.ViewModels.ClienteViewModel vm)
                {
                    DataContext = vm;
                    vm.RegistrationCompleted += (s, ok) =>
                    {
                        // Cerrar la ventana en el hilo de la UI
                        Dispatcher.Invoke(() => this.DialogResult = ok);
                    };
                }
            }
            catch
            {
                // Ignorar si DI no está disponible
            }
        }

        // Constructor para Editar
        /*public FrmCliente(UsuarioModel usuario)
        {
            InitializeComponent();
            lblTituloFormulario.Text = "Modificar Usuario";
            _usuarioEdicion = usuario;
            _esModificacion = true;

            txtNombre.Text = usuario.Nombre;
            txtContrasena.Password = usuario.Contrasena;
            cmbRol.SelectedIndex = usuario.IdRol - 1;
            chkEstado.IsChecked = usuario.Estado;
        }*/

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is Presentacion.ViewModels.ClienteViewModel vm)
            {
                var ok = await vm.RegistrarClienteAsync();
                this.DialogResult = ok;
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
