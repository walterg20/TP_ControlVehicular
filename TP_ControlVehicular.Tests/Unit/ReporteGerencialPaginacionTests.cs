// walterg20
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TP_ControlVehicular.Negocio.DTOs.Reportes;
using TP_ControlVehicular.Negocio.Reportes;
using TP_ControlVehicular.Presentacion.ViewModels;
using Xunit;

namespace TP_ControlVehicular.Tests.Unit
{
    public class ReporteGerencialPaginacionTests
    {
        private (ReporteGerencialViewModel ViewModel, Mock<IReporteGerencialRepository> MockRepo) CrearViewModelConMock()
        {
            var mockRepo = new Mock<IReporteGerencialRepository>();

            // Setup default responses
            mockRepo.Setup(r => r.ObtenerIngresosPorFechaPaginadoAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((DateTime? d, DateTime? h, int pagina, int tamano) =>
                {
                    var items = new List<ReporteIngresoDto>
                    {
                        new() { Fecha = DateTime.Today.AddDays(-pagina), CantidadFacturas = 2, IngresosTotales = 150000m }
                    };
                    return (items, 25); // 25 total records
                });

            mockRepo.Setup(r => r.ObtenerIngresosMensualesAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(new List<ReporteIngresoMensualDto>
                {
                    new() { PeriodoMes = "2026-06", FechaMes = new DateTime(2026, 6, 1), CantidadFacturas = 11, IngresosTotales = 550000m },
                    new() { PeriodoMes = "2026-07", FechaMes = new DateTime(2026, 7, 1), CantidadFacturas = 12, IngresosTotales = 620000m }
                });

            mockRepo.Setup(r => r.ObtenerTopClientesAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>()))
                .ReturnsAsync(new List<ReporteTopClienteDto>());

            mockRepo.Setup(r => r.ObtenerModelosMasReparadosAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>()))
                .ReturnsAsync(new List<ReporteModeloReparadoDto>());

            mockRepo.Setup(r => r.ObtenerServiciosPorRangoAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(new List<ReporteServicioCantidadDto>());

            mockRepo.Setup(r => r.ObtenerIngresosPorFechaAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(new List<ReporteIngresoDto>
                {
                    new() { Fecha = new DateTime(2026, 6, 1), CantidadFacturas = 2, IngresosTotales = 100000m }
                });

            var vm = new ReporteGerencialViewModel(mockRepo.Object);
            return (vm, mockRepo);
        }

        [Fact]
        public async Task LoadAsync_CalculaTotalPaginas_YPopulaIngresosMensuales()
        {
            var (vm, _) = CrearViewModelConMock();
            vm.TamanoPagina = 10;

            await vm.LoadAsync();

            Assert.Equal(1, vm.PaginaActual);
            Assert.Equal(25, vm.TotalRegistros);
            Assert.Equal(3, vm.TotalPaginas); // ceil(25 / 10) = 3
            Assert.False(vm.PuedeIrAnterior);
            Assert.True(vm.PuedeIrSiguiente);
            Assert.Equal(2, vm.IngresosMensuales.Count);
            Assert.Contains("2026-06", vm.IngresosMensuales.First().PeriodoMes);
        }

        [Fact]
        public async Task CambiarPagina_SiguienteYAnterior_ActualizaPaginaYEstados()
        {
            var (vm, _) = CrearViewModelConMock();
            await vm.LoadAsync(); // TotalPaginas = 3

            // Ir a pagina 2
            await vm.CambiarPaginaAsync(2);
            Assert.Equal(2, vm.PaginaActual);
            Assert.True(vm.PuedeIrAnterior);
            Assert.True(vm.PuedeIrSiguiente);

            // Ir a pagina 3 (ultima)
            await vm.CambiarPaginaAsync(3);
            Assert.Equal(3, vm.PaginaActual);
            Assert.True(vm.PuedeIrAnterior);
            Assert.False(vm.PuedeIrSiguiente);

            // Clamping: intentar ir a pagina 4 debe permanecer en 3
            await vm.CambiarPaginaAsync(4);
            Assert.Equal(3, vm.PaginaActual);
        }

        [Fact]
        public async Task CambiarTamanoPagina_ReseteaAPagina1_YRecalculaTotalPaginas()
        {
            var (vm, mockRepo) = CrearViewModelConMock();
            await vm.LoadAsync();
            await vm.CambiarPaginaAsync(2);

            // Cambiar tamaño a 20 (25 / 20 = 2 páginas)
            vm.TamanoPagina = 20;

            Assert.Equal(1, vm.PaginaActual);
            Assert.Equal(2, vm.TotalPaginas);
            Assert.False(vm.PuedeIrAnterior);
            Assert.True(vm.PuedeIrSiguiente);
        }

        [Fact]
        public async Task LoadAsync_SinRegistros_PaginaEs1YTotalEs0()
        {
            var mockRepo = new Mock<IReporteGerencialRepository>();
            mockRepo.Setup(r => r.ObtenerIngresosPorFechaPaginadoAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((Enumerable.Empty<ReporteIngresoDto>(), 0));

            mockRepo.Setup(r => r.ObtenerIngresosMensualesAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(Enumerable.Empty<ReporteIngresoMensualDto>());
            mockRepo.Setup(r => r.ObtenerTopClientesAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>()))
                .ReturnsAsync(Enumerable.Empty<ReporteTopClienteDto>());
            mockRepo.Setup(r => r.ObtenerModelosMasReparadosAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>()))
                .ReturnsAsync(Enumerable.Empty<ReporteModeloReparadoDto>());
            mockRepo.Setup(r => r.ObtenerServiciosPorRangoAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(Enumerable.Empty<ReporteServicioCantidadDto>());

            var vm = new ReporteGerencialViewModel(mockRepo.Object);
            await vm.LoadAsync();

            Assert.Equal(1, vm.PaginaActual);
            Assert.Equal(0, vm.TotalRegistros);
            Assert.Equal(1, vm.TotalPaginas);
            Assert.False(vm.PuedeIrAnterior);
            Assert.False(vm.PuedeIrSiguiente);
            Assert.Empty(vm.Ingresos);
            Assert.Empty(vm.IngresosMensuales);
        }
    }
}
