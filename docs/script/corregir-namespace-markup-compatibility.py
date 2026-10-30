import pathlib

base = pathlib.Path(r"E:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular")

mal = b"http://schemas.opencode.openxmlformats.org/markup-compatibility/2006"
bien = b"http://schemas.openxmlformats.org/markup-compatibility/2006"

objetivos = [
    base / "Presentacion" / "Pantalla" / "Cliente" / "CtlCliente.xaml",
    base / "Presentacion" / "Pantalla" / "Compartido" / "CtlPaginacion.xaml",
]

for p in objetivos:
    data = p.read_bytes()
    if mal in data:
        p.write_bytes(data.replace(mal, bien))
        print("corregido:", p)
    else:
        print("sin cambios:", p)
