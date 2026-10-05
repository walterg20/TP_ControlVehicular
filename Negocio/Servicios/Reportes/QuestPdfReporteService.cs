using QuestPDF.Fluent;
using TP_ControlVehicular.Negocio.DTOs.Reportes;
using TP_ControlVehicular.Negocio.Servicios.Reportes.Documentos;

namespace TP_ControlVehicular.Negocio.Servicios.Reportes
{
    public class QuestPdfReporteService : IReporteService
    {
        public byte[] GenerarComprobanteRecepcion(ComprobanteOrdenDto dto)
        {
            var document = new ComprobanteRecepcionDocument(dto);
            return document.GeneratePdf();
        }

        public byte[] GenerarComprobantePago(ComprobantePagoDto dto)
        {
            var document = new ComprobantePagoDocument(dto);
            return document.GeneratePdf();
        }
    }
}
