# Plan — Compactación de `MEMORY.md`

Bitácora de la fase 0.1 del plan aprobado (mejoras Reporte Gerencial + Dashboard por perfil). No requiere OpenSpec: es documentación.

## Objetivo
Reducir `MEMORY.md` desde 131 líneas a un delta de alta señal, eliminando la duplicación verbatim
de `AGENTS.md` (fuente canónica) y conservando: disclaimer "la memoria es una pista", reglas
críticas de reportes (logo, RBAC, granularidades, no-demo-data, variantes `sp_ReporteIngresos`,
cancelación, E2E), No hagás, flujo de trabajo y OpenSpec/TDD en forma condensada.

## Diagnóstico (evidencia)
- `git status`: limpio, rama `develop`. `git log -n 12`: último commit `docs(memory): ...` en `bada484`
  (fuente de datos por tarea) — `MEMORY.md` previo estaba alineado con la historia.
- `AGENTS.md` (canon) ya contiene Stack, Comandos, Estructura, convenciones completas de
  UI/formularios/popups, RBAC, logos PDF, granularidad, SPs y No hagás → `MEMORY.md` los duplicaba
  sin valor agregado.

## Decisiones
- `AGENTS.md` queda como fuente canónica; `MEMORY.md` conserva apuntador + delta de alta señal.
- Se mantienen íntegras (condensadas) las reglas de reportes porque son el conocimiento más
  reciente y el que guía las próximas specs.
- Compromiso de alineación vigente: toda regla nueva se replica en ambos archivos.

## Cambios
| Archivo | Cambio |
|---|---|
| `MEMORY.md` | Compactado: 131 → ~75 líneas. Sección "Convenciones críticas (resumen)", "No hagas (resumen)", "Flujo de trabajo", "OpenSpec (SDD) + TDD", "Documentación", "Agent Guidelines". |
| `docs/plan/2026-10-10_compactacion-memory.md` | Esta bitácora. |

## Verificación
- `git diff --stat MEMORY.md` → reducción sustancial de líneas.
- `dotnet build "TP_ControlVehicular.slnx"` no aplica (sin cambios de código); no requerido en esta fase.
- Reglas críticas presentes en la versión compactada (lupa manual contra el diff).