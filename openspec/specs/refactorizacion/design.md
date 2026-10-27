# 006 · Refactorización — Plan

_Cómo se implementa lo descrito en `spec.md`. Debe respetar `AGENTS.md`._

## Enfoque

Refactorizar sistemáticamente por capas (`Presentacion`, `Negocio`, `Datos`) los nombres de variables, campos y parámetros genéricos a nombres explícitos de dominio. No se modifica ninguna lógica funcional ni comportamiento en runtime; únicamente se mejoran los identificadores.

## Implementación

1. **Capa Presentación - UserControls y Formularios (`Presentacion/Pantalla/*`)**:
   - Renombrar instancias de ViewModels en code-behind (`vm`, `vm2`, `_vm`) a `vmCliente`, `vmMarca`, `vmModelo`, `vmVehiculo`, `vmTaller`.
2. **Capa Presentación - ViewModels (`Presentacion/ViewModels/*`)**:
   - Renombrar repositorios inyectados (`_repo`, `repo`, `_vehiculoRepo`, etc.) a `_clienteRepository`, `_marcaRepository`, `_modeloRepository`, `_vehiculoRepository`, `_tallerRepository`.
   - Renombrar handlers inyectados en `TallerViewModel` a `_listarTallerHandler`, `_registrarTallerHandler`, `_modificarTallerHandler`.
3. **Capa Negocio - Handlers (`Negocio/Services/*`)**:
   - Renombrar campos/parámetros de repositorios inyectados a sus nombres de dominio explícitos (`clienteRepository`, `vehiculoRepository`, `modeloRepository`, `tallerRepository`).
4. **Capa Datos - Repositorios (`Datos/Repositories/*`)**:
   - Renombrar `_context` / `context` a `_cvDbContext` / `cvDbContext`.
5. **Verificación y Build**:
   - Ejecutar `dotnet build "TP_ControlVehicular.slnx"` y verificar compilación exitosa.

## Decisiones

- **Refactorización quirúrgica sin cambios funcionales**: preserva bindings de WPF y dependencias en `App.xaml.cs`.
- **Nombres explícitos de dominio**: uso de camelCase con prefijos `_` para campos privados (`_clienteRepository`) y camelCase sin prefijo para variables locales / parámetros (`vmCliente`, `clienteRepository`).

## Riesgos

- **Inconsistencia de nombres en lambdas o castings XAML**: Mitigación: revisión exhaustiva archivo por archivo y prueba de build con `dotnet build`.
