import io

with io.open('Presentacion/MainWindow.xaml.cs', 'r', encoding='utf-8') as f:
    content = f.read()

content = content.replace(
    'btnMisTrabajos.Visibility = (permisos.Contains("OrdenServicio.Ver") || UsuarioSesionActual.IdRol == 3) ? Visibility.Visible : Visibility.Collapsed;',
    'btnMisTrabajos.Visibility = permisos.Contains("MisTrabajos.Ver") ? Visibility.Visible : Visibility.Collapsed;'
)

content = content.replace(
    '// Especial para mecánicos u otros que ven órdenes',
    ''
)

with io.open('Presentacion/MainWindow.xaml.cs', 'w', encoding='utf-8') as f:
    f.write(content)