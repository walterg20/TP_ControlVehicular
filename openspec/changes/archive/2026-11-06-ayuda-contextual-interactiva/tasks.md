# Tasks: Ayuda contextual interactiva en Orden de Servicio

- [ ] 1. Hacer `IAyudaService` opcional en el constructor de `OrdenServicioViewModel` (default null) para compilar tests.
- [ ] 2. Establecer rol actual en `CtlOrdenServicioForm.ConfigurarViewModel` (`EstablecerRolActual`).
- [ ] 3. Ampliar `ConfigurarAyudaContextual()` con handlers GotFocus para todos los controles.
- [ ] 4. Reescribir `ayudas_ordenes_servicio.json` con nombres de control reales y textos UTF-8 correctos.
- [ ] 5. Agregar entrada Content en `TP_ControlVehicular.csproj` para copiar el JSON al output.
- [ ] 6. Convertir el panel lateral en colapsable (ToggleButton + visibilidad) en `CtlOrdenServicioForm.xaml` y code-behind.
- [ ] 7. Carga inicial de ayuda al abrir el formulario.
- [ ] 8. Agregar tests: `AyudaServiceTests` y `OrdenServicioAyudaTests`.
- [ ] 9. Build + tests + commit.