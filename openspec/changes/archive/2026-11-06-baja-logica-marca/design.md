# Design: Baja lógica en Marca

## Decisions
- Reusar el patrón de Cliente/Taller/Servicio/Rol/Usuario.
- `Activo` default true en entidad y DDL.
- Filtro por ComboBox de 3 estados en la barra de búsqueda.
- Baja lógica vía `ModificarMarcaHandler` seteando `Activo = false`.
- RBAC: filtro visible solo para Admin; Recepcionista/Mecánico forzados a Activos.

## Risks
- Combos dependientes (Modelo→Marca) deben filtrar solo activas. Actualizar `ListarMarcaHandler.HandleAsync` para devolver solo activas.