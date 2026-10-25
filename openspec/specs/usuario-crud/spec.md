# Usuario CRUD y Extensión de Datos
# Usuario CRUD y Extensión de Datos

## Descripción General
Extender la entidad Usuario para incluir los mismos datos de contacto y personales que posee la entidad Cliente (Apellido, DNI, Email, Teléfono, Domicilio y Fecha de Nacimiento). Implementar el ciclo completo de ABM (Alta, Baja lógica, Modificación y Listado) para la gestión de usuarios, incluyendo la interfaz de usuario con los estándares definidos del proyecto.

## Requerimientos Funcionales
1. **Extensión de Entidad**: La entidad Usuario debe incorporar los campos Apellido, Dni, Email, Telefono, Domicilio (texto largo para direcciones detalladas, ej: "mz 121 pc 07 400 viv. Soegype") y FechaNacimiento.
2. **Listado de Usuarios**: Vista (CtlUsuario) con un DataGrid que muestre la totalidad de usuarios (tanto activos como inactivos) y sus roles.
3. **Alta de Usuario**: Formulario modal (FrmUsuario) para crear un nuevo usuario con todos sus datos y asignación de rol.
4. **Modificación de Usuario**: Permite editar los datos de un usuario existente a través del mismo formulario modal.
5. **Baja Lógica / Cambio de Estado**: Botón para cambiar el Estado del usuario entre activo e inactivo previa confirmación con FrmConfirmacion.
12. **Validaciones**: Los campos DNI, Email, Nombre, Apellido, Domicilio y Teléfono deben ser obligatorios y validados al perder el foco. El teléfono debe aceptar formato de celular (ej. 362 4615825 o 3624615825). La edad del usuario según su Fecha de Nacimiento debe ser 18 años o mayor.
## Criterios de Aceptación
- [x] La base de datos contiene los nuevos campos en la tabla Usuarios tras aplicar la migración.
- [x] El DataGrid de CtlUsuario muestra correctamente la totalidad de los usuarios (activos e inactivos).
- [x] El modal FrmUsuario permite guardar un usuario y actualizar la lista en tiempo real.
- [x] Al cambiar el estado de un usuario, su campo Estado se actualiza en base de datos y se refleja visualmente (Check de Estado activo/inactivo) sin eliminarse.
