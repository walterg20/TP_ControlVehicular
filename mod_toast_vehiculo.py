import os
import re

path = r'e:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular\Presentacion\Pantalla\Vehiculo\CtlVehiculo.xaml.cs'
with open(path, 'r', encoding='utf-8-sig') as f:
    content = f.read()

target1 = '''            if (ok == true && this.DataContext is VehiculoViewModel vmVehiculo)
            {
                await vmVehiculo.LoadAsync();
            }'''
repl1 = '''            if (ok == true && this.DataContext is VehiculoViewModel vmVehiculo)
            {
                if (Application.Current.MainWindow is MainWindow win) win.MostrarToast("Vehículo guardado con éxito.");
                await vmVehiculo.LoadAsync();
            }'''
content = content.replace(target1, repl1)

target2 = '''                if (ok == true)
                {
                    await vmVehiculo.LoadAsync();
                }'''
repl2 = '''                if (ok == true)
                {
                    if (Application.Current.MainWindow is MainWindow win) win.MostrarToast("Vehículo actualizado con éxito.");
                    await vmVehiculo.LoadAsync();
                }'''
content = content.replace(target2, repl2)

target3 = '''                    FrmConfirmacion.MostrarAviso("Vehículo eliminado correctamente.", "Éxito", Window.GetWindow(this), icono: "✔️");'''
repl3 = '''                    if (Application.Current.MainWindow is MainWindow win) win.MostrarToast("Vehículo eliminado correctamente.");'''
content = content.replace(target3, repl3)

with open(path, 'w', encoding='utf-8-sig') as f:
    f.write(content)
