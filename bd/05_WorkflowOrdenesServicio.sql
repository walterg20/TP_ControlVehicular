-- 1. Añadir columna OrdenEjecucion a DetalleServicio
ALTER TABLE [DetalleServicio] 
ADD [OrdenEjecucion] int NOT NULL DEFAULT 1;
GO

-- 2. Limpieza de datos existentes para evitar conflictos con las restricciones (Constraints)
-- Actualizamos estados antiguos o inválidos al estado por defecto del nuevo workflow.
UPDATE [DetalleServicio]
SET [Estado] = 'Finalizada'
WHERE [Estado] NOT IN ('Pendiente', 'En Curso', 'Finalizada');
GO

UPDATE [RegistroServicio]
SET [Estado] = 'Completada'
WHERE [Estado] NOT IN ('Abierta', 'En Proceso', 'Completada', 'Pagada');
GO

-- 3. Añadir CHECK Constraints para los estados válidos
ALTER TABLE [DetalleServicio]
ADD CONSTRAINT [CHK_DetalleServicio_Estado] 
CHECK ([Estado] IN ('Pendiente', 'En Curso', 'Finalizada'));
GO

ALTER TABLE [RegistroServicio]
ADD CONSTRAINT [CHK_RegistroServicio_Estado] 
CHECK ([Estado] IN ('Abierta', 'En Proceso', 'Completada', 'Pagada'));
GO
