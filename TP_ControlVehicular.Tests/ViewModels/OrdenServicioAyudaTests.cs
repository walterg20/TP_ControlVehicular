using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using TP_ControlVehicular.Datos.Repositories;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;
using TP_ControlVehicular.Negocio.Services;
using TP_ControlVehicular.Presentacion.ViewModels;
using Xunit;

namespace TP_ControlVehicular.Tests.ViewModels
{
    public class OrdenServicioAyudaTests
    {
        private static (OrdenServicioViewModel vm, Mock<IAyudaService> mockAyuda) CrearViewModel()
        {
            var mockRepoReg = new Mock<IRegistroServicioRepository>();
            var mockMapper = new Mock<IMapper>();

            var mockListar = new Mock<ListarRegistroServiciosHandler>(mockRepoReg.Object, mockMapper.Object);
            var mockRegistrar = new Mock<RegistrarRegistroServicioHandler>(mockRepoReg.Object, mockMapper.Object);
            var mockModificar = new Mock<ModificarRegistroServicioHandler>(mockRepoReg.Object, new Mock<IFacturaRepository>().Object, mockMapper.Object);
            var mockVehiculoRepo = new Mock<IVehiculoRepository>();
            var mockTallerRepo = new Mock<ITallerRepository>();
            var mockServicioRepo = new Mock<IServicioRepository>();
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var mockClienteRepo = new Mock<IClienteRepository>();
            var mockAyuda = new Mock<IAyudaService>();

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
                mockMapper.Object,
                mockAyuda.Object
            );

            return (vm, mockAyuda);
        }

        [Fact]
        public async Task CargarAyudaPorControl_ConAyudaExistente_AsignaAyudaActual()
        {
            var (vm, mockAyuda) = CrearViewModel();
            var secciones = new List<AyudaSeccionDto>
            {
                new AyudaSeccionDto { IdSeccion = 1, Titulo = "T", Contenido = "C", Orden = 1 }
            };
            var ayuda = new AyudaContextoDto { IdAyuda = 5, NombrePantalla = "OrdenServicio", NombreControl = "cmbVehiculos", Secciones = secciones };
            mockAyuda.Setup(s => s.ObtenerAyudaAsync(It.IsAny<int>(), "OrdenServicio", "cmbVehiculos"))
                .ReturnsAsync(ayuda);

            vm.EstablecerRolActual(1);
            vm.CargarAyudaPorControl("cmbVehiculos");

            // dar tiempo al async void
            await Task.Delay(100);

            Assert.NotNull(vm.AyudaActual);
        }

        [Fact]
        public async Task CargarAyudaPorControl_SinAyuda_AyudaActualNull()
        {
            var (vm, mockAyuda) = CrearViewModel();
            mockAyuda.Setup(s => s.ObtenerAyudaAsync(It.IsAny<int>(), "OrdenServicio", "txtKm"))
                .ReturnsAsync((AyudaContextoDto?)null);

            vm.EstablecerRolActual(1);
            vm.CargarAyudaPorControl("txtKm");

            await Task.Delay(100);

            Assert.Null(vm.AyudaActual);
        }

        [Fact]
        public void CargarAyudaPorControl_ServicioNull_NoLanza()
        {
            var mockRepoReg = new Mock<IRegistroServicioRepository>();
            var mockMapper = new Mock<IMapper>();
            var mockListar = new Mock<ListarRegistroServiciosHandler>(mockRepoReg.Object, mockMapper.Object);
            var mockRegistrar = new Mock<RegistrarRegistroServicioHandler>(mockRepoReg.Object, mockMapper.Object);
            var mockModificar = new Mock<ModificarRegistroServicioHandler>(mockRepoReg.Object, new Mock<IFacturaRepository>().Object, mockMapper.Object);

            var vm = new OrdenServicioViewModel(
                mockListar.Object,
                mockRegistrar.Object,
                mockModificar.Object,
                new Mock<IVehiculoRepository>().Object,
                new Mock<ITallerRepository>().Object,
                new Mock<IServicioRepository>().Object,
                new Mock<IUsuarioRepository>().Object,
                new Mock<IClienteRepository>().Object,
                null!,
                null!,
                null!,
                mockMapper.Object
            );

            // No debe lanzar
            vm.CargarAyudaPorControl("cmbVehiculos");
        }
    }
}