import io

with io.open('bd/10_ConfiguracionPermisosRecepcionMecanico.sql', 'r', encoding='utf-8') as f:
    content = f.read()

content = content.replace(
    '''INSERT INTO Permisos (Nombre, Descripcion) 
    VALUES ('MisTrabajos.Ver', 'Ver panel exclusivo de Mis Trabajos (Mecanicos)');''',
    '''INSERT INTO Permisos (Nombre) 
    VALUES ('MisTrabajos.Ver');'''
)

with io.open('bd/10_ConfiguracionPermisosRecepcionMecanico.sql', 'w', encoding='utf-8') as f:
    f.write(content)