# Spec Delta

## MODIFIED Requirements

### Requirement: Listado de clientes paginado en base de datos
La grilla de clientes (`CtlCliente`) SHALL recuperar sus registros desde la base de datos mediante paginación del lado del servidor, y la búsqueda por texto SHALL ejecutarse en la base de datos sobre las columnas `Nombre`, `Apellido` y `Dni`, sin distinguir mayúsculas de minúsculas. El filtrado SHALL operar sobre la consulta paginada, de modo que el total de registros reportado corresponda al conjunto filtrado. El control de paginación SHALL reiniciar la página actual a 1 cuando el texto de búsqueda o la visibilidad de clientes inactivos cambien.

#### Scenario: Carga inicial paginada
- **WHEN** se abre la pantalla de clientes
- **THEN** el sistema recupera únicamente la primera página de clientes desde la base de datos
- **AND** la grilla se muestra poblada con los registros de esa página
- **AND** la barra de paginación indica la página actual, el total de páginas y el total de registros.

#### Scenario: Búsqueda en el servidor
- **WHEN** el usuario escribe un término de búsqueda y ejecuta la acción `Buscar` (o presiona `Enter`)
- **THEN** la base de datos filtra por `Nombre`, `Apellido` o `Dni` conteniendo ese término, sin distinguir mayúsculas de minúsculas
- **AND** la grilla muestra los registros coincidentes de la página actual
- **AND** el total de registros refleja el conjunto filtrado.

#### Scenario: Cambio de tamaño de página
- **WHEN** el usuario cambia la cantidad de filas por página
- **THEN** la página actual vuelve a 1
- **AND** la consulta se ejecuta con el nuevo tamaño de página.

#### Scenario: Sin resultados
- **WHEN** el término de búsqueda no coincide con ningún cliente
- **THEN** la grilla queda vacía y la barra de paginación informa 0 registros.
