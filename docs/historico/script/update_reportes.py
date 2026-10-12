import io

spec_path = 'openspec/specs/reportes/spec.md'

with io.open(spec_path, 'r', encoding='utf-8') as f:
    content = f.read()

# Check the checkboxes
content = content.replace('- [ ] Se puede exportar el reporte operativo', '- [x] Se puede exportar el reporte operativo')
content = content.replace('- [ ] Se puede imprimir el comprobante de recepci', '- [x] Se puede imprimir el comprobante de recepci')

with io.open(spec_path, 'w', encoding='utf-8') as f:
    f.write(content)

# Generate tasks.md
tasks_path = 'openspec/specs/reportes/tasks.md'
tasks_content = '''# Tareas de Implementacin: Reportes

- [x] Tarea 1: Instalar y configurar QuestPDF.
- [x] Tarea 2: Crear ReporteOperativoViewModel con agrupacin de mtricas por mecnico y vehculos.
- [x] Tarea 3: Crear CtlReporteOperativo.xaml con su maquetado.
- [x] Tarea 4: Inyectar dependencias y conectar el botn lateral en el Dashboard.
- [x] Tarea 5: Habilitar emisin de Comprobante de Recepcin en Orden de Servicio.
'''
with io.open(tasks_path, 'w', encoding='utf-8') as f:
    f.write(tasks_content)

print('Updated spec.md and created tasks.md correctly with UTF-8')