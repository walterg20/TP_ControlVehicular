// walterg20
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;
using TP_ControlVehicular.Negocio.Mappers;
using TP_ControlVehicular.Negocio.Services;
using TP_ControlVehicular.Presentacion.ViewModels;
using Xunit;

namespace TP_ControlVehicular.Tests.ViewModels
{
    /// <summary>
    /// Pruebas de la Fase 2 de la spec paginacion-listados: paginacion y busqueda
    /// en base de datos para los listados CRUD (Marca, Modelo, Vehiculo, Rol,
    /// Usuario, Servicio y Taller).
    /// </summary>
    public class ListadosCrudPaginacionTests
    {
        private static IMapper CrearMapper() =>
            new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>(), NullLoggerFactory.Instance).CreateMapper();

        // ------------------------------------------------------------------
        // MARCA
        // ------------------------------------------------------------------

        [Fact]
        public async Task Marca_LoadAsync_CargaPrimeraPaginaYPropagaElTotal()
        {
            var repo = new Mock<IMarcaRepository>();
            repo.Setup(r => r.GetPaginadoAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((string? b, string? f, int p, int t) => (
                    new List<Marca> { new Marca { Id = 1, NombreMarca = "Toyota", Activo = true } }, 42));

            var mapper = CrearMapper();
            var vm = new MarcaViewModel(
                new ListarMarcaHandler(repo.Object, mapper),
                new RegistrarMarcaHandler(repo.Object, mapper),
                new ModificarMarcaHandler(repo.Object, mapper),
                new EliminarMarcaHandler(repo.Object));

            await vm.LoadAsync();

            repo.Verify(r => r.GetPaginadoAsync("", "Activos", 1, 10), Times.Once);
            Assert.Equal(1, vm.PaginaActual);
            Assert.Equal(42, vm.TotalRegistros);
            Assert.Equal(5, vm.TotalPaginas); // ceil(42 / 10)
            Assert.Single(vm.Marcas);
        }

        [Fact]
        public async Task Marca_BuscarAsync_EnviaElTerminoRecortadoAlRepositorio()
        {
            var repo = new Mock<IMarcaRepository>();
            repo.Setup(r => r.GetPaginadoAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((string? b, string? f, int p, int t) => (Enumerable.Empty<Marca>(), 0));

            var mapper = CrearMapper();
            var vm = new MarcaViewModel(
                new ListarMarcaHandler(repo.Object, mapper),
                new RegistrarMarcaHandler(repo.Object, mapper),
                new ModificarMarcaHandler(repo.Object, mapper),
                new EliminarMarcaHandler(repo.Object));

            vm.TextoBusqueda = "  toyota  ";
            await vm.BuscarAsync();

            repo.Verify(r => r.GetPaginadoAsync("toyota", "Activos", 1, 10), Times.Once);
        }

        // ------------------------------------------------------------------
        // MODELO
        // ------------------------------------------------------------------

        [Fact]
        public async Task Modelo_LoadAsync_CargaPrimeraPaginaYPropagaElTotal()
        {
            var repo = new Mock<IModeloRepository>();
            repo.Setup(r => r.GetPaginadoAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((string? b, string? f, int p, int t) => (
                    new List<Modelo> { new Modelo { Id = 1, NombreModelo = "Corolla", Marca = new Marca { Id = 1, NombreMarca = "Toyota" }, Activo = true } }, 12));

            var marcaRepo = new Mock<IMarcaRepository>();
            marcaRepo.Setup(r => r.GetActivosAsync()).ReturnsAsync(new List<Marca>());

            var mapper = CrearMapper();
            var vm = new ModeloViewModel(
                new ListarModelosHandler(repo.Object, mapper),
                new RegistrarModeloHandler(repo.Object, mapper),
                new ModificarModeloHandler(repo.Object, mapper),
                new EliminarModeloHandler(repo.Object),
                new ListarMarcaHandler(marcaRepo.Object, mapper));

            await vm.LoadAsync();

            repo.Verify(r => r.GetPaginadoAsync("", "Activos", 1, 10), Times.Once);
            Assert.Equal(12, vm.TotalRegistros);
            Assert.Equal(2, vm.TotalPaginas);
            Assert.Single(vm.Modelos);
            Assert.Equal("Toyota", vm.Modelos[0].MarcaNombre);
        }

        // ------------------------------------------------------------------
        // VEHICULO
        // ------------------------------------------------------------------

        [Fact]
        public async Task Vehiculo_LoadAsync_CargaPrimeraPaginaYPropagaElTotal()
        {
            var repo = new Mock<IVehiculoRepository>();
            repo.Setup(r => r.GetPaginadoAsync(It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((string? b, int p, int t) => (
                    new List<Vehiculo> { new Vehiculo { Id = 1, Patente = "AB123CD", Modelo = new Modelo { Id = 1, NombreModelo = "Corolla", Marca = new Marca { Id = 1, NombreMarca = "Toyota" } } } }, 7));

            var clienteRepo = new Mock<IClienteRepository>();
            clienteRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cliente>());

            var modeloRepo = new Mock<IModeloRepository>();
            modeloRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Modelo>());

            var marcaRepo = new Mock<IMarcaRepository>();
            marcaRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Marca>());

            var mapper = CrearMapper();
            var vm = new VehiculoViewModel(
                new ListarVehiculosHandler(repo.Object, mapper),
                new RegistrarVehiculoHandler(repo.Object, mapper),
                new ModificarVehiculoHandler(repo.Object, mapper),
                new EliminarVehiculoHandler(repo.Object),
                clienteRepo.Object,
                new ListarModelosHandler(modeloRepo.Object, mapper),
                new ListarMarcaHandler(marcaRepo.Object, mapper),
                mapper);

            await vm.LoadAsync();

            repo.Verify(r => r.GetPaginadoAsync("", 1, 10), Times.Once);
            Assert.Equal(7, vm.TotalRegistros);
            Assert.Single(vm.Vehiculos);
            Assert.False(vm.IsListEmpty);
        }

        // ------------------------------------------------------------------
        // ROL
        // ------------------------------------------------------------------

        [Fact]
        public async Task Rol_LoadAsync_CargaPrimeraPaginaYPropagaElTotal()
        {
            var repo = new Mock<IRolRepository>();
            repo.Setup(r => r.GetPaginadoAsync(It.IsAny<string?>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((string? b, bool inactivos, int p, int t) => (
                    new List<Rol> { new Rol { Id = 1, Nombre = "Administrador" } }, 4));

            var mapper = CrearMapper();
            var vm = new RolViewModel(
                new ListarRolHandler(repo.Object, mapper),
                new RegistrarRolHandler(repo.Object, mapper),
                new ModificarRolHandler(repo.Object, mapper),
                new ReactivarRolHandler(repo.Object),
                mapper);

            await vm.LoadAsync();

            repo.Verify(r => r.GetPaginadoAsync("", false, 1, 10), Times.Once);
            Assert.Equal(4, vm.TotalRegistros);
            Assert.Single(vm.Roles);
        }

        [Fact]
        public async Task Rol_CambiarMostrarInactivos_ReiniciaLaPaginaYPropagaElFiltro()
        {
            var repo = new Mock<IRolRepository>();
            repo.Setup(r => r.GetPaginadoAsync(It.IsAny<string?>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((string? b, bool inactivos, int p, int t) => (Enumerable.Empty<Rol>(), 0));

            var mapper = CrearMapper();
            var vm = new RolViewModel(
                new ListarRolHandler(repo.Object, mapper),
                new RegistrarRolHandler(repo.Object, mapper),
                new ModificarRolHandler(repo.Object, mapper),
                new ReactivarRolHandler(repo.Object),
                mapper);

            await vm.LoadAsync();
            await vm.CambiarPaginaAsync(1);

            vm.MostrarInactivos = true;
            Assert.Equal(1, vm.PaginaActual);

            await vm.BuscarAsync();
            repo.Verify(r => r.GetPaginadoAsync(It.IsAny<string?>(), true, 1, 10), Times.Once);
        }

        // ------------------------------------------------------------------
        // USUARIO
        // ------------------------------------------------------------------

        [Fact]
        public async Task Usuario_LoadAsync_CargaPrimeraPaginaYPropagaElTotal()
        {
            var repo = new Mock<IUsuarioRepository>();
            repo.Setup(r => r.GetPaginadoAsync(It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((string? b, int p, int t) => (
                    new List<Usuario> { new Usuario { Id = 1, Nombre = "Ana", Apellido = "Perez", Dni = "12345678", Rol = new Rol { Id = 1, Nombre = "Mecanico" } } }, 9));

            var rolRepo = new Mock<IRolRepository>();
            rolRepo.Setup(r => r.GetActivosAsync()).ReturnsAsync(new List<Rol>());

            var mapper = CrearMapper();
            var vm = new UsuarioViewModel(
                new ListarUsuariosHandler(repo.Object, mapper),
                new RegistrarUsuarioHandler(repo.Object, mapper),
                new ModificarUsuarioHandler(repo.Object, mapper),
                rolRepo.Object,
                mapper);

            await vm.LoadAsync();

            repo.Verify(r => r.GetPaginadoAsync("", 1, 10), Times.Once);
            Assert.Equal(9, vm.TotalRegistros);
            Assert.Single(vm.Usuarios);
            Assert.Equal("Mecanico", vm.Usuarios[0].RolNombre);
        }

        // ------------------------------------------------------------------
        // SERVICIO
        // ------------------------------------------------------------------

        [Fact]
        public async Task Servicio_LoadAsync_CargaPrimeraPaginaYPropagaElTotal()
        {
            var repo = new Mock<IServicioRepository>();
            repo.Setup(r => r.GetPaginadoAsync(It.IsAny<string?>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((string? b, bool inactivos, int p, int t) => (
                    new List<Servicio> { new Servicio { Id = 1, Nombre = "Cambio de aceite", Precio = 25000m } }, 15));

            var mapper = CrearMapper();
            var vm = new ServicioViewModel(
                new ListarServiciosHandler(repo.Object, mapper),
                new RegistrarServicioHandler(repo.Object, mapper),
                new ModificarServicioHandler(repo.Object, mapper),
                new EliminarServicioHandler(repo.Object),
                new ReactivarServicioHandler(repo.Object));

            await vm.LoadAsync();

            repo.Verify(r => r.GetPaginadoAsync("", false, 1, 10), Times.Once);
            Assert.Equal(15, vm.TotalRegistros);
            Assert.Equal(2, vm.TotalPaginas);
            Assert.Single(vm.Servicios);
        }

        [Fact]
        public void Servicio_ServicioSeleccionado_NotificaCambioYLimpiarFormularioLoResetea()
        {
            var repo = new Mock<IServicioRepository>();
            var mapper = CrearMapper();
            var vm = new ServicioViewModel(
                new ListarServiciosHandler(repo.Object, mapper),
                new RegistrarServicioHandler(repo.Object, mapper),
                new ModificarServicioHandler(repo.Object, mapper),
                new EliminarServicioHandler(repo.Object),
                new ReactivarServicioHandler(repo.Object));

            var dto = new ServicioDto { IdServicio = 5, Nombre = "Alineación", Precio = 12000m, Activo = true };
            vm.ServicioSeleccionado = dto;
            Assert.Equal(dto, vm.ServicioSeleccionado);

            vm.LimpiarFormulario();
            Assert.Null(vm.ServicioSeleccionado);
            Assert.Equal(0, vm.IdServicio);
            Assert.Empty(vm.Nombre);
        }

        [Fact]
        public async Task Servicio_SaveAsync_ConIdExistente_LlamaModificarServicioHandler()
        {
            var repo = new Mock<IServicioRepository>();
            repo.Setup(r => r.GetByIdAsync(5))
                .ReturnsAsync(new Servicio { Id = 5, Nombre = "Balanceo", Precio = 8000m, Activo = true });
            repo.Setup(r => r.GetPaginadoAsync(It.IsAny<string?>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((string? b, bool inactivos, int p, int t) => (new List<Servicio>(), 0));

            var mapper = CrearMapper();
            var vm = new ServicioViewModel(
                new ListarServiciosHandler(repo.Object, mapper),
                new RegistrarServicioHandler(repo.Object, mapper),
                new ModificarServicioHandler(repo.Object, mapper),
                new EliminarServicioHandler(repo.Object),
                new ReactivarServicioHandler(repo.Object));

            vm.IdServicio = 5;
            vm.Nombre = "Balanceo Computarizado";
            vm.Precio = 9500m;
            vm.Activo = true;

            var ok = await vm.SaveAsync();

            Assert.True(ok);
            repo.Verify(r => r.UpdateAsync(It.Is<Servicio>(s => s.Id == 5 && s.Nombre == "Balanceo Computarizado" && s.Precio == 9500m)), Times.Once);
        }

        // ------------------------------------------------------------------
        // TALLER
        // ------------------------------------------------------------------

        [Fact]
        public async Task Taller_LoadAsync_CargaPrimeraPaginaYPropagaElTotal()
        {
            var repo = new Mock<ITallerRepository>();
            repo.Setup(r => r.GetPaginadoAsync(It.IsAny<string?>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((string? b, bool inactivos, int p, int t) => (
                    new List<Taller> { new Taller { Id = 1, Nombre = "Taller Central", Direccion = "Av. Siempre Viva 742" } }, 3));

            var mapper = CrearMapper();
            var vm = new TallerViewModel(
                new ListarTallerHandler(repo.Object, mapper),
                new RegistrarTallerHandler(repo.Object, mapper),
                new ModificarTallerHandler(repo.Object, mapper),
                new ReactivarTallerHandler(repo.Object),
                mapper);

            await vm.LoadAsync();

            repo.Verify(r => r.GetPaginadoAsync("", false, 1, 10), Times.Once);
            Assert.Equal(3, vm.TotalRegistros);
            Assert.Single(vm.Talleres);
        }

        [Fact]
        public async Task Taller_CambiarElTextoDeBusqueda_ReiniciaElListadoEnLaPagina1()
        {
            var repo = new Mock<ITallerRepository>();
            repo.Setup(r => r.GetPaginadoAsync(It.IsAny<string?>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((string? b, bool inactivos, int p, int t) => (Enumerable.Empty<Taller>(), 0));

            var mapper = CrearMapper();
            var vm = new TallerViewModel(
                new ListarTallerHandler(repo.Object, mapper),
                new RegistrarTallerHandler(repo.Object, mapper),
                new ModificarTallerHandler(repo.Object, mapper),
                new ReactivarTallerHandler(repo.Object),
                mapper);

            await vm.LoadAsync();
            await vm.CambiarPaginaAsync(1);

            vm.TextoBusqueda = "central";

            Assert.Equal(1, vm.PaginaActual);
        }
    }
}
