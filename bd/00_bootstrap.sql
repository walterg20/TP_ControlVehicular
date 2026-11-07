-- ============================================================
-- 00_bootstrap.sql
-- Crea la base de datos ControlVehicular si no existe.
-- Ejecutar conectado a 'master' (sqlcmd -d master).
-- ============================================================
SET NOCOUNT ON;
GO

IF DB_ID(N'ControlVehicular') IS NULL
BEGIN
    CREATE DATABASE [ControlVehicular];
    PRINT 'Base de datos ControlVehicular creada.';
END
ELSE
BEGIN
    PRINT 'La base de datos ControlVehicular ya existe.';
END
GO

USE [ControlVehicular];
GO