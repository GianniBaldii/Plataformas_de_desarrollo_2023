/* Rework opcional para datos de prueba.
   Regla aplicada:
   - costo estimado: 60% del precio actual
   - aumento de precio: 20%
   - redondeo al múltiplo de $500 más cercano
   No ejecutar en producción sin reemplazar esta regla.
*/
SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @porcentajeAumento REAL = 0.20;
DECLARE @porcentajeCosto REAL = 0.60;
DECLARE @fecha DATETIME = GETDATE();

DECLARE @Cambios TABLE
(
    producto_id INT,
    precio_anterior REAL,
    precio_nuevo REAL,
    costo_nuevo REAL
);

INSERT INTO @Cambios (producto_id, precio_anterior, precio_nuevo, costo_nuevo)
SELECT id,
       precio,
       ROUND((precio * (1 + @porcentajeAumento)) / 500, 0) * 500,
       ROUND((precio * @porcentajeCosto) / 500, 0) * 500
FROM dbo.producto
WHERE precio IS NOT NULL;

UPDATE p
SET p.costo = c.costo_nuevo,
    p.costo_moneda = 'ARS',
    p.cotizacion_dolar = NULL,
    p.costo_pesos = c.costo_nuevo,
    p.precio = c.precio_nuevo
FROM dbo.producto p
INNER JOIN @Cambios c ON c.producto_id = p.id;

INSERT INTO dbo.producto_historial_precio
(
    id, producto_id, costo_anterior, costo_nuevo,
    precio_anterior, precio_nuevo, moneda_costo,
    cotizacion_dolar, fecha_cambio, usuario_id, motivo, origen
)
SELECT
    ISNULL((SELECT MAX(id) FROM dbo.producto_historial_precio), 0) + ROW_NUMBER() OVER (ORDER BY producto_id),
    producto_id,
    NULL,
    costo_nuevo,
    precio_anterior,
    precio_nuevo,
    'ARS',
    NULL,
    @fecha,
    NULL,
    'Rework inicial de datos de prueba (+20%, costo estimado 60%)',
    'Prueba'
FROM @Cambios;

COMMIT TRANSACTION;
