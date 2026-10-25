# Proposal: Custom Deletion & Action Confirmation Dialog (FrmConfirmacion)

## Why
Native Windows `MessageBox.Show` dialogs are non-customizable in WPF and break visual consistency with the application design system. When users click the "Eliminar" or "Cambiar Estado" button in list views (`Ctl*.xaml`), standard OS dialogs display default Windows buttons.

Creating a reusable custom confirmation dialog (`FrmConfirmacion`) styled consistently with `Frm*.xaml` ensures a cohesive user experience:
- `#2C3E50` dark header.
- White card container with `CornerRadius="12"`.
- `#27AE60` Green Accept button (`✓ Sí, Aceptar`).
- `#C0392B` Red Cancel button (`❌ No, Cancelar`).

## What
1. Create `Presentacion/Pantalla/Compartido/FrmConfirmacion.xaml` and `FrmConfirmacion.xaml.cs`.
2. Provide a clean static helper method `FrmConfirmacion.Mostrar(mensaje, titulo, owner)` for seamless invocation.
3. Replace all native `MessageBox.Show` confirmation calls in `CtlCliente`, `CtlVehiculo`, `CtlModelo`, `CtlMarca`, `CtlServicio`, `CtlTaller`, `CtlUsuario`, and `CtlRol`.
4. Document the confirmation dialog convention in `AGENTS.md` for future ABMs.

## Impact
- Files created: `Presentacion/Pantalla/Compartido/FrmConfirmacion.xaml`, `Presentacion/Pantalla/Compartido/FrmConfirmacion.xaml.cs`.
- Files updated: All 8 `Ctl*.xaml.cs` list view controllers and `AGENTS.md`.
- Zero impact on backend logic or repositories.
