# Tasks: Reorganización de Scripts de Base de Datos (`bd/`)

- [ ] 1. Preparación
  - [ ] 1.1 Rama `feature/reorganizacion-bd` creada.
  - [ ] 1.2 Change OpenSpec (`proposal.md`, `spec.md`, `design.md`, `tasks.md`).
  - [ ] 1.3 Diagnóstico y clasificación de los 26 `.sql` de `bd/` (estructura, SP, triggers, datos, obsoletos).
  - [ ] 1.4 Mover los 26 originales a `docs/historico/bd/`.
  - [ ] 1.5 Bitácora `docs/plan/2026-10-12_reorganizacion-bd.md`.

- [ ] 2. Estructura y lógica (BD)
  - [ ] 2.1 `bd/01_estructura.sql` — DDL de las 18 tablas (idempotente).
  - [ ] 2.2 `bd/02_procedimientos.sql` — todos los SP con su última definición vigente.
  - [ ] 2.3 `bd/03_triggers.sql` — los 3 triggers con `CREATE OR ALTER`/guardas idempotentes.
  - [ ] 2.4 `bd/README.md` — orden de aplicación, convención y mapa.
  - [ ] 2.5 `bd/05_utilidades_test.sql` — utilidad de desarrollo/test (purga de artefactos E2E).

- [ ] 3. Datos (seed)
  - [ ] 3.1 `bd/04_datos.sql` — roles, permisos, rol-permisos, taller, configuración de empresa y de respaldo, bootstrap Admin.
  - [ ] 3.2 Catálogo ampliado de `Marca`/`Modelo` representativo del mercado argentino (Toyota = 1, Renault = 2 preservados).

- [ ] 4. Documentación (housekeeping)
  - [ ] 4.1 `docs/DER.md` final (DER real; v1 archivada en `docs/historico/DER_v1.md`).
  - [ ] 4.2 `docs/ERS.md` final (ERS real; v1 archivada en `docs/historico/ERS_v1.md`).
  - [ ] 4.3 Fusionar `docs/referencia/backup-configuracion.md` en `docs/referencia/backup.md`.
  - [ ] 4.4 Actualizar `docs/referencia/bd-y-dominio.md` a los nombres canónicos.
  - [ ] 4.5 Recrear carpetas de trabajo `docs/plan/` y `docs/script/`; mover binarios a `docs/referencia/anexos/`.

- [ ] 5. Cierre
  - [ ] 5.1 Verificar idempotencia de los 4 scripts canónicos + la utilidad `05` (dry-run sobre `ControlVehicular`/copia de prueba).
  - [ ] 5.2 `dotnet build "TP_ControlVehicular.slnx"` sin errores.
  - [ ] 5.3 Actualizar `AGENTS.md` y `MEMORY.md` (mapa `bd/`, DER/ERS finales, histórico).
  - [ ] 5.4 Walkthrough `docs/plan/2026-10-12_reorganizacion-bd_walkthrough.md`.
  - [ ] 5.5 Commit `docs(bd): ...` en la rama feature.
  - [ ] 5.6 (Sujeto a confirmación) merge + archivar change.
