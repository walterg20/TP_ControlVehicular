import io

with io.open('bd/11_SP_ReportesMecanico.sql', 'r', encoding='utf-8') as f:
    content = f.read()

content = content.replace('sp_ReporteHistorialVehiculo', 'sp_HistorialClinicoVehiculo')

with io.open('bd/11_SP_ReportesMecanico.sql', 'w', encoding='utf-8') as f:
    f.write(content)