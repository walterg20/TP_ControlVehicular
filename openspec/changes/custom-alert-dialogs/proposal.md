# Proposal: Custom Alert Dialogs for Selection Warnings and Notifications

## Why
When users attempt to edit or delete records in list views (`Ctl*.xaml`) without making a DataGrid selection, native Windows `MessageBox.Show` popups were displayed. These native popups clash with the application's modern UI theme.

Extending `FrmConfirmacion` with a single-button alert mode (`MostrarAviso`) ensures that selection warnings, validation alerts, and status notifications share the exact same visual design language:
- Dark header `#2C3E50` with warning/info title.
- White card container with `CornerRadius="12"`.
- `#27AE60` Green Accept button (`✓ Aceptar`).

## What
1. Update `FrmConfirmacion.xaml` & `FrmConfirmacion.xaml.cs`:
   - Name `lblIcono` to support dynamic icons (`⚠️`, `ℹ️`, `✅`).
   - Add static method `FrmConfirmacion.MostrarAviso(mensaje, titulo, owner, textoBoton, icono)` hiding the cancel button.
2. Replace all selection warning `MessageBox.Show` calls in `CtlCliente`, `CtlVehiculo`, `CtlModelo`, `CtlMarca`, `CtlServicio`, `CtlTaller`, `CtlUsuario`, and `CtlRol`.
3. Update `AGENTS.md` to document `FrmConfirmacion.MostrarAviso(...)` for all selection warnings and alerts.

## Impact
- Files modified: `FrmConfirmacion.xaml`, `FrmConfirmacion.xaml.cs`, `AGENTS.md`, and all 8 `Ctl*.xaml.cs` list view controllers.
- No impact on business logic or repositories.
