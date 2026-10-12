import io
import os

os.makedirs('openspec/specs/configuracion-roles-permisos', exist_ok=True)

design_content = '''# Spec: Configuración de Roles y Permisos (Recepcionista y Mecánico)

## Objetivo
Poblar la matriz de permisos para los roles predeterminados del sistema (Recepcionista y Mecánico) e incorporar un permiso específico para el panel "Mis Trabajos". Eliminar cualquier verificación dura (hardcodeada) restante de IdRol en el código.

## Diseño
- **Base de Datos**: Script 10_ConfiguracionPermisosRecepcionMecanico.sql. Se insertará el permiso MisTrabajos.Ver y se asociarán múltiples permisos para IdRol = 2 y IdRol = 3.
- **UI**:
  - En MainWindow.xaml.cs, usar MisTrabajos.Ver en lugar de IdRol == 3.
  - En OrdenServicioViewModel.cs, remover propiedades sobrescritas PuedeCrear y PuedeEliminar basadas en EsMecanico, delegándolas a la validación RBAC nativa del BaseViewModel.
'''
with io.open('openspec/specs/configuracion-roles-permisos/design.md', 'w', encoding='utf-8') as f:
    f.write(design_content)

tasks_content = '''# Tareas: Configuración de Roles y Permisos
status: IN_PROGRESS

- [ ] Tarea 1: Crear script SQL 10_ConfiguracionPermisosRecepcionMecanico.sql con la inserción de datos.
- [ ] Tarea 2: Refactorizar MainWindow.xaml.cs para el botón Mis Trabajos.
- [ ] Tarea 3: Refactorizar OrdenServicioViewModel.cs para usar los permisos nativos de RBAC (Crear/Eliminar).
'''
with io.open('openspec/specs/configuracion-roles-permisos/tasks.md', 'w', encoding='utf-8') as f:
    f.write(tasks_content)