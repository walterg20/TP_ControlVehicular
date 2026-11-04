# Plan: Buscador Universal y Alta Unificada (Cliente + Vehículo)

> Bitácora de ejecución de la spec `busqueda-universal-alta-unificada`.
> Especificación formal: `openspec/changes/busqueda-universal-alta-unificada/`.

## Objetivo

En la pantalla de alta/edición de órdenes de servicio (`CtlOrdenServicioForm`), reemplazar el combo
de clientes (que sólo busca por nombre/apellido/DNI) por un **buscador universal** que coincide
simultáneamente por DNI, apellido, nombre y patente, con autocompletado de cliente + vehículo, y
agregar un **flujo unificado de alta** (cliente + vehículo + vínculo) cuando la búsqueda no arroja
resultados.

## Decisiones confirmadas

| Decisión | Valor |
|---|---|
| Umbral de caracteres para la búsqueda predictiva | **3** (consistente con los combos ya implementados) |
| Formato de fila | `[Patente] - Apellido, Nombre (DNI) - Vehiculo` |
| Cliente con varios vehículos | Fila agrupada `Apellido, Nombre (DNI: nnn) - [Ver N vehiculos]` |
| Sin resultados | Fila de acción `🔍 No se encontraron resultados. + Registrar Nuevo Cliente y Vehiculo` |
| Persistencia | Sin cambios de esquema ni SP: la búsqueda corre sobre los catálogos ya cargados en memoria |
| Motor (cilindrada) | Capturado en el formulario pero **no persistido** (la tabla `Vehiculo` no tiene esa columna) |
| API de consulta de DNI | No existe en el proyecto: el DNI se carga a mano |

## Fases

### Fase 1 — Modelo de datos
Crear `Negocio/DTOs/BusquedaClienteVehiculoDto.cs`.

### Fase 2 — ViewModel del buscador universal
`OrdenServicioViewModel`: colección `ResultadosBusquedaUniversal`, `FiltrarBusquedaUniversal`,
`SeleccionarResultadoUniversal`, eventos de apertura de modal y de foco.

### Fase 3 — Modal unificado de alta
`ClienteVehiculoViewModel` + `FrmClienteVehiculo` (dos columnas) + registro en DI.

### Fase 4 — Integración en el formulario de orden
XAML del combo universal + code-behind (debounce, selección, foco, modal).

### Fase 5 — Pruebas
`OrdenServicioBuscadorUniversalTests` y `ClienteVehiculoViewModelTests`.

### Fase 6 — Verificación
`dotnet build` + `dotnet test` + validación interactiva.

## Bitácora

- **2026-11-03** — Rama `feature/busqueda-universal-alta-unificada` creada desde `develop`.
  Spec OpenSpec escrita (proposal / spec delta / design / tasks). Umbral de 3 caracteres confirmado
  por el usuario. Fase 1 iniciada.

## Bitácora de ejecución

| Fase | Commit | Resultado |
|---|---|---|
| 1 — Modelo de datos | `feat(orden-servicio): agregar buscador universal...` | `Negocio/DTOs/BusquedaClienteVehiculoDto.cs` + `FiltrarBusquedaUniversal` / `SeleccionarResultadoUniversal` en `OrdenServicioViewModel` |
| 2 — ViewModel | (mismo commit que la fase 1) | Colección, eventos de foco y de apertura de modal, helper `ParecePatente` |
| 3 — Modal unificado | `feat(orden-servicio): agregar modal unificado...` | `ClienteVehiculoViewModel` + `FrmClienteVehiculo` + registro en DI |
| 4 — Integración UI | `feat(orden-servicio): integrar el control de busqueda universal...` | `CtlBuscadorUniversal` (TextBox + Popup/ListBox) reemplaza al combo de clientes |
| 5 — Pruebas | `test(orden-servicio): agregar pruebas del buscador universal...` | 19 + 9 tests nuevos, 144 unitarios en verde |
| 6 — Verificación | — | `dotnet build` sin errores; `dotnet test` sin errores en unitarios |

## Desviaciones respecto del plan inicial

1. **El buscador no se implementó como `ComboBox` editable** sino como `TextBox` + `Popup`/`ListBox`
   (`CtlBuscadorUniversal`). Un `ComboBox` editable escribe el texto del ítem seleccionado en su
   caja interna y pierde el cursor al repoblar la colección, lo que hacía imposible mostrar "el
   nombre del dueño" en el campo y "la fila completa" en el desplegable de forma determinista.
2. **El modal unificado admite cliente preexistente.** El botón `➕` de vehículo, cuando ya hay
   un cliente seleccionado, crea sólo el vehículo y su vínculo (`ClienteVehiculoViewModel.ClienteExistente`),
   de modo que no se pierde la capacidad de agregar un segundo auto a un cliente ya cargado.
3. **`FrmCliente` y `FrmVehiculo` no se tocaron** (siguen en uso por sus propias pantallas).

## Pendiente de validación manual

La tarea 6.3 de la spec (validación interactiva en la pantalla *Registrar Nueva Orden*) queda
pendiente: no es automatizable sin instancia de SQL Server ni interacción con la UI.
