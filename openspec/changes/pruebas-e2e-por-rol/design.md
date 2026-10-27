# Design: Pruebas End-to-End por Rol (Admin / Recepcionista / Mecánico)

## Overview
This change adds a full end-to-end test suite exercising real business flows per role against a dedicated SQL Server test database. Tests run under `dotnet test`, automatically skip when SQL Server is unavailable (no new NuGet dependencies), and enforce strict run-scoped isolation by tagging seeded data with a unique marker and deleting only that marker on teardown.

## Architecture

### Test project structure
New directories under `TP_ControlVehicular.Tests/`:
- `EndToEnd/Infra/` — infrastructure: connection resolution, provisioning, database factory, isolation utilities, seed/cleanup helpers, and the skip attribute/collection fixture.
- `EndToEnd/Rol/` — suites per role: `RecepcionistaFlujoE2ETests`, `MecanicoFlujoE2ETests`, `AdministradorFlujoE2ETests`.
- `EndToEnd/Cobertura/` — cross-cutting: `FacturacionE2ETests`, `ReportesE2ETests`, `RbacPorRolE2ETests`.
- `Unit/` — `HandlersUnitTests.cs` for unit tests (mocked repos, real AutoMapper).

### Connection resolution
- Read `TPCV_TEST_CONNECTION` from environment; if present use it.
- Else read `ConnectionStrings:DefaultConnection` from `appsettings.json` (using the same base directory as App: `AppContext.BaseDirectory`) and replace the database catalog with `ControlVehicular_Test`, building a new connection string via `Microsoft.Data.SqlClient.SqlConnectionStringBuilder` (to preserve other parameters).
- No modification of `appsettings.json`.

### Provisioning (idempotent, script-based)
Scripts `bd/01_estructura.sql`, `bd/02_procedimientos.sql`, `bd/03_triggers.sql`, `bd/04_datos.sql` are copied to test output (csproj includes `bd/*.sql` with `CopyToOutputDirectory=PreserveNewest` and fallback to locate `bd/` upward from output). Provisioning:
1. Ensure database `ControlVehicular_Test` exists (create if missing).
2. For each script: read text, replace `USE [ControlVehicular]` with `USE [ControlVehicular_Test]` (case-insensitive), split by `GO` (batch separators), execute each batch on the target database (trusted connection or as provided).
3. Apply in order once per test run (fixture). Scripts include stored procedures, triggers and catalog seed data required by reports; we do not rely solely on EF Core migrations.
Note: `ControlVehicular_Test` is also the restore target for `IDatabaseBackupService.ProbarBackupAsync` — real restore is never exercised in these tests.

### Automatic skip when SQL Server unavailable
Custom attribute `RequiereSqlServerFactAttribute` derives from `FactAttribute`. On discovery/execution it attempts a short `Open`/`Ping` to the resolved test connection (or a lightweight connectivity check). If unreachable, the test is reported as skipped with a clear message and no failure. Implementation uses built-in ADO.NET (no new NuGet packages).

### Isolation
- Run marker: `TDD-{DateTime.Now:yyyyMMddHHmmss}` (same convention as documented). Stored on a test-scoped context.
- Seed helpers create entities with `Observaciones` or identifiable fields containing the marker (or add a marker tag in a controlled field). Cleanup deletes only rows carrying that marker in FK-safe order (children before parents: DetalleServicio → RegistroServicio, Pagos → Factura, PropietarioVehiculo → Vehiculo/Cliente, etc.). Unrelated rows remain intact.
- `ICollectionFixture` + `CollectionDefinition("e2e-sql")` ensures provisioning runs once per test run and marker is shared.

### Test doubles
- Backup orchestration (Administrador): use Moq for `IDatabaseBackupService`, `IDriveBackupService`, `IBackupConfiguracionService`. Never call real `BACKUP`/`RESTORE` or upload to Drive. Verify delegation and error surfacing.
- Unit tests (L1): use Moq for repositories, use real `AutoMapper` with `MappingProfile` (from `Negocio.Mappers`).

### PDF/report verification
- QuestPDF receipts generated via `QuestPdfReporteService` or via report composers: verify returned `byte[]` length > 0 (non-empty). Do not open `Process.Start` or UI dialogs in tests. For "gerencial/operativo" PDF generation paths that build documents inline (ViewModels), tests verify the PDF bytes produced by the service/extension helpers, not the ViewModel command that launches a process.

### RBAC verification
Leverage `MainWindow.AplicarRestriccionesPorRol()` and `UserSession.CurrentUser` with reflection as in existing `RbacTests`, asserting visibility against permission sets (`Usuario.Ver`, `Taller.Ver`, `Vehiculo.Ver`, `Configuracion.Empresa.Gestionar`, `Configuracion.Backup.Gestionar`, `Cliente.Ver`, `Servicio.Ver`, `OrdenServicio.Ver`, `MisTrabajos.Ver`, `Reporte.Ordenes.Ver`, `Reporte.Operativo.Ver`, `Reporte.Gerencial.Ver`).

### Csproj changes
`TP_ControlVehicular.Tests/TP_ControlVehicular.Tests.csproj` adds:
- Include `bd/*.sql` as `None` with `CopyToOutputDirectory="PreserveNewest"`. Fallback search upward from `AppContext.BaseDirectory` to locate `bd/` if not found in output.

### Contracts mapping (to match reality)
- `GenerarFactura`: idempotent; total = sum of details (0 if none). No validation that order exists (FK enforced).
- `AvanzarEstadoTarea`: marks given task `Finalizada`; enables next pending as `En Curso`; if none pending, order becomes `Completada`. If already finalizada → no-op. Throws `UnauthorizedAccessException` if different mechanic; throws `"Tarea no encontrada."` if missing.
- `AutenticarUsuario`: returns `AuthResponseDto` with `ResultadoAutenticacion` enum values.
- `RegistrarRegistroServicio`: sets correlative `OrdenEjecucion`, first `En Curso`, rest `Pendiente`, order `En Proceso`.
- `BillingService.RegistrarPago`: rejects with exact messages (Factura no encontrada, orden no completada, ya pagada, método inválido, tarjeta monto distinto).

## Data model notes
Seed data from `bd/04_datos.sql` includes roles, permissions, methods of payment (Efectivo, Tarjeta), ConfiguracionEmpresa Id 1, workshop `Taller PRO`. Test seeds add run-marker-tagged rows only.