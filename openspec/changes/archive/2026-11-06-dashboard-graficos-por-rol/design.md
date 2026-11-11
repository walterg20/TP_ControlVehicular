# Design

## Context

Ver `proposal.md` para el problema.

El dashboard tiene tres filas: encabezado, tarjetas de resumen (3 o 4 según el rol) y una tarjeta inferior de altura `*` con la grilla de vehículos. La fila inferior es la que se reemplaza.

Los tres gráficos del Reporte Gerencial existen hoy como **cadenas SVG** generadas por `IngresosBarChartGenerator`, `ModelosBarChartGenerator` y `ServiciosPieChartGenerator`, que se incrustan en los PDF con QuestPDF mediante `.Svg(...)`. WPF no tiene soporte nativo de SVG y el proyecto no incluye ningún rasterizador (ni SkiaSharp, ni Svg.Skia, ni WebView2); además `AGENTS.md` prohíbe agregar dependencias sin avisar.

## Goals / Non-Goals

**Goals:**
- Que el bloque inferior del dashboard muestre visualizaciones acordes al rol.
- Reutilizar los datos que el dashboard ya consulta, sin SQL nuevo.
- Que los gráficos se puedan probar sin depender de WPF ni de hilos STA.
- Mantener la coherencia visual con los PDF: mismos colores y mismo formato abreviado de montos.

**Non-Goals:**
- No agregar paquetes NuGet.
- No tocar las pantallas de reportes.
- No agregar controles de configuración (selector de rango, etc.) al dashboard.

## Decisions

### 1. Gráficos WPF nativos, sin dependencias nuevas
- **Decision:** se implementan controles WPF que dibujan con `Rectangle`, `Path` y `TextBlock` sobre un `Canvas`.
- **Rationale:** es la única vía que no requiere dependencias. Además da gráficos nítidos a cualquier resolución, con tooltip y con los colores del tema (`#2C3E50` títulos, `#2980B9` barras, `#F8F9F9` fondo de tarjeta).
- **Alternative considered:** rasterizar el SVG existente con `Svg.Skia` + `SkiaSharp`. Descartado: suma dos paquetes NuGet y el SVG está dimensionado a 500x190 px fijos, sin adapting al ancho disponible.
- **Alternative considered:** mostrar el PDF del reporte embebido con WebView2. Descartado: dependencia pesada y no aplica a "imágenes".

### 2. La lógica de cálculo vive en una clase pura
- **Decision:** `Presentacion/Graficos/GraficoCalculos.cs` concentra la escala del eje Y, los cortes de la escala, el formato abreviado de montos, la geometría de los sectores de la torta y el agrupamiento "top N + Otros". Los `UserControl` son una capa visual fina que la consume.
- **Rationale:** los controles WPF requieren un hilo STA para instanciarse; aislar la lógica permite probarla sin WPF (los tests de UI del proyecto ya usan el patrón `Thread` + `SetApartmentState(STA)`, pero sólo para un test puntual).

### 3. La grilla se reemplaza en los tres roles
- **Decision:** el bloque inferior deja de mostrar la grilla de vehículos para Administrador, Recepcionista y Mecánico. Se conserva la tarjeta "Vehículos Activos" de la fila de resumen.
- **Rationale:** es lo que pide el pedido ("en ese espacio"), y evita mantener dos representations del mismo dato en la misma pantalla.
- **Consecuencia:** `DashboardMetricsDto.VehiculosEnTaller` y `DashboardViewModel.VehiculosEnTaller` se eliminan. El conteo `VehiculosActivosCount` se mantiene porque alimenta la tarjeta de resumen.

### 4. Rango de fechas fijo por gráfico, sin selector
- **Decision:**
  | Gráfico | Rol | Rango |
  |---|---|---|
  | Ingresos por mes | Administrador | últimos 6 meses (mes en curso incluido) |
  | Modelos más reparados | Administrador | últimos 6 meses (mes en curso incluido) |
  | Servicios por rango | Administrador | últimos 6 meses (mes en curso incluido) |
  | Órdenes por estado | Recepcionista | mes en curso |
  | Tareas por estado | Mecánico | **sin filtro de fecha** |
- **Rationale:** cada gráfico tiene su rango natural y un selector obligaría a recargar tres consultas a la vez para un dato que el usuario no suele cambiar. El caso del mecánico se explica en la decisión 5.

### 5. El mecánico ve todas sus tareas, sin filtro de fecha
- **Decision:** las series de tareas del mecánico se calculan sobre **todas** sus tareas, sin acotar al mes en curso.
- **Rationale:** una tarea `Pendiente` del mes pasado sigue pendiente: el filtro de fecha escondería trabajo real. El avance, en cambio, se calcula sobre el mismo conjunto (todas sus tareas), de modo que porcentaje y barras son coherentes entre sí.

### 6. Sin SQL nuevo
- **Decision:** los tres ranking gerenciales se leen de los procedimientos paginados que ya agregó la spec `paginacion-listados`; las series de órdenes y tareas se calculan en memoria sobre las órdenes que `ObtenerDashboardHandler` ya trae.
- **Rationale:** el dashboard ya carga `GetReporteCompletoAsync()` con el alcance por rol; agrupar por estado o contar tareas del mecánico es O(n) sobre un conjunto que ya está en memoria.

### 7. "Top N + Otros" en la torta
- **Decision:** la torta de servicios muestra los 8 servicios con mayor cantidad y agrupa el resto bajo "Otros", reutilizando la lógica de `ServiciosPieChartGenerator.Normalizar` y su paleta.
- **Rationale:** con 13 servicios en los datos de ejemplo, una torta de 13 porciones es ilegible. El PDF agrupa de la misma forma, así que ambas representaciones coinciden.

### 8. La torta se dibuja con `PathGeometry`
- **Decision:** cada porción es un `Path` con un `ArcSegment` (clockwise) desde el centro, más un `LineSegment` de regreso al centro; la leyenda es una lista de `TextBlock` con un cuadro de color.
- **Rationale:** es el patrón estándar de WPF para sectores y permite reutilizar la paleta del generador de PDF (`ServiciosPieChartGenerator.Paleta`) para que ambos se vean iguales.

### 9. La serie es un DTO genérico
- **Decision:** `DashboardSerieDto { string Etiqueta; decimal Valor; }` representa cualquier serie (conteos y montos). El control decide el formato según el tipo de gráfico.
- **Rationale:** evita un DTO por tipo de serie y permite que el mismo control de barras sirve para ingresos, órdenes y tareas.

## Risks / Trade-offs

- **[Risk]** La lógica de gráfico queda duplicada respecto de los SVG de los PDF (mismo diseño, distinto código).
  - **Mitigation:** los colores y el formato abreviado de montos se replican exactamente; `GraficoCalculos` documenta que es el equivalente WPF de los generadores SVG.
- **[Risk]** El dashboard hace más consultas que antes (3 adicionales para el Administrador).
  - **Mitigation:** las tres son consultas paginadas y agregadas, sobre el rango que ya se consultaba; el botón "🔄 Actualizar" recarga todo.
- **[Risk]** Un `Path` con `ArcSegment` mal calculado produce porciones deformadas.
  - **Mitigation:** la geometría del sector se calcula en `GraficoCalculos` y se cubre con pruebas unitarias de los ángulos; la prueba STA instancia el control con datos reales.
- **[Risk]** Los usuarios que usaban la grilla de vehículos del dashboard la pierden.
  - **Mitigation:** el listado de vehículos con orden activa sigue disponible en *Órdenes de Trabajo* (que es paginado y filtrable por búsqueda, estado y rango de fechas).
