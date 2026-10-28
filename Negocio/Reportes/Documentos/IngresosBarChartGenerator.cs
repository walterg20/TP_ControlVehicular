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
    /// Genera un grafico de barras verticales (en formato SVG) para representar
    /// la evolucion de los ingresos totales por fecha en el reporte gerencial PDF.
    /// </summary>
    public static class IngresosBarChartGenerator
    {
        private const string ColorBarra = "#2980B9";
        private const string ColorEje = "#BDC3C7";
        private const string ColorLineaGuia = "#EAEDED";
        private const string ColorTexto = "#2C3E50";
        private const string ColorSubtexto = "#7F8C8D";

        /// <summary>
        /// Construye el documento SVG que contiene el grafico de barras verticales de ingresos por fecha.
        /// </summary>
        public static string BuildSvg(IReadOnlyList<ReporteIngresoDto>? datos, double ancho = 500, double alto = 190)
        {
            var sb = new StringBuilder();
            sb.Append("<svg xmlns=\"http://www.w3.org/2000/svg\"");
            sb.Append($" width=\"{F(ancho)}\" height=\"{F(alto)}\" viewBox=\"0 0 {F(ancho)} {F(alto)}\">");
            sb.Append($"<rect x=\"0\" y=\"0\" width=\"{F(ancho)}\" height=\"{F(alto)}\" fill=\"#FFFFFF\" rx=\"6\" ry=\"6\"/>");

            var serie = (datos ?? new List<ReporteIngresoDto>())
                .Where(d => d.IngresosTotales >= 0)
                .OrderBy(d => d.Fecha)
                .ToList();

            if (serie.Count == 0 || serie.All(d => d.IngresosTotales <= 0))
            {
                sb.Append($"<text x=\"{F(ancho / 2)}\" y=\"{F(alto / 2)}\" text-anchor=\"middle\" ");
                sb.Append($"font-family=\"Segoe UI, Helvetica, Arial, sans-serif\" font-size=\"12\" fill=\"{ColorSubtexto}\">");
                sb.Append("Sin ingresos registrados en el período");
                sb.Append("</text>");
                sb.Append("</svg>");
                return sb.ToString();
            }

            const double marginLeft = 60.0;
            const double marginRight = 20.0;
            const double marginTop = 22.0;
            const double marginBottom = 34.0;

            var chartWidth = ancho - marginLeft - marginRight;
            var chartHeight = alto - marginTop - marginBottom;

            var maxValor = (double)serie.Max(d => d.IngresosTotales);
            if (maxValor <= 0) maxValor = 100.0;
            var maxEscala = maxValor * 1.15; // Margen superior para las etiquetas

            // Lineas de guia horizontales y escala en eje Y (4 divisiones)
            for (var i = 0; i <= 3; i++)
            {
                var ratio = i / 3.0;
                var valorNivel = maxEscala * ratio;
                var y = marginTop + chartHeight - (chartHeight * ratio);

                sb.Append($"<line x1=\"{F(marginLeft)}\" y1=\"{F(y)}\" x2=\"{F(marginLeft + chartWidth)}\" y2=\"{F(y)}\" ");
                sb.Append($"stroke=\"{ColorLineaGuia}\" stroke-width=\"1\" stroke-dasharray=\"3,3\"/>");

                sb.Append($"<text x=\"{F(marginLeft - 8)}\" y=\"{F(y + 3)}\" text-anchor=\"end\" ");
                sb.Append($"font-family=\"Segoe UI, Helvetica, Arial, sans-serif\" font-size=\"8\" fill=\"{ColorSubtexto}\">");
                sb.Append(FormatearMonto(valorNivel));
                sb.Append("</text>");
            }

            // Eje X e Y base
            sb.Append($"<line x1=\"{F(marginLeft)}\" y1=\"{F(marginTop + chartHeight)}\" x2=\"{F(marginLeft + chartWidth)}\" y2=\"{F(marginTop + chartHeight)}\" stroke=\"{ColorEje}\" stroke-width=\"1.5\"/>");
            sb.Append($"<line x1=\"{F(marginLeft)}\" y1=\"{F(marginTop)}\" x2=\"{F(marginLeft)}\" y2=\"{F(marginTop + chartHeight)}\" stroke=\"{ColorEje}\" stroke-width=\"1.5\"/>");

            // Dibujo de barras
            var count = serie.Count;
            var step = chartWidth / count;
            var barWidth = Math.Clamp(step * 0.65, 6.0, 36.0);
            var mostrarValores = count <= 14;

            for (var i = 0; i < count; i++)
            {
                var item = serie[i];
                var x = marginLeft + (i * step) + ((step - barWidth) / 2.0);
                var barHeight = Math.Max(2.0, ((double)item.IngresosTotales / maxEscala) * chartHeight);
                var y = marginTop + chartHeight - barHeight;

                // Barra
                sb.Append($"<rect x=\"{F(x)}\" y=\"{F(y)}\" width=\"{F(barWidth)}\" height=\"{F(barHeight)}\" ");
                sb.Append($"rx=\"3\" ry=\"3\" fill=\"{ColorBarra}\"/>");

                // Monto sobre la barra
                if (mostrarValores && item.IngresosTotales > 0)
                {
                    sb.Append($"<text x=\"{F(x + barWidth / 2.0)}\" y=\"{F(Math.Max(marginTop + 8.0, y - 4.0))}\" text-anchor=\"middle\" ");
                    sb.Append($"font-family=\"Segoe UI, Helvetica, Arial, sans-serif\" font-size=\"8\" font-weight=\"bold\" fill=\"{ColorTexto}\">");
                    sb.Append(FormatearMonto(item.IngresosTotales));
                    sb.Append("</text>");
                }

                // Etiqueta de fecha en eje X
                sb.Append($"<text x=\"{F(x + barWidth / 2.0)}\" y=\"{F(marginTop + chartHeight + 14.0)}\" text-anchor=\"middle\" ");
                sb.Append($"font-family=\"Segoe UI, Helvetica, Arial, sans-serif\" font-size=\"8\" fill=\"{ColorTexto}\">");
                sb.Append(item.Fecha.ToString("dd/MM"));
                sb.Append("</text>");
            }

            sb.Append("</svg>");
            return sb.ToString();
        }

        private static string FormatearMonto(double monto)
        {
            if (monto >= 1_000_000)
                return $"${(monto / 1_000_000).ToString("0.#", CultureInfo.InvariantCulture)}M";
            if (monto >= 10_000)
                return $"${(monto / 1_000).ToString("0.#", CultureInfo.InvariantCulture)}k";
            return $"${monto.ToString("N0", CultureInfo.InvariantCulture)}";
        }

        private static string FormatearMonto(decimal monto) => FormatearMonto((double)monto);

        private static string F(double value) => value.ToString("0.###", CultureInfo.InvariantCulture);
    }
}
