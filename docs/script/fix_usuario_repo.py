import io

with io.open('Datos/Repositories/UsuarioRepository.cs', 'r', encoding='utf-8') as f:
    content = f.read()

content = content.replace(
    '.Include(u => u.Rol)',
    '.Include(u => u.Rol).ThenInclude(r => r.Permisos)'
)

with io.open('Datos/Repositories/UsuarioRepository.cs', 'w', encoding='utf-8') as f:
    f.write(content)