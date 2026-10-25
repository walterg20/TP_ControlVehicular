import io

with io.open('Negocio/Interfaces/IReporteRepository.cs', 'r', encoding='utf-8') as f:
    content = f.read()

new_methods = '''
        Task<List<HistorialClinicoVehiculoDto>> ObtenerHistorialClinicoVehiculoAsync(int vehiculoId);
        Task<List<HojaTrabajoDiariaDto>> ObtenerHojaTrabajoDiariaAsync(int mecanicoId, DateTime? fecha);
'''
content = content.replace('Task<List<ReporteServicioDto>> ObtenerServiciosPorMecanicoAsync(DateTime desde, DateTime hasta, int mecanicoId);', 'Task<List<ReporteServicioDto>> ObtenerServiciosPorMecanicoAsync(DateTime desde, DateTime hasta, int mecanicoId);' + new_methods)

with io.open('Negocio/Interfaces/IReporteRepository.cs', 'w', encoding='utf-8') as f:
    f.write(content)