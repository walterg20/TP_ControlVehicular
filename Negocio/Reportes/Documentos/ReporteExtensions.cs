// walterg20
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace TP_ControlVehicular.Negocio.Reportes.Documentos
{
    /// <summary>
    /// Componentes visuales reutilizables para cualquier reporte de la aplicacion
    /// </summary>
    public static class ReporteExtensions
    {
        /// <summary>
        /// Ruta del logo efectivo de la empresa. Punto unico de resolucion: delega en
        /// <see cref="EmpresaBranding.RutaLogo"/> (override del taller o logo empaquetado).
        /// </summary>
        public static string RutaLogo => EmpresaBranding.RutaLogo;

        /// <summary>
        /// Dibuja el logo de la empresa. Regla del proyecto: todo PDF debe mostrarlo en el encabezado.
        /// </summary>
        public static void ComposeLogo(this IContainer container)
        {
            if (System.IO.File.Exists(RutaLogo))
            {
                container.Image(RutaLogo);
            }
        }

        public static void ComposeEncabezadoTaller(this IContainer container, string tituloReporte)
        {
            container.Row(row =>
            {
                // Logo a la izquierda
                row.ConstantItem(120).PaddingRight(10).AlignLeft().Element(c => c.ComposeLogo());

                // Datos de la empresa al medio
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text(EmpresaBranding.Nombre).FontSize(24).SemiBold().FontColor(Colors.Blue.Darken2);
                    col.Item().Text(EmpresaBranding.Direccion).FontSize(10).FontColor(Colors.Grey.Darken1);
                    col.Item().Text($"Tel: {EmpresaBranding.Telefono} | Email: {EmpresaBranding.Email}").FontSize(10).FontColor(Colors.Grey.Darken1);
                });

                // Titulo del reporte a la derecha
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
