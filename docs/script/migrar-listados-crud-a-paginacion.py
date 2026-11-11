"""Migra a paginacion en BD las pantallas de listado CRUD (Fase 2 de la spec paginacion-listados).

Transformaciones aplicadas a cada Ctl*.xaml:
  1. Declara el namespace xmlns:compartido (CtlPaginacion).
  2. Agrega una fila para la barra de paginacion (la tabla pasa a la fila 3 y el pie a la 4).
  3. Inserta <compartido:CtlPaginacion Grid.Row="3" Model="{Binding}"/>.
  4. Enlaza la grilla a la coleccion paginada en lugar del listado filtrado en memoria.
  5. Agrega TextChanged / KeyDown al TextBox de busqueda.
  6. Agrega Click al check "Mostrar inactivos".
"""

import pathlib
import re
import sys

BASE = pathlib.Path(r"E:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular")

PANTALLAS = [
    ("Presentacion/Pantalla/Marca/CtlMarca.xaml", "ListadoMarcasFiltered", "Marcas"),
    ("Presentacion/Pantalla/Modelo/CtlModelo.xaml", "ListadoModelosFiltered", "Modelos"),
    ("Presentacion/Pantalla/Vehiculo/CtlVehiculo.xaml", "ListadoVehiculosFiltered", "Vehiculos"),
    ("Presentacion/Pantalla/Rol/CtlRol.xaml", "ListadoRolesFiltered", "Roles"),
    ("Presentacion/Pantalla/Usuario/CtlUsuario.xaml", "ListadoUsuariosFiltered", "Usuarios"),
    ("Presentacion/Pantalla/Servicio/CtlServicio.xaml", "ListadoServiciosFiltered", "Servicios"),
    ("Presentacion/Pantalla/Taller/CtlTaller.xaml", "ListadoTalleresFiltered", "Talleres"),
]

NS_COMPARTIDO = 'xmlns:compartido="clr-namespace:TP_ControlVehicular.Presentacion.Pantalla.Compartido"'


def transformar(contenido: str, listado_viejo: str, coleccion: str) -> str:
    # 1. namespace del control compartido
    if NS_COMPARTIDO not in contenido:
        contenido = re.sub(
            r'(xmlns:d="http://schemas\.microsoft\.com/expression/blend/2008"[^\n]*\n)',
            r'\1             ' + NS_COMPARTIDO + '\n',
            contenido,
            count=1,
        )

    # 2. fila nueva para la barra de paginacion (despues de la fila Height="*")
    if "<!-- Barra de Paginación -->" not in contenido:
        contenido = re.sub(
            r'(\n(\s*)<RowDefinition Height="\*"/>)',
            r'\1\n\2<!-- Barra de Paginación -->\n\2<RowDefinition Height="Auto"/>',
            contenido,
            count=1,
        )

    # 3. pie de acciones pasa a la fila 4 y se inserta la barra en la fila 3
    if "<compartido:CtlPaginacion" not in contenido:
        def _insertar_paginacion(m: "re.Match[str]") -> str:
            sangria = m.group(1)
            return (
                f"{sangria}<!-- BARRA DE PAGINACIÓN -->\n"
                f'{sangria}<compartido:CtlPaginacion Grid.Row="3" Model="{{Binding}}"/>\n'
                f"\n"
                f'{sangria}<Border Grid.Row="4"'
            )

        contenido = re.sub(
            r'(?m)^([ \t]*)<Border Grid\.Row="3"',
            _insertar_paginacion,
            contenido,
            count=1,
        )

    # 4. la grilla se enlaza a la coleccion paginada
    contenido = contenido.replace(
        f'ItemsSource="{{Binding {listado_viejo}}}"',
        f'ItemsSource="{{Binding {coleccion}}}"',
    )

    # 5. busqueda server-side: recarga con debounce y con la tecla Enter
    if 'TextChanged="TxtBusqueda_TextChanged"' not in contenido:
        contenido = re.sub(
            r'(<TextBox Text="\{Binding TextoBusqueda, UpdateSourceTrigger=PropertyChanged\}")(\s*)',
            r'\1\n'
            r'                             TextChanged="TxtBusqueda_TextChanged"\n'
            r'                             KeyDown="TxtBusqueda_KeyDown"\n'
            r'                             ',
            contenido,
            count=1,
        )

    # 6. el check de inactivos recarga el listado
    if 'ChkMostrarInactivos_Click' not in contenido:
        contenido = re.sub(
            r'(<CheckBox Content="Mostrar inactivos"[^>]*?)(/>)',
            r'\1 Click="ChkMostrarInactivos_Click" \2',
            contenido,
            count=1,
        )

    return contenido


def main() -> int:
    for relativa, listado_viejo, coleccion in PANTALLAS:
        ruta = BASE / relativa
        if not ruta.is_file():
            print(f"ERROR: no existe {relativa}")
            return 1

        original = ruta.read_text(encoding="utf-8")
        modificado = transformar(original, listado_viejo, coleccion)

        if original == modificado:
            print(f"SIN CAMBIOS: {relativa}")
            continue

        ruta.write_text(modificado, encoding="utf-8", newline="")
        print(f"OK: {relativa}")

    return 0


if __name__ == "__main__":
    sys.exit(main())
