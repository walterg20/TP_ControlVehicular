# Unificar Nomenclatura de Baja Lógica — Delta Specification

## ADDED Requirements

### Requirement: Active Flag Naming Is Unified

`Rol` and `Usuario` SHALL use `Activo` (not `Estado`) as their logical-deletion flag, consistent with `Cliente`, `Taller` and `Servicio`.

#### Scenario: Rol exposes Activo

- **WHEN** a `Rol` is instantiated or loaded
- **THEN** its logical-deletion flag is `Activo`

#### Scenario: Usuario exposes Activo

- **WHEN** a `Usuario` is instantiated or loaded
- **THEN** its logical-deletion flag is `Activo`

#### Scenario: Database column is Activo

- **WHEN** `01_estructura.sql` is applied
- **THEN** `Roles` and `Usuarios` have `[Activo] bit NOT NULL DEFAULT (1)`

### Requirement: Rol Supports Listing Inactive And Reactivating

`CtlRol` SHALL provide "Mostrar inactivos" and "Reactivar" consistent with the other entities.

#### Scenario: Toggle lists inactive roles

- **WHEN** "Mostrar inactivos" is toggled in `CtlRol`
- **THEN** inactive roles appear in the grid

#### Scenario: Reactivate role

- **WHEN** `ReactivarRolHandler.HandleAsync(id)` is called for an inactive role
- **THEN** the role's `Activo` becomes `true`
