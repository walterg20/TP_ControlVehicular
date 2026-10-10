import os

def create_spec(path, content):
    os.makedirs(os.path.dirname(path), exist_ok=True)
    with open(path, 'w', encoding='utf-8') as f:
        f.write(content.strip())

# 1. Permisos Avanzados
create_spec(r'e:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular\openspec\specs\permisos-avanzados\spec.md', '''
# Especificación: Permisos Avanzados Dinámicos (RBAC)

## 1. Objetivo
Migrar el sistema actual de permisos (hardcodeado en la interfaz por ID de rol) a un sistema dinámico basado en base de datos. Esto permitirá configurar desde una pantalla de administración qué acciones específicas (Ver, Crear, Editar, Eliminar) tiene permitidas cada rol.

## 2. Requerimientos
* Crear una nueva entidad Permiso y una tabla intermedia RolPermiso en la base de datos.
* Interfaz de usuario para que un Administrador asigne o quite permisos a un rol específico usando CheckBoxes.
* Modificar MainWindow.xaml.cs para leer los permisos de la base de datos al iniciar sesión y habilitar/ocultar los botones del menú lateral según los permisos.
* En cada pantalla (ej: Vehículos, Clientes), ocultar o deshabilitar los botones de "Nuevo", "Editar" y "Eliminar" según el permiso granular.

## 3. Criterios de Aceptación
- [ ] La base de datos tiene las tablas de Permisos y su relación con Rol.
- [ ] La pantalla de Roles permite editar la lista de permisos asignados.
- [ ] Un usuario con sesión iniciada solo ve en el menú lateral las pantallas para las que tiene permiso.
''')

# 2. Mis Trabajos
create_spec(r'e:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular\openspec\specs\mis-trabajos\spec.md', '''
# Especificación: Panel "Mis Trabajos"

## 1. Objetivo
Proveer una interfaz optimizada para los mecánicos donde puedan ver rápidamente los servicios (ítems) que les fueron asignados, conocer el vehículo y registrar su finalización.

## 2. Requerimientos
* Mostrar una grilla (DataGrid) enfocada en DetalleServicio en lugar de RegistroServicio completo.
* Filtro automático: si el usuario es Mecánico, la grilla solo carga los detalles asignados a su ID.
* Si el usuario es Administrador (o Recepcionista con permisos), incluir un ComboBox para filtrar por un mecánico específico o ver "Todos".
* La grilla debe mostrar: Fecha de Orden, Patente, Tarea (Servicio), Estado (Pendiente, En Proceso, Finalizada).
* Botón/Checkbox rápido en la fila para marcar el ítem como "Terminado" directamente desde esta pantalla sin abrir la orden completa.

## 3. Criterios de Aceptación
- [ ] El mecánico solo ve los trabajos asignados a él.
- [ ] Se puede marcar un trabajo como finalizado desde la grilla.
- [ ] Al cambiar el estado de un trabajo desde aquí, se evalúa si toda la Orden debe cambiar a estado "Completada".
''')

# 3. Reportes
create_spec(r'e:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular\openspec\specs\reportes\spec.md', '''
# Especificación: Módulo de Reportes

## 1. Objetivo
Generar información estadística y reportes imprimibles/exportables para el taller, separando la visión operativa de la visión gerencial.

## 2. Requerimientos
* **Reporte de Órdenes:** Un PDF imprimible para entregar al cliente cuando deja el vehículo o cuando se le entrega la factura (Comprobante de Recepción / Comprobante de Pago).
* **Reporte Operativo:** Cantidad de órdenes atendidas por cada mecánico en un rango de fechas. Vehículos más frecuentes.
* **Reporte Gerencial:** Ingresos generados ($) en un rango de fechas, separados por tipo de servicio.
* (Opcional) Uso de librerías como iText7 o QuestPDF para la generación de PDFs.

## 3. Criterios de Aceptación
- [ ] Se puede exportar el reporte operativo a un formato visible (PDF o Excel).
- [ ] Se puede imprimir el comprobante de recepción de una Orden de Servicio.
''')

print("Specs creados con éxito.")
