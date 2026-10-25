import os

path = r'e:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular\Presentacion\ViewModels\VehiculoViewModel.cs'
with open(path, 'r', encoding='utf-8-sig') as f:
    content = f.read()

target = '''        public IEnumerable<VehiculoDto> ListadoVehiculosFiltered =>
            string.IsNullOrWhiteSpace(TextoBusqueda)
                ? Vehiculos
                : Vehiculos.Where(vehiculo => (vehiculo.Patente ?? string.Empty).IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0
                                    || (vehiculo.ClienteNombre ?? string.Empty).IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0
                                    || (vehiculo.MarcaNombre ?? string.Empty).IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0
                                    || (vehiculo.ModeloNombre ?? string.Empty).IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0);'''

repl = '''        public IEnumerable<VehiculoDto> ListadoVehiculosFiltered =>
            string.IsNullOrWhiteSpace(TextoBusqueda)
                ? Vehiculos
                : Vehiculos.Where(vehiculo => (vehiculo.Patente ?? string.Empty).IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0
                                    || (vehiculo.ClienteNombre ?? string.Empty).IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0
                                    || (vehiculo.MarcaNombre ?? string.Empty).IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0
                                    || (vehiculo.ModeloNombre ?? string.Empty).IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0);

        public bool IsListEmpty => !ListadoVehiculosFiltered.Any();'''

content = content.replace(target, repl)

with open(path, 'w', encoding='utf-8-sig') as f:
    f.write(content)
