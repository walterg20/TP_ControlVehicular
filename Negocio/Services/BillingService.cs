using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class BillingService : IBillingService
    {
        private readonly CVDbContext _context;
        private readonly IFacturaRepository _facturaRepo;
        private readonly IPagoRepository _pagoRepo;
        private readonly IMetodoPagoRepository _metodoRepo;

        public BillingService(
            CVDbContext context,
            IFacturaRepository facturaRepo,
            IPagoRepository pagoRepo,
            IMetodoPagoRepository metodoRepo)
        {
            _context = context;
            _facturaRepo = facturaRepo;
            _pagoRepo = pagoRepo;
            _metodoRepo = metodoRepo;
        }

        public async Task<Factura> GenerarFactura(int registroServicioId)
        {
            var detalles = await _context.DetalleServicios
                .Where(d => d.RegistroServicioId == registroServicioId)
                .ToListAsync();

            decimal total = detalles.Sum(d => d.Precio * d.Cantidad);

            var facturaExistente = await _context.Facturas.FirstOrDefaultAsync(f => f.RegistroServicioId == registroServicioId);
            if (facturaExistente != null)
                return facturaExistente;

            var factura = new Factura
            {
                RegistroServicioId = registroServicioId,
                Fecha = DateTime.Now,
                Total = total
            };

            await _facturaRepo.Create(factura);

            return factura;
        }

        public async Task<(Pago pago, decimal vuelto)> RegistrarPago(int facturaId, int metodoPagoId, decimal montoAbonado)
        {
            var factura = await _facturaRepo.GetById(facturaId);
            if (factura == null)
                throw new Exception("Factura no encontrada.");

            var registro = await _context.RegistroServicios.FindAsync(factura.RegistroServicioId);
            if (registro == null)
                throw new Exception("Orden de servicio no encontrada.");
            
            if (registro.Estado != "Completado")
                throw new Exception("La orden de servicio debe estar completada para procesar el pago.");

            var pagosRealizados = await _pagoRepo.GetPagosByFactura(facturaId);
            decimal pagado = pagosRealizados.Sum(p => p.Monto);
            decimal balance = factura.Total - pagado;

            if (balance <= 0)
                throw new Exception("La factura ya está pagada.");

            var metodos = await _metodoRepo.GetAll();
            var metodo = metodos.FirstOrDefault(m => m.Id == metodoPagoId);
            if (metodo == null)
                throw new Exception("Método de pago no válido.");

            decimal montoFinal = montoAbonado;
            decimal vuelto = 0;

            if (metodo.Nombre == "Tarjeta")
            {
                if (montoAbonado != balance)
                    throw new Exception("El pago con tarjeta debe ser por el monto exacto del saldo.");
            }
            else if (metodo.Nombre == "Efectivo")
            {
                if (montoAbonado > balance)
                {
                    montoFinal = balance;
                    vuelto = montoAbonado - balance;
                }
            }

            var pago = new Pago
            {
                FacturaId = facturaId,
                MetodoPagoId = metodoPagoId,
                Monto = montoFinal,
                FechaPago = DateTime.Now
            };

            await _pagoRepo.Create(pago);

            decimal nuevoPagado = pagado + montoFinal;
            if (nuevoPagado >= factura.Total)
            {
                if (registro != null)
                {
                    registro.Estado = "Pagado";
                    await _context.SaveChangesAsync();
                }
            }

            return (pago, vuelto);
        }
    }
}
