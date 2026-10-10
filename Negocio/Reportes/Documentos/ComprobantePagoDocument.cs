using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TP_ControlVehicular.Negocio.DTOs.Reportes;

namespace TP_ControlVehicular.Negocio.Reportes.Documentos
{
    public class ComprobantePagoDocument : IDocument
    {
        private readonly ComprobantePagoDto _dto;

        public ComprobantePagoDocument(ComprobantePagoDto dto)
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
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }

        private void ComposeHeader(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().Row(row =>
                {
                    // Logo de la empresa a la izquierda (regla: todo PDF lleva logo)
                    row.ConstantItem(100).PaddingRight(10).AlignLeft().Element(c => c.ComposeLogo());

                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("COMPROBANTE DE PAGO").FontSize(20).Bold();
                        c.Item().Text($"#{_dto.OrdenId:D5}  -  {_dto.FechaPago:dd MMM yyyy}").FontSize(14).SemiBold();
                    });

                    row.ConstantItem(250).AlignRight().Column(c =>
                    {
                        c.Item().Text(_dto.TallerNombre).AlignRight();
                        c.Item().Text(_dto.TallerDireccion).AlignRight();
                        c.Item().Text(_dto.TallerTelefono).AlignRight();
                        c.Item().Text("walterg20@gmail.com").AlignRight(); // Hardcoded from image
                    });
                });
                
                col.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Black);
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.Column(col =>
            {
                // Client and Vehicle Info
                col.Item().Row(row =>
                {
                    // Cliente
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text(_dto.ClienteNombre).Bold().FontSize(12);
                        c.Item().Text($"DNI: {_dto.ClienteDocumento}");
                        c.Item().Text($"Teléfono: {_dto.ClienteTelefono}");
                        c.Item().Text($"Dirección: {_dto.ClienteDireccion}");
                        c.Item().Text($"Email: {_dto.ClienteEmail}");
                    });

                    // Vehiculo
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text(_dto.VehiculoDescripcion);
                        c.Item().Text($"Matrícula: {_dto.VehiculoPatente}");
                        c.Item().Text($"Color: NO DEFINIDO"); // Not in DB
                        c.Item().Text($"VIN: NO DEFINIDO"); // Not in DB
                        c.Item().Text($"Nº Motor: NO DEFINIDO"); // Not in DB
                        c.Item().Text($"Kilometraje: {_dto.Kilometraje}");
                        c.Item().Text($"Nivel de combustible: 0%");
                    });
                });

                col.Item().PaddingVertical(15).LineHorizontal(1).LineColor(Colors.Black);

                // Table
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3); // Producto o servicio
                        columns.RelativeColumn(1); // Precio
                        columns.RelativeColumn(1); // Cant
                        columns.RelativeColumn(1); // Total
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Producto o servicio").SemiBold();
                        header.Cell().Text("Precio").SemiBold();
                        header.Cell().Text("Cant").SemiBold();
                        header.Cell().AlignRight().Text("Total").SemiBold();

                        header.Cell().ColumnSpan(4).PaddingTop(5).PaddingBottom(10).LineHorizontal(1).LineColor(Colors.Black);
                    });

                    foreach (var item in _dto.Detalles)
                    {
                        table.Cell().PaddingBottom(5).Text(item.ProductoOServicio);
                        table.Cell().PaddingBottom(5).Text($"${item.Precio:N0}");
                        table.Cell().PaddingBottom(5).Text(item.Cantidad.ToString());
                        table.Cell().PaddingBottom(5).AlignRight().Text($"${item.Subtotal:N0}");
                    }
                });

                col.Item().PaddingVertical(15).LineHorizontal(1).LineColor(Colors.Black);

                // Totals
                col.Item().Row(row =>
                {
                    row.RelativeItem(); // Espacio en blanco a la izquierda
                    row.ConstantItem(250).Column(c =>
                    {
                        c.Item().Row(r => { r.RelativeItem().Text("Repuestos:"); r.ConstantItem(80).Text($"${_dto.TotalRepuestos:N0}"); });
                        c.Item().Row(r => { r.RelativeItem().Text("Mano de obra:"); r.ConstantItem(80).Text($"${_dto.TotalManoObra:N0}"); });
                        c.Item().PaddingVertical(5);
                        c.Item().Row(r => { r.RelativeItem().Text("Subtotal:"); r.ConstantItem(80).Text($"${_dto.Subtotal:N0}"); });
                        c.Item().Row(r => { r.RelativeItem().Text("IVA:"); r.ConstantItem(80).Text($"${_dto.Iva:N0}"); });
                        c.Item().Row(r => { r.RelativeItem().Text("Total").Bold().FontSize(12); r.ConstantItem(80).Text($"${_dto.TotalGeneral:N0}").Bold().FontSize(12); });
                        
                        c.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Black);
                        
                        var pagadoAmount = _dto.EstadoOrden == "Pagada" ? _dto.TotalGeneral : 0;
                        var saldoAmount = _dto.TotalGeneral - pagadoAmount;
                        c.Item().Row(r => { r.RelativeItem().Text("Pagado:"); r.ConstantItem(80).Text($"${pagadoAmount:N0}"); });
                        c.Item().Row(r => { r.RelativeItem().Text("Saldo pendiente:"); r.ConstantItem(80).Text($"${saldoAmount:N0}"); });
                    });
                });

                // Signatures
                col.Item().PaddingTop(50).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Firma del técnico").FontColor(Colors.Grey.Medium);
                        c.Item().Text("walter velazco").Bold();
                    });
                    
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Firma del cliente").FontColor(Colors.Grey.Medium);
                        c.Item().Text(_dto.ClienteNombre).Bold();
                    });
                });
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().LineHorizontal(1).LineColor(Colors.Black);
                col.Item().PaddingTop(5).AlignCenter().Text($"{_dto.TallerDireccion} | {_dto.TallerTelefono} | walterg20@gmail.com").FontSize(9);
            });
        }
    }
}


