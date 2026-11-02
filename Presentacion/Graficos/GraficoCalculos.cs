// walterg20
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TP_ControlVehicular.Negocio.DTOs;

namespace TP_ControlVehicular.Presentacion.Graficos
{
    /// <summary>
    /// Logica de calculo de los graficos del dashboard, aislada de WPF para poder
    /// probarla sin hilos STA.
    /// Es el equivalente WPF de los generadores SVG que se usan en los PDF
    /// (<c>IngresosBarChartGenerator</c>, <c>ModelosBarChartGenerator</c> y
    /// <c>ServiciosPieChartGenerator</c>), y comparte con ellos la paleta y el
    /// formato abreviado de montos.
    /// </summary>
    public static class GraficoCalculos
    {
        /// <summary>Paleta de la torta, identica a la de los PDF.</summary>
        public static readonly string[] PaletaTorta =
        {
            "#2980B9", "#27AE60", "#F39C12", "#8E44AD", "#E74C3C",
            "#16A085", "#D35400", "#2C3E50", "#7F8C8D", "#C0392B"
        };

        /// <summary>Color de las barras.</summary>
        public const string ColorBarra = "#2980B9";

        /// <summary>Color de las lineas de guia.</summary>
        public const string ColorLineaGuia = "#EAEDED";

        /// <summary>Color del eje.</summary>
        public const string ColorEje = "#BDC3C7";

        /// <summary>Color del texto principal.</summary>
        public const string ColorTexto = "#2C3E50";

        /// <summary>Color del texto secundario.</summary>
        public const string ColorSubtexto = "#7F8C8D";

        /// <summary>Escala del eje vertical de un grafico de barras.</summary>
        public sealed class Escala
        {
            /// <summary>Valor maximo de la serie.</summary>
            public decimal Maximo { get; init; }

            /// <summary>Maximo de la escala (el maximo con un margen del 15%).</summary>
            public double MaximoEscala { get; init; }

            /// <summary>Valores de los cortes de la escala, de abajo hacia arriba.</summary>
            public IReadOnlyList<double> Cortes { get; init; } = Array.Empty<double>();

            /// <summary>Proporcion de la altura ocupada por un valor, en el rango 0..1.</summary>
            public double Proporcion(decimal valor)
            {
                if (MaximoEscala <= 0) return 0;
                return Math.Clamp((double)valor / MaximoEscala, 0, 1);
            }
        }

        /// <summary>Porcion de una torta.</summary>
        public sealed class Sector
        {
            /// <summary>Etiqueta de la porcion.</summary>
            public string Etiqueta { get; init; } = string.Empty;

            /// <summary>Valor de la porcion.</summary>
            public decimal Valor { get; init; }

            /// <summary>Porcentaje sobre el total (0..100).</summary>
            public double Porcentaje { get; init; }

            /// <summary>Angulo inicial en grados, medidos desde las 12 en punto y en sentido horario.</summary>
            public double AnguloInicio { get; init; }

            /// <summary>Angulo final en grados, medidos desde las 12 en punto y en sentido horario.</summary>
            public double AnguloFin { get; init; }

            /// <summary>Verdadero si la porcion abarca mas de media vuelta.</summary>
            public bool EsPorcionGrande => (AnguloFin - AnguloInicio) > 180.0;
        }

        /// <summary>
        /// Calcula la escala del eje vertical a partir de los valores de la serie.
        /// </summary>
        /// <param name="valores">Valores de la serie.</param>
        /// <param name="cantidadCortes">Cantidad de divisiones de la escala.</param>
        public static Escala CalcularEscala(IEnumerable<decimal> valores, int cantidadCortes = 4)
        {
            var lista = (valores ?? Enumerable.Empty<decimal>()).Where(v => v >= 0).ToList();

            if (cantidadCortes < 2) cantidadCortes = 2;

            var maximo = lista.Count > 0 ? lista.Max() : 0m;
            var maximoEscala = maximo > 0 ? (double)maximo * 1.15 : 100.0;

            var cortes = new List<double>(cantidadCortes);
            for (var i = 0; i < cantidadCortes; i++)
            {
                cortes.Add(maximoEscala * (i / (double)(cantidadCortes - 1)));
            }

            return new Escala
            {
                Maximo = maximo,
                MaximoEscala = maximoEscala,
                Cortes = cortes
            };
        }

        /// <summary>
        /// Formatea un monto en forma abreviada, con las mismas reglas que los PDF.
        /// </summary>
        public static string FormatearMonto(decimal monto)
        {
            var valor = (double)monto;

            if (valor >= 1_000_000)
                return $"${(valor / 1_000_000).ToString("0.#", CultureInfo.InvariantCulture)}M";
            if (valor >= 10_000)
                return $"${(valor / 1_000).ToString("0.#", CultureInfo.InvariantCulture)}k";
            return $"${valor.ToString("N0", CultureInfo.InvariantCulture)}";
        }

        /// <summary>
        /// Formatea el valor de una serie: como monto abreviado o como numero con separador.
        /// </summary>
        public static string FormatearValor(decimal valor, bool esMonto) =>
            esMonto ? FormatearMonto(valor) : valor.ToString("N0", CultureInfo.InvariantCulture);

        /// <summary>
        /// Agrupa la serie en los <paramref name="maxSegmentos"/> elementos mas grandes y
        /// suma el resto bajo la etiqueta "Otros" (misma regla que la torta de los PDF).
        /// </summary>
        public static IReadOnlyList<DashboardSerieDto> AgruparTopN(
            IEnumerable<DashboardSerieDto> serie, int maxSegmentos)
        {
            var datos = (serie ?? Enumerable.Empty<DashboardSerieDto>())
                .Where(d => d.Valor > 0)
                .OrderByDescending(d => d.Valor)
                .ToList();

            if (maxSegmentos < 1) maxSegmentos = 1;

            if (datos.Count > maxSegmentos)
            {
                var resto = datos.Skip(maxSegmentos - 1).Sum(d => d.Valor);
                datos = datos.Take(maxSegmentos - 1).ToList();
                datos.Add(new DashboardSerieDto { Etiqueta = "Otros", Valor = resto });
            }

            return datos;
        }

        /// <summary>
        /// Calcula las porciones de la torta con sus ángulos.
        /// </summary>
        public static IReadOnlyList<Sector> CalcularSectores(IEnumerable<DashboardSerieDto> serie)
        {
            var datos = (serie ?? Enumerable.Empty<DashboardSerieDto>())
                .Where(d => d.Valor > 0)
                .OrderByDescending(d => d.Valor)
                .ToList();

            var total = datos.Sum(d => d.Valor);
            if (total <= 0) return Array.Empty<Sector>();

            var sectores = new List<Sector>(datos.Count);
            var angulo = 0.0;

            foreach (var dato in datos)
            {
                var barrido = (double)dato.Valor / (double)total * 360.0;
                sectores.Add(new Sector
                {
                    Etiqueta = dato.Etiqueta,
                    Valor = dato.Valor,
                    Porcentaje = (double)dato.Valor / (double)total * 100.0,
                    AnguloInicio = angulo,
                    AnguloFin = angulo + barrido
                });
                angulo += barrido;
            }

            return sectores;
        }

        /// <summary>
        /// Calcula el punto de una circunferencia a partir de un ángulo medido desde las
        /// 12 en punto y en sentido horario (coordenadas de pantalla: la Y crece hacia abajo).
        /// </summary>
        public static (double X, double Y) PuntoEnCircunferencia(
            double centroX, double centroY, double radio, double anguloGrados)
        {
            var radianes = anguloGrados * Math.PI / 180.0;
            return (
                centroX + radio * Math.Sin(radianes),
                centroY - radio * Math.Cos(radianes)
            );
        }

        /// <summary>
        /// Angulo barrido por el anillo de progreso, en grados (0..360).
        /// </summary>
        public static double CalcularAnguloBarrido(decimal porcentaje)
        {
            var acotado = Math.Clamp((double)porcentaje, 0, 100);
            return acotado * 3.6;
        }
    }
}
