// walterg20
using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;
using TP_ControlVehicular.Negocio.Services;
using TP_ControlVehicular.Presentacion.ViewModels;
using Xunit;

namespace TP_ControlVehicular.Tests.ViewModels
{
    /// <summary>
    /// Pruebas de los filtros por rango de fechas del listado de ordenes.
    /// Desde la Fase 3 de la spec paginacion-listados el filtrado se ejecuta en la base de
    /// datos, asi que el ViewModel se ocupa de reenviar los filtros y de validar el rango.
    /// </summary>
    public class FiltrosRangoFechasTests
    {
        private (OrdenServicioViewModel Vm, Mock<IRegistroServicioRepository> Repo) CrearOrdenServicioViewModel()
        {
            var mockRepoReg = new Mock<IRegistroServicioRepository>();
            var mockMapper = new Mock<IMapper>();

            mockRepoReg.Setup(r => r.GetPaginadoAsync(
                    It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>()))
                .ReturnsAsync((string? b, string? e, DateTime? fd, DateTime? fh, int p, int t, string? fa) =>
                    (Enumerable.Empty<RegistroServicio>(), 0));

            mockMapper.Setup(m => m.Map<IEnumerable<RegistroServicioDto>>(It.IsAny<object>()))
                .Returns((object src) => ((IEnumerable<RegistroServicio>)src)
                    .Select(o => new RegistroServicioDto
                    {
                        Id = o.Id,
                        Fecha = o.Fecha,
                        Estado = o.Estado
                    })
                    .ToList());

            var mockListar = new Mock<ListarRegistroServiciosHandler>(mockRepoReg.Object, mockMapper.Object);
            var mockRegistrar = new Mock<RegistrarRegistroServicioHandler>(mockRepoReg.Object, mockMapper.Object);
            var mockModificar = new Mock<ModificarRegistroServicioHandler>(mockRepoReg.Object, new Mock<IFacturaRepository>().Object, mockMapper.Object);
            var mockVehiculoRepo = new Mock<IVehiculoRepository>();
            var mockTallerRepo = new Mock<ITallerRepository>();
            var mockServicioRepo = new Mock<IServicioRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var mockClienteRepo = new Mock<IClienteRepository>();

            var vm = new OrdenServicioViewModel(
                mockListar.Object,
                mockRegistrar.Object,
                mockModificar.Object,
                mockVehiculoRepo.Object,
                mockTallerRepo.Object,
                mockServicioRepo.Object,
                mockUsuarioRepo.Object,
                mockClienteRepo.Object,
                null!,
                null!,
                null!,
                mockMapper.Object
            );

            return (vm, mockRepoReg);
        }

        [Fact]
        public void OrdenServicio_InicializaConFechasDelMesActual()
        {
            var (vm, _) = CrearOrdenServicioViewModel();

            var primerDiaEsperado = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            Assert.Equal(primerDiaEsperado, vm.FechaDesde);
            Assert.Equal(DateTime.Today, vm.FechaHasta);
            Assert.Equal("Todos", vm.EstadoFiltro);
        }

        [Fact]
        public async Task OrdenServicio_FiltrarPorRangoFechas_ReenviaElRangoAlRepositorio()
        {
            var (vm, repo) = CrearOrdenServicioViewModel();

            vm.FechaDesde = new DateTime(2026, 5, 1);
            vm.FechaHasta = new DateTime(2026, 5, 31);

            await vm.BuscarAsync();

            repo.Verify(r => r.GetPaginadoAsync(
                It.IsAny<string?>(), It.IsAny<string?>(),
                new DateTime(2026, 5, 1), new DateTime(2026, 5, 31),
                1, 10, It.IsAny<string?>()), Times.Once);
        }

        [Fact]
        public async Task OrdenServicio_FiltrarPorEstadoYRangoFechas_ReenviaAmbosCriterios()
        {
            var (vm, repo) = CrearOrdenServicioViewModel();

            vm.FechaDesde = new DateTime(2026, 5, 1);
            vm.FechaHasta = new DateTime(2026, 5, 31);
            vm.EstadoFiltro = "Completada";

            await vm.BuscarAsync();

            repo.Verify(r => r.GetPaginadoAsync(
                It.IsAny<string?>(), "Completada",
                new DateTime(2026, 5, 1), new DateTime(2026, 5, 31),
                1, 10, It.IsAny<string?>()), Times.Once);
        }

        [Fact]
        public async Task OrdenServicio_FechaDesdeMayorAFechaHasta_MuestraMensajeYNoConsulta()
        {
            var (vm, repo) = CrearOrdenServicioViewModel();

            vm.FechaDesde = new DateTime(2026, 6, 1);
            vm.FechaHasta = new DateTime(2026, 5, 1);

            await vm.BuscarAsync();

            Assert.Contains("La fecha 'Desde' no puede ser posterior a 'Hasta'", vm.MensajeValidacionFechas);
            repo.Verify(r => r.GetPaginadoAsync(
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>()), Times.Never);
        }

        [Fact]
        public void OrdenServicio_LimpiarFiltros_RestableceValoresPorDefecto()
        {
            var (vm, _) = CrearOrdenServicioViewModel();

            vm.FechaDesde = new DateTime(2020, 1, 1);
            vm.FechaHasta = new DateTime(2020, 1, 31);
            vm.EstadoFiltro = "Cancelada";
            vm.TextoBusqueda = "Toyota";

            vm.LimpiarFiltrosCommand.Execute(null);

            var primerDiaEsperado = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            Assert.Equal(primerDiaEsperado, vm.FechaDesde);
            Assert.Equal(DateTime.Today, vm.FechaHasta);
            Assert.Equal("Todos", vm.EstadoFiltro);
            Assert.Equal(string.Empty, vm.TextoBusqueda);
        }
    }
}
