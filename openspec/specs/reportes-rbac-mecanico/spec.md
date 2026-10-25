# Spec: Mechanic report visibility, logo and folder-based branding

## Permission
| Report | Permission | Admin | Receptionist | Mechanic |
|---|---|:-:|:-:|:-:|
| Vehicle clinical history / Daily tasks | `Reporte.Mecanico.Ver` | Yes | No | Yes |

## Logo resolution
| Priority | Source | Path |
|---|---|---|
| 1 (override) | Per-user override folder | `%LocalAppData%\TP_ControlVehicular\Empresa\logo.png` |
| 2 (fallback) | Bundled asset | `<BaseDirectory>\Presentacion\Assets\logo.png` |

The override wins only when the file exists. Resolution happens at PDF-generation time.

## Acceptance criteria

```gherkin
Scenario: Mechanic sees the mechanic report buttons
  Given a user with role "Mecanico" is logged in
  And the user has the permission "Reporte.Mecanico.Ver"
  And the user opens the "Mis Trabajos" screen
  Then the "Imprimir Historial Clínico" button is available for the selected task
  And the "Tareas del Día" button is visible

Scenario: User without the mechanic permission cannot see the buttons
  Given a user is logged in without the permission "Reporte.Mecanico.Ver"
  When the user opens the "Mis Trabajos" screen
  Then the "Imprimir Historial Clínico" and "Tareas del Día" buttons are collapsed

Scenario: Admin can generate the mechanic reports
  Given a user with role "Administrador" is logged in
  When the user opens the "Mis Trabajos" screen
  Then both mechanic report buttons are visible

Scenario: Mechanic PDFs use the shared logo header
  Given a user generates the Vehicle clinical history PDF
  Then its page header is composed with ComposeEncabezadoTaller("Historial Clínico")
  And its page footer is composed with ComposePieDePagina()
  Given a user generates the Daily tasks PDF
  Then its page header is composed with ComposeEncabezadoTaller("Tareas del Día")
  And its page footer is composed with ComposePieDePagina()
  And the dynamic subtitle is placed at the top of the page content

Scenario: Logo resolves from the override folder when present
  Given a file exists at "%LocalAppData%\TP_ControlVehicular\Empresa\logo.png"
  When any PDF header is composed
  Then the override logo is used

Scenario: Logo falls back to the bundled asset when the override is missing
  Given no file exists at "%LocalAppData%\TP_ControlVehicular\Empresa\logo.png"
  When any PDF header is composed
  Then the bundled "Presentacion/Assets/logo.png" is used

Scenario: No PDF uses an ad-hoc text-only header
  Given the source tree
  Then no report PDF calls page.Header().Text(...) for its header
```
