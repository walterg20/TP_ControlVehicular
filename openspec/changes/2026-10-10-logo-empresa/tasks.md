# Tasks: Identidad de Empresa Configurable

- [ ] 1. Esquema y permisos (BD)
  - [ ] 1.1 Crear `bd/19_ConfiguracionEmpresa.sql` (tabla `ConfiguracionEmpresa` + fila semilla, idempotente).
  - [ ] 1.2 Crear `bd/20_PermisosConfiguracionEmpresa.sql` (permiso `Configuracion.Empresa.Gestionar` → Admin `IdRol = 1`, idempotente).
  - [ ] 1.3 Ejecutar ambos scripts sobre `ControlVehicular`.

- [ ] 2. Capa de datos
  - [ ] 2.1 Crear `Entidad/ConfiguracionEmpresa.cs`.
  - [ ] 2.2 Crear `Datos/Configuracion/ConfiguracionEmpresaConfiguration.cs` (patrón `TallerConfiguration`).
  - [ ] 2.3 Agregar `DbSet<ConfiguracionEmpresa>` en `Datos/Data/CVDbContext.cs`.
  - [ ] 2.4 Crear `Negocio/Interfaces/IConfiguracionEmpresaRepository.cs` (`ObtenerAsync`).
  - [ ] 2.5 Crear `Datos/Repositories/ConfiguracionEmpresaRepository.cs`.

- [ ] 3. Negocio (servicio y DTO)
  - [ ] 3.1 Crear `Negocio/DTOs/EmpresaConfiguracionDto.cs`.
  - [ ] 3.2 Agregar mapeo `ConfiguracionEmpresa` → `EmpresaConfiguracionDto` en `MappingProfile.cs`.
  - [ ] 3.3 Crear `Negocio/Interfaces/IEmpresaConfiguracionService.cs`.
  - [ ] 3.4 Crear `Negocio/Services/EmpresaConfiguracionService.cs` (`ObtenerAsync`, `GuardarAsync`, `RefrescarCacheAsync`).

- [ ] 4. Caché y encabezado de reportes
  - [ ] 4.1 Extender `EmpresaBranding` (`Nombre`/`Direccion`/`Telefono`, `empresa.json`, `Aplicar(...)`, carga al arranque, fallback a defaults).
  - [ ] 4.2 `ReporteExtensions.ComposeEncabezadoTaller` usa `EmpresaBranding` en vez de literales.
  - [ ] 4.3 Verificar que `ComposeLogo` y el resto de reportes siguen funcionando sin cambios de contrato.

- [ ] 5. Presentación (pantalla, VM, menú, DI)
  - [ ] 5.1 Crear `Presentacion/Pantalla/Configuracion/CtlConfiguracion.xaml` + `.xaml.cs`.
  - [ ] 5.2 Crear `Presentacion/ViewModels/EmpresaConfiguracionViewModel.cs`.
  - [ ] 5.3 Registrar repositorio, servicio, VM y pantalla en `App.xaml.cs`; refrescar caché en login.
  - [ ] 5.4 Agregar sección/menú **CONFIGURACIÓN** y navegación en `MainWindow.xaml`/`.xaml.cs`.
  - [ ] 5.5 Gate por permiso `Configuracion.Empresa.Gestionar` en `AplicarRestriccionesPorRol()`.

- [ ] 6. Verificación y cierre
  - [ ] 6.1 `dotnet build "TP_ControlVehicular.slnx"` sin errores.
  - [ ] 6.2 Probar: Admin ve la pantalla, carga logo y datos, guarda; un PDF muestra Nombre/Dirección/Teléfono + logo; Recepcionista/Mecánico NO ven la opción.
  - [ ] 6.3 Actualizar `docs/referencia/reportes.md` y `docs/referencia/bd-y-dominio.md`.
  - [ ] 6.4 Actualizar `MEMORY.md` y crear `docs/plan/2026-10-10_logo-empresa.md`.
