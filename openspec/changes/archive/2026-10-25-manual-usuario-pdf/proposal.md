# Proposal: Manual de Usuario en PDF

## Why
El sistema TP_ControlVehicular no cuenta con un manual de usuario distribuible. Los usuarios finales (Administrador, Mecánico, Recepcionista) necesitan una guía de uso por rol, y el equipo necesita un documento empaquetable con el instalador.

## What
1. Generar un manual de usuario en PDF con QuestPDF, reutilizando la infraestructura de reportes existente (encabezado/pie de página con logo de empresa).
2. Exponerlo desde la UI (menú "Ayuda → Manual de Usuario").
3. Versionarlo por build y empaquetarlo con el instalador.

## Impact
- Nuevos archivos: `ManualUsuarioDocument.cs`, `GenerarManualUsuarioHandler.cs`.
- Modificaciones: `MainWindow.xaml` / `.xaml.cs`.
- Spec en `openspec/changes/manual-usuario-pdf/`.
