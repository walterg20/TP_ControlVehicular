// walterg20
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Presentacion.Graficos;

namespace TP_ControlVehicular.Presentacion.Pantalla.Compartido
{
    /// <summary>
    /// Grafico de torta con leyenda, dibujado con WPF nativo.
    /// La torta y la leyenda se dibujan en el mismo lienzo, de modo que la leyenda queda
    /// siempre pegada a la torta sin importar el ancho disponible.
    /// </summary>
    public partial class CtlGraficoTorta : UserControl
    {
        /// <summary>Cantidad maxima de porciones: el resto se agrupa bajo "Otros".</summary>
        public const int MaximoSegmentos = 8;

        private const double AltoLeyenda = 14.0;
        private const double MargenSuperior = 22.0;

        public static readonly DependencyProperty SerieProperty = DependencyProperty.Register(
            nameof(Serie), typeof(IEnumerable<DashboardSerieDto>), typeof(CtlGraficoTorta),
            new PropertyMetadata(null, OnSerieChanged));

        public static readonly DependencyProperty TituloProperty = DependencyProperty.Register(
            nameof(Titulo), typeof(string), typeof(CtlGraficoTorta),
            new PropertyMetadata(string.Empty, OnSerieChanged));

        public static readonly DependencyProperty EsMontoProperty = DependencyProperty.Register(
            nameof(EsMonto), typeof(bool), typeof(CtlGraficoTorta),
            new PropertyMetadata(false, OnSerieChanged));

        public CtlGraficoTorta()
        {
            InitializeComponent();
        }

        /// <summary>Serie de datos a representar.</summary>
        public IEnumerable<DashboardSerieDto>? Serie
        {
            get => (IEnumerable<DashboardSerieDto>?)GetValue(SerieProperty);
            set => SetValue(SerieProperty, value);
        }

        /// <summary>Titulo del grafico.</summary>
        public string Titulo
        {
            get => (string)GetValue(TituloProperty);
            set => SetValue(TituloProperty, value);
        }

        /// <summary>Verdadero si los valores se formatean como montos.</summary>
        public bool EsMonto
        {
            get => (bool)GetValue(EsMontoProperty);
            set => SetValue(EsMontoProperty, value);
        }

        private static void OnSerieChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtlGraficoTorta control) control.Dibujar();
        }

        private void Lienzo_SizeChanged(object sender, SizeChangedEventArgs e) => Dibujar();

        private void Dibujar()
        {
            if (Lienzo is null) return;

            Lienzo.Children.Clear();

            var ancho = Lienzo.ActualWidth;
            var alto = Lienzo.ActualHeight;

            if (ancho <= 0 || alto <= 0) return;

            if (!string.IsNullOrWhiteSpace(Titulo))
            {
                Lienzo.Children.Add(GraficoAyudas.CrearTexto(
                    Titulo, 0, 2, GraficoCalculos.ColorTexto, 10, true));
            }

            var datos = GraficoCalculos.AgruparTopN(Serie ?? Enumerable.Empty<DashboardSerieDto>(), MaximoSegmentos);
            var sectores = GraficoCalculos.CalcularSectores(datos);

            if (sectores.Count == 0)
            {
                Lienzo.Children.Add(GraficoAyudas.CrearTexto(
                    "Sin datos para mostrar", 0, alto / 2 - 8, GraficoCalculos.ColorSubtexto, 11,
                    false, TextAlignment.Center, ancho));
                return;
            }

            // La torta ocupa un cuadrado a la izquierda; la leyenda va a su derecha.
            var altoDisponible = alto - MargenSuperior;
            var ladoTorta = System.Math.Min(altoDisponible, Math.Max(60.0, ancho * 0.55));
            var radio = ladoTorta / 2.0 - 4;

            if (radio <= 0) return;

            var centroX = radio + 4;
            var centroY = MargenSuperior + altoDisponible / 2.0;

            for (var i = 0; i < sectores.Count; i++)
            {
                var sector = sectores[i];
                var color = GraficoAyudas.ColorPaleta(i, GraficoCalculos.PaletaTorta);

                Lienzo.Children.Add(CrearPorcion(sector, centroX, centroY, radio, color));
            }

            // Leyenda.
            var xLeyenda = centroX + radio + 14;
            var yLeyenda = centroY - (sectores.Count * AltoLeyenda) / 2.0;

            for (var i = 0; i < sectores.Count; i++)
            {
                var sector = sectores[i];
                var color = GraficoAyudas.ColorPaleta(i, GraficoCalculos.PaletaTorta);
                var y = yLeyenda + (i * AltoLeyenda);

                Lienzo.Children.Add(GraficoAyudas.CrearRectangulo(
                    xLeyenda, y + 1, 9, 9, color, 2));

                Lienzo.Children.Add(GraficoAyudas.CrearTexto(
                    Recortar(sector.Etiqueta, 18),
                    xLeyenda + 14, y,
                    GraficoCalculos.ColorTexto, 8));

                Lienzo.Children.Add(GraficoAyudas.CrearTexto(
                    $"{GraficoCalculos.FormatearValor(sector.Valor, EsMonto)} ({GraficoAyudas.FormatearPorcentaje(sector.Porcentaje)}%)",
                    xLeyenda + 118, y,
                    GraficoCalculos.ColorSubtexto, 8, true));
            }
        }

        /// <summary>Crea la forma de una porción de la torta.</summary>
        private static Shape CrearPorcion(
            GraficoCalculos.Sector sector, double centroX, double centroY, double radio, string color)
        {
            // Una unica porcion con el 100% se dibuja como elipse completa: el arco
            // degeneraria porque el punto inicial coincide con el final.
            if (sector.AnguloFin - sector.AnguloInicio >= 359.9)
            {
                var elipse = new Ellipse
                {
                    Width = radio * 2,
                    Height = radio * 2,
                    Fill = GraficoAyudas.Pincel(color),
                    Stroke = Brushes.White,
                    StrokeThickness = 1
                };
                Canvas.SetLeft(elipse, centroX - radio);
                Canvas.SetTop(elipse, centroY - radio);
                return elipse;
            }

            var inicio = GraficoCalculos.PuntoEnCircunferencia(centroX, centroY, radio, sector.AnguloInicio);
            var fin = GraficoCalculos.PuntoEnCircunferencia(centroX, centroY, radio, sector.AnguloFin);

            var figura = new PathFigure
            {
                StartPoint = new Point(centroX, centroY),
                IsClosed = true
            };
            figura.Segments.Add(new LineSegment(new Point(inicio.X, inicio.Y), true));
            figura.Segments.Add(new ArcSegment(
                new Point(fin.X, fin.Y),
                new Size(radio, radio),
                0,
                sector.EsPorcionGrande,
                SweepDirection.Clockwise,
                true));

            var geometria = new PathGeometry();
            geometria.Figures.Add(figura);

            return new Path
            {
                Data = geometria,
                Fill = GraficoAyudas.Pincel(color),
                Stroke = Brushes.White,
                StrokeThickness = 1
            };
        }

        /// <summary>Recorta la etiqueta para que no invada el area del grafico.</summary>
        private static string Recortar(string texto, int maximo) =>
            string.IsNullOrEmpty(texto) || texto.Length <= maximo
                ? texto
                : texto.Substring(0, maximo - 1) + "…";
    }
}
