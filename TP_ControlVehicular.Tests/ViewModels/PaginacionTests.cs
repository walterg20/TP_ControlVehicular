// walterg20
using System.Threading.Tasks;
using TP_ControlVehicular.Presentacion.ViewModels;
using Xunit;

namespace TP_ControlVehicular.Tests.ViewModels
{
    /// <summary>
    /// Pruebas de la infraestructura de paginacion compartida
    /// (<see cref="BasePaginableViewModel"/>).
    /// </summary>
    public class PaginacionTests
    {
        /// <summary>ViewModel de prueba que simula un listado con un total fijo de registros.</summary>
        private sealed class ViewModelPrueba : BasePaginableViewModel
        {
            public int LlamadasCarga { get; private set; }
            public int TotalSimulado { get; set; } = 25;

            public Task CargarAsync() => CargarPaginaAsync();

            protected override Task CargarPaginaAsync()
            {
                LlamadasCarga++;
                ActualizarTotales(TotalSimulado);
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task Carga_Inicial_ArrancaEnPagina1YCalculaElTotalDePaginas()
        {
            var vm = new ViewModelPrueba();

            await vm.CargarAsync();

            Assert.Equal(1, vm.PaginaActual);
            Assert.Equal(25, vm.TotalRegistros);
            Assert.Equal(3, vm.TotalPaginas); // ceil(25 / 10)
            Assert.Equal(1, vm.LlamadasCarga);
        }

        [Fact]
        public async Task CambiarPagina_AvanzaYRecargaLosDatos()
        {
            var vm = new ViewModelPrueba();
            await vm.CargarAsync();

            await vm.CambiarPaginaAsync(2);

            Assert.Equal(2, vm.PaginaActual);
            Assert.True(vm.PuedeIrAnterior);
            Assert.True(vm.PuedeIrSiguiente);
            Assert.Equal(2, vm.LlamadasCarga);
        }

        [Fact]
        public async Task CambiarPagina_LimitaElIndiceAlRangoValido()
        {
            var vm = new ViewModelPrueba();
            await vm.CargarAsync();

            await vm.CambiarPaginaAsync(99);

            Assert.Equal(3, vm.PaginaActual); // ultimo pagina disponible

            await vm.CambiarPaginaAsync(0);

            Assert.Equal(1, vm.PaginaActual); // primera pagina
        }

        [Fact]
        public async Task CambiarPagina_EnElUltimoDeshabilitaElAvance()
        {
            var vm = new ViewModelPrueba();
            await vm.CargarAsync();

            await vm.CambiarPaginaAsync(3);

            Assert.True(vm.PuedeIrAnterior);
            Assert.False(vm.PuedeIrSiguiente);
            Assert.False(vm.PaginaSiguienteCommand.CanExecute(null));
            Assert.False(vm.IrUltimaPaginaCommand.CanExecute(null));
        }

        [Fact]
        public async Task CambiarPagina_EnLaPrimeraDeshabilitaElRetroceso()
        {
            var vm = new ViewModelPrueba();
            await vm.CargarAsync();

            Assert.False(vm.PuedeIrAnterior);
            Assert.True(vm.PuedeIrSiguiente);
            Assert.False(vm.IrPrimeraPaginaCommand.CanExecute(null));
            Assert.False(vm.PaginaAnteriorCommand.CanExecute(null));
        }

        [Fact]
        public async Task CambiarTamanoPagina_ReiniciaEnPagina1YRecalculaElTotalDePaginas()
        {
            var vm = new ViewModelPrueba();
            await vm.CargarAsync();
            await vm.CambiarPaginaAsync(2);

            vm.TamanoPagina = 20;

            Assert.Equal(1, vm.PaginaActual);
            Assert.Equal(2, vm.TotalPaginas); // ceil(25 / 20)
        }

        [Theory]
        [InlineData(1, 5)]
        [InlineData(0, 5)]
        [InlineData(9999, 200)]
        public async Task TamanoPagina_FueraDeRangoSeNormaliza(int solicitado, int esperado)
        {
            var vm = new ViewModelPrueba();
            await vm.CargarAsync();

            vm.TamanoPagina = solicitado;

            Assert.Equal(esperado, vm.TamanoPagina);
        }

        [Fact]
        public async Task SinRegistros_TotalPaginasEsUnoYTotalEsCero()
        {
            var vm = new ViewModelPrueba { TotalSimulado = 0 };

            await vm.CargarAsync();

            Assert.Equal(1, vm.PaginaActual);
            Assert.Equal(0, vm.TotalRegistros);
            Assert.Equal(1, vm.TotalPaginas);
            Assert.False(vm.PuedeIrAnterior);
            Assert.False(vm.PuedeIrSiguiente);
        }

        [Fact]
        public async Task TextoPaginacion_InformaPaginaTotalPaginasYRegistros()
        {
            var vm = new ViewModelPrueba();
            await vm.CargarAsync();

            Assert.Equal("Página 1 de 3 (25 registros)", vm.TextoPaginacion);

            vm.TotalSimulado = 1;
            await vm.CargarAsync();

            Assert.Equal("Página 1 de 1 (1 registro)", vm.TextoPaginacion);
        }
    }
}
