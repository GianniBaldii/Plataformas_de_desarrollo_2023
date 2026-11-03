SET NOCOUNT ON;

-- Corrige la desincronización histórica entre el bloque de cartucheras/comics
-- y sus archivos de storage. Es idempotente y no modifica productos fuera del
-- bloque afectado.
IF OBJECT_ID(N'dbo.producto', N'U') IS NOT NULL
BEGIN
    UPDATE dbo.producto SET img = N'17.jpg' WHERE id = 17;
    UPDATE dbo.producto SET img = N'18.jpg' WHERE id = 18;
    UPDATE dbo.producto SET img = N'19.jpg' WHERE id = 19;
    UPDATE dbo.producto SET img = N'20.jpg' WHERE id = 20;
    UPDATE dbo.producto SET img = N'21.jpg' WHERE id = 21;
    UPDATE dbo.producto SET img = N'22.jpg' WHERE id = 22;
    UPDATE dbo.producto SET img = N'23.jpg' WHERE id = 23;
    UPDATE dbo.producto SET img = N'24.jpg' WHERE id = 24;
    UPDATE dbo.producto SET img = N'25.jpg' WHERE id = 25;
    UPDATE dbo.producto SET img = N'26.jpg' WHERE id = 26;
    UPDATE dbo.producto SET img = N'27.jpg' WHERE id = 27;
END;
GO
