// walterg20
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;
using TP_ControlVehicular.Negocio.DTOs.Reportes;

namespace TP_ControlVehicular.Negocio.Reportes.Documentos
{
    /// <summary>
    /// Genera un grafico de barras horizontales (en formato SVG) para representar
    /// el ranking de los modelos de vehiculos mas reparados en el reporte gerencial PDF.
    /// </summary>
    public static class ModelosBarChartGenerator
    {
        private const string ColorTop1 = "#27AE60";
        private const string ColorBarras = "#3498DB";
        private const string ColorEje = "#BDC3C7";
        private const string ColorLineaGuia = "#EAEDED";
        private const string ColorTexto = "#2C3E50";
        private const string ColorSubtexto = "#7F8C8D";

        /// <summary>
        /// Construye el documento SVG que contiene el grafico de barras horizontales de modelos.
        /// </summary>
        public static string BuildSvg(IReadOnlyList<ReporteModeloReparadoDto>? datos, double ancho = 500)
        {
            var serie = (datos ?? new List<ReporteModeloReparadoDto>())
                .Where(d => d.CantidadReparaciones > 0)
                .OrderByDescending(d => d.CantidadReparaciones)
                .Take(10)
                .ToList();

            var rowHeight = 24.0;
            var marginTop = 18.0;
            var marginBottom = 22.0;
            var alto = Math.Max(130.0, serie.Count * rowHeight + marginTop + marginBottom);

            var sb = new StringBuilder();
            sb.Append("<svg xmlns=\"http://www.w3.org/2000/svg\"");
            sb.Append($" width=\"{F(ancho)}\" height=\"{F(alto)}\" viewBox=\"0 0 {F(ancho)} {F(alto)}\">");
            sb.Append($"<rect x=\"0\" y=\"0\" width=\"{F(ancho)}\" height=\"{F(alto)}\" fill=\"#FFFFFF\" rx=\"6\" ry=\"6\"/>");

            if (serie.Count == 0)
            {
                sb.Append($"<text x=\"{F(ancho / 2)}\" y=\"{F(alto / 2)}\" text-anchor=\"middle\" ");
                sb.Append($"font-family=\"Segoe UI, Helvetica, Arial, sans-serif\" font-size=\"12\" fill=\"{ColorSubtexto}\">");
                sb.Append("Sin reparaciones registradas en el período");
                sb.Append("</text>");
                sb.Append("</svg>");
                return sb.ToString();
            }

            const double marginLeft = 165.0;
            const double marginRight = 55.0;
            var chartWidth = ancho - marginLeft - marginRight;

            var maxCantidad = (double)serie.Max(d => d.CantidadReparaciones);
            if (maxCantidad <= 0) maxCantidad = 1.0;

            // Lineas guias verticales
            for (var i = 1; i <= 3; i++)
            {
                var ratio = i / 3.0;
                var x = marginLeft + chartWidth * ratio;
                sb.Append($"<line x1=\"{F(x)}\" y1=\"{F(marginTop - 6)}\" x2=\"{F(x)}\" y2=\"{F(marginTop + serie.Count * rowHeight)}\" ");
                sb.Append($"stroke=\"{ColorLineaGuia}\" stroke-width=\"1\" stroke-dasharray=\"3,3\"/>");
            }

            // Eje vertical izquierdo
            sb.Append($"<line x1=\"{F(marginLeft)}\" y1=\"{F(marginTop - 6)}\" x2=\"{F(marginLeft)}\" y2=\"{F(marginTop + serie.Count * rowHeight)}\" ");
            sb.Append($"stroke=\"{ColorEje}\" stroke-width=\"1.5\"/>");

            for (var i = 0; i < serie.Count; i++)
            {
                var item = serie[i];
                var y = marginTop + (i * rowHeight);
                var barHeight = 14.0;
                var barY = y + 2.0;

                var ratio = item.CantidadReparaciones / maxCantidad;
                var barWidth = Math.Max(4.0, ratio * chartWidth);
                var colorBarra = i == 0 ? ColorTop1 : ColorBarras;

                // Etiqueta del modelo (Y-axis)
                var nombreModelo = TruncarTexto(item.MarcaModelo, 24);
                var nombreEscapado = SecurityElement.Escape(nombreModelo) ?? string.Empty;

                sb.Append($"<text x=\"{F(marginLeft - 8)}\" y=\"{F(barY + 11)}\" text-anchor=\"end\" ");
                sb.Append($"font-family=\"Segoe UI, Helvetica, Arial, sans-serif\" font-size=\"9\" font-weight=\"{(i == 0 ? "bold" : "normal")}\" fill=\"{ColorTexto}\">");
                sb.Append(nombreEscapado);
                sb.Append("</text>");

                // Barra horizontal
                sb.Append($"<rect x=\"{F(marginLeft)}\" y=\"{F(barY)}\" width=\"{F(barWidth)}\" height=\"{F(barHeight)}\" ");
                sb.Append($"rx=\"3\" ry=\"3\" fill=\"{colorBarra}\"/>");

                // Etiqueta numérica a la derecha de la barra
                sb.Append($"<text x=\"{F(marginLeft + barWidth + 6)}\" y=\"{F(barY + 11)}\" text-anchor=\"start\" ");
                sb.Append($"font-family=\"Segoe UI, Helvetica, Arial, sans-serif\" font-size=\"8.5\" font-weight=\"bold\" fill=\"{ColorTexto}\">");
                sb.Append($"{item.CantidadReparaciones} rep.");
                sb.Append("</text>");
            }

            sb.Append("</svg>");
            return sb.ToString();
        }

        private static string TruncarTexto(string? texto, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(texto)) return "Desconocido";
            var limpio = texto.Trim();
            return limpio.Length <= maxLength ? limpio : limpio.Substring(0, maxLength - 2) + "..";
        }

        private static string F(double value) => value.ToString("0.###", CultureInfo.InvariantCulture);
    }
}
