import io

with io.open('bd/10_ConfiguracionPermisosRecepcionMecanico.sql', 'r', encoding='utf-8') as f:
    content = f.read()

content = content.replace('RolId', 'IdRol')
content = content.replace('PermisoId', 'IdPermiso')

with io.open('bd/10_ConfiguracionPermisosRecepcionMecanico.sql', 'w', encoding='utf-8') as f:
    f.write(content)