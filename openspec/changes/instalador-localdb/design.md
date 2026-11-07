# Design: Instalador con SQL Server LocalDB y configuración inicial

## Decisions
- Motor: SQL Server Express LocalDB (prerequisito del instalador), reutiliza SPs/triggers actuales. No se embebe SQL Server completo ni se migra a SQLite.
- Contraseñas: se mantienen sin hash (decisión actual); migración a hash queda para una spec futura.
- Seed: solo base (`02_datos.sql`: roles, permisos, matriz, taller, configs, admin, 15 servicios, 45 marcas, 203 modelos). `06_poblacion.sql` (demo) NO se incluye en el instalador.
- Instalador: Inno Setup (ejecuta scripts SQL con elevación, instala LocalDB si falta).
- First-run: la app detecta BD sin usuarios → modal `FrmSetupInicial` (admin + empresa). Bloquea el uso hasta completar.
- SuperAdmin: rol con permiso exclusivo `Configuracion.Backup.Gestionar`; el admin operativo no lo tiene.
- Versionado de esquema: tabla `SchemaVersion` para aplicar deltas en updates.

## Risks
- Elevación: ejecutar scripts SQL requiere admin; el instalador debe correr con privilegios.
- LocalDB no es un servicio persistente: la app debe verificar/crear la instancia al iniciar.
- Migración de datos existentes (hash) se difiere.