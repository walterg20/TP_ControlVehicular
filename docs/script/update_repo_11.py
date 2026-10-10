import io

with io.open('Negocio/Interfaces/IReporteRepository.cs', 'r', encoding='utf-8') as f:
    content = f.read()

new_methods = '''
        Task<List<HistorialClinicoVehiculoDto>> ObtenerHistorialClinicoVehiculoAsync(int vehiculoId);
        Task<List<HojaTrabajoDiariaDto>> ObtenerHojaTrabajoDiariaAsync(int mecanicoId, DateTime? fecha);
'''
content = content.replace('}', new_methods + '}')

with io.open('Negocio/Interfaces/IReporteRepository.cs', 'w', encoding='utf-8') as f:
    f.write(content)

with io.open('Datos/Repositories/ReporteRepository.cs', 'r', encoding='utf-8') as f:
    repo_content = f.read()

repo_methods = '''
        public async Task<List<HistorialClinicoVehiculoDto>> ObtenerHistorialClinicoVehiculoAsync(int vehiculoId)
        {
            var vehiculoIdParam = new SqlParameter("@VehiculoId", vehiculoId);
            return await _dbContext.Database
                .SqlQueryRaw<HistorialClinicoVehiculoDto>(
                    "EXEC sp_HistorialClinicoVehiculo @VehiculoId",
                    vehiculoIdParam)
                .ToListAsync();
        }

        public async Task<List<HojaTrabajoDiariaDto>> ObtenerHojaTrabajoDiariaAsync(int mecanicoId, DateTime? fecha)
        {
            var mecanicoIdParam = new SqlParameter("@MecanicoId", mecanicoId);
            var fechaParam = new SqlParameter("@Fecha", (object?)fecha ?? DBNull.Value);
            return await _dbContext.Database
                .SqlQueryRaw<HojaTrabajoDiariaDto>(
                    "EXEC sp_ReporteHojaTrabajoDiaria @MecanicoId, @Fecha",
                    mecanicoIdParam, fechaParam)
                .ToListAsync();
        }
'''
# Insert before the last two closing braces
lines = repo_content.split('\n')
repo_content = '\n'.join(lines[:-3]) + '\n' + repo_methods + '\n' + '\n'.join(lines[-3:])

with io.open('Datos/Repositories/ReporteRepository.cs', 'w', encoding='utf-8') as f:
    f.write(repo_content)