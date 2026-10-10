-- 10_ConfiguracionPermisosRecepcionMecanico.sql

-- 1. Insertar nuevo permiso para Mis Trabajos
IF NOT EXISTS (SELECT 1 FROM Permisos WHERE Nombre = 'MisTrabajos.Ver')
BEGIN
    INSERT INTO Permisos (Nombre, Descripcion) 
    VALUES ('MisTrabajos.Ver', 'Ver panel exclusivo de Mis Trabajos (Mecanicos)');
END

-- Asignar al Admin (Rol 1) para completitud
INSERT INTO RolPermisos (RolId, PermisoId)
SELECT 1, IdPermiso FROM Permisos 
WHERE Nombre = 'MisTrabajos.Ver' 
AND NOT EXISTS (SELECT 1 FROM RolPermisos WHERE RolId = 1 AND PermisoId = Permisos.IdPermiso);

-- 2. Asignar Permisos a Recepcionista (RolId = 2)
-- Borrar anteriores por si acaso
DELETE FROM RolPermisos WHERE RolId = 2;

-- Clientes
INSERT INTO RolPermisos (RolId, PermisoId) 
SELECT 2, IdPermiso FROM Permisos WHERE Nombre IN ('Cliente.Ver', 'Cliente.Crear', 'Cliente.Editar', 'Cliente.Eliminar');

-- Vehiculos, Marcas, Modelos
INSERT INTO RolPermisos (RolId, PermisoId) 
SELECT 2, IdPermiso FROM Permisos WHERE Nombre IN (
    'Vehiculo.Ver', 'Vehiculo.Crear', 'Vehiculo.Editar', 'Vehiculo.Eliminar',
    'Marca.Ver', 'Marca.Crear', 'Marca.Editar', 'Marca.Eliminar',
    'Modelo.Ver', 'Modelo.Crear', 'Modelo.Editar', 'Modelo.Eliminar'
);

-- Orden Servicio
INSERT INTO RolPermisos (RolId, PermisoId) 
SELECT 2, IdPermiso FROM Permisos WHERE Nombre IN ('OrdenServicio.Ver', 'OrdenServicio.Crear', 'OrdenServicio.Editar', 'OrdenServicio.Eliminar');

-- Servicios (Solo ver catalogo, o capaz crear?)
-- Le damos Ver para agregarlos a las ordenes
INSERT INTO RolPermisos (RolId, PermisoId) 
SELECT 2, IdPermiso FROM Permisos WHERE Nombre IN ('Servicio.Ver');

-- Reportes
INSERT INTO RolPermisos (RolId, PermisoId) 
SELECT 2, IdPermiso FROM Permisos WHERE Nombre IN ('Reporte.Ver');


-- 3. Asignar Permisos a Mecanico (RolId = 3)
DELETE FROM RolPermisos WHERE RolId = 3;

-- Clientes / Vehiculos (Solo ver)
INSERT INTO RolPermisos (RolId, PermisoId) 
SELECT 3, IdPermiso FROM Permisos WHERE Nombre IN ('Cliente.Ver', 'Vehiculo.Ver', 'Marca.Ver', 'Modelo.Ver');

-- Orden de Servicio (Ver, Editar para avanzar estado)
INSERT INTO RolPermisos (RolId, PermisoId) 
SELECT 3, IdPermiso FROM Permisos WHERE Nombre IN ('OrdenServicio.Ver', 'OrdenServicio.Editar');

-- Servicios (Solo ver)
INSERT INTO RolPermisos (RolId, PermisoId) 
SELECT 3, IdPermiso FROM Permisos WHERE Nombre IN ('Servicio.Ver');

-- Mis Trabajos
INSERT INTO RolPermisos (RolId, PermisoId) 
SELECT 3, IdPermiso FROM Permisos WHERE Nombre IN ('MisTrabajos.Ver');
