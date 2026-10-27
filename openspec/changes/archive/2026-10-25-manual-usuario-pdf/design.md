# Design: Manual de Usuario en PDF

## Architectural Context
WPF .NET 10, MVVM, QuestPDF reports with `ReporteExtensions.ComposeEncabezadoTaller` and `ComposePieDePagina`, `EmpresaBranding` logo resolver.

## Technical Strategy
1. Create `ManualUsuarioDocument` implementing `IDocument` with sections for each module.
2. Create `GenerarManualUsuarioHandler` (inject `IMapper` optional, `IVersionProvider`) that writes to `%APPDATA%\TP_ControlVehicular\Manuales\`.
3. Add menu item in `MainWindow.xaml` and command in code-behind.
4. Reuse existing report extensions for header/footer.
5. Version from `Assembly.GetExecutingAssembly().GetName().Version`.

## Files
- `Negocio/Reportes/Documentos/ManualUsuarioDocument.cs` (new)
- `Negocio/Handlers/GenerarManualUsuarioHandler.cs` (new)
- `Presentacion/MainWindow.xaml` (modify)
- `Presentacion/MainWindow.xaml.cs` (modify)
- `openspec/changes/manual-usuario-pdf/` (spec)
