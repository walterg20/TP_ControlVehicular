import pathlib

BASE = pathlib.Path(r"E:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular")
p = BASE / "Presentacion" / "Pantalla" / "Reporte" / "CtlReporteGerencial.xaml"

d = p.read_text(encoding="utf-8")
antes = d.count('\n<compartido:CtlPaginacion Grid.Row="2"')
d = d.replace('\n<compartido:CtlPaginacion Grid.Row="2"',
              '\n                        <compartido:CtlPaginacion Grid.Row="2"')
p.write_text(d, encoding="utf-8", newline="")
print(f"Indentadas {antes} barras de paginacion")
