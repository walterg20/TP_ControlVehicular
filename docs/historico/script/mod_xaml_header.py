import os

path = r'e:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular\Presentacion\Pantalla\OrdenServicio\CtlOrdenServicioForm.xaml'
with open(path, 'r', encoding='utf-8-sig') as f:
    content = f.read()

content = content.replace('Content="Volver"', 'Content="← Volver"')
with open(path, 'w', encoding='utf-8-sig') as f:
    f.write(content)
