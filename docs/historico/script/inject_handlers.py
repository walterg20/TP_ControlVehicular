import io

with io.open('App.xaml.cs', 'r', encoding='utf-8') as f:
    content = f.read()

injection = '''
            services.AddScoped<ObtenerHistorialClinicoVehiculoHandler>();
            services.AddScoped<ObtenerHojaTrabajoDiariaHandler>();
'''
content = content.replace('services.AddScoped<ObtenerReporteTiemposResolucionHandler>();', 'services.AddScoped<ObtenerReporteTiemposResolucionHandler>();' + injection)

with io.open('App.xaml.cs', 'w', encoding='utf-8') as f:
    f.write(content)