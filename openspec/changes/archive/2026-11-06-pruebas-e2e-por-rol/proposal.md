# Proposal: Pruebas End-to-End por Rol (Admin / Recepcionista / Mecánico)

## Why
Hoy la cobertura automatizada se limita a pruebas unitarias de ViewModels y RBAC
(`TP_ControlVehicular.Tests/ViewModels/`): validan Login, ABM de usuarios y gating de
botones, pero **ningún test recorre el flujo real de trabajo de la aplicación**, que es
donde viven las reglas de negocio críticas:

- Recepcionista: abrir una orden, agregar servicios, asignar mecánico, cobrar y emitir
  comprobantes.
- Mecánico: avanzar sus tareas e imprimir su reporte operativo.
- Administrador: configurar taller/empresa, ver el reporte gerencial y gestionar respaldos.

Esas reglas dependen de SQL Server real (SPs, triggers, secuencia de facturación, EF Core
`CVDbContext`) y **no se pueden cubrir con dobles** sin perder fidelidad. El resultado es
que una regresión en el flujo de cobro, en la transición de estados de las tareas o en los
reportes puede llegar a `develop` sin ser detectada.

Esta entrega incorpora una **suite end-to-end por rol** que corre con `dotnet test`, ejercita
los flujos de éxito **y** de fracaso de cada perfil contra una **base de datos dedicada de
pruebas**, y se **salta automáticamente** cuando no hay SQL Server disponible (para que el
build/CI no se rompa en máquinas sin motor).

## What Changes
- **Nueva capacidad `pruebas-e2e`** con su spec en inglés (escenarios BDD por rol, éxito y fracaso).
- **Infraestructura de pruebas de integración** en `TP_ControlVehicular.Tests/EndToEnd/Infra/`:
  - Resolución de cadena de conexión de pruebas (env `TPCV_TEST_CONNECTION`, si no, `appsettings.json`
    con catálogo `ControlVehicular_Test`).
  - *Provisionamiento* idempotente de la base de pruebas: crea la BD si falta y aplica los scripts
    canónicos `bd/01_estructura.sql` → `02_procedimientos.sql` → `03_triggers.sql` → `04_datos.sql`
    (reescribiendo `USE [ControlVehicular]` al catálogo de pruebas), porque `Migrate()` de EF Core **no**
    incluye SPs ni triggers que los reportes necesitan.
  - Atributo propio `[RequiereSqlServerFact]` (subclase de `FactAttribute`) que marca el test como
    *skipped* con motivo cuando el servidor no responde. **Sin dependencias NuGet nuevas.**
  - Fixture de colección (`ICollectionFixture`) que provisiona **una sola vez** por corrida.
  - Semilla marcada (`TDD-{yyyyMMddHHmmss}`) y limpieza acotada (borra únicamente filas con esa marca,
    en orden seguro de FKs), reutilizando la convención de purga ya documentada en `bd/05_utilidades_test.sql`.
- **Suites E2E por rol** (`RecepcionistaFlujoE2ETests`, `MecanicoFlujoE2ETests`, `AdministradorFlujoE2ETests`,
  `FacturacionE2ETests`, `ReportesE2ETests`, `RbacPorRolE2ETests`), cada una con casos de éxito y de fracaso.
- **Pruebas unitarias L1 adicionales** (`Unit/HandlersUnitTests.cs`) para los handlers que sólo dependen de
  interfaces de repositorio (mapeo real + dobles), cubriendo éxito y fracaso.
- **Comprobantes y reportes en PDF reales** (QuestPDF) verificados como bytes no vacíos, sin abrir procesos
  ni mostrar diálogos.
- El `.csproj` del proyecto de tests copia `bd/*.sql` a la salida (con *fallback* de búsqueda hacia arriba
  del árbol) — sin paquetes nuevos.

## Impact
- **Capacidad nueva**: `pruebas-e2e`.
- **Código afectado**: sólo `TP_ControlVehicular.Tests/` (nuevos archivos bajo `EndToEnd/` y `Unit/`) y su
  `TP_ControlVehicular.Tests.csproj`. **No se modifica código de producción.**
- **Base de datos**: se crea/usa `ControlVehicular_Test` (o el catálogo indicado por `TPCV_TEST_CONNECTION`).
  **No se toca `ControlVehicular`** (datos reales).
- **Dependencias nuevas**: ninguna.

## Non-goals (fuera de alcance)
- **Ejecución real de `BACKUP`/`RESTORE`** ni subida real a Google Drive: la orquestación de respaldos del
  Administrador se verifica con dobles controlados (las operaciones reales son destructivas y dependen de
  permisos del servicio SQL). Se documenta el límite.
- **Interacción de UI**: los tests no abren ventanas ni ejecutan los comandos que llaman
  `Process.Start`/`MessageBox`. El gating de UI se cubre con los patrones STA + reflexión ya existentes.
- **Pruebas de carga/estrés**, *snapshot testing* de PDF y cobertura de porcentaje mínima.
- **Automatización de creación de la BD en CI**: el skip automático es la estrategia ante ausencia de motor.
