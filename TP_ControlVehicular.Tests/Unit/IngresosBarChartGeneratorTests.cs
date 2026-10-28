using System;
using System.Collections.Generic;
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
            Assert.Contains("Sin ingresos registrados en el período", svgNull);
            Assert.Contains("Sin ingresos registrados en el período", svgVacio);
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
            Assert.Contains("Sin ingresos registrados en el período", svg);
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
    }
}
