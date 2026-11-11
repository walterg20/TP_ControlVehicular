// walterg20
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IRegistroServicioRepository : IRepository<RegistroServicio>
    {
        Task<List<RegistroServicio>> GetReporteCompletoAsync(int? mecanicoId = null);

        /// <summary>
        /// Recupera todas las ordenes que cumplen los filtros, sin paginar.
        /// Lo usa la exportacion a PDF del listado de ordenes.
        /// </summary>
        Task<List<RegistroServicio>> GetFiltradoAsync(
            string? busqueda,
            string? estado,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            string? filtroActivo = null);

        /// <summary>
        /// Recupera una pagina de ordenes de servicio aplicando los filtros en la base de datos.
        /// </summary>
        Task<(IEnumerable<RegistroServicio> Items, int TotalRegistros)> GetPaginadoAsync(
            string? busqueda,
            string? estado,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            int pagina,
            int tamanoPagina,
            string? filtroActivo = null);

        /// <summary>
        /// Recupera una pagina de ordenes que tienen tareas asignadas al mecanico indicado,
        /// aplicando los filtros en la base de datos.
        /// </summary>
        /// <param name="filtroTareas">"Pendientes", "Finalizados" o null/otro para todos.</param>
        Task<(IEnumerable<RegistroServicio> Items, int TotalRegistros)> GetPaginadoPorMecanicoAsync(
            int usuarioId,
            string? busqueda,
            string? filtroTareas,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            int pagina,
            int tamanoPagina);
    }
}
