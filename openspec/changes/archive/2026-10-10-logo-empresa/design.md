# Technical Design: Identidad de Empresa Configurable

## Design Details

### 1. Modelo de datos

**Entidad** `Entidad/ConfiguracionEmpresa.cs` (singleton, `Id = 1`):

```csharp
public class ConfiguracionEmpresa
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string? Email { get; set; }
    public byte[]? Logo { get; set; }            // binario del logo (fuente de verdad)
    public DateTime FechaActualizacion { get; set; }
}
```

Convenciones del proyecto: `string` → `string.Empty`; sin navegación.

**Configuración EF** `Datos/Configuracion/ConfiguracionEmpresaConfiguration.cs` (`IEntityTypeConfiguration<ConfiguracionEmpresa>`), siguiendo el patrón de `TallerConfiguration`:
- `ToTable("ConfiguracionEmpresa")`, `HasKey(c => c.Id)`.
- `Nombre` (100), `Direccion` (200), `Telefono` (20) required; `Email` (150) opcional; `Logo` `varbinary(max)`.

**`DbSet`** en `Datos/Data/CVDbContext.cs`: `public DbSet<ConfiguracionEmpresa> ConfiguracionEmpresa { get; set; } = null!;`

**Esquema** (`bd/19_ConfiguracionEmpresa.sql`, idempotente — el proyecto **no mantiene migraciones EF**):
- `IF OBJECT_ID('ConfiguracionEmpresa','U') IS NULL CREATE TABLE ...`.
- `IF NOT EXISTS (SELECT 1 FROM ConfiguracionEmpresa) INSERT` la fila semilla con los valores actuales hardcodeados (TALLER PRO / Ruta Nacional 11 Km 1005 - Resistencia, Chaco / +54 362 4123456 / contacto@taller.com) y `Logo = NULL`.

**Permisos** (`bd/20_PermisosConfiguracionEmpresa.sql`, idempotente, patrón `bd/12`):
- `INSERT INTO Permisos (Nombre) VALUES ('Configuracion.Empresa.Gestionar')` con `WHERE NOT EXISTS`.
- Asignar a Administrador (`IdRol = 1`) con `WHERE NOT EXISTS` en `RolPermisos`. No se asigna a Recepcionista (2) ni Mecánico (3).

### 2. Repositorio y servicio

**Repositorio** `Datos/Repositories/ConfiguracionEmpresaRepository.cs` sobre `Repository<T>` +
`Negocio/Interfaces/IConfiguracionEmpresaRepository.cs` con `Task<ConfiguracionEmpresa?> ObtenerAsync()` (siempre la fila `Id = 1`).

**Servicio** `Negocio/Interfaces/IEmpresaConfiguracionService.cs`:
- `Task<EmpresaConfiguracionDto> ObtenerAsync()`
- `Task<EmpresaConfiguracionDto> GuardarAsync(EmpresaConfiguracionDto dto, byte[]? logoBytes)`
- `Task RefrescarCacheAsync()` — baja la config de la BD y llama a `EmpresaBranding.Aplicar(...)`.

Implementación `Negocio/Services/EmpresaConfiguracionService.cs`: usa el repositorio + `IMapper`; al guardar setea `FechaActualizacion = DateTime.Now`, y si `logoBytes` no es null reemplaza `Logo`; luego refresca la caché.

**DTO** `Negocio/DTOs/EmpresaConfiguracionDto.cs`: `Nombre`, `Direccion`, `Telefono`, `Email`, `TieneLogo` (bool derivado), `FechaActualizacion`. `MappingProfile` agrega el mapeo unidireccional `ConfiguracionEmpresa` → `EmpresaConfiguracionDto`.

### 3. Caché de branding (`EmpresaBranding`)

Punto único de resolución del logo (ya existente). Se extiende para:
- Guardar metadatos estáticos en memoria: `Nombre`, `Direccion`, `Telefono`, con **fallback** a los valores por defecto actuales.
- Persistir los metadatos en `%LocalAppData%\TP_ControlVehicular\Empresa\empresa.json` y cargarlos al arranque (así los PDF siguen siendo correctos aunque no se vuelva a loguear).
- `Aplicar(string nombre, string direccion, string telefono, byte[]? logo)`: escribe `logo.png` (si hay binario) y `empresa.json` (siempre).
- Mantener `RutaLogo` / `RutaLogoOverride` / `RutaLogoCompartido` sin cambios (`RutaLogo` sigue prefiriendo el override local y cayendo al asset embebido).

`ReporteExtensions.ComposeEncabezadoTaller` reemplaza los literales por `EmpresaBranding.Nombre/Direccion/Telefono`. El título del reporte de la derecha no cambia. Los reportes que usan este encabezado (`ReporteOperativoViewModel`, `MisTrabajosViewModel`, `ReporteGerencialViewModel`) no requieren cambios.

### 4. UI — Pantalla de Configuración

- `Presentacion/Pantalla/Configuracion/CtlConfiguracion.xaml` + `.xaml.cs` (UserControl), siguiendo el patrón de 4 filas/tarjetas de los `Ctl*` existentes; incluye previsualización del logo (`Image`), botón "Seleccionar imagen" (`OpenFileDialog`), campos `Nombre`/`Direccion`/`Telefono`/`Email` y botón **Guardar** (`#27AE60`, `CornerRadius=6`).
- `Presentacion/ViewModels/EmpresaConfiguracionViewModel.cs` heredando `BaseViewModel`, con `RelayCommand` async, validación de campos requeridos y `ObservableCollection`/propiedades para el binding.
- El code-behind lee el archivo seleccionado (`File.ReadAllBytes`) y llama al servicio; usa `FrmConfirmacion.MostrarAviso`/`Mostrar` para validaciones y confirmación de guardado (nunca `MessageBox`).

### 5. Menú y RBAC

- `Presentacion/MainWindow.xaml`: nueva sección **CONFIGURACIÓN** con botón `⚙️ Configuración de Empresa`.
- `Presentacion/MainWindow.xaml.cs`: handler de click que resuelve `CtlConfiguracion` del DI y lo muestra; en `AplicarRestriccionesPorRol()` se oculta/muestra según `Configuracion.Empresa.Gestionar` (patrón existente de gating por `UsuarioDto.Permisos`).
- `App.xaml.cs`: registrar `IConfiguracionEmpresaRepository` (`AddScoped`), `IEmpresaConfiguracionService` (`AddScoped`), `EmpresaConfiguracionViewModel` (`AddScoped`) y `CtlConfiguracion` (`AddTransient`). En el login exitoso invocar `RefrescarCacheAsync()`.

### 6. Orden de aplicación
1. Scripts `bd/19` y `bd/20` sobre `ControlVehicular`.
2. Entidad + configuración + `DbSet`.
3. Repositorio + servicio + DTO + mapeo.
4. `EmpresaBranding` + `ComposeEncabezadoTaller`.
5. Pantalla + VM + DI + menú + gating.
6. `dotnet build` y prueba manual de PDF con logo/datos cargados.

## Trade-offs / decisiones
- **BD como fuente de verdad, archivo local como caché**: los reportes ya leen la ruta local de forma sincrónica; mantener ese contrato evita tocar cada documento PDF.
- **Tabla singleton en vez de reutilizar `Taller`**: `Taller` es un catálogo de talleres (multi-registro, con `Activo`); la identidad corporativa del sistema es una sola y semánticamente distinta.
- **Scripts idempotentes en vez de migraciones EF**: coincide con cómo se gestiona hoy el esquema real del proyecto.
- **Comprobantes fuera de alcance**: ya muestran los tres campos + logo desde su DTO; unificar la fuente se evaluará por separado.
