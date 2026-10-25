import os

path = r'e:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular\Presentacion\Pantalla\OrdenServicio\CtlOrdenServicio.xaml.cs'
with open(path, 'r', encoding='utf-8-sig') as f:
    content = f.read()

target = '''        private void BtnNuevo_Click(object sender, RoutedEventArgs e)'''
repl = '''        private async void BtnRefrescar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OrdenServicioViewModel vm)
            {
                await vm.LoadAsync();
            }
        }

        private void BtnNuevo_Click(object sender, RoutedEventArgs e)'''

if "BtnRefrescar_Click" not in content:
    content = content.replace(target, repl)
    
with open(path, 'w', encoding='utf-8-sig') as f:
    f.write(content)
