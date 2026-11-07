# manejo-errores Specification (delta)

## ADDED Requirements

### Requirement: Centralized Error Notification
The application SHALL provide a centralized error notifier (`IErrorNotifier`) that captures exceptions from silent catch blocks and surfaces them to the user via a unified toast or modal.

#### Scenario: Silent catch notifies
- **WHEN** a ViewModel catch block receives an exception
- **THEN** the notifier is invoked with the exception and context, and the user sees a toast/modal

#### Scenario: Global unhandled exception
- **WHEN** an unhandled exception occurs on the UI thread
- **THEN** the global handler notifies the user instead of crashing silently

### Requirement: Unified Toast/Modal
`MainWindow` SHALL expose `MostrarError(mensaje, modal)` that shows a toast (non-blocking, 3s) by default or a modal (`FrmConfirmacion.MostrarAviso`) when `modal=true`.

#### Scenario: Non-critical error toast
- **WHEN** a non-critical error is notified
- **THEN** a toast appears for 3 seconds and disappears

#### Scenario: Critical error modal
- **WHEN** a critical operation error is notified with modal=true
- **THEN** a modal dialog is shown requiring acknowledgment

### Requirement: Deduplication
The notifier SHALL deduplicate identical (context+message) notifications within a 2-second window to avoid spam.

#### Scenario: Duplicate suppressed
- **WHEN** the same error is notified twice within 2 seconds
- **THEN** only the first notification is shown

### Requirement: Logging
The notifier SHALL write the exception details (message, stack trace, context, timestamp) to `%AppData%/TP_ControlVehicular/logs/error.log`.

#### Scenario: Log entry created
- **WHEN** an error is notified
- **THEN** a log entry is appended to error.log