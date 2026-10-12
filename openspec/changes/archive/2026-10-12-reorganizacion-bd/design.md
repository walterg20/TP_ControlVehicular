# Technical Design: Reorganización de Scripts de Base de Datos (`bd/`)

## Contexto

No existen migraciones EF Core: el esquema físico de `ControlVehicular` **vive en los scripts `bd/`**. La carpeta acumuló 26 archivos con numeración rota y contenido solapado. Este cambio no altera el modelo de datos; **reorganiza su fuente canónica** en 4 scripts por tipo, una utilidad de desarrollo/test y un README, y amplía el seed de catálogo.

## Modelo físico resultante (18 tablas)

`Cliente`, `Marca`, `Modelo`, `Vehiculo`, `PropietarioVehiculo`, `Roles`, `Usuarios`, `Permisos`, `RolPermisos`, `Taller`, `Servicio`, `RegistroServicio`, `DetalleServicio`, `Factura`, `Pago`, `MetodoPago`, `ConfiguracionEmpresa`, `ConfiguracionBackup`.

> La tabla `Facturas` (plural) de `03_CreateTable_Facturas.sql` es **obsoleta** y se descarta.

## Estructura canónica de `bd/`

| Archivo | Contenido | Origen consolidado |
|---|---|---|
| `01_estructura.sql` | DDL: 18 tablas, PK/FK, constraints, índices. | `01_Estructura.sql`, `04_schema_billing.sql`, `08_Migracion_PropietarioVehiculo.sql`, `19_ConfiguracionEmpresa.sql` (parte DDL), `21_ConfiguracionBackup.sql` (parte DDL) |
| `02_procedimientos.sql` | Todos los SP (reportes por rol, comprobantes, workflow). | `02_StoredProcedures_Reportes.sql`, `sp_GenerarComprobanteOrden.sql`, `sp_GenerarComprobantePago.sql`, `05_WorkflowOrdenesServicio.sql`, `07…18` (SP) y redefiniciones de `06_Migraciones_Manuales.sql` |
| `03_triggers.sql` | 3 triggers del dominio. | Triggers de `01_Estructura.sql` / `06_Migraciones_Manuales.sql` |
| `04_datos.sql` | Seed: roles, permisos, rol-permisos, taller, configuraciones, Admin bootstrap, **marcas y modelos**. | `02_Datos.sql`, `09_PermisosAvanzados.sql`, `10…12`, `17`, `20`, `22`, `BD_role_usuarios_script.sql` + catálogo AR nuevo |
| `05_utilidades_test.sql` | Utilidad de **desarrollo/test**: purga idempotente y FK-safe de artefactos E2E (patentes `TDD%`, clientes `EndToEnd`). No es parte del bootstrap productivo. | `15_Limpieza_Artefactos_TestE2E.sql` |
| `README.md` | Orden de aplicación, convención de idempotencia y mapa. | Nuevo |

## Decisiones de diseño

### 1. Idempotencia uniforme
Todo script se escribe con guardas (`IF OBJECT_ID(...) IS NULL`, `IF NOT EXISTS`, `CREATE OR ALTER`) para poder re-ejecutarse sin error y sin duplicar filas. Es un requisito operativo porque el despliegue es manual (SQL Server 2019 Express sin Agente).

### 2. Triggers (los 3 del dominio)
- `TR_DetalleServicio_ValidarPrecioYCantidad`
- `TR_RegistroServicio_SincronizarKm`
- `TR_DetalleServicio_BloquearModificacionFinalizada`

Se definen en `03_triggers.sql` con guarda de existencia (`DROP … IF EXISTS` + `CREATE`, o `CREATE OR ALTER` en SQL Server 2016+).

### 3. SP "última versión gana"
Varios scripts redefinen el mismo SP (p. ej. `06_Migraciones_Manuales.sql` redefine 5 SP; `14_Fix_ReporteOperativo.sql` y `16_Fix_Colision_sp_ReporteIngresos.sql` corrigen reportes). En `02_procedimientos.sql` se incluye **solo la última definición vigente** de cada SP, con su cuerpo verbatim.

### 4. `04_datos.sql` — seed y catálogo argentino
- **Bootstrap** (idempotente): roles (Admin=1, Recepcionista=2, Mecánico=3), permisos, `RolPermisos`, taller, `ConfiguracionEmpresa` (Id=1) y `ConfiguracionBackup` (Id=1), y el usuario Admin inicial.
- **Catálogo `Marca`/`Modelo` ampliado**: marcas y modelos representativos del mercado argentino (alcance ~40+ marcas con modelos típicos). Se **preservan** `Toyota = Id 1` y `Renault = Id 2` para no romper FKs/Id existentes ni datos ya cargados.
- Se **descarta** `BD_role_usuarios_script.sql` como archivo, plegando su contenido útil aquí.

### 5. Preservación histórica
Los 26 originales se mueven a `docs/historico/bd/` (no se borran). Git detecta los renames. La documentación de dominio (`docs/referencia/bd-y-dominio.md`) se actualiza a los nombres canónicos.

## Alternativas consideradas y descartadas
- **Mantener los 26 scripts numerados**: conserva la trazabilidad, pero perpetúa la numeración rota y los obsoletos. Descartado.
- **Extraer el DDL con `Scaffold-DbContext`/migración EF**: introduce migraciones EF donde hoy no las hay y acopla el esquema al modelo C#. Fuera de alcance.
- **Crear un `.sql` por tabla/SP**: no aporta sobre la división por tipo y multiplica archivos. Descartado.

## Riesgos y mitigación
| Riesgo | Mitigación |
|---|---|
| Perder una definición de SP al consolidar | Inventario por SP y comparación con los originales; el cuerpo se copia verbatim. |
| Romper datos existentes al re-ejecutar el seed | Guardas `IF NOT EXISTS` por clave; no se reescriben filas existentes. |
| Cambiar Ids de `Marca` (Toyota/Renault) | Se preservan explícitamente los Ids 1 y 2. |
| Encoding (tildes/emojis) | Archivos UTF-8 escritos con herramienta segura; prohibido `>`/`Set-Content`. |

## Verificación
1. Ejecutar `01` → `04` sobre una base limpia: se crean 18 tablas, los SP, los 3 triggers y el seed.
2. Re-ejecutar `01` → `04` sobre la misma base: **sin errores y sin duplicados** (idempotencia).
3. Confirmar que no existe la tabla `Facturas` (plural).
4. (Desarrollo) Ejecutar `05_utilidades_test.sql` sobre una base con artefactos E2E: los purga sin tocar datos reales.
5. `dotnet build "TP_ControlVehicular.slnx"` sin errores.
