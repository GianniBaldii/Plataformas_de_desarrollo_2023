IF OBJECT_ID('dbo.contacto', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.contacto
    (
        id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_contacto PRIMARY KEY,
        asunto NVARCHAR(80) NOT NULL,
        email NVARCHAR(150) NOT NULL,
        mensaje NVARCHAR(MAX) NOT NULL,
        fecha DATETIME2 NOT NULL CONSTRAINT DF_contacto_fecha DEFAULT SYSDATETIME(),
        estado NVARCHAR(20) NOT NULL CONSTRAINT DF_contacto_estado DEFAULT 'Pendiente'
    );
END;
