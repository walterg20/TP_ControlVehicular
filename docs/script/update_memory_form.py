# -*- coding: utf-8 -*-
path = 'MEMORY.md'
with open(path, 'r', encoding='utf-8') as f:
    c = f.read()

target = '## Estado actual (verificar con `git`)'
addition = '''## Estado actual (verificar con `git`)
- Rama de trabajo: `feature/fix-form-cliente-vehiculo`. Spec `fix-form-cliente-vehiculo` implementada (build con 0 errores; 147 tests unitarios en verde).
- **Corrección de etiquetas y limpieza de campos en FrmClienteVehiculo (spec `fix-form-cliente-vehiculo`)**:
  - Reorganización de `FrmClienteVehiculo.xaml` con etiquetas individuales arriba de cada campo (DNI, Apellido, Nombre, Teléfono a la izquierda; Patente, Marca, Modelo, Año a la derecha).
  - Eliminación definitiva del campo informativo no persistido `Motor` (`txtMotor`) y de la propiedad correspondiente en `ClienteVehiculoViewModel.cs`.
'''

c = c.replace(target, addition)
with open(path, 'w', encoding='utf-8') as f:
    f.write(c)
print('MEMORY.md updated successfully')
