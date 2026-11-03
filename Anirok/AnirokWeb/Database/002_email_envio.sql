IF OBJECT_ID('dbo.email_envio', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.email_envio
    (
        id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_email_envio PRIMARY KEY,
        tipo NVARCHAR(30) NOT NULL,
        destinatario NVARCHAR(150) NOT NULL,
        asunto NVARCHAR(200) NOT NULL,
        cuerpo NVARCHAR(MAX) NOT NULL,
        adjunto VARBINARY(MAX) NULL,
        nombre_adjunto NVARCHAR(160) NULL,
        estado NVARCHAR(20) NOT NULL CONSTRAINT DF_email_envio_estado DEFAULT 'Pendiente',
        error NVARCHAR(1000) NULL,
        intentos INT NOT NULL CONSTRAINT DF_email_envio_intentos DEFAULT 0,
        fecha DATETIME2 NOT NULL CONSTRAINT DF_email_envio_fecha DEFAULT SYSDATETIME(),
        ultimo_intento DATETIME2 NULL
    );
END;
