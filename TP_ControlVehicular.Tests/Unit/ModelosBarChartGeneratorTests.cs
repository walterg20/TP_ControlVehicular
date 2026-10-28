using System.Collections.Generic;
using TP_ControlVehicular.Negocio.DTOs.Reportes;
using TP_ControlVehicular.Negocio.Reportes.Documentos;
using Xunit;

namespace TP_ControlVehicular.Tests.Unit
{
    public class ModelosBarChartGeneratorTests
    {
        [Fact]
        public void BuildSvg_ConDatos_GeneraBarrasHorizontalesYCantidades()
        {
            // Arrange
            var datos = new List<ReporteModeloReparadoDto>
            {
                new ReporteModeloReparadoDto { Marca = "Toyota", Modelo = "Corolla", CantidadReparaciones = 12 },
                new ReporteModeloReparadoDto { Marca = "Ford", Modelo = "Focus", CantidadReparaciones = 8 },
                new ReporteModeloReparadoDto { Marca = "Fiat", Modelo = "Cronos", CantidadReparaciones = 5 }
            };

            // Act
            var svg = ModelosBarChartGenerator.BuildSvg(datos);

            // Assert
            Assert.NotNull(svg);
            Assert.StartsWith("<svg", svg);
            Assert.EndsWith("</svg>", svg);
            Assert.Contains("Toyota Corolla", svg);
            Assert.Contains("Ford Focus", svg);
            Assert.Contains("Fiat Cronos", svg);
            Assert.Contains("12 rep.", svg);
            Assert.Contains("8 rep.", svg);
            Assert.Contains("5 rep.", svg);
        }

        [Fact]
        public void BuildSvg_SinDatos_GeneraSvgConMensajeVacio()
        {
            // Act
            var svgNull = ModelosBarChartGenerator.BuildSvg(null);
            var svgVacio = ModelosBarChartGenerator.BuildSvg(new List<ReporteModeloReparadoDto>());

            // Assert
            Assert.Contains("Sin reparaciones registradas en el período", svgNull);
            Assert.Contains("Sin reparaciones registradas en el período", svgVacio);
        }

        [Fact]
        public void BuildSvg_CaracteresEspeciales_SeEscapanCorrectamente()
        {
            // Arrange
            var datos = new List<ReporteModeloReparadoDto>
            {
                new ReporteModeloReparadoDto { Marca = "Mercedes", Modelo = "Clase A & B", CantidadReparaciones = 4 }
            };

            // Act
            var svg = ModelosBarChartGenerator.BuildSvg(datos);

            // Assert
            Assert.Contains("&amp;", svg);
            Assert.DoesNotContain("Clase A & B", svg); // Debe estar escapado
        }

        [Fact]
        public void BuildSvg_OrdenaDescendente_PorCantidadReparaciones()
        {
            // Arrange (desordenado)
            var datos = new List<ReporteModeloReparadoDto>
            {
                new ReporteModeloReparadoDto { Marca = "Fiat", Modelo = "Cronos", CantidadReparaciones = 3 },
                new ReporteModeloReparadoDto { Marca = "Toyota", Modelo = "Hilux", CantidadReparaciones = 20 },
                new ReporteModeloReparadoDto { Marca = "Ford", Modelo = "Ranger", CantidadReparaciones = 10 }
            };

            // Act
            var svg = ModelosBarChartGenerator.BuildSvg(datos);

            // Assert
            var posHilux = svg.IndexOf("Toyota Hilux");
            var posRanger = svg.IndexOf("Ford Ranger");
            var posCronos = svg.IndexOf("Fiat Cronos");

            Assert.True(posHilux >= 0 && posRanger >= 0 && posCronos >= 0);
            Assert.True(posHilux < posRanger, "Hilux (20) debe aparecer antes que Ranger (10)");
            Assert.True(posRanger < posCronos, "Ranger (10) debe aparecer antes que Cronos (3)");
        }
    }
}
