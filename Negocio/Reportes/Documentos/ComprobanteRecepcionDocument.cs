using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TP_ControlVehicular.Negocio.DTOs.Reportes;

namespace TP_ControlVehicular.Negocio.Reportes.Documentos
{
    public class ComprobanteRecepcionDocument : IDocument
    {
        private readonly ComprobanteOrdenDto _dto;

        public ComprobanteRecepcionDocument(ComprobanteOrdenDto dto)
        {
            _dto = dto;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Element(ComposeHeader);

                page.Content().PaddingVertical(20).Column(col =>
                {
                    col.Spacing(15);
                    col.Item().Text("Datos de Recepción").FontSize(14).SemiBold().FontColor(Colors.Blue.Medium);
                    
                    col.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Padding(10).Column(inner =>
                    {
                        inner.Spacing(5);
                        inner.Item().Text($"Orden N°: {_dto.OrdenId}").Bold();
                        inner.Item().Text($"Cliente: {_dto.ClienteNombre} (Doc: {_dto.ClienteDocumento})");
                        inner.Item().Text($"Vehículo: {_dto.VehiculoMarca} {_dto.VehiculoModelo} - Dominio: {_dto.VehiculoPatente}");
                        inner.Item().Text($"Fecha de Recepción: {_dto.FechaRecepcion:dd/MM/yyyy HH:mm}");
                        if (_dto.FechaEstimadaEntrega.HasValue)
                        {
                            inner.Item().Text($"Fecha Estimada Entrega: {_dto.FechaEstimadaEntrega:dd/MM/yyyy}");
                        }
                    });

                    col.Item().PaddingTop(20).Text("Detalle de Servicios Solicitados").FontSize(14).SemiBold().FontColor(Colors.Blue.Medium);
                    
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3); // Servicio
                            columns.RelativeColumn(4); // Observaciones
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(x => x.CeldaCabecera()).Text("Servicio").SemiBold();
                            header.Cell().Element(x => x.CeldaCabecera()).Text("Observaciones").SemiBold();
                        });

                        foreach (var item in _dto.Detalles)
                        {
                            table.Cell().Element(x => x.CeldaDato()).Text(item.ProductoOServicio);
                            table.Cell().Element(x => x.CeldaDato()).Text(string.IsNullOrWhiteSpace(item.Observacion) ? "-" : item.Observacion).FontColor(Colors.Grey.Darken2);
                        }
                    });

                    col.Item().PaddingTop(15).Text("Observaciones Generales:").SemiBold();
                    col.Item().Text(string.IsNullOrWhiteSpace(_dto.Observaciones) ? "Sin observaciones particulares en la recepción." : _dto.Observaciones);
                });

                page.Footer().Element(x => x.ComposePieDePagina());
            });
        }

        private void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text(_dto.TallerNombre).FontSize(24).SemiBold().FontColor(Colors.Blue.Darken2);
                    col.Item().Text(_dto.TallerDireccion).FontSize(10).FontColor(Colors.Grey.Darken1);
                    col.Item().Text($"Tel: {_dto.TallerTelefono} | Email: contacto@taller.com").FontSize(10).FontColor(Colors.Grey.Darken1);
                });

                row.ConstantItem(150).AlignRight().Column(col =>
                {
                    col.Item().Text("ORDEN DE SERVICIO").FontSize(16).Bold().FontColor(Colors.Black);
                    col.Item().Text($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(10);
                });
            });
        }
    }
}
