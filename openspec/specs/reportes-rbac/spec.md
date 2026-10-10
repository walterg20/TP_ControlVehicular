# Spec: Role-based report visibility and company logo

## Permission matrix
| Report | Permission | Admin | Receptionist | Mechanic |
|---|---|:-:|:-:|:-:|
| Work orders report | `Reporte.Ordenes.Ver` | Yes | Yes | No |
| Operational report | `Reporte.Operativo.Ver` | Yes | Yes | No |
| Management report | `Reporte.Gerencial.Ver` | Yes | No | No |
| Vehicle history / Daily tasks | `Reporte.Mecanico.Ver` | Yes | No | Yes |

## Acceptance criteria

```gherkin
Scenario: Admin sees every report
  Given a user with role "Administrador" is logged in
  When the sidebar menu is rendered
  Then "Reporte Órdenes", "Reporte Operativo" and "Reporte Gerencial" are visible
  And the "REPORTES" section header is visible

Scenario: Receptionist does not see the management report
  Given a user with role "Recepcionista" is logged in
  When the sidebar menu is rendered
  Then "Reporte Órdenes" and "Reporte Operativo" are visible
  And "Reporte Gerencial" is collapsed

Scenario: Mechanic does not see the reports section
  Given a user with role "Mecanico" is logged in
  When the sidebar menu is rendered
  Then the "REPORTES" section header, its separator and all report buttons are collapsed

Scenario: Every generated PDF shows the company logo
  Given any report or receipt PDF is generated
  Then its page header contains the company logo from "Presentacion/Assets/logo.png"
  And report PDFs use ComposeEncabezadoTaller for the header

Scenario: Permission script is idempotent
  Given the script "bd/12_PermisosReportesPorRol.sql" was already executed
  When it is executed again
  Then no duplicated rows exist in Permisos or RolPermisos
```
