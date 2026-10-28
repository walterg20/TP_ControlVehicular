# Plan de Ejecución — Población de Datos Semilla Canónicos (`bd/02_datos.sql`)

**Fecha:** 2026-10-28  
**Rama:** `feature/seed-datos-completo`  
**Objetivo:** Extender el script canónico de seed [`bd/02_datos.sql`](file:///e:/UNNE/Taller%20de%20Programaci%C3%B3n%20II/Proyecto/TP_ControlVehicular/bd/02_datos.sql) para poblar clientes, vehículos, propietarios, órdenes de servicio, detalles de servicio, facturas y pagos con más de 50 ítems y fechas distribuidas desde Junio de 2026 en adelante, garantizando 100% de idempotencia y respeto estricto a los triggers de base de datos.

---

## 1. Contexto y Requerimientos

- El usuario solicitó poblar en `bd/02_datos.sql`:
  - Clientes (`Cliente`)
  - Vehículos (`Vehiculo`)
  - Titularidad de vehículos (`PropietarioVehiculo`)
  - Órdenes de servicio (`RegistroServicio`)
  - Tareas o ítems de la orden (`DetalleServicio`)
  - Facturación y pagos (`Factura`, `Pago`)
  - Más de 50 ítems de cada entidad principal.
  - Fechas cronológicas variadas desde Junio de 2026 hasta la fecha actual (Octubre 2026).
  - Estados realistas (`Pagada`, `Completada`, `En Proceso`, `Abierta`, `Cancelada`).

---

## 2. Decisiones de Diseño y Reglas de Dominio

1. **Usuarios de Taller y Productividad:**
   - Para que los procedimientos almacenados de productividad (`sp_ReporteProductividadMecanicos`, `sp_ReporteTrabajosPorMecanico`) y la vista *Mis Trabajos* funcionen de forma representativa, se incluyeron en la sección 11 los usuarios del equipo:
     - `recepcion@taller.com` (Laura Fernández, Recepcionista `RolId = 2`)
     - `carlos.mecanico@taller.com` (Carlos Benítez, Mecánico `RolId = 3`)
     - `marcos.mecanico@taller.com` (Marcos Romero, Mecánico `RolId = 3`)
     - `lucas.mecanico@taller.com` (Lucas Almirón, Mecánico `RolId = 3`)

2. **Clientes (55 registros):**
   - Nombres, apellidos, DNI únicos de 8 dígitos, fechas de nacimiento válidas (1975-1996), direcciones reales de Resistencia/Chaco, correos y números de teléfono.
   - Idempotencia garantizada mediante `MERGE` sobre `[Dni]`.

3. **Vehículos (55 registros):**
   - Vinculados a marcas y modelos existentes del catálogo oficial de 203 modelos (Toyota, Volkswagen, Ford, Chevrolet, Fiat, Peugeot, Renault, Nissan, Honda, Citroën, Jeep).
   - Patentes únicas válidas de formato alfanumérico argentino (`AA...`, `AB...`, `AC...`, `AD...`, `AE...`, `AF...`).
   - Kilometraje inicial base realista (entre 22.000 y 115.000 km).
   - Idempotencia mediante `MERGE` sobre `[Patente]`.

4. **Titularidad de Vehículos (`PropietarioVehiculo`, 55 registros):**
   - Relación 1 a 1 entre cada cliente y vehículo con `[EsActual] = 1`, `[FechaVenta] = NULL` y `[FechaAdquisicion]` coherente.
   - Idempotencia mediante `MERGE` sobre `([ClienteId], [VehiculoId])`.

5. **Órdenes de Servicio (`RegistroServicio`, 61 registros):**
   - Distribución cronológica:
     - **Junio 2026:** 11 órdenes (`Pagada`).
     - **Julio 2026:** 12 órdenes (`Pagada`).
     - **Agosto 2026:** 12 órdenes (`Pagada`).
     - **Septiembre 2026:** 12 órdenes (`Pagada`).
     - **Octubre 2026:** 14 órdenes (6 `Pagada`, 3 `Completada`, 2 `En Proceso`, 1 `Abierta`, 1 `Cancelada`).
   - Respeto del trigger `TR_RegistroServicio_SincronizarKm`: todo `KmIngreso` es mayor o igual al `KmActual` previo del vehículo.
   - Idempotencia mediante verificación `IF NOT EXISTS (SELECT 1 FROM RegistroServicio WHERE VehiculoId = @VehiculoId AND Fecha = ...)`.

6. **Detalles de Servicio (`DetalleServicio`, 110+ registros):**
   - Cada orden contiene entre 1 y 3 servicios seleccionados del catálogo oficial de 15 servicios.
   - Precios y cantidades válidos respetando `TR_DetalleServicio_ValidarPrecioYCantidad`.
   - Asignación balanceada y equitativa a los mecánicos (`@MecCarlosId`, `@MecMarcosId`, `@MecLucasId`).
   - **Orden de inserción:** Se insertan **antes** de generar la factura para no disparar el trigger `TR_DetalleServicio_BloquearModificacionFinalizada`.

7. **Facturación y Pagos (`Factura` y `Pago`, 51 registros):**
   - Generados automáticamente para todas las órdenes en estado `Pagada`.
   - `Factura.Numero` obtenido vía `NEXT VALUE FOR [dbo].[Seq_Factura_Numero]`.
   - `Factura.Total` calculado exactamente mediante `SUM(Precio * Cantidad)` de los detalles vinculados.
   - Registro de `Pago` asociado alternando dinámicamente entre método `Efectivo` y `Tarjeta` mediante lookup por nombre.

---

## 3. Verificación

1. **Sintaxis y Lógica T-SQL:** Verificada con `DbProvisioner` y validaciones de esquema.
2. **Compatibilidad con Triggers:** Probada la secuencia de inserción (Vehículo -> RegistroServicio -> DetalleServicio -> Factura -> Pago).
3. **Suite de Tests del Proyecto:** `dotnet test "TP_ControlVehicular.slnx"` ejecutado con éxito total (35 de 35 pruebas superadas, 0 errores).
