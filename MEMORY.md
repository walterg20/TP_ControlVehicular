# TP_ControlVehicular — Memoria de estado

Aplicación WPF .NET 10 MVVM para control vehicular con EF Core y SQL Server.

> `MEMORY.md` es una **pista**, no la evidencia: el estado real se verifica contra `git log` / `git status`; si discrepan, gana el repositorio y se corrige esta memoria.
> **Fuente canónica de reglas: `AGENTS.md`.** El conocimiento de dominio está en `docs/referencia/` (reportes, dashboard, BD y estados, Mis Trabajos). Este archivo NO duplica convenciones.

## Estado actual (verificar con `git`)
- Rama de trabajo: `feature/pruebas-e2e-por-rol` (change activo en curso; nada commiteado aún en la rama).
- Últimas specs archivadas y mergeadas: `reporte-gerencial-mejoras`, `dashboard-por-perfil`, `mis-trabajos-master-detail`, `reporte-operativo-productividad-mecanicos`, `custom-alert-dialogs`, `custom-confirmation-dialog`, `fix-taller-abm`, `orden-servicio-ux-mejorada`, `show-all-users`, `2026-10-12-reorganizacion-bd`, `2026-10-10-logo-empresa`, `2026-10-11-backup-drive`, `2026-10-11-manuales-usuario-tecnico`.
- **Change activo**: `pruebas-e2e-por-rol` (capacidad `pruebas-e2e`; suite E2E por rol contra BD dedicada `ControlVehicular_Test`, con skip automático si no hay SQL Server). Estado: Tarea 1 (copia `bd/*.sql` al output) y Tarea 2 (infra `TP_ControlVehicular.Tests/EndToEnd/Infra/`, compila) hechas; Tareas 3–4 (unit L1 + suites E2E) y 5 (verificación/cierre) pendientes. Plan en `docs/plan/2026-10-12_pruebas-e2e-por-rol.md`.
- **Versionado de tests**: `.gitignore` ya **no** ignora `TP_ControlVehicular.Tests/` (se permite versionar `EndToEnd/` y `Unit/`; `bin/`/`obj/` siguen ignorados por las reglas globales `[Bb]in/` y `[Oo]bj/`).
- Specs publicadas a partir de esos changes: `configuracion-empresa` (logo/identidad de empresa; `reportes` ampliada con datos de encabezado) · `backup-base-datos` (respaldo full + verificación + Google Drive; dominio en `docs/referencia/backup.md`) · `manuales-documentacion` (entrega `docs/manuales/{README.md, manual_usuario_roles.md, manual_tecnico_administracion.md}` con marcadores de captura; las `.png` las agrega el usuario).
- Change cerrado (rama `feature/reorganizacion-bd`, mergeada con `--no-ff` y archivado): `2026-10-12-reorganizacion-bd` (capability `esquema-base-datos`). Consolida `bd/` en scripts **canónicos por tipo** (`01_estructura.sql`, `02_procedimientos.sql`, `03_triggers.sql`, `04_datos.sql`, `05_utilidades_test.sql` + `README.md`) y compacta `docs/` (DER/ERS finales; versiones 1 y material de trabajo en `docs/historico/`). Plan/bitácora en `docs/plan/2026-10-12_reorganizacion-bd.md`.

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
