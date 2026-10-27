SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_usuario_nombre' AND object_id = OBJECT_ID(N'dbo.usuario'))
    CREATE UNIQUE INDEX UX_usuario_nombre ON dbo.usuario(nombre) WHERE nombre IS NOT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_usuario_mail' AND object_id = OBJECT_ID(N'dbo.usuario'))
    CREATE UNIQUE INDEX UX_usuario_mail ON dbo.usuario(mail) WHERE mail IS NOT NULL;
GO

IF COL_LENGTH(N'dbo.detalle_venta', N'precio_unitario') IS NULL
    ALTER TABLE dbo.detalle_venta ADD precio_unitario REAL NULL;
GO

UPDATE detalle
SET precio_unitario = producto.precio
FROM dbo.detalle_venta AS detalle
INNER JOIN dbo.producto AS producto ON producto.id = detalle.producto_id
WHERE detalle.precio_unitario IS NULL;
GO

IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.detalle_venta')
      AND name = N'precio_unitario'
      AND is_nullable = 1
)
    ALTER TABLE dbo.detalle_venta ALTER COLUMN precio_unitario REAL NOT NULL;
GO
