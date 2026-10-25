# Control de Permisos en Botones de Acción (RBAC)

## Overview
Para garantizar la seguridad y experiencia de usuario adecuada según su rol, es necesario ocultar o deshabilitar los botones de acción (`Nuevo`, `Editar`, `Eliminar`) en los listados (pantallas `Ctl*.xaml`) para aquellos usuarios que no posean los permisos correspondientes.

## Functional Requirements
1. **Control Centralizado de Permisos**:
   - Cada pantalla (o su ViewModel) debe ser capaz de consultar el rol actual del `UserSession` para determinar si el usuario tiene privilegios de Escritura (Crear/Editar) o Borrado (Eliminar).
2. **Impacto en la Interfaz de Usuario (Listados)**:
   - **Opción A (Ocultar - Recomendada)**: Si el usuario no tiene permisos, el botón desaparece de la vista. Esto previene ruido visual.
   - **Opción B (Deshabilitar)**: El botón se mantiene visible pero en color gris (inactivo), informando al usuario visualmente que la acción existe pero está restringida para él.
3. **Escalabilidad por Entidad**:
   - Los permisos pueden variar por módulo. Por ejemplo, un *Recepcionista* podría tener permisos para Crear/Editar `Clientes` y `Vehículos`, pero no para Crear `Talleres` ni `Roles`. 
   - El diseño debe permitir definir las reglas por cada módulo de forma individual.

## Acceptance Criteria
- [ ] Los botones de acción responden al rol del usuario que inició sesión.
- [ ] Si un rol no tiene permitido eliminar registros, el botón "Eliminar" en el listado desaparece (o se bloquea).
- [ ] Si un rol no tiene permitido crear registros, el botón "➕ Nuevo" desaparece (o se bloquea).
- [ ] El patrón arquitectónico utilizado respeta MVVM (sin inyectar lógica fuerte de sesión directo en los archivos `.xaml.cs`).
