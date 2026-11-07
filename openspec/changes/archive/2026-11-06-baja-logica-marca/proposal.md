# Proposal: Baja lógica en Marca

## Why
Marca aún no soporta baja lógica: eliminar borra físicamente y no hay filtro por estado. Se unifica con el patrón ya aplicado a Cliente/Taller/Servicio/Rol/Usuario: `Activo`, filtro Todos/Activos/Inactivos, baja lógica, reactivación y color rojo en inactivos, con visibilidad del filtro solo para Admin.

## What Changes
- Agregar `Activo` a `Entidad.Marca`, `MarcaDto`, DDL y migración.
- Repositorio y handler soportan filtro por estado y baja lógica.
- `MarcaViewModel`: propiedad `FiltroEstado`, baja lógica en `EliminarMarcaAsync`, `ReactivarMarcaAsync`.
- `CtlMarca.xaml`: ComboBox de filtro en la barra de búsqueda, RowStyle rojo/rosado para inactivos, botón Reactivar.
- RBAC: filtro oculto para Recepcionista/Mecánico (solo activos); visible para Admin.

## Capabilities
### Modified Capabilities
- `marcas`: baja lógica + filtro por estado + reactivación + estilo visual.

## Impact
- Código: `Entidad/Marca.cs`, `Negocio/DTOs/MarcaDto.cs`, `Negocio/Interfaces/IMarcaRepository.cs`, `Negocio/Services/MarcaRepository.cs`, `ListarMarcaHandler.cs`, `ModificarMarcaHandler.cs`, `MarcaViewModel.cs`, `CtlMarca.xaml`, `CtlMarca.xaml.cs`, `bd/01_estructura.sql`.