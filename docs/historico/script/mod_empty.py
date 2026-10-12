import os
import re

path = r'e:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular\Presentacion\ViewModels\OrdenServicioViewModel.cs'
with open(path, 'r', encoding='utf-8-sig') as f:
    content = f.read()

target1 = '''            ((ObservableCollection<RegistroServicioDto>)Ordenes).CollectionChanged += (s, e) => OnPropertyChanged(nameof(ListadoOrdenesFiltered));'''
repl1 = '''            ((ObservableCollection<RegistroServicioDto>)Ordenes).CollectionChanged += (s, e) => {
                OnPropertyChanged(nameof(ListadoOrdenesFiltered));
                OnPropertyChanged(nameof(IsListEmpty));
            };'''
content = content.replace(target1, repl1)

target2 = '''            set { _textoBusqueda = value; OnPropertyChanged(); OnPropertyChanged(nameof(ListadoOrdenesFiltered)); }'''
repl2 = '''            set { _textoBusqueda = value; OnPropertyChanged(); OnPropertyChanged(nameof(ListadoOrdenesFiltered)); OnPropertyChanged(nameof(IsListEmpty)); }'''
content = content.replace(target2, repl2)

target3 = '''                                  || (o.Estado ?? string.Empty).IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0);'''
repl3 = '''                                  || (o.Estado ?? string.Empty).IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0);
                                  
        public bool IsListEmpty => !ListadoOrdenesFiltered.Any();'''
content = content.replace(target3, repl3)

with open(path, 'w', encoding='utf-8-sig') as f:
    f.write(content)
