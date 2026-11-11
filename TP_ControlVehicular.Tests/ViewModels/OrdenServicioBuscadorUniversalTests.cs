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
    /// Pruebas del buscador universal de la orden de servicio (spec
    /// busqueda-universal-alta-unificada): filtro por DNI, apellido, nombre y patente,
    /// agrupacion de clientes con varios vehiculos y fila de accion sin resultados.
    /// </summary>
    public class OrdenServicioBuscadorUniversalTests
    {
        private static IMapper CrearMapper() =>
            new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>(), NullLoggerFactory.Instance).CreateMapper();

        private (OrdenServicioViewModel vm, Mock<IClienteRepository> repoClientes, Mock<IVehiculoRepository> repoVehiculos) CrearViewModel()
        {
            var mockRepoReg = new Mock<IRegistroServicioRepository>();
            var mapper = CrearMapper();

            var repoClientes = new Mock<IClienteRepository>();
            var repoVehiculos = new Mock<IVehiculoRepository>();

            var vm = new OrdenServicioViewModel(
                new ListarRegistroServiciosHandler(mockRepoReg.Object, mapper),
                new RegistrarRegistroServicioHandler(mockRepoReg.Object, mapper),
                new ModificarRegistroServicioHandler(mockRepoReg.Object, new Mock<IFacturaRepository>().Object, mapper),
                repoVehiculos.Object,
                new Mock<ITallerRepository>().Object,
                new Mock<IServicioRepository>().Object,
                new Mock<IUsuarioRepository>().Object,
                repoClientes.Object,
                null!,
                null!,
                null!,
                mapper);

            return (vm, repoClientes, repoVehiculos);
        }

        private static Cliente EntidadCliente(int id, string nombre, string apellido, string dni)
        {
            return new Cliente
            {
                Id = id,
                Nombre = nombre,
                Apellido = apellido,
                Dni = dni,
                Activo = true
            };
        }

        /// <summary>
        /// Vehiculo con su modelo, marca y propietario actual, de modo que el mapeo a
        /// VehiculoDto resuelva IdCliente, MarcaNombre y ModeloNombre.
        /// </summary>
        private static Vehiculo EntidadVehiculo(int id, Cliente propietario, string patente, string marca = "Ford", string modelo = "Fiesta")
        {
            return new Vehiculo
            {
                Id = id,
                Patente = patente,
                Anio = 2020,
                KmActual = 1000,
                Modelo = new Modelo
                {
                    Id = 1,
                    NombreModelo = modelo,
                    Marca = new Marca { Id = 1, NombreMarca = marca }
                },
                Propietarios = new List<PropietarioVehiculo>
                {
                    new PropietarioVehiculo
                    {
                        ClienteId = propietario.Id,
                        VehiculoId = id,
                        Cliente = propietario,
                        EsActual = true,
                        FechaAdquisicion = System.DateTime.Now
                    }
                }
            };
        }

        /// <summary>Carga el catalogo completo (clientes y vehiculos) en el ViewModel.</summary>
        private async Task CargarCatalogo(
            OrdenServicioViewModel vm,
            Mock<IClienteRepository> repoClientes,
            Mock<IVehiculoRepository> repoVehiculos,
            IEnumerable<Cliente> clientes,
            IEnumerable<Vehiculo> vehiculos)
        {
            repoClientes.Setup(r => r.GetAllAsync()).ReturnsAsync(clientes.ToList());
            repoVehiculos.Setup(r => r.GetAllWithDetailsAsync(It.IsAny<int?>())).ReturnsAsync(vehiculos.ToList());

            await vm.LoadCombosAsync();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("a")]
        [InlineData("AA")]
        public async Task FiltrarBusquedaUniversal_ConMenosDe3Caracteres_NoFiltra(string texto)
        {
            var (vm, repoClientes, repoVehiculos) = CrearViewModel();
            var perez = EntidadCliente(1, "Juan", "Perez", "12345678");
            await CargarCatalogo(vm, repoClientes, repoVehiculos,
                new[] { perez },
                new[] { EntidadVehiculo(10, perez, "AA123BC") });

            vm.FiltrarBusquedaUniversal(texto);

            Assert.Empty(vm.ResultadosBusquedaUniversal);
        }

        [Fact]
        public async Task FiltrarBusquedaUniversal_Con3Caracteres_BuscaPorApellido()
        {
            var (vm, repoClientes, repoVehiculos) = CrearViewModel();
            var perez = EntidadCliente(1, "Juan", "Perez", "12345678");
            var lopez = EntidadCliente(2, "Carlos", "Lopez", "87654321");
            await CargarCatalogo(vm, repoClientes, repoVehiculos,
                new[] { perez, lopez },
                new[] { EntidadVehiculo(10, perez, "AA123BC"), EntidadVehiculo(20, lopez, "XY987ZZ") });

            vm.FiltrarBusquedaUniversal("Per");

            var fila = Assert.Single(vm.ResultadosBusquedaUniversal);
            Assert.Equal(1, fila.ClienteId);
            Assert.Equal(10, fila.VehiculoId);
            Assert.Equal("AA123BC", fila.Patente);
        }

        [Fact]
        public async Task FiltrarBusquedaUniversal_Con3Caracteres_BuscaPorDni()
        {
            var (vm, repoClientes, repoVehiculos) = CrearViewModel();
            var perez = EntidadCliente(1, "Juan", "Perez", "12345678");
            var lopez = EntidadCliente(2, "Carlos", "Lopez", "87654321");
            await CargarCatalogo(vm, repoClientes, repoVehiculos,
                new[] { perez, lopez },
                new[] { EntidadVehiculo(10, perez, "AA123BC"), EntidadVehiculo(20, lopez, "XY987ZZ") });

            vm.FiltrarBusquedaUniversal("876");

            var fila = Assert.Single(vm.ResultadosBusquedaUniversal);
            Assert.Equal(2, fila.ClienteId);
            Assert.Equal(20, fila.VehiculoId);
        }

        [Fact]
        public async Task FiltrarBusquedaUniversal_Con3Caracteres_BuscaPorNombre()
        {
            var (vm, repoClientes, repoVehiculos) = CrearViewModel();
            var perez = EntidadCliente(1, "Juan", "Perez", "12345678");
            var lopez = EntidadCliente(2, "Carlos", "Lopez", "87654321");
            await CargarCatalogo(vm, repoClientes, repoVehiculos,
                new[] { perez, lopez },
                new[] { EntidadVehiculo(10, perez, "AA123BC"), EntidadVehiculo(20, lopez, "XY987ZZ") });

            vm.FiltrarBusquedaUniversal("Car");

            var fila = Assert.Single(vm.ResultadosBusquedaUniversal);
            Assert.Equal(2, fila.ClienteId);
        }

        [Fact]
        public async Task FiltrarBusquedaUniversal_BuscaPorPatente()
        {
            var (vm, repoClientes, repoVehiculos) = CrearViewModel();
            var perez = EntidadCliente(1, "Juan", "Perez", "12345678");
            var lopez = EntidadCliente(2, "Carlos", "Lopez", "87654321");
            await CargarCatalogo(vm, repoClientes, repoVehiculos,
                new[] { perez, lopez },
                new[] { EntidadVehiculo(10, perez, "AA123BC"), EntidadVehiculo(20, lopez, "XY987ZZ") });

            vm.FiltrarBusquedaUniversal("AA1");

            var fila = Assert.Single(vm.ResultadosBusquedaUniversal);
            Assert.Equal("AA123BC", fila.Patente);
            Assert.Equal(1, fila.ClienteId);
        }

        [Fact]
        public async Task FiltrarBusquedaUniversal_FormateaLaFilaComoPatenteApellidoNombreDniVehiculo()
        {
            var (vm, repoClientes, repoVehiculos) = CrearViewModel();
            var perez = EntidadCliente(1, "Juan", "Perez", "12345678");
            await CargarCatalogo(vm, repoClientes, repoVehiculos,
                new[] { perez },
                new[] { EntidadVehiculo(10, perez, "AA123BC") });

            vm.FiltrarBusquedaUniversal("Pere");

            var fila = Assert.Single(vm.ResultadosBusquedaUniversal);
            Assert.Equal("[AA123BC] - Perez, Juan (12345678) - Ford Fiesta", fila.DisplayText);
        }

        [Fact]
        public async Task FiltrarBusquedaUniversal_ClienteConVariosVehiculos_AgrupaEnUnaFila()
        {
            var (vm, repoClientes, repoVehiculos) = CrearViewModel();
            var perez = EntidadCliente(1, "Juan", "Perez", "12345678");
            await CargarCatalogo(vm, repoClientes, repoVehiculos,
                new[] { perez },
                new[] { EntidadVehiculo(10, perez, "AA123BC"), EntidadVehiculo(11, perez, "AA456DE") });

            vm.FiltrarBusquedaUniversal("Pere");

            var fila = Assert.Single(vm.ResultadosBusquedaUniversal);
            Assert.True(fila.EsAgrupado);
            Assert.Equal(2, fila.CantidadVehiculos);
            Assert.Equal(0, fila.VehiculoId);
            Assert.Equal("Perez, Juan (DNI: 12345678) - [Ver 2 vehiculos]", fila.DisplayText);
        }

        [Fact]
        public async Task FiltrarBusquedaUniversal_SinCoincidencias_AgregaLaFilaDeAccionRegistrar()
        {
            var (vm, repoClientes, repoVehiculos) = CrearViewModel();
            var perez = EntidadCliente(1, "Juan", "Perez", "12345678");
            await CargarCatalogo(vm, repoClientes, repoVehiculos,
                new[] { perez },
                new[] { EntidadVehiculo(10, perez, "AA123BC") });

            vm.FiltrarBusquedaUniversal("AG 987 ZK");

            var fila = Assert.Single(vm.ResultadosBusquedaUniversal);
            Assert.True(fila.EsAccionRegistrar);
            Assert.Equal("🔍 No se encontraron resultados. + Registrar Nuevo Cliente y Vehiculo", fila.DisplayText);
        }

        [Fact]
        public async Task SeleccionarResultadoUniversal_FilaNormal_CompletaClienteYVehiculo()
        {
            var (vm, repoClientes, repoVehiculos) = CrearViewModel();
            var perez = EntidadCliente(1, "Juan", "Perez", "12345678");
            await CargarCatalogo(vm, repoClientes, repoVehiculos,
                new[] { perez },
                new[] { EntidadVehiculo(10, perez, "AA123BC") });

            var focoKilometraje = false;
            vm.FocoKilometrajeRequested += (s, e) => focoKilometraje = true;

            vm.FiltrarBusquedaUniversal("Pere");
            var fila = vm.ResultadosBusquedaUniversal.First();

            vm.SeleccionarResultadoUniversal(fila);

            Assert.Equal(1, vm.ClienteId);
            Assert.Equal(10, vm.VehiculoId);
            Assert.True(focoKilometraje);
        }

        [Fact]
        public async Task SeleccionarResultadoUniversal_FilaAgrupada_CompletaSoloElCliente()
        {
            var (vm, repoClientes, repoVehiculos) = CrearViewModel();
            var perez = EntidadCliente(1, "Juan", "Perez", "12345678");
            await CargarCatalogo(vm, repoClientes, repoVehiculos,
                new[] { perez },
                new[] { EntidadVehiculo(10, perez, "AA123BC"), EntidadVehiculo(11, perez, "AA456DE") });

            var focoVehiculo = false;
            var focoKilometraje = false;
            vm.FocoVehiculoRequested += (s, e) => focoVehiculo = true;
            vm.FocoKilometrajeRequested += (s, e) => focoKilometraje = true;

            vm.FiltrarBusquedaUniversal("Pere");
            var fila = vm.ResultadosBusquedaUniversal.First();

            vm.SeleccionarResultadoUniversal(fila);

            Assert.Equal(1, vm.ClienteId);
            Assert.Equal(0, vm.VehiculoId);
            Assert.True(focoVehiculo);
            Assert.False(focoKilometraje);
        }

        [Fact]
        public async Task SeleccionarResultadoUniversal_FilaDeAccionDisparaLaAperturaDelModal()
        {
            var (vm, repoClientes, repoVehiculos) = CrearViewModel();
            var perez = EntidadCliente(1, "Juan", "Perez", "12345678");
            await CargarCatalogo(vm, repoClientes, repoVehiculos,
                new[] { perez },
                new[] { EntidadVehiculo(10, perez, "AA123BC") });

            string? textoRecibido = null;
            vm.AbrirModalNuevoClienteVehiculoRequested += (s, texto) => textoRecibido = texto;

            vm.FiltrarBusquedaUniversal("AG 9");
            var fila = vm.ResultadosBusquedaUniversal.First();

            vm.SeleccionarResultadoUniversal(fila);

            Assert.Equal("AG 9", textoRecibido);
        }

        [Theory]
        [InlineData("AG 987 ZK", true)]
        [InlineData("AA123BC", true)]
        [InlineData("Perez", false)]
        [InlineData("12345678", false)]
        [InlineData("AB", false)]
        public void ParecePatente_DistingueLaPatenteDelNombreYDelDni(string texto, bool esperado)
        {
            var (vm, _, _) = CrearViewModel();
            Assert.Equal(esperado, vm.ParecePatente(texto));
        }
    }
}
