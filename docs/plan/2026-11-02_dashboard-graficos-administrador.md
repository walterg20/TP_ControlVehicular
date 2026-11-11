# Plan de ejecución — Gráficos de Administrador en el Dashboard

**Fecha:** 2026-11-02  
**Autor:** Walter Velazco / Antigravity  
**Spec relacionada:** `openspec/changes/dashboard-graficos-por-rol/`  
**Rama:** `feature/dashboard-graficos-por-rol`

---

## 1. Contexto y Diagnóstico
En la implementación inicial del Dashboard por rol (`d8a38bc`), se incorporaron controles nativos WPF (`CtlGraficoBarras`, `CtlGraficoBarrasHorizontal`, `CtlGraficoTorta`, `CtlGraficoAnilloProgreso`) para reemplazar la antigua grilla de vehículos con visuales adaptadas a cada rol.

Sin embargo, para el rol **Administrador**:
1. Los gráficos de **Modelos más reparados** y **Servicios por cantidad** consultaban exclusivamente el mes calendario actual (`inicioMes, hoy`). En los primeros días de mes o cuando los datos históricos se distribuyen en meses recientes anteriores (como el catálogo semilla de Junio a Octubre 2026), ambas consultas retornaban 0 registros, mostrando *"Sin datos para mostrar"*.
2. La distribución visual situaba a *Ingresos* y *Modelos* en la fila superior (muy comprimidos en altura) y a la *Torta de Servicios* estirada horizontalmente en la fila inferior.

---

## 2. Decisiones de Diseño
1. **Unificación temporal a 6 meses**: Se unifica el rango de consulta de los tres gráficos gerenciales del Administrador (Ingresos por mes, Modelos más reparados y Servicios por cantidad) a los **últimos 6 meses** (`desdeIngresos = inicioMes.AddMonths(-5)` hasta `hoy`), garantizando datos representativos, continuos y coherentes entre sí.
2. **Disposición visual en grilla asimétrica**:
   - **Columna izquierda (`Width="1.1*"`):** Gráfico de barras verticales de *Ingresos por mes* abarcando las dos filas (`RowSpan="2"`), brindando máxima altura vertical para la escala monetaria ($150k, $500k, etc.) y las etiquetas de mes (`MM/aa`).
   - **Columna derecha (`Width="*"`):**
     - **Fila superior:** Gráfico de barras horizontales de *Modelos más reparados* (top 8).
     - **Fila inferior:** Gráfico de torta con leyenda integrada de *Servicios por cantidad*.
3. **Claridad contextual**: Se actualiza el título del bloque a `📊 Reportes gerenciales (últimos 6 meses)` para reflejar la ventana temporal a simple vista.

---

## 3. Archivos Modificados
- `Negocio/Services/ObtenerDashboardHandler.cs`: Pasa `desdeIngresos` en las consultas de Modelos y Servicios.
- `Presentacion/Pantalla/Dashboard/CtlDashboard.xaml`: Reestructura la cuadrícula del Administrador con Ingresos a la izquierda y Modelos/Servicios a la derecha.
- `Presentacion/ViewModels/DashboardViewModel.cs`: Título descriptivo con indicador de rango de 6 meses.
- `openspec/changes/dashboard-graficos-por-rol/design.md`: Documentación de la decisión de rango uniforme.
