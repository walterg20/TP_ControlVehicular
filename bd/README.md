# `bd/` — Scripts de base de datos (canónicos)

Esta carpeta reúne los scripts **canónicos** de la base de datos `ControlVehicular`,
consolidados por tipo a partir de los 26 scripts históricos que hoy viven en
`docs/historico/bd/`. La consolidación se hizo el **2026-10-12**.

El objetivo es tener **pocos archivos, idempotentes y re-ejecutables**, en lugar de
una cadena de scripts numerados donde unos pisan a otros.

## Contenido y orden de aplicación

| Orden | Archivo | Contenido |
|---|---|---|
| 1 | `01_estructura.sql` | Secuencia de facturación, 18 tablas, índices, PK/FK, constraints CHECK y sincronización/guardas defensivas de columnas `Activo` y `OrdenEjecucion`. |
| 2 | `02_datos.sql` | Datos semilla canónicos (seed para producción / base limpia): 3 roles, 38 permisos, matriz RolPermisos, taller base, configuraciones singleton, usuario Admin bootstrap (`admin@example.com` / `10000001`), catálogo oficial de 15 servicios y catálogo ampliado de 45 marcas y 203 modelos argentinos. |
| 3 | `03_procedimientos.sql` | 22 procedimientos almacenados oficiales (reportes, comprobantes y consultas paginadas a nivel de BD). |
| 4 | `04_triggers.sql` | 3 triggers de integridad del workflow. |
| 5 | `05_utilidades_test.sql` | Utilidades de desarrollo/pruebas (purga de artefactos E2E y verificaciones). |
| 6 | `06_poblacion.sql` | Población de datos operativos y de demostración: personal de taller (recepción y mecánicos), 55 clientes, 55 vehículos, 55 titularidades, 61 órdenes de servicio (Junio-Octubre 2026), 110+ tareas de servicio, facturas y pagos. |

Aplicar **siempre** en el orden `01_estructura → 02_datos → 03_procedimientos → 04_triggers`. Si se desea cargar el entorno con datos de prueba/demostración, ejecutar a continuación `06_poblacion.sql`.

## Idempotencia

Todos los objetos se crean con guardas o con `CREATE OR ALTER`, de modo que **los
archivos pueden re-ejecutarse** sobre una base existente sin duplicar ni romper nada:

- Tablas: `IF OBJECT_ID(N'[dbo].[Tabla]', N'U') IS NULL ... BEGIN CREATE TABLE ... END`.
- Secuencia: guarda vía `sys.sequences`.
- Índices: guarda vía `sys.indexes`.
- SP y triggers: `CREATE OR ALTER ...` (reemplazan la definición vigente).

Requisito: la base de datos `[ControlVehicular]` debe existir previamente (los scripts
hacen `USE [ControlVehicular];`). Compatible con **SQL Server 2019 Express**.

## Mapa de consolidación (origen → destino)

| Script histórico (`docs/historico/bd/…`) | Destino canónico | Observación |
|---|---|---|
| `01_Estructura.sql` | `01_estructura.sql` | DDL base de las tablas. |
| `02_Datos.sql` | `04_datos.sql` | Datos semilla. |
| `02_StoredProcedures_Reportes.sql` | `02_procedimientos.sql` | Versión **superada** por `06_` para `sp_ReporteTiemposResolucion`, `sp_HistorialVehiculo`, `sp_VehiculosPorCliente` y `sp_ServiciosPorMecanico`. |
| `03_CreateTable_Facturas.sql` | *(descartado)* | Crea una tabla `Facturas` (plural), obsoleta; el esquema vigente usa `Factura` (singular). Ver nota. |
| `04_schema_billing.sql` | `01_estructura.sql` + `04_datos.sql` | Tablas `MetodoPago`/`Factura`/`Pago` al esquema; semilla de `MetodoPago` a datos. |
| `05_WorkflowOrdenesServicio.sql` | `01_estructura.sql` | Columna `DetalleServicio.OrdenEjecucion` + `CHK_DetalleServicio_Estado` / `CHK_RegistroServicio_Estado`. |
| `06_Migraciones_Manuales.sql` | `02_procedimientos.sql` + `03_triggers.sql` + `01_estructura.sql` | 4 SP, 3 triggers y 5 CHECK. |
| `07_Reportes_Administrador.sql` | `02_procedimientos.sql` | `sp_ReporteTopClientes` y `sp_ReporteModelosReparados`; su `sp_ReporteIngresos` fue **superado** por `16_`. |
| `08_Migracion_PropietarioVehiculo.sql` | `01_estructura.sql` | Tabla `PropietarioVehiculo`. |
| `09_PermisosAvanzados.sql` | `04_datos.sql` | Seed RBAC (23 permisos base + todos a Admin). |
| `10_ConfiguracionPermisosRecepcionMecanico.sql` | `04_datos.sql` | Seed RBAC (`MisTrabajos.Ver` + matrices). |
| `11_SP_ReportesMecanico.sql` | `02_procedimientos.sql` | 2 SP (historial clínico, hoja de trabajo diaria). |
| `12_PermisosReportesPorRol.sql` | `04_datos.sql` | Seed RBAC (4 permisos de reportes por rol). |
| `13_SP_ReporteProductividadMecanicos.sql` | `02_procedimientos.sql` | 1 SP. |
| `14_Fix_ReporteOperativo.sql` | *(no consolidado)* | Corrección puntual de datos (one-off histórico). |
| `15_Limpieza_Artefactos_TestE2E.sql` | `05_utilidades_test.sql` | Utilidad de limpieza de datos de prueba E2E. |
| `16_Fix_Colision_sp_ReporteIngresos.sql` | `02_procedimientos.sql` | Resolución de la colisión de nombre (ver nota). |
| `17_QuitarMisTrabajosAdmin.sql` | *(no consolidado)* | `DELETE` puntual de permiso (one-off histórico). |
| `18_SP_ReporteServiciosPorRango.sql` | `02_procedimientos.sql` | 1 SP. |
| `19_ConfiguracionEmpresa.sql` | `01_estructura.sql` + `04_datos.sql` | Tabla al esquema; fila singleton a datos. |
| `20_PermisosConfiguracionEmpresa.sql` | `04_datos.sql` | Seed RBAC (`Configuracion.Empresa.Gestionar` → Admin). |
| `21_ConfiguracionBackup.sql` | `01_estructura.sql` + `04_datos.sql` | Tabla al esquema; fila singleton a datos. |
| `22_PermisosBackup.sql` | `04_datos.sql` | Seed RBAC (`Configuracion.Backup.Gestionar` → Admin). |
| `BD_role_usuarios_script.sql` | `04_datos.sql` | Seed de roles/usuarios (ver nota). |
| `sp_GenerarComprobanteOrden.sql` | `02_procedimientos.sql` | 1 SP. |
| `sp_GenerarComprobantePago.sql` | `02_procedimientos.sql` | 1 SP. |

## Notas

- **`03_CreateTable_Facturas.sql` se descarta**: crea una tabla `Facturas` en plural,
  obsoleta; el esquema vigente (EF Core y los scripts posteriores) usa `Factura` en
  singular. No se recrea.
- **`BD_role_usuarios_script.sql`**: su contenido semilla (roles/usuarios) se **funde
  en `04_datos.sql`**, que es el dueño único de los datos semilla.
- **`04_datos.sql`**: contiene toda la semilla, incluido el **catálogo ampliado de
  marcas y modelos representativo del mercado argentino** (preservando Toyota = 1 y
  Renault = 2). Se mantiene por separado del esquema y **no** debe tocarse al
  re-ejecutar la estructura.
- **Colisión de `sp_ReporteIngresos`**: `06_` lo definía con 3 parámetros y `07_` lo
  redefinía con 2 parámetros y el **mismo nombre** (la última definición pisaba a la
  anterior). `16_` resolvió el problema renombrando la versión administrativa a
  `sp_ReporteIngresosAdmin` y restaurando la de 3 parámetros. El canónico conserva
  ambas versiones desde `16_`.
- **Constraints CHECK**: se consolidaron como constraints en línea en
  `01_estructura.sql` (esquema canónico) y no como `ALTER TABLE` sueltos.

## Hallazgos e inconsistencias de los originales

Durante la consolidación se detectaron estas diferencias entre scripts:

- `09_PermisosAvanzados.sql` definía una FK que referenciaba una columna inexistente
  `Roles([IdRol])`; el canónico corrige la referencia a `Roles([Id])`.
- `08_Migracion_PropietarioVehiculo.sql` creaba la FK de `Cliente` con `ON DELETE
  CASCADE`; el modelo EF Core usa `Restrict`, por lo que el canónico usa
  `ON DELETE NO ACTION` (divergencia documentada).
- El `Vehiculo` final **no** tiene columna `ClienteId` (la relación es vía
  `PropietarioVehiculo`); el canónico omite la columna, su FK y su índice.
- `04_schema_billing.sql` definía `Factura.Fecha` y `Pago.FechaPago` como `DATE`,
  mientras las entidades usan `DateTime`; el canónico usa `datetime2`. También se
  normalizaron los nombres de FK a la convención EF
  (`FK_Factura_RegistroServicio_RegistroServicioId`, `FK_Pago_Factura_FacturaId`,
  `FK_Pago_MetodoPago_MetodoPagoId`) desde los originales `FK_Factura_Registro`,
  `FK_Pago_Factura`, `FK_Pago_Metodo`.
- La restricción `Numero INT NOT NULL UNIQUE` de `Factura` (`04_`) **no** se recrea:
  el modelo EF Core asegura la unicidad mediante la secuencia
  `Seq_Factura_Numero`.
- `02_Datos.sql` sembraba estados inválidos (`DetalleServicio.Estado = 'Realizado'`,
  `RegistroServicio.Estado = 'Pendiente'`), ajenos a los CHECK vigentes; esos datos
  quedan bajo el dominio de `04_datos.sql`.
- Los DNIs de usuario se repetían entre `BD_role_usuarios_script.sql` (`11111111`,
  `22222222`, `33333333`) y `02_Datos.sql` (`10000001`, `10000002`, `10000003`),
  incompatibles con los índices únicos `IX_Usuarios_Dni` / `IX_Usuarios_Email`;
  `04_datos.sql` debe quedarse con un único juego consistente.
- Los cuerpos de SP y triggers provenientes de `06_` venían con una indentación
  uniforme heredada de un heredoc; en el canónico se les quitó el prefijo común
  (16 espacios) preservando su contenido y formato relativo.

## Referencias

- Diagrama de entidad-relación: [`../docs/DER.md`](../docs/DER.md).
- Material histórico y scripts originales: `docs/historico/bd/`.
