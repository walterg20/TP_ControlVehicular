# Referencia — Base de datos y reglas de dominio

Scripts SQL, estados canónicos y artefactos de prueba. Se enlaza desde `AGENTS.md` → *Referencia por dominio*.

## Scripts de base de datos (`bd/`)
- Todos los scripts son **idempotentes** y se numeran en orden de aplicación (`01_…`, `02_…`, …, `22_…`).
- Un permiso o SP nuevo = script nuevo; no editar scripts ya aplicados.
- Nombres de SP: cada variante con consumidor propio usa un nombre distinto (ver `bd/16_Fix_Colision_sp_ReporteIngresos.sql`).
- Migraciones EF Core viven en `Migracion/` (no en `bd/`).
- **Respaldo de BD**: `bd/21_ConfiguracionBackup.sql` (tabla singleton `dbo.ConfiguracionBackup`, `Id = 1`) y `bd/22_PermisosBackup.sql` (permiso `Configuracion.Backup.Gestionar` → Admin `IdRol = 1`). Detalle en `docs/referencia/backup.md`.

## Estados canónicos
- **Cancelación de órdenes**: usar la constante `Entidad.RegistroServicio.EstadoCancelada` (`"Cancelada"`); nunca el literal `"Cancelado"` (bug: no coincidía con el estado persistido). `DetalleServicio` no tiene estado `Cancelada`.
- `RegistroServicio.Estado` ∈ {`Abierta`, `En Proceso`, `Completada`, `Pagada`, `Cancelada`}; `DetalleServicio.Estado` ∈ {`Pendiente`, `En Curso`, `Finalizada`}.

## Artefactos del test E2E
- Las patentes `TDD%` y los clientes `EndToEnd` son datos de prueba, no reales.
- `bd/15_Limpieza_Artefactos_TestE2E.sql` los purga de forma idempotente y FK-safe (orden: `Pago -> Factura -> DetalleServicio -> RegistroServicio -> PropietarioVehiculo -> Vehiculo -> Cliente`); deshabilita y rehabilita el trigger `TR_DetalleServicio_BloquearModificacionFinalizada`.

## Código muerto
- No usar `ProcesarPagoHandler` (`HandleAsync` lanza `NotImplementedException`); el cobro real se realiza con `IBillingService.RegistrarPago`.
