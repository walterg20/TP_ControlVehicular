// walterg20
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using TP_ControlVehicular.Datos.Repositories;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;
using TP_ControlVehicular.Negocio.Services;
using TP_ControlVehicular.Presentacion.ViewModels;
using Xunit;

namespace TP_ControlVehicular.Tests.ViewModels
{
    public class OrdenServicioMecanicoAsignacionTests
    {
        private (OrdenServicioViewModel vm, Mock<IUsuarioRepository> mockUsuarioRepo, Mock<RegistrarRegistroServicioHandler> mockRegistrar) CrearViewModel()
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

            mockMapper.Setup(m => m.Map<UsuarioDto>(It.IsAny<Usuario>()))
                .Returns((Usuario u) => new UsuarioDto
                {
                    IdUsuario = u.Id,
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    IdRol = u.RolId,
                    RolNombre = u.Rol?.Nombre ?? string.Empty
                });

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

            return (vm, mockUsuarioRepo, mockRegistrar);
        }

        [Fact]
        public async Task GuardarOrdenAsync_ConDetalleSinMecanico_FallaYDisparaRegistrationFailed()
        {
            var (vm, _, _) = CrearViewModel();

            vm.VehiculoId = 1;
            vm.TallerId = 1;
            vm.KmIngreso = 15000;
            vm.Estado = "Abierta";

            vm.DetallesOrdenActual.Add(new DetalleServicioDto
            {
                ServicioId = 1,
                ServicioNombre = "Alineación",
                UsuarioId = 0, // Mecánico NO asignado
                Precio = 5000,
                Cantidad = 1
            });

            string? mensajeError = null;
            vm.RegistrationFailed += (s, e) => mensajeError = e;

            var resultado = await vm.GuardarOrdenAsync();

            Assert.False(resultado);
            Assert.NotNull(mensajeError);
            Assert.Contains("mecánico", mensajeError, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GuardarOrdenAsync_ConTodosLosDetallesConMecanico_PermiteContinuarGuardado()
        {
            var (vm, _, mockRegistrar) = CrearViewModel();

            vm.VehiculoId = 1;
            vm.TallerId = 1;
            vm.KmIngreso = 15000;
            vm.Estado = "Abierta";

            vm.DetallesOrdenActual.Add(new DetalleServicioDto
            {
                ServicioId = 1,
                ServicioNombre = "Alineación",
                UsuarioId = 3, // Mecánico asignado válidamente
                Precio = 5000,
                Cantidad = 1
            });

            mockRegistrar.Setup(r => r.HandleAsync(It.IsAny<RegistroServicio>()))
                .ReturnsAsync(new RegistroServicioDto());

            var resultado = await vm.GuardarOrdenAsync();

            Assert.True(resultado);
            mockRegistrar.Verify(r => r.HandleAsync(It.Is<RegistroServicio>(o => o.Detalles.All(d => d.UsuarioId == 3))), Times.Once);
        }

        [Fact]
        public async Task LoadCombosAsync_CargaSoloMecanicosActivos_FiltraPorRol()
        {
            var (vm, mockUsuarioRepo, _) = CrearViewModel();

            var usuarios = new List<Usuario>
            {
                new Usuario { Id = 1, Nombre = "Admin", RolId = 1, Rol = new Rol { Id = 1, Nombre = "Administrador" }, Activo = true },
                new Usuario { Id = 2, Nombre = "Recepcionista", RolId = 2, Rol = new Rol { Id = 2, Nombre = "Recepcionista" }, Activo = true },
                new Usuario { Id = 3, Nombre = "Carlos", RolId = 3, Rol = new Rol { Id = 3, Nombre = "Mecánico" }, Activo = true },
                new Usuario { Id = 4, Nombre = "Marcos", RolId = 5, Rol = new Rol { Id = 5, Nombre = "Mecánico Especialista" }, Activo = true }
            };

            mockUsuarioRepo.Setup(r => r.GetActivosAsync())
                .ReturnsAsync(usuarios);

            await vm.LoadCombosAsync();

            Assert.Equal(2, vm.MecanicosDisponibles.Count);
            Assert.Contains(vm.MecanicosDisponibles, m => m.IdUsuario == 3);
            Assert.Contains(vm.MecanicosDisponibles, m => m.IdUsuario == 4);
        }
    }
}
