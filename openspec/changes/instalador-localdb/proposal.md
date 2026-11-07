# Proposal: Instalador con SQL Server LocalDB y configuración inicial

## Why
El sistema requiere SQL Server instalado manualmente y configuración de connection string. Para distribuirlo a usuarios finales se necesita un instalador que provisione la base de datos (LocalDB), ejecute el seed base y guíe la configuración inicial (admin + empresa), con un rol SuperAdmin para validar restauraciones.

## What Changes
- Fase 0: actualizar DER.md y ERS.md (roles SuperAdmin, first-run, restauración doble factor).
- Fase 1: limpieza (eliminar ProcesarPagoHandler), bootstrap SQL (`bd/00_bootstrap.sql`), script de instalación.
- Fase 2: rol SuperAdmin + permiso `Configuracion.Backup.Gestionar` (solo SuperAdmin).
- Fase 3: modal de setup inicial (admin + empresa) cuando la BD está vacía.
- Fase 4: instalador (Inno Setup/WiX) con LocalDB + bootstrap + app + versionado de esquema.
- Fase 5: validación en VM limpia.

## Capabilities
### New Capabilities
- `instalacion-localdb`: instalador con LocalDB, seed base y first-run.

## Impact
- Documentación: `docs/DER.md`, `docs/ERS.md`, `docs/referencia/bd-y-dominio.md`.
- BD: `bd/00_bootstrap.sql` (nuevo), seed base en `02_datos.sql`.
- Código: rol SuperAdmin, modal setup, versionado de esquema, eliminación de dead code.