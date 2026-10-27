import io
import os

files_to_fix = [
    'Presentacion/Pantalla/MisTrabajos/CtlMisTrabajos.xaml',
    'Presentacion/Pantalla/Vehiculo/CtlVehiculo.xaml',
    'Presentacion/Pantalla/Cliente/CtlCliente.xaml',
    'Presentacion/Pantalla/Servicio/CtlServicio.xaml',
    'Presentacion/Pantalla/Taller/CtlTaller.xaml',
    'Presentacion/Pantalla/Rol/CtlRol.xaml',
]

resource_string = '<UserControl.Resources>\n        <BooleanToVisibilityConverter x:Key="BoolToVis"/>\n    </UserControl.Resources>\n'

for path in files_to_fix:
    if os.path.exists(path):
        with io.open(path, 'r', encoding='utf-8') as f:
            content = f.read()
            
        if 'BoolToVis' in content and '<BooleanToVisibilityConverter' not in content:
            # insert after <Grid Margin="10"> or whatever
            # actually better to put it right before <Grid>
            if '<Grid' in content:
                parts = content.split('<Grid', 1)
                new_content = parts[0] + resource_string + '    <Grid' + parts[1]
                with io.open(path, 'w', encoding='utf-8') as f:
                    f.write(new_content)