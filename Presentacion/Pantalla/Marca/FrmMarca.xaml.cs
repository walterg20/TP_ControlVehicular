using System.Windows;


namespace TP_ControlVehicular.Presentacion.Marca
{
    /// <summary>
    /// Lógica de interacción para FrmMarca.xaml
    /// </summary>
    public partial class FrmMarca : Window
    {
        private readonly bool _esModificacion;

        public FrmMarca()
        {
            InitializeComponent();
            lblTituloFormulario.Text = "Registrar Nuevo Marca";
            _esModificacion = false;
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

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // [AQUÍ HACES TU INSERCIÓN O UPDATE EN LA BASE DE DATOS]
            // Ejemplo: Instanciar tu BLL/DAL y enviar los parámetros.

            /*  if (_esModificacion)
              {
                  _usuarioEdicion.Nombre = txtNombre.Text;
                  _usuarioEdicion.Contrasena = txtContrasena.Password;
                  _usuarioEdicion.Estado = chkEstado.IsChecked ?? false;
              }*/

            // OPERACIÓN EXITOSA: Cambiar DialogResult a 'true' cierra la ventana automáticamente
            this.DialogResult = true;
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false; // Cierra sin hacer nada
        }
    }
}
