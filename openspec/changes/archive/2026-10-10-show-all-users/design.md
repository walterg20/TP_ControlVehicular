# Technical Design: Show All Users in Usuario Management

## 1. Business Layer (`Negocio/Services/ListarUsuariosHandler.cs`)
Update `HandleAsync()` to invoke `_usuarioRepository.GetWithRolAsync()` instead of `_usuarioRepository.GetActivosAsync()`.

```csharp
public async Task<List<UsuarioDto>> HandleAsync()
{
    var entities = await _usuarioRepository.GetWithRolAsync();
    return _mapper.Map<List<UsuarioDto>>(entities);
}
```

## 2. Presentation Layer (`Presentacion/`)
No structural UI changes needed. `UsuarioViewModel.LoadAsync()` populates `Usuarios` with the full list returned by `ListarUsuariosHandler`. Inactive users will display with `Estado = false` (unchecked in the DataGrid), and clicking `Cambiar Estado` will toggle their state between active and inactive.
