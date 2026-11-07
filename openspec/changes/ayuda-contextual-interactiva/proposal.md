# Proposal: Ayuda contextual interactiva en Orden de Servicio

## Why
Los operadores necesitan guía en contexto mientras completan una orden de servicio. Actualmente no existe ayuda contextual por control: el usuario debe consultar manuales externos. Se añade un panel lateral colapsable que muestra secciones de ayuda filtradas por rol y por el control con foco.

## What Changes
- Servicio `IAyudaService`/`AyudaService` que carga ayudas desde `Datos/Configuracion/ayudas_ordenes_servicio.json` (por rol, pantalla y control) y persiste preferencias de expansión en AppData.
- DTOs `AyudaContextoDto`, `AyudaPorRolDto`, `AyudaSeccionDto`.
- Control reutilizable `CtlAyudaContextual` (panel colapsable) con DependencyProperty `Secciones`.
- Integración en `CtlOrdenServicioForm`: panel lateral colapsable + carga de ayuda al cambiar el foco entre controles.
- `OrdenServicioViewModel`: propiedad `AyudaActual`, método `CargarAyudaPorControl`, `EstablecerRolActual`.
- Registro DI de `IAyudaService` en `App.xaml.cs`.
- Copia del JSON de ayudas al directorio de salida.

## Capabilities
### New Capabilities
- `ayuda-contextual`: ayuda contextual por rol y control en Orden de Servicio, panel colapsable, persistencia de preferencias.

## Impact
- Código: `Negocio/Services/IAyudaService.cs`, `Negocio/Services/AyudaService.cs`, `Negocio/DTOs/Ayuda*.cs`, `Presentacion/Pantalla/Compartido/CtlAyudaContextual.xaml(.cs)`, `Presentacion/Pantalla/OrdenServicio/CtlOrdenServicioForm.xaml(.cs)`, `Presentacion/ViewModels/OrdenServicioViewModel.cs`, `App.xaml(.cs)`, `TP_ControlVehicular.csproj`.
- Recurso: `Datos/Configuracion/ayudas_ordenes_servicio.json`.