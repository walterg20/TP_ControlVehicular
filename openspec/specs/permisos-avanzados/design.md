# Diseño Técnico: Permisos Avanzados Dinámicos (RBAC)

## 1. Cambios en Base de Datos (Entity Framework Core)
- **Entidad Permiso**: Representa una acción granular en el sistema (ej. Cliente.Crear, Cliente.Ver).
  - Propiedades: IdPermiso (int), Nombre (string, max 100).
- **Relación Muchos a Muchos**: Rol tendrá una colección Permisos y Permiso una colección Roles. EF Core 10 manejará la tabla intermedia automáticamente.
- **Migración**: Se creará una migración AddPermisos para actualizar SQL Server.

## 2. Lógica de Negocio (Handlers y DTOs)
- **RolDto**: Se expandirá para incluir List<string> Permisos.
- **PermisoDto**: Id, Nombre.
- Se agregarán Handlers:
  - ObtenerPermisosHandler: Lista todos los permisos disponibles.
  - ActualizarPermisosRolHandler: Recibe un ID de Rol y una lista de IDs de Permisos y actualiza la relación en BD.

## 3. Sesión y Autorización
- Al hacer Login, el UsuarioDto o la sesión global incluirá una lista HashSet<string> con los nombres de todos los permisos asignados a su rol.
- BaseViewModel: Se actualizará para depender de esta lista. En lugar de verificar si el ID del rol es 1 (Admin), se verificarán permisos específicos (ej. App.PermisosSesion.Contains("Vehiculo.Crear")) para exponer propiedades como PuedeCrear, PuedeEditar, PuedeEliminar a las vistas (XAML).

## 4. Interfaz de Usuario (Presentación)
- **Nueva Vista de Roles (CtlRoles)**: Se modificará o creará una pantalla para gestionar roles. Al seleccionar un rol, mostrará un listado de CheckBoxes (DataGrid o ListBox con DataTemplate) con todos los permisos del sistema para activar/desactivar.
- **MainWindow.xaml.cs**: Se modificarán las validaciones del menú lateral para ocultar las pestañas si el usuario no tiene el permiso [Modulo].Ver correspondiente.
