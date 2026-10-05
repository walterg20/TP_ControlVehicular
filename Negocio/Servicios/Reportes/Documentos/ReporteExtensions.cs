using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace TP_ControlVehicular.Negocio.Servicios.Reportes.Documentos
{
    /// <summary>
    /// Componentes visuales reutilizables para cualquier reporte de la aplicacion
    /// </summary>
    public static class ReporteExtensions
    {
        public static void ComposeEncabezadoTaller(this IContainer container, string tituloReporte)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("TALLER MECÁNICO TP").FontSize(24).SemiBold().FontColor(Colors.Blue.Darken2);
                    col.Item().Text("Ruta Nacional 11 Km 1005 - Resistencia, Chaco").FontSize(10).FontColor(Colors.Grey.Darken1);
                    col.Item().Text("Tel: +54 362 4123456 | Email: contacto@taller.com").FontSize(10).FontColor(Colors.Grey.Darken1);
                });

                row.ConstantItem(150).AlignRight().Column(col =>
                {
                    col.Item().Text(tituloReporte).FontSize(16).Bold().FontColor(Colors.Black);
                    col.Item().Text($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(10);
                });
            });
        }

        public static void ComposePieDePagina(this IContainer container)
        {
            container.AlignCenter().Text(x =>
            {
                x.Span("Página ");
                x.CurrentPageNumber();
                x.Span(" de ");
                x.TotalPages();
                x.Span(" - Sistema de Control Vehicular");
            });
        }
        
        public static IContainer CeldaCabecera(this IContainer container)
        {
            return container.BorderBottom(1).BorderColor(Colors.Grey.Medium).PaddingVertical(5);
        }
        
        public static IContainer CeldaDato(this IContainer container)
        {
            return container.PaddingVertical(5);
        }
    }
}
