// walterg20
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TP_ControlVehicular.Presentacion.Graficos;

namespace TP_ControlVehicular.Presentacion.Pantalla.Compartido
{
    /// <summary>
    /// Anillo de progreso (avance) dibujado con WPF nativo.
    /// </summary>
    public partial class CtlGraficoAnilloProgreso : UserControl
    {
        private const double Grosor = 14.0;

        public static readonly DependencyProperty PorcentajeProperty = DependencyProperty.Register(
            nameof(Porcentaje), typeof(double), typeof(CtlGraficoAnilloProgreso),
            new PropertyMetadata(0.0, OnPorcentajeChanged));

        public static readonly DependencyProperty TituloProperty = DependencyProperty.Register(
            nameof(Titulo), typeof(string), typeof(CtlGraficoAnilloProgreso),
            new PropertyMetadata(string.Empty, OnPorcentajeChanged));

        public static readonly DependencyProperty TextoCentralProperty = DependencyProperty.Register(
            nameof(TextoCentral), typeof(string), typeof(CtlGraficoAnilloProgreso),
            new PropertyMetadata(null, OnPorcentajeChanged));

        public CtlGraficoAnilloProgreso()
        {
            InitializeComponent();
        }

        /// <summary>Porcentaje de avance (0..100).</summary>
        public double Porcentaje
        {
            get => (double)GetValue(PorcentajeProperty);
            set => SetValue(PorcentajeProperty, value);
        }

        /// <summary>Titulo sobre el anillo.</summary>
        public string Titulo
        {
            get => (string)GetValue(TituloProperty);
            set => SetValue(TituloProperty, value);
        }

        /// <summary>Texto del centro. Si es null se muestra el porcentaje.</summary>
        public string? TextoCentral
        {
            get => (string?)GetValue(TextoCentralProperty);
            set => SetValue(TextoCentralProperty, value);
        }

        private static void OnPorcentajeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtlGraficoAnilloProgreso control) control.Dibujar();
        }

        private void Lienzo_SizeChanged(object sender, SizeChangedEventArgs e) => Dibujar();

        private void Dibujar()
        {
            if (Lienzo is null || TxtTitulo is null) return;

            Lienzo.Children.Clear();
            TxtTitulo.Text = Titulo;

            var ancho = Lienzo.ActualWidth;
            var alto = Lienzo.ActualHeight;

            if (ancho <= 0 || alto <= 0) return;

            var radio = System.Math.Min(ancho, alto) / 2.0 - (Grosor / 2.0) - 2;
            if (radio <= 0) return;

            var centroX = ancho / 2.0;
            var centroY = alto / 2.0;

            // Anillo de fondo.
            var fondo = new Ellipse
            {
                Width = radio * 2,
                Height = radio * 2,
                Stroke = GraficoAyudas.Pincel("#EAEDED"),
                StrokeThickness = Grosor,
                Fill = Brushes.Transparent
            };
            Canvas.SetLeft(fondo, centroX - radio);
            Canvas.SetTop(fondo, centroY - radio);
            Lienzo.Children.Add(fondo);

            // Arco de avance.
            var angulo = GraficoCalculos.CalcularAnguloBarrido((decimal)Porcentaje);

            if (angulo > 0.1)
            {
                var inicio = GraficoCalculos.PuntoEnCircunferencia(centroX, centroY, radio, 0);
                var fin = GraficoCalculos.PuntoEnCircunferencia(centroX, centroY, radio, angulo);

                var figura = new PathFigure { StartPoint = new Point(inicio.X, inicio.Y) };
                figura.Segments.Add(new ArcSegment(
                    new Point(fin.X, fin.Y),
                    new Size(radio, radio),
                    0,
                    angulo > 180.0,
                    SweepDirection.Clockwise,
                    true));

                var geometria = new PathGeometry();
                geometria.Figures.Add(figura);

                var arco = new Path
                {
                    Data = geometria,
                    Stroke = GraficoAyudas.Pincel(GraficoCalculos.ColorBarra),
                    StrokeThickness = Grosor,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round,
                    Fill = Brushes.Transparent
                };
                Lienzo.Children.Add(arco);
            }

            // Texto central.
            var texto = string.IsNullOrWhiteSpace(TextoCentral)
                ? $"{GraficoAyudas.FormatearPorcentaje(Porcentaje)}%"
                : TextoCentral!;

            Lienzo.Children.Add(GraficoAyudas.CrearTexto(
                texto, 0, centroY - 14, GraficoCalculos.ColorTexto, 22, true,
                TextAlignment.Center, ancho));
        }
    }
}
