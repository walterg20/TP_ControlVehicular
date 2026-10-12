import io

with io.open('App.xaml.cs', 'r', encoding='utf-8') as f:
    content = f.read()

content = content.replace(
    'using TP_ControlVehicular.Negocio.Reportes;',
    'using TP_ControlVehicular.Negocio.Reportes;\nusing TP_ControlVehicular.Negocio.Handlers;'
)

with io.open('App.xaml.cs', 'w', encoding='utf-8') as f:
    f.write(content)