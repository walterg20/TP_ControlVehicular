# Technical Design: Manuales de Usuario y Técnico

## Design Details

### 1. Estructura de artefactos

```text
docs/manuales/
├── README.md                              # índice maestro (Arquitecto)
├── manual_usuario_roles.md                # redactor_roles
├── manual_tecnico_administracion.md       # redactor_tecnico
└── assets/
    ├── roles/.gitkeep                     # capturas operativas
    └── tecnico/.gitkeep                   # capturas técnicas
```

### 2. Estándar de marcador de captura (obligatorio)

```markdown
> 📸 **CAPTURA REQUERIDA:** `docs/manuales/assets/<subcarpeta>/<archivo>.png`
> - **Pantalla / Modal:** <Nombre visible + control XAML, ej: CtlOrdenServicioForm>
> - **Estado recomendado:** <datos de ejemplo cargados>
> - **Elementos a resaltar:** <recuadros/botones>
> - **Epígrafe:** *Figura X.X: <descripción>*

![Figura X.X: Descripción](assets/<subcarpeta>/<archivo>.png)
```

Reglas:
- El enlace `![...](assets/...)` va **inmediatamente debajo** del bloque del marcador, con ruta relativa desde el propio `.md` (por eso `assets/...` y no `docs/manuales/assets/...`).
- Cada pantalla, formulario, diálogo o paso crítico lleva su marcador. No hay tabla-inventario de capturas separada.

### 3. Fundamentos de contenido (fuente de verdad = código real)

- **RBAC real** (`Presentacion/MainWindow.xaml.cs`, `AplicarRestriccionesPorRol`): claves de permiso `Cliente.Ver`, `Vehiculo.Ver`, `Servicio.Ver`, `OrdenServicio.Ver`, `MisTrabajos.Ver`, `Usuario.Ver`, `Taller.Ver`, `Configuracion.Empresa.Gestionar`, `Configuracion.Backup.Gestionar`, `Reporte.Ordenes.Ver`, `Reporte.Operativo.Ver`, `Reporte.Gerencial.Ver`. El Dashboard es siempre visible.
- **Ambigüedades resueltas**:
  - `CtlTaller` = **"🔧 Control de Taller"** → ABM de talleres (no datos de la empresa).
  - `CtlConfiguracion` = **"⚙️ Configuración de Empresa"** → razón social, CUIT, dirección, teléfono y logo.
  - `CtlReporteServicios` y `CtlReportesRecepcion` = pantallas **huérfanas** (no instanciadas en la UI) → no se documentan.
- **Estados de la orden**: usar la constante `RegistroServicio.EstadoCancelada` (`"Cancelada"`); nunca el literal `"Cancelado"`.
- **Convención de botones/colores**: `➕ Nuevo` #27AE60, `✏️ Editar` #F39C12, `🗑️ Eliminar`/`❌ Cancelar` #C0392B, `🔄 Actualizar/Buscar` #2980B9; diálogos `FrmConfirmacion.Mostrar` y `FrmConfirmacion.MostrarAviso`.
- **Respaldos**: `bd/21_ConfiguracionBackup.sql`, `bd/22_PermisosBackup.sql`; `RutaBackupServer` vs `RutaBackupUNC`; sin `WITH COMPRESSION` (Express); restore de prueba a `ControlVehicular_Test`; Google Drive scope `drive.file` con `credenciales.json` junto al `.exe` y token por-PC cifrado con DPAPI.

### 4. Estilo editorial

- Español; tono claro (usuario) y técnico (TI).
- GFM con alertas `> [!NOTE] / [!TIP] / [!IMPORTANT] / [!WARNING] / [!CAUTION]`.
- Diagramas **Mermaid**: flujo de recepción (`+ Rápido`), ciclo de estados de la orden, circuito de respaldo/Drive.
- Badges de rol: 🛡️ Administrador, 📋 Recepcionista, 🔧 Mecánico, 💻 TI.

### 5. Orquestación (arquitecto_manual)

1. Arquitecto crea rama, change OpenSpec, scaffolding y `README.md`.
2. Se lanzan en paralelo dos redactores (`redactor_roles`, `redactor_tecnico`) con las correcciones de dominio del punto 3.
3. Auditoría de cobertura de los 14 módulos + revisión de marcadores/rutas.
4. Verificación `dotnet build "TP_ControlVehicular.slnx"` sin errores (no-regresión).
5. Actualización de `MEMORY.md` y commit `docs(manuales): ...`.

### 6. Ramas y cierre

- Rama `feature/manuales-usuario-tecnico` creada desde `feature/backup-drive` (ya que el módulo de respaldos documentado aún no está fusionado en `develop`).
- Cierre: merge a `develop` **sujeto a confirmación** (arrastraría los commits de `feature/backup-drive`); archivar el change en `openspec/changes/archive/`.
