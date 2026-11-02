# Plan de Implementación: Asignación de Mecánico y Validación en Orden de Servicio

**Fecha:** 2026-11-02  
**Spec OpenSpec:** `orden-servicio-asignacion-mecanico`  
**Rama:** `feature/orden-servicio-asignacion-mecanico`

---

## 1. Contexto y Diagnóstico
Al registrar una orden de servicio con rol **Recepcionista**, cada ítem agregado (`DetalleServicio`) se inicializaba con `UsuarioId = 0` (`idMecanicoAuto = 0` al no ser mecánico).
Si la recepcionista no seleccionaba explícitamente un mecánico en la columna "Mecánico" del DataGrid y guardaba la orden:
- La validación `ValidateAll()` no revisaba los `DetallesOrdenActual`.
- Se intentaba insertar en la base de datos `DetalleServicio` con `UsuarioId = 0`.
- SQL Server arrojaba violación de clave foránea:
  `The INSERT statement conflicted with the FOREIGN KEY constraint "FK_DetalleServicio_Usuarios_UsuarioId".`

## 2. Objetivos
1. **Preselección por defecto:** Al agregar un servicio a la orden (`BtnAgregarItem_Click`), si el usuario no es mecánico (ej. recepcionista o admin) y hay mecánicos disponibles, preseleccionar automáticamente el primer mecánico activo (`MecanicosDisponibles.First().IdUsuario`).
2. **Validación defensiva en UI:** En `BtnGuardar_Click`, validar que todos los ítems de `DetallesOrdenActual` tengan `UsuarioId > 0`. Si alguno no lo tiene, mostrar aviso emergente amigable con `FrmConfirmacion.MostrarAviso`.
3. **Validación defensiva en ViewModel:** En `GuardarOrdenAsync()` / `ValidateAll()`, validar que no existan detalles con `UsuarioId <= 0`, notificando vía `RegistrationFailed`.
4. **Carga robusta de mecánicos:** En `LoadCombosAsync()`, usar `_usuarioRepository.GetActivosAsync()` para garantizar que se incluya la navegación `Rol` y solo se listen mecánicos activos.
5. **Pruebas unitarias:** Agregar tests en `TP_ControlVehicular.Tests` que verifiquen las validaciones y asignaciones.

---

## 3. Tareas
- [x] Tarea 1: Crear especificación formal en `openspec/changes/orden-servicio-asignacion-mecanico/`.
- [x] Tarea 2: Actualizar `OrdenServicioViewModel.cs` (`LoadCombosAsync` con `GetActivosAsync` y validación de `UsuarioId` en detalles).
- [x] Tarea 3: Actualizar `CtlOrdenServicioForm.xaml.cs` (preselección en `BtnAgregarItem_Click` y validación en `BtnGuardar_Click`).
- [x] Tarea 4: Agregar pruebas unitarias en `TP_ControlVehicular.Tests`.
- [x] Tarea 5: Compilación y validación de tests.
