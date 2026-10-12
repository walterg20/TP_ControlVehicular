# Proposal: Reorganización de Scripts de Base de Datos (`bd/`)

## Why
La carpeta `bd/` acumula **26 scripts SQL** aplicados en orden numérico, pero el orden está roto (existen **dos** `02_…`: `02_Datos.sql` y `02_StoredProcedures_Reportes.sql`), hay scripts **obsoletos** (p. ej. `03_CreateTable_Facturas.sql`, que crea una tabla `Facturas` en plural ya descartada) y **duplicados** (`BD_role_usuarios_script.sql` reproduce el bootstrap de roles/usuarios/permisos que ya vive en otros scripts). No existe una fuente canónica única del esquema: para reconstruir la base hay que adivinar el orden y descartar a mano lo obsoleto, lo que hace frágil el despliegue y la verificación.

Además, **no hay migraciones EF Core**: el esquema real vive exclusivamente en estos scripts. Por lo tanto, su consolidación es la única forma de tener un esquema reproducible.

Por último, la población de datos de catálogo (`Marca`/`Modelo`) es incompleta: no refleja las marcas y modelos que realmente circulan en Argentina.

## What Changes
- Consolidar los 26 scripts en **4 scripts canónicos idempotentes** dentro de `bd/`, aplicables en orden:
  - `01_estructura.sql` — DDL completo (18 tablas, constraints, índices).
  - `02_procedimientos.sql` — todos los procedimientos almacenados.
  - `03_triggers.sql` — los 3 triggers del dominio.
  - `04_datos.sql` — población de datos (roles, permisos, rol-permisos, taller, configuración de empresa, configuración de respaldo, bootstrap de usuario Admin) **y** catálogo ampliado de marcas/modelos representativos del mercado argentino.
- Conservar la utilidad de **desarrollo/test** `05_utilidades_test.sql` (purga idempotente y FK-safe de artefactos E2E), fuera del bootstrap productivo.
- Agregar `bd/README.md` con orden de aplicación, convención de idempotencia y mapa de qué contiene cada script.
- Descartar `03_CreateTable_Facturas.sql` (tabla `Facturas` plural) y plegar el seed de `BD_role_usuarios_script.sql` en `04_datos.sql`.
- Preservar los scripts originales en `docs/historico/bd/` (no se borran; quedan como evidencia histórica).
- Reorganizar `docs/`: `docs/DER.md` y `docs/ERS.md` reescritos (versión final) con las versiones 1 archivadas en `docs/historico/`; documentos de trabajo viejos movidos a `docs/historico/`.

## Impact
- **Capacidad**: `esquema-base-datos`.
- **Archivos nuevos**: `bd/01_estructura.sql`, `bd/02_procedimientos.sql`, `bd/03_triggers.sql`, `bd/04_datos.sql`, `bd/05_utilidades_test.sql`, `bd/README.md`.
- **Archivos movidos**: los 26 `.sql` originales → `docs/historico/bd/`; `docs/plan/` y `docs/script/` → `docs/historico/`; `docs/referencia/legacy/` → `docs/historico/legacy/`; `docs/DER.md`/`docs/ERS.md` originales → `docs/historico/*_v1.md`.
- **Sin cambios de código** de la aplicación, sin cambios de dependencias; `dotnet build` debe permanecer sin errores (verificación de no-regresión).
- **Esquema**: mismo modelo físico (18 tablas). No se agregan ni quitan tablas ni columnas; solo se consolida y se amplía el seed de `Marca`/`Modelo`.

## Non-goals (fuera de alcance)
- Crear migraciones EF Core (la fuente sigue siendo `bd/`).
- Cambiar el modelo de datos (tablas/columnas) o los contratos de los SP.
- Modificar código de la aplicación o la UI.
- Reescribir la lógica de los procedimientos: se preservan sus cuerpos **verbatim**, tomando la última versión vigente de cada uno.
