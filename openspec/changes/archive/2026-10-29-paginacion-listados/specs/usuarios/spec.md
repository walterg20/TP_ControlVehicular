# Spec Delta

## MODIFIED Requirements

### Requirement: Listado de usuarios paginado en base de datos
La grilla de usuarios (`CtlUsuario`) SHALL recuperar sus registros desde la base de datos mediante paginación del lado del servidor, y la búsqueda por texto SHALL ejecutarse en la base de datos sobre las columnas `Nombre`, `Apellido`, `Dni` y `Email`, sin distinguir mayúsculas de minúsculas. El total de registros reportado SHALL corresponder al conjunto filtrado, y el cambio del texto de búsqueda SHALL reiniciar la página actual a 1.

#### Scenario: Carga inicial paginada
- **WHEN** se abre la pantalla de usuarios
- **THEN** el sistema recupera únicamente la primera página de usuarios desde la base de datos
- **AND** la barra de paginación indica la página actual, el total de páginas y el total de registros.

#### Scenario: Búsqueda en el servidor
- **WHEN** el usuario escribe un término y ejecuta la acción `Buscar` (o presiona `Enter`)
- **THEN** la base de datos filtra por `Nombre`, `Apellido`, `Dni` o `Email` conteniendo ese término, sin distinguir mayúsculas de minúsculas.

#### Scenario: Sin resultados
- **WHEN** el término de búsqueda no coincide con ningún usuario
- **THEN** la grilla queda vacía y la barra de paginación informa 0 registros.
