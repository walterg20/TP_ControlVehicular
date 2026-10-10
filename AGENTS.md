# TP_ControlVehicular
Aplicación WPF .NET 10 MVVM para control vehicular con EF Core y SQL Server. Capas: Datos, Entidad, Migracion, Negocio, Presentacion.

## Stack
- Lenguaje: C# 13 (.NET 10.0, WinExe, Nullable + ImplicitUsings)
- Framework / runtime: WPF (UseWPF), Microsoft.Extensions.DependencyInjection/Hosting
- Base de datos: SQL Server (Server=WALTERG20\SQLEXPRESS2019) vía EF Core 10.0.11 (Microsoft.EntityFrameworkCore.SqlServer)
- Mapeo: AutoMapper 13.x (perfil Entity→DTO unidireccional)
- Tests: `TP_ControlVehicular.Tests` (xUnit + Moq)

## Comandos
- `dotnet build "TP_ControlVehicular.slnx"` — compila la solución (PowerShell: usa `;` no `&&`)
- `dotnet run --project "TP_ControlVehicular.csproj"` — ejecuta la app en local

## Estructura del proyecto
- `App.xaml.cs` — composición DI, config desde `appsettings.json` con `SetBasePath(AppContext.BaseDirectory)`
- `Datos/` — `CVDbContext`, repositorios base (`Repository<T>`) e implementaciones (`IClienteRepository`, `IVehiculoRepository`, `IModeloRepository`, `IMarcaRepository`)
- `Entidad/` — entidades `Marca`, `Cliente`, `Modelo`, `Vehiculo` (convenciones: `string.Empty`, `new List<T>()`, `null!` en navs)
- `Migracion/` — migraciones EF Core y `CVDbContextModelSnapshot.cs`
- `Negocio/` — handlers (`RegistrarClienteHandler`, `RegistrarVehiculoHandler`, `ListarVehiculosPorClienteHandler`), `MappingProfile.cs` (Entity→DTO), DTOs
- `Presentacion/` — `ViewModels/` (`BaseViewModel`, `ClienteViewModel`, `ModeloViewModel`, `MarcaViewModel`, `VehiculoViewModel`), `Pantalla/*/Ctl*.xaml.cs` (UserControls), `MainWindow`

## Convenciones
- **DI en `App.xaml.cs`**: repositorios `AddScoped`, handlers `AddScoped`, VMs `AddScoped`/`AddTransient`, `MainWindow` singleton.
- **Entidades**: propiedades `string` → `string.Empty`; colecciones de navegación → `new List<T>()`; referencias de navegación → `null!`.
- **Handlers**: inyectan interfaz de repositorio + `IMapper`; `HandleAsync` retorna `_mapper.Map<Dto>(entidad)`.
- **AutoMapper**: perfil unidireccional Entity→DTO con nombres compuestos (`ClienteDto`, `VehiculoDto`, etc.).
- **ViewModels**: heredan `BaseViewModel` (`INotifyPropertyChanged`, `SetProperty<T>`), `RelayCommand` con `async () =>`, `ObservableCollection<Dto>` en ctor, campos `private readonly` inyectados (repo + opcional `IMapper`).
  - **Excepción `VehiculoViewModel`**: clase plana, no hereda `BaseViewModel`, construye `Entidad.Vehiculo` directo.
  - `ClienteViewModel` usa `OnPropertyChanged()` en setters de propiedades.
  - **Pantalla *Mis Trabajos* (mecánico) — maestro-detalle**: `MisTrabajosViewModel` expone `Ordenes`/`OrdenesView` (filas `MiOrdenItemViewModel`, una por orden con tareas propias; progreso `hechas/total`) y `TareasDeOrden` (filas `MiTareaItemViewModel`, las tareas del mecánico de la orden seleccionada; `Observaciones` y `Realizado` editables inline). Orden por nº de orden descendente; filtro `Mostrar` = Pendientes/Finalizados/Todos. Doble clic en una orden abre `CtlOrdenServicioForm` (origen `"MisTrabajos"`).
- **Diseño UI en UserControls (`Ctl*.xaml`)**:
  - `Grid` principal con `Margin="10"`.
  - Estructura de 4 filas (`Row 0`: Encabezado/Título, `Row 1`: Barra de búsqueda + Botón `➕ Nuevo [Entidad]`, `Row 2`: Tarjeta con DataGrid, `Row 3`: Tarjeta inferior/Pie del listado con botones `✏️ Editar` y `🗑️ Eliminar` / `Cambiar Estado`).
  - Tarjetas blancas con `CornerRadius="12"`, `DropShadowEffect` (`Opacity="0.08"`, `Direction="270"`, `BlurRadius="10"` o `12`, `ShadowDepth="2"`).
  - Botones y TextBox dentro de tarjetas usan `Resources > Style` para `CornerRadius="6"` en lugar de sobreescribir la plantilla (Template).
  - DataGrid con encabezaos oscuros (`Background="#2C3E50"`, `Foreground="White"`, `FontWeight="Bold"`, `Padding="10,8"`), `Background="Transparent"`, `BorderThickness="0"`, `RowHeaderWidth="0"`.
  - Paleta de colores de botones: `➕ Nuevo` (`#27AE60` Verde), `✏️ Editar` (`#F39C12` Naranja), `🗑️ Eliminar` (`#C0392B` Rojo), `🔄 Actualizar / Buscar` (`#2980B9` Azul).
  - Code-behind (`Ctl*.xaml.cs`): Registrar el evento `Loaded` una sola vez en el constructor principal para evitar ejecuciones concurrentes de `LoadAsync()` sobre el DbContext.
  - **Nomenclatura**: Al declarar, recuperar del DI o castear el DataContext a un ViewModel en el code-behind, nombrar la variable con el prefijo `vm` + el nombre de la entidad (ej: `vmOrdenServicio`, `vmCliente`). **Nunca** usar simplemente `vm`.
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
- **Reportes PDF (QuestPDF) — logo obligatorio**:
  - TODO PDF generado por el sistema (reportes, hojas de trabajo, comprobantes) DEBE mostrar el logo de la empresa en el encabezado.
  - Reportes: usar siempre `page.Header().Element(c => c.ComposeEncabezadoTaller("Título"))` y `page.Footer().Element(c => c.ComposePieDePagina())` de `Negocio/Reportes/Documentos/ReporteExtensions.cs`.
  - Documentos con encabezado propio (comprobantes): incluir el logo con `.Element(c => c.ComposeLogo())`.
  - Prohibido crear `ComposeHeader`/`ComposeFooter` privados que no muestren el logo, o usar `page.Header().Text(...)`. Los subtítulos y filtros van al inicio de `page.Content()`.
  - El logo se resuelve en un único punto: `EmpresaBranding` (`Negocio/Reportes/Documentos/EmpresaBranding.cs`). Busca primero el override por usuario `%LocalAppData%\TP_ControlVehicular\Empresa\logo.png`; si no existe, usa el asset embebido `Presentacion/Assets/logo.png` (Content, copiado al output). `ReporteExtensions.RutaLogo` delega en él; no leer rutas de logo por fuera ni duplicar la imagen.
  - `EmpresaBranding.AsegurarCarpeta()` crea la carpeta override de forma perezosa en el primer uso (por usuario; no la crea el instalador). Un futuro feature "carga de logo/dirección/teléfono del taller" poblará esa carpeta (`empresa.json`).
- **Visibilidad de reportes por rol (RBAC)**: cada reporte tiene su propio permiso `Reporte.<Nombre>.Ver`. El permiso genérico `Reporte.Ver` está obsoleto y no debe usarse.
  | Reporte | Permiso | Admin | Recepcionista | Mecánico |
  |---|---|:-:|:-:|:-:|
  | Reporte Órdenes | `Reporte.Ordenes.Ver` | ✅ | ✅ | ❌ |
  | Reporte Operativo | `Reporte.Operativo.Ver` | ✅ | ✅ | ❌ |
  | Reporte Gerencial | `Reporte.Gerencial.Ver` | ✅ | ❌ | ❌ |
  | Historial clínico / Tareas del día (en Mis Trabajos) | `Reporte.Mecanico.Ver` | ✅ | ❌ | ✅ |
  - Un reporte nuevo requiere: permiso nuevo en un script SQL idempotente en `bd/`, chequeo en `MainWindow.AplicarRestriccionesPorRol` (o en el VM si el botón vive dentro de una pantalla) y actualizar esta tabla.

## No hagas
- Todos los scripts temporales (.py, .ps1, etc.) utilizados durante el desarrollo DEBEN colocarse o moverse a la carpeta docs/script para mantener la raíz del proyecto limpia.
- No editar archivos de código o XAML usando Set-Content o > / >> en PowerShell bajo ninguna circunstancia. SIEMPRE usar estrictamente la API de .NET: [System.IO.File]::WriteAllText("ruta", $contenido, [System.Text.Encoding]::UTF8) para evitar corromper emojis y caracteres acentuados. El proyecto debe mantenerse rigurosamente en UTF-8.
- No usar && en PowerShell; usa ; para encadenar comandos.
- No rutas sin comillas: la ruta del proyecto tiene espacios y caracteres no ASCII — siempre entre comillas.
- No modificar AGENTS_Template.md ni la carpeta spec_template/ (son plantillas de referencia).
- No asumir patrones genéricos: confía en fuentes ejecutables (config, scripts, código real) sobre prosa.
- No agregar dependencias sin avisar.
- Commits atómicos en convención Conventional Commits; monorepo único.
- No subir secretos: credenciales solo por variables de entorno / perfiles.
- El estado real del proyecto se verifica contra git log / git status, nunca contra MEMORY.md. La memoria es una pista; el repositorio es la evidencia. Si discrepan, el repositorio gana y la memoria se corrige.

## Flujo de trabajo
- **Ramas por Spec**: Para cada spec (feature) se debe crear una rama en Git (ej. git checkout -b feature/nombre). Se trabaja ahí hasta terminar y luego se fusiona.
- **Uso de MEMORY.md**:
  - Al INICIAR una tarea: leer `MEMORY.md` como pista y contrastarlo con `git log -n 10` y `git status`. Si discrepan, el repositorio gana y se corrige `MEMORY.md`.
  - Al CERRAR una spec (antes del merge): actualizar `MEMORY.md` con las nuevas convenciones, reglas, scripts SQL, permisos y decisiones, y commitearlo junto con la spec (`docs(memory): ...`).
  - `MEMORY.md` debe mantenerse alineado con `AGENTS.md`: toda regla nueva en uno se replica en el otro.
- **Planes de implementación (.md)**: todo plan, walkthrough o documento de trabajo en Markdown generado durante el desarrollo DEBE guardarse en `docs/plan/` dentro del repo, no solo en carpetas externas del agente. Nombre: `AAAA-MM-DD_<nombre-spec>.md`; walkthrough: `..._walkthrough.md`. Se commitea junto con la spec (`docs(plan): ...`). Diferencia con OpenSpec: `openspec/` = especificación formal (inglés); `docs/plan/` = plan/bitácora de ejecución (español).
- Antes de una tarea no trivial, propón un plan y espera confirmación.
- Una tarea a la vez; al terminar, informa qué cambiaste para revisión.
- Si no estás seguro al 80%, pregunta. No inventes.
- Haz solo lo que se pide: no añadas funcionalidades por tu cuenta.
- Cambios pequeños y enfocados; no reescribas lo que ya funciona.
- Al terminar, resume qué has cambiado y cualquier decisión que deba revisar.

## Documentación
- `docs/DER.md` — diagrama de entidad relación a tener en cuenta en todo feature o spec en adelante.
- `AGENTS_Template.md` — plantilla de estructura (no modificar).
- `docs/plan/` — planes y walkthroughs de ejecución (español), ver *Flujo de trabajo*.
- `README.md` — solo placeholders, sin contenido real.
- `spec_template/` — plantillas de especificación vacías.
- Código fuente en capas: `Datos/`, `Entidad/`, `Migracion/`, `Negocio/`, `Presentacion/`.

## Agent Guidelines
- Use **CodeGraph** whenever possible for code search and exploration. Ensure the codegraph MCP server is enabled.
- Only use python scripts (docs/script/) to manipulate files if PowerShell file manipulation is needed, to avoid UTF-8 encoding corruption.
- You MUST ALWAYS use OpenSpec (`openspec/` directory) to define feature proposals (`proposal.md`), specifications (`spec.md`), technical designs (`design.md`), and implementation tasks (`tasks.md`) BEFORE writing or modifying implementation code.
- OpenSpec specification files MUST be written in English.
- Ensure all acceptance criteria and BDD scenarios defined via OpenSpec are validated with build verification before declaring completion.
