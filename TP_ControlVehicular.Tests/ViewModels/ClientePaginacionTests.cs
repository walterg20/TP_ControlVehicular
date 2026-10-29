// walterg20
using System;
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
    /// Pruebas del piloto de paginacion en base de datos: la grilla de clientes.
    /// </summary>
    public class ClientePaginacionTests
    {
        private static IMapper CrearMapper() =>
            new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>(), NullLoggerFactory.Instance).CreateMapper();

        private static List<Cliente> CrearClientes(int cantidad) =>
            Enumerable.Range(1, cantidad).Select(i => new Cliente
            {
                Id = i,
                Nombre = $"Nombre{i}",
                Apellido = $"Apellido{i}",
                Dni = $"1000000{i}",
                Activo = true
            }).ToList();

        private (ClienteViewModel Vm, Mock<IClienteRepository> Repo) CrearViewModel(int totalRegistros = 25, int registrosPorPagina = 3)
        {
            var repo = new Mock<IClienteRepository>();

            repo.Setup(r => r.GetPaginadoAsync(It.IsAny<string?>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((string? busqueda, bool incluirInactivos, int pagina, int tamanoPagina) =>
                {
                    var items = CrearClientes(Math.Min(registrosPorPagina, 3));
                    return (items, totalRegistros);
                });

            var mapper = CrearMapper();
            var handler = new RegistrarClienteHandler(repo.Object, mapper);

            var vm = new ClienteViewModel(handler, repo.Object, mapper);
            return (vm, repo);
        }

        [Fact]
        public async Task LoadAsync_CargaLaPrimeraPaginaYPropagaElTotalDeRegistros()
        {
            var (vm, repo) = CrearViewModel(totalRegistros: 25);

            await vm.LoadAsync();

            repo.Verify(r => r.GetPaginadoAsync(It.IsAny<string?>(), It.IsAny<bool>(), 1, 10), Times.Once);
            Assert.Equal(1, vm.PaginaActual);
            Assert.Equal(25, vm.TotalRegistros);
            Assert.Equal(3, vm.TotalPaginas); // ceil(25 / 10)
            Assert.Equal(3, vm.Clientes.Count);
        }

        [Fact]
        public async Task BuscarAsync_EnviaElTerminoDeBusquedaAlRepositorio()
        {
            var (vm, repo) = CrearViewModel();

            vm.TextoBusqueda = "  juan  ";
            await vm.BuscarAsync();

            repo.Verify(r => r.GetPaginadoAsync("juan", It.IsAny<bool>(), 1, 10), Times.Once);
        }

        [Fact]
        public async Task CambiarPagina_PideElIndiceDePaginaCorrespondiente()
        {
            var (vm, repo) = CrearViewModel();

            await vm.LoadAsync();
            await vm.CambiarPaginaAsync(2);

            repo.Verify(r => r.GetPaginadoAsync(It.IsAny<string?>(), It.IsAny<bool>(), 2, 10), Times.Once);
            Assert.Equal(2, vm.PaginaActual);
        }

        [Fact]
        public async Task CambiarElTextoDeBusqueda_ReiniciaElListadoEnLaPagina1()
        {
            var (vm, _) = CrearViewModel();
            await vm.LoadAsync();
            await vm.CambiarPaginaAsync(3);

            vm.TextoBusqueda = "juan";

            Assert.Equal(1, vm.PaginaActual);
        }

        [Fact]
        public async Task CambiarMostrarInactivos_ReiniciaElListadoEnLaPagina1()
        {
            var (vm, repo) = CrearViewModel();
            await vm.LoadAsync();
            await vm.CambiarPaginaAsync(2);

            vm.MostrarInactivos = true;

            Assert.Equal(1, vm.PaginaActual);

            await vm.BuscarAsync();
            repo.Verify(r => r.GetPaginadoAsync(It.IsAny<string?>(), true, 1, 10), Times.Once);
        }

        [Fact]
        public async Task CambiarElTamanoDePagina_ReiniciaEnPagina1YRepasaElTamanoAlRepositorio()
        {
            var (vm, repo) = CrearViewModel();
            await vm.LoadAsync();
            await vm.CambiarPaginaAsync(2);

            vm.TamanoPagina = 20;

            Assert.Equal(1, vm.PaginaActual);
            repo.Verify(r => r.GetPaginadoAsync(It.IsAny<string?>(), It.IsAny<bool>(), 1, 20), Times.Once);
        }

        [Fact]
        public async Task SinResultados_LaGrillaQuedaVaciaYElTotalEsCero()
        {
            var repo = new Mock<IClienteRepository>();
            repo.Setup(r => r.GetPaginadoAsync(It.IsAny<string?>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((string? busqueda, bool incluirInactivos, int pagina, int tamanoPagina) =>
                    (Enumerable.Empty<Cliente>(), 0));

            var mapper = CrearMapper();
            var handler = new RegistrarClienteHandler(repo.Object, mapper);
            var vm = new ClienteViewModel(handler, repo.Object, mapper);

            await vm.LoadAsync();

            Assert.Empty(vm.Clientes);
            Assert.Equal(0, vm.TotalRegistros);
            Assert.Equal(1, vm.TotalPaginas);
        }
    }
}
