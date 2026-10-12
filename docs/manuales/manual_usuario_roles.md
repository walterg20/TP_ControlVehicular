# 📘 Manual de Usuario Operativo por Roles — Taller Pro

**Sistema:** TP_ControlVehicular · **Nombre comercial:** *Taller Pro*
**Versión del documento:** 1.0 · **Plataforma:** Aplicación de escritorio Windows (WPF, .NET 10)
**Documento complementario:** [Manual Técnico de Administración y TI](manual_tecnico_administracion.md) · [Portal de manuales](README.md)

---

## Sobre este manual

Este manual explica, **paso a paso y en lenguaje cotidiano del taller**, cómo usar *Taller Pro* según el rol de cada persona. No hace falta saber de programación: todas las acciones se describen por el nombre exacto del botón o campo que verá en pantalla.

### ¿A quién está dirigido?

| Rol | Badge | ¿Qué encontrará en este manual? |
|---|---|---|
| **Administrador** | 🛡️ | Control total: catálogos, usuarios y permisos, datos de la empresa, respaldos y todos los reportes. Ver **Capítulo 1**. |
| **Recepcionista** | 📋 | Atención al cliente: alta de clientes y vehículos, generación de órdenes de servicio y cobro. Ver **Capítulo 2**. |
| **Mecánico** | 🔧 | Trabajo técnico: sus tareas asignadas en la pantalla *Mis Trabajos*. Ver **Capítulo 3**. |

### ¿Cómo usar este manual?

- El **Capítulo 0 (Primeros pasos)** es común a todos los roles: conviene leerlo una vez.
- Luego salte directamente al **capítulo de su rol**.
- Cada procedimiento sigue siempre el mismo formato: 🎯 Objetivo · 👥 Quién puede · 📋 Paso a paso · 📸 Captura · 💡 Consejos · ⚠️ Qué hacer en caso de error.
- Los cuadros `> [!TIP]` indican atajos o buenas prácticas; los cuadros `> [!WARNING]` indican acciones que conviene pensar antes de hacer.
- Las imágenes llevan **marcadores de captura** (`> 📸 CAPTURA REQUERIDA:`): indican qué pantalla fotografiar, en qué estado y qué resaltar. Las capturas reales se agregan luego en `docs/manuales/assets/roles/`.

### Índice

- [Capítulo 0 — Primeros Pasos y Navegación General](#capítulo-0--primeros-pasos-y-navegación-general)
- [Capítulo 1 — Perfil Administrador 🛡️](#capítulo-1--perfil-administrador-️)
- [Capítulo 2 — Perfil Recepcionista 📋](#capítulo-2--perfil-recepcionista-)
- [Capítulo 3 — Perfil Mecánico 🔧](#capítulo-3--perfil-mecánico-)
- [Capítulo 4 — Dudas Frecuentes, Estados y Glosario](#capítulo-4--dudas-frecuentes-estados-y-glosario)

---

# Capítulo 0 — Primeros Pasos y Navegación General

Este capítulo aplica a **todos los roles**. Describe cómo iniciar sesión, cómo está organizada la pantalla y qué significan los colores y avisos que verá en todo el sistema.

## 0.1 Iniciar sesión (`CtlLogin`)

> [!NOTE]
> El sistema no permite el auto-registro: los usuarios son creados por un Administrador (ver [1.9 Usuarios](#19-gestionar-usuarios-ctlusuario--frmusuario)). Si aún no tiene usuario, solicítelo.

### 🎯 Objetivo
Ingresar a *Taller Pro* con su usuario y contraseña.

### 👥 Quién puede realizarlo
Cualquier persona con un usuario habilitado (Administrador, Recepcionista o Mecánico).

### 📋 Paso a paso
1. Abra la aplicación. Se mostrará la ventana **TALLER PRO — Control Vehicular - Iniciar Sesión**.
2. En el campo **DNI**, escriba su documento de identidad (es su nombre de usuario).
3. En el campo **Contraseña**, escriba su clave.
   - Si necesita verificar lo que escribió, haga clic en el ícono del ojo **👁️** a la derecha del campo para mostrar u ocultar la contraseña.
4. Haga clic en el botón azul **`Iniciar Sesión`**.
5. Si los datos son correctos, se abrirá el **Dashboard** y verá el menú lateral con los módulos habilitados para su rol.

**Resultado esperado:** la ventana de login se cierra y aparece la pantalla principal con su nombre de usuario y su rol en la esquina inferior izquierda.

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/01-login.png`
> - **Pantalla / Diálogo:** CtlLogin — Pantalla de inicio de sesión.
> - **Estado recomendado:** Campos "DNI" y "Contraseña" visibles, con el botón "Iniciar Sesión" en primer plano (no hace falta cargar credenciales reales).
> - **Elementos a resaltar:** Recuadrar el botón verde `Iniciar Sesión`, el ícono del ojo `👁️` y el campo `DNI`.
> - **Epígrafe:** *Figura 0.1: Pantalla de inicio de sesión con los campos DNI, Contraseña y el botón Iniciar Sesión.*

![Figura 0.1: Pantalla de inicio de sesión.](assets/roles/01-login.png)

### 💡 Consejos y alertas
> [!TIP]
> Cuando ingrese su DNI, hágalo **sin puntos ni espacios** (por ejemplo `30123456`). Es el mismo documento que registró el Administrador.

### ⚠️ Qué hacer en caso de error
| Situación | Qué ocurre en pantalla | Solución |
|---|---|---|
| DNI o contraseña incorrectos | Aparece un aviso en rojo con el ícono **⚠️** | Verifique mayúsculas/minúsculas y vuelva a escribir la clave. |
| El usuario está **inhabilitado** | El sistema rechaza el ingreso indicando credenciales inválidas | Solicite al Administrador que vuelva a habilitar su usuario. |
| Olvidó la contraseña | No hay recuperación automática | Pida al Administrador que use **`🔑 Setear / Cambiar Clave`** en *Usuarios*. |
| El campo quedó con borde rojo | El sistema valida mientras escribe | Complete los campos obligatorios antes de presionar `Iniciar Sesión`. |

## 0.2 La estructura de la interfaz y el menú lateral

Una vez dentro, la pantalla tiene siempre la misma estructura:

- **Menú lateral izquierdo (barra oscura):** es el menú de navegación. En la parte superior dice **🛠️ TALLER PRO**.
- **Tarjeta de sesión (debajo del logo):** muestra **👤 Usuario** (su nombre) y **🛡️ Rol: -** (el rol con el que ingresó).
- **Zona central:** aquí se abre cada módulo que usted elige (Dashboard, catálogos, órdenes, etc.).
- **Botón de cerrar la aplicación `✖`** en el extremo superior derecho (color rojo).
- **Mensaje flotante verde (toast):** cuando una operación se realiza con éxito aparece un aviso verde "Operación exitosa" que se desvanece solo.

### El menú lateral es dinámico

El menú **no muestra todos los módulos**: solo aparecen los que su rol tiene permitidos. El menú se agrupa en secciones que se titulan **OPERACIONES**, **ADMINISTRACIÓN** y **REPORTES**. Si un rol no tiene ningún permiso dentro de una sección, la sección entera desaparece automáticamente.

| Módulo (menú) | Ícono | Permiso que lo habilita |
|---|---|---|
| Dashboard | 📊 | Siempre visible |
| Órdenes de Trabajo | 📋 | `OrdenServicio.Ver` |
| Mis Trabajos | 🔧 | `MisTrabajos.Ver` (solo Mecánico) |
| Servicios | 📦 | `Servicio.Ver` |
| Clientes | 👤 | `Cliente.Ver` |
| Vehículos | 🚗 | `Vehiculo.Ver` |
| Modelos de Autos | 🗂️ | `Vehiculo.Ver` |
| Marcas | 🏷️ | `Vehiculo.Ver` |
| Talleres | 🔧 | `Taller.Ver` |
| Usuarios | 🔑 | `Usuario.Ver` |
| Roles | 🛡️ | `Usuario.Ver` |
| Configuración de Empresa | 🏢 | `Configuracion.Empresa.Gestionar` |
| Respaldos | 💾 | `Configuracion.Backup.Gestionar` |
| Configuración de Respaldos | 🗄️ | `Configuracion.Backup.Gestionar` |
| Reporte Órdenes | 📋 | `Reporte.Ordenes.Ver` |
| Reporte Gerencial | 📈 | `Reporte.Gerencial.Ver` |
| Reporte Operativo | 🚙 | `Reporte.Operativo.Ver` |
| Cerrar Sesión | ❌ | Siempre visible |

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/02-menu-lateral.png`
> - **Pantalla / Diálogo:** MainWindow — Menú lateral desplegado, con un rol con varios módulos visibles.
> - **Estado recomendado:** Sesión iniciada como Administrador, con la tarjeta de sesión (nombre y rol) y las secciones OPERACIONES / ADMINISTRACIÓN / REPORTES visibles.
> - **Elementos a resaltar:** Recuadrar la tarjeta de sesión (`👤 Usuario` / `🛡️ Rol`), los títulos de sección y el botón `❌ Cerrar Sesión`.
> - **Epígrafe:** *Figura 0.2: Menú lateral dinámico con los módulos habilitados según el rol.*

![Figura 0.2: Menú lateral dinámico según el rol.](assets/roles/02-menu-lateral.png)

### 💡 Consejos y alertas
> [!TIP]
> Si un módulo que necesita **no aparece** en el menú, es porque su rol no tiene el permiso correspondiente. No es un error de la aplicación: pídale al Administrador que lo habilite (ver [1.10 Roles y permisos](#110-roles-y-permisos-ctlrol--frmrol--frmrolpermiso)).

## 0.3 El Dashboard General (`CtlDashboard`)

El **Dashboard** es la primera pantalla que ve al ingresar (y también la que se abre al presionar **📊 Dashboard**). Muestra un resumen del taller en tiempo real.

### 🎯 Objetivo
Tener una vista rápida del estado del taller: vehículos presentes, órdenes activas, entregas del día e ingresos.

### 👥 Quién puede realizarlo
Todos los roles. **La única diferencia:** la tarjeta **Ingresos del Mes** y el alcance de los datos dependen del rol (ver detalle más abajo).

### 📋 Paso a paso
1. Presione **📊 Dashboard** en el menú lateral (o inicie sesión).
2. Observe las tarjetas de resumen:
   - **Vehículos Activos** — *"Con orden activa en el taller"*: cantidad de vehículos que hoy están en el taller.
   - **En Proceso** — *"Órdenes de trabajo activas"*: órdenes que están abiertas o en proceso.
   - **Entregadas Hoy** — *"Órdenes finalizadas"*: vehículos entregados en el día.
   - **Ingresos del Mes** — *"Facturado del mes a la fecha"*: **solo visible para el Administrador**.
3. Desplace hacia abajo hasta la grilla **🚗 Vehículos en el Taller**, con las columnas **Patente · Marca · Modelo · Año · Cliente · Km Actual**.
4. Si necesita actualizar los datos, haga clic en **`🔄 Actualizar`**.

### Alcance de los datos según el rol

| Rol | Qué ve en el Dashboard |
|---|---|
| 🛡️ Administrador | Todos los vehículos y órdenes del taller + tarjeta **Ingresos del Mes**. |
| 📋 Recepcionista | Todos los vehículos y órdenes del taller (sin la tarjeta de ingresos). |
| 🔧 Mecánico | **Solo sus propias órdenes** asignadas. No ve la tarjeta de ingresos. |

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/03-dashboard-general.png`
> - **Pantalla / Diálogo:** CtlDashboard — Dashboard General.
> - **Estado recomendado:** Datos de ejemplo cargados (varios vehículos en la grilla) y las tarjetas de resumen con valores.
> - **Elementos a resaltar:** Recuadrar las tarjetas "Vehículos Activos", "En Proceso", "Entregadas Hoy" y el botón `🔄 Actualizar`.
> - **Epígrafe:** *Figura 0.3: Dashboard General con las tarjetas de resumen y la grilla de vehículos en el taller.*

![Figura 0.3: Dashboard General.](assets/roles/03-dashboard-general.png)

### 💡 Consejos y alertas
> [!TIP]
> El Dashboard se calcula al abrirlo. Si acaba de cargar una orden y no ve el cambio, presione **`🔄 Actualizar`**.

## 0.4 Convenciones visuales: botones, colores y confirmaciones

Todo el sistema respeta los mismos colores y avisos. Aprenderlos una vez le permite reconocer cualquier botón en cualquier pantalla.

### Colores estándar de los botones

| Botón | Color | Para qué sirve |
|---|---|---|
| **➕ Nuevo…** | Verde `#27AE60` | Crear un registro nuevo. |
| **✏️ Editar…** | Naranja `#F39C12` | Modificar el registro seleccionado. |
| **🗑️ Eliminar / ❌ Cancelar** | Rojo `#C0392B` | Eliminar o cancelar un registro; en formularios, cancelar la edición. |
| **🔄 Actualizar / 🔎 Buscar** | Azul `#2980B9` | Recargar la lista o ejecutar una búsqueda. |

Además: el botón **Guardar / Aceptar** de los formularios es **verde** y el de **Cancelar** es **rojo**.

### Confirmaciones y avisos (no use los carteles de Windows)

*Taller Pro* **no usa** los carteles nativos de Windows (`MessageBox`). Todas las confirmaciones y avisos se muestran con una ventana propia con encabezado oscuro:

- **Confirmación Sí/No** (`FrmConfirmacion.Mostrar`): se usa antes de eliminar o cancelar algo. Título **"Confirmar Acción"**, ícono **⚠️**, texto *"¿Está seguro de realizar esta acción?"* y botones:
  - **`✓ Sí, Aceptar`** (verde) para confirmar.
  - **`❌ No, Cancelar`** (rojo) para volver atrás.
- **Aviso de un solo botón** (`FrmConfirmacion.MostrarAviso`): se usa para alertas de selección, por ejemplo *"Por favor seleccione un registro..."*. Muestra un único botón **`✓ Aceptar`** (verde).

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/04-confirmacion.png`
> - **Pantalla / Diálogo:** FrmConfirmacion — Ventana de confirmación Sí/No.
> - **Estado recomendado:** Diálogo abierto sobre una pantalla de listado (por ejemplo, confirmando la eliminación de un servicio).
> - **Elementos a resaltar:** Recuadrar los botones `✓ Sí, Aceptar` (verde) y `❌ No, Cancelar` (rojo) y el ícono `⚠️`.
> - **Epígrafe:** *Figura 0.4: Ventana de confirmación personalizada con las opciones Sí, Aceptar / No, Cancelar.*

![Figura 0.4: Confirmación personalizada del sistema.](assets/roles/04-confirmacion.png)

### ⚠️ Qué hacer en caso de error
| Situación | Qué ocurre en pantalla | Solución |
|---|---|---|
| Presionó una acción sin seleccionar fila | Aparece el aviso *"Por favor seleccione un registro..."* | Haga clic sobre la fila de la grilla y repita la acción. |
| Se equivocó de botón | Se abre la ventana de confirmación | Presione **`❌ No, Cancelar`**. |

## 0.5 Cerrar sesión

### 🎯 Objetivo
Salir de la aplicación de forma ordenada, liberando el usuario y la sesión actual.

### 👥 Quién puede realizarlo
Todos los roles.

### 📋 Paso a paso
1. En el menú lateral, baje hasta el botón **`❌ Cerrar Sesión`**.
2. Haga clic en él.
3. La aplicación regresa a la pantalla de inicio de sesión (**TALLER PRO**).

### 💡 Consejos y alertas
> [!WARNING]
> Si **cierra la aplicación** con el botón **`✖`** de la esquina superior derecha, la sesión también finaliza, pero hágalo solo cuando haya terminado de guardar cambios. Para cambiar de usuario, es preferible usar **`❌ Cerrar Sesión`**.

---

# Capítulo 1 — Perfil Administrador 🛡️

Este capítulo está dirigido a quien administra el taller: **datos de la empresa, catálogos base, usuarios y permisos, respaldos y reportes**. El Administrador ve **todos los módulos** excepto *Mis Trabajos* (esa pantalla es exclusiva del Mecánico).

> [!IMPORTANT]
> **Dos cosas que suelen confundirse y conviene dejar claras:**
> - **🔧 Talleres** (`CtlTaller`) es el **alta, baja y modificación (ABM) de los talleres/sucursales** registrados en el sistema (la aplicación admite varios talleres).
> - **🏢 Configuración de Empresa** (`CtlConfiguracion`) es donde se cargan **los datos institucionales que salen en los reportes**: nombre de la empresa, dirección, teléfono, email y **logo**. No confunda estos dos módulos.

## 1.1 Dashboard Gerencial con "Ingresos del Mes"

### 🎯 Objetivo
Ver el resumen de gestión con el dato de **facturación mensual**, exclusivo del Administrador.

### 👥 Quién puede realizarlo
Solo **Administrador**.

### 📋 Paso a paso
1. Abra **📊 Dashboard**.
2. Observe las cuatro tarjetas. La cuarta, **Ingresos del Mes** (*"Facturado del mes a la fecha"*), se calcula con el procedimiento `sp_ReporteIngresosAdmin` y **solo la ve el Administrador**.
3. Revise la grilla **🚗 Vehículos en el Taller** para ver qué hay en el taller ahora mismo.
4. Use **`🔄 Actualizar`** para refrescar.

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/05-dashboard-admin.png`
> - **Pantalla / Diálogo:** CtlDashboard — Dashboard con la tarjeta exclusiva "Ingresos del Mes".
> - **Estado recomendado:** Sesión como Administrador, con las cuatro tarjetas y sus valores.
> - **Elementos a resaltar:** Recuadrar la tarjeta **"Ingresos del Mes"** para destacar que es exclusiva del Administrador.
> - **Epígrafe:** *Figura 1.1: Dashboard del Administrador, con la tarjeta exclusiva de Ingresos del Mes.*

![Figura 1.1: Dashboard del Administrador con Ingresos del Mes.](assets/roles/05-dashboard-admin.png)

### 💡 Consejos y alertas
> [!TIP]
> Para un análisis por período (rango de fechas y detalle), use el **📈 Reporte Gerencial** (ver [1.14](#114-reporte-gerencial-ctlreportegerencial)).

## 1.2 Configurar los datos de la empresa (`CtlConfiguracion`)

### 🎯 Objetivo
Cargar la identidad del taller (**nombre, dirección, teléfono, email y logo**) que se imprime automáticamente en **todos los reportes PDF**.

### 👥 Quién puede realizarlo
Solo **Administrador** (permiso `Configuracion.Empresa.Gestionar`).

### 📋 Paso a paso
1. En el menú lateral, dentro de **ADMINISTRACIÓN**, haga clic en **🏢 Configuración de Empresa**.
2. Se abrirá la pantalla **🏢 Configuración de Empresa** con el subtítulo *"Gestiona la identidad del taller (logo y datos para reportes)"*.
3. Complete los campos:
   - **Nombre de la Empresa:** (obligatorio).
   - **Dirección:** (obligatorio).
   - **Teléfono:** (obligatorio).
   - **Email (opcional):** puede dejarlo vacío.
   - **Logo:** presione **`📂 Seleccionar logo`** y elija un archivo de imagen. Verá una vista previa y la nota *"El logo se guardará en caché local y se usará en todos los reportes PDF."*
4. Haga clic en el botón verde **`💾 Guardar`**.
5. Espere el mensaje *"Guardando..."* y luego el aviso verde de éxito.

**Resultado esperado:** los datos y el logo quedan guardados y aparecerán en los encabezados de los reportes.

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/06-configuracion-empresa.png`
> - **Pantalla / Diálogo:** CtlConfiguracion — "🏢 Configuración de Empresa".
> - **Estado recomendado:** Formulario con Nombre, Dirección y Teléfono cargados y un logo seleccionado en vista previa.
> - **Elementos a resaltar:** Recuadrar el botón `📂 Seleccionar logo`, el botón verde `💾 Guardar` y el campo `Nombre de la Empresa:`.
> - **Epígrafe:** *Figura 1.2: Configuración de Empresa, donde se cargan los datos y el logo para los reportes.*

![Figura 1.2: Configuración de Empresa.](assets/roles/06-configuracion-empresa.png)

### ⚠️ Qué hacer en caso de error
| Situación | Qué ocurre en pantalla | Solución |
|---|---|---|
| Deja un campo obligatorio vacío | El campo se marca en rojo con un mensaje debajo | Complete Nombre, Dirección y Teléfono. |
| El logo no aparece en los reportes | Se guardó mal o el archivo no es imagen | Vuelva a presionar `📂 Seleccionar logo`, elija un `.png`/`.jpg` y guarde de nuevo. |

## 1.3 Catálogo de Servicios (`CtlServicio` / `FrmServicio`)

### 🎯 Objetivo
Mantener el catálogo de trabajos que se pueden cargar en una orden (mano de obra) con su **precio base**.

### 👥 Quién puede realizarlo
Administrador y Recepcionista (permiso `Servicio.Ver`).

### 📋 Paso a paso — Consultar y buscar
1. Haga clic en **📦 Servicios**.
2. Se abre **📦 Catálogo de Servicios** con la grilla de columnas **ID · Nombre del Servicio · Precio Base ($) · Activo**.
3. Para buscar, escriba en el campo de búsqueda y presione **`🔎 Buscar`**. Para recargar todo, presione **`🔄 Actualizar`**.

### 📋 Paso a paso — Crear un servicio
1. Presione el botón verde **`➕ Nuevo Servicio`**.
2. En la ventana **Datos del Servicio**, complete:
   - **Nombre:** del servicio (por ejemplo, *"Cambio de aceite y filtro"*).
   - **Precio ($):** importe base.
   - **Estado:** y, si desea, marque la casilla **"Activo en catálogo"**.
3. Presione **`Guardar`** (verde). Para descartar, **`Cancelar`** (rojo).
4. El servicio aparecerá en la grilla y podrá seleccionarse al cargar ítems en una orden.

### 📋 Paso a paso — Editar o eliminar
1. Haga clic sobre la fila del servicio en la grilla.
2. Para modificar, presione **`✏️ Editar`**; cambie los datos y **`Guardar`**.
3. Para eliminar, presione **`🗑️ Eliminar`** y confirme con **`✓ Sí, Aceptar`**.
   - Si el servicio ya está en uso en órdenes, es preferible **desactivarlo** (destildar "Activo en catálogo") antes que eliminarlo.

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/07-servicios-listado.png`
> - **Pantalla / Diálogo:** CtlServicio — "📦 Catálogo de Servicios".
> - **Estado recomendado:** Grilla con varios servicios y precios cargados.
> - **Elementos a resaltar:** Recuadrar el botón verde `➕ Nuevo Servicio` y las columnas "Nombre del Servicio" y "Precio Base ($)".
> - **Epígrafe:** *Figura 1.3: Catálogo de Servicios con el listado de trabajos y sus precios base.*

![Figura 1.3: Catálogo de Servicios.](assets/roles/07-servicios-listado.png)

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/08-servicio-form.png`
> - **Pantalla / Diálogo:** FrmServicio — "Datos del Servicio".
> - **Estado recomendado:** Formulario con Nombre y Precio cargados y la casilla "Activo en catálogo" marcada.
> - **Elementos a resaltar:** Recuadrar los campos `Nombre:`, `Precio ($):` y el botón verde `Guardar`.
> - **Epígrafe:** *Figura 1.4: Formulario de alta/edición de un servicio.*

![Figura 1.4: Formulario de Servicio.](assets/roles/08-servicio-form.png)

### ⚠️ Qué hacer en caso de error
| Situación | Qué ocurre en pantalla | Solución |
|---|---|---|
| Precio con letras o vacío | El campo se marca en rojo | Escriba solo números (use punto o coma decimal según su configuración). |
| No aparece el servicio en una orden | Está desactivado | Edítelo y marque "Activo en catálogo". |

## 1.4 Clientes (`CtlCliente` / `FrmCliente`)

### 🎯 Objetivo
Registrar y mantener los datos de los clientes del taller.

### 👥 Quién puede realizarlo
Administrador, Recepcionista y Mecánico (permiso `Cliente.Ver`).

### 📋 Paso a paso — Consultar y buscar
1. Haga clic en **👤 Clientes**.
2. Se abre **👤 Control de Clientes**, con la grilla de columnas **ID · Nombre · Apellido · DNI · Fecha Nac. · Dirección · Email · Teléfono · Activo**.
3. Escriba el nombre, apellido, DNI o teléfono y presione **`Buscar`**.

### 📋 Paso a paso — Crear un cliente
1. Presione el botón verde **`➕ Nuevo Cliente`**.
2. En **Datos del Cliente** complete: **Nombre:, Apellido:, DNI:, Fecha Nac.:, Dirección:, Email:, Teléfono:, Estado:**.
3. Presione **`Guardar`** (verde). Los mensajes de validación aparecen en rojo debajo del campo.
4. El cliente queda disponible para asociarle vehículos y órdenes.

### 📋 Paso a paso — Editar o eliminar
1. Seleccione la fila del cliente.
2. **`✏️ Editar Seleccionado`** para modificar; **`🗑️ Eliminar Seleccionado`** para borrar (con confirmación).

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/09-clientes-listado.png`
> - **Pantalla / Diálogo:** CtlCliente — "👤 Control de Clientes".
> - **Estado recomendado:** Grilla con varios clientes cargados y el campo de búsqueda con un texto.
> - **Elementos a resaltar:** Recuadrar el botón verde `➕ Nuevo Cliente`, el botón azul `Buscar` y la columna `DNI`.
> - **Epígrafe:** *Figura 1.5: Control de Clientes con la búsqueda y el alta de clientes.*

![Figura 1.5: Control de Clientes.](assets/roles/09-clientes-listado.png)

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/10-cliente-form.png`
> - **Pantalla / Diálogo:** FrmCliente — "Datos del Cliente".
> - **Estado recomendado:** Formulario con los datos personales completos y un ejemplo de error de validación en rojo (opcional).
> - **Elementos a resaltar:** Recuadrar los campos `DNI:` y `Teléfono:` y el botón verde `Guardar`.
> - **Epígrafe:** *Figura 1.6: Formulario de alta/edición de un cliente.*

![Figura 1.6: Formulario de Cliente.](assets/roles/10-cliente-form.png)

### ⚠️ Qué hacer en caso de error
| Situación | Qué ocurre en pantalla | Solución |
|---|---|---|
| DNI duplicado | El sistema rechaza el guardado | Busque el cliente existente y edítelo. |
| Formato de DNI inválido | Borde rojo y mensaje | Escriba solo números, sin puntos. |
| Email con formato inválido | Borde rojo y mensaje | Corrija el formato (`usuario@dominio.com`) o déjelo vacío. |

## 1.5 Vehículos (`CtlVehiculo` / `FrmVehiculo`)

### 🎯 Objetivo
Registrar y mantener el padrón de vehículos, vinculados a un cliente, una marca y un modelo.

### 👥 Quién puede realizarlo
Administrador, Recepcionista y Mecánico (permiso `Vehiculo.Ver`).

### 📋 Paso a paso
1. Haga clic en **🚗 Vehículos**.
2. Se abre **🚗 Control de Vehículos** con las columnas **ID · Patente · Cliente · Marca · Modelo · Año · Km Actual**.
3. Para crear, presione **`➕ Nuevo Vehículo`**.
4. En **Datos del Vehículo** complete: **Patente:, Cliente:, Marca:, Modelo:, Año:, Km Actual:.**
   - Si la marca o el modelo no existen, use los botones **`➕`** que están junto a esos desplegables para crearlos sin salir del formulario.
5. Presione **`Guardar`**.
6. Seleccione una fila para **`✏️ Editar Seleccionado`** o **`🗑️ Eliminar Seleccionado`**.

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/11-vehiculos-listado.png`
> - **Pantalla / Diálogo:** CtlVehiculo — "🚗 Control de Vehículos".
> - **Estado recomendado:** Grilla con varios vehículos (patente, marca, modelo, cliente, km).
> - **Elementos a resaltar:** Recuadrar el botón verde `➕ Nuevo Vehículo` y las columnas `Patente` y `Km Actual`.
> - **Epígrafe:** *Figura 1.7: Control de Vehículos con el padrón y su kilometraje.*

![Figura 1.7: Control de Vehículos.](assets/roles/11-vehiculos-listado.png)

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/12-vehiculo-form.png`
> - **Pantalla / Diálogo:** FrmVehiculo — "Datos del Vehículo".
> - **Estado recomendado:** Formulario con cliente, marca, modelo, año y km cargados.
> - **Elementos a resaltar:** Recuadrar los botones `➕` para agregar marca/modelo y el botón verde `Guardar`.
> - **Epígrafe:** *Figura 1.8: Formulario de alta/edición de un vehículo.*

![Figura 1.8: Formulario de Vehículo.](assets/roles/12-vehiculo-form.png)

### 💡 Consejos y alertas
> [!TIP]
> En la práctica, la mayoría de los vehículos se dan de alta desde el flujo de la orden con el botón **`+ Rápido`** (por patente), que además transfiere la titularidad al cliente correcto. Ver [2.5](#25-flujo-principal-generar-una-orden-de-servicio).

### ⚠️ Qué hacer en caso de error
| Situación | Qué ocurre en pantalla | Solución |
|---|---|---|
| Patente ya registrada | El sistema lo indica al guardar | Búsquela y edítela, o use `+ Rápido` en la orden para reasignarla. |
| Marca/modelo no existe | No aparece en el desplegable | Use los botones `➕` junto al campo para crearlos. |
| Km con formato inválido | Borde rojo | Escriba solo números enteros. |

## 1.6 Modelos de Autos (`CtlModelo` / `FrmModelo`)

### 🎯 Objetivo
Mantener el catálogo de modelos, cada uno asociado a una marca.

### 👥 Quién puede realizarlo
Administrador, Recepcionista y Mecánico (permiso `Vehiculo.Ver`).

### 📋 Paso a paso
1. Haga clic en **🗂️ Modelos de Autos**.
2. Se abre **🏷️ Control de Modelos** con las columnas **ID · Nombre Modelo · Marca**.
3. Presione **`➕ Nuevo Modelo`** y en **Datos del Modelo** complete **Nombre Modelo:** y **Marca:**.
4. **`Guardar`** (verde). Para modificar o borrar, seleccione la fila y use **`✏️ Editar Seleccionado`** o **`🗑️ Eliminar Seleccionado`**.

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/13-modelos-listado.png`
> - **Pantalla / Diálogo:** CtlModelo — "🏷️ Control de Modelos".
> - **Estado recomendado:** Grilla con varios modelos y su marca.
> - **Elementos a resaltar:** Recuadrar el botón verde `➕ Nuevo Modelo` y la columna `Marca`.
> - **Epígrafe:** *Figura 1.9: Control de Modelos asociados a cada marca.*

![Figura 1.9: Control de Modelos.](assets/roles/13-modelos-listado.png)

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/14-modelo-form.png`
> - **Pantalla / Diálogo:** FrmModelo — "Datos del Modelo".
> - **Estado recomendado:** Formulario con Nombre Modelo y Marca seleccionada.
> - **Elementos a resaltar:** Recuadrar los campos `Nombre Modelo:` y `Marca:` y el botón `Guardar`.
> - **Epígrafe:** *Figura 1.10: Formulario de alta/edición de un modelo.*

![Figura 1.10: Formulario de Modelo.](assets/roles/14-modelo-form.png)

## 1.7 Marcas (`CtlMarca` / `FrmMarca`)

### 🎯 Objetivo
Mantener el catálogo de marcas de vehículos.

### 👥 Quién puede realizarlo
Administrador, Recepcionista y Mecánico (permiso `Vehiculo.Ver`).

### 📋 Paso a paso
1. Haga clic en **🏷️ Marcas**.
2. Se abre **🏢 Control de Marcas**, con la columna **ID · Nombre de Marca**.
3. Presione **`➕ Nueva Marca`**, escriba **Nombre Marca:** y presione **`Guardar`**.
4. Para modificar o borrar, seleccione la fila y use **`✏️ Editar Seleccionada`** o **`🗑️ Eliminar Seleccionada`**.

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/15-marcas-listado.png`
> - **Pantalla / Diálogo:** CtlMarca — "🏢 Control de Marcas".
> - **Estado recomendado:** Grilla con varias marcas cargadas.
> - **Elementos a resaltar:** Recuadrar el botón verde `➕ Nueva Marca`.
> - **Epígrafe:** *Figura 1.11: Control de Marcas del catálogo base.*

![Figura 1.11: Control de Marcas.](assets/roles/15-marcas-listado.png)

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/16-marca-form.png`
> - **Pantalla / Diálogo:** FrmMarca — "Datos de la Marca".
> - **Estado recomendado:** Formulario con el Nombre Marca cargado.
> - **Elementos a resaltar:** Recuadrar el campo `Nombre Marca:` y el botón `Guardar`.
> - **Epígrafe:** *Figura 1.12: Formulario de alta/edición de una marca.*

![Figura 1.12: Formulario de Marca.](assets/roles/16-marca-form.png)

## 1.8 Talleres (`CtlTaller` / `FrmTaller`)

### 🎯 Objetivo
Administrar los **talleres/sucursales** dados de alta en el sistema (la aplicación admite varios talleres).

### 👥 Quién puede realizarlo
Solo **Administrador** (permiso `Taller.Ver`).

### 📋 Paso a paso
1. Haga clic en **🔧 Talleres**.
2. Se abre **🔧 Control de Taller** con las columnas **ID · Nombre · Dirección · Teléfono · Activo**.
3. Presione **`➕ Nuevo Taller`** y en **Datos del Taller** complete **Nombre:, Dirección:, Teléfono:**.
4. **`Guardar`** (verde).
5. Seleccione una fila para **`✏️ Editar Seleccionado`** o **`🗑️ Eliminar Seleccionado`**. Use **`Buscar`** para localizar un taller.

> [!WARNING]
> **No** cargue aquí los datos institucionales de la empresa (nombre comercial, logo, etc.). Eso se hace en **🏢 Configuración de Empresa** ([1.2](#12-configurar-los-datos-de-la-empresa-ctlconfiguracion)). Este módulo es solo el ABM de talleres.

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/17-talleres-listado.png`
> - **Pantalla / Diálogo:** CtlTaller — "🔧 Control de Taller".
> - **Estado recomendado:** Grilla con al menos un taller cargado.
> - **Elementos a resaltar:** Recuadrar el botón verde `➕ Nuevo Taller` y la columna `Activo`.
> - **Epígrafe:** *Figura 1.13: Control de Talleres (ABM de sucursales del sistema).*

![Figura 1.13: Control de Talleres.](assets/roles/17-talleres-listado.png)

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/18-taller-form.png`
> - **Pantalla / Diálogo:** FrmTaller — "Datos del Taller".
> - **Estado recomendado:** Formulario con Nombre, Dirección y Teléfono cargados.
> - **Elementos a resaltar:** Recuadrar los campos `Nombre:` y `Teléfono:` y el botón verde `Guardar`.
> - **Epígrafe:** *Figura 1.14: Formulario de alta/edición de un taller.*

![Figura 1.14: Formulario de Taller.](assets/roles/18-taller-form.png)

## 1.9 Gestionar Usuarios (`CtlUsuario` / `FrmUsuario`)

### 🎯 Objetivo
Dar de alta operadores, asignarles un **rol**, cambiar su estado y resetear contraseñas.

### 👥 Quién puede realizarlo
Solo **Administrador** (permiso `Usuario.Ver`).

### 📋 Paso a paso — Crear un usuario
1. Haga clic en **🔑 Usuarios**.
2. Se abre **🔑 Gestión de Usuarios** con las columnas **ID · Nombre · Apellido · DNI · Email · Rol · Estado**.
3. Presione el botón verde **`➕ Nuevo Usuario`**.
4. En **Datos del Usuario** complete: **Nombre:, Apellido:, DNI:, Email:, Teléfono:, Fecha Nac.:, Domicilio:, Contraseña:, Rol:, Estado:**.
5. Asigne la **contraseña** de una de estas dos formas:
   - Escribirla en el campo **Contraseña:** y usar el ícono **`👁️`** para verificarla, o
   - Presionar **`🎲 Generar`** para que el sistema proponga una, o
   - Presionar **`🔑 Setear / Cambiar Clave`** para fijarla.
6. Seleccione el **Rol:** en el desplegable. Si necesita un rol que no existe, use el botón **`➕`** para agregarlo.
7. Presione **`Guardar`** (verde).

### 📋 Paso a paso — Modificar, cambiar estado o resetear clave
1. Seleccione la fila del usuario.
2. **`✏️ Modificar Seleccionado`** para editar sus datos.
3. **`🔄 Cambiar Estado`** para habilitarlo o inhabilitarlo (un usuario inhabilitado no puede iniciar sesión).
4. Para resetear la contraseña, abra el usuario y use **`🔑 Setear / Cambiar Clave`**.

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/19-usuarios-listado.png`
> - **Pantalla / Diálogo:** CtlUsuario — "🔑 Gestión de Usuarios".
> - **Estado recomendado:** Grilla con varios usuarios y distintos roles y estados.
> - **Elementos a resaltar:** Recuadrar las columnas `Rol` y `Estado` y el botón `🔄 Cambiar Estado`.
> - **Epígrafe:** *Figura 1.15: Gestión de Usuarios con su rol y estado.*

![Figura 1.15: Gestión de Usuarios.](assets/roles/19-usuarios-listado.png)

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/20-usuario-form.png`
> - **Pantalla / Diálogo:** FrmUsuario — "Datos del Usuario".
> - **Estado recomendado:** Formulario con datos cargados, rol seleccionado y la zona de contraseña visible.
> - **Elementos a resaltar:** Recuadrar los botones `🔑 Setear / Cambiar Clave`, `🎲 Generar` y el desplegable `Rol:`.
> - **Epígrafe:** *Figura 1.16: Formulario de alta/edición de un usuario con gestión de clave y rol.*

![Figura 1.16: Formulario de Usuario.](assets/roles/20-usuario-form.png)

### 💡 Consejos y alertas
> [!WARNING]
> Al **inhabilitar** un usuario, este no podrá iniciar sesión, pero su historial de trabajo se conserva. Para dar de baja temporal a un empleado, prefiera cambiar estado antes que eliminar.

### ⚠️ Qué hacer en caso de error
| Situación | Qué ocurre en pantalla | Solución |
|---|---|---|
| DNI duplicado | El sistema rechaza el guardado | Busque el usuario existente. |
| El usuario nuevo no puede entrar | Quedó inhabilitado | Use `🔄 Cambiar Estado`. |
| No se ve el rol deseado | No existe el rol | Créelo en **Roles** ([1.10](#110-roles-y-permisos-ctlrol--frmrol--frmrolpermiso)). |

## 1.10 Roles y permisos (`CtlRol` / `FrmRol` / `FrmRolPermiso`)

### 🎯 Objetivo
Crear roles y definir **qué módulos ve y usa** cada uno. Es lo que determina el menú lateral de cada usuario.

### 👥 Quién puede realizarlo
Solo **Administrador** (permiso `Usuario.Ver`).

### 📋 Paso a paso — Crear un rol
1. Haga clic en **🛡️ Roles**.
2. Se abre **🛡️ Gestión de Roles** con las columnas **ID · Nombre · Descripción · Estado**.
3. Presione el botón verde **`➕ Nuevo Rol`**.
4. En **Datos del Rol** complete **Nombre:, Descripción:, Estado:** y presione **`Guardar`**.

### 📋 Paso a paso — Asignar permisos a un rol
1. Seleccione el rol en la grilla.
2. Presione **`🔑 Administrar Permisos`**.
3. Se abre **Permisos del Rol** (*"Seleccione los permisos permitidos"*) con la lista de permisos en casillas.
4. Marque los permisos que correspondan. Como referencia, esta es la distribución estándar de los tres roles base:

| Rol | Permisos incluidos |
|---|---|
| **Administrador** | Todo **excepto** `MisTrabajos.Ver`: `OrdenServicio.Ver`, `Servicio.Ver`, `Cliente.Ver`, `Vehiculo.Ver`, `Taller.Ver`, `Usuario.Ver`, `Configuracion.Empresa.Gestionar`, `Configuracion.Backup.Gestionar`, `Reporte.Ordenes.Ver`, `Reporte.Gerencial.Ver`, `Reporte.Operativo.Ver`. |
| **Recepcionista** | `OrdenServicio.Ver`, `Servicio.Ver`, `Cliente.Ver`, `Vehiculo.Ver`, `Reporte.Ordenes.Ver`, `Reporte.Operativo.Ver`. |
| **Mecánico** | `OrdenServicio.Ver`, `MisTrabajos.Ver`, `Servicio.Ver`, `Cliente.Ver`, `Vehiculo.Ver`. |

5. Guarde los cambios.

### 📋 Paso a paso — Modificar o cambiar estado
1. Seleccione el rol.
2. **`✏️ Modificar Seleccionado`** para editar nombre/descripción; **`🔄 Cambiar Estado`** para activarlo o desactivarlo.

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/21-roles-listado.png`
> - **Pantalla / Diálogo:** CtlRol — "🛡️ Gestión de Roles".
> - **Estado recomendado:** Grilla con los roles Administrador, Recepcionista y Mecánico.
> - **Elementos a resaltar:** Recuadrar el botón `🔑 Administrar Permisos` y el botón verde `➕ Nuevo Rol`.
> - **Epígrafe:** *Figura 1.17: Gestión de Roles con el botón para administrar permisos.*

![Figura 1.17: Gestión de Roles.](assets/roles/21-roles-listado.png)

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/22-rol-form.png`
> - **Pantalla / Diálogo:** FrmRol — "Datos del Rol".
> - **Estado recomendado:** Formulario con Nombre y Descripción cargados.
> - **Elementos a resaltar:** Recuadrar los campos `Nombre:` y `Descripción:` y el botón `Guardar`.
> - **Epígrafe:** *Figura 1.18: Formulario de alta/edición de un rol.*

![Figura 1.18: Formulario de Rol.](assets/roles/22-rol-form.png)

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/23-rol-permisos.png`
> - **Pantalla / Diálogo:** FrmRolPermiso — "Permisos del Rol".
> - **Estado recomendado:** Lista de permisos con varios marcados (por ejemplo, los del Recepcionista).
> - **Elementos a resaltar:** Recuadrar algunas casillas de permiso y el botón de guardado.
> - **Epígrafe:** *Figura 1.19: Asignación de permisos a un rol (RBAC).*

![Figura 1.19: Permisos del Rol.](assets/roles/23-rol-permisos.png)

### 💡 Consejos y alertas
> [!TIP]
> Un usuario puede acumular permisos de más de un rol. El menú lateral mostrará la **unión** de los permisos de sus roles.

## 1.11 Órdenes de Trabajo (`CtlOrdenServicio` / `CtlOrdenServicioForm`)

### 🎯 Objetivo
Ver, crear, editar y cancelar **órdenes de servicio**, y cobrarlas. Es la pantalla central del taller.

### 👥 Quién puede realizarlo
Administrador, Recepcionista y Mecánico (permiso `OrdenServicio.Ver`). El **cobro** y la **cancelación** se realizan según las reglas del taller.

### 📋 Paso a paso — Consultar y buscar
1. Haga clic en **📋 Órdenes de Trabajo**.
2. Se abre **📋 Control de Órdenes de Servicio** con las columnas **N° · Fecha · Cliente · Patente · Vehículo · Recepción · Km · Estado**.
3. Escriba en **🔍 Buscar:** y presione **`Buscar`** para filtrar (por cliente, patente, etc.).

### 📋 Paso a paso — Crear una orden
1. Presione el botón verde **`➕ Nueva Orden`**.
2. Se abre el formulario de la orden (ver el detalle completo en [2.5 Flujo de orden](#25-flujo-principal-generar-una-orden-de-servicio), que es igual para todos los roles).
3. Al terminar, presione **`Guardar`**.

### 📋 Paso a paso — Acciones sobre una orden seleccionada
Con la fila de la orden seleccionada, el bloque inferior permite:
- **`📄 Comp. Recepción`** — generar/imprimir el **comprobante de recepción** del vehículo.
- **`📄 Comp. Pago`** — generar/imprimir el **comprobante de pago**.
- **`💳 Pagar`** — abrir la ventana de cobro (ver [2.6](#26-cobrar-una-orden-frmpago)).
- **`✏️ Editar Seleccionada`** — abrir la orden para modificarla.
- **`🗑️ Eliminar / Cancelar`** — eliminar o **cancelar** la orden (con confirmación).

Si no hay resultados, verá el cartel *"No se encontraron órdenes de servicio"*.

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/24-ordenes-servicio.png`
> - **Pantalla / Diálogo:** CtlOrdenServicio — "📋 Control de Órdenes de Servicio".
> - **Estado recomendado:** Grilla con varias órdenes en distintos estados y el campo de búsqueda con texto.
> - **Elementos a resaltar:** Recuadrar el botón verde `➕ Nueva Orden` y los botones `💳 Pagar` y `🗑️ Eliminar / Cancelar`.
> - **Epígrafe:** *Figura 1.20: Control de Órdenes de Servicio con sus acciones principales.*

![Figura 1.20: Control de Órdenes de Servicio.](assets/roles/24-ordenes-servicio.png)

### 💡 Consejos y alertas
> [!WARNING]
> El botón **`🗑️ Eliminar / Cancelar`** es rojo a propósito: piense bien la acción. Si solo quiere anular una orden ya emitida, la acción correcta es **cancelarla** (pasa a estado *Cancelada*), no borrarla.

## 1.12 Respaldos de Base de Datos (`CtlBackup`)

### 🎯 Objetivo
Generar, verificar y resguardar copias de seguridad de la base de datos.

### 👥 Quién puede realizarlo
Solo **Administrador** (permiso `Configuracion.Backup.Gestionar`).

### 📋 Paso a paso
1. Haga clic en **💾 Respaldos**.
2. Se abre **💾 Respaldos de Base de Datos** (*"Genera, verifica y resguarda copias de seguridad de la base de datos"*), con la grilla de **Archivo · Fecha · Tamaño**.
3. Presione **`📀 Hacer Backup`** para generar una copia nueva.
4. Seleccione un respaldo y presione **`🧪 Probar Backup`** para verificar su integridad (verá *"Seleccione un respaldo y use «Probar Backup» para verificar su integridad."*).
5. Para subir la copia a la nube, presione **`☁️ Resguardar`** (requiere la conexión con Google Drive configurada, ver [1.13](#113-configuración-de-respaldos-ctlbackupconfiguracion)).
6. Use **`🔄 Actualizar`** para recargar el listado.

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/28-respaldos.png`
> - **Pantalla / Diálogo:** CtlBackup — "💾 Respaldos de Base de Datos".
> - **Estado recomendado:** Grilla con al menos un respaldo generado.
> - **Elementos a resaltar:** Recuadrar los botones `📀 Hacer Backup`, `🧪 Probar Backup` y `☁️ Resguardar`.
> - **Epígrafe:** *Figura 1.21: Respaldos de Base de Datos con las acciones de generación, prueba y resguardo.*

![Figura 1.21: Respaldos de Base de Datos.](assets/roles/28-respaldos.png)

### ⚠️ Qué hacer en caso de error
| Situación | Qué ocurre en pantalla | Solución |
|---|---|---|
| No se genera el backup | Se muestra un mensaje de error | Verifique las rutas en **Configuración de Respaldos** ([1.13](#113-configuración-de-respaldos-ctlbackupconfiguracion)). |
| `☁️ Resguardar` falla | Aviso de error de Drive | Reconecte Google Drive ([1.13](#113-configuración-de-respaldos-ctlbackupconfiguracion)). |

> [!NOTE]
> El procedimiento técnico completo (servidor SQL, rutas, Google Drive) está en el [Manual Técnico de Administración y TI](manual_tecnico_administracion.md).

## 1.13 Configuración de Respaldos (`CtlBackupConfiguracion`)

### 🎯 Objetivo
Definir **dónde** se guardan los respaldos y conectar **Google Drive** para el resguardo en la nube.

### 👥 Quién puede realizarlo
Solo **Administrador** (permiso `Configuracion.Backup.Gestionar`).

### 📋 Paso a paso
1. Haga clic en **🗄️ Configuración de Respaldos**.
2. Se abre **🗄️ Configuración de Respaldos** (*"Rutas de backup y conexión con Google Drive para resguardar las copias"*). Complete:
   - **Base de datos:** nombre de la base.
   - **Ruta en el servidor:** ruta absoluta local del servidor SQL donde se escribe el `.bak` (por ejemplo `C:\Backups`).
   - **Ruta compartida:** ruta UNC que lee la aplicación (por ejemplo `\\SERVIDOR\Backups`).
   - **Carpeta en Drive (opcional):** solo el Id de la carpeta. Si se deja vacía, la aplicación crea/busca automáticamente la carpeta *ControlVehicularBackups*.
   - **Google Drive:** presione **`🔗 Conectar con Google Drive`** y verá el estado de la conexión.
   - **Última subida:** muestra la fecha de la última subida (solo lectura).
3. Presione **`💾 Guardar`** (verde).

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/29-respaldo-configuracion.png`
> - **Pantalla / Diálogo:** CtlBackupConfiguracion — "🗄️ Configuración de Respaldos".
> - **Estado recomendado:** Formulario con las rutas cargadas y el estado de Google Drive conectado.
> - **Elementos a resaltar:** Recuadrar el botón `🔗 Conectar con Google Drive` y el botón verde `💾 Guardar`.
> - **Epígrafe:** *Figura 1.22: Configuración de Respaldos con rutas y conexión a Google Drive.*

![Figura 1.22: Configuración de Respaldos.](assets/roles/29-respaldo-configuracion.png)

### ⚠️ Qué hacer en caso de error
| Situación | Qué ocurre en pantalla | Solución |
|---|---|---|
| La ruta no existe | La validación marca el campo en rojo | Asegúrese de que la carpeta exista en el servidor. |
| Drive no conecta | *Estado Drive* con error | Vuelva a presionar `🔗 Conectar con Google Drive`. |

## 1.14 Reportes (Órdenes, Gerencial y Operativo)

El Administrador tiene acceso a **los tres reportes**. Todos se pueden **imprimir/exportar a PDF** (QuestPDF).

### 1.14.1 Reporte de Órdenes de Trabajo (`CtlReporteOrdenes`)

**🎯 Objetivo:** listar las órdenes de un período con todo su detalle.

**👥 Quién puede:** Administrador y Recepcionista (permiso `Reporte.Ordenes.Ver`).

**📋 Paso a paso:**
1. Haga clic en **📋 Reporte Órdenes**.
2. Se abre **📋 Reporte de Órdenes de Trabajo**. Complete **Desde:** y **Hasta:** y, si lo desea, filtre por **Estado:** con las opciones **Todos / Pendiente / En Proceso / Completada / Entregada**.
3. En **🔍 Buscar:** puede escribir cliente/patente y presionar el botón de búsqueda.
4. La grilla muestra: **Nº Orden · Fecha · Patente · Vehículo (Marca-Modelo) · Cliente · Km Ingreso · Servicios Aplicados · Mecánico · Recepcionista · Estado**.
5. Presione **`📄 Imprimir PDF`** para generar el documento.

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/30-reporte-ordenes.png`
> - **Pantalla / Diálogo:** CtlReporteOrdenes — "📋 Reporte de Órdenes de Trabajo".
> - **Estado recomendado:** Filtros de fecha cargados y grilla con resultados; botón de impresión visible.
> - **Elementos a resaltar:** Recuadrar los campos `Desde:`/`Hasta:`, el desplegable `Estado:` y el botón `📄 Imprimir PDF`.
> - **Epígrafe:** *Figura 1.23: Reporte de Órdenes de Trabajo con filtros y exportación a PDF.*

![Figura 1.23: Reporte de Órdenes de Trabajo.](assets/roles/30-reporte-ordenes.png)

### 1.14.2 Reporte Gerencial (`CtlReporteGerencial`)

**🎯 Objetivo:** analizar ingresos, mejores clientes, modelos más reparados y servicios por rango.

**👥 Quién puede:** solo Administrador (permiso `Reporte.Gerencial.Ver`).

**📋 Paso a paso:**
1. Haga clic en **📈 Reporte Gerencial**.
2. Se abre **📈 Reporte Gerencial (Administrador)**. Presione **`Consultar`**.
3. Recorra las pestañas:
   - **Ingresos Totales**
   - **Top Clientes**
   - **Modelos Más Reparados**
   - **Servicios por Rango**
4. Imprima cada pestaña con su botón: **`📄 Imprimir PDF (Ingresos)`**, **`📄 Imprimir PDF (Top Clientes)`**, **`📄 Imprimir PDF (Modelos)`** e **`Imprimir PDF (Servicios por Rango)`**.

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/31-reporte-gerencial.png`
> - **Pantalla / Diálogo:** CtlReporteGerencial — "📈 Reporte Gerencial (Administrador)".
> - **Estado recomendado:** Pestaña "Ingresos Totales" con datos cargados.
> - **Elementos a resaltar:** Recuadrar el botón `Consultar`, las pestañas y el botón `📄 Imprimir PDF (Ingresos)`.
> - **Epígrafe:** *Figura 1.24: Reporte Gerencial con pestañas de análisis y exportación a PDF.*

![Figura 1.24: Reporte Gerencial.](assets/roles/31-reporte-gerencial.png)

### 1.14.3 Reporte Operativo (`CtlReporteOperativo`)

**🎯 Objetivo:** medir la productividad por mecánico y los vehículos más frecuentes.

**👥 Quién puede:** Administrador y Recepcionista (permiso `Reporte.Operativo.Ver`).

**📋 Paso a paso:**
1. Haga clic en **🚙 Reporte Operativo**.
2. Se abre el **Reporte Operativo**. Presione **`Filtrar`**.
3. Revise las secciones **Productividad por Mecanico** y **Vehiculos Mas Frecuentes (Top 10)**.
4. Presione **`Exportar a PDF`** para generar el documento.

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/32-reporte-operativo.png`
> - **Pantalla / Diálogo:** CtlReporteOperativo — "Reporte Operativo".
> - **Estado recomendado:** Secciones de productividad y top 10 con datos cargados.
> - **Elementos a resaltar:** Recuadrar el botón `Filtrar` y el botón `Exportar a PDF`.
> - **Epígrafe:** *Figura 1.25: Reporte Operativo con productividad por mecánico y vehículos frecuentes.*

![Figura 1.25: Reporte Operativo.](assets/roles/32-reporte-operativo.png)

---

# Capítulo 2 — Perfil Recepcionista 📋

El Recepcionista atiende al público: **da de alta clientes y vehículos, genera órdenes de servicio y cobra**. Ve los módulos: Dashboard, Órdenes de Trabajo, Servicios, Clientes, Vehículos, Modelos, Marcas y los reportes **Órdenes** y **Operativo**. **No** ve Talleres, Usuarios, Roles, Configuración de Empresa, Respaldos ni el Reporte Gerencial.

## 2.1 Dashboard de Recepción

### 🎯 Objetivo
Tener el control diario de vehículos en taller y entregas previstas.

### 👥 Quién puede realizarlo
Administrador y Recepcionista.

### 📋 Paso a paso
1. Abra **📊 Dashboard**.
2. Revise **Vehículos Activos**, **En Proceso** y **Entregadas Hoy** (el Recepcionista **no** ve *Ingresos del Mes*).
3. Consulte la grilla **🚗 Vehículos en el Taller** para ver qué autos están presentes.
4. Use **`🔄 Actualizar`** para refrescar.

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/33-dashboard-recepcion.png`
> - **Pantalla / Diálogo:** CtlDashboard — Dashboard visto por el Recepcionista.
> - **Estado recomendado:** Datos cargados; observar que **no** aparece la tarjeta "Ingresos del Mes".
> - **Elementos a resaltar:** Recuadrar las tarjetas de resumen y la grilla de vehículos; remarcar la ausencia de la tarjeta de ingresos.
> - **Epígrafe:** *Figura 2.1: Dashboard del Recepcionista (sin la tarjeta de ingresos).*

![Figura 2.1: Dashboard del Recepcionista.](assets/roles/33-dashboard-recepcion.png)

## 2.2 Gestionar Clientes (búsqueda ágil y alta)

### 🎯 Objetivo
Encontrar rápido a un cliente por DNI, apellido o teléfono, y dar de alta clientes nuevos.

### 👥 Quién puede realizarlo
Administrador, Recepcionista y Mecánico.

### 📋 Paso a paso
1. Haga clic en **👤 Clientes**.
2. Escriba en el campo de búsqueda (DNI, apellido o teléfono) y presione **`Buscar`**.
3. Para un cliente nuevo, presione **`➕ Nuevo Cliente`** y complete el formulario **Datos del Cliente** (ver [1.4](#14-clientes-ctlcliente--frmcliente)).
4. Presione **`Guardar`**. Los errores de validación se muestran en rojo en tiempo real.

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/34-clientes-busqueda.png`
> - **Pantalla / Diálogo:** CtlCliente — búsqueda de clientes.
> - **Estado recomendado:** Campo de búsqueda con un apellido escrito y resultados filtrados en la grilla.
> - **Elementos a resaltar:** Recuadrar el campo de búsqueda, el botón azul `Buscar` y el botón verde `➕ Nuevo Cliente`.
> - **Epígrafe:** *Figura 2.2: Búsqueda ágil de clientes y alta de un cliente nuevo.*

![Figura 2.2: Búsqueda de clientes.](assets/roles/34-clientes-busqueda.png)

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/35-cliente-nuevo.png`
> - **Pantalla / Diálogo:** FrmCliente — "Datos del Cliente" en modo alta.
> - **Estado recomendado:** Formulario cargado y un ejemplo de validación en rojo (por ejemplo, DNI con letras).
> - **Elementos a resaltar:** Recuadrar el campo `DNI:` con su mensaje de error y el botón verde `Guardar`.
> - **Epígrafe:** *Figura 2.3: Alta de cliente con validación de datos en tiempo real.*

![Figura 2.3: Alta de cliente.](assets/roles/35-cliente-nuevo.png)

### ⚠️ Qué hacer en caso de error
| Situación | Qué ocurre en pantalla | Solución |
|---|---|---|
| No encuentra al cliente | La grilla queda vacía | Pruebe por DNI solo con números; si no existe, créelo. |
| DNI inválido | Borde rojo y mensaje | Escriba solo números. |

## 2.3 Padrón de Vehículos

### 🎯 Objetivo
Consultar los vehículos, vincular marca/modelo y conocer el titular.

### 👥 Quién puede realizarlo
Administrador, Recepcionista y Mecánico.

### 📋 Paso a paso
1. Haga clic en **🚗 Vehículos**.
2. Consulte la grilla **🚗 Control de Vehículos** (**Patente · Cliente · Marca · Modelo · Año · Km Actual**).
3. Para dar de alta, use **`➕ Nuevo Vehículo`** (ver [1.5](#15-vehículos-ctlvehiculo--frmvehiculo)).

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/36-vehiculos-recepcion.png`
> - **Pantalla / Diálogo:** CtlVehiculo — padrón de vehículos visto por Recepción.
> - **Estado recomendado:** Grilla con vehículos y sus titulares (cliente).
> - **Elementos a resaltar:** Recuadrar la columna `Cliente` (titular) y el botón verde `➕ Nuevo Vehículo`.
> - **Epígrafe:** *Figura 2.4: Padrón de Vehículos con el titular de cada unidad.*

![Figura 2.4: Padrón de Vehículos.](assets/roles/36-vehiculos-recepcion.png)

## 2.4 Catálogos auxiliares (Servicios, Marcas y Modelos)

En el día a día, el Recepcionista también puede consultar y mantener **Servicios** ([1.3](#13-catálogo-de-servicios-ctlservicio--frmservicio)), **Marcas** ([1.7](#17-marcas-ctlmarca--frmmarca)) y **Modelos** ([1.6](#16-modelos-de-autos-ctlmodelo--frmmodelo)) porque su rol tiene el permiso `Servicio.Ver` y `Vehiculo.Ver`. Estos procedimientos son idénticos a los del Administrador; consúltelos en el Capítulo 1.

## 2.5 Flujo principal: generar una Orden de Servicio

> [!IMPORTANT]
> Este es el procedimiento central del Recepcionista. Se detalla paso a paso porque es el que se realiza con más frecuencia.

### 🎯 Objetivo
Registrar el ingreso de un vehículo, las fallas reportadas y los servicios a realizar, asignando un mecánico a cada tarea.

### 👥 Quién puede realizarlo
Administrador, Recepcionista y Mecánico (permiso `OrdenServicio.Ver`).

### 📋 Paso a paso

**Paso 1 — Abrir la orden y elegir el cliente**
1. Vaya a **📋 Órdenes de Trabajo** y presione **`➕ Nueva Orden`**.
2. En el formulario, en el campo **Cliente:**, seleccione al cliente. Si no existe, presione el botón **`➕`** junto al campo para crearlo sin salir.

**Paso 2 — Elegir el vehículo**
3. En el campo **Vehículo:**, seleccione el auto del cliente. Si el auto no está listado, use el paso 3 alternativo (botón `+ Rápido`).

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/37-orden-nueva-cliente.png`
> - **Pantalla / Diálogo:** CtlOrdenServicioForm — "Datos de la Inspección / Servicio" (selección de cliente y vehículo).
> - **Estado recomendado:** Cliente seleccionado y lista de vehículos desplegada.
> - **Elementos a resaltar:** Recuadrar los campos `Cliente:` y `Vehículo:` y el botón `➕` de alta rápida.
> - **Epígrafe:** *Figura 2.5: Selección del cliente y del vehículo al crear una orden.*

![Figura 2.5: Selección de cliente y vehículo.](assets/roles/37-orden-nueva-cliente.png)

**Paso 3 (alternativo) — Vehículo no listado o transferido: botón `+ Rápido`**
4. Si el cliente trae un auto **nuevo** o **usado que figura a nombre de otro dueño**, presione el botón azul **`+ Rápido`**.
5. Escriba la **patente** del vehículo.
6. El sistema hace una **asignación inteligente**: **transfiere la titularidad al cliente actual**, conserva el historial del auto y lo selecciona al instante en la orden.

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/38-orden-nueva-rapido.png`
> - **Pantalla / Diálogo:** CtlOrdenServicioForm — alta/transferencia rápida por patente (botón `+ Rápido`).
> - **Estado recomendado:** Patente ya ingresada y el vehículo asignado al cliente en el formulario.
> - **Elementos a resaltar:** Recuadrar el botón azul `+ Rápido` y el campo de patente.
> - **Epígrafe:** *Figura 2.6: Alta rápida por patente con transferencia automática de titularidad.*

![Figura 2.6: Alta rápida por patente (+ Rápido).](assets/roles/38-orden-nueva-rapido.png)

**Paso 4 — Cargar el detalle de la orden**
7. Complete el **Kilometraje:** actual del vehículo.
8. En **Agregar Ítem de Inspección:**, seleccione el **Servicio** y presione **`➕ Agregar`**. Repita por cada trabajo. Si falta un servicio en el catálogo, use el botón **`➕`** junto al desplegable para crear un **Nuevo Servicio** al vuelo.
9. En la grilla de ítems (**Ítem / Servicio · Precio · Mecánico · Revisado / OK · Observaciones**) asigne un **Mecánico** a cada tarea y, si corresponde, escriba observaciones.
10. Verifique el campo **Total:**.
11. Revise el **Estado:** de la orden. Por defecto, una orden nueva queda en **En Proceso**.
12. Presione **`Guardar`** (verde). Para descartar la carga, **`Cancelar`** (rojo).

**Resultado esperado:** la orden aparece en el listado con su número y queda disponible para que el Mecánico la trabaje.

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/40-orden-nueva-detalle.png`
> - **Pantalla / Diálogo:** CtlOrdenServicioForm — formulario completo con ítems cargados.
> - **Estado recomendado:** Cliente y vehículo seleccionados, kilometraje, varios ítems con servicio, precio y mecánico, y el total visible.
> - **Elementos a resaltar:** Recuadrar el botón `➕ Agregar`, la columna `Mecánico`, el campo `Total:` y el botón verde `Guardar`.
> - **Epígrafe:** *Figura 2.7: Carga del detalle de la orden (servicios, mecánico y total).*

![Figura 2.7: Detalle de la orden de servicio.](assets/roles/40-orden-nueva-detalle.png)

### 💡 Consejos y alertas
> [!TIP]
> El botón **`← Volver`** en la parte superior regresa al listado sin guardar. Úselo solo si desea descartar la carga.
> [!TIP]
> El campo **Taller:** está disponible pero puede permanecer oculto según la configuración; no es obligatorio completarlo.

### ⚠️ Qué hacer en caso de error
| Situación | Qué ocurre en pantalla | Solución |
|---|---|---|
| La patente ya figura a otro cliente | El sistema la reconoce | Use **`+ Rápido`** para reasignar la titularidad al cliente correcto. |
| No puede guardar | Campo obligatorio en rojo | Complete Cliente, Vehículo y al menos un ítem. |
| Servicio no encontrado en el desplegable | No aparece la opción | Créelo con el botón `➕` de **Nuevo Servicio**. |
| El total no coincide | — | Revise precios en el catálogo de Servicios ([1.3](#13-catálogo-de-servicios-ctlservicio--frmservicio)). |

## 2.6 Cobrar una orden (`FrmPago`)

### 🎯 Objetivo
Registrar el pago de una orden y emitir el comprobante.

### 👥 Quién puede realizarlo
Recepcionista y Administrador.

### 📋 Paso a paso
1. En **📋 Órdenes de Trabajo**, localice la orden en estado **Completada** (use la búsqueda si hace falta).
2. Seleccione la fila y presione **`💳 Pagar`**.
3. Se abre la ventana **Procesar Pago**. Verifique el **Total a Pagar:**.
4. En **Método de Pago**, elija con el botón de opción:
   - **Efectivo** — complete **Monto Recibido:**.
   - **Tarjeta** — complete **Número de Tarjeta:**, **Titular (apro, lim, rec):**, **Vencimiento:** y **CVV:**.
5. Presione el botón verde **`Pagar`**. Verá *"Procesando..."*. Para descartar, presione **`Cancelar`** (rojo).
6. Al finalizar, la orden pasa a **Pagada** y puede generar el comprobante con **`📄 Comp. Pago`**.

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/41-cobro-pago.png`
> - **Pantalla / Diálogo:** FrmPago — "Procesar Pago".
> - **Estado recomendado:** Ventana de cobro con el total y el método de pago seleccionado.
> - **Elementos a resaltar:** Recuadrar el campo `Total a Pagar:`, los botones de opción de método de pago y el botón verde `Pagar`.
> - **Epígrafe:** *Figura 2.8: Ventana de cobro con selección de método de pago.*

![Figura 2.8: Procesar Pago.](assets/roles/41-cobro-pago.png)

### 📋 Paso a paso — Emitir el comprobante
1. Con la orden seleccionada en el listado, presione **`📄 Comp. Pago`**.
2. Se genera el comprobante listo para imprimir/guardar en PDF.

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/42-comprobante-pago.png`
> - **Pantalla / Diálogo:** Comprobante de pago (generado desde `📄 Comp. Pago`).
> - **Estado recomendado:** Comprobante de una orden pagada, con el logo de la empresa en el encabezado.
> - **Elementos a resaltar:** Recuadrar el encabezado con el logo y los datos de la empresa.
> - **Epígrafe:** *Figura 2.9: Comprobante de pago con los datos de la empresa.*

![Figura 2.9: Comprobante de pago.](assets/roles/42-comprobante-pago.png)

### 💡 Consejos y alertas
> [!WARNING]
> Solo se cobra sobre órdenes **Completadas**. Si la orden aún está *En Proceso*, primero debe completarse el trabajo.

### ⚠️ Qué hacer en caso de error
| Situación | Qué ocurre en pantalla | Solución |
|---|---|---|
| El monto recibido es menor al total | El sistema no permite confirmar | Verifique el importe; debe cubrir el total (o usar el cobro parcial habilitado por el taller). |
| Datos de tarjeta incompletos | Campos en rojo | Complete Número, Titular, Vencimiento y CVV. |
| Presionó `Pagar` sin efecto | Quedó *"Procesando..."* | Espere a que termine el proceso; no cierre la ventana. |

## 2.7 Reportes del Recepcionista

El Recepcionista tiene acceso a dos reportes:
- **📋 Reporte Órdenes** — listado de órdenes por período. Ver [1.14.1](#1141-reporte-de-órdenes-de-trabajo-ctlreporteordenes).
- **🚙 Reporte Operativo** — productividad por mecánico y vehículos frecuentes. Ver [1.14.3](#1143-reporte-operativo-ctlreporteoperativo).

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/43-reporte-ordenes-recepcion.png`
> - **Pantalla / Diálogo:** CtlReporteOrdenes — uso desde el rol Recepcionista.
> - **Estado recomendado:** Filtro por rango de fechas y resultados en grilla.
> - **Elementos a resaltar:** Recuadrar los campos de fecha y el botón `📄 Imprimir PDF`.
> - **Epígrafe:** *Figura 2.10: Reporte de Órdenes de Trabajo desde Recepción.*

![Figura 2.10: Reporte de Órdenes desde Recepción.](assets/roles/43-reporte-ordenes-recepcion.png)

> [!NOTE]
> El Recepcionista **no** ve **📈 Reporte Gerencial** (es exclusivo del Administrador) ni los módulos de Talleres, Usuarios, Roles, Configuración de Empresa y Respaldos.

---

# Capítulo 3 — Perfil Mecánico 🔧

El Mecánico trabaja sobre sus **tareas asignadas**. Su menú incluye Dashboard, Órdenes de Trabajo, **Mis Trabajos** (exclusivo suyo), Servicios, Clientes, Vehículos, Modelos y Marcas. **No** ve reportes ni módulos de administración.

> [!IMPORTANT]
> La pantalla **🔧 Mis Trabajos** es **exclusiva del Mecánico**: el Administrador y el Recepcionista no la ven.

## 3.1 Dashboard del Mecánico

### 🎯 Objetivo
Ver sus vehículos asignados en el taller.

### 👥 Quién puede realizarlo
Todos los roles, pero el Mecánico ve **solo sus propias órdenes**.

### 📋 Paso a paso
1. Abra **📊 Dashboard**.
2. Observe las tarjetas **Vehículos Activos**, **En Proceso** y **Entregadas Hoy**, calculadas **solo sobre sus órdenes**.
3. Revise la grilla **🚗 Vehículos en el Taller** para ver sus trabajos pendientes.
4. Use **`🔄 Actualizar`** para refrescar.

> [!NOTE]
> El Mecánico **no** ve la tarjeta **Ingresos del Mes** (dato de gestión, solo del Administrador).

## 3.2 Mis Trabajos (`CtlMisTrabajos`) — pantalla maestro-detalle

### 🎯 Objetivo
Ver las órdenes asignadas y sus tareas, marcar tareas como revisadas y dejar observaciones técnicas.

### 👥 Quién puede realizarlo
Solo **Mecánico** (permiso `MisTrabajos.Ver`).

### 📋 Paso a paso
1. Haga clic en **🔧 Mis Trabajos**.
2. Se abre la pantalla **🔧 Mis Trabajos** (*"Panel de órdenes y tareas asignadas"*). Tiene dos paneles:
   - **Panel maestro (arriba):** **🧾 Órdenes asignadas (doble clic para abrir)**, con las columnas **Nº Orden · Fecha · Vehículo · Cliente · Km · Progreso** (la barra de *Progreso* muestra *tareas hechas / total*).
   - **Panel detalle (abajo):** **🛠️ Tareas de la Orden**, con las columnas **Tarea · Observaciones · Revisado / OK**.
3. Use **🔍 Buscar:** para filtrar y **Mostrar:** para elegir **Pendientes**, **Finalizados** o **Todos**.
4. Haga clic sobre una orden para cargar sus tareas en el panel inferior. Si no hay datos, verá *"No hay órdenes para mostrar"* o *"Seleccione una orden para ver sus tareas"*.

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/45-mis-trabajos.png`
> - **Pantalla / Diálogo:** CtlMisTrabajos — "🔧 Mis Trabajos".
> - **Estado recomendado:** Órdenes asignadas en el panel maestro y una orden seleccionada con sus tareas en el panel detalle.
> - **Elementos a resaltar:** Recuadrar el filtro **Mostrar:** (Pendientes/Finalizados/Todos), la columna **Progreso** y el campo **🔍 Buscar:**.
> - **Epígrafe:** *Figura 3.1: Pantalla Mis Trabajos con el panel maestro (órdenes) y el panel detalle (tareas).*

![Figura 3.1: Mis Trabajos (maestro-detalle).](assets/roles/45-mis-trabajos.png)

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/46-mis-trabajos-detalle.png`
> - **Pantalla / Diálogo:** CtlMisTrabajos — panel detalle "🛠️ Tareas de la Orden".
> - **Estado recomendado:** Tareas de una orden seleccionada, con algunas marcadas como revisadas.
> - **Elementos a resaltar:** Recuadrar las columnas `Tarea`, `Observaciones` y `Revisado / OK`.
> - **Epígrafe:** *Figura 3.2: Detalle de tareas de la orden seleccionada.*

![Figura 3.2: Detalle de tareas.](assets/roles/46-mis-trabajos-detalle.png)

## 3.3 Edición rápida inline de tareas

### 🎯 Objetivo
Marcar tareas realizadas y dejar notas técnicas sin salir de la grilla.

### 👥 Quién puede realizarlo
Solo **Mecánico**.

### 📋 Paso a paso
1. En **🔧 Mis Trabajos**, seleccione la orden.
2. En el panel **🛠️ Tareas de la Orden**:
   - Marque la casilla de la columna **Revisado / OK** cuando una tarea fue completada.
   - Escriba las **Observaciones** técnicas directamente sobre la celda de la grilla.
3. Presione **`💾 Guardar Cambios`** para confirmar.
4. Al marcar tareas, la barra de **Progreso** de la orden en el panel superior se actualiza.

Además, la botonera de esta pantalla ofrece:
- **`🔗 Ver Orden Completa`** — abre la orden en modo consulta.
- **`📋 Tareas del Día`** — resumen de las tareas del día.
- **`🕒 Imprimir Historial`** — imprime el historial de la orden.

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/47-mis-trabajos-edicion-inline.png`
> - **Pantalla / Diálogo:** CtlMisTrabajos — edición inline de una tarea.
> - **Estado recomendado:** Una tarea con la casilla `Revisado / OK` marcada y Observaciones escritas; barra de progreso actualizada.
> - **Elementos a resaltar:** Recuadrar la casilla `Revisado / OK`, la celda de `Observaciones` y el botón `💾 Guardar Cambios`.
> - **Epígrafe:** *Figura 3.3: Edición rápida inline de una tarea (Revisado / OK y Observaciones).*

![Figura 3.3: Edición inline de tareas.](assets/roles/47-mis-trabajos-edicion-inline.png)

### 💡 Consejos y alertas
> [!TIP]
> Recuerde presionar **`💾 Guardar Cambios`**: los cambios en la grilla no se guardan solos.

## 3.4 Consultar la orden completa (modo lectura)

### 🎯 Objetivo
Ver el detalle completo de la orden (cliente, vehículo, ítems y mecánicos) sin modificarlo.

### 👥 Quién puede realizarlo
Todos los roles con `OrdenServicio.Ver`.

### 📋 Paso a paso
1. En **🔧 Mis Trabajos**, seleccione una orden.
2. Haga **doble clic** sobre la orden **(doble clic para abrir)** o presione **`🔗 Ver Orden Completa`**.
3. Se abre el formulario de la orden en modo **consulta**. Revise los datos y cierre con **`← Volver`**.

> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/roles/48-orden-consulta.png`
> - **Pantalla / Diálogo:** CtlOrdenServicioForm — orden abierta en modo consulta desde Mis Trabajos.
> - **Estado recomendado:** Orden con ítems y mecánicos visibles, en modo lectura.
> - **Elementos a resaltar:** Recuadrar el botón `← Volver` y la grilla de ítems.
> - **Epígrafe:** *Figura 3.4: Orden completa consultada en modo lectura desde Mis Trabajos.*

![Figura 3.4: Consulta de la orden completa.](assets/roles/48-orden-consulta.png)

### ⚠️ Qué hacer en caso de error
| Situación | Qué ocurre en pantalla | Solución |
|---|---|---|
| No aparecen órdenes asignadas | El panel maestro está vacío | Verifique que sea el mecánico asignado; consulte con Recepción o Administración. |
| No se guardan las tareas | Los cambios se pierden al salir | Presione **`💾 Guardar Cambios`** antes de cambiar de orden. |

---

# Capítulo 4 — Dudas Frecuentes, Estados y Glosario

## 4.1 Preguntas frecuentes

<details>
<summary><strong>¿Por qué no veo un módulo en el menú?</strong></summary>

El menú es dinámico según su rol. Si un módulo no aparece, su rol no tiene el permiso correspondiente. Consulte la [matriz de módulos y permisos](#02-la-estructura-de-la-interfaz-y-el-menú-lateral) o pídale al Administrador que le asigne el permiso (ver [1.10](#110-roles-y-permisos-ctlrol--frmrol--frmrolpermiso)).
</details>

<details>
<summary><strong>¿Qué hago si una patente ya existe registrada a otro cliente?</strong></summary>

No cree un vehículo duplicado. En el formulario de la orden use el botón **`+ Rápido`** e ingrese la patente: el sistema **transfiere la titularidad** al cliente actual, conserva el historial y selecciona el auto al instante. Ver [Paso 3 del flujo de orden](#25-flujo-principal-generar-una-orden-de-servicio).
</details>

<details>
<summary><strong>¿Qué significan los campos resaltados en rojo?</strong></summary>

Son validaciones de datos: falta completar un campo obligatorio o el formato es incorrecto (por ejemplo, un DNI con letras o un email mal escrito). Corrija el valor y el mensaje en rojo desaparecerá.
</details>

<details>
<summary><strong>¿Cómo reimprimo un comprobante de recepción o de pago?</strong></summary>

Vaya a **📋 Órdenes de Trabajo**, seleccione la orden y presione **`📄 Comp. Recepción`** o **`📄 Comp. Pago`** según corresponda. El comprobante se regenera y puede imprimirse o guardarse en PDF.
</details>

<details>
<summary><strong>¿Dónde se cargan el nombre y el logo de la empresa que aparecen en los reportes?</strong></summary>

En **🏢 Configuración de Empresa** (`CtlConfiguracion`), como Administrador. **No** en el módulo **🔧 Talleres**, que es el ABM de talleres/sucursales. Ver [1.2](#12-configurar-los-datos-de-la-empresa-ctlconfiguracion).
</details>

<details>
<summary><strong>¿Por qué un usuario no puede iniciar sesión?</strong></summary>

Puede estar **inhabilitado**. Como Administrador, rehabilítelo con **`🔄 Cambiar Estado`** en **Usuarios**. Ver [1.9](#19-gestionar-usuarios-ctlusuario--frmusuario).
</details>

<details>
<summary><strong>¿Por qué el mensaje se ve como un cartel propio de la aplicación y no el de Windows?</strong></summary>

*Taller Pro* usa ventanas de confirmación y avisos propias (`FrmConfirmacion`) para mantener el mismo estilo visual en todo el sistema. No es un error. Ver [0.4](#04-convenciones-visuales-botones-colores-y-confirmaciones).
</details>

## 4.2 Estados de una orden de servicio

Los estados de una **orden** son los siguientes (tal como se muestran en pantalla):

| Estado | Significado | Quién suele modificarlo |
|---|---|---|
| **Abierta** | Orden creada, aún sin trabajo iniciado. | Recepcionista / Administrador. |
| **En Proceso** | El vehículo está en reparación. Estado por defecto de una orden nueva. | Recepcionista / Administrador. |
| **Completada** | El trabajo terminó y el vehículo está listo para entregar/cobrar. | Recepcionista / Administrador. |
| **Pagada** | Se registró el pago de la orden. | Recepcionista / Administrador (al usar `💳 Pagar`). |
| **Cancelada** | La orden se anuló. | Recepcionista / Administrador (con `🗑️ Eliminar / Cancelar`). |

Además, cada **tarea** dentro de la orden tiene su propio estado interno: **Pendiente**, **En Curso** y **Finalizada**. El Mecánico refleja el avance marcando **Revisado / OK** en *Mis Trabajos*.

> [!NOTE]
> En el **Reporte de Órdenes** el filtro de estado ofrece las opciones **Todos / Pendiente / En Proceso / Completada / Entregada**. Use *Todos* para no perder ninguna orden del rango.

## 4.3 Glosario

| Término | Significado |
|---|---|
| **Orden de Servicio** | Registro del ingreso de un vehículo al taller, con las tareas a realizar y su estado. |
| **Ítem / Tarea** | Cada trabajo individual dentro de una orden (por ejemplo, "Cambio de aceite"), con su precio y mecánico asignado. |
| **Recepción (Km Ingreso)** | Kilometraje del vehículo registrado al momento de recibirlo. |
| **Comprobante de Recepción** | Documento que certifica el ingreso del vehículo al taller. |
| **Comprobante de Pago** | Documento que certifica el pago de la orden. |
| **RBAC** | Control de acceso basado en roles: cada rol define qué módulos ve y usa cada usuario. |
| **Permiso** | Habilitación concreta (por ejemplo `Cliente.Ver`) que un rol concede. |
| **`+ Rápido`** | Alta/transferencia rápida de un vehículo por patente desde la orden. |
| **Dashboard** | Pantalla de resumen con indicadores del taller. |
| **Respaldo (Backup)** | Copia de seguridad de la base de datos. |
| **Resguardar** | Subir el respaldo a Google Drive. |
| **Toast** | Mensaje flotante verde de "Operación exitosa". |

---

> **Fin del Manual de Usuario Operativo por Roles.**
> Para instalación, conexión a SQL Server, scripts `bd/`, respaldos y recuperación ante desastres, consulte el [Manual Técnico de Administración y TI](manual_tecnico_administracion.md).
