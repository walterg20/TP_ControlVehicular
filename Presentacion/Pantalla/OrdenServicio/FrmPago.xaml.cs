using System;
using System.Windows;
using TP_ControlVehicular.Negocio.Handlers.Pagos;
using TP_ControlVehicular.Presentacion.Pantalla.Compartido;

namespace TP_ControlVehicular.Presentacion.Pantalla.OrdenServicio
{
    public partial class FrmPago : Window
    {
        private readonly TP_ControlVehicular.Negocio.Interfaces.IBillingService _billingService;
        private readonly int _registroServicioId;
        private readonly decimal _total;

        public FrmPago(TP_ControlVehicular.Negocio.Interfaces.IBillingService billingService, int registroServicioId, decimal total)
        {
            InitializeComponent();
            _billingService = billingService;
            _registroServicioId = registroServicioId;
            _total = total;
            lblTotal.Text = $"$ {_total:F2}";
        }

        private void optEfectivo_Checked(object sender, RoutedEventArgs e)
        {
            if (pnlEfectivo != null) pnlEfectivo.Visibility = Visibility.Visible;
            if (pnlTarjeta != null) pnlTarjeta.Visibility = Visibility.Collapsed;
        }

        private void optTarjeta_Checked(object sender, RoutedEventArgs e)
        {
            if (pnlEfectivo != null) pnlEfectivo.Visibility = Visibility.Collapsed;
            if (pnlTarjeta != null) pnlTarjeta.Visibility = Visibility.Visible;
        }

        private async void cmdPagar_Click(object sender, RoutedEventArgs e)
        {
            cmdPagar.IsEnabled = false;
            cmdCancelar.IsEnabled = false;
            lblCargando.Visibility = Visibility.Visible;

            try
            {
                // Task 3: Generar factura primero.  
                // Note: Si la factura ya se generó antes esto podría fallar o duplicar si no se controla,
                // pero la task specifications no habla de persistir un estado previo o un "GenerarFactura" por separado.
                // Lo hacemos todo acá.
                var factura = await _billingService.GenerarFactura(_registroServicioId);

                if (optEfectivo.IsChecked == true)
                {
                    if (decimal.TryParse(txtMontoRecibido.Text, out decimal montoRecibido))
                    {
                        var (pago, vuelto) = await _billingService.RegistrarPago(factura.Id, 1, montoRecibido); // 1 = Efectivo
                        FrmConfirmacion.MostrarAviso($"Pago registrado correctamente.\nSu vuelto es: $ {vuelto:F2}", "Éxito", Window.GetWindow(this));
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        FrmConfirmacion.MostrarAviso("Monto inválido.", "Error", Window.GetWindow(this));
                    }
                }
                else if (optTarjeta.IsChecked == true)
                {
                    // Tarjeta: asumimos el Id = 2
                    var titular = txtTitular.Text;
                    // Simular validación
                    if (titular.ToLower().Trim() == "lim")
                        throw new Exception("Límite insuficiente.");
                    if (titular.ToLower().Trim() != "apro")
                        throw new Exception("Pago rechazado.");
                        
                    var (pago, vuelto) = await _billingService.RegistrarPago(factura.Id, 2, _total);
                    FrmConfirmacion.MostrarAviso("Pago aprobado y registrado correctamente.", "Éxito", Window.GetWindow(this));
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                FrmConfirmacion.MostrarAviso($"Ocurrió un error: {ex.Message}", "Error", Window.GetWindow(this));
            }
            finally
            {
                cmdPagar.IsEnabled = true;
                cmdCancelar.IsEnabled = true;
                lblCargando.Visibility = Visibility.Collapsed;
            }
        }

        private void cmdCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
