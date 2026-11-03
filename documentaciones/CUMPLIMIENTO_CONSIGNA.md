# Cumplimiento de la consigna

Fuente analizada: `Plataformas de desarrollo-final.docx.pdf` (6 páginas).

## Aplicación Desktop

| Requisito | Estado | Implementación |
|---|---|---|
| Login | Cumplido | Acceso por usuario o email. |
| Inicio y navegación | Cumplido | Menú principal de Windows Forms. |
| Dos roles con permisos | Cumplido | Administrador y vendedor; la administración de usuarios queda restringida al administrador. |
| Tabla, paginación y filtros | Cumplido | Listado de productos con búsqueda y paginado. |
| Categorías de dos niveles | Cumplido | Categoría y subcategoría. |
| CRUD | Cumplido | Productos, categorías, subcategorías, usuarios y clientes. |
| Validaciones | Cumplido básico | Controles de datos y consultas parametrizadas. |
| Imágenes | Cumplido | Imágenes asociadas a productos. |
| Venta, stock y comprobante | Cumplido | Venta local, actualización de stock y PDF. |

## Aplicación Web

| Requisito | Estado | Implementación |
|---|---|---|
| Misma base SQL | Cumplido | Usa `anirok2` mediante `ANIROK_DB_CONNECTION`. |
| Login | Cumplido | Sesión ASP.NET; acceso por usuario o email. |
| Registro y unicidad | Cumplido | Validaciones de formulario e índices únicos en SQL Server. |
| Inicio y navegación | Cumplido | Inicio, catálogo, contacto, carrito y cuenta. |
| Contacto | Cumplido | Valida y muestra la consulta en la misma página, sin correo externo. |
| Carrito | Cumplido | Persistencia por sesión, actualización y eliminación por cantidad cero. |
| Compra, stock y comprobante | Cumplido | Transacción SQL, bloqueo de stock, detalle de venta y PDF. |

## Mejoras incorporadas

- SQL parametrizado en toda la Web.
- Validaciones de servidor con Data Annotations.
- Compra atómica: si falla una línea no se guarda la venta ni se descuenta stock.
- La clave de base no queda escrita en la configuración Web.
- Excepciones de negocio visibles sin exponer datos internos.
- Carrito persistido en la sesión de ASP.NET.

## Deuda técnica recomendada

- Migrar las contraseñas históricas en texto plano a hashes compatibles entre Desktop y Web.
- Reemplazar columnas monetarias `FLOAT/REAL` por `DECIMAL(18,2)`.
- Incorporar pruebas automatizadas de integración para compras concurrentes.
- Modernizar gradualmente el acceso a datos compartido por la aplicación Desktop.
