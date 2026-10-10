# Simplificación del Menú Administración

## Overview
Actualmente, el sistema expone accesos directos individuales para gestionar `Vehículos`, `Modelos de auto` y `Marcas` en la barra lateral bajo la sección de "ADMINISTRACIÓN". Para simplificar la interfaz y mejorar la experiencia del usuario, se requiere ocultar temporalmente o por defecto los accesos a `Modelos de auto` y `Marcas`, dejando únicamente visible el acceso a `Vehículos`.

## Functional Requirements
1. **Ocultamiento en la Barra Lateral**:
   - El botón correspondiente a `Modelos de auto` (`btnModelo`) no debe ser visible en el menú principal.
   - El botón correspondiente a `Marcas` (`btnMarca`) no debe ser visible en el menú principal.
2. **Impacto en Roles**:
   - Este ocultamiento aplica de manera global para todos los roles (incluyendo Administradores). Los menús ya no se mostrarán independientemente del nivel de permisos, hasta que se defina un nuevo diseño (por ejemplo, gestionar marcas y modelos dentro de la misma pantalla de vehículos).

## Acceptance Criteria
- [ ] Al iniciar sesión como Administrador, los botones "Modelos de autos" y "Marcas" no aparecen en el menú izquierdo.
- [ ] Al iniciar sesión con cualquier otro rol permitido, tampoco aparecen.
- [ ] El botón "Vehículos" sigue mostrándose y funcionando correctamente.
