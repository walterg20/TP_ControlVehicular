# Tasks: Reorganización de Scripts de Base de Datos (`bd/`)

- [x] 1. Preparación
  - [x] 1.1 Rama `feature/reorganizacion-bd` creada.
  - [x] 1.2 Change OpenSpec (`proposal.md`, `spec.md`, `design.md`, `tasks.md`).
  - [x] 1.3 Diagnóstico y clasificación de los 26 `.sql` de `bd/` (estructura, SP, triggers, datos, obsoletos).
  - [x] 1.4 Mover los 26 originales a `docs/historico/bd/`.
  - [x] 1.5 Bitácora `docs/plan/2026-10-12_reorganizacion-bd.md`.

- [x] 2. Estructura y lógica (BD)
  - [x] 2.1 `bd/01_estructura.sql` — DDL de las 18 tablas (idempotente).
  - [x] 2.2 `bd/02_procedimientos.sql` — todos los SP con su última definición vigente.
  - [x] 2.3 `bd/03_triggers.sql` — los 3 triggers con `CREATE OR ALTER`/guardas idempotentes.
  - [x] 2.4 `bd/README.md` — orden de aplicación, convención y mapa.
  - [x] 2.5 `bd/05_utilidades_test.sql` — utilidad de desarrollo/test (purga de artefactos E2E).

- [x] 3. Datos (seed)
  - [x] 3.1 `bd/04_datos.sql` — roles, permisos, rol-permisos, taller, configuración de empresa y de respaldo, bootstrap Admin.
  - [x] 3.2 Catálogo ampliado de `Marca`/`Modelo` representativo del mercado argentino (45 marcas / 202 modelos; Toyota = 1, Renault = 2 preservados).

- [x] 4. Documentación (housekeeping)
  - [x] 4.1 `docs/DER.md` final (DER real; v1 archivada en `docs/historico/DER_v1.md`).
  - [x] 4.2 `docs/ERS.md` final (ERS real; v1 archivada en `docs/historico/ERS_v1.md`).
  - [x] 4.3 Fusionar `docs/referencia/backup-configuracion.md` en `docs/referencia/backup.md`.
  - [x] 4.4 Actualizar `docs/referencia/bd-y-dominio.md` a los nombres canónicos.
  - [x] 4.5 Recrear carpetas de trabajo `docs/plan/` y `docs/script/`; mover binarios a `docs/referencia/anexos/`.

- [ ] 5. Cierre
  - [ ] 5.1 Verificar idempotencia de los 4 scripts canónicos + la utilidad `05` (dry-run sobre `ControlVehicular`/copia de prueba). **Pendiente manual**: requiere una instancia SQL Server; la verificación estática (cruce de columnas, estados y claves) ya fue realizada.
  - [x] 5.2 `dotnet build "TP_ControlVehicular.slnx"` sin errores.
  - [x] 5.3 Actualizar `AGENTS.md` y `MEMORY.md` (mapa `bd/`, DER/ERS finales, histórico).
  - [x] 5.4 Bitácora `docs/plan/2026-10-12_reorganizacion-bd.md` (se optó por bitácora en lugar de un `_walkthrough`).
  - [x] 5.5 Commit `chore(bd): consolidar esquema en scripts canonicos por tipo y compactar docs` en la rama feature.
  - [x] 5.6 Merge `--no-ff` a `develop` y archivado del change.
