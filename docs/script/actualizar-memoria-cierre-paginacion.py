import pathlib

BASE = pathlib.Path(r"E:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular")
p = BASE / "MEMORY.md"

d = p.read_text(encoding="utf-8")

# 1. Rama de trabajo: la spec quedo mergeada y archivada.
viejo = (
    "- Rama de trabajo: `feature/paginacion-listados` (desde `develop`). "
    "Change en curso: `openspec/changes/paginacion-listados/`."
)
nuevo = (
    "- Rama de trabajo: `develop`. La spec `paginacion-listados` quedo **mergeada y archivada** "
    "en `openspec/changes/archive/2026-10-29-paginacion-listados/` (rama `feature/paginacion-listados`, "
    "merge `--no-ff`). El trabajo siguiente es la spec `dashboard-graficos-por-rol` "
    "(rama `feature/dashboard-graficos-por-rol` desde `develop`)."
)
if viejo in d:
    d = d.replace(viejo, nuevo)
    print("OK: rama de trabajo actualizada")
else:
    print("AVISO: no se encontro la linea de rama de trabajo")

# 2. La spec pasa a la lista de specs archivadas.
marca = "`2026-10-28-reporte-gerencial-paginacion-agrupacion-ingresos`."
if marca in d:
    d = d.replace(marca, marca + " `2026-10-29-paginacion-listados`.")
    print("OK: spec agregada al historial de archivadas")
else:
    print("AVISO: no se encontro el historial de specs archivadas")

# 3. La entrada de la spec pasa a estar cerrada.
viejo2 = "- **Paginación en BD (spec `paginacion-listados`, Fases 1, 2 y 3 completas)**:"
nuevo2 = "- **Paginación en BD (spec `paginacion-listados`, MERGEADA Y ARCHIVADA)**:"
if viejo2 in d:
    d = d.replace(viejo2, nuevo2)
    print("OK: estado de la spec actualizado")

p.write_text(d, encoding="utf-8", newline="")
print("MEMORY.md actualizado")
