import pathlib

BASE = pathlib.Path(r"E:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular")

p = BASE / "Presentacion" / "Pantalla" / "Reporte" / "CtlReportesRecepcion.xaml"
d = p.read_text(encoding="utf-8")
d = d.replace(
    '<DataGrid x:Name="dgResultadosHistorial" Grid.Row="0" ',
    '<DataGrid x:Name="dgResultadosHistorial" Grid.Row="0" ItemsSource="{Binding ResultadosHistorial}" ',
)
d = d.replace(
    '<DataGrid x:Name="dgResultadosVehiculos" Grid.Row="0" ',
    '<DataGrid x:Name="dgResultadosVehiculos" Grid.Row="0" ItemsSource="{Binding ResultadosVehiculos}" ',
)
p.write_text(d, encoding="utf-8", newline="")
print("ItemsSource restaurados en CtlReportesRecepcion.xaml")
