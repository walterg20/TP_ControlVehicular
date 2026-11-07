# TP_ControlVehicular — Memoria de estado

Aplicación WPF .NET 10 MVVM para control vehicular con EF Core y SQL Server.

> `MEMORY.md` es una **pista**, no la evidencia: el estado real se verifica contra `git log` / `git status`; si discrepan, gana el repositorio y se corrige esta memoria.
> **Fuente canónica de reglas: `AGENTS.md`.** El conocimiento de dominio está en `docs/referencia/` (reportes, dashboard, BD y estados, Mis Trabajos). Este archivo NO duplica convenciones.

## Estado actual (verificar con `git`)
- Rama de trabajo: `feature/manejo-errores-unificado`. Spec `manejo-errores-unificado` implementada (build con 0 errores; 150 tests unitarios en verde).
- **Manejo unificado de errores silenciosos (spec `manejo-errores-unificado`)**:
  - Servicio `IErrorNotifier`/`ErrorNotifierService` con `Notify(ex, contexto, modal)`; dedup 2s; log a `%AppData%/TP_ControlVehicular/logs/error.log`; evento `ErrorOccurred`.
  - Registro global en `App` (`DispatcherUnhandledException`, `TaskScheduler.UnobservedTaskException`, `AppDomain.UnhandledException`).
  - `MainWindow.MostrarError(mensaje, modal)` unificado (toast por defecto, modal opcional) suscrito al evento.
  - Auditoría de ~59 catch silenciosos en code-behind + VMs: ahora notifican vía el notifier. Catch con `Debug.WriteLine` también notifican.
- Rama de trabajo: `feature/fix-form-cliente-vehiculo`. Spec `fix-form-cliente-vehiculo` implementada (build con 0 errores; 147 tests unitarios en verde).
- **Corrección de etiquetas y limpieza de campos en FrmClienteVehiculo (spec `fix-form-cliente-vehiculo`)**:
  - Reorganización de `FrmClienteVehiculo.xaml` con etiquetas individuales arriba de cada campo (DNI, Apellido, Nombre, Teléfono a la izquierda; Patente, Marca, Modelo, Año a la derecha).
  - Eliminación definitiva del campo informativo no persistido `Motor` (`txtMotor`) y de la propiedad correspondiente en `ClienteVehiculoViewModel.cs`.

- Rama de trabajo: `feature/test-simulacion-15-ordenes`. Spec `test-simulacion-15-ordenes` implementada (build con 0 errores; test de simulación end-to-end y 144 tests en verde).
- **Test automatizado de simulación de 15 órdenes de servicio (spec `test-simulacion-15-ordenes`)**:
  - Suite de integración completa `SimulacionOrdenesServicioTests` (`TP_ControlVehicular.Tests/EndToEnd/`).
  - Creación de 8 clientes y 8 vehículos vinculados (`PropietarioVehiculo`).
  - Generación de 15 órdenes de servicio con asignaciones de 1 mecánico (órdenes 1 a 5), 2 mecánicos colaborando en paralelo (órdenes 6 a 10) y 3 mecánicos atendiendo distintas tareas (órdenes 11 a 15).
  - Cobertura de estados canónicos (`Abierta`, `En Proceso`, `Completada`, `Pagada`, `Cancelada`).
  - Facturación y cobro real con `IFacturacionService` (`Factura` + `Pago` con vuelto para efectivo y pago exacto para tarjeta).
  - Limpieza automática idempotente con prefijo `SIM-` en orden FK-safe.

- Rama de trabajo: `feature/alineacion-estilos-crud`. Spec `alineacion-estilos-crud` implementada (build con 0 errores; 147 tests unitarios en verde).
- **Alineación de estilos y estados en pantallas CRUD (spec `alineacion-estilos-crud`)**:
  - Estandarización de `CtlCliente`, `CtlUsuario`, `CtlRol`, `CtlServicio` y `CtlTaller` tomando como base `CtlModelo` (sumándose a `CtlModelo`, `CtlMarca` y `CtlVehiculo` ya alineados).
  - **Filtro de Estado**: Reemplazo de checkbox 'Mostrar inactivos' o ausencia de filtro por ComboBox 'Estado:' con opciones 'Todos', 'Activos', 'Inactivos', condicionado por `PuedeVerFiltroEstado`.
  - **Grilla DataGrid**: Eliminación de la columna redundante de checkbox 'Activo'. Filas inactivas resaltadas automáticamente en rojo suave (`Foreground="#C0392B"`, `Background="#FDECEA"`).
  - **Botonera Inferior**: Botoneras alineadas a la derecha con paleta uniforme (✏️ Editar `#F39C12`, 🗑️ Eliminar `#C0392B`, 🔄 Reactivar `#27AE60`).
  - **Capa Negocio y Datos**: Métodos `GetPaginadoAsync` y handlers actualizados para recibir `string? filtroEstado` ('Todos', 'Activos', 'Inactivos').
  - **Tests**: 147 tests unitarios en verde sin paralelización conflictiva de sesiones (`TestCollectionBehavior.cs`).

- Rama de trabajo: `feature/recuperacion-base-datos`. Spec `recuperacion-base-datos-doble-factor` implementada (build sin errores; los 6 E2E de paginación fallan por falta de SQL Server, igual que antes del cambio).
- **Recuperación de base de datos con doble factor (spec `recuperacion-base-datos-doble-factor`)**:
  - Nuevo método `RestaurarAsync(archivoBak, dniAutorizador, contrasenaAutorizador)` en `IDatabaseBackupService`/`DatabaseBackupService`. Valida segundo factor (usuario distinto, activo, contraseña correcta, rol con permiso `Configuracion.Backup.Gestionar`) y restaura la BD productiva conectándose a `master`, con `SINGLE_USER` → `RESTORE ... WITH REPLACE, RECOVERY` → `MULTI_USER`.
  - `BackupViewModel`: propiedad `PuedeRestaurar`, comando `RestaurarBackupCommand`, método `RestaurarBackupAsync()` (confirmación + modal + servicio + resultado).
  - Nuevo modal `FrmAutorizacionBackup` (`Presentacion/Pantalla/Configuracion/`) para DNI + contraseña del autorizador.
  - Botón "🔁 Restaurar Backup" (rojo) en `CtlBackup.xaml`.
- **Buscador universal + alta unificada en Orden de Servicio (spec `busqueda-universal-alta-unificada`)**:
  - El selector de cliente de `CtlOrdenServicioForm` se reemplazó por `CtlBuscadorUniversal` (`Presentacion/Pantalla/OrdenServicio/`), un control `TextBox` + `Popup`/`ListBox` (no un `ComboBox` editable: sus semánticas de texto/foco son frágiles). Filtra desde **3 caracteres** por DNI, apellido, nombre y patente a la vez (`OrdenServicioViewModel.FiltrarBusquedaUniversal`), con debounce de 250 ms.
  - Fila del desplegable: `[Patente] - Apellido, Nombre (DNI) - Vehiculo` (`DisplayText` del DTO `Negocio/DTOs/BusquedaClienteVehiculoDto.cs`); el campo de texto queda con el nombre del dueño (`TextMostrar`).
  - Cliente con 2 o más vehículos → fila agrupada `Apellido, Nombre (DNI: nnn) - [Ver N vehiculos]`: al elegirla se completa sólo `ClienteId` y se abre el desplegable de vehículos. Fila normal → completa `ClienteId` y `VehiculoId` y el foco pasa a Kilometraje.
  - Sin coincidencias → fila de acción `🔍 No se encontraron resultados. + Registrar Nuevo Cliente y Vehiculo` que abre `FrmClienteVehiculo` (modal de dos columnas: cliente a la izquierda, vehículo a la derecha) con `ClienteVehiculoViewModel` (`Presentacion/ViewModels/`), que inserta cliente → vehículo → vínculo `PropietarioVehiculo` y expone `ClienteCreado`/`VehiculoCreado`. La patente se prellena si el texto tipeado "parece patente" (`OrdenServicioViewModel.ParecePatente`: letras + números, mínimo 5 caracteres). Al guardar, la orden queda con cliente y vehículo seleccionados y el foco pasa a `Kilometraje`.
  - El botón `➕` de vehículo, si ya hay un cliente seleccionado, reutiliza ese cliente (`ClienteVehiculoViewModel.ClienteExistente` / `Reiniciar(...)`): solo crea el vehículo y su vínculo.
  - 28 tests nuevos en `TP_ControlVehicular.Tests/ViewModels/OrdenServicioBuscadorUniversalTests.cs` y `ClienteVehiculoViewModelTests.cs`.
- Rama de trabajo previa: `feature/orden-servicio-asignacion-mecanico`. Spec `orden-servicio-asignacion-mecanico` implementada y testeada con 117 tests unitarios en verde.
- **Asignación y validación de mecánico en Orden de Servicio (spec `orden-servicio-asignacion-mecanico`)**:
  - Preselección automática del primer mecánico disponible en `BtnAgregarItem_Click` cuando el usuario en sesión es recepcionista o administrador, evitando que los ítems queden con `UsuarioId = 0`.
  - Validación defensiva en UI (`BtnGuardar_Click` en `CtlOrdenServicioForm`) y en ViewModel (`GuardarOrdenAsync` en `OrdenServicioViewModel`), alertando amigablemente si algún servicio no tiene mecánico asignado y evitando excepciones de clave foránea `FK_DetalleServicio_Usuarios_UsuarioId`.
  - Carga segura de mecánicos en `LoadCombosAsync` usando `_usuarioRepository.GetActivosAsync()` con navegación `Rol` incluida y filtrado por estado activo.
  - 3 pruebas unitarias nuevas en `TP_ControlVehicular.Tests/ViewModels/OrdenServicioMecanicoAsignacionTests.cs`.
- Rama de trabajo previa: `feature/orden-servicio-busqueda-combos`. La spec `orden-servicio-busqueda-combos` implementada y testeada.
- **Búsqueda predictiva en Orden de Servicio (spec `orden-servicio-busqueda-combos`)**:
  - Selector de cliente (`cmbClientes`, luego reemplazado por el buscador universal `CtlBuscadorUniversal`) editable con umbral de 3 caracteres (`FiltrarClientes(string? texto)`): filtra insensible a mayúsculas/minúsculas por Nombre, Apellido, DNI o NombreCompletoConDni, y con menos de 3 caracteres preserva o restaura la colección sin filtrado agresivo. Despliegue automático preservando el cursor (`SelectionStart`). Cascada automática con `VehiculosDisponibles` al seleccionar un cliente.
  - Selector de servicios (`cmbServicios`) editable con modo dual (`FiltrarServicios(string? texto)`): desplegable manual muestra el catálogo completo, y al teclear 3 o más caracteres filtra por nombre abriendo sugerencias. Al pulsar "➕ Agregar", se añade a la orden actual y se restablece el catálogo completo.
  - 16 pruebas unitarias en `TP_ControlVehicular.Tests/ViewModels/OrdenServicioCombosBusquedaTests.cs`.
- Rama de trabajo previa: `feature/dashboard-graficos-por-rol` (gráficos adicionales de Modelos Más Reparados y Servicios por Cantidad para Administrador).
- **Paginación en BD (spec `paginacion-listados`, MERGEADA Y ARCHIVADA)**: infraestructura compartida `IPaginacion` + `BasePaginableViewModel` (`Presentacion/ViewModels/`), `PaginacionViewModel` para pantallas con varias grillas paginadas, control reutilizable `CtlPaginacion` (`Presentacion/Pantalla/Compartido/`) y helper `Debouncer` (`Presentacion/Helpers/`, 500 ms). Listados migrados: Reporte Gerencial (las 4 pestañas: Ingresos, Top Clientes, Modelos más reparados y Servicios por rango), Clientes, Marcas, Modelos, Vehículos, Roles, Usuarios, Servicios, Talleres, Órdenes de Trabajo, Mis Trabajos (maestro), Reporte Órdenes y Reportes de Recepción. SPs en `bd/03_procedimientos.sql`: `sp_ReporteOrdenesPaginado` (con `STRING_AGG`), `sp_ReporteTopClientesPaginado`, `sp_ReporteModelosReparadosPaginado`, `sp_ReporteServiciosPorRangoPaginado`, `sp_HistorialVehiculoPaginado` y `sp_VehiculosPorClientePaginado` (con `@TamanoPagina = 0` devuelven el conjunto completo para el PDF). Los PDFs de reporte siguen exportando el conjunto filtrado completo. 87 tests en verde (11 E2E de paginación + 6 E2E del reporte gerencial).
- **Pestañas del Reporte Gerencial**: las tres ranking (Top Clientes, Modelos más reparados, Servicios por rango) dejaron de limitarse con `TOP 10` y ahora muestran el ranking completo paginado; sus PDF también exportan el ranking completo. Los SP originales con `@TopN` se conservan porque `ReporteOperativoViewModel` los sigue usando con top 10.
- **Gráfico de ingresos del Reporte Gerencial**: `IngresosBarChartGenerator` elige la agregación automáticamente — por período mensual (etiqueta `MM/aa`) cuando el rango abarca más de un mes calendario, por día (etiqueta `dd/MM`) cuando el rango cubre un solo mes. El SVG incluye el título "Ingresos por mes"/"Ingresos por día". El gráfico y la tabla de resumen por mes usan la misma fuente (`IngresosMensuales`), así que no pueden diferir.
- Rama de trabajo anterior: `develop` (ambas specs cerradas y mergeadas: `baja-logica-consistente` y `unificar-nomenclatura-activo`).
- Últimas specs archivadas y mergeadas: `reporte-gerencial-mejoras`, `dashboard-por-perfil`, `mis-trabajos-master-detail`, `reporte-operativo-productividad-mecanicos`, `custom-alert-dialogs`, `custom-confirmation-dialog`, `fix-taller-abm`, `orden-servicio-ux-mejorada`, `show-all-users`, `2026-10-12-reorganizacion-bd`, `2026-10-10-logo-empresa`, `2026-10-11-backup-drive`, `2026-10-11-manuales-usuario-tecnico`, `2026-10-26-baja-logica-consistente`, `2026-10-26-unificar-nomenclatura-activo`, `2026-10-28-graficos-barras-reporte-gerencial`, `2026-10-28-reporte-gerencial-paginacion-agrupacion-ingresos`. `2026-10-29-paginacion-listados`.
- **Paginación en BD y agrupación mensual en Reporte Gerencial**: paginación a nivel de base de datos en la grilla de Ingresos Totales mediante `sp_ReporteIngresosAdminPaginado` (`OFFSET ... FETCH NEXT ... ROWS ONLY` con CTE y `COUNT(*) OVER() AS TotalRegistros`), barra de navegación en WPF con botones Inicio/Anterior/Siguiente/Fin, selector de tamaño de página (10, 20, 50, 100) y contador dinámico. Agrupación mensual mediante `sp_ReporteIngresosMensualesAdmin` con tarjeta colapsable/expansible en pantalla y sección de resumen mensual con tabla y gráfico de barras en el PDF exportado de QuestPDF.
- **Gráficos de barras en Reporte Gerencial**: generadores SVG puros (`IngresosBarChartGenerator` para barras verticales de ingresos por fecha y `ModelosBarChartGenerator` para barras horizontales de ranking de modelos más reparados) incrustados en los PDFs exportados con QuestPDF vía `.Svg(...)`. Mantiene la vista WPF rápida sin dependencias pesadas.
- Baja lógica unificada bajo `Activo` en `Cliente`, `Taller`, `Servicio`, `Rol` y `Usuario`: defaults `Activo=true` (entidad + `DEFAULT (1)` en BD), listado de activos por defecto + `GetTodosAsync()` explícito, acción "Reactivar" y control "Mostrar inactivos" en Cliente/Taller/Servicio/Rol. `RegistroServicio`/`DetalleServicio` mantienen `Estado` (estado de flujo).
- **Versionado de tests**: `.gitignore` ya **no** ignora `TP_ControlVehicular.Tests/` (se permite versionar `EndToEnd/` y `Unit/`; `bin/`/`obj/` siguen ignorados por las reglas globales `[Bb]in/` y `[Oo]bj/`). 87 tests en verde (81 unitarios + 6 E2E nuevos del reporte gerencial; 17 E2E en total contra `ControlVehicular_Test`).
- Specs publicadas a partir de esos changes: `configuracion-empresa` (logo/identidad de empresa; `reportes` ampliada con datos de encabezado) · `backup-base-datos` (respaldo full + verificación + Google Drive; dominio en `docs/referencia/backup.md`) · `manuales-documentacion` (entrega `docs/manuales/{README.md, manual_usuario_roles.md, manual_tecnico_administracion.md}` con marcadores de captura; las `.png` las agrega el usuario).
- Unificación canónica de scripts en `bd/`: `01_estructura.sql` (DDL 18 tablas + migración defensiva Activo/Checks), `02_datos.sql` (seed canónico limpio: 3 roles, 38 permisos, matriz RolPermisos, taller, configs singleton, usuario Admin bootstrap, 15 servicios, 45 marcas y 203 modelos), `03_procedimientos.sql` (22 SPs oficiales), `04_triggers.sql` (3 triggers de negocio), `05_utilidades_test.sql` (purga artefactos E2E) y `06_poblacion.sql` (población operativa/demo independiente: personal recepcionista y mecánicos, 55 clientes, 55 vehículos, 55 titularidades, 61 órdenes de servicio desde junio 2026 en distintos estados, 110+ detalles y facturación/pagos).
- **Separación Canónica vs. Población (`bd/02_datos.sql` y `bd/06_poblacion.sql`)**: `02_datos.sql` queda como semilla base exclusiva de producción / inicialización limpia sin datos ficticios. Los datos ampliados de prueba y demostración (55 clientes, 55 vehículos, 61 órdenes, facturas y pagos) se aíslan en `06_poblacion.sql`, de ejecución opcional e independiente con deshabilitación temporal de triggers durante la carga masiva.
- Change cerrado (rama `feature/reorganizacion-bd`, mergeada con `--no-ff` y archivado): `2026-10-12-reorganizacion-bd` (capability `esquema-base-datos`). Consolida `bd/` en scripts **canónicos por tipo** (`01_estructura.sql`, `02_datos.sql`, `03_procedimientos.sql`, `04_triggers.sql`, `05_utilidades_test.sql`, `06_poblacion.sql` + `README.md`) y compacta `docs/` (DER/ERS finales; versiones 1 y material de trabajo en `docs/historico/`). Plan/bitácora en `docs/plan/2026-10-12_reorganizacion-bd.md`.

## Flujo (detalle en `AGENTS.md`)
- Rama por spec (`feature/<nombre>`); OpenSpec en inglés antes de tocar código.
- **Commitear SIEMPRE en la rama `feature/<nombre>`, nunca directamente en `develop`.** Si el editor/IDE auto-commitea la implementación en `develop` (en vez de la feature), la rama queda "no mergeada" y `git branch -d` se niega a borrarla; hay que mergear igual para poder limpiarla.
- Bitácoras/planes en `docs/plan/AAAA-MM-DD_<spec>.md` (español).
- Cerrar spec: merge → archivar change → actualizar `MEMORY.md` → commit `docs(memory): ...`.
- Verificación obligatoria: `dotnet build "TP_ControlVehicular.slnx"` sin errores.

## Registro histórico (punteros)
| Spec | Dominio | Dónde |
|---|---|---|
| `reporte-gerencial-mejoras` | Reportes Gerencial (sp, SVG torta) | `docs/referencia/reportes.md` |
| `dashboard-por-perfil` | Dashboard por rol + Ingresos Admin | `docs/referencia/dashboard.md` |
| Cancelación de órdenes / artefactos E2E | Estados y scripts `bd/` | `docs/referencia/bd-y-dominio.md` |
| Mis Trabajos (mecánico) | Pantalla maestro-detalle | `docs/referencia/mis-trabajos.md` |
| Respaldo de BD + Google Drive | Config, backup/restore, permisos (DDL `bd/01`, seed `bd/04`) | `docs/referencia/backup.md` |
| `manuales-usuario-tecnico` | Manuales: usuario por roles + técnico/TI | `docs/manuales/` |
| `2026-10-12-reorganizacion-bd` | Estructura `bd/` canónica por tipo + seed de marcas/modelos AR | `docs/referencia/bd-y-dominio.md` |
