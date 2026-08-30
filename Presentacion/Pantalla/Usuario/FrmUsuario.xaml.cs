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
using System.Windows.Shapes;

namespace TP_ControlVehicular.Presentacion.Usuario
{
    /// <summary>
    /// Lógica de interacción para FrmUsuario.xaml
    /// </summary>
    public partial class FrmUsuario : Window
    {
        public FrmUsuario()
        {
            InitializeComponent();
        }
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
