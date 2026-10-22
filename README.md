# Anirok

Aplicación de escritorio Windows Forms sobre .NET Framework 4.7.2 y SQL Server.

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

4. Abrir `Anirok/EjemploABM.sln` en Visual Studio y ejecutar el proyecto.

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
