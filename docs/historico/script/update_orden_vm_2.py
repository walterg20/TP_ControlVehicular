import io
import re

with io.open('Presentacion/ViewModels/OrdenServicioViewModel.cs', 'r', encoding='utf-8') as f:
    content = f.read()

replacement = '''
        private bool EsMecanico
        {
            get { return !TienePermiso("OrdenServicio.Crear"); }
        }
        public bool EsRecepcionista
        {
            get { return TienePermiso("OrdenServicio.Crear"); }
        }
'''

# Replace the block from private bool EsMecanico down to end of EsRecepcionista
content = re.sub(r'private bool EsMecanico[\s\S]*?return !EsMecanico;\s*\}\s*\}', replacement, content)

content = content.replace(
    'public bool PuedeEliminar => !EsMecanico && OrdenSeleccionada != null;',
    'public new bool PuedeEliminar => TienePermiso("OrdenServicio.Eliminar") && OrdenSeleccionada != null;'
)

with io.open('Presentacion/ViewModels/OrdenServicioViewModel.cs', 'w', encoding='utf-8') as f:
    f.write(content)