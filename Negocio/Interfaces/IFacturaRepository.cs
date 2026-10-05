using System.Threading.Tasks;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IFacturaRepository
    {
        Task Create(Factura factura);
        Task<Factura?> GetById(int idFactura);
        Task<Factura?> GetByRegistroServicio(int idRegistro);
    }
}
