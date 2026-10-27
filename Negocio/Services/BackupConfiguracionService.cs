// walterg20
using AutoMapper;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class BackupConfiguracionService : IBackupConfiguracionService
    {
        private readonly IConfiguracionBackupRepository _repository;
        private readonly IMapper _mapper;

        public BackupConfiguracionService(IConfiguracionBackupRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<BackupConfiguracionDto> ObtenerAsync()
        {
            var entidad = await _repository.ObtenerAsync();

            if (entidad == null)
            {
                entidad = new ConfiguracionBackup
                {
                    Id = 1,
                    NombreBaseDatos = "ControlVehicular",
                    RutaBackupServer = string.Empty,
                    RutaBackupUNC = string.Empty,
                    DriveFolderId = null,
                    UltimaSubidaArchivo = null,
                    FechaUltimaSubida = null,
                    FechaActualizacion = DateTime.Now
                };

                await _repository.AddAsync(entidad);
            }

            return _mapper.Map<BackupConfiguracionDto>(entidad);
        }

        public async Task<BackupConfiguracionDto> GuardarAsync(BackupConfiguracionDto dto)
        {
            var entidad = await _repository.ObtenerAsync();

            if (entidad == null)
            {
                entidad = new ConfiguracionBackup
                {
                    Id = 1
                };

                await _repository.AddAsync(entidad);
                entidad = await _repository.ObtenerAsync() ?? entidad;
            }

            entidad.NombreBaseDatos = string.IsNullOrWhiteSpace(dto.NombreBaseDatos)
                ? "ControlVehicular"
                : dto.NombreBaseDatos.Trim();
            entidad.RutaBackupServer = (dto.RutaBackupServer ?? string.Empty).Trim();
            entidad.RutaBackupUNC = (dto.RutaBackupUNC ?? string.Empty).Trim();
            entidad.DriveFolderId = string.IsNullOrWhiteSpace(dto.DriveFolderId) ? null : dto.DriveFolderId.Trim();
            entidad.FechaActualizacion = DateTime.Now;

            await _repository.UpdateAsync(entidad);

            return _mapper.Map<BackupConfiguracionDto>(entidad);
        }

        public async Task RegistrarSubidaAsync(string nombreArchivo, DateTime fecha)
        {
            var entidad = await _repository.ObtenerAsync();

            if (entidad == null)
            {
                entidad = new ConfiguracionBackup
                {
                    Id = 1
                };

                await _repository.AddAsync(entidad);
                entidad = await _repository.ObtenerAsync() ?? entidad;
            }

            entidad.UltimaSubidaArchivo = nombreArchivo;
            entidad.FechaUltimaSubida = fecha;

            await _repository.UpdateAsync(entidad);
        }
    }
}
