using System.Linq;
using System.Windows;

namespace TP_ControlVehicular.Presentacion.Pantalla.Compartido
{
    /// <summary>
    /// Interaction logic for FrmConfirmacion.xaml
    /// Modal estándar de confirmación para eliminar o cambiar estados.
    /// </summary>
    public partial class FrmConfirmacion : Window
    {
        public bool Confirmado { get; private set; } = false;

        public FrmConfirmacion(string mensaje, string titulo = "Confirmar eliminación", string textoAceptar = "✓ Sí, Aceptar", string textoCancelar = "❌ No, Cancelar")
        {
            InitializeComponent();
            lblTitulo.Text = titulo;
            lblMensaje.Text = mensaje;
            btnAceptar.Content = textoAceptar;
            btnCancelar.Content = textoCancelar;
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            Confirmado = true;
            DialogResult = true;
            Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Confirmado = false;
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Muestra el modal de confirmación y retorna true si el usuario aceptó.
        /// </summary>
        public static bool Mostrar(string mensaje, string titulo = "Confirmar eliminación", Window? owner = null)
        {
            var dialog = new FrmConfirmacion(mensaje, titulo);

            if (owner != null)
            {
                dialog.Owner = owner;
            }
            else
            {
                var activeWin = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
                if (activeWin != null)
                {
                    dialog.Owner = activeWin;
                }
                else
                {
                    dialog.Owner = Application.Current.MainWindow;
                }
            }

            return dialog.ShowDialog() == true;
        }

        /// <summary>
        /// Muestra un aviso/alerta informativo con el estilo del diálogo de confirmación (un solo botón Aceptar).
        /// </summary>
        public static void MostrarAviso(string mensaje, string titulo = "Aviso", Window? owner = null, string textoBoton = "✓ Aceptar", string icono = "⚠️")
        {
            var dialog = new FrmConfirmacion(mensaje, titulo, textoAceptar: textoBoton);
            dialog.btnCancelar.Visibility = Visibility.Collapsed;
            dialog.lblIcono.Text = icono;

            if (owner != null)
            {
                dialog.Owner = owner;
            }
            else
            {
                var activeWin = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
                if (activeWin != null)
                {
                    dialog.Owner = activeWin;
                }
                else
                {
                    dialog.Owner = Application.Current.MainWindow;
                }
            }

            dialog.ShowDialog();
        }
    }
}
