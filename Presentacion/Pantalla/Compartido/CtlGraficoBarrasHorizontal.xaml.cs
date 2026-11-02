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
    /// Grafico de barras horizontales dibujado con WPF nativo.
    /// Pensado para rankings con etiquetas largas (modelos mas reparados).
    /// </summary>
    public partial class CtlGraficoBarrasHorizontal : UserControl
    {
        private const double MargenIzquierdo = 132.0;
        private const double MargenDerecho = 52.0;
        private const double MargenSuperior = 22.0;
        private const double MargenInferior = 6.0;
        private const double AltoFila = 18.0;

        public static readonly DependencyProperty SerieProperty = DependencyProperty.Register(
            nameof(Serie), typeof(IEnumerable<DashboardSerieDto>), typeof(CtlGraficoBarrasHorizontal),
            new PropertyMetadata(null, OnSerieChanged));

        public static readonly DependencyProperty TituloProperty = DependencyProperty.Register(
            nameof(Titulo), typeof(string), typeof(CtlGraficoBarrasHorizontal),
            new PropertyMetadata(string.Empty, OnSerieChanged));

        public static readonly DependencyProperty EsMontoProperty = DependencyProperty.Register(
            nameof(EsMonto), typeof(bool), typeof(CtlGraficoBarrasHorizontal),
            new PropertyMetadata(false, OnSerieChanged));

        public CtlGraficoBarrasHorizontal()
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
            if (d is CtlGraficoBarrasHorizontal control) control.Dibujar();
        }

        private void Lienzo_SizeChanged(object sender, SizeChangedEventArgs e) => Dibujar();

        private void Dibujar()
        {
            if (Lienzo is null) return;

            Lienzo.Children.Clear();

            var serie = (Serie ?? Enumerable.Empty<DashboardSerieDto>())
                .Where(s => s.Valor > 0)
                .OrderByDescending(s => s.Valor)
                .ToList();

            var ancho = Lienzo.ActualWidth;
            var alto = Lienzo.ActualHeight;

            if (ancho <= 0 || alto <= 0) return;

            if (!string.IsNullOrWhiteSpace(Titulo))
            {
                Lienzo.Children.Add(GraficoAyudas.CrearTexto(
                    Titulo, 0, 2, GraficoCalculos.ColorTexto, 10, true));
            }

            if (serie.Count == 0)
            {
                Lienzo.Children.Add(GraficoAyudas.CrearTexto(
                    "Sin datos para mostrar", 0, alto / 2 - 8, GraficoCalculos.ColorSubtexto, 11,
                    false, TextAlignment.Center, ancho));
                return;
            }

            var escala = GraficoCalculos.CalcularEscala(serie.Select(s => s.Valor));
            var anchoUtil = ancho - MargenIzquierdo - MargenDerecho;

            if (anchoUtil <= 0) return;

            var altoDisponible = alto - MargenSuperior - MargenInferior;
            var altoFila = System.Math.Min(AltoFila, altoDisponible / System.Math.Max(1, serie.Count));
            var altoBarra = System.Math.Max(4.0, altoFila * 0.62);

            for (var i = 0; i < serie.Count; i++)
            {
                var item = serie[i];
                var y = MargenSuperior + (i * altoFila);
                var anchoBarra = System.Math.Max(2.0, escala.Proporcion(item.Valor) * anchoUtil);

                // Etiqueta a la izquierda.
                Lienzo.Children.Add(GraficoAyudas.CrearTexto(
                    Recortar(item.Etiqueta, 22),
                    0, y + (altoFila - 12) / 2.0,
                    GraficoCalculos.ColorTexto, 8, false, TextAlignment.Right,
                    MargenIzquierdo - 8));

                // Barra.
                Lienzo.Children.Add(GraficoAyudas.CrearRectangulo(
                    MargenIzquierdo, y + (altoFila - altoBarra) / 2.0,
                    anchoBarra, altoBarra, GraficoCalculos.ColorBarra));

                // Valor a la derecha.
                Lienzo.Children.Add(GraficoAyudas.CrearTexto(
                    GraficoCalculos.FormatearValor(item.Valor, EsMonto),
                    MargenIzquierdo + anchoUtil + 4, y + (altoFila - 12) / 2.0,
                    GraficoCalculos.ColorTexto, 8, true));
            }
        }

        /// <summary>Recorta la etiqueta para que no invada el area del grafico.</summary>
        private static string Recortar(string texto, int maximo) =>
            string.IsNullOrEmpty(texto) || texto.Length <= maximo
                ? texto
                : texto.Substring(0, maximo - 1) + "…";
    }
}
