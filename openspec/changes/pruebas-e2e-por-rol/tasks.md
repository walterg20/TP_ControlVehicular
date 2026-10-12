# Tasks: Pruebas End-to-End por Rol (Admin / Recepcionista / Mecánico)

## Task 1: Update test csproj to copy bd scripts
- [x] Edit `TP_ControlVehicular.Tests/TP_ControlVehicular.Tests.csproj` to add `None` entries for `bd/*.sql` with `CopyToOutputDirectory="PreserveNewest"`. Include comment noting fallback lookup.

## Task 2: Create EndToEnd/Infra infrastructure
- [x] `EndToEnd/Infra/SqlConnectionResolver.cs` — resolves test connection string (env `TPCV_TEST_CONNECTION`, else from `appsettings.json` with catalog replaced to `ControlVehicular_Test`). Uses `Microsoft.Data.SqlClient.SqlConnectionStringBuilder`.
- [x] `EndToEnd/Infra/SqlServerReachability.cs` — ping/connect check returning bool and optional exception.
- [x] `EndToEnd/Infra/RequiereSqlServerFactAttribute.cs` — custom `FactAttribute` that skips if not reachable (no new packages).
- [x] `EndToEnd/Infra/TestRunContext.cs` — holds run marker `TDD-{yyyyMMddHHmmss}` and resolved connection string.
- [x] `EndToEnd/Infra/DbProvisioner.cs` — creates DB if missing and applies `01_estructura.sql`, `02_procedimientos.sql`, `03_triggers.sql`, `04_datos.sql` in order, replacing `USE [ControlVehicular]` with test catalog and splitting by `GO`.
- [x] `EndToEnd/Infra/DbTestFixture.cs` — `ICollectionFixture<DbTestFixture>`; provisions once per run, sets `TestRunContext`.
- [x] `EndToEnd/Infra/DbTestCollection.cs` — `[CollectionDefinition("e2e-sql")]` with `ICollectionFixture<DbTestFixture>`.
- [x] `EndToEnd/Infra/ContextoTestFactory.cs` — creates `CVDbContext` configured for test connection (use same DI pattern as app or `new DbContextOptionsBuilder`).
- [x] `EndToEnd/Infra/SemillaE2E.cs` — helpers to seed run-tagged entities (clients, vehicles, talleres, users, registro servicios with tasks) and to obtain seeded IDs.
- [x] `EndToEnd/Infra/LimpiezaE2E.cs` — FK-safe cleanup of rows tagged with run marker.

## Task 3: Unit tests (L1) — HandlersUnitTests
- [ ] Create `Unit/HandlersUnitTests.cs` covering: `AutenticarUsuarioHandler` success and failures (`UsuarioNoEncontrado`, `UsuarioInactivo`, `ContrasenaIncorrecta`) with real AutoMapper and mocked repos.
- [ ] `EmpresaConfiguracionService` — returns default when repo returns null (unit level).
- [ ] Additional handler success/failure cases as needed (e.g. `ListarTareasPorEspecialistaHandler` filtering) using mocks.

## Task 4: End-to-end suites
- [ ] `EndToEnd/Cobertura/RbacPorRolE2ETests.cs` — verify Admin/Recepcionista/Mecánico capabilities via `AplicarRestriccionesPorRol` + reflection (align with existing RbacTests). Unknown user case.
- [ ] `EndToEnd/Rol/RecepcionistaFlujoE2ETests.cs` — registration assigns sequence/initial states, full happy path to payment (order becomes Completada before pago, Pagada after; invoice total sum), invoice idempotent, invoice total 0 when no tasks, receipts null for unknown/missing payment.
- [ ] `EndToEnd/Cobertura/FacturacionE2ETests.cs` — all billing rejections with exact messages (factura no encontrada, order not completed, already paid, invalid method, card wrong amount).
- [ ] `EndToEnd/Rol/MecanicoFlujoE2ETests.cs` — own pending list, advance enables next, last completes order, no-op if already finalizada, finalized leaves list, daily work-sheet returns rows and non-empty PDF, unauthorized if different mechanic, not found if missing.
- [ ] `EndToEnd/Rol/AdministradorFlujoE2ETests.cs` — taller register/modify/list; company config round-trip with logo; backup config round-trip; backup success/failure with doubles; resguardar requires credentials (double behavior).
- [ ] `EndToEnd/Cobertura/ReportesE2ETests.cs` — managerial reports return seeded rows for range with paid order; work-orders filtered by mechanic when current user is mechanic; empty collection when no data; receipts null cases covered as above.

## Task 5: Verify and finalize
- [ ] `dotnet build "TP_ControlVehicular.slnx"` — no errors (PowerShell `;`).
- [ ] `dotnet test "TP_ControlVehicular.slnx"` — unit tests run; DB-dependent tests skip if no SQL Server (no failures). If SQL Server available, E2E run with isolation.
- [ ] Create plan doc `docs/plan/2026-10-12_pruebas-e2e-por-rol.md` summarizing implementation and verification.
- [ ] Update `MEMORY.md` with new test infra conventions (run marker, skip attribute, script provisioning, bd copy).
- [ ] Commit with conventional commits, archive change in `openspec/changes/archive/`, merge feature branch to develop.

## Notes
- Use `[RequiereSqlServerFact]` for DB-dependent tests; unit tests use `[Fact]`.
- Never call real BACKUP/RESTORE or Drive upload — only doubles.
- Never modify appsettings.json; resolve connection at runtime.
- Keep changes in `TP_ControlVehicular.Tests/` only (no production code changes).