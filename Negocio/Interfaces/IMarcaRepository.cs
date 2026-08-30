

using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IMarcaRepository: IRepository<Marca>
    {
        Task<Marca?> GetByNombreAsync(string nombre);
    }
}
