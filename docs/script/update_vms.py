import io
import os

viewmodels = {
    'ClienteViewModel.cs': 'Cliente',
    'RolViewModel.cs': 'Usuario', # or Rol?
    'UsuarioViewModel.cs': 'Usuario',
    'TallerViewModel.cs': 'Taller',
    'ServicioViewModel.cs': 'Servicio',
    'ModeloViewModel.cs': 'Vehiculo',
    'MarcaViewModel.cs': 'Vehiculo',
}

for vm, modulo in viewmodels.items():
    path = os.path.join('Presentacion/ViewModels', vm)
    if os.path.exists(path):
        with io.open(path, 'r', encoding='utf-8') as f:
            content = f.read()
            
        if 'protected override string Modulo =>' not in content:
            content = content.replace(
                'public class ' + vm.replace('.cs', '') + ' : BaseViewModel\n    {',
                'public class ' + vm.replace('.cs', '') + ' : BaseViewModel\n    {\n        protected override string Modulo => "' + modulo + '";\n'
            )
            with io.open(path, 'w', encoding='utf-8') as f:
                f.write(content)

# Special cases
# OrdenServicioViewModel (already has PuedeCrear, PuedeEliminar)
orden_path = 'Presentacion/ViewModels/OrdenServicioViewModel.cs'
if os.path.exists(orden_path):
    with io.open(orden_path, 'r', encoding='utf-8') as f:
        o_content = f.read()
    if 'protected override string Modulo =>' not in o_content:
        o_content = o_content.replace(
            'public bool PuedeCrear => !EsMecanico;',
            'protected override string Modulo => "OrdenServicio";'
        )
        o_content = o_content.replace(
            'public bool PuedeEliminar => (System.Windows.Application.Current.MainWindow as MainWindow)?.UsuarioSesionActual?.IdRol != 3;',
            ''
        )
        with io.open(orden_path, 'w', encoding='utf-8') as f:
            f.write(o_content)
