# Plan: Unificación y Reorganización Canónica de Scripts SQL en `bd/`

**Fecha:** 2026-10-28  
**Autor:** walterg20 / Antigravity  
**Objetivo:** Consolidar y unificar los scripts SQL de `bd/` en 5 archivos canónicos ordenados lógicamente, limpiando scripts residuales, basando los datos semilla en el catálogo completo oficial (marcas, modelos, servicios, roles, permisos y solo el usuario Administrador), y adaptando la infraestructura de pruebas.

---

## 1. Diagnóstico del Estado Actual

En el directorio `bd/` conviven scripts canónicos desactualizados con archivos huérfanos/basura introducidos por commits previos:
- `01_estructura.sql` (Esquema DDL con 18 tablas).
- `02_procedimientos.sql` (14 SPs).
- `03_CreateTable_Facturas.sql` (Archivo vacío, 2 bytes - **huérfano a eliminar**).
- `03_triggers.sql` (3 Triggers).
- `04_datos.sql` (Datos semilla antiguos con catálogo incompleto y usuarios demo).
- `05_utilidades_test.sql` (Consultas de verificación).
- `06_Estructura_datos_actualizado.sql` (Dump desordenado de SSMS - **huérfano a eliminar**).
- `bd.slnx` (Solución vacía generada por error de IDE - **huérfano a eliminar**).
- `db_factura_metodo_pago.sql` (Script de prueba desactualizado - **huérfano a eliminar**).
- `sp_GenerarComprobantePago.sql` (Borrador desactualizado en la raíz de `bd/` - **huérfano a eliminar**).

---

## 2. Nuevo Esquema Canónico Propuesto (5 Scripts)

Se establecerá el orden solicitado:

```
bd/
├── 01_estructura.sql       (DDL completo de 18 tablas, secuencias, índices y FKs)
├── 02_datos.sql            (Catálogo canónico completo: marcas, modelos, servicios, roles, permisos, Admin)
├── 03_procedimientos.sql   (Los 14 procedimientos almacenados consolidados)
├── 04_triggers.sql         (Los 3 triggers de negocio)
└── 05_utilidades_test.sql  (Scripts de verificación y utilidades de test)
```

### Detalle de cada script:

### `01_estructura.sql`
- **Contenido:** Secuencia `Seq_Factura_Numero` + DDL de las 18 tablas del sistema (`Cliente`, `Marca`, `Modelo`, `Vehiculo`, `PropietarioVehiculo`, `Roles`, `Permisos`, `RolPermisos`, `Usuarios`, `Taller`, `Servicio`, `RegistroServicio`, `DetalleServicio`, `Factura`, `MetodoPago`, `Pago`, `ConfiguracionEmpresa`, `ConfiguracionBackup`).
- **Idempotencia:** `IF OBJECT_ID ... IS NULL CREATE TABLE ...`.
- **Compatibilidad y resiliencia:**
  - Columnas `Activo` presentes en las tablas requeridas con default `(1)`.
  - Migración segura para bases existentes (renombrar `Estado` a `Activo` en `Roles` y `Usuarios` si aún existiese).

### `02_datos.sql` (Antes `04_datos.sql`)
- **Contenido:**
  1. **Roles:** Los 3 roles del sistema (1: Administrador, 2: Recepcionista, 3: Mecánico).
  2. **Permisos:** Catálogo completo de los 38 permisos RBAC.
  3. **RolPermisos:** Matriz de asignación Rol ➔ Permiso completa y autorreparable.
  4. **Usuario Administrador:** **Únicamente el usuario Admin** (`admin@example.com`, pass hash inicial `CHANGE_ME`, rol 1, `Activo = 1`). *Se eliminan del seed base los usuarios demo de mecánico y recepción.*
  5. **Configuración Inicial:** Fila singleton de `ConfiguracionEmpresa` (Id=1) y `ConfiguracionBackup` (Id=1).
  6. **Taller:** Fila singleton de `Taller` (Id=1, Taller PRO).
  7. **Métodos de Pago:** `Efectivo` y `Tarjeta`.
  8. **Servicios:** Los 15 servicios oficiales con precios actualizados y `Activo = 1`.
  9. **Marcas y Modelos:** Catálogo completo argentino con **45 marcas** y **203 modelos**, cargados e insertados mediante `MERGE` idempotente (preservando `Toyota = Id 1` y `Renault = Id 2`).
  *Sin inserción de clientes demo, vehículos demo ni órdenes demo; la base queda limpia para producción.*

### `03_procedimientos.sql` (Renombrado desde `02_procedimientos.sql`)
- **Contenido:** Consolidación de los 14 procedimientos almacenados oficiales (`sp_GenerarComprobanteOrden`, `sp_GenerarComprobantePago`, reportes operativos, reportes gerenciales, productividad de mecánicos, etc.).
- **Técnica:** Todos con `CREATE OR ALTER PROCEDURE` para ejecución idempotente.

### `04_triggers.sql` (Renombrado desde `03_triggers.sql`)
- **Contenido:** Los 3 triggers de auditoría y negocio:
  1. `TR_DetalleServicio_ValidarPrecioYCantidad` (valida precio y cantidad > 0).
  2. `TR_DetalleServicio_BloquearModificacionFinalizada` (protege órdenes cerradas).
  3. `TR_RegistroServicio_SincronizarKm` (sincroniza odómetro en el vehículo).
- **Técnica:** Todos con `CREATE OR ALTER TRIGGER`.

### `05_utilidades_test.sql`
- **Contenido:** Scripts de consulta rápida de conteos, verificación de integridad referencial, estado de índices y consultas para entorno de prueba.

---

## 3. Tratamiento de los Scripts en `docs/script/`

En `docs/script/` residen actualmente 3 archivos SQL:

1. **`migracion_actualizar_bd_segura.sql`** (13 KB):
   - **Contenido:** Renombramiento de `Estado` a `Activo`, defaults `DEFAULT (1)`, catálogo de 15 servicios y marcas/modelos.
   - **Destino:** Su contenido queda **100% absorbido e integrado** en los scripts canónicos `bd/01_estructura.sql` y `bd/02_datos.sql`. Se conservará en `docs/script/` como historial/bitácora ejecutable de la migración local, o se podrá archivar.
2. **`sincronizar_todas_marcas_modelos.sql`** (18 KB):
   - **Contenido:** Inserción de las columnas `Activo` faltantes y los 203 modelos del mercado argentino.
   - **Destino:** Su catálogo completo de 203 modelos y 45 marcas se incorpora **directamente en el `MERGE` de `bd/02_datos.sql`**. Al quedar integrado en el script canónico, no requiere volver a ejecutarse por separado.
3. **`BAckup_activos.sql`** (6 MB):
   - **Contenido:** Respaldo completo de la base de datos local con datos operativos (10 clientes, 8 vehículos, historial).
   - **Destino:** Es un backup personal de resguardo. **NO debe commitearse a Git** para no inflar el repositorio con 6 MB innecesarios. Se añadirá `*.sql` en `docs/script/` o específicamente `docs/script/BAckup_activos.sql` a `.gitignore` para que quede protegido localmente.

---

## 4. Plan de Acción y Tareas

- [x] **Fase 1: Preparación y Creación de Scripts Canónicos en `bd/`**
  - [x] 1.1 Crear `bd/02_datos.sql` unificando roles, permisos, rol-permisos, taller, configuraciones, **únicamente usuario Admin**, 15 servicios y el catálogo completo de 45 marcas y 203 modelos (absorbiendo lo de `docs/script/`).
  - [x] 1.2 Mover/Renombrar `bd/02_procedimientos.sql` a `bd/03_procedimientos.sql`.
  - [x] 1.3 Mover/Renombrar `bd/03_triggers.sql` a `bd/04_triggers.sql`.
  - [x] 1.4 Eliminar el archivo antiguo `bd/04_datos.sql`.

- [x] **Fase 2: Limpieza de Archivos Residuales en `bd/` y Protección de Backups**
  - [x] 2.1 Eliminar `bd/03_CreateTable_Facturas.sql`.
  - [x] 2.2 Eliminar `bd/06_Estructura_datos_actualizado.sql`.
  - [x] 2.3 Eliminar `bd/bd.slnx`.
  - [x] 2.4 Eliminar `bd/db_factura_metodo_pago.sql`.
  - [x] 2.5 Eliminar `bd/sp_GenerarComprobantePago.sql`.
  - [x] 2.6 Proteger `docs/script/BAckup_activos.sql` en `.gitignore` para no subir 6 MB al repositorio.

- [x] **Fase 3: Adaptación de Tests y Código de Soporte**
  - [x] 3.1 Actualizar `TP_ControlVehicular.Tests\EndToEnd\Infra\DbProvisioner.cs`:
    - Cambiar array de scripts a: `"01_estructura.sql"`, `"02_datos.sql"`, `"03_procedimientos.sql"`, `"04_triggers.sql"`.
  - [x] 3.2 Actualizar `TP_ControlVehicular.Tests\TP_ControlVehicular.Tests.csproj`:
    - Actualizar los elementos `<None Include="..\bd\..." />` para reflejar los nuevos nombres de archivo.
  - [x] 3.3 Actualizar `TP_ControlVehicular.Tests\EndToEnd\Infra\SemillaE2E.cs`:
    - Agregar guard `if (recepcionista == null)` para crear un usuario recepcionista demo en la base de test si solo existe el Admin, garantizando que los tests E2E sigan pasando de forma autónoma.

- [x] **Fase 4: Actualización de Documentación del Repositorio**
  - [x] 4.1 Actualizar `bd/README.md` detallando el nuevo orden `01 -> 02 -> 03 -> 04 -> 05`.
  - [x] 4.2 Actualizar referencias en `AGENTS.md` y `MEMORY.md`.

- [x] **Fase 5: Verificación y Pruebas**
  - [x] 5.1 Ejecutar `dotnet build "TP_ControlVehicular.slnx"` y verificar 0 errores.
  - [x] 5.2 Ejecutar `dotnet test "TP_ControlVehicular.slnx"` y verificar que los 24 tests pasen exitosamente.

