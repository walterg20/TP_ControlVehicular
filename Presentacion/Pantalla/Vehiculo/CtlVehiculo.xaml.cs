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
using TP_ControlVehicular.Presentacion.Cliente;

namespace TP_ControlVehicular.Presentacion.Vehiculo
{
    /// <summary>
    /// Lógica de interacción para CtlVehiculo.xaml
    /// </summary>
    public partial class CtlVehiculo : UserControl
    {
        public CtlVehiculo()
        {
            InitializeComponent();
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

        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
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
                //RefrescarGrillaDesdeBaseDatos();
            }
        }

        private void BtnBorrar_Click(object sender, RoutedEventArgs e)
        {
            // Captura la fila que el usuario dejó marcada con un clic común
            // var usuarioSeleccionado = new(); //dgUsuarios.SelectedItem as UsuarioModel;

            /* if (usuarioSeleccionado != null)
             {
                 var result = MessageBox.Show($"¿Seguro que querés eliminar a {usuarioSeleccionado.Nombre}?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                 if (result == MessageBoxResult.Yes)
                 {
                     //Usuarios.Remove(usuarioSeleccionado);
                 }
             }
             else
             {
                 MessageBox.Show("Por favor, selecciona primero un usuario de la lista haciendo un clic sobre él.", "Aviso");
             }*/
        }
        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            // Captura la fila que el usuario seleccionó con un clic común en la grilla
            /* var usuarioSeleccionado = dgUsuarios.SelectedItem as UsuarioModel;

             if (usuarioSeleccionado != null)
             {
                 MessageBox.Show($"Abriendo formulario de edición para: {usuarioSeleccionado.Nombre}", "Sistema MDI");
                 // Acá ponés la lógica para pasar 'usuarioSeleccionado' a tu pantalla de carga
             }
             else
             {
                 MessageBox.Show("Por favor, selecciona un usuario de la lista antes de editar.", "Aviso");
             }*/
        }
    }
}
