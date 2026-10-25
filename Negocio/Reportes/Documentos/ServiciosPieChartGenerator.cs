using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TP_ControlVehicular.Negocio.Reportes.Documentos
{
    /// <summary>
    /// Genera el grafico de torta (en SVG) con la cantidad de cada servicio dentro
    /// de un rango de fechas. Se usa unicamente en el PDF del reporte gerencial;
    /// la leyenda se compone aparte con elementos nativos de QuestPDF.
    /// </summary>
    public static class ServiciosPieChartGenerator
    {
        /// <summary>Paleta de colores reutilizada por la leyenda para mantener la correspondencia con cada porcion.</summary>
        public static readonly string[] Paleta =
        {
            "#2980B9", "#27AE60", "#F39C12", "#8E44AD", "#E74C3C",
            "#16A085", "#D35400", "#2C3E50", "#7F8C8D", "#C0392B"
        };

        /// <summary>
        /// Ordena de mayor a menor la cantidad y, si hay mas segmentos que
        /// <paramref name="maxSegmentos"/>, agrupa el resto bajo la etiqueta "Otros"
        /// para mantener el grafico legible.
        /// </summary>
        public static IReadOnlyList<(string Etiqueta, int Cantidad)> Normalizar(
            IReadOnlyList<(string Etiqueta, int Cantidad)> datos, int maxSegmentos = 10)
        {
            var series = (datos ?? new List<(string, int)>())
                .Where(d => d.Cantidad > 0)
                .OrderByDescending(d => d.Cantidad)
                .ToList();

            if (maxSegmentos < 1)
            {
                maxSegmentos = 1;
            }

            if (series.Count > maxSegmentos)
            {
                var resto = series.Skip(maxSegmentos - 1).Sum(d => d.Cantidad);
                series = series.Take(maxSegmentos - 1).ToList();
                series.Add(("Otros", resto));
            }

            return series;
        }

        /// <summary>Construye el SVG del grafico de torta. No incluye leyenda ni texto.</summary>
        public static string BuildSvg(IReadOnlyList<(string Etiqueta, int Cantidad)> series, double tamano = 280)
        {
            var total = series.Sum(s => s.Cantidad);
            var centro = tamano / 2.0;
            var radio = centro - 4.0;

            var sb = new StringBuilder();
            sb.Append("<svg xmlns=\"http://www.w3.org/2000/svg\"");
            sb.Append($" width=\"{F(tamano)}\" height=\"{F(tamano)}\" viewBox=\"0 0 {F(tamano)} {F(tamano)}\">");
            sb.Append($"<rect x=\"0\" y=\"0\" width=\"{F(tamano)}\" height=\"{F(tamano)}\" fill=\"#FFFFFF\"/>");

            if (total > 0)
            {
                if (series.Count == 1)
                {
                    sb.Append($"<circle cx=\"{F(centro)}\" cy=\"{F(centro)}\" r=\"{F(radio)}\" fill=\"{Paleta[0]}\"/>");
                }
                else
                {
                    var inicio = -90.0;
                    for (var i = 0; i < series.Count; i++)
                    {
                        var porcion = (double)series[i].Cantidad / total * 360.0;
                        var fin = inicio + porcion;
                        var color = Paleta[i % Paleta.Length];

                        var x0 = centro + radio * Math.Cos(inicio * Math.PI / 180.0);
                        var y0 = centro + radio * Math.Sin(inicio * Math.PI / 180.0);
                        var x1 = centro + radio * Math.Cos(fin * Math.PI / 180.0);
                        var y1 = centro + radio * Math.Sin(fin * Math.PI / 180.0);
                        var arcoGrande = porcion > 180.0 ? 1 : 0;

                        sb.Append($"<path d=\"M {F(centro)} {F(centro)} L {F(x0)} {F(y0)} ");
                        sb.Append($"A {F(radio)} {F(radio)} 0 {arcoGrande} 1 {F(x1)} {F(y1)} Z\" ");
                        sb.Append($"fill=\"{color}\" stroke=\"#FFFFFF\" stroke-width=\"1\"/>");

                        inicio = fin;
                    }
                }
            }
            else
            {
                sb.Append($"<circle cx=\"{F(centro)}\" cy=\"{F(centro)}\" r=\"{F(radio)}\" fill=\"#ECF0F1\"/>");
            }

            sb.Append("</svg>");
            return sb.ToString();
        }

        private static string F(double value) => value.ToString("0.###", CultureInfo.InvariantCulture);
    }
}
