# Spec: Reportes para Mecánicos (Historial y Hoja Diaria)

## Objetivo
Implementar dos reportes operativos en PDF diseñados específicamente para el rol de Mecánico:
1. **Historial Clínico del Vehículo**: Permite ver todas las reparaciones y servicios previos realizados al vehículo seleccionado.
2. **Hoja de Trabajo Diaria**: Una lista con los vehículos y tareas asignadas al mecánico para la jornada, excluyendo información financiera.
Ambos usarán Procedimientos Almacenados en la base de datos y se generarán en PDF con QuestPDF.

## User Review Required
> [!IMPORTANT]
> - El mecánico verá un botón "Historial Clínico" al seleccionar un vehículo en "Mis Trabajos".
> - Verá un botón "Tareas del Día" arriba de su grilla para imprimir su hoja de ruta.
> - Ninguno de los dos reportes mostrará precios o totales (solo datos técnicos y operativos). ¿Estás de acuerdo con omitir los precios para el mecánico?

## Proposed Changes

---

### Base de Datos
#### [NEW] bd/11_SP_ReportesMecanico.sql
Creará dos Stored Procedures:
- sp_ReporteHistorialVehiculo: Recibe @VehiculoId. Devuelve Fecha, Servicio, Mecánico que lo hizo, Estado y Observaciones.
- sp_ReporteHojaTrabajoDiaria: Recibe @MecanicoId y @Fecha. Devuelve los vehículos asignados al mecánico en estados no finalizados.

---

### Backend
#### [NEW] Negocio/DTOs/Reportes/HistorialVehiculoDto.cs y HojaTrabajoDto.cs
Data Transfer Objects para mapear los resultados de los SPs.
#### [MODIFY] Datos/Repositories/ReporteRepository.cs (e IReporteRepository)
Se añadirán ObtenerHistorialVehiculoAsync y ObtenerHojaTrabajoDiariaAsync usando _dbContext.Database.SqlQueryRaw<T>("EXEC ...").
#### [NEW] Negocio/Services/ObtenerHistorialVehiculoHandler.cs y ObtenerHojaTrabajoDiariaHandler.cs
Encapsulan las llamadas a los repositorios.

---

### Frontend
#### [MODIFY] Presentacion/Pantalla/MisTrabajos/CtlMisTrabajos.xaml
Añadir botón "Imprimir Historial Clínico" (habilitado al seleccionar un item) y "Imprimir Tareas del Día".
#### [MODIFY] Presentacion/ViewModels/MisTrabajosViewModel.cs
Añadir GenerarPdfHistorialCommand y GenerarPdfHojaTrabajoCommand usando QuestPDF para construir un documento estético y llamar a Process.Start para abrir el PDF, idéntico a cómo se hace en ReporteOrdenesViewModel.cs.

## Verification Plan
### Manual Verification
1. Entrar como Mecánico.
2. Hacer clic en "Tareas del Día" y verificar que se abra un PDF con la hoja de ruta usando QuestPDF.
3. Seleccionar una orden, hacer clic en "Historial Clínico" y comprobar que el SP devuelva servicios anteriores y se genere el PDF correctamente.
