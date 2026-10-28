# -*- coding: utf-8 -*-
"""
Aplica el bloque generado a bd/02_datos.sql asegurando codificación UTF-8 pura.
"""
import os

BD_PATH = os.path.abspath("bd/02_datos.sql")
SEED_BLOCK_PATH = os.path.abspath("docs/script/bloque_seed_generado.sql")

with open(BD_PATH, "r", encoding="utf-8") as f:
    original_sql = f.read()

with open(SEED_BLOCK_PATH, "r", encoding="utf-8") as f:
    seed_block = f.read()

# Actualizar cabecera
old_header_target = """--   * Catálogo ampliado de Marca/Modelo representativo del mercado argentino
--     (45 marcas, 203 modelos; se preservan Toyota = Id 1 y Renault = Id 2)."""

new_header = """--   * Catálogo ampliado de Marca/Modelo representativo del mercado argentino
--     (45 marcas, 203 modelos; se preservan Toyota = Id 1 y Renault = Id 2).
--   * Usuarios del equipo: Recepcionista y Mecánicos de taller.
--   * Clientes (55 clientes locales con datos de contacto completos).
--   * Vehículos (55 vehículos vinculados a modelos oficiales con kilometraje real).
--   * PropietarioVehiculo (55 vinculaciones de titularidad vigentes).
--   * Órdenes de Servicio (61 órdenes entre Junio y Octubre 2026 en distintos estados).
--   * Detalles de Servicio (110+ tareas asignadas a mecánicos).
--   * Facturación y Pagos para órdenes finalizadas y cobradas."""

if old_header_target in original_sql:
    original_sql = original_sql.replace(old_header_target, new_header)

# Si el archivo original ya tiene la sección 11, reemplazar desde allí; si no, anexar al final
if "-- 11. Usuarios del equipo de trabajo" in original_sql:
    idx = original_sql.index("-- ============================================================================\n-- 11. Usuarios del equipo de trabajo")
    final_sql = original_sql[:idx] + seed_block
else:
    # Asegurar salto de línea
    if not original_sql.endswith("\n"):
        original_sql += "\n"
    final_sql = original_sql + "\n" + seed_block

with open(BD_PATH, "w", encoding="utf-8") as f:
    f.write(final_sql)

print(f"02_datos.sql actualizado con éxito. Tamaño final: {len(final_sql)} caracteres.")
