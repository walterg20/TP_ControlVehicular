# Design: Clean Unused Usings and Add Author Comment

## Architectural Context
WPF .NET 10 solution. Source files are `.cs` across Datos, Entidad, Migracion, Negocio, Presentacion, and Tests.

## Technical Strategy
1. Use `dotnet format` with diagnostic IDE0005 to remove only unnecessary using directives.
2. Run a Python script (in `docs/script/`) to prepend `// walterg20` to each `.cs` file that does not already start with it.
3. Verify build and tests.
