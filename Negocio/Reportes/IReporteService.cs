using TP_ControlVehicular.Negocio.DTOs.Reportes;

namespace TP_ControlVehicular.Negocio.Reportes
{
    public interface IReporteService
    {
        byte[] GenerarComprobanteRecepcion(ComprobanteOrdenDto dto);
        byte[] GenerarComprobantePago(ComprobantePagoDto dto);
    }
}
