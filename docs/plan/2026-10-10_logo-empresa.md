# Plan / Bitácora — Identidad de Empresa Configurable (`logo-empresa`)

> Change OpenSpec: `2026-10-10-logo-empresa` (capabilities `configuracion-empresa` y `reportes`).
> Spec publicada: `openspec/specs/configuracion-empresa/spec.md` (+ ampliación de `reportes`).
> Bitácora reconstruida al archivar el change (tarea 6.4 pendiente); refleja los artefactos entregados en el repositorio.

## Objetivo
Permitir que el Administrador configure la identidad corporativa (nombre, dirección, teléfono, email y logo)
desde una pantalla, y que tanto la interfaz como los PDF usen esos datos en lugar de literales hardcodeados.

## Decisiones clave
- **BD como fuente de verdad, archivo local como caché**: los reportes ya leen la ruta local de forma
  sincrónica; mantener ese contrato evita tocar cada documento PDF.
- **Tabla singleton (`ConfiguracionEmpresa`, `Id = 1`) en vez de reutilizar `Taller`**: `Taller` es un catálogo
  multi-registro; la identidad corporativa del sistema es una sola y semánticamente distinta.
- **Scripts SQL idempotentes en vez de migraciones EF**: coincide con cómo se gestiona el esquema real.
- **Comprobantes fuera de alcance**: ya muestran los tres campos + logo desde su DTO; unificar la fuente se
  evalúa por separado.

## Artefactos entregados
- **Esquema/permisos**: tabla `ConfiguracionEmpresa` + fila semilla y permiso `Configuracion.Empresa.Gestionar`
  (Admin `IdRol = 1`), idempotentes.
- **Datos**: `Entidad/ConfiguracionEmpresa.cs`, `Datos/Configuracion/ConfiguracionEmpresaConfiguration.cs`,
  `DbSet` en `Datos/Data/CVDbContext.cs`, `Negocio/Interfaces/IConfiguracionEmpresaRepository.cs`,
  `Datos/Repositories/ConfiguracionEmpresaRepository.cs`.
- **Negocio**: `Negocio/DTOs/EmpresaConfiguracionDto.cs`, mapeo en `MappingProfile.cs`,
  `Negocio/Interfaces/IEmpresaConfiguracionService.cs`, `Negocio/Services/EmpresaConfiguracionService.cs`.
- **Caché/encabezado**: `EmpresaBranding` extendido (`Nombre`/`Direccion`/`Telefono`, `empresa.json`,
  `Aplicar(...)`, carga al arranque, fallback a defaults) y `ReporteExtensions.ComposeEncabezadoTaller`
  leyendo de `EmpresaBranding`.
- **Presentación**: `Presentacion/Pantalla/Configuracion/CtlConfiguracion.xaml` (+ `.xaml.cs`),
  `Presentacion/ViewModels/EmpresaConfiguracionViewModel.cs`, registro en `App.xaml.cs`, sección
  **CONFIGURACIÓN** y gating por permiso en `MainWindow.xaml(.cs)`.

## Verificación
- `dotnet build "TP_ControlVehicular.slnx"` sin errores.
- Prueba manual: Admin ve la pantalla, carga logo y datos y guarda; un PDF muestra Nombre/Dirección/Teléfono +
  logo; Recepcionista/Mecánico **no** ven la opción.
- Documentación: `docs/referencia/reportes.md` y `docs/referencia/bd-y-dominio.md` actualizadas.

## Notas
- Los scripts originales de este change (`bd/19_ConfiguracionEmpresa.sql`, `bd/20_PermisosConfiguracionEmpresa.sql`)
  fueron **consolidados** en los canónicos por tipo (`bd/01_estructura.sql` y `bd/04_datos.sql`) durante el change
  `2026-10-12-reorganizacion-bd`.
