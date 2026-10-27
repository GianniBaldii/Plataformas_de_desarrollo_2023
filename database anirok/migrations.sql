SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_usuario_nombre' AND object_id = OBJECT_ID(N'dbo.usuario'))
    CREATE UNIQUE INDEX UX_usuario_nombre ON dbo.usuario(nombre) WHERE nombre IS NOT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_usuario_mail' AND object_id = OBJECT_ID(N'dbo.usuario'))
    CREATE UNIQUE INDEX UX_usuario_mail ON dbo.usuario(mail) WHERE mail IS NOT NULL;
GO
