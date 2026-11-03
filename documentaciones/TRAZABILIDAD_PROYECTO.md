# Trazabilidad y evolución del proyecto Anirok

## 1. Introducción

Este documento explica el recorrido del proyecto Anirok desde su desarrollo inicial hasta la versión actual. La intención no es solamente enumerar funcionalidades, sino dejar registro de cómo fue pensado el sistema, qué decisiones se tomaron en su momento, qué dificultades aparecieron al retomarlo y cómo se fue mejorando con los conocimientos adquiridos durante la carrera y la experiencia laboral.

El proyecto se desarrolló originalmente como trabajo práctico de la materia **Plataformas de Desarrollo**. Durante ese período se realizaron revisiones semanales con el profesor, se recibieron observaciones sobre la implementación y se fueron incorporando correcciones de manera incremental. El desarrollo académico quedó funcional, pero el examen final permaneció pendiente desde ese año.

Al retomar el proyecto, aproximadamente tres años después, fue necesario volver a comprender el código, reconstruir el entorno de ejecución y revisar decisiones que habían sido razonables para una primera entrega, pero que podían mejorarse desde una mirada más madura.

## 2. Idea original del sistema

Anirok fue pensado como un sistema de ventas para una tienda de productos de cultura pop: mochilas, bufandas, cartucheras, comics, figuras, peluches y accesorios.

La propuesta original contemplaba dos aplicaciones conectadas a una misma base de datos:

- una aplicación Desktop para administrar productos, categorías, usuarios, stock y ventas en el local;
- una aplicación Web para mostrar el catálogo y permitir que los clientes consulten productos y realicen compras.

La aplicación Desktop fue desarrollada con Windows Forms sobre .NET Framework y la aplicación Web con ASP.NET MVC. SQL Server se utilizó como base común para que el stock y la información de los productos fueran compartidos.

Desde el comienzo se buscó aplicar conceptos vistos en clase: separación entre modelos, controladores y vistas; operaciones ABM; validaciones; relaciones entre tablas; manejo de ventas y detalles de venta; y control de permisos según el usuario.

## 3. Estado del proyecto original

En la versión académica inicial el sistema ya contaba con:

- login de usuarios;
- administración de productos;
- categorías y subcategorías;
- stock;
- clientes;
- armado de ventas;
- tablas `venta` y `detalle_venta`;
- generación inicial de comprobantes;
- una aplicación Web de catálogo.

Sin embargo, la solución tenía características propias de un proyecto en evolución:

- la Web y el Desktop no estaban completamente unificados en el mismo repositorio;
- las imágenes dependían de rutas configuradas manualmente;
- la base almacenaba nombres de archivo, pero no existía una estrategia central de almacenamiento;
- los comprobantes PDF estaban dispersos entre distintas implementaciones;
- los errores de conexión dependían de configuraciones locales antiguas;
- algunas validaciones y mensajes de error eran poco claros;
- la información de precios y costos no tenía trazabilidad;
- los listados tenían estilos y comportamientos diferentes;
- las contraseñas históricas no seguían un esquema seguro de almacenamiento.

Estas decisiones no representan necesariamente errores dentro del contexto original: fueron parte del aprendizaje y de las restricciones de tiempo, conocimientos y alcance de la entrega académica.

## 4. Retomar el proyecto después del período de inactividad

Al volver a trabajar sobre el sistema, el primer desafío fue recuperar el entorno de desarrollo. Se revisaron las versiones de Visual Studio, .NET Framework, Entity Framework, SQL Server, IIS Express y los paquetes utilizados por la Web.

También fue necesario volver a levantar la base de datos y entender qué parte del sistema dependía de una instancia LocalDB instalada en la computadora original. Para hacer el proyecto más reproducible se incorporó Docker Compose con SQL Server.

La nueva forma de trabajo permite:

1. iniciar SQL Server desde un contenedor;
2. crear o actualizar la base mediante scripts y migraciones;
3. conectar Desktop y Web a `localhost:1433`;
4. abrir la solución en Visual Studio;
5. ejecutar la Web con IIS Express y el Desktop desde la misma solución.

El script `scripts/iniciar-proyecto.ps1` automatiza el inicio de Docker, espera a que SQL Server esté saludable y abre Visual Studio con permisos adecuados para IIS Express.

## 5. Migración y consolidación de repositorios

### 5.1. Repositorio original de la Web

La aplicación Web provenía del repositorio:

[https://github.com/VickyTroiano/AnirokWeb](https://github.com/VickyTroiano/AnirokWeb)

Ese repositorio representaba una parte del sistema, principalmente la aplicación Web y sus recursos. Al analizar el proyecto completo se decidió que mantener Desktop y Web separados dificultaba la ejecución, la documentación y la sincronización de cambios.

### 5.2. Repositorio personal actual

El proyecto consolidado se trasladó al repositorio personal:

[https://github.com/GianniBaldii/Plataformas_de_desarrollo_2023](https://github.com/GianniBaldii/Plataformas_de_desarrollo_2023)

La migración no consistió en crear una Web completamente nueva desde cero. Se tomó la aplicación existente como punto de partida, se incorporó dentro de la solución principal y se adaptaron sus rutas, modelos, conexión a base y vistas para que pudiera convivir con Desktop.

### 5.3. Motivos de la consolidación

La unificación permitió:

- tener una única solución de Visual Studio;
- compartir la misma base SQL Server;
- centralizar scripts, migraciones, imágenes y comprobantes;
- evitar duplicar modelos y configuraciones;
- documentar el sistema como un único producto;
- probar el flujo completo desde la administración hasta la compra Web;
- dejar una estructura más clara para la presentación final.

La carpeta `Anirok/AnirokWeb` contiene actualmente la aplicación Web integrada. El código histórico de algunas versiones anteriores se conserva dentro de `Controllers/Legacy` como referencia de migración, sin utilizarse como implementación principal.

## 6. Evolución técnica realizada

### 6.1. Base de datos y entorno

- incorporación de Docker Compose para SQL Server;
- scripts idempotentes de inicialización y migración;
- conexión por `localhost,1433` en lugar de depender de LocalDB;
- separación de configuración sensible mediante variables de entorno;
- migración de importes monetarios a `DECIMAL(18,2)`;
- conservación de scripts originales y migraciones posteriores.

### 6.2. Imágenes de productos

Inicialmente las imágenes dependían de una carpeta configurada manualmente y la Web mantenía otra copia.

Se pasó a un almacenamiento central:

`storage/productos`

Desktop copia allí las imágenes seleccionadas, guarda en SQL el nombre relativo y Web consume esa misma carpeta. También se agregó una imagen predeterminada para los casos en que un archivo no exista.

Esto evita que cada aplicación tenga una fuente diferente y permite respaldar las imágenes junto con el proyecto.

### 6.3. Ventas y comprobantes

El flujo de venta fue reorganizado para que:

- se valide el stock antes de confirmar;
- se bloquee el producto durante la operación;
- se cree la cabecera en `venta`;
- se creen sus líneas en `detalle_venta`;
- se descuente el stock dentro de la misma transacción;
- se conserve el precio histórico vendido;
- se genere un comprobante PDF;
- Desktop y Web utilicen el mismo almacenamiento de comprobantes.

Los PDF se guardan en:

`storage/comprobantes/<año>/<mes>/venta-<id>.pdf`

### 6.4. Precios, costos y trazabilidad

Se incorporó un módulo de **Precios y Costos** para separar el precio de venta del costo del producto. El módulo permite:

- editar costos;
- aplicar aumentos porcentuales;
- consultar la cotización del dólar;
- registrar la moneda del costo;
- conservar un historial de modificaciones;
- visualizar el margen estimado;
- identificar si el producto genera ganancia, se vende al costo o produce pérdida.

Esta funcionalidad surge de una necesidad concreta de gestión y también de una mejora conceptual: el precio actual no debería sobrescribir la historia de cómo fue calculado.

### 6.5. Proveedores

Se agregó un ABM de proveedores y una asignación inicial por categoría. Esto permite ampliar el sistema desde un catálogo de productos hacia una herramienta de gestión más cercana a la operación real de un comercio.

Como deuda técnica, la base todavía conserva en `producto.proovedor` el nombre del proveedor como texto histórico. El modelo futuro debería reemplazarlo por `proveedor_id` como clave foránea.

### 6.6. Usuarios, roles y seguridad

Se formalizaron tres perfiles:

- **Administrador:** administra usuarios, catálogo, precios, costos, proveedores y ventas.
- **Vendedor:** trabaja con productos, stock y ventas del local.
- **Cliente Web:** se registra, inicia sesión, consulta el catálogo y compra desde la Web.

También se eliminó la exposición de contraseñas en los listados, se incorporaron acciones de reseteo y se preparó el almacenamiento mediante hash para las nuevas credenciales.

### 6.7. Contacto y correos

La Web ahora registra las consultas de contacto en la tabla `contacto`. Los envíos de correo se registran en `email_envio`, incluyendo estado, cantidad de intentos, error y adjunto. Esto permite consultar desde Desktop qué mensajes fallaron y reintentarlos.

La misma infraestructura puede utilizarse para enviar comprobantes de compra y notificaciones de reseteo de contraseña.

### 6.8. Interfaz y experiencia de uso

Además de corregir errores funcionales, se revisó la experiencia de uso:

- encabezados más compactos;
- listados visualmente unificados;
- paginación;
- búsquedas;
- mensajes de error más claros;
- botones con acciones consistentes;
- mejoras en el armador de ventas;
- carrito Web con cantidades, resumen y confirmación;
- correcciones de codificación para evitar textos con caracteres ilegibles;
- incorporación gradual de estilos Tailwind en la Web.

## 7. Aprendizajes incorporados

La segunda etapa del proyecto fue posible porque se pudo observar el código original con una mirada diferente a la del primer desarrollo. Los conocimientos de la carrera ayudaron a revisar:

- modelado relacional;
- responsabilidades entre capas;
- transacciones;
- validaciones;
- control de acceso;
- trazabilidad de cambios;
- documentación técnica.

La experiencia laboral aportó una mirada adicional sobre:

- mantenibilidad;
- configuración reproducible;
- separación entre código y secretos;
- mensajes orientados al usuario;
- manejo de errores;
- almacenamiento de archivos;
- consistencia entre aplicaciones;
- importancia de poder levantar el sistema en otra computadora.

Por ese motivo, la evolución no se plantea como una crítica al trabajo inicial, sino como una segunda etapa del mismo aprendizaje.

## 8. Estado actual

Actualmente Anirok se organiza como una solución con dos aplicaciones:

```text
Anirok/
├── EjemploABM/       Desktop Windows Forms
├── AnirokWeb/        Web ASP.NET MVC 5
└── EjemploABM.sln    Solución unificada
```

La base se inicializa mediante Docker y ambas aplicaciones consumen SQL Server. Las imágenes y comprobantes se almacenan de forma centralizada. La documentación, los scripts y las migraciones se encuentran versionados junto con el código.

## 9. Pendientes y límites conocidos

El proyecto mejoró considerablemente, pero todavía conserva puntos que pueden desarrollarse en etapas futuras:

- reemplazar el proveedor textual por una relación formal con `proveedor_id`;
- completar la migración de todas las contraseñas históricas a hash;
- incorporar pruebas automatizadas de integración;
- revisar el uso de `MAX(id) + 1` y migrar a columnas `IDENTITY`;
- agregar cancelación de ventas con reposición transaccional de stock;
- separar definitivamente configuraciones de desarrollo y producción;
- incorporar un servicio de correo real mediante credenciales configuradas fuera del repositorio.

## 10. Cierre

Anirok comenzó como un proyecto académico desarrollado con acompañamiento semanal y un alcance orientado a demostrar los conceptos principales de una aplicación de ventas. Al retomarlo tiempo después, se transformó en una oportunidad para aplicar esos mismos conceptos con mayor criterio técnico.

La consolidación de repositorios, Docker, la centralización de archivos, el historial de precios, los roles, la trazabilidad de ventas, la gestión de proveedores y la documentación actual no reemplazan el proyecto original: muestran su evolución.

El resultado final representa el paso de una entrega académica funcional a una solución más mantenible, reproducible y cercana a un sistema que podría continuar desarrollándose en un contexto real.

