# Plan de ejecución — Reporte Gerencial: consulta explícita, gráfico de torta y limpieza

- **Spec:** `openspec/changes/reporte-gerencial-mejoras/` (proposal / design / tasks / delta `reportes`)
- **Rama:** `feature/reporte-gerencial-mejoras` (base `develop`)
- **Fecha:** 2026-10-10

## Alcance
A1. Corregir `ReporteModeloReparadoDto.MarcaModelo` (interpolación rota → literal `{Marca} {Modelo}`).
A2. Botón **Consultar** explícito + validación de rango (Desde ≤ Hasta); quitar `_ = LoadAsync()` de los setters de fecha.
A3. Nueva métrica **cantidad de cada servicio en el rango** (SP `sp_ReporteServiciosPorRango` en `bd/18` + DTO `ReporteServicioCantidadDto` + método de repositorio + 4.º tab con DataGrid).
A4. **Gráfico de torta SOLO en el PDF** gerencial (helper `ServiciosPieChartGenerator` con Canvas/SkiaSharp + leyenda; logo obligatorio en el encabezado).
A5. Eliminar la vista muerta `ReporteGerencialView.xaml(.cs)` y su registro DI en `App.xaml.cs`.

## Fuera de alcance
- Graficar dentro de la vista WPF.
- Permisos nuevos (sigue `Reporte.Gerencial.Ver`, solo Administrador).
- Cambiar la métrica de ingresos ni exportar a Excel.

## Decisiones clave
- La torta se dibuja con `IContainer.Canvas(...)` + SkiaSharp (`SKPath.ArcTo`), evitando dependencia de SVG. Fallback documentado: primitivas de QuestPDF si `Canvas` no estuviera disponible.
- Frontera de fechas: `>= @FechaDesde AND < DATEADD(DAY,1,@FechaHasta)` (incluye el día final completo).
- Agregación a nivel `DetalleServicio`, excluyendo órdenes `Cancelada`.
- Sin datos ⇒ tab vacío y PDF sin gráfico (prohibido el fallback a datos demostrativos).

## Bitácora
- [x] Artefactos OpenSpec creados; `openspec validate reporte-gerencial-mejoras` → *valid*, 4/4.
- [ ] A1..A5.
- [ ] Build + tests + `openspec validate`.
- [ ] Merge a `develop`, actualizar `MEMORY.md`, archivar el change.
