# Especificación: Permisos Avanzados Dinámicos (RBAC)

## 1. Objetivo
Migrar el sistema actual de permisos (hardcodeado en la interfaz por ID de rol) a un sistema dinámico basado en base de datos. Esto permitirá configurar desde una pantalla de administración qué acciones específicas (Ver, Crear, Editar, Eliminar) tiene permitidas cada rol.

## 2. Requerimientos
* Crear una nueva entidad Permiso y una tabla intermedia RolPermiso en la base de datos.
* Interfaz de usuario para que un Administrador asigne o quite permisos a un rol específico usando CheckBoxes.
* Modificar MainWindow.xaml.cs para leer los permisos de la base de datos al iniciar sesión y habilitar/ocultar los botones del menú lateral según los permisos.
* En cada pantalla (ej: Vehículos, Clientes), ocultar o deshabilitar los botones de "Nuevo", "Editar" y "Eliminar" según el permiso granular.

## 3. Criterios de Aceptación
- [ ] La base de datos tiene las tablas de Permisos y su relación con Rol.
- [ ] La pantalla de Roles permite editar la lista de permisos asignados.
- [ ] Un usuario con sesión iniciada solo ve en el menú lateral las pantallas para las que tiene permiso.