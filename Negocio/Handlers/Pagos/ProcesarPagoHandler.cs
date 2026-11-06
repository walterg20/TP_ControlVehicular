// walterg20
using TP_ControlVehicular.Negocio.Interfaces;
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
    }
}
