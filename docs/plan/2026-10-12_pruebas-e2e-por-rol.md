# Plan: Pruebas End-to-End por Rol (Admin / Recepcionista / Mecánico)

Fecha: 2026-10-12

## Objetivo
Implementar una suite end-to-end por rol que ejercite los flujos reales (Recepcionista, Mecánico, Administrador), cubriendo éxito y fracaso, contra una base de datos de pruebas dedicada (`ControlVehicular_Test`). Los tests se saltan automáticamente cuando no hay SQL Server disponible.

## Decisiones de diseño (alineadas con la spec corregida)
- **Estrategia híbrida**: L1 (unitarias con Moq + AutoMapper real) + E2E (integración real contra SQL Server).
- **BD dedicada**: `ControlVehicular_Test`. Cadena de conexión: env `TPCV_TEST_CONNECTION`, si no, `appsettings.json` con catálogo reemplazado. No se modifica `appsettings.json`.
- **Provisionamiento script-based**: aplica `bd/01_estructura.sql`, `02_procedimientos.sql`, `03_triggers.sql`, `04_datos.sql` en orden (reescribe `USE [ControlVehicular]` a `ControlVehicular_Test`, split por `GO`). Incluye SPs/triggers (no solo migraciones EF).
- **Skip automático**: atributo propio `[RequiereSqlServerFact]` (sin nuevos paquetes NuGet). Si no hay servidor, los tests DB-dependent se marcan skipped y el run no falla.
- **Aislamiento por corrida**: marcador `TDD-{yyyyMMddHHmmss}`; limpieza FK-safe solo de filas con ese marcador. Una colección por corrida.
- **Sin nuevos paquetes NuGet**: se usa ADO.NET + xUnit + Moq existentes.
- **Solo en Tests**: todos los cambios en `TP_ControlVehicular.Tests/`; no se toca código de producción.
- **Backups/Drive**: solo dobles (Moq). No se ejecutan `BACKUP`/`RESTORE` reales ni subida a Drive.
- **PDFs**: se verifica que los bytes generados (QuestPDF) no estén vacíos; no se abren procesos ni diálogos.

## Implementación

### 1. csproj
Agregar a `TP_ControlVehicular.Tests/TP_ControlVehicular.Tests.csproj` la copia de `bd/*.sql` a `OutputDirectory` (con nota de fallback). Esto permite provisionar desde los scripts.

### 2. Infraestructura EndToEnd/Infra
Archivos a crear:
- `SqlConnectionResolver.cs`: resuelve conexión de test (env + fallback appsettings + reemplazo de catálogo).
- `SqlServerReachability.cs`: chequeo de conectividad.
- `RequiereSqlServerFactAttribute.cs`: atributo de skip personalizado.
- `TestRunContext.cs`: marcador y cadena.
- `DbProvisioner.cs`: crea BD y aplica scripts (replace USE, split GO).
- `DbTestFixture.cs`: provisiona una vez por corrida, inicializa contexto.
- `DbTestCollection.cs`: colección `[CollectionDefinition("e2e-sql")]`.
- `ContextoTestFactory.cs`: fabrica `CVDbContext` para test.
- `SemillaE2E.cs`: siembra con marcador y devuelve IDs.
- `LimpiezaE2E.cs`: limpieza FK-safe por marcador.

### 3. Unit tests (L1)
`Unit/HandlersUnitTests.cs`: `AutenticarUsuarioHandler` (éxito + 3 fallos), `EmpresaConfiguracionService.ObtenerAsync` (default si null), casos de filtrado con mocks + AutoMapper real.

### 4. Suites E2E
- `RbacPorRolE2ETests.cs`: verifica capacidades Admin/Recepcionista/Mecánico + sin usuario (usa patrón STA/reflection como RbacTests existente).
- `RecepcionistaFlujoE2ETests.cs`: registro con secuencia/estados, flujo completo hasta pago (Completada→Pagada, factura suma, PDFs no vacíos), idempotencia de factura, total 0 sin tareas, comprobantes null para inexistente/sin pago.
- `FacturacionE2ETests.cs`: todos los rechazos con mensajes exactos (BillingService).
- `MecanicoFlujoE2ETests.cs`: listado propio, avance habilita siguiente, último completa orden, no-op si ya finalizada, finalizada sale de listado, hoja diaria con filas + PDF, no autorizado, no encontrada.
- `AdministradorFlujoE2ETests.cs`: Taller CRUD (register/modify/list), EmpresaConfig round-trip con logo, BackupConfig round-trip, backup éxito/fracaso con dobles, resguardar requiere credenciales (dobles).
- `ReportesE2ETests.cs`: reportes gerenciales con filas para rango con orden pagada, filtro mecánico en reporte de órdenes, colección vacía sin datos.

## Verificación
- `dotnet build "TP_ControlVehicular.slnx"` (PowerShell `;`).
- `dotnet test "TP_ControlVehicular.slnx"`: unitarias pasan; DB-dependent se skipean si no hay SQL Server (sin fallo). Con SQL Server, E2E corren con aislamiento.

## Finalización
- Crear `docs/plan/2026-10-12_pruebas-e2e-por-rol.md`.
- Actualizar `MEMORY.md` con convenciones (marcador, skip, provisioning scripts, copia bd).
- Commits convencionales, archivar change en `openspec/changes/archive/`, merge a develop.