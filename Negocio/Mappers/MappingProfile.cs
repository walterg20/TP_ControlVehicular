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
                .ForMember(dest => dest.NombreCompleto,
                           opt => opt.MapFrom(src => $"{src.Nombre} {src.Apellido}"));

            // Vehiculo → VehiculoDto
            CreateMap<Vehiculo, VehiculoDto>()
                .ForMember(dest => dest.ClienteNombre,
                           opt => opt.MapFrom(src => src.Cliente.Nombre + " " + src.Cliente.Apellido))
                .ForMember(dest => dest.ModeloNombre,
                           opt => opt.MapFrom(src => src.Modelo.NombreModelo))
                .ForMember(dest => dest.MarcaNombre,
                           opt => opt.MapFrom(src => src.Modelo.Marca.NombreMarca));

            // Modelo → ModeloDto
            CreateMap<Modelo, ModeloDto>()
                .ForMember(dest => dest.MarcaNombre,
                           opt => opt.MapFrom(src => src.Marca.NombreMarca));

            // Marca → MarcaDto
            CreateMap<Marca, MarcaDto>();
        }
    }
}
