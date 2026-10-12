# Proposal: Identidad de Empresa Configurable (Logo y Datos del Taller)

## Why
Hoy la identidad de la empresa que aparece en los PDF está **hardcodeada** en `ReporteExtensions.ComposeEncabezadoTaller` ("TALLER PRO", "Ruta Nacional 11 Km 1005…", "Tel: +54 362 4123456"). El logo se resuelve con un override **por usuario** (`%LocalAppData%\TP_ControlVehicular\Empresa\logo.png`) que no se comparte ni se administra, y no existe ninguna pantalla para que el Administrador cargue el logo ni los datos del taller. Como resultado, cada instalación muestra una identidad de fantasía y no hay forma de personalizarla sin recompilar.

Esta entrega introduce una **configuración de empresa centralizada** (persistida en la base), accesible **solo al Administrador**, cuyo logo y datos alimentan el encabezado de **todos** los PDF.

## What Changes
- Nueva entidad singleton `ConfiguracionEmpresa` (una sola fila, `Id = 1`) con `Nombre`, `Direccion`, `Telefono`, `Email`, `Logo` (binario) y `FechaActualizacion`.
- Nuevo script idempotente de esquema/semilla (`bd/19_ConfiguracionEmpresa.sql`) y de permisos (`bd/20_PermisosConfiguracionEmpresa.sql`), porque el proyecto no mantiene migraciones EF.
- Repositorio + servicio `IEmpresaConfiguracionService` para leer/guardar la configuración.
- **Caché local materializada en `EmpresaBranding`**: al iniciar sesión y al guardar, la config se baja de la BD y se materializa en el logo local (`logo.png`) más un `empresa.json` con los metadatos; los reportes siguen leyendo la ruta local **sin cambiar el contrato** de `ReporteExtensions`.
- Nueva pantalla **Configuración de Empresa** (UserControl + ViewModel + formulario) visible **solo con el permiso `Configuracion.Empresa.Gestionar`** (Administrador).
- Encabezado de los PDF sensible a datos: `ComposeEncabezadoTaller` deja de estar hardcodeado y muestra **Nombre + Dirección + Teléfono** de la caché + el logo (regla ya vigente "todo PDF lleva logo").

## Impact
- **Capacidad nueva**: `configuracion-empresa`.
- **Capacidad modificada**: `reportes` (encabezado de PDF ahora data-driven).
- **Código afectado**: `Entidad/`, `Datos/Data/CVDbContext.cs`, `Datos/Configuracion/`, `Datos/Repositories/`, `Negocio/Interfaces/`, `Negocio/DTOs/`, `Negocio/Services/`, `Negocio/Reportes/Documentos/EmpresaBranding.cs` y `ReporteExtensions.cs`, `Presentacion/Pantalla/Configuracion/`, `Presentacion/ViewModels/`, `Presentacion/MainWindow.xaml`/`.xaml.cs`, `App.xaml.cs`.
- **Base de datos**: alta de tabla `ConfiguracionEmpresa`, fila semilla y permiso `Configuracion.Empresa.Gestionar`.
- **Sin cambios** en el contrato de generación de PDF (`ReporteExtensions` sigue exponiendo `ComposeEncabezadoTaller`/`ComposeLogo`/`ComposePieDePagina`).
- Los comprobantes (`ComprobanteRecepcionDocument`/`ComprobantePagoDocument`) ya muestran Nombre/Dirección/Teléfono + logo desde su DTO y quedan **fuera de alcance** (se evalúan en un change posterior si se decide unificar la fuente).

## Non-goals
- Backup/restauración de base de datos (iniciativa diferida).
- Manual de usuario (se entrega como documentación Markdown en `docs/manual/`, no requiere spec).
- Migraciones EF: el DDL se aplica con scripts idempotentes en `bd/`.
- Unificar el encabezado de los comprobantes con la nueva configuración.
