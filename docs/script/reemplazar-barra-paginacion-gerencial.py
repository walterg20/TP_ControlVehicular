import re
import sys

path = r"E:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular\Presentacion\Pantalla\Reporte\CtlReporteGerencial.xaml"

with open(path, "r", encoding="utf-8") as f:
    content = f.read()

start_marker = "<!-- Fila 3: Barra de Paginaci"
start = content.find(start_marker)
if start == -1:
    print("ERROR: no se encontro el inicio de la barra de paginacion")
    sys.exit(1)

end_marker = "</Border>"
end = content.find(end_marker, start)
if end == -1:
    print("ERROR: no se encontro el cierre del Border")
    sys.exit(1)
end += len(end_marker)

replacement = (
    "<!-- Fila 3: Barra de Paginacion (control reutilizable) -->\n"
    "                        <compartido:CtlPaginacion Grid.Row=\"3\" Model=\"{Binding}\"/>"
)

new_content = content[:start] + replacement + content[end:]

with open(path, "w", encoding="utf-8", newline="") as f:
    f.write(new_content)

print("OK: barra de paginacion reemplazada por CtlPaginacion")
