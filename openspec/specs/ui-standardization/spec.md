# Capability: UI Standardization for UserControls

## Overview
All list UserControls (`Ctl*.xaml`) in `TP_ControlVehicular.Presentacion` must adhere to a unified, modern WPF design system using 4-row card layouts, consistent typography, shadow effects, and standardized action button color coding.

## Requirements

### Requirement: Unified 4-Row Grid Structure
Each `Ctl*.xaml` list UserControl MUST structure its main layout into 4 rows:
1. **Row 0 (Header)**: Title icon + text (`#2C3E50`, bold `FontSize="22"` or `20`) with optional subtitle or refresh button.
2. **Row 1 (Search & New Bar)**: White card container (`CornerRadius="12"`, `DropShadowEffect`) containing the search TextBox on the left and the **"➕ Nuevo [Entidad]"** primary action button (`Background="#27AE60"`) on the right.
3. **Row 2 (DataGrid Card)**: White card container (`CornerRadius="12"`, `DropShadowEffect`) wrapping the DataGrid with dark header styling (`Background="#2C3E50"`, `Foreground="White"`).
4. **Row 3 (Footer Action Bar)**: Bottom bar containing **"✏️ Editar"** (`Background="#F39C12"`) and **"🗑️ Eliminar" / "Cambiar Estado"** (`Background="#C0392B"`) buttons aligned to the right.

### Requirement: Standardized Button Color Scheme
- **Primary / Create ("Nuevo")**: `#27AE60` (Green), `Foreground="White"`, `FontWeight="Bold"`, `CornerRadius="6"`.
- **Secondary / Edit ("Editar")**: `#F39C12` (Orange), `Foreground="White"`, `FontWeight="Bold"`, `CornerRadius="6"`.
- **Destructive / Delete ("Eliminar" / "Estado")**: `#C0392B` (Red), `Foreground="White"`, `FontWeight="Bold"`, `CornerRadius="6"`.
- **Utility / Refresh / Search**: `#2980B9` (Blue), `Foreground="White"`, `FontWeight="Bold"`, `CornerRadius="6"`.

### Requirement: Card & Shadow Styling Rules
- Main containers MUST use `Background="White"`, `CornerRadius="12"`, and `DropShadowEffect` with `BlurRadius="10"` or `12`, `ShadowDepth="2"`, `Opacity="0.08"`.
