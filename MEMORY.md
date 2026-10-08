# TP_ControlVehicular
Aplicación WPF .NET 10 MVVM para control vehicular con EF Core y SQL Server. Capas: Datos, Entidad, Migracion, Negocio, Presentacion.

## Stack
- Lenguaje: C# 13 (.NET 10.0, WinExe, Nullable + ImplicitUsings)
- Framework / runtime: WPF (UseWPF), Microsoft.Extensions.DependencyInjection/Hosting
- Base de datos: SQL Server (Server=WALTERG20\SQLEXPRESS2019) vía EF Core 10.0.11 (Microsoft.EntityFrameworkCore.SqlServer)
- Mapeo: AutoMapper 13.x (perfil Entity→DTO unidireccional)
- Tests: (no configurado - pero apto para TDD)

## Comandos
- `dotnet build "TP_ControlVehicular.slnx"` — compila la solución (PowerShell: usa `;` no `&&`)
- `dotnet run --project "TP_ControlVehicular.csproj"` — ejecuta la app en local
- `dotnet ef database update` — aplica migraciones (desde carpeta Migracion)

## Estructura del proyecto
- `App.xaml.cs` — composición DI, `db.Database.Migrate()` al inicio, config desde `appsettings.json` con `SetBasePath(AppContext.BaseDirectory)`
- `Datos/` — `CVDbContext`, repositorios base (`Repository<T>`) e implementaciones (`IClienteRepository`, `IVehiculoRepository`, `IModeloRepository`, `IMarcaRepository`)
- `Entidad/` — entidades `Marca`, `Cliente`, `Modelo`, `Vehiculo` (convenciones: `string.Empty`, `new List<T>()`, `null!` en navs)
- `Migracion/` — `CVDbContextModelSnapshot.cs` y `20260827020907_InitialCreate.cs` (ya aplicadas)
- `Negocio/` — handlers (`RegistrarClienteHandler`, `RegistrarVehiculoHandler`, `ListarVehiculosPorClienteHandler`), `MappingProfile.cs` (Entity→DTO), DTOs
- `Presentacion/` — `ViewModels/` (`BaseViewModel`, `ClienteViewModel`, `ModeloViewModel`, `MarcaViewModel`, `VehiculoViewModel`), `Pantalla/*/Ctl*.xaml.cs` (UserControls), `MainWindow`
- `openspec/` — Directorio de especificaciones OpenSpec (propuestas, specs, diseño, tareas).

## Flujo de Trabajo y Arquitectura: OpenSpec (SDD + TDD)
- **Specification-Driven Development (SDD):** TODA característica nueva o modificación significativa debe definirse primero en el directorio `openspec/`.
  - Pasos: `proposal.md` -> `spec.md` -> `design.md` -> `tasks.md`.
  - No se escribe código de implementación sin antes tener las especificaciones y diseños aprobados y validados.
  - Las `spec.md` deben ser redactadas en Inglés e incluir Criterios de Aceptación claros (preferiblemente estilo BDD: Given/When/Then).
- **Test-Driven Development (TDD):** El desarrollo de implementación se guía por las especificaciones (y tests).
  - Ciclo de desarrollo enfocado en asegurar que se cumplan primero los Criterios de Aceptación (OpenSpec).
  - Se debe validar funcionalmente cada escenario (idealmente con pruebas si están configuradas) antes de refactorizar o avanzar.
  - La verificación de construcción (build) y validación del comportamiento son obligatorias antes de marcar una tarea como completada.

## Convenciones
- **DI en `App.xaml.cs`**: repositorios `AddScoped`, handlers `AddScoped`, VMs `AddScoped`/`AddTransient`, `MainWindow` singleton.
- **Entidades**: propiedades `string` → `string.Empty`; colecciones de navegación → `new List<T>()`; referencias de navegación → `null!`.
- **Handlers**: inyectan interfaz de repositorio + `IMapper`; `HandleAsync` retorna `_mapper.Map<Dto>(entidad)`.
- **AutoMapper**: perfil unidireccional Entity→DTO con nombres compuestos (`ClienteDto`, `VehiculoDto`, etc.).
- **ViewModels**: heredan `BaseViewModel` (`INotifyPropertyChanged`, `SetProperty<T>`), `RelayCommand` con `async () =>`, `ObservableCollection<Dto>` en ctor, campos `private readonly` inyectados (repo + opcional `IMapper`).
  - **Excepción `VehiculoViewModel`**: clase plana, no hereda `BaseViewModel`, construye `Entidad.Vehiculo` directo.
  - `ClienteViewModel` usa `OnPropertyChanged()` en setters de propiedades.
- **Diseño UI en UserControls (`Ctl*.xaml`)**:
  - `Grid` principal con `Margin="10"`.
  - Estructura de 4 filas (`Row 0`: Encabezado/Título, `Row 1`: Barra de búsqueda + Botón `➕ Nuevo [Entidad]`, `Row 2`: Tarjeta con DataGrid, `Row 3`: Tarjeta inferior/Pie del listado con botones `✏️ Editar` y `🗑️ Eliminar` / `Cambiar Estado`).
  - Tarjetas blancas con `CornerRadius="12"`, `DropShadowEffect` (`Opacity="0.08"`, `Direction="270"`, `BlurRadius="10"` o `12`, `ShadowDepth="2"`).
  - Botones y TextBox dentro de tarjetas usan `Resources > Style` para `CornerRadius="6"` en lugar de sobreescribir la plantilla (Template).
  - DataGrid con encabezaos oscuros (`Background="#2C3E50"`, `Foreground="White"`, `FontWeight="Bold"`, `Padding="10,8"`), `Background="Transparent"`, `BorderThickness="0"`, `RowHeaderWidth="0"`.
  - Paleta de colores de botones: `➕ Nuevo` (`#27AE60` Verde), `✏️ Editar` (`#F39C12` Naranja), `🗑️ Eliminar` (`#C0392B` Rojo), `🔄 Actualizar / Buscar` (`#2980B9` Azul).
  - Code-behind (`Ctl*.xaml.cs`): Registrar el evento `Loaded` una sola vez en el constructor principal para evitar ejecuciones concurrentes de `LoadAsync()` sobre el DbContext.
- **Diseño UI y Validaciones en Formularios Modales (`Frm*.xaml`)**:
  - `Grid` principal con `Margin="24"`.
  - Estructura de 3 filas (`Row 0`: Cabecera, `Row 1`: Formulario dentro de `ScrollViewer`, `Row 2`: Botones).
  - Layout del Formulario: `Grid` con 2 columnas (`Column 0`: Width="130" para etiquetas, `Column 1`: Width="*" para inputs en `StackPanel`), alineados horizontalmente estilo "side-by-side".
  - Entradas TextBox/ComboBox con `Height="30"`, `UpdateSourceTrigger=LostFocus, ValidatesOnNotifyDataErrors=True` y mensajes de error en rojo (`#E74C3C`).
  - `Owner = Window.GetWindow(this)` en code-behind.
  - Método `ConfigurarValidacionAlPerderFoco()` en code-behind para disparar validación en tiempo real.
  - Botón **Guardar / Aceptar**: `#27AE60` (Verde, igual a Nuevo), `IsDefault="True"`. Usa `Resources > Style` para `CornerRadius="6"`.
  - Botón **Cancelar**: `#C0392B` (Rojo). Usa `Resources > Style` para `CornerRadius="6"`.
- **Popups de Confirmación / Eliminación y Avisos (`FrmConfirmacion`)**:
  - NO usar el `MessageBox.Show` nativo de Windows para solicitudes de confirmación (`YesNo`) ni avisos/alertas de selección.
  - Para confirmaciones (Sí/No): Invocar siempre `FrmConfirmacion.Mostrar(mensaje, titulo, Window.GetWindow(this))` de `Presentacion.Pantalla.Compartido`.
  - Para avisos/alertas de selección de registros ("Por favor seleccione un registro..."): Invocar siempre `FrmConfirmacion.MostrarAviso(mensaje, titulo, Window.GetWindow(this))` que muestra el mismo estilo personalizado con un único botón **Aceptar** (`#27AE60` Verde).
  - Encabezado oscuro (`#2C3E50`), Botón **Aceptar**: `#27AE60` (Verde, `✓ Sí, Aceptar` / `✓ Aceptar`), Botón **Cancelar**: `#C0392B` (Rojo, `❌ No, Cancelar`).
- **Inyección de dependencias**: ambos patrones son válidos en el código actual — handlers usan inyección basada en handler para lógica de escritura/validación; `VehiculoViewModel` usa inyección directa de repositorio (patrón aceptado).

## No hagas
- No usar `&&` en PowerShell; usa `;` para encadenar comandos.
- No rutas sin comillas: la ruta del proyecto tiene espacios y caracteres no ASCII — siempre entre comillas.
- No modificar `AGENTS_Template.md` ni la carpeta `spec_template/` (son plantillas de referencia).
- No asumir patrones genéricos: confía en fuentes ejecutables (config, scripts, código real) sobre prosa.
- No agregar dependencias sin avisar.
- Commits atómicos en convención Conventional Commits; monorepo único.
- No subir secretos: credenciales solo por variables de entorno / perfiles.
- El estado real del proyecto se verifica contra `git log` / `git status`, nunca contra `MEMORY.md`. La memoria es una pista; el repositorio es la evidencia. Si discrepan, el repositorio gana y la memoria se corrige.

## Flujo de trabajo general
- Antes de una tarea no trivial, propón un plan y espera confirmación.
- Una tarea a la vez; al terminar, informa qué cambiaste para revisión.
- Si no estás seguro al 80%, pregunta. No inventes.
- Haz solo lo que se pide: no añadas funcionalidades por tu cuenta.
- Cambios pequeños y enfocados; no reescribas lo que ya funciona.
- Al terminar, resume qué has cambiado y cualquier decisión que deba revisar.

## Documentación
- `AGENTS_Template.md` — plantilla de estructura (no modificar).
- `README.md` — solo placeholders, sin contenido real.
- `spec_template/` — plantillas de especificación vacías.
- Código fuente en capas: `Datos/`, `Entidad/`, `Migracion/`, `Negocio/`, `Presentacion/`.

## Agent Guidelines
- You MUST ALWAYS use OpenSpec (`openspec/` directory) to define feature proposals (`proposal.md`), specifications (`spec.md`), technical designs (`design.md`), and implementation tasks (`tasks.md`) BEFORE writing or modifying implementation code.
- OpenSpec specification files MUST be written in English.
- Ensure all acceptance criteria and BDD scenarios defined via OpenSpec are validated with build verification before declaring completion.
