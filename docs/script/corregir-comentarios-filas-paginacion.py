"""Corrige el orden de los comentarios de las filas tras insertar la barra de paginacion."""

import pathlib
import re
import sys

BASE = pathlib.Path(r"E:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular")

PANTALLAS = [
    "Presentacion/Pantalla/Marca/CtlMarca.xaml",
    "Presentacion/Pantalla/Modelo/CtlModelo.xaml",
    "Presentacion/Pantalla/Vehiculo/CtlVehiculo.xaml",
    "Presentacion/Pantalla/Rol/CtlRol.xaml",
    "Presentacion/Pantalla/Usuario/CtlUsuario.xaml",
    "Presentacion/Pantalla/Servicio/CtlServicio.xaml",
    "Presentacion/Pantalla/Taller/CtlTaller.xaml",
]


def main() -> int:
    for relativa in PANTALLAS:
        ruta = BASE / relativa
        original = ruta.read_text(encoding="utf-8")
        contenido = original

        # 1. el comentario de la tabla debe preceder a la fila Height="*"
        contenido = re.sub(
            r'(\n(\s*)<RowDefinition Height="\*"/>\n\2<!-- Barra de Paginación -->\n\2<RowDefinition Height="Auto"/>\n\2)(<!-- Tabla de [^>]*? -->)',
            r'\n\2\3\n\2<RowDefinition Height="*"/>\n\2<!-- Barra de Paginación -->\n\2<RowDefinition Height="Auto"/>',
            contenido,
            count=1,
        )

        # 2. el comentario del pie va justo antes del Border del pie (fila 4)
        contenido = re.sub(
            r'\n(\s*)<!-- PIE DE ACCIONES -->\n(\s*)<!-- BARRA DE PAGINACIÓN -->\n',
            '\n\\2<!-- BARRA DE PAGINACIÓN -->\n',
            contenido,
            count=1,
        )
        contenido = re.sub(
            r'(\n(\s*)<compartido:CtlPaginacion Grid\.Row="3" Model="\{Binding\}"/>\n\n)',
            r'\1\2<!-- PIE DE ACCIONES -->\n',
            contenido,
            count=1,
        )

        if contenido != original:
            ruta.write_text(contenido, encoding="utf-8", newline="")
            print(f"OK: {relativa}")
        else:
            print(f"SIN CAMBIOS: {relativa}")

    return 0


if __name__ == "__main__":
    sys.exit(main())
