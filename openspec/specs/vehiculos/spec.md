# 004 · Vehículos

**Estado:** implementado ✅

## Qué hace

Permite gestionar vehículos registrados: alta (patente única, año, color, cliente propietario, modelo), listado general con filtros, listado por cliente, edición y eliminación. Cada vehículo vincula un `Cliente` y un `Modelo` (que a su vez trae su `Marca`). La UI muestra grilla con patente, año, color, cliente, modelo y marca; modal para alta/edición con combos de clientes y modelos.

## Por qué

Es la entidad transaccional central del sistema: materializa la relación propietario–vehículo–modelo/marca. Habilita consultas operativas (¿qué autos tiene este cliente?), control de patentes únicas y reportes por marca/modelo. Requisito previo para futuras features de movimiento, infracciones, seguros.

## Criterios de aceptación

_Condiciones verificables que deben cumplirse para dar la feature por terminada. Redacta cada una de forma que se pueda comprobar con un sí/no. Marca `[x]` al cumplirse._

- [x] Registrar vehículo con patente única, cliente y modelo válidos persiste y devuelve `VehiculoDto` con nombres compuestos.
- [x] Rechazar patente duplicada (normalizada sin espacios, mayúsculas) con error controlado.
- [x] Rechazar cliente inexistente / modelo inexistente (validación handler + FK BD).
- [x] Listar vehículos general carga `ObservableCollection<VehiculoDto>` con `ClienteNombre`, `ModeloNombre`, `MarcaNombre`.
- [x] Listar vehículos por cliente (`ListarVehiculosPorClienteHandler`) filtra correctamente y devuelve `VehiculoPorClienteDto`.
- [x] Editar vehículo actualiza campos (patente, año, color, cliente, modelo) y persiste.
- [x] Eliminar vehículo sin dependencias borra registro.
- [x] Modal `CtlVehiculo` abre centrado, `Owner = Window.GetWindow(this)`, combos Cliente/Modelo poblados y bindeados.
- [x] `VehiculoViewModel` (sin `BaseViewModel`) expone propiedades planas con `OnPropertyChanged` manual; binding funcional.
- [x] AutoMapper resuelve `MarcaNombre` vía `Modelo.Marca.Nombre` sin ciclos.

## Fuera de alcance

_Lo que esta feature NO incluye, para evitar que crezca. Si algo se difiere, enlaza a dónde (roadmap/backlog)._

- Historial de cambios de propietario (transferencias) — roadmap.
- Vencimientos (VTV, seguro, patente) y alertas — feature "Vencimientos".
- Imágenes/fotos del vehículo — backlog.
- Importación masiva desde DNRPA — roadmap.
- Soft delete / baja lógica — no requerido.