# Pruebas Automatizadas (Login, ABM Usuarios y RBAC)

## Descripción General
Implementar un conjunto de pruebas automatizadas (Unit/Integration Tests) para garantizar la calidad y prevenir regresiones en los flujos críticos de la aplicación. Las pruebas se centrarán en la lógica de autenticación (Login), la gestión de usuarios (ABM) y el control de acceso basado en roles (RBAC) que oculta elementos del menú y botones de edición/eliminación.

Dado que la aplicación es en WPF utilizando el patrón MVVM, las pruebas se ejecutarán principalmente sobre los **ViewModels** inyectando repositorios falsos (Mocks) o una base de datos en memoria (In-Memory Database de EF Core).

## Requerimientos Funcionales
1. **Configuración del Proyecto de Pruebas**: Crear un nuevo proyecto de tipo xUnit (ej. TP_ControlVehicular.Tests) y referenciar las demás capas.
2. **Pruebas de Autenticación (Login)**:
   - *Caso de Éxito*: Ingresar DNI y Clave válidos y verificar que devuelva éxito.
   - *Caso de Error*: Ingresar DNI/Clave incorrectos y verificar el mensaje de error.
3. **Pruebas de ABM de Usuarios (Usuario)**:
   - *Alta*: Verificar que al ejecutar el comando guardar con datos válidos, se invoque el repositorio correctamente.
   - *Validaciones*: Verificar que si faltan datos obligatorios, el ViewModel genere errores de validación.
4. **Pruebas de Control de Accesos (RBAC)**:
   - *Visibilidad de Menú*: Simular un Recepcionista y verificar que menú de Usuarios esté oculto.
   - *Botones CRUD*: Simular un Mecánico y validar que no pueda ver los botones de Agregar Cliente/Vehículo.

## Criterios de Aceptación
- [ ] El proyecto de pruebas ejecuta exitosamente (dotnet test).
- [ ] Cobertura de login de éxito y fallos.
- [ ] Cobertura de validaciones y guardado de usuarios.
- [ ] Pruebas sobre los permisos de UI según roles.
