using TP_ControlVehicular.Negocio.DTOs.Reportes;

namespace TP_ControlVehicular.Negocio.Servicios.Reportes
{
    public interface IReporteService
    {
        byte[] GenerarComprobanteRecepcion(ComprobanteOrdenDto dto);
        byte[] GenerarComprobantePago(ComprobantePagoDto dto);
    }
}
