// walterg20
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TP_ControlVehicular.Datos.Repositories;
using TP_ControlVehicular.Tests.EndToEnd.Infra;
using Xunit;

namespace TP_ControlVehicular.Tests.EndToEnd
{
    /// <summary>
    /// Pruebas end-to-end de la paginacion en base de datos de las pestanas del
    /// Reporte Gerencial (Top Clientes, Modelos Mas Reparados y Servicios por Rango).
    /// </summary>
    [Collection("e2e-sql")]
    public class ReporteGerencialPaginacionTests
    {
        private static ReporteGerencialRepository CrearRepo()
        {
            var configuracion = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = TestRunContext.ConnectionString
                })
                .Build();

            return new ReporteGerencialRepository(configuracion);
        }

        [RequiereSqlServerFact]
        public async Task ObtenerTopClientesPaginadoAsync_DevuelveLaPaginaYElTotal()
        {
            var repo = CrearRepo();

            var (items, total) = await repo.ObtenerTopClientesPaginadoAsync(null, null, 1, 5);

            Assert.True(total > 5, $"El ranking deberia tener mas de 5 clientes, fue {total}.");
            Assert.Equal(5, items.Count());
        }

        [RequiereSqlServerFact]
        public async Task ObtenerTopClientesPaginadoAsync_SegundaPaginaDevuelveRegistrosDistintos()
        {
            var repo = CrearRepo();

            var (primera, total) = await repo.ObtenerTopClientesPaginadoAsync(null, null, 1, 5);
            var (segunda, _) = await repo.ObtenerTopClientesPaginadoAsync(null, null, 2, 5);

            Assert.True(total >= 10, $"El ranking deberia tener al menos 10 clientes, fue {total}.");
            Assert.Equal(5, segunda.Count());
            Assert.Empty(primera.Select(i => i.ClienteId).Intersect(segunda.Select(i => i.ClienteId)));
        }

        [RequiereSqlServerFact]
        public async Task ObtenerTopClientesPaginadoAsync_ConTamanoCeroDevuelveElConjuntoCompleto()
        {
            var repo = CrearRepo();

            var (todos, total) = await repo.ObtenerTopClientesPaginadoAsync(null, null, 1, 0);

            Assert.Equal(total, todos.Count());
        }

        [RequiereSqlServerFact]
        public async Task ObtenerModelosMasReparadosPaginadoAsync_DevuelveLaPaginaYElTotal()
        {
            var repo = CrearRepo();

            var (items, total) = await repo.ObtenerModelosMasReparadosPaginadoAsync(null, null, 1, 4);

            Assert.True(total > 4, $"El ranking deberia tener mas de 4 modelos, fue {total}.");
            Assert.Equal(4, items.Count());
        }

        [RequiereSqlServerFact]
        public async Task ObtenerServiciosPorRangoPaginadoAsync_DevuelveLaPaginaYElTotal()
        {
            var repo = CrearRepo();

            var (items, total) = await repo.ObtenerServiciosPorRangoPaginadoAsync(null, null, 1, 5);

            Assert.True(total > 5, $"Deberian existir mas de 5 servicios con cantidad, fue {total}.");
            Assert.Equal(5, items.Count());
        }

        [RequiereSqlServerFact]
        public async Task ObtenerServiciosPorRangoPaginadoAsync_ConTamanoCeroDevuelveElConjuntoCompleto()
        {
            var repo = CrearRepo();

            var (todos, total) = await repo.ObtenerServiciosPorRangoPaginadoAsync(null, null, 1, 0);

            Assert.Equal(total, todos.Count());
        }
    }
}
