# Anirok

Sistema de ventas con dos aplicaciones conectadas a la misma base SQL Server:

- **Anirok Desktop:** Windows Forms sobre .NET Framework 4.7.2 para administración, stock y ventas locales.
- **Anirok Web:** la aplicación ASP.NET MVC 5 original de `VickyTroiano/AnirokWeb`, migrada a esta solución y adaptada para catálogo, registro, carrito y compras.

## Levantar la base con Docker

1. Iniciar Docker Desktop usando contenedores Linux.
2. Desde la raíz del repositorio, ejecutar:

   ```powershell
   docker compose up -d
   ```

3. Comprobar que SQL Server esté listo y que la inicialización haya terminado:

   ```powershell
   docker compose ps
   docker compose logs db-init
   ```

4. Abrir `Anirok/EjemploABM.sln` en Visual Studio para trabajar con ambos proyectos.

La aplicación se conecta por defecto a `localhost,1433`, base `anirok2`, usuario `sa` y contraseña `Anirok_dev_2026!`. Para usar otra conexión, definir la variable de entorno `ANIROK_DB_CONNECTION` antes de iniciar Visual Studio:

```powershell
$env:ANIROK_DB_CONNECTION = "Server=localhost,1433;Database=anirok2;User Id=sa;Password=otra-clave;TrustServerCertificate=True"
```

Usuarios de prueba:

- Administrador: `Gianni` / `gianni123`
- Vendedor: `Victoria` / `vic345`

En el primer ingreso la aplicación avisa que faltan las carpetas de imágenes y PDF. Se configuran desde el botón **Setear URL** usando estas carpetas del proyecto:

- `Anirok/EjemploABM/Recursos/img`
- `Anirok/EjemploABM/Recursos/pdf_ventas`

Para detener la base sin borrar sus datos:

```powershell
docker compose down
```

Los datos quedan guardados en el volumen `anirok-sqlserver-data`.

## Ejecutar la aplicación Web

La Web está en `Anirok/AnirokWeb` y usa .NET Framework 4.7.2, Entity Framework 6 y ASP.NET MVC 5. No es la aplicación ASP.NET Core creada durante el primer intento de migración.

Antes de abrir Visual Studio desde PowerShell, en la raíz del repositorio:

```powershell
$passwordLine = Get-Content .env.example | Where-Object { $_ -like "MSSQL_SA_PASSWORD=*" } | Select-Object -First 1
$password = $passwordLine.Substring("MSSQL_SA_PASSWORD=".Length)
$env:ANIROK_DB_CONNECTION = "Server=localhost,1433;Database=anirok2;User Id=sa;Password=$password;TrustServerCertificate=True"
Start-Process "C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\devenv.exe" "Anirok\EjemploABM.sln"
```

En Visual Studio, seleccionar `AnirokWeb` como proyecto de inicio y ejecutarlo con IIS Express. Si IIS Express no aparece como opción, agregar la carga de trabajo **Desarrollo de ASP.NET y web** desde Visual Studio Installer.

La Web migrada incluye:

- registro con validación de usuario/email únicos;
- login por usuario o email;
- catálogo con buscador y filtro por categoría;
- detalle e imágenes de productos;
- contacto con simulación del mensaje enviado;
- carrito en sesión;
- compra transaccional que valida y descuenta stock;
- comprobante PDF descargable.

La cadena de conexión no se guarda dentro del proyecto Web: se recibe mediante `ANIROK_DB_CONNECTION`.

## Compatibilidad con Visual Studio

- El proyecto Desktop requiere las herramientas de desarrollo de escritorio de .NET y el targeting pack de .NET Framework 4.7.2.
- El proyecto Web requiere Visual Studio 2022 con las herramientas de ASP.NET clásico/IIS Express y el targeting pack de .NET Framework 4.7.2.

El detalle de cobertura está en [CUMPLIMIENTO_CONSIGNA.md](CUMPLIMIENTO_CONSIGNA.md).
