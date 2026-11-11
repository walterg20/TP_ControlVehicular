import pathlib

BASE = pathlib.Path(r"E:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular")
p = BASE / "bd" / "03_procedimientos.sql"

d = p.read_text(encoding="utf-8")
antes = d.count("AS CantidadServicio\n")
d = d.replace("COUNT(rs.Id) AS CantidadServicio\n", "COUNT(rs.Id) AS CantidadServicios\n")
p.write_text(d, encoding="utf-8", newline="")
print(f"Renombradas {antes} apariciones de AS CantidadServicio -> AS CantidadServicios")
