# ERS – Sistema de Control Vehicular y Mantenimiento

## Capítulo 1 – Introducción

### 1.1 Propósito
Definir los requisitos funcionales y no funcionales del sistema de gestión de vehículos, clientes, talleres y mantenimientos, incluyendo la comunicación mediante avisos sms y correos electrónicos.

### 1.2 Alcance
El sistema permitirá:
- Registrar clientes, vehículos, talleres y servicios.
- Configurar plantillas de mantenimiento con intervalos por kilometraje o tiempo.
- Registrar servicios y sus detalles asociados.
- Emitir avisos de vencimiento o confirmación de mantenimiento.
- Gestionar usuarios, roles y control de acceso al sistema.
- Exportar reportes y mantener trazabilidad completa de las operaciones.

### 1.3 Definiciones y siglas
- **ERS**: Especificación de Requisitos de Software.
- **CRUD**: Create, Read, Update, Delete (Operaciones básicas).
- **DER**: Diagrama Entidad-Relación.
- **UML**: Lenguaje Unificado de Modelado.

### 1.4 Referencias
No aplica.

### 1.5 Visión general del documento
El presente documento se estructura en siete capítulos: el **Capítulo 1** introduce el propósito y alcance; el **Capítulo 2** detalla los requisitos funcionales; el **Capítulo 3** los no funcionales; el **Capítulo 4** presenta los casos de uso; el **Capítulo 5** el modelo de datos; el **Capítulo 6** los diagramas UML y, finalmente, el **Capítulo 7** concluye el trabajo.

---

## Capítulo 2 – Requisitos Funcionales

| Código | Requisito | Descripción | Prioridad |
| :--- | :--- | :--- | :--- |
| **RF‑01** | Gestión de vehículos | Registrar, editar y eliminar vehículos asociados a clientes, incluyendo modelo, marca, año, patente y kilometraje actual. | Alta |
| **RF‑02** | Gestión de clientes | Registrar datos personales del cliente (nombre, apellido, DNI, dirección, fecha de nacimiento) y su estado activo. | Alta |
| **RF‑03** | Gestión de talleres | Registrar talleres con nombre, dirección, teléfono y estado activo. | Media |
| **RF‑04** | Gestión de servicios | Registrar tipos de servicio y asociarlos a plantillas y registros de mantenimiento. | Alta |
| **RF‑05** | Registro de servicios | Guardar fecha, kilometraje, notas, comprobante, mecánico y recepcionista. Permitir múltiples detalles por servicio. | Alta |
| **RF‑06** | Gestión de usuarios | Crear, editar, eliminar usuarios, asignar roles/permisos y restablecer contraseñas de usuarios. | Alta |
| **RF‑07** | Exportación de reportes | Generar reportes de mantenimiento, avisos y servicios en formato digital (PDF/Excel). | Media |
| **RF‑08** | Gestión de acceso | Validar credenciales de acceso (usuario y contraseña) para iniciar sesión en la aplicación. | Alta |

---

## Capítulo 3 – Requisitos No Funcionales

- **RNF‑01: Rendimiento** → Respuesta rápida en operaciones CRUD y generación de avisos.
- **RNF‑02: Usabilidad** → Interfaz intuitiva.
- **RNF‑03: Seguridad** → Cifrado de datos, autenticación segura y control de roles.
- **RNF‑04: Integridad referencial** → Validación de claves foráneas entre entidades.

---

## Capítulo 4 – Casos de Uso

| Código | Caso de Uso | Actor | Descripción |
| :--- | :--- | :--- | :--- |
| **CU‑01** | Registrar vehículo | Recepcionista | El usuario ingresa datos del vehículo y los guarda en el sistema. |
| **CU‑02** | Configurar mantenimiento | Mecánico | Define intervalos de mantenimiento por km o tiempo en plantillas. |
| **CU‑03** | Registrar servicio | Mecánico | Registra un servicio realizado, con detalles y comprobante. |
| **CU‑04** | Exportar Reporte | Administrador | Genera reportes digitales de mantenimientos y avisos. |
| **CU‑05** | Registrar cliente | Recepcionista | Registra datos personales y contactos del cliente. |
| **CU‑06** | Registrar taller | Administrador | Registra datos del taller y su estado activo. |
| **CU‑07** | Gestionar usuario | Administrador | Crea, edita o elimina usuarios. Asigna roles y restablece contraseñas a solicitud. |
| **CU‑08** | Iniciar sesión | Todos | El usuario ingresa sus credenciales para acceder a la aplicación. |

---

## Capítulo 5 – Modelo Conceptual de Datos

Se inserta el **DER** como Entidades principales: ROL, USUARIO, CLIENTE, CONTACTO, VEHICULO, MARCA, MODELO, TALLER, SERVICIO, DETALLE_SERVICIO, REGISTRO_SERVICIO, PLANTILLA_MANTENIMIENTO, AVISO.

*Figura 1 – Diagrama Entidad Relación*.

---

## Capítulo 6 – Diagramas UML

- **Casos de Uso**

*Figura 2 – Diagrama Caso de uso*.

---

## Capítulo 7 – Conclusión

El sistema evoluciona hacia una solución de gestión vehicular y mantenimiento, con trazabilidad completa desde el cliente hasta el aviso por sms/mail. El nuevo modelo de datos garantiza consistencia, escalabilidad y comunicación eficiente, integrando seguridad y control de acceso, junto a roles, clientes, talleres y servicios.