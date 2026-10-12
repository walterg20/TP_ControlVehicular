# -*- coding: utf-8 -*-
import re

with open('docs/DER.md', 'r', encoding='utf-8') as f:
    text = f.read()

vehiculo_section = r'\* \*\*VEHICULO\*\*[\s\S]*?(?=---)'
new_entities = '''* **PROPIETARIO_VEHICULO**
  * id_propietario_vehiculo (int, PK): Identificador único de la asignación.
  * id_cliente (int, FK): Cliente propietario (CLIENTE.id_cliente).
  * id_vehiculo (int, FK): Vehículo adquirido (VEHICULO.id_vehiculo).
  * echa_adquisicion (date): Fecha en que adquiere la unidad.
  * echa_venta (date, null): Fecha en que se vendió (null si es el dueño actual).
  * es_actual (bool): Bandera para facilitar la búsqueda del dueño activo.

* **VEHICULO**
  * id_vehiculo (int, PK): Identificador único de la unidad.
  * id_modelo (int, FK): Modelo (MODELO.id_modelo).
  * nio (int): Año de fabricación.
  * patente (string): Dominio/patente del vehículo.
  * km_actual (int): Kilometraje acumulado al último registro.

'''
text = re.sub(vehiculo_section, new_entities, text)

row1 = r'\| CLIENTE \| Es due.*?\|'
row2 = r'\| VEHICULO \| Ingresa.*?\|'
# just replace the table block entirely to be safe
table_regex = r'\| CLIENTE \| Es due[\s\S]*?(?=\| TALLER)'
new_table_part = '''| CLIENTE | Posee | PROPIETARIO_VEHICULO | 1 a N | Un cliente puede tener registrados varios vínculos de posesión. |
| VEHICULO | Pertenece a | PROPIETARIO_VEHICULO | 1 a N | Un vehículo puede tener un historial de varios dueños en el tiempo. |
| VEHICULO | Ingresa a | REGISTRO_SERVICIO | 1 a N | Un vehículo genera múltiples órdenes de servicio en el tiempo. |
'''
text = re.sub(table_regex, new_table_part, text)

# Just append to the end of the document
text = text + '\n4. **Posesión Independiente:** Un vehículo puede existir en el sistema sin un cliente asignado inicialmente. La relación de propiedad se registra en la tabla PROPIETARIO_VEHICULO para conservar el historial de dueños frente a la venta de unidades.\n'

with open('docs/DER.md', 'w', encoding='utf-8') as f:
    f.write(text)

