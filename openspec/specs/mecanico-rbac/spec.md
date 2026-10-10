# Role-Based Access Control (RBAC): Rol Mecánico

## Overview
Implementar restricciones avanzadas de vista y datos para el rol de **Mecánico**. El objetivo es que los mecánicos solo puedan visualizar información (en el Dashboard y en los Reportes) relacionada exclusivamente a las órdenes de servicio en las que ellos hayan participado. Además, se debe ocultar completamente la cabecera "ADMINISTRACIÓN" del menú lateral, ya que no tienen acceso a ninguna de esas opciones.

## Functional Requirements
1. **Restricción de Menú (MainWindow)**:
   - Si el rol es *Mecánico*, además de ocultar los botones de Usuarios, Roles, Talleres, Clientes, Modelos, Marcas y Servicios, se debe ocultar el encabezado de la sección "ADMINISTRACIÓN" (`secAdministracion`).

2. **Filtro de Datos en Dashboard (`CtlDashboard`)**:
   - El Dashboard debe identificar al usuario logueado.
   - Si el usuario logueado es *Mecánico*, las estadísticas y contadores deben calcularse filtrando la base de datos para incluir solo los registros (`DetalleServicio`) donde `UsuarioId == id_del_mecanico_logueado`.
   - Las métricas afectadas son: cantidad de servicios completados, ganancias generadas, etc. (se calcularán con base en sus propios trabajos).

3. **Filtro de Datos en Reportes (`CtlReporteOrdenes`)**:
   - El reporte de órdenes debe identificar al usuario logueado.
   - Si el usuario logueado es *Mecánico*, la grilla solo debe listar los `RegistroServicio` (órdenes) que tengan al menos un `DetalleServicio` asignado a su `UsuarioId`.

4. **Contexto de Sesión**:
   - Proveer un mecanismo estandarizado para que los ViewModels puedan consultar quién es el usuario logueado (por ejemplo, a través de `((MainWindow)Application.Current.MainWindow).UsuarioSesionActual` o una clase estática `UserSession`).

## Acceptance Criteria
- [ ] La palabra "ADMINISTRACIÓN" no se muestra en el menú lateral cuando el rol es Mecánico.
- [ ] El Dashboard del mecánico muestra únicamente los servicios y montos asociados a él.
- [ ] El Reporte de Órdenes del mecánico lista solo aquellas órdenes en las que haya realizado al menos un servicio (tiene un detalle asignado a su ID).
- [ ] Si inicia sesión un usuario Administrador o Recepcionista, los datos se siguen mostrando globalmente (sin filtro por mecánico) y ven el menú correspondiente.
