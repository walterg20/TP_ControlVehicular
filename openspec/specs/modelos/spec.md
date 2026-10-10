# 002 · Modelos

**Estado:** implementado ✅

## Qué hace

Permite gestionar modelos de vehículos (nombre + marca): alta, edición, listado y eliminación. Cada modelo pertenece a una marca obligatoria (`MarcaId` FK). La UI muestra un combo de marcas en el modal de alta/edición y una grilla con nombre del modelo y nombre de su marca.

## Por qué

Los modelos son el catálogo estandarizado para registrar vehículos concretos. Evitan duplicados y errores de tipeo (p. ej. "Corola" vs "Corolla") y permiten reportes agrupados por modelo. Requisito previo para la feature Vehículos.

## Criterios de aceptación

_Condiciones verificables que deben cumplirse para dar la feature por terminada. Redacta cada una de forma que se pueda comprobar con un sí/no. Marca `[x]` al cumplirse._

- [x] Registrar modelo con nombre y marca válida persiste y devuelve `ModeloDto` con `ModeloId`.
- [x] Rechazar nombre duplicado dentro de la misma marca (error controlado).
- [x] Rechazar modelo sin marca seleccionada (validación VM + NOT NULL BD).
- [x] Listar modelos carga `ObservableCollection<ModeloDto>` con `MarcaNombre` visible.
- [x] Editar modelo actualiza nombre y/o marca y persiste.
- [x] Eliminar modelo sin vehículos asociados borra registro; con vehículos muestra error referencial.
- [x] Modal de modelo abre centrado, `Owner = Window.GetWindow(this)`, combo marcas poblado desde `IMarcaRepository`.
- [x] `ModeloViewModel` notifica cambios vía `OnPropertyChanged()` / `SetProperty`.
- [x] AutoMapper mapea `Modelo` → `ModeloDto` incluyendo `MarcaNombre`.

## Fuera de alcance

_Lo que esta feature NO incluye, para evitar que crezca. Si algo se difiere, enlaza a dónde (roadmap/backlog)._

- Versionado histórico de modelos (año inicio/fin producción) — roadmap.
- Atributos técnicos por modelo (cilindrada, combustible, transmisión) — backlog.
- Importación catálogo desde fuente externa — roadmap.