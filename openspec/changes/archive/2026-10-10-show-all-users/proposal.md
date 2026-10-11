# Change Proposal: Show Active and Inactive Users in Usuario Management

## Problem
Currently, `ListarUsuariosHandler` calls `_usuarioRepository.GetActivosAsync()`, which only returns active users (`Estado == true`). Administrators are unable to view or reactivate inactive users (`Estado == false`) from the user management screen (`CtlUsuario`).

## Proposed Solution
Modify `ListarUsuariosHandler` to fetch all users using `_usuarioRepository.GetWithRolAsync()`. This ensures administrators can view all users (both active and inactive) in `CtlUsuario` and toggle their active/inactive status seamlessly.

## Impact
- `Negocio/Services/ListarUsuariosHandler.cs`: Change repository method call from `GetActivosAsync()` to `GetWithRolAsync()`.
- `openspec/specs/usuario-crud/spec.md`: Update specification requirement and acceptance criteria to reflect listing of both active and inactive users.
