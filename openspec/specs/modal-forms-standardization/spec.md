# Capability: Modal Dialog Forms Standardization

## Overview
All modal dialog forms (`Frm*.xaml`) in `TP_ControlVehicular.Presentacion` must adhere to a unified UI design system matching `FrmServicio.xaml`:
1. Header titles styled with `#2C3E50` bold typography.
2. Accept / Save button styled in Green (`Background="#27AE60"`).
3. Cancel button styled in Red (`Background="#C0392B"`).
4. Inline entity addition buttons (e.g. `btnAgregarMarca`, `btnAgregarModelo`, `btnAgregarRol`) styled in Green (`Background="#27AE60"`, icon-only `➕` or `+`, no text).

## Requirements

### Requirement: Standardized Form Action Buttons
- **Save / Accept ("Guardar")**: `Background="#27AE60"` (Green), `Foreground="White"`, `FontWeight="Bold"`, `CornerRadius="6"`, `Width="95"`, `Height="34"`.
- **Cancel ("Cancelar")**: `Background="#C0392B"` (Red), `Foreground="White"`, `FontWeight="Bold"`, `CornerRadius="6"`, `Width="95"`, `Height="34"`.

### Requirement: Inline Related Entity Addition Buttons
- Inline add buttons adjacent to ComboBoxes MUST use `Background="#27AE60"` (Green), `Foreground="White"`, `FontWeight="Bold"`, `CornerRadius="6"`, `BorderThickness="0"`, `Cursor="Hand"`, `Width="30"`, `Height="30"`.
- The button content MUST consist exclusively of the icon (`➕` or `+`) without descriptive text.
