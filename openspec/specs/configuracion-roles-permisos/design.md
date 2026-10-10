# Spec: Configuración de Roles y Permisos (Recepcionista y Mecánico)

## Objetivo
Poblar la matriz de permisos para los roles predeterminados del sistema (Recepcionista y Mecánico) e incorporar un permiso específico para el panel "Mis Trabajos". Eliminar cualquier verificación dura (hardcodeada) restante de IdRol en el código.

## Diseño
- **Base de Datos**: Script 10_ConfiguracionPermisosRecepcionMecanico.sql. Se insertará el permiso MisTrabajos.Ver y se asociarán múltiples permisos para IdRol = 2 y IdRol = 3.
- **UI**:
  - En MainWindow.xaml.cs, usar MisTrabajos.Ver en lugar de IdRol == 3.
  - En OrdenServicioViewModel.cs, remover propiedades sobrescritas PuedeCrear y PuedeEliminar basadas en EsMecanico, delegándolas a la validación RBAC nativa del BaseViewModel.
