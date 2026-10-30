USE anirok2;
GO
IF OBJECT_ID(N'dbo.proveedor', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.proveedor
    (
        id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_proveedor PRIMARY KEY,
        nombre VARCHAR(100) NOT NULL,
        contacto VARCHAR(100) NULL,
        telefono VARCHAR(40) NULL,
        email VARCHAR(120) NULL,
        direccion VARCHAR(160) NULL,
        cuit VARCHAR(20) NULL,
        esta_activo VARCHAR(10) NOT NULL CONSTRAINT DF_proveedor_activo DEFAULT ('Activa')
    );
END;
GO
