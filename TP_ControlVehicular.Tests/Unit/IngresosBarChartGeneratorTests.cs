// walterg20
using System;
using System.Collections.Generic;
using System.IO;
using QuestPDF.Fluent;
using TP_ControlVehicular.Negocio.DTOs.Reportes;
using TP_ControlVehicular.Negocio.Reportes.Documentos;
using Xunit;

namespace TP_ControlVehicular.Tests.Unit
{
    public class IngresosBarChartGeneratorTests
    {
        [Fact]
        public void BuildSvg_ConDatos_GeneraSvgConEjesBarrasYMontos()
        {
            // Arrange
            var datos = new List<ReporteIngresoDto>
            {
                new ReporteIngresoDto { Fecha = new DateTime(2026, 10, 1), CantidadFacturas = 3, IngresosTotales = 150000m },
                new ReporteIngresoDto { Fecha = new DateTime(2026, 10, 2), CantidadFacturas = 5, IngresosTotales = 280000m },
                new ReporteIngresoDto { Fecha = new DateTime(2026, 10, 3), CantidadFacturas = 2, IngresosTotales = 95000m }
            };

            // Act
            var svg = IngresosBarChartGenerator.BuildSvg(datos);

            // Assert
            Assert.NotNull(svg);
            Assert.StartsWith("<svg", svg);
            Assert.EndsWith("</svg>", svg);
            Assert.Contains("<rect", svg);
            Assert.Contains("01/10", svg);
            Assert.Contains("02/10", svg);
            Assert.Contains("03/10", svg);
        }

        [Fact]
        public void BuildSvg_SinDatos_GeneraSvgConMensajeVacio()
        {
            // Act
            var svgNull = IngresosBarChartGenerator.BuildSvg(null);
            var svgVacio = IngresosBarChartGenerator.BuildSvg(new List<ReporteIngresoDto>());

            // Assert
            Assert.Contains("Sin ingresos registrados en el per��odo", svgNull);
            Assert.Contains("Sin ingresos registrados en el per��odo", svgVacio);
        }

        [Fact]
        public void BuildSvg_ConIngresosCero_GeneraSvgConMensajeVacio()
        {
            // Arrange
            var datos = new List<ReporteIngresoDto>
            {
                new ReporteIngresoDto { Fecha = new DateTime(2026, 10, 1), CantidadFacturas = 0, IngresosTotales = 0m }
            };

            // Act
            var svg = IngresosBarChartGenerator.BuildSvg(datos);

            // Assert
            Assert.Contains("Sin ingresos registrados en el per��odo", svg);
        }

        [Fact]
        public void BuildSvg_ConMontosGrandes_GeneraFormatoAbreviado()
        {
            // Arrange
            var datos = new List<ReporteIngresoDto>
            {
                new ReporteIngresoDto { Fecha = new DateTime(2026, 10, 1), CantidadFacturas = 10, IngresosTotales = 2500000m }
            };

            // Act
            var svg = IngresosBarChartGenerator.BuildSvg(datos);

            // Assert
            Assert.Contains("$2.5M", svg);
        }

        // ------------------------------------------------------------------
        // Agregación por periodo
        // ------------------------------------------------------------------

        [Fact]
        public void AbarcaMasDeUnMes_SoloCuentaLosMesesCalendarioDistintos()
        {
            var datos = new List<ReporteIngresoDto>
            {
                new ReporteIngresoDto { Fecha = new DateTime(2026, 10, 1) },
                new ReporteIngresoDto { Fecha = new DateTime(2026, 10, 15) },
                new ReporteIngresoDto { Fecha = new DateTime(2026, 11, 2) }
            };

            Assert.True(IngresosBarChartGenerator.AbarcaMasDeUnMes(datos));

            var unSoloMes = new List<ReporteIngresoDto>
            {
                new ReporteIngresoDto { Fecha = new DateTime(2026, 10, 1) },
                new ReporteIngresoDto { Fecha = new DateTime(2026, 10, 31) }
            };

            Assert.False(IngresosBarChartGenerator.AbarcaMasDeUnMes(unSoloMes));
            Assert.False(IngresosBarChartGenerator.AbarcaMasDeUnMes(new List<ReporteIngresoDto>()));
            Assert.False(IngresosBarChartGenerator.AbarcaMasDeUnMes(null));
        }

        [Fact]
        public void BuildSvgPorMeses_GeneraUnaBarraPorPeriodoConEtiquetaDeMes()
        {
            var mensuales = new List<ReporteIngresoMensualDto>
            {
                new ReporteIngresoMensualDto { PeriodoMes = "2026-06", FechaMes = new DateTime(2026, 6, 1), IngresosTotales = 550000m },
                new ReporteIngresoMensualDto { PeriodoMes = "2026-07", FechaMes = new DateTime(2026, 7, 1), IngresosTotales = 620000m }
            };

            var svg = IngresosBarChartGenerator.BuildSvgPorMeses(mensuales);

            Assert.Contains("Ingresos por mes", svg);
            Assert.Contains("06/26", svg);
            Assert.Contains("07/26", svg);
            Assert.Contains("$620k", svg);
        }

        [Fact]
        public void BuildSvg_ConRangoDeVariosMeses_AgrupaPorPeriodoMensual()
        {
            var diarios = new List<ReporteIngresoDto>
            {
                new ReporteIngresoDto { Fecha = new DateTime(2026, 6, 10), IngresosTotales = 100000m },
                new ReporteIngresoDto { Fecha = new DateTime(2026, 6, 20), IngresosTotales = 200000m },
                new ReporteIngresoDto { Fecha = new DateTime(2026, 7, 5), IngresosTotales = 300000m }
            };

            var mensuales = new List<ReporteIngresoMensualDto>
            {
                new ReporteIngresoMensualDto { PeriodoMes = "2026-06", FechaMes = new DateTime(2026, 6, 1), IngresosTotales = 300000m },
                new ReporteIngresoMensualDto { PeriodoMes = "2026-07", FechaMes = new DateTime(2026, 7, 1), IngresosTotales = 300000m }
            };

            var svg = IngresosBarChartGenerator.BuildSvg(diarios, mensuales);

            Assert.Contains("Ingresos por mes", svg);
            Assert.Contains("06/26", svg);
            Assert.Contains("07/26", svg);
            // Las etiquetas diarias no deben aparecer cuando se agrupa por mes.
            Assert.DoesNotContain("10/06", svg);
        }

        [Fact]
        public void BuildSvg_ConRangoDeUnSoloMes_MantieneElDetalleDiario()
        {
            var diarios = new List<ReporteIngresoDto>
            {
                new ReporteIngresoDto { Fecha = new DateTime(2026, 6, 10), IngresosTotales = 100000m },
                new ReporteIngresoDto { Fecha = new DateTime(2026, 6, 20), IngresosTotales = 200000m }
            };

            var mensuales = new List<ReporteIngresoMensualDto>
            {
                new ReporteIngresoMensualDto { PeriodoMes = "2026-06", FechaMes = new DateTime(2026, 6, 1), IngresosTotales = 300000m }
            };

            var svg = IngresosBarChartGenerator.BuildSvg(diarios, mensuales);

            Assert.Contains("Ingresos por dia", svg);
            Assert.Contains("10/06", svg);
            Assert.Contains("20/06", svg);
        }

        [Fact]
        public void BuildSvg_SinResumenMensual_UsaElDetalleDiario()
        {
            var diarios = new List<ReporteIngresoDto>
            {
                new ReporteIngresoDto { Fecha = new DateTime(2026, 6, 10), IngresosTotales = 100000m },
                new ReporteIngresoDto { Fecha = new DateTime(2026, 7, 20), IngresosTotales = 200000m }
            };

            var svg = IngresosBarChartGenerator.BuildSvg(diarios, new List<ReporteIngresoMensualDto>());

            Assert.Contains("Ingresos por dia", svg);
        }

        [Fact]
        public void BuildSvgPorMeses_ElSvgSeRenderizaEnElPdfDelReporte()
        {
            // Comprueba la ruta real del PDF: el SVG generado (ya sea diario o mensual)
            // tiene que ser aceptado por el parser de QuestPDF.
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var mensuales = new List<ReporteIngresoMensualDto>
            {
                new ReporteIngresoMensualDto { PeriodoMes = "2026-06", FechaMes = new DateTime(2026, 6, 1), IngresosTotales = 550000m },
                new ReporteIngresoMensualDto { PeriodoMes = "2026-07", FechaMes = new DateTime(2026, 7, 1), IngresosTotales = 620000m }
            };

            var svg = IngresosBarChartGenerator.BuildSvgPorMeses(mensuales);

            var filePath = Path.Combine(Path.GetTempPath(), $"test_grafico_ingresos_{Guid.NewGuid():N}.pdf");
            try
            {
                var document = QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(QuestPDF.Helpers.PageSizes.A4);
                        page.Content().Svg(svg);
                    });
                });

                document.GeneratePdf(filePath);

                Assert.True(File.Exists(filePath));
                Assert.True(new FileInfo(filePath).Length > 0);
            }
            finally
            {
                if (File.Exists(filePath)) File.Delete(filePath);
            }
        }
    }
}
