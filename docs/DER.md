# Contexto del Sistema: Gestión de Taller Mecánico y Servicios de Vehículos

Documento de referencia para el modelo de datos relacional del sistema de gestión de taller, registro de servicios, vehículos y facturación (`PropuestaServicioAuto`).

---

## 1. Entidades y Atributos

### Seguridad y Usuarios
* **ROL**
  * `id_rol` (int, PK): Identificador único del rol.
  * `nombre` (string): Nombre del rol (ej. Administrador, Mecánico, Recepcionista).
  * `descripcion` (string): Descripción de responsabilidades y permisos.
  * `estado` (bool): Estado activo/inactivo.

* **USUARIO**
  * `id_usuario` (int, PK): Identificador único del usuario.
  * `id_rol` (int, FK): Rol asociado (`ROL.id_rol`).
  * `nombre` (string): Nombre de usuario o identificador de acceso.
  * `contrasena` (string): Hash de la credencial de acceso.
  * `estado` (bool): Estado de cuenta (activo/inactivo).

---

### Clientes y Parque Automotor
* **CLIENTE**
  * `id_cliente` (int, PK): Identificador único del cliente.
  * `nombre` (string): Nombre(s).
  * `apellido` (string): Apellido(s).
  * `dni` (string): Documento de identidad / CUIT.
  * `fecha_nac` (date): Fecha de nacimiento.
  * `direccion` (string): Domicilio.
  * `email` (string): Correo electrónico de contacto.
  * `telefono` (string): Teléfono de contacto.
  * `activo` (bool): Estado del registro de cliente.

* **MARCA**
  * `id_marca` (int, PK): Identificador de la marca de vehículos.
  * `nombre_marca` (string): Denominación de la marca (ej. Volkswagen, Ford).

* **MODELO**
  * `id_modelo` (int, PK): Identificador del modelo.
  * `id_marca` (int, FK): Marca fabricante (`MARCA.id_marca`).
  * `nombre_modelo` (string): Denominación del modelo (ej. Gol Trend, Fiesta).

* **VEHICULO**
  * `id_vehiculo` (int, PK): Identificador único de la unidad.
  * `id_cliente` (int, FK): Propietario (`CLIENTE.id_cliente`).
  * `id_modelo` (int, FK): Modelo (`MODELO.id_modelo`).
  * `anio` (int): Año de fabricación.
  * `patente` (string): Dominio/patente del vehículo.
  * `km_actual` (int): Kilometraje acumulado al último registro.

---

### Talleres, Servicios y Operación
* **TALLER**
  * `id_taller` (int, PK): Identificador del taller o sucursal.
  * `nombre_taller` (string): Nombre de fantasía o sucursal.
  * `direccion` (string): Ubicación física.
  * `telefono` (string): Teléfono de contacto.
  * `activo` (bool): Sucursal habilitada/deshabilitada.

* **SERVICIO**
  * `id_servicio` (int, PK): Identificador del tipo de servicio/tarea.
  * `nombre` (string): Descripción de la tarea (ej. Cambio de aceite, Alineación).
  * `precio` (decimal): Tarifa o costo base unitario.
  * `activo` (string/bool): Disponibilidad del catálogo.

* **REGISTRO_SERVICIO**
  * `id_registro` (int, PK): Orden o registro principal de ingreso al taller.
  * `id_vehiculo` (int, FK): Vehículo atendido (`VEHICULO.id_vehiculo`).
  * `id_taller` (int, FK): Taller donde se efectúa (`TALLER.id_taller`).
  * `id_usuario` (int, FK): Usuario/operador que recepciona (`USUARIO.id_usuario`).
  * `fecha` (date): Fecha de ingreso.
  * `km_ingreso` (int): Kilometraje al momento de ingresar.
  * `estado` (enum): Estado de la orden (ej. Pendiente, En Proceso, Finalizado, Cancelado).

* **DETALLE_SERVICIO**
  * `id_detalle` (int, PK): Renglón o ítem dentro del registro de servicio.
  * `id_registro` (int, FK): Orden cabecera (`REGISTRO_SERVICIO.id_registro`).
  * `id_servicio` (int, FK): Servicio o tarea aplicada (`SERVICIO.id_servicio`).
  * `id_usuario` (int, FK): Operador o técnico que ejecuta/confecciona (`USUARIO.id_usuario`).
  * `cantidad` (int): Cantidad de ítems/horas aplicadas.
  * `precio` (decimal): Precio unitario acordado o facturado para el detalle.
  * `origen` (string): Origen del repuesto o mano de obra.
  * `estado` (enum): Estado individual del ítem.

---

### Facturación y Pagos
* **FACTURA**
  * `id_factura` (int, PK): Identificador del comprobante.
  * `id_servicio` / `id_registro` (int, FK): Vínculo hacia la orden de servicio correspondiente.
  * `fecha` (date): Fecha de emisión.
  * `total` (decimal): Importe liquidado final.

* **METODO_PAGO**
  * `id_metodo_pago` (int, PK): Identificador de la vía de pago.
  * `nombre` (string): Nombre del método (ej. Efectivo, Tarjeta de Crédito, Transferencia).

* **PAGO**
  * `id_pago` (int, PK): Registro individual de transacción.
  * `id_factura` (int, FK): Factura cancelada/asociada (`FACTURA.id_factura`).
  * `id_metodo_pago` (int, FK): Medio utilizado (`METODO_PAGO.id_metodo_pago`).
  * `monto` (decimal): Suma abonada.

---

## 2. Mapa de Relaciones (Cardinalidades)

| Entidad Origen | Relación | Entidad Destino | Tipo | Detalle |
| :--- | :--- | :--- | :--- | :--- |
| `ROL` | Posee | `USUARIO` | 1 a N | Un rol puede ser asignado a muchos usuarios. |
| `MARCA` | Especifica | `MODELO` | 1 a N | Una marca tiene múltiples modelos. |
| `MODELO` | Especifica | `VEHICULO` | 1 a N | Un modelo tipifica a muchos vehículos. |
| `CLIENTE` | Es dueño | `VEHICULO` | 1 a N | Un cliente puede tener registrados varios vehículos. |
| `VEHICULO` | Ingresa a | `REGISTRO_SERVICIO` | 1 a N | Un vehículo genera múltiples órdenes de servicio en el tiempo. |
| `TALLER` | Realiza | `REGISTRO_SERVICIO` | 1 a N | Una sucursal procesa múltiples registros de servicio. |
| `USUARIO` | Recepciona | `REGISTRO_SERVICIO` | 1 a N | Un operador/recepcionista abre múltiples registros. |
| `REGISTRO_SERVICIO`| Contiene | `DETALLE_SERVICIO` | 1 a N | Una orden agrupa uno o más detalles de servicio. |
| `SERVICIO` | Tipifica | `DETALLE_SERVICIO` | 1 a N | Un ítem del catálogo puede aparecer en múltiples órdenes. |
| `USUARIO` | Confecciona | `DETALLE_SERVICIO` | 1 a N | Un técnico registra o ejecuta los ítems del detalle. |
| `REGISTRO_SERVICIO`| Genera | `FACTURA` | 1 a 1 / 1 a N | El servicio cerrado emite el comprobante de liquidación. |
| `FACTURA` | Origina | `PAGO` | 1 a N | Una factura puede recibir uno o varios pagos parciales/totales. |
| `METODO_PAGO` | Aplica en | `PAGO` | 1 a N | Un método de pago se usa en múltiples cobros. |

---

## 3. Reglas de Negocio Clave
1. **Consistencia de Kilometraje:** Al ingresar un `REGISTRO_SERVICIO`, el valor de `km_ingreso` debe ser igual o superior al `km_actual` registrado en la tabla `VEHICULO`, actualizando este último al finalizar el trabajo.
2. **Histórico de Precios:** El campo `precio` en `DETALLE_SERVICIO` es un snapshot del valor vigente en `SERVICIO` al momento de la intervención, para evitar descalces contables por futuras actualizaciones de tarifas.
3. **Validación de Pagos:** La suma de los registros `PAGO.monto` vinculados a una `FACTURA` no puede exceder el `FACTURA.total`.