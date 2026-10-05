using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IMetodoPagoRepository
    {
        Task<IEnumerable<MetodoPago>> GetAll();
    }
}
