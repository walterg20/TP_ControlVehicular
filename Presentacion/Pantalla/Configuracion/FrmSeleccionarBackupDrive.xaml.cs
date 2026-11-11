using System.Collections.Generic;
using System.Windows;
using TP_ControlVehicular.Negocio.DTOs;

namespace TP_ControlVehicular.Presentacion.Pantalla.Configuracion
{
    public partial class FrmSeleccionarBackupDrive : Window
    {
        public DriveArchivoDto? ArchivoSeleccionado { get; private set; }

        public FrmSeleccionarBackupDrive(IEnumerable<DriveArchivoDto> archivos)
        {
            InitializeComponent();
            dgArchivos.ItemsSource = archivos;
            if (dgArchivos.Items.Count > 0)
                dgArchivos.SelectedIndex = 0;
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            if (dgArchivos.SelectedItem is DriveArchivoDto sel)
            {
                ArchivoSeleccionado = sel;
                DialogResult = true;
            }
            else
            {
                Presentacion.Pantalla.Compartido.FrmConfirmacion.MostrarAviso(
                    "Debe seleccionar un archivo de la lista.", "Sin selección", this);
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}