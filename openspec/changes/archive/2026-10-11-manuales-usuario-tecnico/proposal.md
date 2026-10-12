# Proposal: Manuales de Usuario y Técnico de TP_ControlVehicular

## Why
El sistema `TP_ControlVehicular` ya alcanzó un alcance funcional completo (login, dashboard por perfil, clientes, vehículos, órdenes de servicio, mis trabajos, reportes, taller, marcas/modelos/servicios, usuarios/roles y el módulo de respaldos con resguardo en Google Drive), pero **no existe documentación de uso ni de administración**. El personal del taller (Administrador, Recepcionista, Mecánico) no tiene una guía operativa paso a paso, y el responsable de TI/soporte no cuenta con un manual de despliegue, conectividad SQL Server, respaldos y recuperación ante desastres. Esto genera dependencia total del desarrollador, errores operativos y riesgo en la puesta en marcha.

Esta entrega produce la **documentación oficial** del sistema en `docs/manuales/`: un índice maestro, un **Manual de Usuario Operativo por roles** y un **Manual Técnico de Administración y TI**, siguiendo el estándar editorial y visual del *Arquitecto de Documentación*.

## What Changes
- Nuevo portal `docs/manuales/README.md`: bienvenida, arquitectura general (N-capas + MVVM), matriz de perfiles vs módulos y mapa de navegación a los manuales.
- Nuevo `docs/manuales/manual_usuario_roles.md`: manual operativo con portada, TOC, Capítulo 0 (primeros pasos/navegación) y capítulos por perfil (Administrador 🛡️, Recepcionista 📋, Mecánico 🔧) más guía de dudas y errores frecuentes.
- Nuevo `docs/manuales/manual_tecnico_administracion.md`: manual de infraestructura y administración (requisitos, conexión SQL Server, scripts `bd/`, backups + Google Drive, DR, permisos NTFS, RBAC y troubleshooting).
- Carpetas de assets `docs/manuales/assets/roles/` y `docs/manuales/assets/tecnico/`.
- **Marcadores de captura obligatorios** (`> 📸 CAPTURA REQUERIDA:`) en cada pantalla/formulario/diálogo/paso, seguidos del enlace Markdown `![...](assets/...)`. Las capturas `.png` las tomará el usuario posteriormente.

## Impact
- **Capacidad nueva**: `manuales-documentacion` (documentación).
- **Archivos nuevos**: `docs/manuales/README.md`, `docs/manuales/manual_usuario_roles.md`, `docs/manuales/manual_tecnico_administracion.md`, `docs/manuales/assets/{roles,tecnico}/.gitkeep`.
- **Sin cambios de código** de la aplicación, sin cambios de base de datos y sin dependencias nuevas. El build `dotnet build` debe permanecer sin errores (verificación de no-regresión).
- **Fuentes de verdad** consultadas: código en `Presentacion/Pantalla/**`, `Presentacion/MainWindow.xaml(.cs)` (RBAC real), y `docs/referencia/{dashboard,mis-trabajos,backup,reportes,bd-y-dominio,UX_Flujo_OrdenServicio,README_FLUJO_ARQUITECTURA}.md`.

## Non-goals (fuera de alcance de esta entrega)
- **Tomar** las capturas de pantalla (a cargo del usuario); se entregan marcadores y rutas.
- Documentar pantallas **huérfanas** no alcanzables en la UI (`CtlReporteServicios`, `CtlReportesRecepcion`), salvo mención en la auditoría.
- Traducción a otros idiomas.
- Sitio web/portal publicado (se entrega Markdown en el repositorio).
- Generación de PDF de los manuales.
