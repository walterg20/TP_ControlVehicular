using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IPagoRepository
    {
        Task Create(Pago pago);
        Task<IEnumerable<Pago>> GetPagosByFactura(int idFactura);
    }
}
