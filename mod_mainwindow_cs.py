import os

path = r'e:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular\Presentacion\MainWindow.xaml.cs'
with open(path, 'r', encoding='utf-8-sig') as f:
    content = f.read()

target = '''        private void ConfigurarSeguridadPorRol()'''

repl = '''        public async void MostrarToast(string mensaje)
        {
            lblToastMessage.Text = mensaje;
            bdrToast.Visibility = Visibility.Visible;
            await Task.Delay(3000);
            bdrToast.Visibility = Visibility.Collapsed;
        }

        private void ConfigurarSeguridadPorRol()'''

content = content.replace(target, repl)
with open(path, 'w', encoding='utf-8-sig') as f:
    f.write(content)
