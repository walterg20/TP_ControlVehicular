// walterg20
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IConfiguracionEmpresaRepository : IRepository<ConfiguracionEmpresa>
    {
        Task<ConfiguracionEmpresa?> ObtenerAsync();
    }
}