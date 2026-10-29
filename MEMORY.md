# TP_ControlVehicular — Memoria de estado

Aplicación WPF .NET 10 MVVM para control vehicular con EF Core y SQL Server.

> `MEMORY.md` es una **pista**, no la evidencia: el estado real se verifica contra `git log` / `git status`; si discrepan, gana el repositorio y se corrige esta memoria.
> **Fuente canónica de reglas: `AGENTS.md`.** El conocimiento de dominio está en `docs/referencia/` (reportes, dashboard, BD y estados, Mis Trabajos). Este archivo NO duplica convenciones.

## Estado actual (verificar con `git`)
- Rama de trabajo: `feature/paginacion-listados` (desde `develop`). Change en curso: `openspec/changes/paginacion-listados/`.
- **Paginación en BD (spec `paginacion-listados`, Fases 1 y 2 completas)**: infraestructura compartida `IPaginacion` + `BasePaginableViewModel` (`Presentacion/ViewModels/`), control reutilizable `CtlPaginacion` (`Presentacion/Pantalla/Compartido/`) y helper `Debouncer` (`Presentacion/Helpers/`, 500 ms). Listados migrados: Reporte Gerencial (barra existente), Clientes (piloto), Marcas, Modelos, Vehículos, Roles, Usuarios, Servicios y Talleres — todos con `Repository<T>.GetPaginadoAsync(...)` (EF Core → `OFFSET ... FETCH NEXT`), método paginado por repositorio, `HandlePaginadoAsync` en el `Listar*Handler` y búsqueda server-side (botón Buscar + Enter + debounce). Los listados transaccionales y de reportes (OrdenServicio, Mis Trabajos, Reporte Órdenes, Reportes Recepción) y sus SPs nuevos quedan para la Fase 3.
- Rama de trabajo anterior: `develop` (ambas specs cerradas y mergeadas: `baja-logica-consistente` y `unificar-nomenclatura-activo`).
- Últimas specs archivadas y mergeadas: `reporte-gerencial-mejoras`, `dashboard-por-perfil`, `mis-trabajos-master-detail`, `reporte-operativo-productividad-mecanicos`, `custom-alert-dialogs`, `custom-confirmation-dialog`, `fix-taller-abm`, `orden-servicio-ux-mejorada`, `show-all-users`, `2026-10-12-reorganizacion-bd`, `2026-10-10-logo-empresa`, `2026-10-11-backup-drive`, `2026-10-11-manuales-usuario-tecnico`, `2026-10-26-baja-logica-consistente`, `2026-10-26-unificar-nomenclatura-activo`, `2026-10-28-graficos-barras-reporte-gerencial`, `2026-10-28-reporte-gerencial-paginacion-agrupacion-ingresos`.
- **Paginación en BD y agrupación mensual en Reporte Gerencial**: paginación a nivel de base de datos en la grilla de Ingresos Totales mediante `sp_ReporteIngresosAdminPaginado` (`OFFSET ... FETCH NEXT ... ROWS ONLY` con CTE y `COUNT(*) OVER() AS TotalRegistros`), barra de navegación en WPF con botones Inicio/Anterior/Siguiente/Fin, selector de tamaño de página (10, 20, 50, 100) y contador dinámico. Agrupación mensual mediante `sp_ReporteIngresosMensualesAdmin` con tarjeta colapsable/expansible en pantalla y sección de resumen mensual con tabla y gráfico de barras en el PDF exportado de QuestPDF.
- **Gráficos de barras en Reporte Gerencial**: generadores SVG puros (`IngresosBarChartGenerator` para barras verticales de ingresos por fecha y `ModelosBarChartGenerator` para barras horizontales de ranking de modelos más reparados) incrustados en los PDFs exportados con QuestPDF vía `.Svg(...)`. Mantiene la vista WPF rápida sin dependencias pesadas.
- Baja lógica unificada bajo `Activo` en `Cliente`, `Taller`, `Servicio`, `Rol` y `Usuario`: defaults `Activo=true` (entidad + `DEFAULT (1)` en BD), listado de activos por defecto + `GetTodosAsync()` explícito, acción "Reactivar" y control "Mostrar inactivos" en Cliente/Taller/Servicio/Rol. `RegistroServicio`/`DetalleServicio` mantienen `Estado` (estado de flujo).
- **Versionado de tests**: `.gitignore` ya **no** ignora `TP_ControlVehicular.Tests/` (se permite versionar `EndToEnd/` y `Unit/`; `bin/`/`obj/` siguen ignorados por las reglas globales `[Bb]in/` y `[Oo]bj/`). 67 tests unitarios pasando al 100% (39 previos + 28 nuevos de paginación).
- Specs publicadas a partir de esos changes: `configuracion-empresa` (logo/identidad de empresa; `reportes` ampliada con datos de encabezado) · `backup-base-datos` (respaldo full + verificación + Google Drive; dominio en `docs/referencia/backup.md`) · `manuales-documentacion` (entrega `docs/manuales/{README.md, manual_usuario_roles.md, manual_tecnico_administracion.md}` con marcadores de captura; las `.png` las agrega el usuario).
- Unificación canónica de scripts en `bd/`: `01_estructura.sql` (DDL + migración Activo), `02_datos.sql` (seed canónico completo: roles, permisos, usuario Admin, recepcionista y mecánicos de equipo, 15 servicios, 45 marcas y 203 modelos, 55 clientes, 55 vehículos, 55 titularidades, 61 órdenes de servicio desde junio 2026 en distintos estados, 110+ detalles y facturación/pagos), `03_procedimientos.sql` (16 SPs), `04_triggers.sql` (3 triggers) y `05_utilidades_test.sql`.
- **Datos semilla ampliados (`bd/02_datos.sql`)**: 55 clientes locales, 55 vehículos asociados al catálogo oficial, 55 vínculos de titularidad (`PropietarioVehiculo`), 61 órdenes de servicio distribuidas entre Junio y Octubre de 2026 (abarcando estados Pagada, Completada, En Proceso, Abierta y Cancelada), 110+ detalles de servicio asignados equitativamente a mecánicos y 51 facturas y pagos sincronizados. Totalmente idempotente y compatible con los triggers de integridad de kilometraje y bloqueo de facturación.
- Change cerrado (rama `feature/reorganizacion-bd`, mergeada con `--no-ff` y archivado): `2026-10-12-reorganizacion-bd` (capability `esquema-base-datos`). Consolida `bd/` en scripts **canónicos por tipo** (`01_estructura.sql`, `02_datos.sql`, `03_procedimientos.sql`, `04_triggers.sql`, `05_utilidades_test.sql` + `README.md`) y compacta `docs/` (DER/ERS finales; versiones 1 y material de trabajo en `docs/historico/`). Plan/bitácora en `docs/plan/2026-10-12_reorganizacion-bd.md`.

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
