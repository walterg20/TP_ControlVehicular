# Contexto del Sistema: GestiÃ³n de Taller MecÃ¡nico y Servicios de VehÃ­culos

Documento de referencia para el modelo de datos relacional del sistema de gestiÃ³n de taller, registro de servicios, vehÃ­culos y facturaciÃ³n (`PropuestaServicioAuto`).

---

## 1. Entidades y Atributos

### Seguridad y Usuarios
* **ROL**
  * `id_rol` (int, PK): Identificador Ãºnico del rol.
  * `nombre` (string): Nombre del rol (ej. Administrador, MecÃ¡nico, Recepcionista).
  * `descripcion` (string): DescripciÃ³n de responsabilidades y permisos.
  * `estado` (bool): Estado activo/inactivo.

* **USUARIO**
  * `id_usuario` (int, PK): Identificador Ãºnico del usuario.
  * `id_rol` (int, FK): Rol asociado (`ROL.id_rol`).
  * `nombre` (string): Nombre de usuario o identificador de acceso.
  * `contrasena` (string): Hash de la credencial de acceso.
  * `estado` (bool): Estado de cuenta (activo/inactivo).

---

### Clientes y Parque Automotor
* **CLIENTE**
  * `id_cliente` (int, PK): Identificador Ãºnico del cliente.
  * `nombre` (string): Nombre(s).
  * `apellido` (string): Apellido(s).
  * `dni` (string): Documento de identidad / CUIT.
  * `fecha_nac` (date): Fecha de nacimiento.
  * `direccion` (string): Domicilio.
  * `email` (string): Correo electrÃ³nico de contacto.
  * `telefono` (string): TelÃ©fono de contacto.
  * `activo` (bool): Estado del registro de cliente.

* **MARCA**
  * `id_marca` (int, PK): Identificador de la marca de vehÃ­culos.
  * `nombre_marca` (string): DenominaciÃ³n de la marca (ej. Volkswagen, Ford).

* **MODELO**
  * `id_modelo` (int, PK): Identificador del modelo.
  * `id_marca` (int, FK): Marca fabricante (`MARCA.id_marca`).
  * `nombre_modelo` (string): DenominaciÃ³n del modelo (ej. Gol Trend, Fiesta).

* **PROPIETARIO_VEHICULO**
  * id_propietario_vehiculo (int, PK): Identificador único de la asignación.
  * id_cliente (int, FK): Cliente propietario (CLIENTE.id_cliente).
  * id_vehiculo (int, FK): Vehículo adquirido (VEHICULO.id_vehiculo).
  * echa_adquisicion (date): Fecha en que adquiere la unidad.
  * echa_venta (date, null): Fecha en que se vendió (null si es el dueño actual).
  * es_actual (bool): Bandera para facilitar la búsqueda del dueño activo.

* **VEHICULO**
  * id_vehiculo (int, PK): Identificador único de la unidad.
  * id_modelo (int, FK): Modelo (MODELO.id_modelo).
  * nio (int): Año de fabricación.
  * patente (string): Dominio/patente del vehículo.
  * km_actual (int): Kilometraje acumulado al último registro.

---

### Talleres, Servicios y OperaciÃ³n
* **TALLER**
  * `id_taller` (int, PK): Identificador del taller o sucursal.
  * `nombre_taller` (string): Nombre de fantasÃ­a o sucursal.
  * `direccion` (string): UbicaciÃ³n fÃ­sica.
  * `telefono` (string): TelÃ©fono de contacto.
  * `activo` (bool): Sucursal habilitada/deshabilitada.

* **SERVICIO**
  * `id_servicio` (int, PK): Identificador del tipo de servicio/tarea.
  * `nombre` (string): DescripciÃ³n de la tarea (ej. Cambio de aceite, AlineaciÃ³n).
  * `precio` (decimal): Tarifa o costo base unitario.
  * `activo` (string/bool): Disponibilidad del catÃ¡logo.

* **REGISTRO_SERVICIO**
  * `id_registro` (int, PK): Orden o registro principal de ingreso al taller.
  * `id_vehiculo` (int, FK): VehÃ­culo atendido (`VEHICULO.id_vehiculo`).
  * `id_taller` (int, FK): Taller donde se efectÃºa (`TALLER.id_taller`).
  * `id_usuario` (int, FK): Usuario/operador que recepciona (`USUARIO.id_usuario`).
  * `fecha` (date): Fecha de ingreso.
  * `km_ingreso` (int): Kilometraje al momento de ingresar.
  * `estado` (enum): Estado de la orden (ej. Pendiente, En Proceso, Finalizado, Cancelado).

* **DETALLE_SERVICIO**
  * `id_detalle` (int, PK): RenglÃ³n o Ã­tem dentro del registro de servicio.
  * `id_registro` (int, FK): Orden cabecera (`REGISTRO_SERVICIO.id_registro`).
  * `id_servicio` (int, FK): Servicio o tarea aplicada (`SERVICIO.id_servicio`).
  * `id_usuario` (int, FK): Operador o tÃ©cnico que ejecuta/confecciona (`USUARIO.id_usuario`).
  * `cantidad` (int): Cantidad de Ã­tems/horas aplicadas.
  * `precio` (decimal): Precio unitario acordado o facturado para el detalle.
  * `origen` (string): Origen del repuesto o mano de obra.
  * `estado` (enum): Estado individual del Ã­tem.

---

### FacturaciÃ³n y Pagos
* **FACTURA**
  * `id_factura` (int, PK): Identificador del comprobante.
  * `id_servicio` / `id_registro` (int, FK): VÃ­nculo hacia la orden de servicio correspondiente.
  * `fecha` (date): Fecha de emisiÃ³n.
  * `total` (decimal): Importe liquidado final.

* **METODO_PAGO**
  * `id_metodo_pago` (int, PK): Identificador de la vÃ­a de pago.
  * `nombre` (string): Nombre del mÃ©todo (ej. Efectivo, Tarjeta de CrÃ©dito, Transferencia).

* **PAGO**
  * `id_pago` (int, PK): Registro individual de transacciÃ³n.
  * `id_factura` (int, FK): Factura cancelada/asociada (`FACTURA.id_factura`).
  * `id_metodo_pago` (int, FK): Medio utilizado (`METODO_PAGO.id_metodo_pago`).
  * `monto` (decimal): Suma abonada.

---

## 2. Mapa de Relaciones (Cardinalidades)

| Entidad Origen | RelaciÃ³n | Entidad Destino | Tipo | Detalle |
| :--- | :--- | :--- | :--- | :--- |
| `ROL` | Posee | `USUARIO` | 1 a N | Un rol puede ser asignado a muchos usuarios. |
| `MARCA` | Especifica | `MODELO` | 1 a N | Una marca tiene mÃºltiples modelos. |
| `MODELO` | Especifica | `VEHICULO` | 1 a N | Un modelo tipifica a muchos vehÃ­culos. |
| `CLIENTE` | Es dueÃ±o | `VEHICULO` | 1 a N | Un cliente puede tener registrados varios vehÃ­culos. |
| `VEHICULO` | Ingresa a | `REGISTRO_SERVICIO` | 1 a N | Un vehÃ­culo genera mÃºltiples Ã³rdenes de servicio en el tiempo. |
| `TALLER` | Realiza | `REGISTRO_SERVICIO` | 1 a N | Una sucursal procesa mÃºltiples registros de servicio. |
| `USUARIO` | Recepciona | `REGISTRO_SERVICIO` | 1 a N | Un operador/recepcionista abre mÃºltiples registros. |
| `REGISTRO_SERVICIO`| Contiene | `DETALLE_SERVICIO` | 1 a N | Una orden agrupa uno o mÃ¡s detalles de servicio. |
| `SERVICIO` | Tipifica | `DETALLE_SERVICIO` | 1 a N | Un Ã­tem del catÃ¡logo puede aparecer en mÃºltiples Ã³rdenes. |
| `USUARIO` | Confecciona | `DETALLE_SERVICIO` | 1 a N | Un tÃ©cnico registra o ejecuta los Ã­tems del detalle. |
| `REGISTRO_SERVICIO`| Genera | `FACTURA` | 1 a 1 / 1 a N | El servicio cerrado emite el comprobante de liquidaciÃ³n. |
| `FACTURA` | Origina | `PAGO` | 1 a N | Una factura puede recibir uno o varios pagos parciales/totales. |
| `METODO_PAGO` | Aplica en | `PAGO` | 1 a N | Un mÃ©todo de pago se usa en mÃºltiples cobros. |

---

## 3. Reglas de Negocio Clave
1. **Consistencia de Kilometraje:** Al ingresar un `REGISTRO_SERVICIO`, el valor de `km_ingreso` debe ser igual o superior al `km_actual` registrado en la tabla `VEHICULO`, actualizando este Ãºltimo al finalizar el trabajo.
2. **HistÃ³rico de Precios:** El campo `precio` en `DETALLE_SERVICIO` es un snapshot del valor vigente en `SERVICIO` al momento de la intervenciÃ³n, para evitar descalces contables por futuras actualizaciones de tarifas.
3. **ValidaciÃ³n de Pagos:** La suma de los registros `PAGO.monto` vinculados a una `FACTURA` no puede exceder el `FACTURA.total`.


4. **Posesión Independiente:** Un vehículo puede existir en el sistema sin un cliente asignado inicialmente. La relación de propiedad se registra en la tabla PROPIETARIO_VEHICULO para conservar el historial de dueños frente a la venta de unidades.
