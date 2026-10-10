# Propuesta de Feature: Workflow de Órdenes y Vistas por Rol

## 1. Objetivo
Implementar la lógica de estados automáticos de la Orden de Servicio y adaptar la interfaz de la grilla de detalles dependiendo del rol del usuario que haya iniciado sesión (Recepcionista vs. Mecánico).

## 2. Requerimientos y Reglas de Negocio

### A. Estados por Defecto y Automatización
* **Nueva Orden:** Al hacer clic en "Nueva Orden", el campo `Estado` debe inicializarse automáticamente en `"Abierta"` (o `"Pendiente"`).
* **Detalles por Defecto:** Al agregar un nuevo servicio a la grilla, el checkbox de "Revisado / OK" siempre inicia destildado (Falso).
* **Transición Automática:**
  * Si un mecánico tilda al menos un servicio, la Orden general pasa a estado `"En Proceso"`.
  * Si **todos** los servicios de la grilla están tildados (Revisado/OK), la Orden general pasa automáticamente a estado `"Completo"` o `"Finalizada"`.

### B. Experiencia de Usuario (UX) en Grilla por Roles
El sistema debe comportarse distinto según el rol del usuario conectado:

#### Rol: Recepcionista / Administrador
* Ve **todas** las filas de la grilla.
* Puede agregar servicios, asignar a cualquier mecánico, modificar precios y tildar/destildar el "Revisado / OK" libremente para cualquier ítem.

#### Rol: Mecánico (Ej. Pedro)
* **Visibilidad:** Puede ver **todas** las filas de la orden (para tener contexto general del vehículo y evitar intentar agregar servicios que ya está haciendo otro mecánico, como Sergio).
* **Bloqueo (Read-Only):** Las filas de servicios asignadas a *otros* mecánicos (ej. Sergio) aparecerán deshabilitadas (grisadas). El mecánico Pedro no podrá modificarlas ni tildar su estado.
* **Interacción:** El mecánico Pedro **solo podrá interactuar con las filas donde él esté asignado**. En sus filas, podrá tildar el checkbox "Revisado / OK", lo cual guardará su progreso.
* **Filtro Rápido (Opcional):** Se puede añadir un pequeño botón/check arriba de la grilla que diga "Ver solo mis servicios" para limpiar la vista si el auto tiene demasiados arreglos, pero por defecto verá el contexto completo.

## 3. Implementación Técnica Sugerida
* **ViewModel:** Inyectar o simular un "Contexto de Usuario Actual" (Session) para saber qué Rol e ID de Usuario está viendo la pantalla.
* **XAML (UI):** Usar `DataTriggers` en el estilo del `DataGridRow` para bloquear (`IsEnabled = False`) y opacar (`Opacity = 0.6`) las filas donde `Detalle.UsuarioId != Session.CurrentUsuarioId` si el rol es Mecánico.
* **Eventos:** Atar el evento `Checked` del checkbox a un Comando en el ViewModel que revalúe el estado general de la Orden.
