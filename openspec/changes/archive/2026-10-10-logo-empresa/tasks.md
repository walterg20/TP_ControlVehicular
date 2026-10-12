# Tasks: Identidad de Empresa Configurable

- [x] 1. Esquema y permisos (BD)
  - [x] 1.1 Crear `bd/19_ConfiguracionEmpresa.sql` (tabla `ConfiguracionEmpresa` + fila semilla, idempotente).
  - [x] 1.2 Crear `bd/20_PermisosConfiguracionEmpresa.sql` (permiso `Configuracion.Empresa.Gestionar` → Admin `IdRol = 1`, idempotente).
  - [x] 1.3 Ejecutar ambos scripts sobre `ControlVehicular`.

- [x] 2. Capa de datos
  - [x] 2.1 Crear `Entidad/ConfiguracionEmpresa.cs`.
  - [x] 2.2 Crear `Datos/Configuracion/ConfiguracionEmpresaConfiguration.cs` (patrón `TallerConfiguration`).
  - [x] 2.3 Agregar `DbSet<ConfiguracionEmpresa>` en `Datos/Data/CVDbContext.cs`.
  - [x] 2.4 Crear `Negocio/Interfaces/IConfiguracionEmpresaRepository.cs` (`ObtenerAsync`).
  - [x] 2.5 Crear `Datos/Repositories/ConfiguracionEmpresaRepository.cs`.

- [x] 3. Negocio (servicio y DTO)
  - [x] 3.1 Crear `Negocio/DTOs/EmpresaConfiguracionDto.cs`.
  - [x] 3.2 Agregar mapeo `ConfiguracionEmpresa` → `EmpresaConfiguracionDto` en `MappingProfile.cs`.
  - [x] 3.3 Crear `Negocio/Interfaces/IEmpresaConfiguracionService.cs`.
  - [x] 3.4 Crear `Negocio/Services/EmpresaConfiguracionService.cs` (`ObtenerAsync`, `GuardarAsync`, `RefrescarCacheAsync`).

- [x] 4. Caché y encabezado de reportes
  - [x] 4.1 Extender `EmpresaBranding` (`Nombre`/`Direccion`/`Telefono`, `empresa.json`, `Aplicar(...)`, carga al arranque, fallback a defaults).
  - [x] 4.2 `ReporteExtensions.ComposeEncabezadoTaller` usa `EmpresaBranding` en vez de literales.
  - [x] 4.3 Verificar que `ComposeLogo` y el resto de reportes siguen funcionando sin cambios de contrato.

- [x] 5. Presentación (pantalla, VM, menú, DI)
  - [x] 5.1 Crear `Presentacion/Pantalla/Configuracion/CtlConfiguracion.xaml` + `.xaml.cs`.
  - [x] 5.2 Crear `Presentacion/ViewModels/EmpresaConfiguracionViewModel.cs`.
  - [x] 5.3 Registrar repositorio, servicio, VM y pantalla en `App.xaml.cs`; refrescar caché en login.
  - [x] 5.4 Agregar sección/menú **CONFIGURACIÓN** y navegación en `MainWindow.xaml`/`.xaml.cs`.
  - [x] 5.5 Gate por permiso `Configuracion.Empresa.Gestionar` en `AplicarRestriccionesPorRol()`.

- [x] 6. Verificación y cierre
  - [x] 6.1 `dotnet build "TP_ControlVehicular.slnx"` sin errores.
  - [x] 6.2 Probar: Admin ve la pantalla, carga logo y datos, guarda; un PDF muestra Nombre/Dirección/Teléfono + logo; Recepcionista/Mecánico NO ven la opción.
  - [x] 6.3 Actualizar `docs/referencia/reportes.md` y `docs/referencia/bd-y-dominio.md`.
  - [x] 6.4 Actualizar `MEMORY.md` y crear `docs/plan/2026-10-10_logo-empresa.md`.
