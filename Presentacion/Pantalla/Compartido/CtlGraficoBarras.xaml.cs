// walterg20
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Presentacion.Graficos;

namespace TP_ControlVehicular.Presentacion.Pantalla.Compartido
{
    /// <summary>
    /// Grafico de barras verticales dibujado con WPF nativo.
    /// </summary>
    public partial class CtlGraficoBarras : UserControl
    {
        private const double MargenIzquierdo = 58.0;
        private const double MargenDerecho = 12.0;
        private const double MargenSuperior = 24.0;
        private const double MargenInferior = 26.0;

        public static readonly DependencyProperty SerieProperty = DependencyProperty.Register(
            nameof(Serie), typeof(IEnumerable<DashboardSerieDto>), typeof(CtlGraficoBarras),
            new PropertyMetadata(null, OnSerieChanged));

        public static readonly DependencyProperty TituloProperty = DependencyProperty.Register(
            nameof(Titulo), typeof(string), typeof(CtlGraficoBarras),
            new PropertyMetadata(string.Empty, OnSerieChanged));

        public static readonly DependencyProperty EsMontoProperty = DependencyProperty.Register(
            nameof(EsMonto), typeof(bool), typeof(CtlGraficoBarras),
            new PropertyMetadata(false, OnSerieChanged));

        public static readonly DependencyProperty MostrarValoresProperty = DependencyProperty.Register(
            nameof(MostrarValores), typeof(bool), typeof(CtlGraficoBarras),
            new PropertyMetadata(true, OnSerieChanged));

        public CtlGraficoBarras()
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

        /// <summary>Verdadero si se muestra el valor sobre cada barra.</summary>
        public bool MostrarValores
        {
            get => (bool)GetValue(MostrarValoresProperty);
            set => SetValue(MostrarValoresProperty, value);
        }

        private static void OnSerieChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtlGraficoBarras control) control.Dibujar();
        }

        private void Lienzo_SizeChanged(object sender, SizeChangedEventArgs e) => Dibujar();

        private void Dibujar()
        {
            if (Lienzo is null) return;

            Lienzo.Children.Clear();

            var serie = (Serie ?? Enumerable.Empty<DashboardSerieDto>())
                .Where(s => s.Valor >= 0)
                .OrderBy(s => s.Etiqueta)
                .ToList();

            var ancho = Lienzo.ActualWidth;
            var alto = Lienzo.ActualHeight;

            if (ancho <= 0 || alto <= 0) return;

            if (!string.IsNullOrWhiteSpace(Titulo))
            {
                Lienzo.Children.Add(GraficoAyudas.CrearTexto(
                    Titulo, MargenIzquierdo, 4, GraficoCalculos.ColorTexto, 10, true));
            }

            if (serie.Count == 0)
            {
                Lienzo.Children.Add(GraficoAyudas.CrearTexto(
                    "Sin datos para mostrar", 0, alto / 2 - 8, GraficoCalculos.ColorSubtexto, 11,
                    false, TextAlignment.Center, ancho));
                return;
            }

            var escala = GraficoCalculos.CalcularEscala(serie.Select(s => s.Valor));
            var anchoGrafico = ancho - MargenIzquierdo - MargenDerecho;
            var altoGrafico = alto - MargenSuperior - MargenInferior;

            if (anchoGrafico <= 0 || altoGrafico <= 0) return;

            // Lineas de guia y escala del eje Y.
            for (var i = 0; i < escala.Cortes.Count; i++)
            {
                var valor = escala.Cortes[i];
                var ratio = escala.MaximoEscala <= 0 ? 0 : valor / escala.MaximoEscala;
                var y = MargenSuperior + altoGrafico - (altoGrafico * ratio);

                Lienzo.Children.Add(GraficoAyudas.CrearLinea(
                    MargenIzquierdo, y, MargenIzquierdo + anchoGrafico, y,
                    GraficoCalculos.ColorLineaGuia, 1, true));

                Lienzo.Children.Add(GraficoAyudas.CrearTexto(
                    GraficoCalculos.FormatearValor((decimal)valor, EsMonto),
                    0, y - 6, GraficoCalculos.ColorSubtexto, 8, false, TextAlignment.Right,
                    MargenIzquierdo - 8));
            }

            // Ejes.
            Lienzo.Children.Add(GraficoAyudas.CrearLinea(
                MargenIzquierdo, MargenSuperior + altoGrafico,
                MargenIzquierdo + anchoGrafico, MargenSuperior + altoGrafico,
                GraficoCalculos.ColorEje, 1.5));
            Lienzo.Children.Add(GraficoAyudas.CrearLinea(
                MargenIzquierdo, MargenSuperior,
                MargenIzquierdo, MargenSuperior + altoGrafico,
                GraficoCalculos.ColorEje, 1.5));

            // Barras.
            var paso = anchoGrafico / serie.Count;
            var anchoBarra = System.Math.Clamp(paso * 0.65, 6.0, 36.0);
            var mostrarValor = MostrarValores && serie.Count <= 14;

            for (var i = 0; i < serie.Count; i++)
            {
                var item = serie[i];
                var x = MargenIzquierdo + (i * paso) + ((paso - anchoBarra) / 2.0);
                var alturaBarra = System.Math.Max(2.0, escala.Proporcion(item.Valor) * altoGrafico);
                var y = MargenSuperior + altoGrafico - alturaBarra;

                Lienzo.Children.Add(GraficoAyudas.CrearRectangulo(
                    x, y, anchoBarra, alturaBarra, GraficoCalculos.ColorBarra));

                if (mostrarValor && item.Valor > 0)
                {
                    Lienzo.Children.Add(GraficoAyudas.CrearTexto(
                        GraficoCalculos.FormatearValor(item.Valor, EsMonto),
                        MargenIzquierdo + (i * paso),
                        System.Math.Max(MargenSuperior - 14, y - 14),
                        GraficoCalculos.ColorTexto, 8, true, TextAlignment.Center, paso));
                }

                Lienzo.Children.Add(GraficoAyudas.CrearTexto(
                    item.Etiqueta,
                    MargenIzquierdo + (i * paso),
                    MargenSuperior + altoGrafico + 4,
                    GraficoCalculos.ColorTexto, 8, false, TextAlignment.Center, paso));
            }
        }
    }
}
