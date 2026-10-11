# Flujo UX: Generación de Órdenes de Servicio

Este documento describe la experiencia de usuario (UX) implementada para la recepción del taller mecánico al momento de ingresar un vehículo y generar una Orden de Servicio. El diseño sigue un **Enfoque Cliente-Céntrico** priorizando la agilidad de carga y garantizando la coherencia histórica de los dueños de los vehículos.

## 1. Diagrama de Flujo (Recepcionista)

```mermaid
flowchart TD
    A[Inicio: Recepcionista abre Nueva Orden] --> B{¿El cliente existe?}
    B -- No --> C[Registrar Cliente Rápido]
    B -- Sí --> D[Seleccionar Cliente]
    C --> D
    D --> E{¿El vehículo está en su lista?}
    E -- Sí --> F[Seleccionar Vehículo del Combo]
    E -- No --> G[Click en '+ Rápido']
    G --> H[Ingresar Patente en InputBox]
    H --> I{¿Auto ya existe en el Taller?}
    I -- Sí --> J[Asignación Inteligente (Transfiere Titularidad)]
    I -- No --> K[Alta Completa de Vehículo]
    J --> F
    K --> F
    F --> L[Cargar Trabajos y Guardar Orden]
```

## 2. Descripción de Pantalla y Comportamientos

### A. Selección de Cliente (Punto de Partida)
La interfaz exige que lo primero en definirse sea el **Cliente**.
- Si es cliente habitual: Se lo busca por DNI o Nombre.
- Si es cliente nuevo: Se lo da de alta con un formulario abreviado.

### B. Filtrado Dinámico de Vehículos
Una vez que el cliente está seleccionado en la pantalla de la Orden de Servicio, el **ComboBox de Vehículos** aplica un filtro dinámico. 
- *Solo se despliegan las patentes de los vehículos que le pertenecen actualmente* (`PropietarioVehiculo.EsActual = true`).

### C. El "Bache" Lógico: Vehículos No Listados
Existen escenarios donde el cliente trae un vehículo que no figura en su combo desplegable:
1. Es un auto 0KM o que nunca vino al taller.
2. Es un auto usado que sí vino al taller antes, pero registrado a nombre del dueño anterior.

Para resolverlo sin salir de la pantalla, la recepcionista utiliza el botón **`+ Rápido`**:
1. Se abre un pequeño cuadro de diálogo solicitando únicamente la **Patente**.
2. El sistema busca esa patente en el repositorio central (`AsignarVehiculoPorPatenteHandler`).
3. **Manejo Histórico Automático:** Si el vehículo existía a nombre de "Juan" y lo acaba de traer "María" (el cliente actual), el sistema cierra la titularidad de Juan (agregando la `FechaVenta` y marcando `EsActual = false`) e inicia la titularidad de María (`EsActual = true`, `FechaAdquisicion = Hoy`).
4. El vehículo pasa automáticamente al listado de María y queda pre-seleccionado, listo para iniciar el trabajo.

## 3. Ventajas del Enfoque
- **Reducción de clics:** La recepcionista no necesita navegar por el ABM de Vehículos y cambiar dueños a mano.
- **Integridad:** Nunca se generan "patentes duplicadas".
- **Trazabilidad:** Se mantiene el historial de clientes que pasaron por el mismo vehículo a lo largo del tiempo.
