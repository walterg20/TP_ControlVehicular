# Referencia — Dashboard por perfil

Conocimiento de dominio de `CtlDashboard` / `ObtenerDashboardHandler`. Se enlaza desde `AGENTS.md` → *Referencia por dominio*.

- **Métricas reales por orden** (prohibido fabricar factores): `ObtenerDashboardHandler` inyecta `IRegistroServicioRepository` + `IReporteGerencialRepository` + `IMapper` (ya no `IVehiculoRepository`) y calcula todo desde órdenes reales de `GetReporteCompletoAsync`.
  - **Vehículos Activos** = vehículos distintos con orden en {`Abierta`, `En Proceso`}.
  - **En Proceso** = órdenes activas (unidad orden, no tarea).
  - **Entregadas Hoy** = órdenes en {`Completada`, `Pagada`} con `Fecha` = hoy (aproximado: el modelo no tiene marca de entrega/finalización).
- **Scoping por rol**: Admin/Recepcionista → taller completo (`GetReporteCompletoAsync(null)`); Mecánico (`Rol=3`) → solo sus órdenes (`GetReporteCompletoAsync(mecanicoId)`).
- **Ingresos del Mes** solo Administrador: `ObtenerIngresosPorFechaAsync(primerDíaDelMes, hoy)` (`sp_ReporteIngresosAdmin`, sin SQL nuevo) → `IngresosMesTotal` + `MostrarIngresos`.
- La fila de resumen es un `UniformGrid` con `Columns="{Binding TarjetasResumenVisible}"` (3 o 4) y la 4.ª tarjeta con `Visibility` ↔ `MostrarIngresos`.
- Botón **🔄 Actualizar** ligado a `CargarDashboardCommand`.
