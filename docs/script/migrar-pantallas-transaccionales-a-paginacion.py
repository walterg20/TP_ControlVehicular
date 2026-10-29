"""Migracion a paginacion en BD de las pantallas transaccionales y de reportes (Fase 3).

Cambios por pantalla:
  - CtlReporteOrdenes: barra CtlPaginacion dentro de la tarjeta de la grilla, grilla enlazada
    a la coleccion paginada, boton Buscar y eventos de busqueda.
  - CtlOrdenServicio: fila de paginacion entre la grilla y el pie de acciones.
  - CtlReportesRecepcion: una barra por grilla (historial y vehiculos por cliente).
  - CtlMisTrabajos: barra de paginacion en la grilla maestra.
"""

import pathlib
import re
import sys

BASE = pathlib.Path(r"E:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular")
PANTALLA = BASE / "Presentacion" / "Pantalla"

NS_COMPARTIDO = 'xmlns:compartido="clr-namespace:TP_ControlVehicular.Presentacion.Compartido"'


def agregar_ns(contenido: str) -> str:
    if "xmlns:compartido=" in contenido:
        return contenido
    return re.sub(
        r'(xmlns:d="http://schemas\.microsoft\.com/expression/blend/2008"[^\n]*\n)',
        r'\1             xmlns:compartido="clr-namespace:TP_ControlVehicular.Presentacion.Pantalla.Compartido"\n',
        contenido,
        count=1,
    )


def agregar_eventos_busqueda(contenido: str) -> str:
    """Agrega TextChanged / KeyDown al TextBox de busqueda."""
    if 'TextChanged="TxtBusqueda_TextChanged"' not in contenido:
        contenido = re.sub(
            r'(<TextBox\s+Text="\{Binding TextoBusqueda, UpdateSourceTrigger=PropertyChanged\}")(\s*)',
            r'\1\n'
            r'                                 TextChanged="TxtBusqueda_TextChanged"\n'
            r'                                 KeyDown="TxtBusqueda_KeyDown"\n'
            r'                                 ',
            contenido,
            count=1,
        )
    return contenido


def agregar_eventos_filtros(contenido: str) -> str:
    """Agrega los eventos de los DatePicker y del ComboBox de estado."""
    if 'SelectedDateChanged="DtpFechaDesde_SelectedDateChanged"' not in contenido:
        contenido = re.sub(
            r'(<DatePicker\s+SelectedDate="\{Binding FechaDesde, UpdateSourceTrigger=PropertyChanged\}")(\s*)',
            r'\1\n'
            r'                                    SelectedDateChanged="DtpFechaDesde_SelectedDateChanged"\n'
            r'                                    ',
            contenido,
            count=1,
        )
    if 'SelectedDateChanged="DtpFechaHasta_SelectedDateChanged"' not in contenido:
        contenido = re.sub(
            r'(<DatePicker\s+SelectedDate="\{Binding FechaHasta, UpdateSourceTrigger=PropertyChanged\}")(\s*)',
            r'\1\n'
            r'                                    SelectedDateChanged="DtpFechaHasta_SelectedDateChanged"\n'
            r'                                    ',
            contenido,
            count=1,
        )
    if 'SelectionChanged="CboEstado_SelectionChanged"' not in contenido:
        contenido = re.sub(
            r'(<ComboBox\s+SelectedValue="\{Binding EstadoFiltro, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged\}")(\s*)',
            r'\1\n'
            r'                                  SelectionChanged="CboEstado_SelectionChanged"\n'
            r'                                  ',
            contenido,
            count=1,
        )
    return contenido


def envolver_grilla(contenido: str, apertura_grid: str, cierre_grid: str, grid_reemplazo: str) -> str:
    """Reemplaza un bloque <Grid> ... </Grid> por el contenido indicado."""
    inicio = contenido.find(apertura_grid)
    if inicio == -1:
        raise RuntimeError(f"No se encontro el bloque: {apertura_grid[:60]}")
    fin = contenido.find(cierre_grid, inicio)
    if fin == -1:
        raise RuntimeError(f"No se encontro el cierre: {cierre_grid[:60]}")
    fin += len(cierre_grid)
    return contenido[:inicio] + grid_reemplazo + contenido[fin:]


# ----------------------------------------------------------------------------
# CtlReporteOrdenes
# ----------------------------------------------------------------------------
def migrar_reporte_ordenes() -> None:
    ruta = PANTALLA / "Reporte" / "CtlReporteOrdenes.xaml"
    contenido = ruta.read_text(encoding="utf-8")

    contenido = agregar_eventos_busqueda(contenido)

    # Boton Buscar junto al boton Limpiar.
    if 'Click="BtnBuscar_Click"' not in contenido:
        patron = re.compile(
            r'(?P<ind>[ \t]*)<Button HorizontalAlignment="Left" Content="[^"]*" Command="\{Binding LimpiarFiltrosCommand\}"'
            r'.*?</Button>',
            re.DOTALL,
        )
        m = patron.search(contenido)
        if not m:
            raise RuntimeError("No se encontro el boton Limpiar del Reporte de Ordenes")
        ind = m.group("ind")
        nuevo = (
            f'{ind}<StackPanel HorizontalAlignment="Left" Orientation="Horizontal">\n'
            f'{ind}    <Button Content="Limpiar" Command="{{Binding LimpiarFiltrosCommand}}"\n'
            f'{ind}            Padding="12,7" Background="#95A5A6" Foreground="White" FontWeight="Bold"\n'
            f'{ind}            BorderThickness="0" Cursor="Hand" ToolTip="Restablecer filtros">\n'
            f'{ind}        <Button.Resources>\n'
            f'{ind}            <Style TargetType="Border">\n'
            f'{ind}                <Setter Property="CornerRadius" Value="6"/>\n'
            f'{ind}            </Style>\n'
            f'{ind}        </Button.Resources>\n'
            f'{ind}    </Button>\n'
            f'{ind}    <Button Content="Buscar" Click="BtnBuscar_Click"\n'
            f'{ind}            Padding="12,7" Margin="8,0,0,0" Background="#2980B9" Foreground="White" FontWeight="Bold"\n'
            f'{ind}            BorderThickness="0" Cursor="Hand" ToolTip="Consultar con los filtros aplicados">\n'
            f'{ind}        <Button.Resources>\n'
            f'{ind}            <Style TargetType="Border">\n'
            f'{ind}                <Setter Property="CornerRadius" Value="6"/>\n'
            f'{ind}            </Style>\n'
            f'{ind}        </Button.Resources>\n'
            f'{ind}    </Button>\n'
            f'{ind}</StackPanel>'
        )
        contenido = contenido[: m.start()] + nuevo + contenido[m.end():]

    # La grilla pasa a ocupar la fila 0 de una Grid con la barra de paginacion en la fila 1.
    contenido = contenido.replace('ItemsSource="{Binding ListadoOrdenesFiltered}"', 'ItemsSource="{Binding Ordenes}"')

    if "<compartido:CtlPaginacion" not in contenido:
        apertura = "            <Grid>\n                <DataGrid x:Name=\"dgReporte\""
        m = re.search(re.escape(apertura), contenido)
        if not m:
            raise RuntimeError("No se encontro la Grid de la tabla del Reporte de Ordenes")
        ind = "                "
        nuevo_grid = (
            "            <Grid>\n"
            "                <Grid.RowDefinitions>\n"
            "                    <RowDefinition Height=\"*\"/>\n"
            "                    <RowDefinition Height=\"Auto\"/>\n"
            "                </Grid.RowDefinitions>\n"
            "                <DataGrid x:Name=\"dgReporte\" Grid.Row=\"0\""
        )
        contenido = contenido[: m.start()] + nuevo_grid + contenido[m.end():]

        # Cierre de la Grid: la barra va justo antes.
        cierre = "                </DataGrid>\n            </Grid>"
        m2 = contenido.find(cierre)
        if m2 == -1:
            raise RuntimeError("No se encontro el cierre de la tabla del Reporte de Ordenes")
        nuevo_cierre = (
            "                </DataGrid>\n"
            "                <compartido:CtlPaginacion Grid.Row=\"1\" Model=\"{Binding}\"/>\n"
            "            </Grid>"
        )
        contenido = contenido[:m2] + nuevo_cierre + contenido[m2 + len(cierre):]

    ruta.write_text(contenido, encoding="utf-8", newline="")
    print("OK: CtlReporteOrdenes.xaml")


# ----------------------------------------------------------------------------
# CtlOrdenServicio
# ----------------------------------------------------------------------------
def migrar_orden_servicio() -> None:
    ruta = PANTALLA / "OrdenServicio" / "CtlOrdenServicio.xaml"
    contenido = ruta.read_text(encoding="utf-8")

    contenido = agregar_ns(contenido)
    contenido = agregar_eventos_busqueda(contenido)
    contenido = agregar_eventos_filtros(contenido)
    contenido = contenido.replace('ItemsSource="{Binding ListadoOrdenesFiltered}"', 'ItemsSource="{Binding Ordenes}"')

    # Fila de paginacion entre la grilla (fila 2) y el pie de acciones.
    if "<!-- Barra de Paginación -->" not in contenido:
        contenido = re.sub(
            r'(\n([ \t]*)<RowDefinition Height="\*"/>)',
            r'\1\n\2<!-- Barra de Paginación -->\n\2<RowDefinition Height="Auto"/>',
            contenido,
            count=1,
        )

    if "<compartido:CtlPaginacion" not in contenido:
        m = re.search(r'(?m)^([ \t]*)<Border Grid\.Row="3"', contenido)
        if not m:
            raise RuntimeError("No se encontro el pie de acciones de OrdenServicio")
        sangria = m.group(1)
        nuevo = (
            f'{sangria}<!-- BARRA DE PAGINACIÓN -->\n'
            f'{sangria}<compartido:CtlPaginacion Grid.Row="3" Model="{{Binding}}"/>\n'
            f"\n"
            f'{sangria}<Border Grid.Row="4"'
        )
        contenido = contenido[: m.start()] + nuevo + contenido[m.end():]

    # Boton Buscar junto a las acciones inferiores.
    if 'Click="BtnBuscar_Click"' not in contenido:
        m = re.search(
            r'(?P<ind>[ \t]*)<Button Content="[^"]*" Command="\{Binding ImprimirListadoFiltradoCommand\}"',
            contenido,
        )
        if not m:
            raise RuntimeError("No se encontro el boton Imprimir Listado de OrdenServicio")
        ind = m.group("ind")
        boton = (
            f'{ind}<Button Content="Buscar" Click="BtnBuscar_Click"\n'
            f'{ind}        Padding="12,8" Margin="0,0,8,0" Background="#2980B9" Foreground="White" FontWeight="Bold"\n'
            f'{ind}        BorderThickness="0" Cursor="Hand" ToolTip="Consultar con los filtros aplicados">\n'
            f'{ind}    <Button.Resources>\n'
            f'{ind}        <Style TargetType="Border">\n'
            f'{ind}            <Setter Property="CornerRadius" Value="6"/>\n'
            f'{ind}        </Style>\n'
            f'{ind}    </Button.Resources>\n'
            f'{ind}</Button>\n'
        )
        contenido = contenido[: m.start()] + boton + contenido[m.start():]

    ruta.write_text(contenido, encoding="utf-8", newline="")
    print("OK: CtlOrdenServicio.xaml")


# ----------------------------------------------------------------------------
# CtlReportesRecepcion
# ----------------------------------------------------------------------------
def migrar_reportes_recepcion() -> None:
    ruta = PANTALLA / "Reporte" / "CtlReportesRecepcion.xaml"
    contenido = ruta.read_text(encoding="utf-8")

    contenido = agregar_ns(contenido)

    # La busqueda por patente y por DNI se lanzan con los botones "Generar".
    if 'TextChanged="TxtPatente_TextChanged"' not in contenido:
        contenido = re.sub(
            r'(<TextBox x:Name="txtPatente" Text="\{Binding Patente, UpdateSourceTrigger=PropertyChanged\}")',
            r'\1 TextChanged="TxtPatente_TextChanged"',
            contenido,
            count=1,
        )
    if 'TextChanged="TxtDni_TextChanged"' not in contenido:
        contenido = re.sub(
            r'(<TextBox x:Name="txtDNI" Text="\{Binding DNI, UpdateSourceTrigger=PropertyChanged\}")',
            r'\1 TextChanged="TxtDni_TextChanged"',
            contenido,
            count=1,
        )

    # Cada grilla gana su propia barra de paginacion.
    if "<compartido:CtlPaginacion" not in contenido:
        for nombre_grid, model in (
            ("dgResultadosHistorial", "{Binding}"),
            ("dgResultadosVehiculos", "{Binding PaginacionVehiculos}"),
        ):
            m = re.search(
                rf'(?P<ind>[ \t]*)<DataGrid x:Name="{nombre_grid}" ItemsSource="[^"]*" AutoGenerateColumns="True"',
                contenido,
            )
            if not m:
                raise RuntimeError(f"No se encontro la grilla {nombre_grid}")
            ind = m.group("ind")
            apertura = (
                f'{ind}<Grid>\n'
                f'{ind}    <Grid.RowDefinitions>\n'
                f'{ind}        <RowDefinition Height="*"/>\n'
                f'{ind}        <RowDefinition Height="Auto"/>\n'
                f'{ind}    </Grid.RowDefinitions>\n'
                f'{ind}    <DataGrid x:Name="{nombre_grid}" Grid.Row="0" ItemsSource="'
            )
            # Se conserva el resto de los atributos de la grilla.
            cola = contenido[m.end():]
            fin_grid = cola.find(f'</DataGrid>')
            if fin_grid == -1:
                raise RuntimeError(f"No se encontro el cierre de {nombre_grid}")
            atributos = cola[:fin_grid]
            cierre = (
                f'{atributos}</DataGrid>\n'
                f'{ind}    <compartido:CtlPaginacion Grid.Row="1" Model="{model}"/>\n'
                f'{ind}</Grid>'
            )
            contenido = (
                contenido[: m.start()] + apertura + contenido[m.end():]
            )
            # Reemplaza el cierre original de la grilla por el bloque completo.
            m2 = re.search(rf'(?P<ind>[ \t]*)<DataGrid x:Name="{nombre_grid}" Grid\.Row="0"', contenido)
            cola = contenido[m2.end():]
            fin_grid = cola.find(f'</DataGrid>')
            contenido = (
                contenido[: m2.end()]
                + atributos
                + f'</DataGrid>\n{ind}    <compartido:CtlPaginacion Grid.Row="1" Model="{model}"/>\n{ind}</Grid>'
                + cola[fin_grid + len("</DataGrid>"):]
            )

    ruta.write_text(contenido, encoding="utf-8", newline="")
    print("OK: CtlReportesRecepcion.xaml")


# ----------------------------------------------------------------------------
# CtlMisTrabajos
# ----------------------------------------------------------------------------
def migrar_mis_trabajos() -> None:
    ruta = PANTALLA / "MisTrabajos" / "CtlMisTrabajos.xaml"
    contenido = ruta.read_text(encoding="utf-8")

    contenido = agregar_ns(contenido)
    contenido = contenido.replace('ItemsSource="{Binding OrdenesView}"', 'ItemsSource="{Binding Ordenes}"')

    if 'TextChanged="TxtBusqueda_TextChanged"' not in contenido:
        contenido = re.sub(
            r'(<TextBox Text="\{Binding TextoBusqueda, UpdateSourceTrigger=PropertyChanged\}")(\s*)',
            r'\1\n'
            r'                         TextChanged="TxtBusqueda_TextChanged"\n'
            r'                         KeyDown="TxtBusqueda_KeyDown"\n'
            r'                         ',
            contenido,
            count=1,
        )

    if 'SelectedDateChanged="DtpFechaDesde_SelectedDateChanged"' not in contenido:
        contenido = re.sub(
            r'(<DatePicker SelectedDate="\{Binding FechaDesde, UpdateSourceTrigger=PropertyChanged\}")(\s*)',
            r'\1\n'
            r'                         SelectedDateChanged="DtpFechaDesde_SelectedDateChanged"\n'
            r'                         ',
            contenido,
            count=1,
        )
    if 'SelectedDateChanged="DtpFechaHasta_SelectedDateChanged"' not in contenido:
        contenido = re.sub(
            r'(<DatePicker SelectedDate="\{Binding FechaHasta, UpdateSourceTrigger=PropertyChanged\}")(\s*)',
            r'\1\n'
            r'                         SelectedDateChanged="DtpFechaHasta_SelectedDateChanged"\n'
            r'                         ',
            contenido,
            count=1,
        )
    if 'SelectionChanged="CboFiltroEstado_SelectionChanged"' not in contenido:
        contenido = re.sub(
            r'(<ComboBox Width="130" Height="30" SelectedValue="\{Binding FiltroEstado, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged\}" SelectedValuePath="Content")',
            r'\1 SelectionChanged="CboFiltroEstado_SelectionChanged"',
            contenido,
            count=1,
        )

    # La grilla maestra gana la fila de la barra de paginacion.
    if "<compartido:CtlPaginacion" not in contenido:
        m = re.search(
            r'(?P<ind>[ \t]*)<Grid Grid\.Row="1">\n'
            r'(?P=ind)    <StackPanel VerticalAlignment="Center"',
            contenido,
        )
        if not m:
            raise RuntimeError("No se encontro la Grid de la grilla maestra de MisTrabajos")
        ind = m.group("ind")
        nuevo = (
            f'{ind}<Grid Grid.Row="1">\n'
            f'{ind}    <Grid.RowDefinitions>\n'
            f'{ind}        <RowDefinition Height="*"/>\n'
            f'{ind}        <RowDefinition Height="Auto"/>\n'
            f'{ind}    </Grid.RowDefinitions>\n'
            f'{ind}    <StackPanel VerticalAlignment="Center"'
        )
        contenido = contenido[: m.start()] + nuevo + contenido[m.end():]

        # La DataGrid pasa a la fila 0 y la barra se agrega al cierre de la Grid.
        contenido = re.sub(
            r'(<DataGrid x:Name="dgOrdenes")',
            r'<DataGrid x:Name="dgOrdenes" Grid.Row="0"',
            contenido,
            count=1,
        )

        # Cierre de la Grid que contiene la grilla maestra.
        m2 = re.search(
            r'(?P<ind>[ \t]*)(?P<datagrid_ind>[ \t]*)<DataGrid x:Name="dgOrdenes" Grid\.Row="0"',
            contenido,
        )
        fin = contenido.find(f'{m2.group("datagrid_ind")}</DataGrid>', m2.end())
        if fin == -1:
            raise RuntimeError("No se encontro el cierre de la grilla maestra de MisTrabajos")
        fin += len(f'{m2.group("datagrid_ind")}</DataGrid>')
        # El cierre de la Grid interior sigue despues.
        cierre_grid = f'{m2.group("datagrid_ind")}</Grid>'
        fin2 = contenido.find(cierre_grid, fin)
        if fin2 == -1:
            raise RuntimeError("No se encontro el cierre de la Grid de MisTrabajos")
        nuevo_cierre = (
            f'{m2.group("datagrid_ind")}<compartido:CtlPaginacion Grid.Row="1" Model="{{Binding}}"/>\n'
            f'{cierre_grid}'
        )
        contenido = contenido[:fin2] + nuevo_cierre + contenido[fin2 + len(cierre_grid):]

    ruta.write_text(contenido, encoding="utf-8", newline="")
    print("OK: CtlMisTrabajos.xaml")


def main() -> int:
    migrar_reporte_ordenes()
    migrar_orden_servicio()
    migrar_reportes_recepcion()
    migrar_mis_trabajos()
    return 0


if __name__ == "__main__":
    sys.exit(main())
