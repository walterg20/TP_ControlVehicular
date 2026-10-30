// walterg20
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using TP_ControlVehicular.Negocio.DTOs.Reportes;
using TP_ControlVehicular.Negocio.Reportes;

namespace TP_ControlVehicular.Datos.Repositories
{
    public class ReporteGerencialRepository : IReporteGerencialRepository
    {
        private readonly string _connectionString;

        public ReporteGerencialRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection'.");
        }

        public async Task<IEnumerable<ReporteIngresoDto>> ObtenerIngresosPorFechaAsync(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@FechaDesde", fechaDesde, DbType.Date);
            parameters.Add("@FechaHasta", fechaHasta, DbType.Date);

            return await connection.QueryAsync<ReporteIngresoDto>(
                "sp_ReporteIngresosAdmin",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<(IEnumerable<ReporteIngresoDto> Items, int TotalRegistros)> ObtenerIngresosPorFechaPaginadoAsync(
            DateTime? fechaDesde, DateTime? fechaHasta, int pagina, int tamanoPagina)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@FechaDesde", fechaDesde, DbType.Date);
            parameters.Add("@FechaHasta", fechaHasta, DbType.Date);
            parameters.Add("@Pagina", pagina, DbType.Int32);
            parameters.Add("@TamanoPagina", tamanoPagina, DbType.Int32);

            var rows = (await connection.QueryAsync<ReporteIngresoPaginadoDto>(
                "sp_ReporteIngresosAdminPaginado",
                parameters,
                commandType: CommandType.StoredProcedure)).ToList();

            int totalRegistros = rows.FirstOrDefault()?.TotalRegistros ?? 0;
            return (rows, totalRegistros);
        }

        public async Task<IEnumerable<ReporteIngresoMensualDto>> ObtenerIngresosMensualesAsync(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@FechaDesde", fechaDesde, DbType.Date);
            parameters.Add("@FechaHasta", fechaHasta, DbType.Date);

            return await connection.QueryAsync<ReporteIngresoMensualDto>(
                "sp_ReporteIngresosMensualesAdmin",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<ReporteTopClienteDto>> ObtenerTopClientesAsync(DateTime? fechaDesde, DateTime? fechaHasta, int topN = 10)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@FechaDesde", fechaDesde, DbType.Date);
            parameters.Add("@FechaHasta", fechaHasta, DbType.Date);
            parameters.Add("@TopN", topN, DbType.Int32);

            return await connection.QueryAsync<ReporteTopClienteDto>(
                "sp_ReporteTopClientes",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<ReporteModeloReparadoDto>> ObtenerModelosMasReparadosAsync(DateTime? fechaDesde, DateTime? fechaHasta, int topN = 10)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@FechaDesde", fechaDesde, DbType.Date);
            parameters.Add("@FechaHasta", fechaHasta, DbType.Date);
            parameters.Add("@TopN", topN, DbType.Int32);

            return await connection.QueryAsync<ReporteModeloReparadoDto>(
                "sp_ReporteModelosReparados",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<ProductividadMecanicoDto>> ObtenerProductividadMecanicosAsync(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@FechaDesde", fechaDesde, DbType.Date);
            parameters.Add("@FechaHasta", fechaHasta, DbType.Date);

            return await connection.QueryAsync<ProductividadMecanicoDto>(
                "sp_ReporteProductividadMecanicos",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<ReporteServicioCantidadDto>> ObtenerServiciosPorRangoAsync(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@FechaDesde", fechaDesde, DbType.Date);
            parameters.Add("@FechaHasta", fechaHasta, DbType.Date);

            return await connection.QueryAsync<ReporteServicioCantidadDto>(
                "sp_ReporteServiciosPorRango",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<(IEnumerable<ReporteTopClienteDto> Items, int TotalRegistros)> ObtenerTopClientesPaginadoAsync(
            DateTime? fechaDesde, DateTime? fechaHasta, int pagina, int tamanoPagina)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@FechaDesde", fechaDesde, DbType.Date);
            parameters.Add("@FechaHasta", fechaHasta, DbType.Date);
            parameters.Add("@Pagina", pagina);
            parameters.Add("@TamanoPagina", tamanoPagina);

            var rows = (await connection.QueryAsync<ReporteTopClientePaginadoDto>(
                "sp_ReporteTopClientesPaginado",
                parameters,
                commandType: CommandType.StoredProcedure)).ToList();

            int totalRegistros = rows.FirstOrDefault()?.TotalRegistros ?? 0;
            return (rows.Cast<ReporteTopClienteDto>().ToList(), totalRegistros);
        }

        public async Task<(IEnumerable<ReporteModeloReparadoDto> Items, int TotalRegistros)> ObtenerModelosMasReparadosPaginadoAsync(
            DateTime? fechaDesde, DateTime? fechaHasta, int pagina, int tamanoPagina)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@FechaDesde", fechaDesde, DbType.Date);
            parameters.Add("@FechaHasta", fechaHasta, DbType.Date);
            parameters.Add("@Pagina", pagina);
            parameters.Add("@TamanoPagina", tamanoPagina);

            var rows = (await connection.QueryAsync<ReporteModeloReparadoPaginadoDto>(
                "sp_ReporteModelosReparadosPaginado",
                parameters,
                commandType: CommandType.StoredProcedure)).ToList();

            int totalRegistros = rows.FirstOrDefault()?.TotalRegistros ?? 0;
            return (rows.Cast<ReporteModeloReparadoDto>().ToList(), totalRegistros);
        }

        public async Task<(IEnumerable<ReporteServicioCantidadDto> Items, int TotalRegistros)> ObtenerServiciosPorRangoPaginadoAsync(
            DateTime? fechaDesde, DateTime? fechaHasta, int pagina, int tamanoPagina)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@FechaDesde", fechaDesde, DbType.Date);
            parameters.Add("@FechaHasta", fechaHasta, DbType.Date);
            parameters.Add("@Pagina", pagina);
            parameters.Add("@TamanoPagina", tamanoPagina);

            var rows = (await connection.QueryAsync<ReporteServicioCantidadPaginadoDto>(
                "sp_ReporteServiciosPorRangoPaginado",
                parameters,
                commandType: CommandType.StoredProcedure)).ToList();

            int totalRegistros = rows.FirstOrDefault()?.TotalRegistros ?? 0;
            return (rows.Cast<ReporteServicioCantidadDto>().ToList(), totalRegistros);
        }
    }
}
