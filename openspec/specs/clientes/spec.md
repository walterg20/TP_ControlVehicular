# 001 · Clientes

**Estado:** implementado ✅

## Qué hace

Permite gestionar clientes del sistema: registrar nuevo cliente (nombre, apellido, DNI, email, teléfono, dirección), listar clientes existentes, editar datos y eliminar clientes. La UI presenta un UserControl modal (`CtlCliente`) para alta/edición y una grilla en la vista principal con comandos de acción. Cada cliente puede tener cero o más vehículos asociados (navegación `Cliente.Vehiculos`).

## Por qué

Es la entidad base del dominio vehicular: sin clientes no se pueden registrar vehículos ni generar reportes por propietario. Centraliza la identidad del propietario y soporta trazabilidad vehicular. Resuelve la necesidad operativa de dar de alta propietarios antes de ingresar sus vehículos.

## Criterios de aceptación

_Condiciones verificables que deben cumplirse para dar la feature por terminada. Redacta cada una de forma que se pueda comprobar con un sí/no. Marca `[x]` al cumplirse._

- [x] Registrar cliente válido persiste en SQL Server y devuelve `ClienteDto` con `ClienteId` generado.
- [x] Rechazar DNI duplicado con mensaje de error claro (no excepción sin manejar).
- [x] Rechazar email duplicado con mensaje de error claro.
- [x] Listar clientes carga `ObservableCollection<ClienteDto>` en `ClienteViewModel` y muestra en UI.
- [x] Editar cliente actualiza campos y persiste cambios (`UpdateAsync` en repo).
- [x] Eliminar cliente sin vehículos asociados borra registro; con vehículos muestra error referencial.
- [x] Modal `CtlCliente` se abre centrado, con `Owner = Window.GetWindow(this)`, y cierra tras guardar/cancelar.
- [x] Validación de campos requeridos (Nombre, Apellido, DNI, Email) en VM antes de invocar handler.
- [x] `ClienteViewModel` notifica cambios de propiedad vía `OnPropertyChanged()` (binding WPF funcional).
- [x] AutoMapper mapea `Cliente` → `ClienteDto` sin ciclos ni referencias nulas.

## Fuera de alcance

_Lo que esta feature NO incluye, para evitar que crezca. Si algo se difiere, enlaza a dónde (roadmap/backlog)._

- Búsqueda/filtrado avanzado de clientes (se hará en feature de listados/reportes).
- Historial de cambios/auditoría de clientes (backlog).
- Importación masiva desde Excel/CSV (roadmap: feature "Importación").
- Soft delete / papelera (no requerido; borrado físico actual).