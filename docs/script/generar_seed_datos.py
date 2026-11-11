# -*- coding: utf-8 -*-
"""
Genera el bloque SQL canónico e idempotente para poblar en 02_datos.sql:
- Usuarios del equipo (Recepcionista y Mecánicos)
- 55 Clientes
- 55 Vehículos
- 55 Propietarios de Vehículos
- 60 Órdenes de Servicio (RegistroServicio) desde Junio 2026
- 110+ Detalles de Servicio (DetalleServicio)
- Facturas y Pagos correspondientes a las órdenes pagadas
"""

import sys

clientes_data = [
    ("Juan", "Pérez", "28341901", "1981-03-15", "Av. Castelli 1240", "juan.perez@gmail.com", "3624112233"),
    ("María", "González", "30452812", "1983-07-22", "Calle Alvear 450", "maria.gonzalez@hotmail.com", "3624223344"),
    ("Carlos", "Rodríguez", "25619304", "1977-11-04", "Av. 9 de Julio 1820", "carlos.rodriguez@yahoo.com.ar", "3624334455"),
    ("Ana", "Martínez", "33789451", "1988-02-18", "Santa María de Oro 320", "ana.martinez@gmail.com", "3624445566"),
    ("Lucas", "López", "35123984", "1990-09-30", "Av. Italia 890", "lucas.lopez@outlook.com", "3624556677"),
    ("Florencia", "Gómez", "32876123", "1987-05-12", "Julio A. Roca 215", "flor.gomez@gmail.com", "3624667788"),
    ("Matías", "Díaz", "29431876", "1982-12-08", "Av. Sarmiento 1540", "matias.diaz@gmail.com", "3624778899"),
    ("Sofía", "Alvarez", "36987452", "1992-04-25", "Marcelo T. de Alvear 620", "sofia.alvarez@gmail.com", "3624889900"),
    ("Martín", "Romero", "27845129", "1980-08-14", "Av. Belgrano 1120", "martin.romero@hotmail.com", "3624990011"),
    ("Camila", "Sosa", "38124963", "1994-01-19", "Pueyrredón 780", "camila.sosa@gmail.com", "3624001122"),
    ("Nicolás", "Torres", "34562187", "1989-10-03", "Av. San Martín 1350", "nicolas.torres@outlook.com", "3624123456"),
    ("Valentina", "Ruiz", "37458912", "1993-06-28", "Obligado 340", "valentina.ruiz@gmail.com", "3624234567"),
    ("Gonzalo", "Benítez", "26987413", "1978-03-09", "Av. Las Heras 920", "gonzalo.benitez@yahoo.com.ar", "3624345678"),
    ("Lucía", "Flores", "39125874", "1995-11-17", "Güemes 560", "lucia.flores@gmail.com", "3624456789"),
    ("Tomás", "Acosta", "31784523", "1985-08-21", "Av. Mac Lean 1740", "tomas.acosta@gmail.com", "3624567890"),
    ("Julieta", "Medina", "35896321", "1991-02-14", "Don Bosco 430", "julieta.medina@gmail.com", "3624678901"),
    ("Diego", "Herrera", "24896325", "1975-09-05", "Av. Hernandarias 850", "diego.herrera@hotmail.com", "3624789012"),
    ("Paula", "Aguirre", "33458962", "1988-04-11", "Vedia 290", "paula.aguirre@gmail.com", "3624890123"),
    ("Agustín", "Pereira", "36125478", "1991-12-02", "Av. 25 de Mayo 1680", "agustin.pereira@outlook.com", "3624901234"),
    ("Micaela", "Castro", "38745219", "1994-07-07", "Brown 610", "micaela.castro@gmail.com", "3624012345"),
    ("Federico", "Giménez", "28965412", "1981-10-29", "Av. Alvear 1920", "fede.gimenez@gmail.com", "3624113355"),
    ("Daniela", "Silva", "31258964", "1985-05-16", "Mitre 480", "daniela.silva@hotmail.com", "3624224466"),
    ("Gabriel", "Morales", "27458963", "1979-01-23", "Av. Chaco 1140", "gabriel.morales@yahoo.com.ar", "3624335577"),
    ("Victoria", "Cabrera", "37896541", "1993-09-12", "Yrigoyen 730", "victoria.cabrera@gmail.com", "3624446688"),
    ("Esteban", "Vega", "30125896", "1983-06-04", "Av. Rivadavia 1410", "esteban.vega@gmail.com", "3624557799"),
    ("Mariana", "Ríos", "34896521", "1989-11-20", "Liniers 350", "mariana.rios@outlook.com", "3624668800"),
    ("Facundo", "Duarte", "36541289", "1992-03-31", "Av. Edison 1830", "facundo.duarte@gmail.com", "3624779911"),
    ("Antonella", "Molina", "39458712", "1996-08-15", "Salta 520", "antonella.molina@gmail.com", "3624880022"),
    ("Rodrigo", "Ortiz", "29874512", "1982-04-09", "Av. Avalos 960", "rodrigo.ortiz@hotmail.com", "3624991133"),
    ("Belén", "Rivas", "35412896", "1990-10-24", "Frondizi 280", "belen.rivas@gmail.com", "3624002244"),
    ("Cristian", "Cardozo", "32145896", "1986-07-18", "Av. Lavalle 1520", "cristian.cardozo@gmail.com", "3624114477"),
    ("Natalia", "Núñez", "26541289", "1978-12-05", "Perón 410", "natalia.nunez@yahoo.com.ar", "3624225588"),
    ("Ignacio", "Vera", "38451296", "1994-02-27", "Av. Paraguay 1210", "ignacio.vera@outlook.com", "3624336699"),
    ("Melina", "Luna", "33985412", "1988-09-08", "Corrientes 640", "melina.luna@gmail.com", "3624447700"),
    ("Lautaro", "Juárez", "37125896", "1993-01-14", "Av. Vélez Sarsfield 1650", "lautaro.juarez@gmail.com", "3624558811"),
    ("Daiana", "Maidana", "34125896", "1989-06-19", "Entre Ríos 390", "daiana.maidana@hotmail.com", "3624669922"),
    ("Maximiliano", "Godoy", "28451296", "1981-08-30", "Av. Moreno 1180", "maxi.godoy@gmail.com", "3624770033"),
    ("Sabrina", "Barrientos", "36854129", "1992-11-11", "Jujuy 510", "sabrina.barrientos@gmail.com", "3624881144"),
    ("Leandro", "Ferreyra", "30985412", "1984-05-03", "Av. Wilde 1430", "leandro.ferreyra@outlook.com", "3624992255"),
    ("Rocío", "Ibarra", "39854126", "1996-03-22", "Catamarca 270", "rocio.ibarra@gmail.com", "3624003366"),
    ("Santiago", "Coronel", "27125896", "1979-10-15", "Av. Sabin 1790", "santiago.coronel@gmail.com", "3624115588"),
    ("Romina", "Leguizamón", "35985412", "1991-07-09", "La Rioja 460", "romina.leguizamon@yahoo.com.ar", "3624226699"),
    ("Damián", "Ojeda", "32985412", "1987-04-01", "Av. Martina 1250", "damian.ojeda@gmail.com", "3624337700"),
    ("Yamila", "Mansilla", "37985412", "1993-12-26", "San Juan 580", "yamila.mansilla@hotmail.com", "3624448811"),
    ("Franco", "Galeano", "31985412", "1986-02-07", "Av. Alvear 2100", "franco.galeano@gmail.com", "3624559922"),
    ("Gisela", "Valdez", "34985412", "1990-08-16", "Mendoza 310", "gisela.valdez@outlook.com", "3624660033"),
    ("Hernán", "Escalante", "29125896", "1982-11-28", "Av. Castelli 1890", "hernan.escalante@gmail.com", "3624771144"),
    ("Noelia", "Cardozo", "36451298", "1992-06-10", "Posadas 490", "noelia.cardozo@gmail.com", "3624882255"),
    ("Bruno", "Peralta", "38125896", "1994-09-04", "Av. 9 de Julio 2450", "bruno.peralta@gmail.com", "3624993366"),
    ("Andrea", "Bogado", "33125896", "1987-01-21", "Necochea 370", "andrea.bogado@yahoo.com.ar", "3624004477"),
    ("Sebastián", "Villalba", "30784512", "1984-08-17", "Av. Belgrano 1650", "sebastian.villalba@gmail.com", "3624116699"),
    ("Lorena", "Aquino", "35784512", "1991-04-12", "Arturo Frondizi 820", "lorena.aquino@hotmail.com", "3624227700"),
    ("Emiliano", "Gauto", "27985412", "1980-11-06", "Av. Italia 1490", "emiliano.gauto@gmail.com", "3624338811"),
    ("Tamara", "Salinas", "37412589", "1993-05-29", "Ameghino 540", "tamara.salinas@outlook.com", "3624449922"),
    ("Joaquín", "Paz", "39145896", "1995-10-14", "Av. San Martín 1980", "joaquin.paz@gmail.com", "3624550033")
]

vehiculos_data = [
    ("Toyota", "Corolla", 2019, "AD123AA", 54000),
    ("Toyota", "Hilux", 2021, "AE456BB", 78000),
    ("Toyota", "Yaris", 2020, "AD789CC", 41000),
    ("Toyota", "Etios", 2018, "AC321DD", 68000),
    ("Toyota", "Corolla Cross", 2022, "AF654EE", 32000),
    ("Volkswagen", "Gol", 2017, "AB987FF", 89000),
    ("Volkswagen", "Amarok", 2020, "AD159GG", 92000),
    ("Volkswagen", "Polo", 2021, "AE357HH", 37000),
    ("Volkswagen", "Saveiro", 2016, "AA753II", 115000),
    ("Volkswagen", "T-Cross", 2022, "AF951JJ", 28000),
    ("Ford", "Ranger", 2019, "AD852KK", 84000),
    ("Ford", "Focus", 2018, "AC147LL", 73000),
    ("Ford", "Fiesta", 2017, "AB258MM", 81000),
    ("Ford", "Ka", 2019, "AD369NN", 59000),
    ("Ford", "EcoSport", 2016, "AA963OO", 102000),
    ("Chevrolet", "Cruze", 2020, "AD741PP", 49000),
    ("Chevrolet", "Onix", 2021, "AE852QQ", 35000),
    ("Chevrolet", "Tracker", 2022, "AF963RR", 26000),
    ("Chevrolet", "S10", 2018, "AC159SS", 98000),
    ("Chevrolet", "Spin", 2017, "AB357TT", 87000),
    ("Fiat", "Cronos", 2022, "AF753UU", 31000),
    ("Fiat", "Toro", 2020, "AD951VV", 64000),
    ("Fiat", "Strada", 2021, "AE159WW", 47000),
    ("Fiat", "Argo", 2019, "AD357XX", 58000),
    ("Fiat", "Mobi", 2018, "AC753YY", 62000),
    ("Peugeot", "208", 2022, "AF159ZZ", 29000),
    ("Peugeot", "2008", 2020, "AD357AB", 51000),
    ("Peugeot", "Partner", 2017, "AB753CD", 108000),
    ("Peugeot", "308", 2016, "AA951EF", 94000),
    ("Renault", "Sandero", 2019, "AD159GH", 63000),
    ("Renault", "Kangoo", 2021, "AE357IJ", 44000),
    ("Renault", "Duster", 2018, "AC753KL", 79000),
    ("Renault", "Logan", 2017, "AB951MN", 86000),
    ("Renault", "Stepway", 2020, "AD159OP", 52000),
    ("Renault", "Kwid", 2019, "AD357QR", 48000),
    ("Nissan", "Frontier", 2021, "AE753ST", 67000),
    ("Nissan", "Kicks", 2020, "AD951UV", 43000),
    ("Nissan", "Versa", 2022, "AF159WX", 25000),
    ("Honda", "Civic", 2018, "AC357YZ", 69000),
    ("Honda", "HR-V", 2021, "AE753AA", 38000),
    ("Citroën", "C3", 2022, "AF951BB", 22000),
    ("Citroën", "C4 Cactus", 2020, "AD159CC", 53000),
    ("Citroën", "Berlingo", 2018, "AC357DD", 91000),
    ("Jeep", "Renegade", 2021, "AE753EE", 39000),
    ("Jeep", "Compass", 2020, "AD951FF", 56000),
    ("Toyota", "Corolla", 2021, "AE159GG", 42000),
    ("Toyota", "Hilux", 2019, "AD357HH", 88000),
    ("Volkswagen", "Gol", 2018, "AC753II", 76000),
    ("Fiat", "Cronos", 2021, "AE951JJ", 36000),
    ("Ford", "Ranger", 2022, "AF159KK", 31000),
    ("Peugeot", "208", 2021, "AE357LL", 33000),
    ("Chevrolet", "Cruze", 2019, "AD753MM", 61000),
    ("Renault", "Kangoo", 2020, "AD951NN", 54000),
    ("Volkswagen", "Amarok", 2022, "AF159OO", 35000),
    ("Toyota", "Yaris", 2022, "AF357PP", 24000)
]

# Fechas y configuraciones de 60 órdenes de servicio distribuidas desde Junio 2026 a Octubre 2026
# (VehiculoIdx, FechaStr, KmOffset, Estado, [(ServicioNombre, Cantidad, Precio, MecanicoEmail, DetalleEstado, Obs), ...])
ordenes_def = [
    # --- JUNIO 2026 (12 ordenes) ---
    (0, "2026-06-02 08:30:00", 0, "Pagada", [
        ("Cambio de Aceite de Motor", 1, 65000.00, "carlos.mecanico@taller.com", "Finalizada", "Aceite sintético 5W-30"),
        ("Cambio de Filtro de Aceite", 1, 34000.00, "carlos.mecanico@taller.com", "Finalizada", "Filtro original"),
        ("Cambio de Filtro Aire", 1, 25000.00, "carlos.mecanico@taller.com", "Finalizada", "Reemplazo de elemento filtrante")
    ]),
    (1, "2026-06-04 09:15:00", 0, "Pagada", [
        ("Alineación y Balanceo", 1, 38000.00, "marcos.mecanico@taller.com", "Finalizada", "Control en las 4 ruedas"),
        ("Rotación de Neumáticos", 1, 22000.00, "marcos.mecanico@taller.com", "Finalizada", "Ajuste de presión según manual")
    ]),
    (2, "2026-06-07 10:00:00", 0, "Pagada", [
        ("Cambio de Pastillas de Freno", 1, 45000.00, "lucas.mecanico@taller.com", "Finalizada", "Pastillas delanteras reemplazadas"),
        ("Cambio de Líquido de Frenos", 1, 30000.00, "lucas.mecanico@taller.com", "Finalizada", "Purga de circuito completo")
    ]),
    (3, "2026-06-09 11:20:00", 0, "Pagada", [
        ("Cambio de Batería", 1, 95000.00, "carlos.mecanico@taller.com", "Finalizada", "Batería Moura 12V 65Ah"),
        ("Diagnóstico Computarizado OBD2", 1, 20000.00, "carlos.mecanico@taller.com", "Finalizada", "Borrado de errores de baja tensión")
    ]),
    (4, "2026-06-12 14:00:00", 0, "Pagada", [
        ("Cambio de Bujías", 4, 7000.00, "marcos.mecanico@taller.com", "Finalizada", "Bujías NGK Iridium"),
        ("Limpieza de Inyectores", 1, 48000.00, "marcos.mecanico@taller.com", "Finalizada", "Limpieza ultrasónica en banco")
    ]),
    (5, "2026-06-15 08:45:00", 0, "Pagada", [
        ("Cambio de Aceite de Motor", 1, 65000.00, "lucas.mecanico@taller.com", "Finalizada", "Servicio preventivo"),
        ("Cambio de Filtro de Aceite", 1, 34000.00, "lucas.mecanico@taller.com", "Finalizada", "Filtro sustituido")
    ]),
    (6, "2026-06-18 10:30:00", 0, "Pagada", [
        ("Recarga de Aire Acondicionado", 1, 55000.00, "carlos.mecanico@taller.com", "Finalizada", "Prueba de vacío y recarga R134a")
    ]),
    (7, "2026-06-21 15:10:00", 0, "Pagada", [
        ("Cambio de Correa de Distribucion", 1, 15000.00, "marcos.mecanico@taller.com", "Finalizada", "Kit distribución y bomba de agua"),
        ("Servicio Completo", 1, 120000.00, "marcos.mecanico@taller.com", "Finalizada", "Inspección de 25 puntos de seguridad")
    ]),
    (8, "2026-06-24 09:00:00", 0, "Pagada", [
        ("Alineación y Balanceo", 1, 38000.00, "lucas.mecanico@taller.com", "Finalizada", "Corrección de comba delantera")
    ]),
    (9, "2026-06-26 11:40:00", 0, "Pagada", [
        ("Cambio de Pastillas de Freno", 1, 45000.00, "carlos.mecanico@taller.com", "Finalizada", "Discos en buen estado"),
        ("Escaneo General", 1, 25000.00, "carlos.mecanico@taller.com", "Finalizada", "Sin fallas en centralita")
    ]),
    (10, "2026-06-28 16:00:00", 0, "Pagada", [
        ("Cambio de Aceite de Motor", 1, 65000.00, "marcos.mecanico@taller.com", "Finalizada", "Aceite semi-sintético 10W-40"),
        ("Cambio de Filtro Aire", 1, 25000.00, "marcos.mecanico@taller.com", "Finalizada", "Filtro nuevo colocado")
    ]),
    (11, "2026-06-30 17:15:00", 0, "Pagada", [
        ("Limpieza de Inyectores", 1, 48000.00, "lucas.mecanico@taller.com", "Finalizada", "Mejora de consumo y ralentí")
    ]),

    # --- JULIO 2026 (12 ordenes) ---
    (12, "2026-07-02 08:30:00", 0, "Pagada", [
        ("Cambio de Aceite de Motor", 1, 65000.00, "carlos.mecanico@taller.com", "Finalizada", "Aceite 5W-30 sintético"),
        ("Cambio de Filtro de Aceite", 1, 34000.00, "carlos.mecanico@taller.com", "Finalizada", "Filtro reemplazado"),
        ("Alineación y Balanceo", 1, 38000.00, "marcos.mecanico@taller.com", "Finalizada", "Balanceo dinámico")
    ]),
    (13, "2026-07-05 10:15:00", 0, "Pagada", [
        ("Cambio de Bujías", 4, 7000.00, "lucas.mecanico@taller.com", "Finalizada", "Juego de bujías nuevas"),
        ("Diagnóstico Computarizado OBD2", 1, 20000.00, "lucas.mecanico@taller.com", "Finalizada", "Sensor MAF chequeado")
    ]),
    (14, "2026-07-08 14:00:00", 0, "Pagada", [
        ("Cambio de Pastillas de Freno", 1, 45000.00, "carlos.mecanico@taller.com", "Finalizada", "Pastillas cerámicas"),
        ("Cambio de Líquido de Frenos", 1, 30000.00, "carlos.mecanico@taller.com", "Finalizada", "Líquido DOT 4")
    ]),
    (15, "2026-07-11 09:20:00", 0, "Pagada", [
        ("Recarga de Aire Acondicionado", 1, 55000.00, "marcos.mecanico@taller.com", "Finalizada", "Filtro de polen sustituido")
    ]),
    (16, "2026-07-14 11:30:00", 0, "Pagada", [
        ("Cambio de Batería", 1, 95000.00, "lucas.mecanico@taller.com", "Finalizada", "Batería Willard 12V 75Ah")
    ]),
    (17, "2026-07-17 15:45:00", 0, "Pagada", [
        ("Cambio de Aceite de Motor", 1, 65000.00, "carlos.mecanico@taller.com", "Finalizada", "Mantenimiento periódico"),
        ("Cambio de Filtro Aire", 1, 25000.00, "carlos.mecanico@taller.com", "Finalizada", "Caja filtrera higienizada")
    ]),
    (18, "2026-07-20 08:50:00", 0, "Pagada", [
        ("Servicio Completo", 1, 120000.00, "marcos.mecanico@taller.com", "Finalizada", "Revisión preventiva antes de viaje")
    ]),
    (19, "2026-07-22 10:40:00", 0, "Pagada", [
        ("Alineación y Balanceo", 1, 38000.00, "lucas.mecanico@taller.com", "Finalizada", "Alineado láser"),
        ("Rotación de Neumáticos", 1, 22000.00, "lucas.mecanico@taller.com", "Finalizada", "Desgaste uniforme")
    ]),
    (20, "2026-07-25 12:10:00", 0, "Pagada", [
        ("Limpieza de Inyectores", 1, 48000.00, "carlos.mecanico@taller.com", "Finalizada", "Desarme y reemplazo de o-rings")
    ]),
    (21, "2026-07-27 14:30:00", 0, "Pagada", [
        ("Cambio de Pastillas de Freno", 1, 45000.00, "marcos.mecanico@taller.com", "Finalizada", "Revisión de campanas traseras")
    ]),
    (22, "2026-07-29 16:20:00", 0, "Pagada", [
        ("Cambio de Correa de Distribucion", 1, 15000.00, "lucas.mecanico@taller.com", "Finalizada", "Tensor y rodillo guía nuevos")
    ]),
    (23, "2026-07-31 17:50:00", 0, "Pagada", [
        ("Diagnóstico Computarizado OBD2", 1, 20000.00, "carlos.mecanico@taller.com", "Finalizada", "Luz de Check Engine apagada")
    ]),

    # --- AGOSTO 2026 (12 ordenes) ---
    (24, "2026-08-03 08:30:00", 0, "Pagada", [
        ("Cambio de Aceite de Motor", 1, 65000.00, "marcos.mecanico@taller.com", "Finalizada", "Aceite Castrol 5W-40"),
        ("Cambio de Filtro de Aceite", 1, 34000.00, "marcos.mecanico@taller.com", "Finalizada", "Filtro Mann")
    ]),
    (25, "2026-08-06 09:40:00", 0, "Pagada", [
        ("Alineación y Balanceo", 1, 38000.00, "lucas.mecanico@taller.com", "Finalizada", "Calibración de tren delantero")
    ]),
    (26, "2026-08-09 11:15:00", 0, "Pagada", [
        ("Cambio de Pastillas de Freno", 1, 45000.00, "carlos.mecanico@taller.com", "Finalizada", "Cambio de líquido DOT 4"),
        ("Cambio de Líquido de Frenos", 1, 30000.00, "carlos.mecanico@taller.com", "Finalizada", "Sin pérdidas en caliper")
    ]),
    (27, "2026-08-12 14:00:00", 0, "Pagada", [
        ("Cambio de Batería", 1, 95000.00, "marcos.mecanico@taller.com", "Finalizada", "Revisión de alternador OK")
    ]),
    (28, "2026-08-14 15:30:00", 0, "Pagada", [
        ("Recarga de Aire Acondicionado", 1, 55000.00, "lucas.mecanico@taller.com", "Finalizada", "Compresor verificado"),
        ("Cambio de Filtro Aire", 1, 25000.00, "lucas.mecanico@taller.com", "Finalizada", "Habitáculo desinfectado")
    ]),
    (29, "2026-08-17 09:10:00", 0, "Pagada", [
        ("Limpieza de Inyectores", 1, 48000.00, "carlos.mecanico@taller.com", "Finalizada", "Caudal equilibrado")
    ]),
    (30, "2026-08-19 10:45:00", 0, "Pagada", [
        ("Cambio de Bujías", 4, 7000.00, "marcos.mecanico@taller.com", "Finalizada", "Cables de bujía testeados")
    ]),
    (31, "2026-08-22 13:20:00", 0, "Pagada", [
        ("Servicio Completo", 1, 120000.00, "lucas.mecanico@taller.com", "Finalizada", "Control de suspensión y frenos")
    ]),
    (32, "2026-08-25 15:00:00", 0, "Pagada", [
        ("Cambio de Aceite de Motor", 1, 65000.00, "carlos.mecanico@taller.com", "Finalizada", "Aceite Shell Helix"),
        ("Cambio de Filtro de Aceite", 1, 34000.00, "carlos.mecanico@taller.com", "Finalizada", "Arandela de tapón nueva"),
        ("Alineación y Balanceo", 1, 38000.00, "lucas.mecanico@taller.com", "Finalizada", "Alineado integral")
    ]),
    (33, "2026-08-27 16:40:00", 0, "Pagada", [
        ("Escaneo General", 1, 25000.00, "marcos.mecanico@taller.com", "Finalizada", "Diagnóstico de inyección y ABS")
    ]),
    (34, "2026-08-29 11:30:00", 0, "Pagada", [
        ("Cambio de Pastillas de Freno", 1, 45000.00, "lucas.mecanico@taller.com", "Finalizada", "Limpieza de mordazas")
    ]),
    (35, "2026-08-31 17:00:00", 0, "Pagada", [
        ("Rotación de Neumáticos", 1, 22000.00, "carlos.mecanico@taller.com", "Finalizada", "Neumáticos traseros a posición delantera")
    ]),

    # --- SEPTIEMBRE 2026 (12 ordenes) ---
    (36, "2026-09-02 08:30:00", 0, "Pagada", [
        ("Cambio de Aceite de Motor", 1, 65000.00, "marcos.mecanico@taller.com", "Finalizada", "Mantenimiento programado"),
        ("Cambio de Filtro Aire", 1, 25000.00, "marcos.mecanico@taller.com", "Finalizada", "Filtro aire motor")
    ]),
    (37, "2026-09-04 10:15:00", 0, "Pagada", [
        ("Alineación y Balanceo", 1, 38000.00, "lucas.mecanico@taller.com", "Finalizada", "Corrección de desvío a la derecha")
    ]),
    (38, "2026-09-07 14:20:00", 0, "Pagada", [
        ("Cambio de Pastillas de Freno", 1, 45000.00, "carlos.mecanico@taller.com", "Finalizada", "Pastillas Fras-le")
    ]),
    (39, "2026-09-10 09:00:00", 0, "Pagada", [
        ("Cambio de Batería", 1, 95000.00, "marcos.mecanico@taller.com", "Finalizada", "Prueba de carga del alternador exitosa")
    ]),
    (40, "2026-09-13 11:45:00", 0, "Pagada", [
        ("Recarga de Aire Acondicionado", 1, 55000.00, "lucas.mecanico@taller.com", "Finalizada", "Control estanqueidad")
    ]),
    (41, "2026-09-16 15:10:00", 0, "Pagada", [
        ("Cambio de Correa de Distribucion", 1, 15000.00, "carlos.mecanico@taller.com", "Finalizada", "Cambio a los 80.000 km"),
        ("Servicio Completo", 1, 120000.00, "carlos.mecanico@taller.com", "Finalizada", "Revisión integral de fluidos")
    ]),
    (42, "2026-09-19 09:30:00", 0, "Pagada", [
        ("Limpieza de Inyectores", 1, 48000.00, "marcos.mecanico@taller.com", "Finalizada", "Pulverización y estanqueidad ensayadas")
    ]),
    (43, "2026-09-22 12:00:00", 0, "Pagada", [
        ("Cambio de Bujías", 4, 7000.00, "lucas.mecanico@taller.com", "Finalizada", "Bujías Bosch 4 electrodos"),
        ("Diagnóstico Computarizado OBD2", 1, 20000.00, "lucas.mecanico@taller.com", "Finalizada", "Lectura de parámetros en vivo")
    ]),
    (44, "2026-09-25 14:40:00", 0, "Pagada", [
        ("Alineación y Balanceo", 1, 38000.00, "carlos.mecanico@taller.com", "Finalizada", "Plomadas adhesivas en llantas de aleación")
    ]),
    (45, "2026-09-27 16:15:00", 0, "Pagada", [
        ("Cambio de Aceite de Motor", 1, 65000.00, "marcos.mecanico@taller.com", "Finalizada", "Aceite Total Quartz 7000"),
        ("Cambio de Filtro de Aceite", 1, 34000.00, "marcos.mecanico@taller.com", "Finalizada", "Filtro cambiado")
    ]),
    (46, "2026-09-29 10:20:00", 0, "Pagada", [
        ("Cambio de Pastillas de Freno", 1, 45000.00, "lucas.mecanico@taller.com", "Finalizada", "Sensibilidad del pedal restablecida"),
        ("Cambio de Líquido de Frenos", 1, 30000.00, "lucas.mecanico@taller.com", "Finalizada", "Líquido de frenos reemplazado")
    ]),
    (47, "2026-09-30 17:30:00", 0, "Pagada", [
        ("Escaneo General", 1, 25000.00, "carlos.mecanico@taller.com", "Finalizada", "Escaneo completo sin novedades")
    ]),

    # --- OCTUBRE 2026 (12 ordenes con estados variados) ---
    (48, "2026-10-02 08:30:00", 0, "Pagada", [
        ("Cambio de Aceite de Motor", 1, 65000.00, "marcos.mecanico@taller.com", "Finalizada", "Servicio preventivo"),
        ("Cambio de Filtro Aire", 1, 25000.00, "marcos.mecanico@taller.com", "Finalizada", "Filtro sustituido")
    ]),
    (49, "2026-10-05 10:00:00", 0, "Pagada", [
        ("Alineación y Balanceo", 1, 38000.00, "lucas.mecanico@taller.com", "Finalizada", "Control de convergencia"),
        ("Rotación de Neumáticos", 1, 22000.00, "lucas.mecanico@taller.com", "Finalizada", "Rotación en X")
    ]),
    (50, "2026-10-08 14:15:00", 0, "Pagada", [
        ("Cambio de Pastillas de Freno", 1, 45000.00, "carlos.mecanico@taller.com", "Finalizada", "Revisión general de frenos")
    ]),
    (51, "2026-10-11 09:10:00", 0, "Pagada", [
        ("Cambio de Batería", 1, 95000.00, "marcos.mecanico@taller.com", "Finalizada", "Borne limpiado y engrasado")
    ]),
    (52, "2026-10-14 11:30:00", 0, "Pagada", [
        ("Recarga de Aire Acondicionado", 1, 55000.00, "lucas.mecanico@taller.com", "Finalizada", "Carga completa de gas")
    ]),
    (53, "2026-10-16 15:45:00", 0, "Pagada", [
        ("Limpieza de Inyectores", 1, 48000.00, "carlos.mecanico@taller.com", "Finalizada", "Excelente respuesta")
    ]),
    # Órdenes Completadas (listas para facturar)
    (54, "2026-10-19 09:00:00", 0, "Completada", [
        ("Cambio de Aceite de Motor", 1, 65000.00, "marcos.mecanico@taller.com", "Finalizada", "Aceite sintético nuevo"),
        ("Cambio de Filtro de Aceite", 1, 34000.00, "marcos.mecanico@taller.com", "Finalizada", "Filtro instalado")
    ]),
    (0, "2026-10-21 11:15:00", 2500, "Completada", [
        ("Alineación y Balanceo", 1, 38000.00, "lucas.mecanico@taller.com", "Finalizada", "Comprobación final terminada")
    ]),
    (1, "2026-10-22 14:00:00", 3000, "Completada", [
        ("Cambio de Pastillas de Freno", 1, 45000.00, "carlos.mecanico@taller.com", "Finalizada", "Trabajo finalizado")
    ]),
    # Órdenes En Proceso
    (2, "2026-10-24 08:30:00", 1500, "En Proceso", [
        ("Cambio de Correa de Distribucion", 1, 15000.00, "marcos.mecanico@taller.com", "En Curso", "Desarmando distribución"),
        ("Servicio Completo", 1, 120000.00, "lucas.mecanico@taller.com", "Pendiente", "Esperando finalización de correa")
    ]),
    (3, "2026-10-25 10:20:00", 1200, "En Proceso", [
        ("Diagnóstico Computarizado OBD2", 1, 20000.00, "carlos.mecanico@taller.com", "En Curso", "Escaneando módulos de carrocería")
    ]),
    # Orden Abierta
    (4, "2026-10-26 09:10:00", 800, "Abierta", [
        ("Alineación y Balanceo", 1, 38000.00, "marcos.mecanico@taller.com", "Pendiente", "Vehículo ingresado a la espera de turno")
    ]),
    # Orden Cancelada
    (5, "2026-10-27 11:00:00", 500, "Cancelada", [
        ("Cambio de Batería", 1, 95000.00, "lucas.mecanico@taller.com", "Pendiente", "Cliente desestimó presupuesto")
    ])
]

print(f"Total de órdenes definidas: {len(ordenes_def)}")

def generar_sql():
    sql = []
    
    # -------------------------------------------------------------
    # 11. Usuarios adicionales del equipo (Recepcionista y Mecánicos)
    # -------------------------------------------------------------
    sql.append("""-- ============================================================================
-- 11. Usuarios del equipo de trabajo (Recepcionista y Mecánicos)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Usuarios] WHERE [Email] = N'recepcion@taller.com')
    INSERT INTO [dbo].[Usuarios] ([RolId], [Nombre], [Apellido], [Dni], [Email], [Telefono], [Domicilio], [FechaNacimiento], [Contrasena], [Activo])
    VALUES (2, N'Laura', N'Fernández', N'28111222', N'recepcion@taller.com', N'3624000101', N'Calle Mitre 350', CAST(N'1985-04-12T00:00:00' AS datetime2), N'CHANGE_ME', 1);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Usuarios] WHERE [Email] = N'carlos.mecanico@taller.com')
    INSERT INTO [dbo].[Usuarios] ([RolId], [Nombre], [Apellido], [Dni], [Email], [Telefono], [Domicilio], [FechaNacimiento], [Contrasena], [Activo])
    VALUES (3, N'Carlos', N'Benítez', N'32444555', N'carlos.mecanico@taller.com', N'3624000102', N'Av. Sarmiento 1200', CAST(N'1987-08-20T00:00:00' AS datetime2), N'CHANGE_ME', 1);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Usuarios] WHERE [Email] = N'marcos.mecanico@taller.com')
    INSERT INTO [dbo].[Usuarios] ([RolId], [Nombre], [Apellido], [Dni], [Email], [Telefono], [Domicilio], [FechaNacimiento], [Contrasena], [Activo])
    VALUES (3, N'Marcos', N'Romero', N'35666777', N'marcos.mecanico@taller.com', N'3624000103', N'Calle Vedia 560', CAST(N'1990-11-15T00:00:00' AS datetime2), N'CHANGE_ME', 1);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Usuarios] WHERE [Email] = N'lucas.mecanico@taller.com')
    INSERT INTO [dbo].[Usuarios] ([RolId], [Nombre], [Apellido], [Dni], [Email], [Telefono], [Domicilio], [FechaNacimiento], [Contrasena], [Activo])
    VALUES (3, N'Lucas', N'Almirón', N'38888999', N'lucas.mecanico@taller.com', N'3624000104', N'Av. Castelli 980', CAST(N'1994-02-28T00:00:00' AS datetime2), N'CHANGE_ME', 1);
GO
""")

    # -------------------------------------------------------------
    # 12. Clientes Semilla (55 clientes)
    # -------------------------------------------------------------
    sql.append("""-- ============================================================================
-- 12. Clientes (55 clientes del mercado local argentino)
-- ============================================================================
CREATE TABLE #ClientesSeed (
    [Nombre] NVARCHAR(100),
    [Apellido] NVARCHAR(100),
    [Dni] NVARCHAR(8),
    [FechaNacimiento] DATETIME2,
    [Direccion] NVARCHAR(200),
    [Email] NVARCHAR(MAX),
    [Telefono] NVARCHAR(MAX)
);

INSERT INTO #ClientesSeed ([Nombre], [Apellido], [Dni], [FechaNacimiento], [Direccion], [Email], [Telefono])
VALUES""")

    cli_values = []
    for c in clientes_data:
        cli_values.append(f"    (N'{c[0]}', N'{c[1]}', N'{c[2]}', CAST(N'{c[3]}T00:00:00' AS DATETIME2), N'{c[4]}', N'{c[5]}', N'{c[6]}')")
    sql.append(",\n".join(cli_values) + ";\n")

    sql.append("""MERGE [dbo].[Cliente] AS target
USING #ClientesSeed AS source
ON target.[Dni] = source.[Dni]
WHEN NOT MATCHED THEN
    INSERT ([Nombre], [Apellido], [Dni], [FechaNacimiento], [Direccion], [Email], [Telefono], [Activo])
    VALUES (source.[Nombre], source.[Apellido], source.[Dni], source.[FechaNacimiento], source.[Direccion], source.[Email], source.[Telefono], 1);

DROP TABLE #ClientesSeed;
GO
""")

    # -------------------------------------------------------------
    # 13. Vehículos Semilla (55 vehículos con Marca y Modelo)
    # -------------------------------------------------------------
    sql.append("""-- ============================================================================
-- 13. Vehículos (55 vehículos vinculados a modelos oficiales)
-- ============================================================================
CREATE TABLE #VehiculosSeed (
    [Marca] NVARCHAR(100),
    [Modelo] NVARCHAR(100),
    [Anio] INT,
    [Patente] NVARCHAR(15),
    [KmActual] INT
);

INSERT INTO #VehiculosSeed ([Marca], [Modelo], [Anio], [Patente], [KmActual])
VALUES""")

    veh_values = []
    for v in vehiculos_data:
        veh_values.append(f"    (N'{v[0]}', N'{v[1]}', {v[2]}, N'{v[3]}', {v[4]})")
    sql.append(",\n".join(veh_values) + ";\n")

    sql.append("""MERGE [dbo].[Vehiculo] AS target
USING (
    SELECT mo.[Id] AS [ModeloId], s.[Anio], s.[Patente], s.[KmActual]
    FROM #VehiculosSeed s
    INNER JOIN [dbo].[Marca] ma ON ma.[NombreMarca] = s.[Marca]
    INNER JOIN [dbo].[Modelo] mo ON mo.[MarcaId] = ma.[Id] AND mo.[NombreModelo] = s.[Modelo]
) AS source
ON target.[Patente] = source.[Patente]
WHEN NOT MATCHED THEN
    INSERT ([ModeloId], [Anio], [Patente], [KmActual])
    VALUES (source.[ModeloId], source.[Anio], source.[Patente], source.[KmActual]);

DROP TABLE #VehiculosSeed;
GO
""")

    # -------------------------------------------------------------
    # 14. Propietarios de Vehículos (relación Cliente <-> Vehículo)
    # -------------------------------------------------------------
    sql.append("""-- ============================================================================
-- 14. PropietarioVehiculo (vinculación de titularidad Cliente <-> Vehículo)
-- ============================================================================
CREATE TABLE #PropietariosSeed (
    [DniCliente] NVARCHAR(8),
    [PatenteVehiculo] NVARCHAR(15),
    [FechaAdquisicion] DATETIME2
);

INSERT INTO #PropietariosSeed ([DniCliente], [PatenteVehiculo], [FechaAdquisicion])
VALUES""")

    prop_values = []
    for i in range(len(vehiculos_data)):
        c = clientes_data[i % len(clientes_data)]
        v = vehiculos_data[i]
        anio_adq = max(v[2], 2021)
        prop_values.append(f"    (N'{c[2]}', N'{v[3]}', CAST(N'{anio_adq}-03-01T00:00:00' AS DATETIME2))")
    sql.append(",\n".join(prop_values) + ";\n")

    sql.append("""MERGE [dbo].[PropietarioVehiculo] AS target
USING (
    SELECT c.[Id] AS [ClienteId], v.[Id] AS [VehiculoId], s.[FechaAdquisicion]
    FROM #PropietariosSeed s
    INNER JOIN [dbo].[Cliente] c ON c.[Dni] = s.[DniCliente]
    INNER JOIN [dbo].[Vehiculo] v ON v.[Patente] = s.[PatenteVehiculo]
) AS source
ON target.[ClienteId] = source.[ClienteId] AND target.[VehiculoId] = source.[VehiculoId]
WHEN NOT MATCHED THEN
    INSERT ([ClienteId], [VehiculoId], [FechaAdquisicion], [FechaVenta], [EsActual])
    VALUES (source.[ClienteId], source.[VehiculoId], source.[FechaAdquisicion], NULL, 1);

DROP TABLE #PropietariosSeed;
GO
""")

    # -------------------------------------------------------------
    # 15, 16, 17. RegistroServicio, DetalleServicio, Factura y Pago
    # -------------------------------------------------------------
    sql.append("""-- ============================================================================
-- 15. Órdenes de Servicio (RegistroServicio), Detalles, Facturas y Pagos
--     (Desde Junio 2026 a Octubre 2026, con estados realistas)
-- ============================================================================
SET NOCOUNT ON;

DECLARE @TallerId INT = (SELECT TOP 1 [Id] FROM [dbo].[Taller] WHERE [Nombre] = N'Taller PRO');
IF @TallerId IS NULL SET @TallerId = 1;

DECLARE @UsuarioRecepId INT = (SELECT TOP 1 [Id] FROM [dbo].[Usuarios] WHERE [Email] = N'recepcion@taller.com');
IF @UsuarioRecepId IS NULL SET @UsuarioRecepId = (SELECT TOP 1 [Id] FROM [dbo].[Usuarios] WHERE [RolId] = 1);

DECLARE @MecCarlosId INT = (SELECT TOP 1 [Id] FROM [dbo].[Usuarios] WHERE [Email] = N'carlos.mecanico@taller.com');
DECLARE @MecMarcosId INT = (SELECT TOP 1 [Id] FROM [dbo].[Usuarios] WHERE [Email] = N'marcos.mecanico@taller.com');
DECLARE @MecLucasId INT  = (SELECT TOP 1 [Id] FROM [dbo].[Usuarios] WHERE [Email] = N'lucas.mecanico@taller.com');

IF @MecCarlosId IS NULL SET @MecCarlosId = @UsuarioRecepId;
IF @MecMarcosId IS NULL SET @MecMarcosId = @UsuarioRecepId;
IF @MecLucasId IS NULL  SET @MecLucasId  = @UsuarioRecepId;

DECLARE @MetodoEfectivoId INT = (SELECT TOP 1 [Id] FROM [dbo].[MetodoPago] WHERE [Nombre] = N'Efectivo');
DECLARE @MetodoTarjetaId INT  = (SELECT TOP 1 [Id] FROM [dbo].[MetodoPago] WHERE [Nombre] = N'Tarjeta');
IF @MetodoEfectivoId IS NULL SET @MetodoEfectivoId = 1;
IF @MetodoTarjetaId IS NULL  SET @MetodoTarjetaId  = 2;

DECLARE @VehiculoId INT;
DECLARE @RegistroId INT;
DECLARE @FacturaId INT;
DECLARE @ServicioId INT;
DECLARE @PrecioServicio DECIMAL(18,2);
DECLARE @KmCalculado INT;
DECLARE @TotalOrden DECIMAL(10,2);
DECLARE @MetodoPagoId INT;
DECLARE @ContadorFactura INT = 1;
""")

    for idx, (veh_idx, fecha, km_offset, estado, detalles) in enumerate(ordenes_def, 1):
        patente = vehiculos_data[veh_idx][3]
        base_km = vehiculos_data[veh_idx][4]
        km_ingreso = base_km + km_offset

        sql.append(f"""
-- --- Orden {idx:02d} ({patente} - {fecha[:10]} - {estado}) ---
SELECT @VehiculoId = [Id] FROM [dbo].[Vehiculo] WHERE [Patente] = N'{patente}';

IF @VehiculoId IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [dbo].[RegistroServicio] 
    WHERE [VehiculoId] = @VehiculoId AND [Fecha] = CAST(N'{fecha}' AS DATETIME2)
)
BEGIN
    -- 1. Insertar RegistroServicio
    INSERT INTO [dbo].[RegistroServicio] ([VehiculoId], [TallerId], [UsuarioId], [Fecha], [KmIngreso], [Estado])
    VALUES (@VehiculoId, @TallerId, @UsuarioRecepId, CAST(N'{fecha}' AS DATETIME2), {km_ingreso}, N'{estado}');

    SET @RegistroId = SCOPE_IDENTITY();

    -- 2. Insertar Detalles de Servicio (ANTES de facturar para respetar TR_DetalleServicio_BloquearModificacionFinalizada)""")

        for orden_ejec, (serv_nom, cant, precio, mec_email, det_estado, obs) in enumerate(detalles, 1):
            if "carlos" in mec_email:
                mec_var = "@MecCarlosId"
            elif "marcos" in mec_email:
                mec_var = "@MecMarcosId"
            else:
                mec_var = "@MecLucasId"

            sql.append(f"""
    SELECT @ServicioId = [Id], @PrecioServicio = [Precio] FROM [dbo].[Servicio] WHERE [Nombre] = N'{serv_nom}';
    IF @ServicioId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[DetalleServicio] ([RegistroServicioId], [UsuarioId], [ServicioId], [Cantidad], [Precio], [OrdenEjecucion], [Origen], [Estado], [Observaciones])
        VALUES (@RegistroId, {mec_var}, @ServicioId, {cant}, {precio:.2f}, {orden_ejec}, N'Presencial', N'{det_estado}', N'{obs}');
    END""")

        if estado == "Pagada":
            sql.append(f"""
    -- 3. Facturación y Pago (solo órdenes Pagadas)
    SELECT @TotalOrden = ISNULL(SUM([Precio] * [Cantidad]), 0) FROM [dbo].[DetalleServicio] WHERE [RegistroServicioId] = @RegistroId;

    IF @TotalOrden > 0
    BEGIN
        INSERT INTO [dbo].[Factura] ([RegistroServicioId], [Numero], [Fecha], [Total])
        VALUES (@RegistroId, NEXT VALUE FOR [dbo].[Seq_Factura_Numero], CAST(N'{fecha}' AS DATETIME2), @TotalOrden);

        SET @FacturaId = SCOPE_IDENTITY();
        SET @MetodoPagoId = CASE WHEN (@RegistroId % 2) = 0 THEN @MetodoTarjetaId ELSE @MetodoEfectivoId END;

        INSERT INTO [dbo].[Pago] ([FacturaId], [MetodoPagoId], [Monto], [FechaPago])
        VALUES (@FacturaId, @MetodoPagoId, @TotalOrden, CAST(N'{fecha}' AS DATETIME2));
    END""")

        sql.append("""
END;
""")

    sql.append("""GO
""")
    return "".join(sql)

if __name__ == "__main__":
    bloque = generar_sql()
    with open("docs/script/bloque_seed_generado.sql", "w", encoding="utf-8") as f:
        f.write(bloque)
    print("Bloque SQL generado exitosamente en docs/script/bloque_seed_generado.sql")
