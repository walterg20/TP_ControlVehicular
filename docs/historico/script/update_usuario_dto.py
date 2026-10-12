import io

with io.open('Negocio/DTOs/UsuarioDto.cs', 'r', encoding='utf-8') as f:
    content = f.read()

content = content.replace(
    'public bool Estado { get; set; }',
    'public bool Estado { get; set; }\n        public System.Collections.Generic.List<string> Permisos { get; set; } = new();'
)

with io.open('Negocio/DTOs/UsuarioDto.cs', 'w', encoding='utf-8') as f:
    f.write(content)

with io.open('Negocio/Mappers/MappingProfile.cs', 'r', encoding='utf-8') as f:
    mapping_content = f.read()

mapping_content = mapping_content.replace(
    '.ForMember(dest => dest.RolNombre, opt => opt.MapFrom(src => src.Rol != null ? src.Rol.Nombre : string.Empty));',
    '.ForMember(dest => dest.RolNombre, opt => opt.MapFrom(src => src.Rol != null ? src.Rol.Nombre : string.Empty))\n                .ForMember(dest => dest.Permisos, opt => opt.MapFrom(src => src.Rol != null && src.Rol.Permisos != null ? src.Rol.Permisos.Select(p => p.Nombre).ToList() : new System.Collections.Generic.List<string>()));'
)

with io.open('Negocio/Mappers/MappingProfile.cs', 'w', encoding='utf-8') as f:
    f.write(mapping_content)