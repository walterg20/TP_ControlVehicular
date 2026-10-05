using System;
using System.Threading.Tasks;
using TP_ControlVehicular.Negocio.Interfaces;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Entidad;
using AutoMapper;
using System.Linq;

namespace TP_ControlVehicular.Negocio.Handlers.Pagos
{
    public class ProcesarPagoHandler
    {
        private readonly IRepository<Factura> _facturaRepository;
        private readonly IMapper _mapper;

        public ProcesarPagoHandler(IRepository<Factura> facturaRepository, IMapper mapper)
        {
            _facturaRepository = facturaRepository;
            _mapper = mapper;
        }

        public async Task<FacturaDto> HandleAsync(int registroServicioId, decimal total, string metodoPago, decimal montoRecibido, string titular = "")
        {
            throw new NotImplementedException("Pending Phase 3 Implementation");
        }
    }
}
