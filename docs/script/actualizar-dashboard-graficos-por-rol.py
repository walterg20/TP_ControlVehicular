"""Reemplaza en CtlDashboard.xaml el bloque de la grilla de vehiculos por las tres
zonas de graficos por rol (Administrador / Recepcionista / Mecanico)."""

import pathlib
import re
import sys

BASE = pathlib.Path(r"E:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular")
RUTA = BASE / "Presentacion" / "Pantalla" / "Dashboard" / "CtlDashboard.xaml"

NS_COMPARTIDO = 'xmlns:compartido="clr-namespace:TP_ControlVehicular.Presentacion.Pantalla.Compartido"'

NUEVO_BLOQUE = """        <!-- GRAFICOS SEGUN EL ROL -->
        <Border Grid.Row="2" Background="White" CornerRadius="12" Padding="20" Margin="0,0,0,20">
            <Border.Effect>
                <DropShadowEffect BlurRadius="12" ShadowDepth="2" Direction="270" Color="#000000" Opacity="0.08"/>
            </Border.Effect>

            <Grid>
                <Grid.RowDefinitions>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="*"/>
                </Grid.RowDefinitions>

                <TextBlock Grid.Row="0" Text="{Binding TituloSeccion}" FontSize="16" FontWeight="Bold" Foreground="#2C3E50" Margin="0,0,0,15"/>

                <!-- ADMINISTRADOR: los tres graficos del Reporte Gerencial -->
                <Grid Grid.Row="1" Visibility="{Binding MostrarGraficosGerenciales, Converter={StaticResource BoolToVis}}">
                    <Grid.RowDefinitions>
                        <RowDefinition Height="*"/>
                        <RowDefinition Height="*"/>
                    </Grid.RowDefinitions>
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="*"/>
                        <ColumnDefinition Width="*"/>
                    </Grid.ColumnDefinitions>

                    <compartido:CtlGraficoBarras Grid.Row="0" Grid.Column="0"
                        Serie="{Binding IngresosPorMes}" Titulo="Ingresos por mes" EsMonto="True"
                        Margin="0,0,14,10"/>
                    <compartido:CtlGraficoBarrasHorizontal Grid.Row="0" Grid.Column="1"
                        Serie="{Binding ModelosMasReparados}" Titulo="Modelos mas reparados"
                        Margin="0,0,0,10"/>
                    <compartido:CtlGraficoTorta Grid.Row="1" Grid.Column="0" Grid.ColumnSpan="2"
                        Serie="{Binding ServiciosPorRango}" Titulo="Servicios por cantidad"
                        Margin="0,10,0,0"/>
                </Grid>

                <!-- RECEPCIONISTA: ordenes por estado -->
                <compartido:CtlGraficoBarras Grid.Row="1"
                    Serie="{Binding OrdenesPorEstado}" Titulo="Ordenes por estado"
                    Visibility="{Binding MostrarGraficoOrdenes, Converter={StaticResource BoolToVis}}"/>

                <!-- MECANICO: sus tareas por estado y su avance -->
                <Grid Grid.Row="1" Visibility="{Binding MostrarGraficoMecanico, Converter={StaticResource BoolToVis}}">
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="*"/>
                        <ColumnDefinition Width="240"/>
                    </Grid.ColumnDefinitions>

                    <compartido:CtlGraficoBarras Grid.Column="0"
                        Serie="{Binding TareasPorEstado}" Titulo="Mis tareas por estado"
                        Margin="0,0,18,0"/>
                    <compartido:CtlGraficoAnilloProgreso Grid.Column="1"
                        Porcentaje="{Binding PorcentajeAvance}" Titulo="Avance de mis tareas"/>
                </Grid>
            </Grid>
        </Border>
"""


def main() -> int:
    d = RUTA.read_text(encoding="utf-8")

    if NS_COMPARTIDO not in d:
        d = re.sub(
            r'(xmlns:d="http://schemas\.microsoft\.com/expression/blend/2008"[^\n]*\n)',
            r'\1             ' + NS_COMPARTIDO + '\n',
            d,
            count=1,
        )

    if "CtlGraficoBarras" in d:
        print("El dashboard ya tiene los graficos por rol.")
        return 0

    # Se reemplaza el Border de la fila 2 completo (el de la grilla de vehiculos).
    inicio = d.find("        <!-- LISTADO DE VEH")
    if inicio == -1:
        print("ERROR: no se encontro el bloque de la grilla de vehiculos")
        return 1

    fin = d.find("        </Border>", inicio)
    if fin == -1:
        print("ERROR: no se encontro el cierre del bloque")
        return 1
    fin += len("        </Border>\n")

    d = d[:inicio] + NUEVO_BLOQUE + d[fin:]

    RUTA.write_text(d, encoding="utf-8", newline="")
    print("OK: CtlDashboard.xaml actualizado con los graficos por rol")
    return 0


if __name__ == "__main__":
    sys.exit(main())
