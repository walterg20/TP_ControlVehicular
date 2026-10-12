import io

with io.open('App.xaml.cs', 'r', encoding='utf-8') as f:
    content = f.read()

content = content.replace(
    'services.AddScoped<AutenticarUsuarioHandler>();',
    'services.AddScoped<AutenticarUsuarioHandler>();\n            services.AddScoped<ObtenerPermisosHandler>();\n            services.AddScoped<ActualizarPermisosRolHandler>();'
)

with io.open('App.xaml.cs', 'w', encoding='utf-8') as f:
    f.write(content)