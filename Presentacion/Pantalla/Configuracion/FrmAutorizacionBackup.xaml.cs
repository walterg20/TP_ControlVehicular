using System.Windows;

namespace TP_ControlVehicular.Presentacion.Pantalla.Configuracion
{
    public partial class FrmAutorizacionBackup : Window
    {
        public string DniAutorizador { get; private set; } = string.Empty;
        public string ContrasenaAutorizador { get; private set; } = string.Empty;

        public FrmAutorizacionBackup()
        {
            InitializeComponent();
            Loaded += (s, e) => txtDni.Focus();
        }

        private void TxtDni_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            DniAutorizador = txtDni.Text;
        }

        private void PwdContrasena_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ContrasenaAutorizador = pwdContrasena.Password;
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            DniAutorizador = txtDni.Text.Trim();
            ContrasenaAutorizador = pwdContrasena.Password;

            if (string.IsNullOrWhiteSpace(DniAutorizador))
            {
                TP_ControlVehicular.Presentacion.Pantalla.Compartido.FrmConfirmacion.MostrarAviso(
                    "Debe ingresar el DNI del usuario autorizador.", "Validación", this);
                return;
            }
            if (string.IsNullOrWhiteSpace(ContrasenaAutorizador))
            {
                TP_ControlVehicular.Presentacion.Pantalla.Compartido.FrmConfirmacion.MostrarAviso(
                    "Debe ingresar la contraseña del usuario autorizador.", "Validación", this);
                return;
            }

            DialogResult = true;
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}