# 003 · Marcas

**Estado:** implementado ✅

## Qué hace

Permite gestionar marcas de vehículos: alta, edición, listado y eliminación. Cada marca tiene un nombre único (p. ej. "Toyota", "Ford"). La UI presenta una grilla con nombres y un modal simple para ingresar/editar el nombre.

## Por qué

Las marcas son la categorización de primer nivel del catálogo vehicular. Requisito previo obligatorio para crear Modelos (FK `MarcaId`). Estandarizan la nomenclatura y evitan variantes ("VW" vs "Volkswagen").

## Criterios de aceptación

_Condiciones verificables que deben cumplirse para dar la feature por terminada. Redacta cada una de forma que se pueda comprobar con un sí/no. Marca `[x]` al cumplirse._

- [x] Registrar marca con nombre único persiste y devuelve `MarcaDto` con `MarcaId`.
- [x] Rechazar nombre duplicado (case-insensitive) con error controlado.
- [x] Rechazar nombre vacío (validación VM + NOT NULL BD).
- [x] Listar marcas carga `ObservableCollection<MarcaDto>` en grilla.
- [x] Editar marca actualiza nombre y persiste (respeta unicidad).
- [x] Eliminar marca sin modelos asociados borra registro; con modelos muestra error referencial.
- [x] Modal de marca abre centrado, `Owner = Window.GetWindow(this)`.
- [x] `MarcaViewModel` notifica cambios vía `OnPropertyChanged()` / `SetProperty`.
- [x] AutoMapper mapea `Marca` → `MarcaDto` sin exponer `Modelos`.

## Fuera de alcance

_Lo que esta feature NO incluye, para evitar que crezca. Si algo se difiere, enlaza a dónde (roadmap/backlog)._

- Logo/imagen de marca — backlog.
- País de origen, sitio web, metadata extendida — roadmap.
- Marcas inactivas/históricas (soft delete) — no requerido.