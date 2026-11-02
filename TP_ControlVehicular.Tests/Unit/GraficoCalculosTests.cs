// walterg20
using System;
using System.Collections.Generic;
using System.Linq;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Presentacion.Graficos;
using Xunit;

namespace TP_ControlVehicular.Tests.Unit
{
    /// <summary>
    /// Pruebas de la lógica de cálculo de los gráficos del dashboard.
    /// </summary>
    public class GraficoCalculosTests
    {
        private static List<DashboardSerieDto> Serie(params (string Etiqueta, decimal Valor)[] datos) =>
            datos.Select(d => new DashboardSerieDto { Etiqueta = d.Etiqueta, Valor = d.Valor }).ToList();

        // ------------------------------------------------------------------
        // Escala
        // ------------------------------------------------------------------

        [Fact]
        public void CalcularEscala_UsaElMaximoConMargenDelQuincePorCiento()
        {
            var escala = GraficoCalculos.CalcularEscala(Serie(("a", 100m), ("b", 200m)));

            Assert.Equal(200m, escala.Maximo);
            Assert.Equal(230d, escala.MaximoEscala, 3);
            Assert.Equal(4, escala.Cortes.Count);
            Assert.Equal(0d, escala.Cortes[0], 3);
            Assert.Equal(escala.MaximoEscala, escala.Cortes[3], 3);
        }

        [Fact]
        public void CalcularEscala_SinValoresUsaUnaEscalaPorDefecto()
        {
            var escala = GraficoCalculos.CalcularEscala(Serie());

            Assert.Equal(0m, escala.Maximo);
            Assert.Equal(100d, escala.MaximoEscala, 3);
            Assert.Equal(0d, escala.Proporcion(50m), 3);
        }

        [Fact]
        public void Proporcion_AcotaElValorEntreCeroYUno()
        {
            var escala = GraficoCalculos.CalcularEscala(Serie(("a", 100m)));

            Assert.Equal(1d, escala.Proporcion(100m), 3);
            Assert.Equal(0d, escala.Proporcion(-50m), 3);
        }

        // ------------------------------------------------------------------
        // Formato de montos (mismas reglas que los PDF)
        // ------------------------------------------------------------------

        [Theory]
        [InlineData(2500000, "$2.5M")]
        [InlineData(150000, "$150k")]
        [InlineData(9500, "$9,500")]
        public void FormatearMonto_UsaElFormatoAbreviado(decimal valor, string esperado)
        {
            Assert.Equal(esperado, GraficoCalculos.FormatearMonto(valor));
        }

        [Fact]
        public void FormatearValor_SegunEsMontoMuestraMontoOCantidad()
        {
            Assert.Equal("$150k", GraficoCalculos.FormatearValor(150000m, true));
            Assert.Equal("1,500", GraficoCalculos.FormatearValor(1500m, false));
        }

        // ------------------------------------------------------------------
        // Agrupamiento top N + Otros
        // ------------------------------------------------------------------

        [Fact]
        public void AgruparTopN_AgrupaElRestoBajoOtros()
        {
            var datos = Serie(("a", 10m), ("b", 9m), ("c", 8m), ("d", 7m));

            var resultado = GraficoCalculos.AgruparTopN(datos, 3);

            Assert.Equal(3, resultado.Count);
            Assert.Equal("Otros", resultado[2].Etiqueta);
            Assert.Equal(15m, resultado[2].Valor); // 8 + 7
        }

        [Fact]
        public void AgruparTopN_SinExcesoDejaLaSerieComoEsta()
        {
            var datos = Serie(("a", 10m), ("b", 9m));

            var resultado = GraficoCalculos.AgruparTopN(datos, 8);

            Assert.Equal(2, resultado.Count);
            Assert.DoesNotContain(resultado, r => r.Etiqueta == "Otros");
        }

        [Fact]
        public void AgruparTopN_OrdenaDeMayorAMenorYDescartaLosCeros()
        {
            var datos = Serie(("a", 10m), ("b", 30m), ("c", 0m));

            var resultado = GraficoCalculos.AgruparTopN(datos, 8);

            Assert.Equal(2, resultado.Count);
            Assert.Equal("b", resultado[0].Etiqueta);
            Assert.Equal("a", resultado[1].Etiqueta);
        }

        // ------------------------------------------------------------------
        // Sectores de la torta
        // ------------------------------------------------------------------

        [Fact]
        public void CalcularSectores_ReparteLosGradosSegunElValor()
        {
            var datos = Serie(("a", 75m), ("b", 25m));

            var sectores = GraficoCalculos.CalcularSectores(datos);

            Assert.Equal(2, sectores.Count);
            Assert.Equal(0d, sectores[0].AnguloInicio, 3);
            Assert.Equal(270d, sectores[0].AnguloFin, 3);
            Assert.Equal(75d, sectores[0].Porcentaje, 3);
            Assert.Equal(270d, sectores[1].AnguloInicio, 3);
            Assert.Equal(360d, sectores[1].AnguloFin, 3);
            Assert.Equal(25d, sectores[1].Porcentaje, 3);
        }

        [Fact]
        public void CalcularSectores_UnaPorcionMayorAMediaVueltaSeMarcaComoGrande()
        {
            var datos = Serie(("a", 90m), ("b", 10m));

            var sectores = GraficoCalculos.CalcularSectores(datos);

            Assert.True(sectores[0].EsPorcionGrande);
            Assert.False(sectores[1].EsPorcionGrande);
        }

        [Fact]
        public void CalcularSectores_SinValoresNoDevuelveSectores()
        {
            Assert.Empty(GraficoCalculos.CalcularSectores(Serie()));
            Assert.Empty(GraficoCalculos.CalcularSectores(Serie(("a", 0m))));
        }

        // ------------------------------------------------------------------
        // Geometria
        // ------------------------------------------------------------------

        [Fact]
        public void PuntoEnCircunferencia_ArrancaEnLasDoceYAvanzaEnSentidoHorario()
        {
            var (x, y) = GraficoCalculos.PuntoEnCircunferencia(100, 100, 50, 0);
            Assert.Equal(100d, x, 3);
            Assert.Equal(50d, y, 3); // arriba

            var (x90, y90) = GraficoCalculos.PuntoEnCircunferencia(100, 100, 50, 90);
            Assert.Equal(150d, x90, 3); // a la derecha
            Assert.Equal(100d, y90, 3);

            var (x180, y180) = GraficoCalculos.PuntoEnCircunferencia(100, 100, 50, 180);
            Assert.Equal(100d, x180, 3);
            Assert.Equal(150d, y180, 3); // abajo
        }

        [Fact]
        public void CalcularAnguloBarrido_TraduceElPorcentajeAGradosYSeAcota()
        {
            Assert.Equal(180d, GraficoCalculos.CalcularAnguloBarrido(50m), 3);
            Assert.Equal(360d, GraficoCalculos.CalcularAnguloBarrido(100m), 3);
            Assert.Equal(0d, GraficoCalculos.CalcularAnguloBarrido(0m), 3);
            Assert.Equal(360d, GraficoCalculos.CalcularAnguloBarrido(150m), 3);
        }
    }
}
