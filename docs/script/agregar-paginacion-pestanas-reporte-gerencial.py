"""Agrega la barra de paginacion a las pestanas Top Clientes, Modelos Mas Reparados y
Servicios por Rango del Reporte Gerencial.

Cada pestana pasa a tener tres filas: boton de PDF, grilla y barra de paginacion.
"""

import pathlib
import re
import sys

BASE = pathlib.Path(r"E:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular")
RUTA = BASE / "Presentacion" / "Pantalla" / "Reporte" / "CtlReporteGerencial.xaml"

# (etiqueta del TabItem, propiedad del ViewModel, ItemsSource de la grilla)
PESTANAS = [
    ("Top Clientes", "PaginacionTopClientes", "TopClientes"),
    ("Modelos Más Reparados", "PaginacionModelos", "ModelosReparados"),
    ("Servicios por Rango", "PaginacionServicios", "ServiciosPorRango"),
]


def main() -> int:
    d = RUTA.read_text(encoding="utf-8")

    for etiqueta, propiedad, items_source in PESTANAS:
        if propiedad in d:
            print(f"- {etiqueta}: ya tiene barra")
            continue

        # Segmento de la pestana: desde su TabItem hasta el cierre del TabItem.
        inicio = d.find(f'<TabItem Header="{etiqueta}"')
        if inicio == -1:
            print(f"ERROR: no se encontro la pestana {etiqueta}")
            return 1
        fin = d.find("</TabItem>", inicio)
        if fin == -1:
            print(f"ERROR: no se encontro el cierre de la pestana {etiqueta}")
            return 1
        segmento = d[inicio:fin]

        # 1. Fila nueva para la barra, después de la fila Height="*".
        m = re.search(r'(?P<ind>[ \t]*)<Grid\.RowDefinitions>\n'
                      r'(?P=ind)    <RowDefinition Height="Auto"/>\n'
                      r'(?P=ind)    <RowDefinition Height="\*"/>\n'
                      r'(?P=ind)</Grid\.RowDefinitions>', segmento)
        if not m:
            print(f"ERROR: no se encontraron las filas de la pestana {etiqueta}")
            return 1
        ind = m.group("ind")
        nuevas_filas = (
            f"{ind}<Grid.RowDefinitions>\n"
            f"{ind}    <RowDefinition Height=\"Auto\"/>\n"
            f"{ind}    <RowDefinition Height=\"*\"/>\n"
            f"{ind}    <RowDefinition Height=\"Auto\"/>\n"
            f"{ind}</Grid.RowDefinitions>"
        )
        segmento = segmento[: m.start()] + nuevas_filas + segmento[m.end():]

        # 2. La barra se inserta justo despues del cierre de la grilla.
        m2 = re.search(r'(?P<ind>[ \t]*)(?P<dg>[ \t]*)<DataGrid Grid\.Row="1" ItemsSource="\{Binding '
                       + re.escape(items_source) + r'\}"',
                       segmento)
        if not m2:
            print(f"ERROR: no se encontro la grilla de la pestana {etiqueta}")
            return 1
        cierre_grilla = segmento.find("</DataGrid>", m2.end())
        if cierre_grilla == -1:
            print(f"ERROR: no se encontro el cierre de la grilla de {etiqueta}")
            return 1
        cierre_grilla += len("</DataGrid>\n")

        sangria = m2.group("dg")
        barra = f"{sangria}<compartido:CtlPaginacion Grid.Row=\"2\" Model=\"{{Binding {propiedad}}}\"/>\n"
        segmento = segmento[:cierre_grilla] + barra + segmento[cierre_grilla:]

        d = d[:inicio] + segmento + d[fin:]
        print(f"- {etiqueta}: barra agregada")

    RUTA.write_text(d, encoding="utf-8", newline="")
    print("OK: CtlReporteGerencial.xaml actualizado")
    return 0


if __name__ == "__main__":
    sys.exit(main())
