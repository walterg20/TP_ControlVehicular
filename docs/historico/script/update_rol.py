import io

with io.open('Entidad/Rol.cs', 'r', encoding='utf-8') as f:
    content = f.read()

content = content.replace(
    'public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();',
    'public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();\n        public ICollection<Permiso> Permisos { get; set; } = new List<Permiso>();'
)

with io.open('Entidad/Rol.cs', 'w', encoding='utf-8') as f:
    f.write(content)