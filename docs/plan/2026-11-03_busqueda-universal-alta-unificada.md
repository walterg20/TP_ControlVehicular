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
