// walterg20
using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;
using TP_ControlVehicular.Negocio.Services;
using TP_ControlVehicular.Presentacion.ViewModels;
using Xunit;

namespace TP_ControlVehicular.Tests.ViewModels
{
    public class OrdenServicioCombosBusquedaTests
    {
        private OrdenServicioViewModel CrearViewModel()
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

            return vm;
        }

        private void CargarDatosPrueba(OrdenServicioViewModel vm)
        {
            vm.ClientesDisponibles.Clear();
            vm.ClientesDisponibles.Add(new ClienteDto { IdCliente = 1, Nombre = "Juan", Apellido = "Perez", Dni = "12345678", Activo = true });
            vm.ClientesDisponibles.Add(new ClienteDto { IdCliente = 2, Nombre = "Carlos", Apellido = "Lopez", Dni = "87654321", Activo = true });
            vm.ClientesDisponibles.Add(new ClienteDto { IdCliente = 3, Nombre = "Maria", Apellido = "Gomez", Dni = "45678901", Activo = true });

            vm.ServiciosDisponibles.Clear();
            vm.ServiciosDisponibles.Add(new ServicioDto { IdServicio = 1, Nombre = "Cambio de Aceite", Precio = 5000, Activo = true });
            vm.ServiciosDisponibles.Add(new ServicioDto { IdServicio = 2, Nombre = "Alineación y Balanceo", Precio = 8000, Activo = true });
            vm.ServiciosDisponibles.Add(new ServicioDto { IdServicio = 3, Nombre = "Frenos Delanteros", Precio = 12000, Activo = true });
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("a")]
        [InlineData("ju")]
        [InlineData("12")]
        public void FiltrarClientes_ConMenosDe3Caracteres_NoActivaFiltro(string texto)
        {
            var vm = CrearViewModel();
            CargarDatosPrueba(vm);

            vm.FiltrarClientes(texto);

            Assert.Equal(3, vm.ClientesDisponibles.Count);
        }

        [Fact]
        public void FiltrarClientes_Con3OMasCaracteres_FiltraPorNombre()
        {
            var vm = CrearViewModel();
            CargarDatosPrueba(vm);

            vm.FiltrarClientes("Jua");

            Assert.Single(vm.ClientesDisponibles);
            Assert.Equal("Juan", vm.ClientesDisponibles.First().Nombre);
        }

        [Fact]
        public void FiltrarClientes_Con3OMasCaracteres_FiltraPorApellido()
        {
            var vm = CrearViewModel();
            CargarDatosPrueba(vm);

            vm.FiltrarClientes("lopez");

            Assert.Single(vm.ClientesDisponibles);
            Assert.Equal("Carlos", vm.ClientesDisponibles.First().Nombre);
            Assert.Equal("Lopez", vm.ClientesDisponibles.First().Apellido);
        }

        [Fact]
        public void FiltrarClientes_Con3OMasCaracteres_FiltraPorDni()
        {
            var vm = CrearViewModel();
            CargarDatosPrueba(vm);

            vm.FiltrarClientes("876");

            Assert.Single(vm.ClientesDisponibles);
            Assert.Equal("87654321", vm.ClientesDisponibles.First().Dni);
        }

        [Fact]
        public void FiltrarClientes_AlBorrarATextoMenorA3_RestauraColeccionCompleta()
        {
            var vm = CrearViewModel();
            CargarDatosPrueba(vm);

            vm.FiltrarClientes("Carlos");
            Assert.Single(vm.ClientesDisponibles);

            // Borra hasta 2 caracteres
            vm.FiltrarClientes("Ca");
            Assert.Equal(3, vm.ClientesDisponibles.Count);

            // Borra completamente
            vm.FiltrarClientes(string.Empty);
            Assert.Equal(3, vm.ClientesDisponibles.Count);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("al")]
        [InlineData("fr")]
        public void FiltrarServicios_ConMenosDe3Caracteres_MuestraCatalogoCompleto(string? texto)
        {
            var vm = CrearViewModel();
            CargarDatosPrueba(vm);

            vm.FiltrarServicios(texto);

            Assert.Equal(3, vm.ServiciosDisponibles.Count);
        }

        [Fact]
        public void FiltrarServicios_Con3OMasCaracteres_FiltraPorNombre()
        {
            var vm = CrearViewModel();
            CargarDatosPrueba(vm);

            vm.FiltrarServicios("aceite");

            Assert.Single(vm.ServiciosDisponibles);
            Assert.Equal("Cambio de Aceite", vm.ServiciosDisponibles.First().Nombre);
        }

        [Fact]
        public void FiltrarServicios_AlBorrarATextoMenorA3_RestauraCatalogoCompleto()
        {
            var vm = CrearViewModel();
            CargarDatosPrueba(vm);

            vm.FiltrarServicios("Frenos");
            Assert.Single(vm.ServiciosDisponibles);

            // Borra a menos de 3 caracteres
            vm.FiltrarServicios("Fr");
            Assert.Equal(3, vm.ServiciosDisponibles.Count);
        }
    }
}
