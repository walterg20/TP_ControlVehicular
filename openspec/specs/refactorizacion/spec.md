# 006 · Refactorización de Nombres de Variables Explícitas

**Estado:** implementado ✅

## Qué hace

Refactoriza los nombres de variables genéricas (`vm`, `vm2`, `_repo`, `repo`, `_handler`, `handler`, `_context`) en todas las capas del proyecto (`Presentacion`, `Negocio`, `Datos`, `App.xaml.cs`) para utilizar nombres explícitos con sufijo o prefijo de dominio (ej. `vmCliente`, `vmTaller`, `clienteRepository`, `registrarClienteHandler`, `_cvDbContext`).

## Por qué

Mejora la legibilidad, mantenibilidad y auto-documentación del código fuente, reduciendo la ambigüedad en el origen de las dependencias, instanciaciones y bindings, facilitando el desarrollo y la auditoría del proyecto.

## Criterios de aceptación

- [x] Todas las referencias a ViewModels en archivos code-behind (`Ctl*.xaml.cs` y `Frm*.xaml.cs`) utilizan nombres explícitos como `vmCliente`, `vmMarca`, `vmModelo`, `vmVehiculo`, `vmTaller`.
- [x] Todos los campos y parámetros de repositorios inyectados en ViewModels y Handlers utilizan nombres explícitos de dominio (`_clienteRepository`, `_marcaRepository`, `_modeloRepository`, `_vehiculoRepository`, `_tallerRepository`).
- [x] Todos los campos y parámetros de Handlers (Casos de Uso) inyectados en ViewModels utilizan nombres explícitos (`_listarTallerHandler`, `_registrarTallerHandler`, `_modificarTallerHandler`, etc.).
- [x] El campo del contexto EF Core en la capa de datos (`Repository<T>`) utiliza un nombre explícito (`_cvDbContext`).
- [x] La solución `TP_ControlVehicular.slnx` compila con 0 errores tras la refactorización (`dotnet build "TP_ControlVehicular.slnx"`).
- [x] No se alteró la funcionalidad existente ni la lógica de negocio/UI de la aplicación.

## Fuera de alcance

- Cambios en nombres de archivos o carpetas del proyecto.
- Cambios en las interfaces o contratos públicos de repositorios y handlers.
- Modificación de esquema de base de datos o migraciones EF Core.
