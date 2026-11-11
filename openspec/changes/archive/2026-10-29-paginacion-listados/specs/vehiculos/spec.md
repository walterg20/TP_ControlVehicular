# Spec Delta

## MODIFIED Requirements

### Requirement: Listado de vehículos paginado en base de datos
La grilla de vehículos (`CtlVehiculo`) SHALL recuperar sus registros desde la base de datos mediante paginación del lado del servidor, y la búsqueda por texto SHALL ejecutarse en la base de datos sobre las columnas de identificación del vehículo y del propietario, sin distinguir mayúsculas de minúsculas. El total de registros reportado SHALL corresponder al conjunto filtrado, y el cambio del texto de búsqueda SHALL reiniciar la página actual a 1.

#### Scenario: Carga inicial paginada
- **WHEN** se abre la pantalla de vehículos
- **THEN** el sistema recupera únicamente la primera página de vehículos desde la base de datos
- **AND** la barra de paginación indica la página actual, el total de páginas y el total de registros.

#### Scenario: Búsqueda en el servidor
- **WHEN** el usuario escribe un término y ejecuta la acción `Buscar` (o presiona `Enter`)
- **THEN** la base de datos aplica el término sobre la patente y los datos del vehículo y del propietario, sin distinguir mayúsculas de minúsculas
- **AND** la grilla muestra los registros coincidentes de la página actual.

#### Scenario: Sin resultados
- **WHEN** el término de búsqueda no coincide con ningún vehículo
- **THEN** la grilla queda vacía y la barra de paginación informa 0 registros.
