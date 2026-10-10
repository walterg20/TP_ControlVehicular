import os
import re

path = r'e:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular\Presentacion\ViewModels\VehiculoViewModel.cs'
with open(path, 'r', encoding='utf-8-sig') as f:
    content = f.read()

target1 = '''            ((ObservableCollection<VehiculoDto>)Vehiculos).CollectionChanged += (s, e) => OnPropertyChanged(nameof(ListadoVehiculosFiltered));'''
repl1 = '''            ((ObservableCollection<VehiculoDto>)Vehiculos).CollectionChanged += (s, e) => {
                OnPropertyChanged(nameof(ListadoVehiculosFiltered));
                OnPropertyChanged(nameof(IsListEmpty));
            };'''
content = content.replace(target1, repl1)

target2 = '''            set { _textoBusqueda = value; OnPropertyChanged(); OnPropertyChanged(nameof(ListadoVehiculosFiltered)); }'''
repl2 = '''            set { _textoBusqueda = value; OnPropertyChanged(); OnPropertyChanged(nameof(ListadoVehiculosFiltered)); OnPropertyChanged(nameof(IsListEmpty)); }'''
content = content.replace(target2, repl2)

target3 = '''                                  || (v.ClienteNombre ?? string.Empty).IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0);'''
repl3 = '''                                  || (v.ClienteNombre ?? string.Empty).IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0);
                                  
        public bool IsListEmpty => !ListadoVehiculosFiltered.Any();'''
content = content.replace(target3, repl3)

with open(path, 'w', encoding='utf-8-sig') as f:
    f.write(content)
