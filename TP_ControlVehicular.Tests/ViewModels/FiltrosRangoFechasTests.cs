using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;
using TP_ControlVehicular.Negocio.Services;
using TP_ControlVehicular.Presentacion.ViewModels;
using Xunit;

namespace TP_ControlVehicular.Tests.ViewModels
{
    public class FiltrosRangoFechasTests
    {
        private OrdenServicioViewModel CrearOrdenServicioViewModel()
        {
            var mockRepoReg = new Mock<IRegistroServicioRepository>();
            var mockMapper = new Mock<IMapper>();
            var mockListar = new Mock<ListarRegistroServiciosHandler>(mockRepoReg.Object, mockMapper.Object);
            var mockRegistrar = new Mock<RegistrarRegistroServicioHandler>(mockRepoReg.Object, mockMapper.Object);
            var mockModificar = new Mock<ModificarRegistroServicioHandler>(mockRepoReg.Object, mockMapper.Object);
            var mockVehiculoRepo = new Mock<IVehiculoRepository>();
            var mockTallerRepo = new Mock<ITallerRepository>();
            var mockServicioRepo = new Mock<IServicioRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var mockClienteRepo = new Mock<IClienteRepository>();
            return new OrdenServicioViewModel(
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
        }

        [Fact]
        public void OrdenServicio_InicializaConFechasDelMesActual()
        {
            var vm = CrearOrdenServicioViewModel();

            var primerDiaEsperado = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            Assert.Equal(primerDiaEsperado, vm.FechaDesde);
            Assert.Equal(DateTime.Today, vm.FechaHasta);
            Assert.Equal("Todos", vm.EstadoFiltro);
        }

        [Fact]
        public void OrdenServicio_FiltrarPorRangoFechas_ExcluyeOrdenesFueraDelPeriodo()
        {
            var vm = CrearOrdenServicioViewModel();

            var orden1 = new RegistroServicioDto { Id = 1, Fecha = new DateTime(2026, 5, 10), VehiculoPatente = "AAA111", Estado = "Abierta" };
            var orden2 = new RegistroServicioDto { Id = 2, Fecha = new DateTime(2026, 5, 20), VehiculoPatente = "BBB222", Estado = "En Proceso" };
            var orden3 = new RegistroServicioDto { Id = 3, Fecha = new DateTime(2026, 6, 5), VehiculoPatente = "CCC333", Estado = "Completada" };

            vm.Ordenes.Add(orden1);
            vm.Ordenes.Add(orden2);
            vm.Ordenes.Add(orden3);

            // Filtrar solo mayo 2026
            vm.FechaDesde = new DateTime(2026, 5, 1);
            vm.FechaHasta = new DateTime(2026, 5, 31);

            var resultado = vm.ListadoOrdenesFiltered.ToList();

            Assert.Equal(2, resultado.Count);
            Assert.Contains(resultado, o => o.Id == 1);
            Assert.Contains(resultado, o => o.Id == 2);
            Assert.DoesNotContain(resultado, o => o.Id == 3);
        }

        [Fact]
        public void OrdenServicio_FiltrarPorEstadoYRangoFechas_CombinaAmbosCriterios()
        {
            var vm = CrearOrdenServicioViewModel();

            var orden1 = new RegistroServicioDto { Id = 1, Fecha = new DateTime(2026, 5, 10), VehiculoPatente = "AAA111", Estado = "Abierta" };
            var orden2 = new RegistroServicioDto { Id = 2, Fecha = new DateTime(2026, 5, 20), VehiculoPatente = "BBB222", Estado = "Completada" };
            var orden3 = new RegistroServicioDto { Id = 3, Fecha = new DateTime(2026, 5, 25), VehiculoPatente = "CCC333", Estado = "Abierta" };

            vm.Ordenes.Add(orden1);
            vm.Ordenes.Add(orden2);
            vm.Ordenes.Add(orden3);

            vm.FechaDesde = new DateTime(2026, 5, 1);
            vm.FechaHasta = new DateTime(2026, 5, 31);
            vm.EstadoFiltro = "Completada";

            var resultado = vm.ListadoOrdenesFiltered.ToList();

            Assert.Single(resultado);
            Assert.Equal(2, resultado.First().Id);
        }

        [Fact]
        public void OrdenServicio_FechaDesdeMayorAFechaHasta_MuestraMensajeYRetornaVacio()
        {
            var vm = CrearOrdenServicioViewModel();
            vm.Ordenes.Add(new RegistroServicioDto { Id = 1, Fecha = DateTime.Today });

            vm.FechaDesde = new DateTime(2026, 6, 1);
            vm.FechaHasta = new DateTime(2026, 5, 1);

            var resultado = vm.ListadoOrdenesFiltered.ToList();

            Assert.Empty(resultado);
            Assert.Contains("La fecha 'Desde' no puede ser posterior a 'Hasta'", vm.MensajeValidacionFechas);
        }

        [Fact]
        public void OrdenServicio_LimpiarFiltros_RestableceValoresPorDefecto()
        {
            var vm = CrearOrdenServicioViewModel();

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
