# -*- coding: utf-8 -*-
path = 'MEMORY.md'
with open(path, 'r', encoding='utf-8') as f:
    c = f.read()

target = '## Estado actual (verificar con `git`)'
addition = '''## Estado actual (verificar con `git`)
- Rama de trabajo: `feature/alineacion-estilos-crud`. Spec `alineacion-estilos-crud` implementada (build con 0 errores; 147 tests unitarios en verde).
- **Alineación de estilos y estados en pantallas CRUD (spec `alineacion-estilos-crud`)**:
  - Estandarización de `CtlCliente`, `CtlUsuario`, `CtlRol`, `CtlServicio` y `CtlTaller` tomando como base `CtlModelo` (sumándose a `CtlModelo`, `CtlMarca` y `CtlVehiculo` ya alineados).
  - **Filtro de Estado**: Reemplazo de checkbox 'Mostrar inactivos' o ausencia de filtro por ComboBox 'Estado:' con opciones 'Todos', 'Activos', 'Inactivos', condicionado por `PuedeVerFiltroEstado`.
  - **Grilla DataGrid**: Eliminación de la columna redundante de checkbox 'Activo'. Filas inactivas resaltadas automáticamente en rojo suave (`Foreground="#C0392B"`, `Background="#FDECEA"`).
  - **Botonera Inferior**: Botoneras alineadas a la derecha con paleta uniforme (✏️ Editar `#F39C12`, 🗑️ Eliminar `#C0392B`, 🔄 Reactivar `#27AE60`).
  - **Capa Negocio y Datos**: Métodos `GetPaginadoAsync` y handlers actualizados para recibir `string? filtroEstado` ('Todos', 'Activos', 'Inactivos').
  - **Tests**: 147 tests unitarios en verde sin paralelización conflictiva de sesiones (`TestCollectionBehavior.cs`).
'''

c = c.replace(target, addition)
with open(path, 'w', encoding='utf-8') as f:
    f.write(c)
print('MEMORY.md updated successfully')
