// walterg20
using AutoMapper;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;
using TP_ControlVehicular.Negocio.Reportes.Documentos;

namespace TP_ControlVehicular.Negocio.Services
{
    public class EmpresaConfiguracionService : IEmpresaConfiguracionService
    {
        private readonly IConfiguracionEmpresaRepository _repository;
        private readonly IMapper _mapper;

        public EmpresaConfiguracionService(IConfiguracionEmpresaRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<EmpresaConfiguracionDto> ObtenerAsync()
        {
            var entidad = await _repository.ObtenerAsync();

            if (entidad == null)
            {
                entidad = new ConfiguracionEmpresa
                {
                    Id = 1,
                    Nombre = EmpresaBranding.Nombre,
                    Direccion = EmpresaBranding.Direccion,
                    Telefono = EmpresaBranding.Telefono,
                    Email = null,
                    Logo = null,
                    FechaActualizacion = DateTime.Now
                };

                await _repository.AddAsync(entidad);
            }

            return _mapper.Map<EmpresaConfiguracionDto>(entidad);
        }

        public async Task<EmpresaConfiguracionDto> GuardarAsync(EmpresaConfiguracionDto dto, byte[]? logoBytes)
        {
            var entidad = await _repository.ObtenerAsync();

            if (entidad == null)
            {
                entidad = new ConfiguracionEmpresa
                {
                    Id = 1
                };

                await _repository.AddAsync(entidad);
                entidad = await _repository.ObtenerAsync() ?? entidad;
            }

            entidad.Nombre = (dto.Nombre ?? string.Empty).Trim();
            entidad.Direccion = (dto.Direccion ?? string.Empty).Trim();
            entidad.Telefono = (dto.Telefono ?? string.Empty).Trim();
            entidad.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();

            if (logoBytes != null)
            {
                entidad.Logo = logoBytes;
            }

            entidad.FechaActualizacion = DateTime.Now;

            await _repository.UpdateAsync(entidad);

            await RefrescarCacheAsync();

            return _mapper.Map<EmpresaConfiguracionDto>(entidad);
        }

        public async Task RefrescarCacheAsync()
        {
            var entidad = await _repository.ObtenerAsync();

            if (entidad != null)
            {
                EmpresaBranding.Aplicar(
                    entidad.Nombre,
                    entidad.Direccion,
                    entidad.Telefono,
                    entidad.Logo
                );
                return;
            }

            var dto = await ObtenerAsync();
            EmpresaBranding.Aplicar(dto.Nombre, dto.Direccion, dto.Telefono, null);
        }
    }
}