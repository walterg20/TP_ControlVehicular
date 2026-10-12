import io

with io.open('openspec/specs/permisos-avanzados/tasks.md', 'r', encoding='utf-8') as f:
    content = f.read()

content = content.replace('- [ ] Tarea 4:', '- [x] Tarea 4:')
content = content.replace('- [ ] Tarea 5:', '- [x] Tarea 5:')
content = content.replace('status: IMPLEMENTING', 'status: DONE')

with io.open('openspec/specs/permisos-avanzados/tasks.md', 'w', encoding='utf-8') as f:
    f.write(content)