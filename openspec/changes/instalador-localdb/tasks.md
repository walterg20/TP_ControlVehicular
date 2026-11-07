# Tasks: Instalador con SQL Server LocalDB y configuración inicial

- [ ] Fase 0. Actualizar `docs/DER.md` y `docs/ERS.md` (roles SuperAdmin, first-run, restauración doble factor, RNF portabilidad).
- [ ] Fase 1. Limpiar dead code (`ProcesarPagoHandler`), crear `bd/00_bootstrap.sql` + `bd/instalar.ps1`, consolidar scripts.
- [ ] Fase 2. Rol SuperAdmin + permiso `Configuracion.Backup.Gestionar` (solo SuperAdmin) en seed.
- [ ] Fase 3. Modal de setup inicial (admin + empresa) + detección de BD vacía.
- [ ] Fase 4. Instalador Inno Setup (LocalDB + bootstrap + app + versionado).
- [ ] Fase 5. Validación en VM limpia.