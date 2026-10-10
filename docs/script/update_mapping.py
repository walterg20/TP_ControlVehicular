import io

with io.open('Negocio/Mappers/MappingProfile.cs', 'r', encoding='utf-8') as f:
    content = f.read()

replacement = '''            // Permiso -> PermisoDto
            CreateMap<Permiso, PermisoDto>()
                .ForMember(dest => dest.Asignado, opt => opt.Ignore());

            // Rol -> RolDto
            CreateMap<Rol, RolDto>()
                .ForMember(dest => dest.IdRol, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Permisos, opt => opt.MapFrom(src => src.Permisos.Select(p => p.Nombre).ToList()));'''

content = content.replace(
    '''// Rol -> RolDto
            CreateMap<Rol, RolDto>()
                .ForMember(dest => dest.IdRol, opt => opt.MapFrom(src => src.Id));''',
    replacement
)

with io.open('Negocio/Mappers/MappingProfile.cs', 'w', encoding='utf-8') as f:
    f.write(content)

# Update RolDto
with io.open('Negocio/DTOs/RolDto.cs', 'r', encoding='utf-8') as f:
    rol_content = f.read()

rol_content = rol_content.replace(
    'public bool Estado { get; set; }',
    'public bool Estado { get; set; }\n        public System.Collections.Generic.List<string> Permisos { get; set; } = new();'
)

with io.open('Negocio/DTOs/RolDto.cs', 'w', encoding='utf-8') as f:
    f.write(rol_content)

# Create PermisoDto
permiso_dto = '''namespace TP_ControlVehicular.Negocio.DTOs
{
    public class PermisoDto
    {
        public int IdPermiso { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Asignado { get; set; }
    }
}
'''
with io.open('Negocio/DTOs/PermisoDto.cs', 'w', encoding='utf-8') as f:
    f.write(permiso_dto)
