"""Corrige la indentacion y el orden de los comentarios tras insertar las barras de paginacion."""

import pathlib
import sys

BASE = pathlib.Path(r"E:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular")
PANTALLA = BASE / "Presentacion" / "Pantalla"


def corregir_orden_servicio() -> None:
    ruta = PANTALLA / "OrdenServicio" / "CtlOrdenServicio.xaml"
    d = ruta.read_text(encoding="utf-8")

    # El comentario de la tabla debe preceder a la fila Height="*".
    d = d.replace(
        '            <RowDefinition Height="*"/>\n'
        '            <!-- Barra de Paginación -->\n'
        '            <RowDefinition Height="Auto"/>\n'
        '            <!-- Tabla de Órdenes -->\n',
        '            <!-- Tabla de Órdenes -->\n'
        '            <RowDefinition Height="*"/>\n'
        '            <!-- Barra de Paginación -->\n'
        '            <RowDefinition Height="Auto"/>\n',
    )

    # El comentario del pie va junto al Border del pie (fila 4).
    d = d.replace(
        '        <!-- PIE DE ACCIONES -->\n'
        '        <!-- BARRA DE PAGINACIÓN -->\n',
        '        <!-- BARRA DE PAGINACIÓN -->\n',
    )
    d = d.replace(
        '        <compartido:CtlPaginacion Grid.Row="3" Model="{Binding}"/>\n'
        '\n'
        '        <Border Grid.Row="4"',
        '        <compartido:CtlPaginacion Grid.Row="3" Model="{Binding}"/>\n'
        '\n'
        '        <!-- PIE DE ACCIONES -->\n'
        '        <Border Grid.Row="4"',
    )

    ruta.write_text(d, encoding="utf-8", newline="")
    print("OK: CtlOrdenServicio.xaml")


def corregir_mis_trabajos() -> None:
    ruta = PANTALLA / "MisTrabajos" / "CtlMisTrabajos.xaml"
    d = ruta.read_text(encoding="utf-8")

    d = d.replace(
        '                <compartido:CtlPaginacion Grid.Row="1" Model="{Binding}"/>\n'
        '</Grid>\n',
        '                <compartido:CtlPaginacion Grid.Row="1" Model="{Binding}"/>\n'
        '                </Grid>\n',
    )

    ruta.write_text(d, encoding="utf-8", newline="")
    print("OK: CtlMisTrabajos.xaml")


def corregir_reporte_ordenes() -> None:
    ruta = PANTALLA / "Reporte" / "CtlReporteOrdenes.xaml"
    d = ruta.read_text(encoding="utf-8")

    # Restaura el icono del boton Limpiar.
    d = d.replace(
        '<Button Content="Limpiar" Command="{Binding LimpiarFiltrosCommand}"',
        '<Button Content="\U0001F504 Limpiar" Command="{Binding LimpiarFiltrosCommand}"',
    )

    ruta.write_text(d, encoding="utf-8", newline="")
    print("OK: CtlReporteOrdenes.xaml")


def main() -> int:
    corregir_orden_servicio()
    corregir_mis_trabajos()
    corregir_reporte_ordenes()
    return 0


if __name__ == "__main__":
    sys.exit(main())
