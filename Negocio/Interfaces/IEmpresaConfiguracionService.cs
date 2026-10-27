// walterg20
using TP_ControlVehicular.Negocio.DTOs;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IEmpresaConfiguracionService
    {
        Task<EmpresaConfiguracionDto> ObtenerAsync();
        Task<EmpresaConfiguracionDto> GuardarAsync(EmpresaConfiguracionDto dto, byte[]? logoBytes);
        Task RefrescarCacheAsync();
    }
}