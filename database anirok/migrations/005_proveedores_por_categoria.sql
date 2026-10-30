USE anirok2;
GO

DECLARE @Proveedores TABLE (nombre VARCHAR(100), contacto VARCHAR(100), telefono VARCHAR(40), email VARCHAR(120));
INSERT INTO @Proveedores VALUES
('Distribuidora Mochilas', 'Mariana López', '11-4500-1001', 'mochilas@anirok.test'),
('Importadora Materos', 'Pablo Gómez', '11-4500-1002', 'materos@anirok.test'),
('Textiles del Sur', 'Lucía Fernández', '11-4500-1003', 'textiles@anirok.test'),
('Accesorios Urbanos', 'Nicolás Torres', '11-4500-1004', 'accesorios@anirok.test'),
('Comics Central', 'Sofía Ruiz', '11-4500-1005', 'comics@anirok.test'),
('Figuras Coleccionables', 'Diego Acosta', '11-4500-1006', 'figuras@anirok.test'),
('Mundo Peluche', 'Carla Méndez', '11-4500-1007', 'peluches@anirok.test');

INSERT INTO dbo.proveedor (nombre, contacto, telefono, email, esta_activo)
SELECT p.nombre, p.contacto, p.telefono, p.email, 'Activa'
FROM @Proveedores p
WHERE NOT EXISTS (SELECT 1 FROM dbo.proveedor existente WHERE existente.nombre = p.nombre);

UPDATE producto
SET proovedor = CASE c.nombre
    WHEN 'Mochilas' THEN 'Distribuidora Mochilas'
    WHEN 'Set Materos' THEN 'Importadora Materos'
    WHEN 'Bufandas' THEN 'Textiles del Sur'
    WHEN 'Cartucheras' THEN 'Accesorios Urbanos'
    WHEN 'Comics' THEN 'Comics Central'
    WHEN 'Figuras' THEN 'Figuras Coleccionables'
    WHEN 'Peluches' THEN 'Mundo Peluche'
    ELSE proovedor
END
FROM dbo.producto producto
INNER JOIN dbo.categoria c ON c.id = producto.categoria_id;
GO
