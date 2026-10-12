# Plan de ejecución — Manuales de Usuario y Técnico

**Spec:** `openspec/changes/2026-10-11-manuales-usuario-tecnico/`
**Rama:** `feature/manuales-usuario-tecnico` (base: `feature/backup-drive`, 551862d)
**Fecha:** 2026-10-11

## 1. Objetivo
Generar la documentación oficial del sistema bajo `docs/manuales/`: portal `README.md`, **Manual de Usuario Operativo** por roles y **Manual Técnico de Administración y TI**, con marcadores de captura estándar en cada pantalla/paso.

## 2. Base de la rama
Se parte de `feature/backup-drive` y no de `develop`, porque el módulo de **Respaldos con resguardo en Google Drive** (`CtlBackup`, `bd/21`, `bd/22`) que debe documentar el manual técnico aún **no está fusionado** en `develop`. Documentar desde `develop` produciría omisiones.

## 3. Decisiones de contenido
- **`CtlTaller`** = “🔧 Control de Taller” (ABM de talleres). **No** es el alta de datos de la empresa.
- **`CtlConfiguracion`** = “⚙️ Configuración de Empresa” (razón social, CUIT, dirección, teléfono, logo). Corrige el outline original del redactor que atribuía estos datos a `CtlTaller`.
- **Pantallas huérfanas** `CtlReporteServicios` y `CtlReportesRecepcion`: no instanciadas en la UI → **no** se documentan (solo mención en auditoría).
- **RBAC** tomado de `MainWindow.xaml.cs → AplicarRestriccionesPorRol`.
- **Estados de orden**: usar `RegistroServicio.EstadoCancelada` (`"Cancelada"`), nunca `"Cancelado"`.
- **Capturas**: solo marcadores + enlaces (`assets/roles/...`, `assets/tecnico/...`); sin tabla-inventario.

## 4. Orquestación
1. Arquitecto: rama + change + scaffolding + `README.md`.
2. Redactores en paralelo (`redactor_roles`, `redactor_tecnico`) con el handoff de correcciones de dominio.
3. Auditoría de cobertura (14 módulos) y de marcadores.
4. Verificación `dotnet build "TP_ControlVehicular.slnx"` (no-regresión; no se toca código).
5. `MEMORY.md` + commit `docs(manuales): ...`.
6. Cierre (merge + archivar change) **sujeto a confirmación** por arrastrar los commits de `feature/backup-drive`.

## 5. Criterios de aceptación
Ver `openspec/changes/2026-10-11-manuales-usuario-tecnico/specs/manuales-documentacion/spec.md`.

## 6. Bitácora
- [ ] Rama creada.
- [ ] Change OpenSpec creado.
- [ ] Scaffolding `docs/manuales/`.
- [ ] `README.md` redactado.
- [ ] Manual de usuario redactado.
- [ ] Manual técnico redactado.
- [ ] Auditoría + build.
- [ ] MEMORY + commit.
