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
    }
}
