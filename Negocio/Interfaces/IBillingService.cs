using System.Threading.Tasks;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IBillingService
    {
        Task<Factura> GenerarFactura(int idRegistro);
        Task<(Pago pago, decimal vuelto)> RegistrarPago(int idFactura, int idMetodoPago, decimal montoAbonado);
    }
}
