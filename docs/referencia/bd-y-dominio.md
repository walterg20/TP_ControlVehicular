# Referencia — Base de datos y reglas de dominio

Scripts SQL, estados canónicos y artefactos de prueba. Se enlaza desde `AGENTS.md` → *Referencia por dominio*.

## Scripts de base de datos (`bd/`)
- `bd/` está **consolidado por tipo**: cinco scripts canónicos del sistema más una utilidad de pruebas y un script de población demo. Todos son **idempotentes** y se aplican en orden `01` → `04` (o `06` para entorno demo):
  - `01_estructura.sql` — DDL: secuencia, las 18 tablas, PK/FK, constraints e índices con guardas defensivas.
  - `02_datos.sql` — seed canónico limpio: roles, permisos (RBAC), matriz RolPermisos, taller, configuraciones singleton, Admin bootstrap (`admin@example.com`), 15 servicios y catálogo de marcas/modelos.
  - `03_procedimientos.sql` — los 22 SP oficiales (reportes por rol, comprobantes, workflow y consultas paginadas en BD).
  - `04_triggers.sql` — los tres triggers del dominio (precios/cantidades, odómetro, bloqueo de factura).
  - `05_utilidades_test.sql` — utilidad de desarrollo/test (purga de artefactos E2E); no es parte del bootstrap productivo.
  - `06_poblacion.sql` — población opcional de datos operativos y de prueba (recepcionista y mecánicos, 55 clientes, 55 vehículos, 61 órdenes, detalles, facturas y pagos).
- Un objeto nuevo se agrega al script canónico de su **tipo** (tabla → `01_estructura.sql`, permiso/catalogo base → `02_datos.sql`, SP → `03_procedimientos.sql`, trigger → `04_triggers.sql`); no se crean scripts numerados nuevos.
- Nombres de SP: cada variante con consumidor propio usa un nombre distinto (p. ej. `sp_ReporteIngresos` vs. `sp_ReporteIngresosAdmin`, ambos en `03_procedimientos.sql`).
- Migraciones EF Core viven en `Migracion/` (no en `bd/`).
- **Respaldo de BD**: tabla singleton `dbo.ConfiguracionBackup` (DDL en `01_estructura.sql`; fila semilla y permiso `Configuracion.Backup.Gestionar` → Admin `IdRol = 1`, en `02_datos.sql`). Detalle en `docs/referencia/backup.md`.
- Los 26 scripts originales (numeración histórica `01_…`–`22_…`) quedan archivados en `docs/historico/bd/`.

## Estados canónicos
- **Cancelación de órdenes**: usar la constante `Entidad.RegistroServicio.EstadoCancelada` (`"Cancelada"`); nunca el literal `"Cancelado"` (bug: no coincidía con el estado persistido). `DetalleServicio` no tiene estado `Cancelada`.
- `RegistroServicio.Estado` ∈ {`Abierta`, `En Proceso`, `Completada`, `Pagada`, `Cancelada`}; `DetalleServicio.Estado` ∈ {`Pendiente`, `En Curso`, `Finalizada`}.

## Artefactos del test E2E
- Las patentes `TDD%` y los clientes `EndToEnd` son datos de prueba, no reales.
- `bd/05_utilidades_test.sql` los purga de forma idempotente y FK-safe (orden: `Pago -> Factura -> DetalleServicio -> RegistroServicio -> PropietarioVehiculo -> Vehiculo -> Cliente`); deshabilita y rehabilita el trigger `TR_DetalleServicio_BloquearModificacionFinalizada`.

## Código muerto
- No usar `ProcesarPagoHandler` (`HandleAsync` lanza `NotImplementedException`); el cobro real se realiza con `IBillingService.RegistrarPago`.
