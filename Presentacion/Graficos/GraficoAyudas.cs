// walterg20
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TP_ControlVehicular.Presentacion.Graficos
{
    /// <summary>
    /// Ayudas de dibujo para los gráficos del dashboard (lienzo WPF).
    /// </summary>
    public static class GraficoAyudas
    {
        /// <summary>Convierte un color hexadecimal (#RRGGBB) en pincel.</summary>
        public static SolidColorBrush Pincel(string colorHex) =>
            new((Color)ColorConverter.ConvertFromString(colorHex));

        /// <summary>Crea un <see cref="TextBlock"/> posicionado en el lienzo.</summary>
        public static TextBlock CrearTexto(
            string texto,
            double x,
            double y,
            string colorHex,
            double fontSize = 8,
            bool bold = false,
            TextAlignment alignment = TextAlignment.Left,
            double ancho = double.NaN)
        {
            var block = new TextBlock
            {
                Text = texto,
                FontFamily = new FontFamily("Segoe UI, Helvetica, Arial, sans-serif"),
                FontSize = fontSize,
                FontWeight = bold ? FontWeights.Bold : FontWeights.Normal,
                Foreground = Pincel(colorHex),
                TextAlignment = alignment
            };

            if (!double.IsNaN(ancho))
            {
                block.Width = ancho;
            }

            Canvas.SetLeft(block, x);
            Canvas.SetTop(block, y);
            return block;
        }

        /// <summary>Crea una <see cref="Line"/> en el lienzo.</summary>
        public static Line CrearLinea(
            double x1, double y1, double x2, double y2,
            string colorHex, double grosor = 1, bool punteada = false)
        {
            var line = new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = Pincel(colorHex),
                StrokeThickness = grosor
            };

            if (punteada)
            {
                line.StrokeDashArray = new DoubleCollection { 3, 3 };
            }

            return line;
        }

        /// <summary>Crea un <see cref="Rectangle"/> en el lienzo.</summary>
        public static Rectangle CrearRectangulo(
            double x, double y, double ancho, double alto, string colorHex, double radio = 3)
        {
            var rect = new Rectangle
            {
                Width = Math.Max(0, ancho),
                Height = Math.Max(0, alto),
                RadiusX = radio,
                RadiusY = radio,
                Fill = Pincel(colorHex)
            };

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            return rect;
        }

        /// <summary>Devuelve el color de la paleta que corresponde al indice.</summary>
        public static string ColorPaleta(int indice, string[] paleta)
        {
            if (paleta.Length == 0) return GraficoCalculos.ColorBarra;
            return paleta[indice % paleta.Length];
        }

        /// <summary>Formatea un porcentaje sin decimales.</summary>
        public static string FormatearPorcentaje(double porcentaje) =>
            porcentaje.ToString("0", CultureInfo.InvariantCulture);
    }
}
