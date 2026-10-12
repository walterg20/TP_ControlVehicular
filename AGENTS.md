# TP_ControlVehicular
Aplicación WPF .NET 10 MVVM para control vehicular con EF Core y SQL Server. Capas: Datos, Entidad, Migracion, Negocio, Presentacion.

> **Este archivo es la constitución**: stack, comandos, estructura, convenciones de código/UI y reglas que no se rompen.
> El conocimiento de **dominio** (reportes, dashboard, BD, pantallas) vive en `docs/referencia/` y se enlaza en *Referencia por dominio*.
> El **estado** del proyecto vive en `MEMORY.md` (pista) y en `git` (evidencia).

## Cómo encarar una tarea (checklist)
1. Leer `MEMORY.md` y contrastarlo con `git log -n 10` y `git status`; si discrepan, gana el repositorio.
2. Si la tarea no es trivial: proponer plan y esperar confirmación.
3. Definir la spec en `openspec/` **antes** de tocar código: `proposal.md` → `spec.md` → `design.md` → `tasks.md` (`spec.md` en inglés, criterios BDD Given/When/Then). Rama por spec: `git checkout -b feature/<nombre>`.
4. Implementar con cambios pequeños y enfocados; una tarea a la vez.
5. Verificar: `dotnet build "TP_ControlVehicular.slnx"` sin errores (+ `dotnet test` cuando aplique) y el comportamiento esperado.
6. Actualizar `MEMORY.md` (y replicar aquí toda convención nueva).
7. Commit atómico (Conventional Commits). Al cerrar la spec: merge y archivar el change en `openspec/`.

## Stack
- Lenguaje: C# 13 (.NET 10.0, WinExe, Nullable + ImplicitUsings)
- Framework / runtime: WPF (UseWPF), Microsoft.Extensions.DependencyInjection/Hosting
- Base de datos: SQL Server (Server=<SERVIDOR>\<INSTANCIA>) vía EF Core 10.0.11 (Microsoft.EntityFrameworkCore.SqlServer)
- Mapeo: AutoMapper 13.x (perfil Entity→DTO unidireccional)
- Tests: `TP_ControlVehicular.Tests` (xUnit + Moq)

## Comandos
- `dotnet build "TP_ControlVehicular.slnx"` — compila la solución (PowerShell: usa `;` no `&&`)
- `dotnet run --project "TP_ControlVehicular.csproj"` — ejecuta la app en local
- `dotnet test "TP_ControlVehicular.slnx"` — corre los tests (xUnit + Moq)

## Estructura del proyecto
- `App.xaml.cs` — composición DI, config desde `appsettings.json` con `SetBasePath(AppContext.BaseDirectory)`
- `Datos/` — `CVDbContext`, repositorios base (`Repository<T>`) e implementaciones (`IClienteRepository`, `IVehiculoRepository`, `IModeloRepository`, `IMarcaRepository`)
- `Entidad/` — entidades `Marca`, `Cliente`, `Modelo`, `Vehiculo` (convenciones: `string.Empty`, `new List<T>()`, `null!` en navs)
- `Migracion/` — migraciones EF Core y `CVDbContextModelSnapshot.cs`
- `Negocio/` — handlers (`RegistrarClienteHandler`, `RegistrarVehiculoHandler`, `ListarVehiculosPorClienteHandler`), `MappingProfile.cs` (Entity→DTO), DTOs, reportes QuestPDF
- `Presentacion/` — `ViewModels/` (`BaseViewModel`, `ClienteViewModel`, `ModeloViewModel`, `MarcaViewModel`, `VehiculoViewModel`), `Pantalla/*/Ctl*.xaml.cs` (UserControls), `MainWindow`
- `bd/` — scripts SQL idempotentes (esquema, permisos, SP) · `docs/` — documentación · `openspec/` — specs

## Convenciones de código
- **DI en `App.xaml.cs`**: repositorios `AddScoped`, handlers `AddScoped`, VMs `AddScoped`/`AddTransient`, `MainWindow` singleton.
- **Entidades**: propiedades `string` → `string.Empty`; colecciones de navegación → `new List<T>()`; referencias de navegación → `null!`.
- **Handlers**: inyectan interfaz de repositorio + `IMapper`; `HandleAsync` retorna `_mapper.Map<Dto>(entidad)`.
- **AutoMapper**: perfil unidireccional Entity→DTO con nombres compuestos (`ClienteDto`, `VehiculoDto`, etc.).
- **ViewModels**: heredan `BaseViewModel` (`INotifyPropertyChanged`, `SetProperty<T>`), `RelayCommand` con `async () =>`, `ObservableCollection<Dto>` en ctor, campos `private readonly` inyectados (repo + opcional `IMapper`).
  - **Excepción `VehiculoViewModel`**: clase plana, no hereda `BaseViewModel`, construye `Entidad.Vehiculo` directo.
  - `ClienteViewModel` usa `OnPropertyChanged()` en setters de propiedades.
- **Inyección de dependencias**: ambos patrones son válidos — handlers usan inyección basada en handler para escritura/validación; `VehiculoViewModel` usa inyección directa de repositorio (patrón aceptado).

## UI — UserControls (`Ctl*.xaml`)
- `Grid` principal con `Margin="10"`.
- Estructura de 4 filas (`Row 0`: Encabezado/Título, `Row 1`: Barra de búsqueda + Botón `➕ Nuevo [Entidad]`, `Row 2`: Tarjeta con DataGrid, `Row 3`: Tarjeta inferior/Pie del listado con botones `✏️ Editar` y `🗑️ Eliminar` / `Cambiar Estado`).
- Tarjetas blancas con `CornerRadius="12"`, `DropShadowEffect` (`Opacity="0.08"`, `Direction="270"`, `BlurRadius="10"` o `12`, `ShadowDepth="2"`).
- Botones y TextBox dentro de tarjetas usan `Resources > Style` para `CornerRadius="6"` en lugar de sobreescribir la plantilla (Template).
- DataGrid con encabezados oscuros (`Background="#2C3E50"`, `Foreground="White"`, `FontWeight="Bold"`, `Padding="10,8"`), `Background="Transparent"`, `BorderThickness="0"`, `RowHeaderWidth="0"`.
- Paleta de colores de botones: `➕ Nuevo` (`#27AE60` Verde), `✏️ Editar` (`#F39C12` Naranja), `🗑️ Eliminar` (`#C0392B` Rojo), `🔄 Actualizar / Buscar` (`#2980B9` Azul).
- Code-behind (`Ctl*.xaml.cs`): registrar el evento `Loaded` una sola vez en el constructor principal para evitar ejecuciones concurrentes de `LoadAsync()` sobre el DbContext.
- **Nomenclatura**: al declarar, recuperar del DI o castear el DataContext a un ViewModel en el code-behind, nombrar la variable con el prefijo `vm` + el nombre de la entidad (ej: `vmOrdenServicio`, `vmCliente`). **Nunca** usar simplemente `vm`.

## UI — Formularios modales (`Frm*.xaml`)
- `Grid` principal con `Margin="24"`.
- Estructura de 3 filas (`Row 0`: Cabecera, `Row 1`: Formulario dentro de `ScrollViewer`, `Row 2`: Botones).
- Layout del Formulario: `Grid` con 2 columnas (`Column 0`: Width="130" para etiquetas, `Column 1`: Width="*" para inputs en `StackPanel`), alineados horizontalmente estilo "side-by-side".
- Entradas TextBox/ComboBox con `Height="30"`, `UpdateSourceTrigger=LostFocus, ValidatesOnNotifyDataErrors=True` y mensajes de error en rojo (`#E74C3C`).
- `Owner = Window.GetWindow(this)` en code-behind.
- Método `ConfigurarValidacionAlPerderFoco()` en code-behind para disparar validación en tiempo real.
- Botón **Guardar / Aceptar**: `#27AE60` (Verde), `IsDefault="True"`. Botón **Cancelar**: `#C0392B` (Rojo). Ambos usan `Resources > Style` para `CornerRadius="6"`.

## Popups de Confirmación / Eliminación y Avisos (`FrmConfirmacion`)
- NO usar el `MessageBox.Show` nativo de Windows para confirmaciones (`YesNo`) ni avisos/alertas de selección.
- Para confirmaciones (Sí/No): `FrmConfirmacion.Mostrar(mensaje, titulo, Window.GetWindow(this))` de `Presentacion.Pantalla.Compartido`.
- Para avisos/alertas de selección ("Por favor seleccione un registro..."): `FrmConfirmacion.MostrarAviso(mensaje, titulo, Window.GetWindow(this))` — mismo estilo con un único botón **Aceptar** (`#27AE60`).
- Encabezado oscuro (`#2C3E50`); **Aceptar**: `#27AE60` (`✓ Sí, Aceptar` / `✓ Aceptar`); **Cancelar**: `#C0392B` (`❌ No, Cancelar`).

## Referencia por dominio (leer solo si la tarea lo toca)
| Dominio | Archivo |
|---|---|
| Reportes (PDF/logo, RBAC, granularidad, SP) | `docs/referencia/reportes.md` |
| Dashboard por perfil | `docs/referencia/dashboard.md` |
| Base de datos, scripts `bd/` y estados de orden | `docs/referencia/bd-y-dominio.md` |
| Pantalla *Mis Trabajos* (mecánico) | `docs/referencia/mis-trabajos.md` |
| Respaldo de BD y resguardo en Google Drive | `docs/referencia/backup.md` |
| DER (a respetar en todo feature) | `docs/DER.md` |

## Mapa de herramientas
- **CodeGraph** — explorar/buscar código antes de grep o leer archivos. MCP `codegraph_explore` (o CLI `codegraph explore "<symbols>"`). Índice en `.codegraph/` (git-ignored); si no existe, se omite (indexar es decisión del usuario).
- **OpenSpec** (`openspec/`) — SDD obligatorio antes de modificar código. Skills/workflows `openspec-*` y comandos `opsx-*` en `.opencode/` y `.agents/`.
- **Subagentes SDD** (`.opencode/agents/`): `coordinator`, `planner`, `implementer`, `reviewer`.
- **MCP servers** (`mcp.json`): `codegraph`, `github`, `sqlite`.
- **Base de datos**: SQL Server `<SERVIDOR>\<INSTANCIA>` vía EF Core 10; migraciones en `Migracion/`; scripts idempotentes en `bd/`.
- **Tests**: `TP_ControlVehicular.Tests` (xUnit + Moq), `dotnet test` · **Scripts temporales**: `docs/script/` (.py/.ps1).

## No hagas
- Todos los scripts temporales (.py, .ps1, etc.) utilizados durante el desarrollo DEBEN colocarse o moverse a `docs/script/` para mantener la raíz limpia.
- No editar archivos de código o XAML usando `Set-Content` o `>`/`>>` en PowerShell. Usar `[System.IO.File]::WriteAllText("ruta", $contenido, [System.Text.Encoding]::UTF8)` para evitar corromper emojis y acentos. El proyecto debe mantenerse rigurosamente en UTF-8.
- No usar `&&` en PowerShell; usa `;` para encadenar comandos.
- No rutas sin comillas: la ruta del proyecto tiene espacios y caracteres no ASCII — siempre entre comillas.
- No modificar AGENTS.md de plantilla; no tocar plantillas de referencia.
- No asumir patrones genéricos: confía en fuentes ejecutables (config, scripts, código real) sobre prosa.
- No agregar dependencias sin avisar.
- No usar `ProcesarPagoHandler` (código muerto: `HandleAsync` lanza `NotImplementedException`); el cobro real se realiza con `IBillingService.RegistrarPago`.
- Commits atómicos en convención Conventional Commits; monorepo único.
- No subir secretos: credenciales solo por variables de entorno / perfiles.
- El estado real del proyecto se verifica contra `git log` / `git status`, nunca contra `MEMORY.md`. La memoria es una pista; el repositorio es la evidencia. Si discrepan, el repositorio gana y la memoria se corrige.

## Flujo de trabajo
- **Ramas por Spec**: para cada spec (feature) crear una rama (`git checkout -b feature/<nombre>`), trabajar ahí hasta terminar y luego fusionar.
- **Commitear en la rama feature, nunca en `develop`**: la implementación se confirma en `feature/<nombre>`. Si el editor/IDE auto-commitea cambios directamente en `develop`, la rama feature queda "no mergeada" y `git branch -d` se niega a borrarla; hay que mergear igual para poder limpiarla.
- **Uso de `MEMORY.md`**:
  - Al INICIAR una tarea: leer `MEMORY.md` como pista y contrastarlo con `git log -n 10` y `git status`. Si discrepan, el repositorio gana y se corrige `MEMORY.md`.
  - Al CERRAR una spec (antes del merge): actualizar `MEMORY.md` con las nuevas convenciones, reglas, scripts SQL, permisos y decisiones, y commitearlo junto con la spec (`docs(memory): ...`).
  - `MEMORY.md` debe mantenerse alineado con `AGENTS.md`: toda regla nueva en uno se replica en el otro.
- **Planes de implementación (.md)**: todo plan, walkthrough o documento de trabajo en Markdown generado durante el desarrollo DEBE guardarse en `docs/plan/` dentro del repo. Nombre: `AAAA-MM-DD_<nombre-spec>.md`; walkthrough: `..._walkthrough.md`. Se commitea junto con la spec (`docs(plan): ...`). Diferencia con OpenSpec: `openspec/` = especificación formal (inglés); `docs/plan/` = plan/bitácora de ejecución (español).
- Antes de una tarea no trivial, proponer un plan y esperar confirmación.
- Una tarea a la vez; al terminar, informar qué cambiaste para revisión.
- Si no estás seguro al 80%, pregunta. No inventes.
- Haz solo lo que se pide: no añadas funcionalidades por tu cuenta.
- Cambios pequeños y enfocados; no reescribas lo que ya funciona.
- Al terminar, resume qué has cambiado y cualquier decisión que deba revisar.

## Documentación
- `docs/DER.md` — diagrama de entidad relación a tener en cuenta en todo feature o spec.
- `docs/constitution.md` — constitución de la especificación del proyecto.
- `docs/referencia/` — conocimiento de dominio (reportes, dashboard, BD, pantallas) y `docs/referencia/legacy/` (material histórico).
- `docs/plan/` — planes y walkthroughs de ejecución (español), ver *Flujo de trabajo*.
- `docs/script/` — scripts temporales de desarrollo.
- `openspec/` — especificación formal (inglés): `specs/` (vigentes), `changes/` (en curso) y `changes/archive/` (cerradas).
- `README.md` — solo placeholders, sin contenido real.
- Código fuente en capas: `Datos/`, `Entidad/`, `Migracion/`, `Negocio/`, `Presentacion/`.

## Agent Guidelines
- Usar **CodeGraph** siempre que se pueda para buscar y explorar código (MCP `codegraph_explore` o CLI `codegraph explore`). Si no hay `.codegraph/`, omitirlo.
- Manipular archivos solo con la API de .NET / scripts Python (`docs/script/`) si hace falta, para evitar corrupción UTF-8 en PowerShell.
- Usar **OpenSpec** (`openspec/`) obligatoriamente: definir `proposal.md`, `spec.md`, `design.md`, `tasks.md` **antes** de escribir o modificar código de implementación. Las specs van en inglés.
- Validar todos los criterios de aceptación y escenarios BDD con la verificación de build antes de declarar completada una tarea.
