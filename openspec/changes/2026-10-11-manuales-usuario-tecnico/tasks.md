# Tasks: Manuales de Usuario y Técnico

- [ ] 1. Preparación
  - [ ] 1.1 Rama `feature/manuales-usuario-tecnico` creada.
  - [ ] 1.2 Change OpenSpec (`proposal.md`, `design.md`, `spec.md`, `tasks.md`).
  - [ ] 1.3 Scaffolding `docs/manuales/` + `assets/roles/` + `assets/tecnico/` (`.gitkeep`).
  - [ ] 1.4 Bitácora `docs/plan/2026-10-11_manuales-usuario-tecnico.md`.

- [ ] 2. Orquestación (arquitecto_manual)
  - [ ] 2.1 Relevar módulos/pantallas reales (32 XAML) y matriz RBAC real.
  - [ ] 2.2 Redactar `docs/manuales/README.md` (portal, arquitectura, matriz perfiles × módulos).
  - [ ] 2.3 Handoff a redactores con correcciones de dominio (`CtlTaller` vs `CtlConfiguracion`; huérfanas).

- [ ] 3. Manual de Usuario Operativo (`redactor_roles`)
  - [ ] 3.1 Portada, TOC, Capítulo 0 (login + interfaz + convenciones).
  - [ ] 3.2 Capítulo Administrador 🛡️ (dashboard, empresa, catálogos, usuarios/roles, reportes).
  - [ ] 3.3 Capítulo Recepcionista 📋 (dashboard, clientes, vehículos, orden `+ Rápido`, cobro).
  - [ ] 3.4 Capítulo Mecánico 🔧 (dashboard, *Mis Trabajos* maestro-detalle, edición inline).
  - [ ] 3.5 Capítulo dudas/errores frecuentes + tabla de estados de la orden.

- [ ] 4. Manual Técnico (`redactor_tecnico`)
  - [ ] 4.1 Requisitos y topología cliente-servidor.
  - [ ] 4.2 Conexión (`appsettings.json`), TCP/1433, firewall y despliegue de scripts `bd/01`–`22`.
  - [ ] 4.3 Respaldos (`CtlBackup`/`CtlBackupConfiguracion`), rutas duales, prueba, Google Drive (credenciales.json + DPAPI).
  - [ ] 4.4 Disaster Recovery, permisos NTFS y RBAC.
  - [ ] 4.5 Troubleshooting y consulta de logs.

- [ ] 5. Cierre
  - [ ] 5.1 Auditoría de cobertura (14 módulos) y de marcadores/rutas.
  - [ ] 5.2 `dotnet build "TP_ControlVehicular.slnx"` sin errores.
  - [ ] 5.3 Actualizar `MEMORY.md`.
  - [ ] 5.4 Commit `docs(manuales): ...` en la rama feature.
  - [ ] 5.5 (Sujeto a confirmación) merge + archivar change.
