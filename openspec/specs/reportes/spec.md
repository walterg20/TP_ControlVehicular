# Especificación: Módulo de Reportes

## 1. Objetivo
Generar información estadística y reportes imprimibles/exportables para el taller, separando la visión operativa de la visión gerencial.

## 2. Requerimientos
* **Reporte de Órdenes:** Un PDF imprimible para entregar al cliente cuando deja el vehículo o cuando se le entrega la factura (Comprobante de Recepción / Comprobante de Pago).
* **Reporte Operativo:** Cantidad de órdenes atendidas por cada mecánico en un rango de fechas. Vehículos más frecuentes.
* **Reporte Gerencial:** Ingresos generados ($) en un rango de fechas, separados por tipo de servicio.
* (Opcional) Uso de librerías como iText7 o QuestPDF para la generación de PDFs.

## 3. Criterios de Aceptación
- [x] Se puede exportar el reporte operativo a un formato visible (PDF o Excel).
- [x] Se puede imprimir el comprobante de recepción de una Orden de Servicio.