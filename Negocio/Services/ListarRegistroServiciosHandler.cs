// walterg20
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ListarRegistroServiciosHandler
    {
        private readonly IRegistroServicioRepository _repository;
        private readonly IMapper _mapper;

        public ListarRegistroServiciosHandler(IRegistroServicioRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RegistroServicioDto>> HandleAsync()
        {
            // Usamos GetReporteCompletoAsync para obtener las propiedades de navegación
            var registros = await _repository.GetReporteCompletoAsync();
            return _mapper.Map<IEnumerable<RegistroServicioDto>>(registros);
        }

        /// <summary>
        /// Recupera todas las ordenes que cumplen los filtros, sin paginar.
        /// Lo usa la exportacion a PDF del listado de ordenes.
        /// </summary>
        public async Task<IEnumerable<RegistroServicioDto>> HandleCompletoAsync(
            string? busqueda,
            string? estado,
            DateTime? fechaDesde,
            DateTime? fechaHasta)
        {
            var registros = await _repository.GetFiltradoAsync(busqueda, estado, fechaDesde, fechaHasta);
            return _mapper.Map<IEnumerable<RegistroServicioDto>>(registros);
        }

        /// <summary>
        /// Recupera una pagina de ordenes con los filtros aplicados en la base de datos.
        /// </summary>
        public async Task<(IEnumerable<RegistroServicioDto> Items, int TotalRegistros)> HandlePaginadoAsync(
            string? busqueda,
            string? estado,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            int pagina,
            int tamanoPagina)
        {
            var (registros, total) = await _repository.GetPaginadoAsync(
                busqueda, estado, fechaDesde, fechaHasta, pagina, tamanoPagina);

            return (_mapper.Map<IEnumerable<RegistroServicioDto>>(registros), total);
        }

        /// <summary>
        /// Recupera una pagina de ordenes con tareas del mecanico indicado, con los filtros
        /// aplicados en la base de datos.
        /// </summary>
        public async Task<(IEnumerable<RegistroServicioDto> Items, int TotalRegistros)> HandlePaginadoPorMecanicoAsync(
            int usuarioId,
            string? busqueda,
            string? filtroTareas,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            int pagina,
            int tamanoPagina)
        {
            var (registros, total) = await _repository.GetPaginadoPorMecanicoAsync(
                usuarioId, busqueda, filtroTareas, fechaDesde, fechaHasta, pagina, tamanoPagina);

            return (_mapper.Map<IEnumerable<RegistroServicioDto>>(registros), total);
        }
    }
}
