/* Dinero siempre con precisión fija. Es seguro ejecutarlo más de una vez. */
IF COL_LENGTH(N'dbo.producto', N'precio') IS NOT NULL
    ALTER TABLE dbo.producto ALTER COLUMN precio DECIMAL(18,2) NULL;
IF COL_LENGTH(N'dbo.venta', N'precio_total') IS NOT NULL
    ALTER TABLE dbo.venta ALTER COLUMN precio_total DECIMAL(18,2) NULL;
IF COL_LENGTH(N'dbo.venta', N'descuento') IS NOT NULL
    ALTER TABLE dbo.venta ALTER COLUMN descuento DECIMAL(18,2) NULL;
IF COL_LENGTH(N'dbo.detalle_venta', N'precio_unitario') IS NOT NULL
    ALTER TABLE dbo.detalle_venta ALTER COLUMN precio_unitario DECIMAL(18,2) NOT NULL;
IF COL_LENGTH(N'dbo.producto', N'costo') IS NOT NULL
    ALTER TABLE dbo.producto ALTER COLUMN costo DECIMAL(18,2) NULL;
IF COL_LENGTH(N'dbo.producto', N'costo_pesos') IS NOT NULL
    ALTER TABLE dbo.producto ALTER COLUMN costo_pesos DECIMAL(18,2) NULL;
IF COL_LENGTH(N'dbo.producto', N'cotizacion_dolar') IS NOT NULL
    ALTER TABLE dbo.producto ALTER COLUMN cotizacion_dolar DECIMAL(18,2) NULL;
IF COL_LENGTH(N'dbo.producto_historial_precio', N'costo_anterior') IS NOT NULL
    ALTER TABLE dbo.producto_historial_precio ALTER COLUMN costo_anterior DECIMAL(18,2) NULL;
IF COL_LENGTH(N'dbo.producto_historial_precio', N'costo_nuevo') IS NOT NULL
    ALTER TABLE dbo.producto_historial_precio ALTER COLUMN costo_nuevo DECIMAL(18,2) NULL;
IF COL_LENGTH(N'dbo.producto_historial_precio', N'precio_anterior') IS NOT NULL
    ALTER TABLE dbo.producto_historial_precio ALTER COLUMN precio_anterior DECIMAL(18,2) NULL;
IF COL_LENGTH(N'dbo.producto_historial_precio', N'precio_nuevo') IS NOT NULL
    ALTER TABLE dbo.producto_historial_precio ALTER COLUMN precio_nuevo DECIMAL(18,2) NULL;
IF COL_LENGTH(N'dbo.producto_historial_precio', N'cotizacion_dolar') IS NOT NULL
    ALTER TABLE dbo.producto_historial_precio ALTER COLUMN cotizacion_dolar DECIMAL(18,2) NULL;
GO
