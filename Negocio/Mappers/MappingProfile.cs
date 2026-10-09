using AutoMapper;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;

namespace TP_ControlVehicular.Negocio.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Cliente â†’ ClienteDto
            CreateMap<Cliente, ClienteDto>()
                .ForMember(dest => dest.IdCliente, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FechaNac, opt => opt.MapFrom(src => src.FechaNacimiento));

            // Vehiculo -> VehiculoDto
            CreateMap<Vehiculo, VehiculoDto>()
                .ForMember(dest => dest.IdCliente, opt => opt.MapFrom((src, dest) => src.Propietarios.FirstOrDefault(p => p.EsActual)?.Cliente?.Id ?? 0))
                .ForMember(dest => dest.IdModelo, opt => opt.MapFrom(src => src.ModeloId))
                .ForMember(dest => dest.ClienteNombre, opt => opt.MapFrom((src, dest) => 
                {
                    var c = src.Propietarios.FirstOrDefault(p => p.EsActual)?.Cliente;
                    return c != null ? $"[{c.Dni}] {c.Nombre} {c.Apellido}".Trim() : "Sin asignar";
                }))
                .ForMember(dest => dest.ModeloNombre, opt => opt.MapFrom(src => src.Modelo != null ? src.Modelo.NombreModelo : string.Empty))
                .ForMember(dest => dest.MarcaNombre, opt => opt.MapFrom(src => src.Modelo != null && src.Modelo.Marca != null ? src.Modelo.Marca.NombreMarca : string.Empty));

            // Modelo -> ModeloDto
            CreateMap<Modelo, ModeloDto>()
                .ForMember(dest => dest.IdMarca, opt => opt.MapFrom(src => src.MarcaId))
                .ForMember(dest => dest.MarcaNombre, opt => opt.MapFrom(src => src.Marca != null ? src.Marca.NombreMarca : string.Empty));

            // Marca -> MarcaDto
            CreateMap<Marca, MarcaDto>();

            // Taller -> TallerDto
            CreateMap<Taller, TallerDto>()
                .ForMember(dest => dest.IdTaller, opt => opt.MapFrom(src => src.Id));

            // Rol -> RolDto
            CreateMap<Rol, RolDto>()
                .ForMember(dest => dest.IdRol, opt => opt.MapFrom(src => src.Id));

            // Usuario -> UsuarioDto
            CreateMap<Usuario, UsuarioDto>()
                .ForMember(dest => dest.IdUsuario, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IdRol, opt => opt.MapFrom(src => src.RolId))
                .ForMember(dest => dest.RolNombre, opt => opt.MapFrom(src => src.Rol != null ? src.Rol.Nombre : string.Empty));

            // Servicio -> ServicioDto
            CreateMap<Servicio, ServicioDto>()
                .ForMember(dest => dest.IdServicio, opt => opt.MapFrom(src => src.Id));

            // RegistroServicio -> RegistroServicioDto
            CreateMap<RegistroServicio, RegistroServicioDto>()
                .ForMember(dest => dest.ClienteDetalle, opt => opt.MapFrom((src, dest) => 
                {
                    var c = src.Vehiculo?.Propietarios.FirstOrDefault(p => p.EsActual)?.Cliente;
                    return c != null ? $"{c.Nombre} {c.Apellido} [{c.Dni}]" : "Sin asignar";
                }))
                .ForMember(dest => dest.VehiculoPatente, opt => opt.MapFrom(src => src.Vehiculo != null ? src.Vehiculo.Patente : string.Empty))
                .ForMember(dest => dest.VehiculoDetalle, opt => opt.MapFrom(src => src.Vehiculo != null && src.Vehiculo.Modelo != null && src.Vehiculo.Modelo.Marca != null 
                    ? $"{src.Vehiculo.Modelo.Marca.NombreMarca} {src.Vehiculo.Modelo.NombreModelo}" : string.Empty))
                .ForMember(dest => dest.TallerNombre, opt => opt.MapFrom(src => src.Taller != null ? src.Taller.Nombre : string.Empty))
                .ForMember(dest => dest.RecepcionistaNombre, opt => opt.MapFrom(src => src.Usuario != null ? $"{src.Usuario.Nombre} {src.Usuario.Apellido}".Trim() : string.Empty))
                .ForMember(dest => dest.Detalles, opt => opt.MapFrom(src => src.Detalles));

            // DetalleServicio -> DetalleServicioDto
            CreateMap<DetalleServicio, DetalleServicioDto>()
                .ForMember(dest => dest.ServicioNombre, opt => opt.MapFrom(src => src.Servicio != null ? src.Servicio.Nombre : string.Empty))
                .ForMember(dest => dest.Realizado, opt => opt.MapFrom(src => src.Estado == "Finalizada" || src.Estado == "Realizado"));

            CreateMap<Factura, FacturaDto>();
        }
    }
}

