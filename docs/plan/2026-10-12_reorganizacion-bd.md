# Bitácora — Reorganización de `bd/` y compactación de `docs/`

- **Fecha:** 2026-10-12
- **Rama:** `feature/reorganizacion-bd`
- **Change OpenSpec:** `openspec/changes/2026-10-12-reorganizacion-bd/`
- **Tipo:** housekeeping / infraestructura de datos (no cambia el modelo de datos).

## 1. Objetivo

1. Compactar `bd/`: pasar de **26 scripts** con numeración rota y solapada a un **juego canónico por tipo** (DDL, SP, triggers, datos), más una utilidad de desarrollo/test y un `README.md`.
2. Ampliar el seed de `Marca`/`Modelo` con catálogo representativo del mercado argentino.
3. Compactar `docs/`: dejar solo las decisiones finales (DER y ERS finales; las versiones 1 archivadas) y mover material de trabajo a `docs/historico/`.

## 2. Diagnóstico

- **Sin migraciones EF Core**: el esquema físico de `ControlVehicular` vive exclusivamente en los scripts `bd/`.
- **Numeración rota**: existían dos `02_…` (`02_Datos.sql` y `02_StoredProcedures_Reportes.sql`).
- **Obsoletos**: `03_CreateTable_Facturas.sql` (tabla `Facturas` en plural, descartada).
- **Duplicados**: `BD_role_usuarios_script.sql` reproducía el bootstrap de roles/usuarios/permisos ya presente en otros scripts.
- **SP redefinidos varias veces** (`06_Migraciones_Manuales.sql`, `14_Fix_ReporteOperativo.sql`, `16_Fix_Colision_sp_ReporteIngresos.sql`): debía conservarse solo la **última versión vigente**.
- **Esquema real**: 18 tablas y 3 triggers.

## 3. Mapa de consolidación

| Archivo canónico | Contenido | Origen consolidado |
|---|---|---|
| `bd/01_estructura.sql` | DDL: 18 tablas, PK/FK, constraints, índices. | `01_Estructura.sql`, `04_schema_billing.sql`, `08_Migracion_PropietarioVehiculo.sql`, `19_ConfiguracionEmpresa.sql` (DDL), `21_ConfiguracionBackup.sql` (DDL) |
| `bd/02_procedimientos.sql` | Todos los SP (reportes por rol, comprobantes, workflow). | `02_StoredProcedures_Reportes.sql`, `sp_GenerarComprobanteOrden.sql`, `sp_GenerarComprobantePago.sql`, `05_WorkflowOrdenesServicio.sql`, `07…18` (SP) y redefiniciones de `06_Migraciones_Manuales.sql` |
| `bd/03_triggers.sql` | Los 3 triggers del dominio. | Triggers de `01_Estructura.sql` / `06_Migraciones_Manuales.sql` |
| `bd/04_datos.sql` | Seed: roles, permisos, rol-permisos, taller, configuraciones, Admin bootstrap, **marcas y modelos**. | `02_Datos.sql`, `09_PermisosAvanzados.sql`, `10…12`, `17`, `20`, `22`, `BD_role_usuarios_script.sql` + catálogo AR nuevo |
| `bd/05_utilidades_test.sql` | Utilidad de **desarrollo/test**: purga idempotente y FK-safe de artefactos E2E (patentes `TDD%`, clientes `EndToEnd`). Fuera del bootstrap productivo. | `15_Limpieza_Artefactos_TestE2E.sql` |
| `bd/README.md` | Orden de aplicación, convención de idempotencia y mapa. | Nuevo |

- **Descartado:** `03_CreateTable_Facturas.sql` (tabla `Facturas` plural obsoleta).
- **Orden de aplicación:** `01` → `04` (bootstrap productivo) y `05` solo en desarrollo/test.
- **Convención:** un objeto nuevo (tabla/SP/trigger/permiso) se agrega al script canónico de su **tipo**; no se crean scripts numerados nuevos.
- Los 26 originales se preservan en `docs/historico/bd/` (no se borran).
- **Seed `04_datos.sql`:** 45 marcas y 202 modelos representativos del mercado argentino (preserva Toyota = Id 1 y Renault = Id 2), 38 permisos, 3 roles y la matriz rol→permiso.

## 4. Compactación de `docs/`

| Antes | Después |
|---|---|
| `docs/DER.md` | `docs/DER.md` (final) + `docs/historico/DER_v1.md` |
| `docs/ERS.md` | `docs/ERS.md` (final) + `docs/historico/ERS_v1.md` |
| `docs/plan/*` (14) | `docs/historico/plan/*`; `docs/plan/` recreada como carpeta de trabajo |
| `docs/script/*` (65 `.py`) | `docs/historico/script/*`; `docs/script/` recreada como carpeta de trabajo |
| `docs/referencia/legacy/*` | `docs/historico/legacy/*` |
| Binarios de `docs/referencia/` | `docs/referencia/anexos/` |
| `docs/referencia/backup-configuracion.md` | Fusionado en `docs/referencia/backup.md` |

## 5. Documentos actualizados a los nombres canónicos

- `docs/referencia/backup.md` (fusionado), `docs/referencia/bd-y-dominio.md`, `docs/referencia/reportes.md`.
- `docs/manuales/manual_tecnico_administracion.md`, `docs/manuales/README.md`.
- `README.md` (§DB init), `AGENTS.md`, `MEMORY.md`.

## 6. Verificación

- [x] Confirmar que no existe la tabla `Facturas` (plural): `grep` sobre `bd/` solo encuentra la palabra en comentarios/etiquetas y en el alias `COUNT(f.Id) AS CantidadFacturas` de `02_procedimientos.sql`; ninguna tabla `Facturas` se crea.
- [x] `dotnet build "TP_ControlVehicular.slnx"` → **Compilación correcta** (solo warnings preexistentes `NU1510` de `System.Security.Cryptography.ProtectedData`).
- [x] `docs/DER.md`, `docs/ERS.md` y los 5 scripts canónicos sin mojibake (UTF-8 sin BOM, 0 hits).
- [x] Cruce `04_datos.sql` ↔ `01_estructura.sql`: listas de columnas, estados válidos según `CHECK` y claves únicas (`IX_Usuarios_Dni` / `IX_Usuarios_Email`) compatibles.
- [x] Nombres de permisos sembrados coinciden con el código (`Configuracion.Empresa.Gestionar`, `Configuracion.Backup.Gestionar`, `MisTrabajos.Ver`, `Reporte.{Ordenes,Operativo,Gerencial,Mecanico}.Ver`, `Usuario.Ver`, `Taller.Ver`, `Vehiculo.Ver`).
- [ ] **Idempotencia contra SQL Server** (`01` → `04` sobre base limpia + re-ejecución sin duplicados): pendiente de ejecución manual contra `<SERVIDOR>\<INSTANCIA>`; requiere la instancia real y no se automatizó en el entorno del agente. `05_utilidades_test.sql` no se ejecutó (utilidad destructiva, solo de desarrollo/test).

## 7. Pendiente / notas

- El `design.md` del change `2026-10-11-backup-drive` menciona `WITH INIT, COMPRESSION, CHECKSUM`; el código y `backup.md` indican **sin compresión** (SQL Server Express). Inconsistencia histórica, fuera del alcance de esta spec.
