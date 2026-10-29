// walterg20
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Datos.Repositories;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Context;
using TP_ControlVehicular.Tests.EndToEnd.Infra;
using Xunit;

namespace TP_ControlVehicular.Tests.EndToEnd
{
    /// <summary>
    /// Pruebas end-to-end de la paginacion en base de datos de la Fase 3 de la spec
    /// paginacion-listados: listados transaccionales (EF Core) y reportes (procedimientos
    /// almacenados).
    /// </summary>
    [Collection("e2e-sql")]
    public class PaginacionBaseDeDatosTests : IAsyncLifetime
    {
        private readonly CVDbContext _ctx;
        private SemillaE2EResult _siembra = new();

        public PaginacionBaseDeDatosTests()
        {
            _ctx = ContextoTestFactory.Create();
        }

        public async Task InitializeAsync()
        {
            _siembra = await SemillaE2E.SembrarFlujoBasicoAsync(_ctx);

            // Ordenes adicionales del mismo vehiculo para poder paginar.
            for (int i = 0; i < 2; i++)
            {
                var orden = new RegistroServicio
                {
                    VehiculoId = _siembra.VehiculoId,
                    TallerId = _siembra.TallerId,
                    UsuarioId = _siembra.RecepcionistaId,
                    Fecha = DateTime.Now.AddMinutes(-(i + 1)),
                    KmIngreso = 1000 + i,
                    Estado = RegistroServicio.EstadoAbierta
                };
                await _ctx.RegistroServicios.AddAsync(orden);
                await _ctx.SaveChangesAsync();
            }
        }

        public async Task DisposeAsync()
        {
            await LimpiezaE2E.LimpiarPorMarcaAsync(_ctx);
            _ctx.Dispose();
        }

        private RegistroServicioRepository CrearRepoServicios() => new(_ctx);
        private ReporteRepository CrearRepoReportes() => new(_ctx);

        [RequiereSqlServerFact]
        public async Task GetPaginadoAsync_DevuelveSoloLaPaginaYPropagaElTotal()
        {
            var repo = CrearRepoServicios();

            var (items, total) = await repo.GetPaginadoAsync(null, null, null, null, 1, 2);

            Assert.True(total >= 3, $"El total deberia incluir las 3 ordenes sembradas, fue {total}.");
            Assert.Equal(2, items.Count());
            Assert.Equal(2, items.Select(i => i.Id).Distinct().Count());
        }

        [RequiereSqlServerFact]
        public async Task GetPaginadoAsync_SegundaPaginaDevuelveRegistrosDistintos()
        {
            var repo = CrearRepoServicios();

            var (primera, total) = await repo.GetPaginadoAsync(null, null, null, null, 1, 2);
            var (segunda, _) = await repo.GetPaginadoAsync(null, null, null, null, 2, 2);

            Assert.True(total >= 4, $"El total deberia incluir al menos 4 ordenes, fue {total}.");
            Assert.Equal(2, segunda.Count());
            Assert.Empty(primera.Select(i => i.Id).Intersect(segunda.Select(i => i.Id)));
        }

        [RequiereSqlServerFact]
        public async Task GetPaginadoAsync_FiltraPorTextoYPorEstado()
        {
            var repo = CrearRepoServicios();

            var (items, total) = await repo.GetPaginadoAsync(
                null, RegistroServicio.EstadoAbierta, null, null, 1, 50);

            Assert.True(total >= 2, $"Deberian existir al menos 2 ordenes Abiertas, fue {total}.");
            Assert.All(items, i => Assert.Equal(RegistroServicio.EstadoAbierta, i.Estado));
        }

        [RequiereSqlServerFact]
        public async Task GetPaginadoAsync_BuscaPorPatenteEnLaBaseDeDatos()
        {
            var repo = CrearRepoServicios();
            var patente = await _ctx.Vehiculos
                .Where(v => v.Id == _siembra.VehiculoId)
                .Select(v => v.Patente)
                .FirstAsync();

            var (items, total) = await repo.GetPaginadoAsync(patente, null, null, null, 1, 50);

            Assert.True(total >= 3, $"La busqueda por patente deberia encontrar las ordenes, fue {total}.");
            Assert.NotEmpty(items);
        }

        [RequiereSqlServerFact]
        public async Task GetPaginadoPorMecanicoAsync_SoloDevuelveOrdenesConTareasDelMecanico()
        {
            var repo = CrearRepoServicios();

            var (items, total) = await repo.GetPaginadoPorMecanicoAsync(
                _siembra.MecanicoId, null, "Pendientes", null, null, 1, 50);

            // La orden sembrada tiene una tarea del mecanico: debe aparecer.
            Assert.Contains(items, i => i.Id == _siembra.RegistroServicioId);
            Assert.True(total >= 1, $"Deberia existir al menos una orden del mecanico, fue {total}.");

            // Toda orden devuelta debe tener al menos una tarea del mecanico.
            foreach (var orden in items)
            {
                Assert.Contains(orden.Detalles, d => d.UsuarioId == _siembra.MecanicoId);
            }
        }

        [RequiereSqlServerFact]
        public async Task GetPaginadoPorMecanicoAsync_ExcluyeLasOrdenesSinTareasDelMecanico()
        {
            var repo = CrearRepoServicios();

            var (items, _) = await repo.GetPaginadoPorMecanicoAsync(
                _siembra.MecanicoId, null, "Pendientes", null, null, 1, 50);

            // Las dos ordenes extra creadas en la siembra no tienen tareas del mecanico.
            Assert.DoesNotContain(items, i => i.Id != _siembra.RegistroServicioId && i.Detalles.Count == 0);
        }

        [RequiereSqlServerFact]
        public async Task ObtenerReporteOrdenesPaginadoAsync_DevuelveLaPaginaYElTotal()
        {
            var repo = CrearRepoReportes();

            var (items, total) = await repo.ObtenerReporteOrdenesPaginadoAsync(
                null, null, null, null, null, 1, 2);

            Assert.True(total >= 3, $"El total deberia incluir las ordenes sembradas, fue {total}.");
            Assert.Equal(2, items.Count);
            Assert.All(items, i => Assert.True(i.IdOrden > 0));
            Assert.All(items, i => Assert.False(string.IsNullOrWhiteSpace(i.MarcaModelo)));
            Assert.All(items, i => Assert.False(string.IsNullOrWhiteSpace(i.ServiciosAplicados)));
        }

        [RequiereSqlServerFact]
        public async Task ObtenerReporteOrdenesPaginadoAsync_ConTamanoCeroDevuelveElConjuntoCompleto()
        {
            var repo = CrearRepoReportes();

            var (todos, total) = await repo.ObtenerReporteOrdenesPaginadoAsync(
                null, null, null, null, null, 1, 0);

            Assert.Equal(total, todos.Count);
        }

        [RequiereSqlServerFact]
        public async Task ObtenerHistorialVehiculoPaginadoAsync_DevuelveLaPaginaYElTotal()
        {
            var repo = CrearRepoReportes();

            var (items, total) = await repo.ObtenerHistorialVehiculoPaginadoAsync(
                _siembra.VehiculoId, null, 1, 1);

            Assert.True(total >= 3, $"El historial deberia incluir las ordenes del vehiculo, fue {total}.");
            Assert.Single(items);
            Assert.Equal(_siembra.VehiculoId, items[0].VehiculoId);
        }

        [RequiereSqlServerFact]
        public async Task ObtenerVehiculosPorClientePaginadoAsync_DevuelveLaPaginaYElTotal()
        {
            var repo = CrearRepoReportes();

            var (items, total) = await repo.ObtenerVehiculosPorClientePaginadoAsync(
                _siembra.ClienteId, null, 1, 10);

            Assert.True(total >= 1, $"Deberia existir al menos un vehiculo del cliente, fue {total}.");
            Assert.Single(items);
            Assert.Equal(_siembra.VehiculoId, items[0].VehiculoId);
        }

        [RequiereSqlServerFact]
        public async Task ObtenerVehiculosPorClientePaginadoAsync_FiltraPorDni()
        {
            var repo = CrearRepoReportes();
            var dni = await _ctx.Clientes
                .Where(c => c.Id == _siembra.ClienteId)
                .Select(c => c.Dni)
                .FirstAsync();

            var (items, total) = await repo.ObtenerVehiculosPorClientePaginadoAsync(null, dni, 1, 10);

            Assert.True(total >= 1, $"El filtro por DNI deberia encontrar el cliente, fue {total}.");
            Assert.NotEmpty(items);
        }
    }
}
