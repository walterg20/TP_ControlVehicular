# Plan de Ejecución: Gráficos de Barras en Reporte Gerencial (PDF)

**Fecha:** 2026-10-28  
**Autor:** Antigravity  
**Contexto:** Se requiere incorporar gráficos de barras a las secciones de "Ingresos Totales" y "Modelos Más Reparados" dentro del Reporte Gerencial.

## 1. Alcance y Decisiones de Diseño

- **Destino de los Gráficos:** Exclusivamente en los reportes PDF exportados, en consistencia con la directriz de arquitectura documentada en `docs/referencia/reportes.md` y el precedente de `ServiciosPieChartGenerator`.
- **Formato y Tecnología:** 
  - Generación de SVG vectoriales puros en la capa `Negocio/Reportes/Documentos/`.
  - Integración nativa con QuestPDF mediante la extensión `.Svg(...)`.
  - Cero dependencias adicionales en `TP_ControlVehicular.csproj`.
- **Orientación de Gráficos:**
  - **Ingresos Totales por Fecha:** Barras verticales (columnas) para representar la evolución temporal cronológica con líneas guía y montos.
  - **Modelos Más Reparados:** Barras horizontales ordenadas de mayor a menor con nombres de modelo (`MarcaModelo`) en el eje Y y cantidad de reparaciones al extremo de la barra.

## 2. Archivos a Modificar y Crear

1. **`Negocio/Reportes/Documentos/IngresosBarChartGenerator.cs` (Nuevo)**
   - Generación de SVG de barras verticales para ingresos diarios.
2. **`Negocio/Reportes/Documentos/ModelosBarChartGenerator.cs` (Nuevo)**
   - Generación de SVG de barras horizontales para el ranking de modelos.
3. **`Presentacion/ViewModels/ReporteGerencialViewModel.cs` (Modificación)**
   - Inserción de las secciones gráficas en `ComposeIngresosContent` y `ComposeModelosContent`.
4. **`TP_ControlVehicular.Tests/Unit/IngresosBarChartGeneratorTests.cs` (Nuevo)**
   - Pruebas unitarias de generación de SVG de ingresos (datos válidos, vacíos, escala).
5. **`TP_ControlVehicular.Tests/Unit/ModelosBarChartGeneratorTests.cs` (Nuevo)**
   - Pruebas unitarias de generación de SVG de modelos (datos válidos, escape XML, vacíos).

## 3. Criterios de Aceptación
- El PDF de Ingresos Totales muestra el gráfico de barras verticales encima de la tabla detallada.
- El PDF de Modelos Más Reparados muestra el gráfico de barras horizontales encima de la tabla detallada.
- Si no hay datos en el período, el PDF maneja el estado sin excepciones.
- Toda la suite de pruebas unitarias compila y pasa al 100%.
