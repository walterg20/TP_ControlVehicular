import os

path = r'e:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular\Presentacion\Pantalla\OrdenServicio\CtlOrdenServicioForm.xaml.cs'
with open(path, 'r', encoding='utf-8-sig') as f:
    content = f.read()

target = '''                if (ok)
                {
                    VolverAlListado();
                }'''

repl = '''                if (ok)
                {
                    VolverAlListado();
                    if (Application.Current.MainWindow is MainWindow win)
                    {
                        win.MostrarToast("Orden de servicio guardada con éxito.");
                    }
                }'''

content = content.replace(target, repl)
with open(path, 'w', encoding='utf-8-sig') as f:
    f.write(content)
