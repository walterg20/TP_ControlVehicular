# Referencia — Base de datos y reglas de dominio

Scripts SQL, estados canónicos y artefactos de prueba. Se enlaza desde `AGENTS.md` → *Referencia por dominio*.

## Scripts de base de datos (`bd/`)
- `bd/` está **consolidado por tipo**: cuatro scripts canónicos de esquema/datos más una utilidad de desarrollo. Todos son **idempotentes** y se aplican en orden `01` → `05`:
  - `01_estructura.sql` — DDL: las 18 tablas, PK/FK, constraints e índices.
  - `02_procedimientos.sql` — todos los SP (reportes por rol, comprobantes, workflow).
  - `03_triggers.sql` — los tres triggers del dominio.
  - `04_datos.sql` — seed: roles, usuarios, permisos y rol-permisos (RBAC), taller, configuraciones, Admin bootstrap y catálogo de marcas/modelos.
  - `05_utilidades_test.sql` — utilidad de desarrollo/test (purga de artefactos E2E); no es parte del bootstrap productivo.
- Un objeto nuevo se agrega al script canónico de su **tipo** (tabla → `01_estructura.sql`, SP → `02_procedimientos.sql`, trigger → `03_triggers.sql`, permiso/dato → `04_datos.sql`); no se crean scripts numerados nuevos.
- Nombres de SP: cada variante con consumidor propio usa un nombre distinto (p. ej. `sp_ReporteIngresos` vs. `sp_ReporteIngresosAdmin`, ambos en `02_procedimientos.sql`).
- Migraciones EF Core viven en `Migracion/` (no en `bd/`).
- **Respaldo de BD**: tabla singleton `dbo.ConfiguracionBackup` (DDL en `01_estructura.sql`; fila semilla y permiso `Configuracion.Backup.Gestionar` → Admin `IdRol = 1`, en `04_datos.sql`). Detalle en `docs/referencia/backup.md`.
- Los 26 scripts originales (numeración histórica `01_…`–`22_…`) quedan archivados en `docs/historico/bd/`.

## Estados canónicos
- **Cancelación de órdenes**: usar la constante `Entidad.RegistroServicio.EstadoCancelada` (`"Cancelada"`); nunca el literal `"Cancelado"` (bug: no coincidía con el estado persistido). `DetalleServicio` no tiene estado `Cancelada`.
- `RegistroServicio.Estado` ∈ {`Abierta`, `En Proceso`, `Completada`, `Pagada`, `Cancelada`}; `DetalleServicio.Estado` ∈ {`Pendiente`, `En Curso`, `Finalizada`}.

## Artefactos del test E2E
- Las patentes `TDD%` y los clientes `EndToEnd` son datos de prueba, no reales.
- `bd/05_utilidades_test.sql` los purga de forma idempotente y FK-safe (orden: `Pago -> Factura -> DetalleServicio -> RegistroServicio -> PropietarioVehiculo -> Vehiculo -> Cliente`); deshabilita y rehabilita el trigger `TR_DetalleServicio_BloquearModificacionFinalizada`.

## Código muerto
- No usar `ProcesarPagoHandler` (`HandleAsync` lanza `NotImplementedException`); el cobro real se realiza con `IBillingService.RegistrarPago`.
