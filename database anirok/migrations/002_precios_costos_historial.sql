/*
   Precios, costos y trazabilidad.
   Ejecutar sobre SQL Server una sola vez antes de usar el nuevo módulo.
*/
IF COL_LENGTH('dbo.producto', 'costo') IS NULL
    ALTER TABLE dbo.producto ADD costo REAL NULL;

IF COL_LENGTH('dbo.producto', 'costo_moneda') IS NULL
    ALTER TABLE dbo.producto ADD costo_moneda VARCHAR(3) NULL;

IF COL_LENGTH('dbo.producto', 'cotizacion_dolar') IS NULL
    ALTER TABLE dbo.producto ADD cotizacion_dolar REAL NULL;

IF COL_LENGTH('dbo.producto', 'costo_pesos') IS NULL
    ALTER TABLE dbo.producto ADD costo_pesos REAL NULL;

IF OBJECT_ID('dbo.producto_historial_precio', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.producto_historial_precio
    (
        id INT NOT NULL PRIMARY KEY,
        producto_id INT NOT NULL,
        costo_anterior REAL NULL,
        costo_nuevo REAL NULL,
        precio_anterior REAL NULL,
        precio_nuevo REAL NULL,
        moneda_costo VARCHAR(3) NULL,
        cotizacion_dolar REAL NULL,
        fecha_cambio DATETIME NOT NULL,
        usuario_id INT NULL,
        motivo VARCHAR(250) NULL,
        origen VARCHAR(20) NOT NULL,
        CONSTRAINT fk_historial_producto FOREIGN KEY (producto_id) REFERENCES dbo.producto(id),
        CONSTRAINT fk_historial_usuario FOREIGN KEY (usuario_id) REFERENCES dbo.usuario(id)
    );
END;

GO

/* Productos existentes: se consideran costos aún no informados. */
UPDATE dbo.producto
SET costo_moneda = ISNULL(costo_moneda, 'ARS')
WHERE costo_moneda IS NULL;
