import io

with io.open('Presentacion/ViewModels/OrdenServicioViewModel.cs', 'r', encoding='utf-8') as f:
    content = f.read()

# Remove private bool EsMecanico
import re
content = re.sub(r'private bool EsMecanico[\s\S]*?\}', '', content)

# Update permissions
content = content.replace(
    'public bool PuedeEliminar => !EsMecanico && OrdenSeleccionada != null;',
    'public new bool PuedeEliminar => TienePermiso("OrdenServicio.Eliminar") && OrdenSeleccionada != null;'
)
content = content.replace(
    'public bool PuedeImprimirRecepcion => !EsMecanico && OrdenSeleccionada != null;',
    'public bool PuedeImprimirRecepcion => TienePermiso("OrdenServicio.Ver") && OrdenSeleccionada != null;'
)
content = content.replace(
    'public bool PuedeVerComprobantePago => !EsMecanico && OrdenSeleccionada?.Estado == "Pagada";',
    'public bool PuedeVerComprobantePago => TienePermiso("OrdenServicio.Ver") && OrdenSeleccionada?.Estado == "Pagada";'
)
# PuedeCrear was replaced by modulo in update_vms.py? Let's check what it has now.
# Wait, let's just make sure we replace anything with EsMecanico

with io.open('Presentacion/ViewModels/OrdenServicioViewModel.cs', 'w', encoding='utf-8') as f:
    f.write(content)