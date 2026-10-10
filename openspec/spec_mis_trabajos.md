# Specification: Mis Trabajos (My Jobs) Panel

## 1. Overview
The "Mis Trabajos" panel is a dedicated workspace for users with the "Mecánico" (Mechanic) role. It aggregates all service details (tasks) assigned specifically to the logged-in mechanic across all active service orders, allowing them to track and update their workload efficiently without navigating through each individual order manually.

## 2. User Roles & Permissions
* **Mecánico:** Has full access to this panel. Can view assigned tasks, update their status (e.g., mark as "Revisado / OK"), and add observations.
* **Recepcionista / Administrador:** This view is primarily tailored for the mechanic's day-to-day workflow and might be hidden or read-only for admins.

## 3. Key Features & Acceptance Criteria

### 3.1. Workload Dashboard (Data Grid)
* **AC 3.1.1:** The panel must display a data grid of `DetalleServicio` (Service Details) assigned to the current `UsuarioId`.
* **AC 3.1.2:** The grid must show relevant context for each task: 
  * Order Number (ID)
  * Vehicle License Plate (Patente) & Model
  * Service/Task Name
  * Observations (Editable text field)
  * Action: Checkbox for "Revisado / OK"
  * Action: Button to "Ver Orden Completa" (View Full Order)

### 3.2. Task Interaction & Observations
* **AC 3.2.1:** The mechanic can toggle the "Revisado / OK" checkbox directly from this grid.
* **AC 3.2.2:** The mechanic can write and save "Observaciones" (Notes) directly in the grid.
* **AC 3.2.3:** Toggling the checkbox or saving observations must instantly update the `DetalleServicio` in the database, and trigger the `EvaluarEstadoGeneral()` logic on the parent `RegistroServicio`.

### 3.3. Full Order Context Navigation
* **AC 3.3.1:** Clicking on a task (or a specific "View" button) opens the full Order Details form (`CtlOrdenServicioForm`) so the mechanic can see the entire context (other tasks, vehicle details, etc.).
* **AC 3.3.2:** When the mechanic clicks "Volver" (Back) or finishes viewing the full order, the application must correctly route them back to the "Mis Trabajos" panel, not to the general Orders list.

### 3.4. Filtering and Sorting
* **AC 3.4.1:** A search bar must allow the mechanic to search by Vehicle License Plate or Order Number.
* **AC 3.4.2:** Quick filters (e.g., ComboBox or RadioButtons) must be included above the grid to filter by status: 
  * "Pendientes" (Pending)
  * "Finalizados" (Completed)
  * "Todos" (All)
* **AC 3.4.3:** By default, the grid should display "Pendientes" to focus on pending work.

## 4. Out of Scope
* Modifying the price or quantity of the assigned services (Mechanics cannot do this).
* Assigning tasks to other mechanics (Mechanics cannot re-assign).
