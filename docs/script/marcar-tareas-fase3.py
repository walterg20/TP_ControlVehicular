import pathlib

BASE = pathlib.Path(r"E:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular")
p = BASE / "openspec" / "changes" / "paginacion-listados" / "tasks.md"

d = p.read_text(encoding="utf-8")
d = d.replace("- [ ] 3.", "- [x] 3.")
d = d.replace("- [ ] C.1", "- [x] C.1").replace("- [ ] C.2", "- [x] C.2").replace("- [ ] C.3", "- [x] C.3")
p.write_text(d, encoding="utf-8", newline="")
print("tareas de Fase 3 y cierre marcadas (falta C.4: merge y archivado)")
