-- =============================================
-- 19_ConfiguracionEmpresa.sql
-- Identidad de empresa: logo + datos del taller.
-- Spec: openspec/changes/2026-10-10-logo-empresa
-- Tabla singleton (una sola fila, Id = 1).
-- Script idempotente: puede ejecutarse varias veces sin duplicar.
-- =============================================
USE [ControlVehicular];
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConfiguracionEmpresa]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ConfiguracionEmpresa](
        [Id] [int] NOT NULL,
        [Nombre] [nvarchar](100) NOT NULL,
        [Direccion] [nvarchar](200) NOT NULL,
        [Telefono] [nvarchar](20) NOT NULL,
        [Email] [nvarchar](150) NULL,
        [Logo] [varbinary](max) NULL,
        [FechaActualizacion] [datetime2] NOT NULL,
        CONSTRAINT [PK_ConfiguracionEmpresa] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

-- Fila semilla con los valores por defecto actuales del sistema (si la tabla esta vacia).
IF NOT EXISTS (SELECT 1 FROM [dbo].[ConfiguracionEmpresa])
BEGIN
    INSERT INTO [dbo].[ConfiguracionEmpresa] ([Id], [Nombre], [Direccion], [Telefono], [Email], [Logo], [FechaActualizacion])
    VALUES (1, N'TALLER PRO', N'Ruta Nacional 11 Km 1005 - Resistencia, Chaco', N'+54 362 4123456', N'contacto@taller.com', NULL, SYSDATETIME());
END
GO
