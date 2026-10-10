# Proposal: Standardize UI Design & Inline Add Buttons Across All Frm Modal Dialogs

## Why
`FrmServicio.xaml` established a modern design pattern for modal forms with a dark blue header (`#2C3E50`), green accept button (`#27AE60`), and red cancel button (`#C0392B`). Other modal forms (`FrmCliente`, `FrmVehiculo`, `FrmModelo`, `FrmMarca`, `FrmTaller`, `FrmUsuario`, `FrmRol`) were using default WPF buttons and inline add buttons with inconsistent styling.

Standardizing all modal dialogs ensures complete visual alignment with `FrmServicio` and establishes clean icon-only green buttons (`➕`) for inline related entity creation.

## What
1. Update `FrmCliente`, `FrmVehiculo`, `FrmModelo`, `FrmMarca`, `FrmTaller`, `FrmUsuario`, `FrmRol`:
   - Header title: `#2C3E50`, `FontSize="20"`, `FontWeight="Bold"`.
   - Guardar button: `#27AE60` Green.
   - Cancelar button: `#C0392B` Red.
2. Update inline related entity add buttons (`btnAgregarMarca`, `btnAgregarModelo`, `btnAgregarRol`) to use `#27AE60` Green, `CornerRadius="6"`, and icon-only `➕` content without text.

## Impact
- Files affected: `FrmCliente.xaml`, `FrmVehiculo.xaml`, `FrmModelo.xaml`, `FrmMarca.xaml`, `FrmTaller.xaml`, `FrmUsuario.xaml`, `FrmRol.xaml`.
- No C# code-behind logic or event handler names changed.
