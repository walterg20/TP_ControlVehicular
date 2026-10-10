# Tareas de Implementación: Permisos Avanzados

- [ ] **Tarea 1: Modelo y Migración**. Crear entidad Permiso, agregar relación M:N en Rol, y aplicar migración en SQL Server. Se insertarán los permisos semilla (ej. Cliente.Ver, Cliente.Crear, etc.).
- [ ] **Tarea 2: Lógica de Acceso a Datos**. Crear DTOs, el repositorio/handler para listar todos los permisos, y el handler para guardar los permisos asociados a un rol específico (ActualizarPermisosRolHandler).
- [ ] **Tarea 3: Sesión Global (Login)**. Actualizar el proceso de inicio de sesión para que cargue en memoria (App o MainWindow) los permisos del usuario logueado en un HashSet<string>.
- [ ] **Tarea 4: Interfaz de Gestión de Permisos**. Construir CtlRolPermiso.xaml para que un Administrador pueda asignar o revocar permisos a los roles usando CheckBoxes y guardarlos.
- [ ] **Tarea 5: Refactorización de Accesos en UI**. Modificar MainWindow para ocultar menús no permitidos. Actualizar BaseViewModel para exponer PuedeCrear, PuedeEditar, PuedeEliminar basados en la lista de permisos en memoria (reemplazando los IF hardcodeados por Rol ID).
