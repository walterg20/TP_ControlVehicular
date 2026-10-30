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
    /// la evolucion de los ingresos totales en el reporte gerencial PDF.
    /// </summary>
    /// <remarks>
    /// La agregacion es automatica: si el rango de fechas abarca mas de un mes calendario
    /// el grafico agrupa por periodo mensual (para que sea coherente con el resumen por mes
    /// que se imprime justo debajo); si el rango cubre un solo mes, mantiene el detalle
    /// diario, porque un unico barra mensual no aportaria informacion.
    /// </remarks>
    public static class IngresosBarChartGenerator
    {
        private const string ColorBarra = "#2980B9";
        private const string ColorEje = "#BDC3C7";
        private const string ColorLineaGuia = "#EAEDED";
        private const string ColorTexto = "#2C3E50";
        private const string ColorSubtexto = "#7F8C8D";

        private const string TituloDiario = "Ingresos por dia";
        private const string TituloMensual = "Ingresos por mes";

        /// <summary>
        /// Construye el documento SVG que contiene el grafico de barras verticales de ingresos por fecha.
        /// </summary>
        public static string BuildSvg(IReadOnlyList<ReporteIngresoDto>? datos, double ancho = 500, double alto = 190)
        {
            var serie = (datos ?? new List<ReporteIngresoDto>())
                .Where(d => d.IngresosTotales >= 0)
                .OrderBy(d => d.Fecha)
                .Select(d => (Etiqueta: d.Fecha.ToString("dd/MM"), Valor: d.IngresosTotales))
                .ToList();

            return ConstruirSvg(serie, ancho, alto, TituloDiario);
        }

        /// <summary>
        /// Construye el grafico de barras verticales agrupado por periodo mensual.
        /// </summary>
        public static string BuildSvgPorMeses(IReadOnlyList<ReporteIngresoMensualDto>? datos, double ancho = 500, double alto = 190)
        {
            var serie = (datos ?? new List<ReporteIngresoMensualDto>())
                .Where(d => d.IngresosTotales >= 0)
                .OrderBy(d => d.FechaMes)
                .Select(d => (Etiqueta: FormatearPeriodo(d), Valor: d.IngresosTotales))
                .ToList();

            return ConstruirSvg(serie, ancho, alto, TituloMensual);
        }

        /// <summary>
        /// Elige la agregacion segun el rango de fechas: por periodo mensual cuando abarca
        /// mas de un mes calendario y por dia cuando cubre un solo mes.
        /// </summary>
        public static string BuildSvg(
            IReadOnlyList<ReporteIngresoDto>? ingresosDiarios,
            IReadOnlyList<ReporteIngresoMensualDto>? ingresosMensuales,
            double ancho = 500, double alto = 190)
        {
            if (UsaAgregacionMensual(ingresosDiarios, ingresosMensuales))
            {
                return BuildSvgPorMeses(ingresosMensuales, ancho, alto);
            }

            return BuildSvg(ingresosDiarios, ancho, alto);
        }

        /// <summary>
        /// Indica si el grafico debe agrupar por periodo mensual.
        /// </summary>
        public static bool UsaAgregacionMensual(
            IReadOnlyList<ReporteIngresoDto>? ingresosDiarios,
            IReadOnlyList<ReporteIngresoMensualDto>? ingresosMensuales)
        {
            if (ingresosMensuales is null || ingresosMensuales.Count == 0) return false;
            return AbarcaMasDeUnMes(ingresosDiarios);
        }

        /// <summary>
        /// Indica si los datos diarios abarcan mas de un mes calendario.
        /// </summary>
        public static bool AbarcaMasDeUnMes(IReadOnlyList<ReporteIngresoDto>? datos)
        {
            if (datos is null || datos.Count == 0) return false;

            return datos
                .Select(d => (d.Fecha.Year, d.Fecha.Month))
                .Distinct()
                .Count() > 1;
        }

        private static string ConstruirSvg(
            IReadOnlyList<(string Etiqueta, decimal Valor)> serie,
            double ancho,
            double alto,
            string titulo)
        {
            var sb = new StringBuilder();
            sb.Append("<svg xmlns=\"http://www.w3.org/2000/svg\"");
            sb.Append($" width=\"{F(ancho)}\" height=\"{F(alto)}\" viewBox=\"0 0 {F(ancho)} {F(alto)}\">");
            sb.Append($"<rect x=\"0\" y=\"0\" width=\"{F(ancho)}\" height=\"{F(alto)}\" fill=\"#FFFFFF\" rx=\"6\" ry=\"6\"/>");

            if (serie.Count == 0 || serie.All(d => d.Valor <= 0))
            {
                sb.Append($"<text x=\"{F(ancho / 2)}\" y=\"{F(alto / 2)}\" text-anchor=\"middle\" ");
                sb.Append($"font-family=\"Segoe UI, Helvetica, Arial, sans-serif\" font-size=\"12\" fill=\"{ColorSubtexto}\">");
                sb.Append("Sin ingresos registrados en el per��odo");
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

            var maxValor = (double)serie.Max(d => d.Valor);
            if (maxValor <= 0) maxValor = 100.0;
            var maxEscala = maxValor * 1.15; // Margen superior para las etiquetas

            // Titulo del grafico (indica la agregacion aplicada)
            sb.Append($"<text x=\"{F(marginLeft)}\" y=\"13\" ");
            sb.Append($"font-family=\"Segoe UI, Helvetica, Arial, sans-serif\" font-size=\"9\" font-weight=\"bold\" fill=\"{ColorTexto}\">");
            sb.Append(titulo);
            sb.Append("</text>");

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
                var barHeight = Math.Max(2.0, ((double)item.Valor / maxEscala) * chartHeight);
                var y = marginTop + chartHeight - barHeight;

                // Barra
                sb.Append($"<rect x=\"{F(x)}\" y=\"{F(y)}\" width=\"{F(barWidth)}\" height=\"{F(barHeight)}\" ");
                sb.Append($"rx=\"3\" ry=\"3\" fill=\"{ColorBarra}\"/>");

                // Monto sobre la barra
                if (mostrarValores && item.Valor > 0)
                {
                    sb.Append($"<text x=\"{F(x + barWidth / 2.0)}\" y=\"{F(Math.Max(marginTop + 8.0, y - 4.0))}\" text-anchor=\"middle\" ");
                    sb.Append($"font-family=\"Segoe UI, Helvetica, Arial, sans-serif\" font-size=\"8\" font-weight=\"bold\" fill=\"{ColorTexto}\">");
                    sb.Append(FormatearMonto(item.Valor));
                    sb.Append("</text>");
                }

                // Etiqueta del eje X
                sb.Append($"<text x=\"{F(x + barWidth / 2.0)}\" y=\"{F(marginTop + chartHeight + 14.0)}\" text-anchor=\"middle\" ");
                sb.Append($"font-family=\"Segoe UI, Helvetica, Arial, sans-serif\" font-size=\"8\" fill=\"{ColorTexto}\">");
                sb.Append(item.Etiqueta);
                sb.Append("</text>");
            }

            sb.Append("</svg>");
            return sb.ToString();
        }

        /// <summary>
        /// Etiqueta del eje X para un periodo mensual: "MM/aa".
        /// </summary>
        private static string FormatearPeriodo(ReporteIngresoMensualDto dto)
        {
            if (dto.FechaMes > DateTime.MinValue)
            {
                return dto.FechaMes.ToString("MM/yy", CultureInfo.InvariantCulture);
            }

            if (DateTime.TryParseExact(dto.PeriodoMes, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
            {
                return fecha.ToString("MM/yy", CultureInfo.InvariantCulture);
            }

            return dto.PeriodoMes;
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
