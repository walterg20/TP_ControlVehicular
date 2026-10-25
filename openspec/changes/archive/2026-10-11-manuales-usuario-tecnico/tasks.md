# Tasks: Manuales de Usuario y Técnico

- [x] 1. Preparación
  - [x] 1.1 Rama `feature/manuales-usuario-tecnico` creada.
  - [x] 1.2 Change OpenSpec (`proposal.md`, `design.md`, `spec.md`, `tasks.md`).
  - [x] 1.3 Scaffolding `docs/manuales/` + `assets/roles/` + `assets/tecnico/` (`.gitkeep`).
  - [x] 1.4 Bitácora `docs/plan/2026-10-11_manuales-usuario-tecnico.md`.

- [x] 2. Orquestación (arquitecto_manual)
  - [x] 2.1 Relevar módulos/pantallas reales (32 XAML) y matriz RBAC real.
  - [x] 2.2 Redactar `docs/manuales/README.md` (portal, arquitectura, matriz perfiles × módulos).
  - [x] 2.3 Handoff a redactores con correcciones de dominio (`CtlTaller` vs `CtlConfiguracion`; huérfanas).

- [x] 3. Manual de Usuario Operativo (`redactor_roles`)
  - [x] 3.1 Portada, TOC, Capítulo 0 (login + interfaz + convenciones).
  - [x] 3.2 Capítulo Administrador 🛡️ (dashboard, empresa, catálogos, usuarios/roles, reportes).
  - [x] 3.3 Capítulo Recepcionista 📋 (dashboard, clientes, vehículos, orden `+ Rápido`, cobro).
  - [x] 3.4 Capítulo Mecánico 🔧 (dashboard, *Mis Trabajos* maestro-detalle, edición inline).
  - [x] 3.5 Capítulo dudas/errores frecuentes + tabla de estados de la orden.

- [x] 4. Manual Técnico (`redactor_tecnico`)
  - [x] 4.1 Requisitos y topología cliente-servidor.
  - [x] 4.2 Conexión (`appsettings.json`), TCP/1433, firewall y despliegue de scripts `bd/01`–`22`.
  - [x] 4.3 Respaldos (`CtlBackup`/`CtlBackupConfiguracion`), rutas duales, prueba, Google Drive (credenciales.json + DPAPI).
  - [x] 4.4 Disaster Recovery, permisos NTFS y RBAC.
  - [x] 4.5 Troubleshooting y consulta de logs.

- [x] 5. Cierre
  - [x] 5.1 Auditoría de cobertura (14 módulos) y de marcadores/rutas.
  - [x] 5.2 `dotnet build "TP_ControlVehicular.slnx"` sin errores.
  - [x] 5.3 Actualizar `MEMORY.md`.
  - [x] 5.4 Commit `docs(manuales): ...` en la rama feature.
  - [x] 5.5 (Sujeto a confirmación) merge + archivar change.
