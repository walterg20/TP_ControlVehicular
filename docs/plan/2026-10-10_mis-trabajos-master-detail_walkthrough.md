# Walkthrough — Mis Trabajos: rediseño a maestro-detalle (escritorio)

Bitácora de ejecución del plan [`2026-10-10_mis-trabajos-master-detail.md`](./2026-10-10_mis-trabajos-master-detail.md).
Spec formal: `openspec/changes/mis-trabajos-master-detail/`.

> [!NOTE]
> `openspec/` = especificación formal (inglés). `docs/plan/` = plan y bitácora de
> ejecución (español).

## Resumen

La pantalla *Mis Trabajos* pasa de una grilla plana (una fila por tarea) a un patrón
**maestro-detalle**:

1. **Maestro**: `dgOrdenes` lista **órdenes** (una fila por orden con tareas propias),
   ordenadas por nº de orden descendente, con progreso `hechas/total` de las tareas del
   mecánico.
2. **Separador** `GridSplitter` entre maestro y detalle.
3. **Detalle**: `dgTareasDeOrden` con las tareas propias de la orden seleccionada; edición
   inline de `Observaciones` y `Revisado / OK`; al alternar `Realizado` se refresca el
   progreso del maestro en vivo.
4. **Filtros**: `Mostrar` (Pendientes / Finalizados / Todos, por defecto Pendientes) y
   búsqueda libre (nº de orden, patente, cliente).
5. **Abrir orden completa**: botón `🔗 Ver Orden Completa` y doble clic en la fila.

## Archivos

| Archivo | Cambio |
|---|---|
| `Presentacion/ViewModels/MisTrabajosViewModel.cs` | Nuevo `MiOrdenItemViewModel` (maestro); `MiTrabajoItemViewModel` → `MiTareaItemViewModel` (detalle, con `OrdenPadre`, `IsModified`); `Ordenes`/`OrdenesView` + `TareasDeOrden`; `LoadAsync` agrupa por orden y ordena desc; `GuardarCambiosAsync` persiste todas las órdenes; `PuedeImprimirHistorial` según `OrdenSeleccionada`. |
| `Presentacion/Pantalla/MisTrabajos/CtlMisTrabajos.xaml` | Layout maestro-detalle (maestro `3*` / detalle `4*` con `GridSplitter`); `dgOrdenes` + `dgTareasDeOrden`; estados vacíos; pie con `Tareas del Día`, `Historial Clínico` y `Guardar Cambios`. |
| `Presentacion/Pantalla/MisTrabajos/CtlMisTrabajos.xaml.cs` | `ConfigurarViewModel` con `vmMisTrabajos`; `BtnVerOrden_Click`; `DgOrdenes_MouseDoubleClick` (solo si el doble clic cae sobre una fila). |

Sin cambios en `RegistroServicioDto`, `DetalleServicioDto`, repositorios, handlers, DI ni
permisos.

## Verificación

```powershell
dotnet build "TP_ControlVehicular.slnx"   # 0 Errores
openspec validate mis-trabajos-master-detail --strict   # Change '...' is valid
```

- `openspec validate` exige un bloque `#### Scenario:` por requisito; la spec delta se
  redactó con escenarios BDD para cumplir el esquema.
- `LoadAsync` conserva la selección previa si la orden sigue visible; si no, selecciona la
  primera orden visible.

## Pendiente

- **Smoke test manual** (a cargo del usuario): seleccionar orden → cargar detalle; editar
  `Observaciones` y alternar `Revisado / OK` → `Guardar Cambios` → recargar y comprobar
  persistencia y progreso.

## Notas

- Sin commitear (no es alcance): `.agents/`, `docs/script/update_vm_11.py`,
  `docs/script/update_xaml_11.py`.
