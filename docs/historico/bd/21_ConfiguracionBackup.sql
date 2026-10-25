-- =============================================
-- 21_ConfiguracionBackup.sql
-- Configuracion de respaldo de base de datos y resguardo en Google Drive.
-- Spec: openspec/changes/2026-10-11-backup-drive
-- Tabla singleton (una sola fila, Id = 1).
-- Script idempotente: puede ejecutarse varias veces sin duplicar.
-- =============================================
USE [ControlVehicular];
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConfiguracionBackup]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ConfiguracionBackup](
        [Id] [int] NOT NULL,
        [NombreBaseDatos] [nvarchar](128) NOT NULL,
        [RutaBackupServer] [nvarchar](260) NOT NULL,
        [RutaBackupUNC] [nvarchar](260) NOT NULL,
        [DriveFolderId] [nvarchar](260) NULL,
        [UltimaSubidaArchivo] [nvarchar](260) NULL,
        [FechaUltimaSubida] [datetime2] NULL,
        [FechaActualizacion] [datetime2] NOT NULL,
        CONSTRAINT [PK_ConfiguracionBackup] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

-- Fila semilla con los valores por defecto del sistema (si la tabla esta vacia).
-- Las rutas quedan vacias: se configuran por taller en la pantalla "Configuracion de Respaldo".
IF NOT EXISTS (SELECT 1 FROM [dbo].[ConfiguracionBackup])
BEGIN
    INSERT INTO [dbo].[ConfiguracionBackup] ([Id], [NombreBaseDatos], [RutaBackupServer], [RutaBackupUNC], [DriveFolderId], [UltimaSubidaArchivo], [FechaUltimaSubida], [FechaActualizacion])
    VALUES (1, N'ControlVehicular', N'', N'', NULL, NULL, NULL, SYSDATETIME());
END
GO
