# ERS – Sistema de Control Vehicular y Mantenimiento

> **Versión:** final · **Fecha:** 2026-10-12
> Documento vigente y **autoritativo**. Describe los requisitos del sistema tal como está implementado.
> La versión 1 (que referenciaba entidades inexistentes como `CONTACTO`, `PLANTILLA_MANTENIMIENTO` y `AVISO`) quedó archivada en `docs/historico/ERS_v1.md` y **no debe usarse** como referencia.

---

## Capítulo 1 – Introducción

### 1.1 Propósito
Definir los requisitos funcionales y no funcionales del sistema de control vehicular y mantenimiento, que gestiona clientes, vehículos, talleres, órdenes de servicio, facturación y control de acceso por roles.

### 1.2 Alcance
El sistema permitirá:
- Autenticar usuarios y controlar el acceso mediante roles y permisos (RBAC).
- Registrar y administrar clientes, marcas, modelos, vehículos y su histórico de propiedad.
- Registrar talleres y el catálogo de servicios.
- Registrar órdenes de servicio con sus tareas (detalle), estados y kilometraje.
- Ejecutar tareas desde la pantalla **Mis Trabajos** (perfil Mecánico).
- Facturar órdenes y registrar pagos por distintos medios.
- Visualizar un dashboard y generar reportes según el perfil.
- Configurar los datos de la empresa (incluido el logo) y los parámetros de respaldo.

**Fuera de alcance:** el envío de avisos por SMS/correo y la programación de mantenimientos periódicos no forman parte del sistema actual.

### 1.3 Definiciones y siglas
- **ERS**: Especificación de Requisitos de Software.
- **DER**: Diagrama Entidad-Relación.
- **CRUD**: Create, Read, Update, Delete (operaciones básicas).
- **RBAC**: Control de acceso basado en roles (*Role-Based Access Control*).
- **UML**: Lenguaje Unificado de Modelado.
- **SP**: *Stored Procedure* (procedimiento almacenado).

### 1.4 Referencias
- `docs/DER.md` – Diagrama Entidad-Relación vigente (18 tablas).
- `bd/01_Estructura.sql` – DDL canónico de la base de datos.
- `AGENTS.md` – Constitución técnica del proyecto (stack, convenciones y reglas).
- `docs/referencia/` – Conocimiento de dominio (reportes, dashboard, BD, pantallas).

### 1.5 Visión general del documento
El documento se estructura en siete capítulos: el **Capítulo 1** introduce propósito y alcance; el **Capítulo 2** presenta la descripción general del sistema; el **Capítulo 3** detalla los requisitos funcionales; el **Capítulo 4** los requisitos no funcionales; el **Capítulo 5** los casos de uso; el **Capítulo 6** el modelo de datos; y el **Capítulo 7** concluye.

---

## Capítulo 2 – Descripción general

### 2.1 Contexto
Aplicación de escritorio **WPF (.NET 10)** con arquitectura **MVVM** y **EF Core 10** sobre **SQL Server**. La persistencia se organiza en las capas `Datos`, `Entidad`, `Migracion`, `Negocio` y `Presentacion`.

### 2.2 Módulos y pantallas
- **Login** – autenticación.
- **Clientes**, **Vehículos**, **Modelos**, **Marcas** – parque automotor.
- **Talleres**, **Servicios** – catálogo operativo.
- **Órdenes de Servicio** (`OrdenServicio` + formulario modal) – registro y gestión de órdenes.
- **Mis Trabajos** – ejecución de tareas por el mecánico.
- **Dashboard** – indicadores según perfil.
- **Reportes** – Ordenes, Operativo, Gerencial y Mecánico.
- **Pagos** (`FrmPago`) – cobro de facturas.
- **Seguridad** – Roles y Usuarios (`FrmRolPermiso` asigna permisos).
- **Configuración** – datos de la empresa (`CtlConfiguracion`) y respaldos (`CtlBackup`, `CtlBackupConfiguracion`).
- **Confirmaciones** (`FrmConfirmacion`) – diálogos de confirmación y avisos unificados.

### 2.3 Actores
| Actor | Descripción |
| :--- | :--- |
| **Administrador** | Control total: seguridad, configuración, reportes gerenciales y respaldos. |
| **Recepcionista** | Registra clientes, vehículos, órdenes y pagos; reportes operativos. |
| **Mecánico** | Ejecuta las tareas asignadas desde *Mis Trabajos*; reporte mecánico. |

---

## Capítulo 3 – Requisitos funcionales

| Código | Requisito | Descripción | Prioridad |
| :--- | :--- | :--- | :--- |
| **RF‑01** | Autenticación | Validar usuario y contraseña para iniciar sesión y establecer el rol activo. | Alta |
| **RF‑02** | Gestión de clientes | Registrar, editar, listar y dar de baja clientes (datos personales y estado activo). | Alta |
| **RF‑03** | Gestión de marcas | Registrar, editar y listar marcas de vehículos. | Media |
| **RF‑04** | Gestión de modelos | Registrar, editar y listar modelos, asociados a una marca. | Media |
| **RF‑05** | Gestión de vehículos | Registrar, editar y listar vehículos (modelo, año, patente, kilometraje). | Alta |
| **RF‑06** | Registro de propiedad | Asociar clientes y vehículos conservando el histórico de dueños. | Media |
| **RF‑07** | Gestión de talleres | Registrar talleres con nombre, dirección, teléfono y estado activo. | Media |
| **RF‑08** | Catálogo de servicios | Registrar tipos de servicio con su precio y estado activo. | Alta |
| **RF‑09** | Órdenes de servicio | Registrar órdenes (vehículo, taller, usuario, fecha, km de ingreso) con múltiples tareas. | Alta |
| **RF‑10** | Tareas del detalle | Agregar/editar tareas (servicio, técnico, cantidad, precio, orden, origen, estado, observaciones). | Alta |
| **RF‑11** | Estados de la orden | Gestionar los estados `Abierta`, `En Proceso`, `Completada`, `Pagada` y `Cancelada`. | Alta |
| **RF‑12** | Sincronización de kilometraje | Actualizar el kilometraje del vehículo al ingresar una orden, validando que no retroceda. | Alta |
| **RF‑13** | Facturación | Emitir una factura por orden de servicio con numeración consecutiva y total. | Alta |
| **RF‑14** | Registro de pagos | Registrar pagos sobre una factura, totales o parciales, indicando el medio de pago. | Alta |
| **RF‑15** | Gestión de usuarios | Crear, editar, listar y desactivar usuarios, asignándoles un rol. | Alta |
| **RF‑16** | Roles y permisos | Gestionar roles y asignarles permisos (RBAC) que habilitan funcionalidades. | Alta |
| **RF‑17** | Dashboard | Mostrar indicadores operativos adaptados al perfil del usuario. | Media |
| **RF‑18** | Reportes | Generar reportes por perfil: Órdenes, Operativo, Gerencial y Mecánico (PDF). | Media |
| **RF‑19** | Configuración de la empresa | Registrar datos de la empresa (nombre, dirección, teléfono, email y logo). | Media |
| **RF‑20** | Configuración de respaldos | Configurar rutas de respaldo y carpeta de Drive, y ejecutar el respaldo de la base. | Media |

---

## Capítulo 4 – Requisitos no funcionales

- **RNF‑01: Rendimiento** → Respuestas ágiles en operaciones CRUD y en la generación de reportes.
- **RNF‑02: Usabilidad** → Interfaz WPF intuitiva, con formularios modales y diálogos de confirmación unificados.
- **RNF‑03: Seguridad** → Autenticación de usuarios y control de acceso por roles y permisos (RBAC).
- **RNF‑04: Integridad referencial** → Claves foráneas, restricciones `CHECK` y triggers que garantizan consistencia (kilometraje, precios, estados).
- **RNF‑05: Mantenibilidad** → Arquitectura por capas (Datos, Entidad, Migracion, Negocio, Presentacion) con patrón MVVM y EF Core.
- **RNF‑06: Portabilidad** → Ejecutable de escritorio sobre Windows con SQL Server.
- **RNF‑07: Trazabilidad** → Registro histórico de operaciones (propiedad de vehículos, órdenes y pagos).

---

## Capítulo 5 – Casos de uso

| Código | Caso de uso | Actor | Descripción |
| :--- | :--- | :--- | :--- |
| **CU‑01** | Iniciar sesión | Todos | El usuario ingresa sus credenciales y accede según su rol. |
| **CU‑02** | Registrar cliente | Recepcionista | Carga los datos personales y el estado del cliente. |
| **CU‑03** | Registrar vehículo y propietario | Recepcionista | Carga el vehículo y lo asocia a su propietario. |
| **CU‑04** | Registrar taller | Administrador | Carga los datos del taller y su estado. |
| **CU‑05** | Registrar servicio | Administrador | Carga un tipo de servicio con su precio. |
| **CU‑06** | Registrar orden de servicio | Recepcionista | Crea la orden con sus tareas y kilometraje de ingreso. |
| **CU‑07** | Ejecutar tareas | Mecánico | Trabaja las tareas asignadas desde *Mis Trabajos*. |
| **CU‑08** | Facturar orden | Recepcionista | Emite la factura de una orden de servicio. |
| **CU‑09** | Registrar pago | Recepcionista | Registra pagos totales o parciales sobre la factura. |
| **CU‑10** | Gestionar roles y permisos | Administrador | Crea roles y asigna permisos. |
| **CU‑11** | Gestionar usuario | Administrador | Crea, edita o desactiva usuarios y les asigna rol. |
| **CU‑12** | Consultar dashboard | Todos | Visualiza los indicadores según su perfil. |
| **CU‑13** | Generar reporte | Administrador / Recepcionista / Mecánico | Exporta el reporte habilitado para su perfil. |
| **CU‑14** | Configurar empresa | Administrador | Actualiza datos y logo de la empresa. |
| **CU‑15** | Configurar y ejecutar respaldo | Administrador | Define rutas y ejecuta (o sube) el respaldo de la base. |

---

## Capítulo 6 – Modelo de datos

El modelo relacional consta de **18 tablas** en SQL Server, agrupadas por dominio:

- **Seguridad y acceso:** `Roles`, `Permisos`, `Usuarios`, `RolPermisos`.
- **Clientes y parque automotor:** `Cliente`, `Marca`, `Modelo`, `Vehiculo`, `PropietarioVehiculo`.
- **Operación de taller:** `Taller`, `Servicio`, `RegistroServicio`, `DetalleServicio`.
- **Facturación y cobros:** `Factura`, `MetodoPago`, `Pago`.
- **Configuración:** `ConfiguracionEmpresa`, `ConfiguracionBackup`.

El detalle de atributos, claves y cardinalidades se encuentra en el documento **DER vigente** (`docs/DER.md`).

---

## Capítulo 7 – Conclusión

El sistema de control vehicular cubre el ciclo completo desde el alta del cliente y su vehículo hasta la ejecución de las tareas en el taller, la facturación y el cobro, con control de acceso por roles, dashboard y reportes por perfil. El modelo de 18 tablas y las validaciones a nivel de base (claves foráneas, `CHECK` y triggers) garantizan consistencia, trazabilidad e integridad operativa.
