using AutoMapper;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;

namespace TP_ControlVehicular.Negocio.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Cliente → ClienteDto
            CreateMap<Cliente, ClienteDto>()
                .ForMember(dest => dest.IdCliente, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FechaNac, opt => opt.MapFrom(src => src.FechaNacimiento));

            // Vehiculo → VehiculoDto
            CreateMap<Vehiculo, VehiculoDto>()
                .ForMember(dest => dest.IdCliente, opt => opt.MapFrom(src => src.ClienteId))
                .ForMember(dest => dest.IdModelo, opt => opt.MapFrom(src => src.ModeloId))
                .ForMember(dest => dest.ClienteNombre,
                           opt => opt.MapFrom(src => src.Cliente != null ? $"[{src.Cliente.Dni}] {src.Cliente.Nombre} {src.Cliente.Apellido}".Trim() : string.Empty))
                .ForMember(dest => dest.ModeloNombre,
                           opt => opt.MapFrom(src => src.Modelo != null ? src.Modelo.NombreModelo : string.Empty))
                .ForMember(dest => dest.MarcaNombre,
                           opt => opt.MapFrom(src => src.Modelo != null && src.Modelo.Marca != null ? src.Modelo.Marca.NombreMarca : string.Empty));

            // Modelo → ModeloDto
            CreateMap<Modelo, ModeloDto>()
                .ForMember(dest => dest.IdMarca, opt => opt.MapFrom(src => src.MarcaId))
                .ForMember(dest => dest.MarcaNombre,
                           opt => opt.MapFrom(src => src.Marca != null ? src.Marca.NombreMarca : string.Empty));

            // Marca → MarcaDto
            CreateMap<Marca, MarcaDto>();

            // Taller → TallerDto
            CreateMap<Taller, TallerDto>()
                .ForMember(dest => dest.IdTaller, opt => opt.MapFrom(src => src.Id));

            // Rol → RolDto
            CreateMap<Rol, RolDto>()
                .ForMember(dest => dest.IdRol, opt => opt.MapFrom(src => src.Id));

            // Usuario → UsuarioDto
            CreateMap<Usuario, UsuarioDto>()
                .ForMember(dest => dest.IdUsuario, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IdRol, opt => opt.MapFrom(src => src.RolId))
                .ForMember(dest => dest.RolNombre, opt => opt.MapFrom(src => src.Rol != null ? src.Rol.Nombre : string.Empty));

            // Servicio → ServicioDto
            CreateMap<Servicio, ServicioDto>()
                .ForMember(dest => dest.IdServicio, opt => opt.MapFrom(src => src.Id));

            // RegistroServicio -> RegistroServicioDto
            CreateMap<RegistroServicio, RegistroServicioDto>()
                .ForMember(dest => dest.VehiculoPatente, opt => opt.MapFrom(src => src.Vehiculo != null ? src.Vehiculo.Patente : string.Empty))
                .ForMember(dest => dest.VehiculoDetalle, opt => opt.MapFrom(src => src.Vehiculo != null && src.Vehiculo.Modelo != null && src.Vehiculo.Modelo.Marca != null 
                    ? $"{src.Vehiculo.Modelo.Marca.NombreMarca} {src.Vehiculo.Modelo.NombreModelo}" : string.Empty))
                .ForMember(dest => dest.TallerNombre, opt => opt.MapFrom(src => src.Taller != null ? src.Taller.Nombre : string.Empty))
                .ForMember(dest => dest.RecepcionistaNombre, opt => opt.MapFrom(src => src.Usuario != null ? $"{src.Usuario.Nombre} {src.Usuario.Apellido}".Trim() : string.Empty));

        }
    }
}
