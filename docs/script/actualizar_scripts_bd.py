# -*- coding: utf-8 -*-
"""
actualizar_scripts_bd.py
-------------------------
Reorganiza y actualiza los scripts canónicos en bd/:
  * 01_estructura.sql: Esquema DDL con 18 tablas, secuencias, índices, checks y guardas defensivas completas.
  * 02_datos.sql: Únicamente datos semilla canónicos (roles, permisos, matriz rol-permiso, taller, configs, admin, 15 servicios, 45 marcas y 203 modelos).
  * 03_procedimientos.sql: 22 procedimientos almacenados oficiales (CREATE OR ALTER).
  * 04_triggers.sql: 3 triggers de negocio e integridad (CREATE OR ALTER).
  * 05_utilidades_test.sql: Limpieza de artefactos de test E2E.
  * 06_poblacion.sql: Población de datos operativos/demo (equipo, 55 clientes, 55 vehículos, 61 órdenes, detalles, facturas y pagos).
"""
import os

BD_DIR = os.path.abspath("bd")

# ----------------------------------------------------------------------------
# 1. Separar 02_datos.sql en canónico y población (06_poblacion.sql)
# ----------------------------------------------------------------------------
datos_path = os.path.join(BD_DIR, "02_datos.sql")
with open(datos_path, "r", encoding="utf-8") as f:
    datos_content = f.read()

target_split = "-- ============================================================================\n-- 11. Usuarios del equipo de trabajo"
idx = datos_content.find(target_split)
if idx == -1:
    raise ValueError(f"No se encontró el delimitador de sección 11 en {datos_path}")

canonical_body = datos_content[:idx].strip()
population_body = datos_content[idx:].strip()

# Nuevo encabezado canónico para 02_datos.sql
canonical_header = """-- ============================================================================
-- 02_datos.sql — Datos semilla canónicos del sistema de control vehicular.
-- ----------------------------------------------------------------------------
-- Contenido canónico oficial (Bootstrap de Producción / Base Limpia):
--   * 1. Roles del sistema (Administrador = 1, Recepcionista = 2, Mecánico = 3).
--   * 2. Catálogo completo de permisos RBAC (38 permisos).
--   * 3. Matriz de asignación Rol -> Permiso (roles 1, 2 y 3; autorreparable).
--   * 4. Taller base (Id = 1, Taller PRO).
--   * 5. ConfiguracionEmpresa (fila singleton, Id = 1).
--   * 6. ConfiguracionBackup (fila singleton, Id = 1).
--   * 7. Métodos de pago (Efectivo, Tarjeta).
--   * 8. Usuario inicial Administrador bootstrap (admin@example.com / 10000001).
--   * 9. Catálogo oficial de Servicios (15 servicios con precios vigentes y Activo = 1).
--   * 10. Catálogo completo de Marcas y Modelos argentinos (45 marcas, 203 modelos;
--         preservando Toyota = Id 1 y Renault = Id 2).
--
-- NOTA: Los datos de población operativa y de demostración (personal de taller,
-- clientes, vehículos, órdenes de servicio, facturas y pagos) residen en '06_poblacion.sql'.
--
-- Idempotencia:
--   * Cada sección usa MERGE o IF NOT EXISTS para re-ejecución segura sin duplicados.
-- Compatible con SQL Server 2019 Express. Codificación UTF-8.
-- Orden de aplicación: 01_estructura -> 02_datos -> 03_procedimientos -> 04_triggers -> 05_utilidades -> 06_poblacion.
-- ============================================================================

USE [ControlVehicular];
GO
"""

# Reemplazar el encabezado viejo en canonical_body
old_header_end = canonical_body.find("-- ============================================================================\n-- 1. Roles")
if old_header_end == -1:
    raise ValueError("No se encontró el inicio de la sección 1 en 02_datos.sql")

new_02_datos = canonical_header + "\n" + canonical_body[old_header_end:] + "\n"

with open(datos_path, "w", encoding="utf-8") as f:
    f.write(new_02_datos)
print(f"[OK] bd/02_datos.sql actualizado ({len(new_02_datos)} caracteres).")

# Crear 06_poblacion.sql
population_header = """-- ============================================================================
-- 06_poblacion.sql — Población de datos operativos y de demostración (Demo).
-- ----------------------------------------------------------------------------
-- Contenido:
--   * 11. Usuarios del equipo de trabajo:
--         - Recepcionista: Laura Fernández (recepcion@taller.com / DNI 28111222)
--         - Mecánicos: Carlos Benítez (carlos.mecanico@taller.com / DNI 32444555)
--                      Marcos Romero (marcos.mecanico@taller.com / DNI 35666777)
--                      Lucas Almirón (lucas.mecanico@taller.com / DNI 38888999)
--   * 12. Clientes: 55 clientes locales argentinos con datos reales de contacto.
--   * 13. Vehículos: 55 vehículos vinculados a modelos oficiales y kilometrajes reales.
--   * 14. PropietarioVehiculo: 55 titularidades vigentes asociando cliente y vehículo.
--   * 15. Órdenes de Servicio (RegistroServicio): 61 órdenes entre Junio y Octubre 2026
--         en estados realistas ('Abierta', 'En Proceso', 'Completada', 'Pagada', 'Cancelada').
--   * 16. Detalles de Servicio (DetalleServicio): más de 110 tareas asignadas a mecánicos.
--   * 17. Facturación (Factura) y Pagos (Pago) para órdenes finalizadas y cobradas.
--
-- Idempotencia y Resiliencia:
--   * Cada cliente y vehículo se inserta con MERGE o IF NOT EXISTS.
--   * Cada orden de servicio verifica no existencia previa por Vehículo y Fecha.
--   * Se deshabilitan temporalmente los triggers de validación durante la carga
--     masiva y se re-habilitan al finalizar para garantizar ejecución libre de bloqueos.
--
-- Requiere:
--   * 01_estructura.sql ejecutado previamente.
--   * 02_datos.sql (datos canónicos) ejecutado previamente.
-- Compatible con SQL Server 2019 Express. Codificación UTF-8.
-- Orden de aplicación sugerido: 01_estructura -> 02_datos -> 03_procedimientos -> 04_triggers -> 06_poblacion.
-- ============================================================================
USE [ControlVehicular];
GO

SET NOCOUNT ON;

-- Deshabilitar temporalmente triggers de workflow para inserción histórica masiva
IF OBJECT_ID('TR_DetalleServicio_BloquearModificacionFinalizada', 'TR') IS NOT NULL
    DISABLE TRIGGER TR_DetalleServicio_BloquearModificacionFinalizada ON DetalleServicio;

IF OBJECT_ID('TR_RegistroServicio_SincronizarKm', 'TR') IS NOT NULL
    DISABLE TRIGGER TR_RegistroServicio_SincronizarKm ON RegistroServicio;
GO

"""

population_footer = """
-- Re-habilitar triggers de workflow
IF OBJECT_ID('TR_DetalleServicio_BloquearModificacionFinalizada', 'TR') IS NOT NULL
    ENABLE TRIGGER TR_DetalleServicio_BloquearModificacionFinalizada ON DetalleServicio;

IF OBJECT_ID('TR_RegistroServicio_SincronizarKm', 'TR') IS NOT NULL
    ENABLE TRIGGER TR_RegistroServicio_SincronizarKm ON RegistroServicio;
GO

PRINT N'Población de datos operativos y de demostración completada con éxito.';
GO
"""

new_06_poblacion = population_header + population_body + "\n" + population_footer

poblacion_path = os.path.join(BD_DIR, "06_poblacion.sql")
with open(poblacion_path, "w", encoding="utf-8") as f:
    f.write(new_06_poblacion)
print(f"[OK] bd/06_poblacion.sql creado ({len(new_06_poblacion)} caracteres).")

# ----------------------------------------------------------------------------
# 2. Actualizar 01_estructura.sql con guardas defensivas de migración
# ----------------------------------------------------------------------------
estructura_path = os.path.join(BD_DIR, "01_estructura.sql")
with open(estructura_path, "r", encoding="utf-8") as f:
    estructura_content = f.read()

# Guardas defensivas a insertar tras la sincronización Estado -> Activo
defensive_guard = """-- ----------------------------------------------------------------------------
-- Sincronización de nomenclatura (Estado -> Activo) si las tablas ya existían
-- ----------------------------------------------------------------------------
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Roles]') AND name = 'Estado')
   AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Roles]') AND name = 'Activo')
BEGIN
    EXEC sp_rename '[dbo].[Roles].[Estado]', 'Activo', 'COLUMN';
END
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Usuarios]') AND name = 'Estado')
   AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Usuarios]') AND name = 'Activo')
BEGIN
    EXEC sp_rename '[dbo].[Usuarios].[Estado]', 'Activo', 'COLUMN';
END
GO

-- ----------------------------------------------------------------------------
-- Asegurar columnas Activo en tablas existentes
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[Cliente]', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Cliente]') AND name = 'Activo')
BEGIN
    ALTER TABLE [dbo].[Cliente] ADD [Activo] bit NOT NULL CONSTRAINT DF_Cliente_Activo DEFAULT (1);
END
GO

IF OBJECT_ID(N'[dbo].[Roles]', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Roles]') AND name = 'Activo')
BEGIN
    ALTER TABLE [dbo].[Roles] ADD [Activo] bit NOT NULL CONSTRAINT DF_Roles_Activo DEFAULT (1);
END
GO

IF OBJECT_ID(N'[dbo].[Usuarios]', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Usuarios]') AND name = 'Activo')
BEGIN
    ALTER TABLE [dbo].[Usuarios] ADD [Activo] bit NOT NULL CONSTRAINT DF_Usuarios_Activo DEFAULT (1);
END
GO

IF OBJECT_ID(N'[dbo].[Servicio]', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Servicio]') AND name = 'Activo')
BEGIN
    ALTER TABLE [dbo].[Servicio] ADD [Activo] bit NOT NULL CONSTRAINT DF_Servicio_Activo DEFAULT (1);
END
GO

IF OBJECT_ID(N'[dbo].[Taller]', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Taller]') AND name = 'Activo')
BEGIN
    ALTER TABLE [dbo].[Taller] ADD [Activo] bit NOT NULL CONSTRAINT DF_Taller_Activo DEFAULT (1);
END
GO

-- ----------------------------------------------------------------------------
-- Asegurar columna OrdenEjecucion en DetalleServicio
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[DetalleServicio]', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[DetalleServicio]') AND name = 'OrdenEjecucion')
BEGIN
    ALTER TABLE [dbo].[DetalleServicio] ADD [OrdenEjecucion] int NOT NULL CONSTRAINT DF_DetalleServicio_OrdenEjecucion DEFAULT (1);
END
GO

-- ----------------------------------------------------------------------------
-- Asegurar restricciones CHECK en tablas existentes
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[RegistroServicio]', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CHK_RegistroServicio_Estado')
BEGIN
    ALTER TABLE [dbo].[RegistroServicio] ADD CONSTRAINT [CHK_RegistroServicio_Estado]
        CHECK ([Estado] IN ('Abierta', 'En Proceso', 'Completada', 'Pagada', 'Cancelada'));
END
GO

IF OBJECT_ID(N'[dbo].[DetalleServicio]', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CHK_DetalleServicio_Estado')
BEGIN
    ALTER TABLE [dbo].[DetalleServicio] ADD CONSTRAINT [CHK_DetalleServicio_Estado]
        CHECK ([Estado] IN ('Pendiente', 'En Curso', 'Finalizada'));
END
GO

IF OBJECT_ID(N'[dbo].[Servicio]', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_Servicio_Precio')
BEGIN
    ALTER TABLE [dbo].[Servicio] ADD CONSTRAINT [CK_Servicio_Precio] CHECK ([Precio] > 0);
END
GO

IF OBJECT_ID(N'[dbo].[Vehiculo]', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_Vehiculo_KmActual')
BEGIN
    ALTER TABLE [dbo].[Vehiculo] ADD CONSTRAINT [CK_Vehiculo_KmActual] CHECK ([KmActual] >= 0);
END
GO

IF OBJECT_ID(N'[dbo].[RegistroServicio]', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_RegistroServicio_KmIngreso')
BEGIN
    ALTER TABLE [dbo].[RegistroServicio] ADD CONSTRAINT [CK_RegistroServicio_KmIngreso] CHECK ([KmIngreso] >= 0);
END
GO

IF OBJECT_ID(N'[dbo].[DetalleServicio]', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_DetalleServicio_Precio')
BEGIN
    ALTER TABLE [dbo].[DetalleServicio] ADD CONSTRAINT [CK_DetalleServicio_Precio] CHECK ([Precio] >= 0);
END
GO

IF OBJECT_ID(N'[dbo].[DetalleServicio]', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_DetalleServicio_Cantidad')
BEGIN
    ALTER TABLE [dbo].[DetalleServicio] ADD CONSTRAINT [CK_DetalleServicio_Cantidad] CHECK ([Cantidad] > 0);
END
GO"""

old_sync_start = estructura_content.find("-- ----------------------------------------------------------------------------\n-- Sincronización de nomenclatura")
if old_sync_start != -1:
    old_seq_start = estructura_content.find("-- ----------------------------------------------------------------------------\n-- Secuencia para el número")
    estructura_content = estructura_content[:old_sync_start] + defensive_guard + "\n\n" + estructura_content[old_seq_start:]

# Actualizar el orden en la cabecera de 01_estructura.sql
old_order = "Orden de aplicación: 01_estructura -> 02_datos -> 03_procedimientos -> 04_triggers -> 05_utilidades."
new_order = "Orden de aplicación: 01_estructura -> 02_datos -> 03_procedimientos -> 04_triggers -> 05_utilidades -> 06_poblacion."
estructura_content = estructura_content.replace(old_order, new_order)

with open(estructura_path, "w", encoding="utf-8") as f:
    f.write(estructura_content)
print(f"[OK] bd/01_estructura.sql actualizado con guardas defensivas ({len(estructura_content)} caracteres).")

# ----------------------------------------------------------------------------
# 3. Actualizar cabecera de 03_procedimientos.sql (22 procedimientos)
# ----------------------------------------------------------------------------
procedimientos_path = os.path.join(BD_DIR, "03_procedimientos.sql")
with open(procedimientos_path, "r", encoding="utf-8") as f:
    proc_content = f.read()

old_proc_header = "-- Contenido: 16 procedimientos almacenados, cada uno con CREATE OR ALTER"
new_proc_header = "-- Contenido: 22 procedimientos almacenados oficiales (reportes, comprobantes y paginados),\n--            cada uno con CREATE OR ALTER"
proc_content = proc_content.replace(old_proc_header, new_proc_header)
proc_content = proc_content.replace(old_order, new_order)

with open(procedimientos_path, "w", encoding="utf-8") as f:
    f.write(proc_content)
print(f"[OK] bd/03_procedimientos.sql cabecera actualizada.")

# ----------------------------------------------------------------------------
# 4. Actualizar 04_triggers.sql
# ----------------------------------------------------------------------------
triggers_path = os.path.join(BD_DIR, "04_triggers.sql")
with open(triggers_path, "r", encoding="utf-8") as f:
    trig_content = f.read()

trig_content = trig_content.replace(old_order, new_order)
with open(triggers_path, "w", encoding="utf-8") as f:
    f.write(trig_content)
print(f"[OK] bd/04_triggers.sql actualizado.")

print("\n--- RESUMEN DE PROCESO ---")
print("1. 01_estructura.sql: DDL 18 tablas + índices + constraints + guardas defensivas.")
print("2. 02_datos.sql: Datos canónicos (roles, permisos, matriz, configs, admin, 15 servicios, marcas/modelos).")
print("3. 03_procedimientos.sql: 22 SPs con CREATE OR ALTER.")
print("4. 04_triggers.sql: 3 Triggers con CREATE OR ALTER.")
print("5. 05_utilidades_test.sql: Limpieza E2E (invariable).")
print("6. 06_poblacion.sql: Datos operativos/demo (55 clientes, 55 vehículos, 61 órdenes, detalles, facturas y pagos).")
