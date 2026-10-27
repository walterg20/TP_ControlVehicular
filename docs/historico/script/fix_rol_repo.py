import io

with io.open('Datos/Repositories/RolRepository.cs', 'r', encoding='utf-8') as f:
    content = f.read()

content = content.replace(
    'await _cvDbContext.Roles.Where(r => r.Estado).ToListAsync();',
    'await _cvDbContext.Roles.Include(r => r.Permisos).Where(r => r.Estado).ToListAsync();'
)

with io.open('Datos/Repositories/RolRepository.cs', 'w', encoding='utf-8') as f:
    f.write(content)