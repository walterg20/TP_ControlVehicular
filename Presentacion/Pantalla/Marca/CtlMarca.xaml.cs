using System.Windows;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace TP_ControlVehicular.Presentacion.Marca
{
    /// <summary>
    /// Lógica de interacción para CtlMarca.xaml
    /// </summary>
    public partial class CtlMarca : UserControl
    {
        public CtlMarca()
        {
            InitializeComponent();
        }
        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Abrir formulario de creación vacío.", "Control Vehicular");

            FrmMarca modal = new FrmMarca();
            modal.Owner = Window.GetWindow(this);
            bool? resultado = modal.ShowDialog();
            if (resultado == true)
            {
                // Refrescar la grilla si es necesario
            }
        }

        private void BtnBorrar_Click(object sender, RoutedEventArgs e)
        {
            // Implementar lógica de borrado aquí
            // Por ejemplo: var marcaSeleccionada = dgMarcas.SelectedItem as MarcaModel;
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            // Implementar lógica de edición aquí
            // Por ejemplo: abrir FrmMarca con datos de la marca seleccionada
        }
    }
}
