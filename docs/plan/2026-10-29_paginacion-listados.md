# Plan de ejecución — `paginacion-listados` (Fase 1)

**Spec:** `openspec/changes/paginacion-listados/`
**Rama:** `feature/paginacion-listados` (base: `develop` @ `b86b6bf`)
**Fecha:** 2026-10-29

---

## Objetivo

Eliminar el filtrado en memoria de los listados: la paginación y la búsqueda pasan a ejecutarse en la base de datos. La spec se organiza en tres fases por riesgo:

| Fase | Contenido | Riesgo |
|---|---|---|
| 1 | Infraestructura compartida + piloto en Clientes | Bajo |
| 2 | CRUD restantes (Marca, Modelo, Vehículo, Rol, Usuario, Servicio, Taller) | Bajo |
| 3 | Transaccionales y reportes (OrdenServicio, Mis Trabajos, Reporte Órdenes, Reportes Recepción) | Alto (SQL nuevo) |

---

## Fase 1 — Infraestructura y piloto (COMPLETADA)

### 1. Qué se construyó

**Infraestructura compartida**

- `Presentacion/ViewModels/IPaginacion.cs` — contrato de paginación (`INotifyPropertyChanged`) que consume el control.
- `Presentacion/ViewModels/BasePaginableViewModel.cs` — `BaseViewModel` + `IPaginacion`. Centraliza estado de página, clamp del índice, cálculo de `TotalPaginas`, normalización de `TamanoPagina` (5..200), comandos de navegación y el hook abstracto `CargarPaginaAsync()`.
- `Presentacion/Pantalla/Compartido/CtlPaginacion.xaml(.cs)` — control reutilizable. Réplica exacta de la barra que tenía `CtlReporteGerencial` embebida (`#F8F9F9`, borde `#E5E7E9`, `CornerRadius=8`, `Padding=12,8`, botones `#2C3E50`). Recibe el estado por la DP `Model` de tipo `IPaginacion` y enlaza el contenido contra ese modelo (no contra el `DataContext` de la pantalla).
- `Presentacion/Helpers/Debouncer.cs` — coalesce de llamadas con retardo de 500 ms. Usa `Task.Delay` con `CancellationTokenSource`, que captura el `SynchronizationContext` de WPF: la acción diferida vuelve al hilo de interfaz.

**Migración del Reporte Gerencial**

- `ReporteGerencialViewModel` ahora hereda `BasePaginableViewModel`; se eliminó el estado de paginación duplicado. Se conservaron los nombres y tipos públicos (`PaginaActual`, `TotalPaginas`, `TotalRegistros`, `TamanoPagina`, `OpcionesTamanoPagina`, `TextoPaginacion`, `PuedeIrAnterior`, `PuedeIrSiguiente` y los cuatro comandos `RelayCommand`), por lo que `ReporteGerencialPaginacionTests` sigue pasando sin cambios.
- `CtlReporteGerencial.xaml` reemplaza el `Border` de la barra por `<compartido:CtlPaginacion Grid.Row="3" Model="{Binding}"/>`.

**Capa de datos**

- `Datos/Repositories/Repository.cs` — nuevo `GetPaginadoAsync(pagina, tamanoPagina, filtro, configurador, orden)`: `Skip`/`Take` (EF Core 10 los traduce a `OFFSET ... FETCH NEXT ... ROWS ONLY`) + `CountAsync()` sobre la misma consulta filtrada. Se agrega el helper `CrearPatronLike` que escapa `%`, `_` y `[` para que la búsqueda sea de subcadena literal y no de comodines.
- `Negocio/Interfaces/IClienteRepository.cs` + `Datos/Repositories/ClienteRepository.cs` — `GetPaginadoAsync(busqueda, incluirInactivos, pagina, tamanoPagina)` con `EF.Functions.Like` sobre `Nombre`, `Apellido` y `Dni`, y filtro de `Activo` según `incluirInactivos`.

**Piloto en Clientes**

- `ClienteViewModel` hereda `BasePaginableViewModel`. Se eliminó `ListadoClientesFiltered` (el filtrado en memoria). `CargarPaginaAsync()` consulta el repositorio paginado y llama a `ActualizarTotales(total)`.
- Búsqueda server-side: el setter de `TextoBusqueda` sólo actualiza estado y reinicia la página a 1; la consulta se dispara desde `BuscarAsync()` (botón `Buscar` y tecla `Enter`) y desde `ProgramarBusqueda()` (debounce de 500 ms disparado por `TextChanged`).
- `CtlCliente.xaml` — la grilla sigue enlazando a `Clientes`; se agrega la fila con `<compartido:CtlPaginacion Model="{Binding}"/>`; el `TextBox` de búsqueda gana `TextChanged` y `KeyDown`; el check `Mostrar inactivos` gana `Click` para recargar.
- `CtlCliente.xaml.cs` — `BtnBuscar_Click`, `TxtBusqueda_TextChanged`, `TxtBusqueda_KeyDown` y `ChkMostrarInactivos_Click` delegan en el ViewModel.

### 2. Decisiones tomadas durante la implementación

| Decisión | Motivo |
|---|---|
| `IPaginacion` declara los comandos como `ICommand`, pero la clase base los expone como `RelayCommand` (implementación explícita de la interfaz) | El `RelayCommand` concreto es necesario para `RaiseCanExecuteChanged()`; se preserva el tipo público que ya usaba `ReporteGerencialViewModel`. |
| `BaseViewModel.IsLoading` pasa a `virtual` y se sobreescribe en la clase base paginada | `PuedeIrAnterior`/`PuedeIrSiguiente` dependen de `IsLoading`; sin el override los comandos no se refrescaban al terminar la carga. |
| El debounce vive en `Presentacion/Helpers/` y se dispara desde el code-behind (`TextChanged`), no desde el setter de la propiedad | Mantiene el setter del ViewModel libre de efectos sobre la base de datos y hace el comportamiento determinista y testeable. |
| `TextoBusqueda` se recorta en el ViewModel antes de enviarse al repositorio | Deja explícito el contrato ViewModel → repositorio; el repositorio también recorta. |
| El filtro de búsqueda se aplica dentro de la misma consulta paginada | El `TotalRegistros` corresponde al conjunto filtrado. |
| `ClienteViewModel.CargarPaginaAsync` captura las excepciones y deja el listado vacío | Mantiene el comportamiento previo de `LoadAsync`, que no propagaba fallos del repositorio. |

### 3. Verificación

- `dotnet build "TP_ControlVehicular.slnx"` → compilación correcta, 0 errores.
- `dotnet test "TP_ControlVehicular.slnx"` → **57 / 57 tests en verde** (39 previos + 18 nuevos):
  - `TP_ControlVehicular.Tests/ViewModels/PaginacionTests.cs` (10) — clamp del índice de página, `TotalPaginas` mínimo 1, normalización de `TamanoPagina`, texto de estado, comandos deshabilitados en los bordes.
  - `TP_ControlVehicular.Tests/ViewModels/ClientePaginacionTests.cs` (7) — carga de la primera página, propagación del total, envío del término de búsqueda, índice de página solicitado, reinicio a página 1 al cambiar filtro o tamaño de página, listado vacío sin resultados.

### 4. Pendiente de validación manual

- [ ] Abrir *Control de Clientes*: la grilla trae solo la primera página y la barra muestra `Página 1 de N (M registros)`.
- [ ] Escribir en el buscador: la grilla se actualiza solo a los ~500 ms; el botón `Buscar` y `Enter` la fuerzan de inmediato.
- [ ] Navegar con ⏮ ◀ ▶ ⏭; los botones de borde quedan deshabilitados.
- [ ] Cambiar el tamaño de página: vuelve a la página 1 y recarga.
- [ ] Marcar *Mostrar inactivos*: recarga el listado.
- [ ] Abrir *Reporte Gerencial* → Tab *Ingresos Totales*: la barra migrada se ve igual que antes y navega correctamente.

---

## Fase 2 — CRUD restantes (COMPLETADA)

Mismo patrón de la Fase 1, un commit por pantalla: Marca, Modelo, Vehículo, Rol, Usuario, Servicio y Taller.

### 1. Qué se construyó

**Capa de datos** — cada repositorio expone un método paginado con búsqueda server-side, y su `Listar*Handler` agrega `HandlePaginadoAsync(...)` que mapea la página a DTOs:

| Listado | Método del repositorio | Columnas de la búsqueda |
|---|---|---|
| Marca | `IMarcaRepository.GetPaginadoAsync(busqueda, pagina, tamanoPagina)` | `NombreMarca` |
| Modelo | `IModeloRepository.GetPaginadoAsync(busqueda, pagina, tamanoPagina)` | `NombreModelo`, `Marca.NombreMarca` |
| Vehículo | `IVehiculoRepository.GetPaginadoAsync(busqueda, pagina, tamanoPagina)` | `Patente`, `Modelo.NombreModelo`, `Modelo.Marca.NombreMarca`, nombre/apellido/DNI del propietario actual |
| Rol | `IRolRepository.GetPaginadoAsync(busqueda, incluirInactivos, pagina, tamanoPagina)` | `Nombre` |
| Usuario | `IUsuarioRepository.GetPaginadoAsync(busqueda, pagina, tamanoPagina)` | `Nombre`, `Apellido`, `Dni`, `Rol.Nombre` |
| Servicio | `IServicioRepository.GetPaginadoAsync(busqueda, incluirInactivos, pagina, tamanoPagina)` | `Nombre` |
| Taller | `ITallerRepository.GetPaginadoAsync(busqueda, incluirInactivos, pagina, tamanoPagina)` | `Nombre` |

**ViewModels** — los siete pasan a `BasePaginableViewModel` y eliminan su `ListadoXFiltered`. La búsqueda se ejecuta en la base de datos y se dispara con el botón **Buscar**, con **Enter** y con el debounce de 500 ms. Los setters de filtro (`TextoBusqueda`, `MostrarInactivos`) sólo actualizan estado y reinician la página a 1.

**Vistas** — cada `Ctl*.xaml` gana la fila de la barra `<compartido:CtlPaginacion Model="{Binding}"/>` entre la grilla y el pie de acciones, y la grilla se enlaza a la colección paginada en lugar del listado filtrado.

**Code-behind** — se agregan `BtnBuscar_Click`, `TxtBusqueda_TextChanged`, `TxtBusqueda_KeyDown` y `ChkMostrarInactivos_Click` (este último solo en las pantallas con check de inactivos).

### 2. Decisiones tomadas durante la implementación

| Decisión | Motivo |
|---|---|
| `RolRepository.GetPaginadoAsync` incluye `Permisos` | `FrmRolPermiso` (botón *Administrar Permisos*) lee `RolDto.Permisos` del rol seleccionado. Antes, con *Mostrar inactivos* activado el listado usaba `GetTodosAsync()` y los permisos venían vacíos. |
| `AsSplitQuery()` en Vehículo, Rol y Usuario | Los `Include` de colecciones (propietarios, permisos) generaban un JOIN 1:N que puede alterar el `Skip`/`Take`. Con split query se pagina primero y las colecciones se cargan después para los registros de la página. |
| El filtro de `Activo` va en el `configurador` de la consulta base, no en el `filtro` | La clase base aplica `configurador` → `filtro` → `CountAsync` → `orden`. El filtro de estado tiene que quedar dentro de la misma consulta que el conteo para que `TotalRegistros` sea consistente. |
| `ServicioViewModel` elimina su `IsLoading` propio | Ocultaba (`new`) el `IsLoading` virtual de `BaseViewModel`, con lo que `PuedeIrAnterior`/`PuedeIrSiguiente` nunca se refrescaban. |
| Se agrega el botón **Buscar** a las pantallas que no lo tenían | El requisito de la spec es que la búsqueda se dispare por una acción explícita, con Enter y con debounce. |
| Las altas/ediciones recargan la página en lugar de manipular la colección | Con paginación en BD, agregar el registro a la colección en memoria lo mostraría aunque no corresponda a la página ni al filtro activo. |

### 3. Verificación

- `dotnet build "TP_ControlVehicular.slnx"` → compilación correcta, 0 errores.
- `dotnet test "TP_ControlVehicular.slnx"` → **67 / 67 tests en verde** (57 previos + 10 nuevos):
  - `TP_ControlVehicular.Tests/ViewModels/ListadosCrudPaginacionTests.cs` — carga de la primera página y propagación del total en los siete listados, envío del término de búsqueda recortado, reinicio a página 1 al cambiar el filtro y propagación del flag de inactivos.

### 4. Pendiente de validación manual

- [ ] Abrir cada una de las 7 pantallas: la grilla trae una sola página y la barra informa `Página 1 de N (M registros)`.
- [ ] Buscar con el botón, con Enter y escribiendo (debounce).
- [ ] Cambiar el tamaño de página y navegar con ⏮ ◀ ▶ ⏭.
- [ ] Marcar *Mostrar inactivos* en Roles, Servicios y Talleres.
- [ ] En Roles: abrir *Administrar Permisos* sobre un rol y verificar que los permisos figuran marcados.
- [ ] En Vehículos: verificar que la columna de cliente, marca y modelo se sigue mostrando (el `Include` con `AsSplitQuery` cambia la forma de la consulta).

---

## Fases 2 y 3 (pendientes)

### Fase 2 — CRUD restantes

Mismo patrón, un commit por pantalla: Marca, Modelo, Vehículo, Rol, Usuario, Servicio, Taller.
Requiere en cada caso: método paginado en el repositorio + interfaz, `ViewModel` paginado, `Ctl*.xaml` con `CtlPaginacion` y eventos de búsqueda, y tests unitarios del predicado de búsqueda.

### Fase 3 — Transaccionales y reportes

Requiere SQL nuevo en `bd/03_procedimientos.sql`:

- `sp_ReporteOrdenesPaginado` — proyección aplanada de órdenes (`STRING_AGG` para *Servicios Aplicados*, que no es traducible a SQL por EF Core), filtros de texto, estado, rango de fechas y mecánico.
- `sp_HistorialVehiculoPaginado` y `sp_VehiculosPorClientePaginado` — versión paginada de los SP existentes.

Se consumen con el patrón ya establecido `SqlQueryRaw("EXEC sp_...")` de `ReporteRepository`.

Además: `OrdenServicio` y el maestro de `MisTrabajos` pasan a paginar sobre `RegistroServicio` con `Include(r => r.Taller)` (hoy `TallerNombre` sale vacío en la grilla por falta de ese `Include` — se corrige de paso), y los PDF de reporte siguen exportando el conjunto completo filtrado.

---

## Notas de proceso

- El `read`/`edit` de este entorno reescribe el namespace `http://schemas.openxmlformats.org/markup-compatibility/2006` como `http://schemas.opencode.openxmlformats.org/...`. Eso rompió la compilación de los XAML nuevos (`MC3072`). Se corrigió a nivel de bytes con `docs/script/corregir-namespace-markup-compatibility.py`. **A futuro: al escribir XAML, usar entidades XML o el namespace correcto verificado con `ReadAllLines`.**
- El repositorio trabaja con `core.autocrlf=true`; los archivos se normalizaron a CRLF antes de commitear.
