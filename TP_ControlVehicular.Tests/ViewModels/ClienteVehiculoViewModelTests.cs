// walterg20
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;
using TP_ControlVehicular.Negocio.Mappers;
using TP_ControlVehicular.Negocio.Services;
using TP_ControlVehicular.Presentacion.ViewModels;
using Xunit;

namespace TP_ControlVehicular.Tests.ViewModels
{
    /// <summary>
    /// Pruebas del ViewModel del modal unificado de alta (spec
    /// busqueda-universal-alta-unificada): validacion de cliente y vehiculo, y
    /// registro atomico de cliente + vehiculo + vinculo de propiedad.
    /// </summary>
    public class ClienteVehiculoViewModelTests
    {
        private static IMapper CrearMapper() =>
            new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>(), NullLoggerFactory.Instance).CreateMapper();

        private (ClienteVehiculoViewModel vm,
                  Mock<IClienteRepository> repoClientes,
                  Mock<IVehiculoRepository> repoVehiculos,
                  Mock<IRepository<PropietarioVehiculo>> repoPropietarios) CrearViewModel()
        {
            var mapper = CrearMapper();

            var repoClientes = new Mock<IClienteRepository>();
            var repoVehiculos = new Mock<IVehiculoRepository>();
            var repoPropietarios = new Mock<IRepository<PropietarioVehiculo>>();

            var vm = new ClienteVehiculoViewModel(
                repoClientes.Object,
                repoVehiculos.Object,
                new ListarModelosHandler(new Mock<IModeloRepository>().Object, mapper),
                new ListarMarcaHandler(new Mock<IMarcaRepository>().Object, mapper),
                repoPropietarios.Object,
                mapper);

            return (vm, repoClientes, repoVehiculos, repoPropietarios);
        }

        private static void CargarDatosValidos(ClienteVehiculoViewModel vm)
        {
            vm.Dni = "12345678";
            vm.Apellido = "Perez";
            vm.Nombre = "Juan";
            vm.Telefono = "3515551234";
            vm.Patente = "AA123BC";
            vm.IdMarca = 1;
            vm.IdModelo = 2;
            vm.Anio = 2020;
        }

        [Fact]
        public void ValidateAll_SinDatos_ReportaErroresDeClienteYVehiculo()
        {
            var (vm, _, _, _) = CrearViewModel();

            bool ok = vm.ValidateAll();

            Assert.False(ok);
            Assert.True(vm.HasErrors);
        }

        [Fact]
        public void ValidateAll_SinPatente_ReportaElErrorDePatente()
        {
            var (vm, _, _, _) = CrearViewModel();
            CargarDatosValidos(vm);
            vm.Patente = "";

            bool ok = vm.ValidateAll();

            Assert.False(ok);
            Assert.Contains(vm.GetErrors(nameof(vm.Patente)).Cast<string>(),
                e => e.Contains("Patente", System.StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void ValidateAll_SinMarcaNiModelo_ReportaLosErroresCorrespondientes()
        {
            var (vm, _, _, _) = CrearViewModel();
            CargarDatosValidos(vm);
            vm.IdMarca = 0;
            vm.IdModelo = 0;

            bool ok = vm.ValidateAll();

            Assert.False(ok);
            Assert.NotEmpty(vm.GetErrors(nameof(vm.IdMarca)));
            Assert.NotEmpty(vm.GetErrors(nameof(vm.IdModelo)));
        }

        [Fact]
        public void ValidateAll_ConDatosCompletos_NoReportaErrores()
        {
            var (vm, _, _, _) = CrearViewModel();
            CargarDatosValidos(vm);

            bool ok = vm.ValidateAll();

            Assert.True(ok);
            Assert.False(vm.HasErrors);
        }

        [Fact]
        public async Task RegistrarAsync_ConDatosValidos_InsertaClienteVehiculoYVinculo()
        {
            var (vm, repoClientes, repoVehiculos, repoPropietarios) = CrearViewModel();
            CargarDatosValidos(vm);

            // Los repositorios simulan el identity de la base: asignan el Id a la entidad.
            repoClientes.Setup(r => r.AddAsync(It.IsAny<Cliente>()))
                .Callback<Cliente>(c => c.Id = 7)
                .Returns(Task.CompletedTask);
            repoVehiculos.Setup(r => r.AddAsync(It.IsAny<Vehiculo>()))
                .Callback<Vehiculo>(v => v.Id = 9)
                .Returns(Task.CompletedTask);

            bool ok = await vm.RegistrarAsync();

            Assert.True(ok);

            repoClientes.Verify(r => r.AddAsync(It.Is<Cliente>(c =>
                c.Nombre == "Juan" && c.Apellido == "Perez" && c.Dni == "12345678" && c.Telefono == "3515551234")), Times.Once);

            repoVehiculos.Verify(r => r.AddAsync(It.Is<Vehiculo>(v =>
                v.Patente == "AA123BC" && v.ModeloId == 2 && v.Anio == 2020)), Times.Once);

            repoPropietarios.Verify(r => r.AddAsync(It.Is<PropietarioVehiculo>(p =>
                p.ClienteId == 7 && p.VehiculoId == 9 && p.EsActual)), Times.Once);

            Assert.NotNull(vm.ClienteCreado);
            Assert.Equal(7, vm.ClienteCreado!.IdCliente);
            Assert.NotNull(vm.VehiculoCreado);
            Assert.Equal(9, vm.VehiculoCreado!.Id);
        }

        [Fact]
        public async Task RegistrarAsync_ConClienteExistente_NoCreaUnClienteNuevo()
        {
            var (vm, repoClientes, repoVehiculos, repoPropietarios) = CrearViewModel();
            vm.Reiniciar(new TP_ControlVehicular.Negocio.DTOs.ClienteDto
            {
                IdCliente = 42,
                Nombre = "Juan",
                Apellido = "Perez",
                Dni = "12345678",
                Telefono = "3515551234"
            });

            // El vehiculo se completa despues del reinicio (el reinicio limpia el formulario).
            vm.Patente = "AA123BC";
            vm.IdMarca = 1;
            vm.IdModelo = 2;
            vm.Anio = 2020;

            repoVehiculos.Setup(r => r.AddAsync(It.IsAny<Vehiculo>()))
                .Callback<Vehiculo>(v => v.Id = 9)
                .Returns(Task.CompletedTask);

            bool ok = await vm.RegistrarAsync();

            Assert.True(ok);
            repoClientes.Verify(r => r.AddAsync(It.IsAny<Cliente>()), Times.Never);
            repoPropietarios.Verify(r => r.AddAsync(It.Is<PropietarioVehiculo>(p =>
                p.ClienteId == 42 && p.VehiculoId == 9)), Times.Once);
            Assert.Equal(42, vm.ClienteCreado!.IdCliente);
        }

        [Fact]
        public async Task RegistrarAsync_SinValidar_NoPersisteNada()
        {
            var (vm, repoClientes, repoVehiculos, repoPropietarios) = CrearViewModel();

            bool ok = await vm.RegistrarAsync();

            Assert.False(ok);
            repoClientes.Verify(r => r.AddAsync(It.IsAny<Cliente>()), Times.Never);
            repoVehiculos.Verify(r => r.AddAsync(It.IsAny<Vehiculo>()), Times.Never);
            repoPropietarios.Verify(r => r.AddAsync(It.IsAny<PropietarioVehiculo>()), Times.Never);
        }

        [Fact]
        public async Task RegistrarAsync_CuandoFallaLaBase_NotificaElErrorYNoCreaElVinculo()
        {
            var (vm, repoClientes, repoVehiculos, repoPropietarios) = CrearViewModel();
            CargarDatosValidos(vm);

            repoClientes.Setup(r => r.AddAsync(It.IsAny<Cliente>()))
                .ThrowsAsync(new System.InvalidOperationException("Error de base de datos"));

            string? error = null;
            vm.RegistrationFailed += (s, e) => error = e;

            bool ok = await vm.RegistrarAsync();

            Assert.False(ok);
            Assert.Contains("Error de base de datos", error);
            repoPropietarios.Verify(r => r.AddAsync(It.IsAny<PropietarioVehiculo>()), Times.Never);
        }

        [Fact]
        public void Reiniciar_LimpiaElEstadoParaUnaNuevaApertura()
        {
            var (vm, _, _, _) = CrearViewModel();
            CargarDatosValidos(vm);

            vm.Reiniciar();

            Assert.Equal(string.Empty, vm.Dni);
            Assert.Equal(string.Empty, vm.Apellido);
            Assert.Equal(string.Empty, vm.Nombre);
            Assert.Equal(string.Empty, vm.Telefono);
            Assert.Equal(string.Empty, vm.Patente);
            Assert.Equal(0, vm.IdMarca);
            Assert.Equal(0, vm.IdModelo);
            Assert.Null(vm.ClienteExistente);
            Assert.False(vm.UsaClienteExistente);
            Assert.True(vm.PuedeEditarCliente);
        }
    }
}
