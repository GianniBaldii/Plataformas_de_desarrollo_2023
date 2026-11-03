# DER — Base de datos Anirok

Este diagrama representa el esquema SQL Server utilizado por Desktop y Web después de aplicar el script inicial y las migraciones Docker.

```mermaid
erDiagram
    CATEGORIA ||--o{ SUBCATEGORIA : contiene
    CATEGORIA ||--o{ PRODUCTO : clasifica
    SUBCATEGORIA ||--o{ PRODUCTO : agrupa
    CLIENTE ||--o{ VENTA : realiza
    USUARIO ||--o{ VENTA : registra
    VENTA ||--|{ DETALLE_VENTA : incluye
    PRODUCTO ||--o{ DETALLE_VENTA : vendido_en
    PRODUCTO ||--o{ PRODUCTO_HISTORIAL_PRECIO : registra
    USUARIO ||--o{ PRODUCTO_HISTORIAL_PRECIO : modifica

    CATEGORIA {
        int id PK
        varchar nombre
        varchar esta_activo
    }

    SUBCATEGORIA {
        int id PK
        varchar nombre
        int categoria_id FK
        varchar esta_activo
    }

    PRODUCTO {
        int id PK
        varchar nombre
        varchar descripcion
        decimal precio
        int stock
        varchar codigo
        varchar img
        varchar proovedor
        int categoria_id FK
        int subcategoria_id FK
        varchar talle
        decimal costo
        varchar costo_moneda
        decimal costo_pesos
        decimal cotizacion_dolar
    }

    PROVEEDOR {
        int id PK
        varchar nombre
        varchar contacto
        varchar telefono
        varchar email
        varchar direccion
        varchar cuit
        varchar esta_activo
    }

    CLIENTE {
        int id PK
        varchar nombre
        varchar apellido
        varchar mail
        varchar telefono
        varchar direccion
        varchar dni
    }

    USUARIO {
        int id PK
        varchar nombre UK
        varchar apellido
        varchar mail UK
        varchar telefono
        varchar direccion
        varchar dni
        varchar contrasena_hash
        int rol
    }

    VENTA {
        int id PK
        decimal precio_total
        date fecha
        varchar metodo_de_pago
        decimal descuento
        int cliente_id FK
        int usuario_id FK
    }

    DETALLE_VENTA {
        int id PK
        int cantidad
        int venta_id FK
        int producto_id FK
        decimal precio_unitario
    }

    PRODUCTO_HISTORIAL_PRECIO {
        int id PK
        int producto_id FK
        decimal costo_anterior
        decimal costo_nuevo
        decimal precio_anterior
        decimal precio_nuevo
        varchar moneda_costo
        decimal cotizacion_dolar
        datetime fecha_cambio
        int usuario_id FK
        varchar motivo
        varchar origen
    }

    CONTACTO {
        int id PK
        nvarchar asunto
        nvarchar email
        nvarchar mensaje
        datetime fecha
        nvarchar estado
    }

    EMAIL_ENVIO {
        int id PK
        nvarchar tipo
        nvarchar destinatario
        nvarchar asunto
        nvarchar cuerpo
        varbinary adjunto
        nvarchar nombre_adjunto
        nvarchar estado
        nvarchar error
        int intentos
        datetime fecha
        datetime ultimo_intento
    }
```

## Observaciones de implementación

- `VENTA` es la cabecera de la operación y `DETALLE_VENTA` conserva el precio histórico de cada producto vendido.
- `PRODUCTO_HISTORIAL_PRECIO` registra cambios de costo/precio y el usuario que los realizó.
- `PROVEEDOR` existe como ABM, pero la columna histórica `producto.proovedor` todavía guarda el nombre como texto y no una clave foránea. Es una deuda de normalización pendiente.
- `CONTACTO` almacena las consultas enviadas desde la Web.
- `EMAIL_ENVIO` funciona como bandeja de salida y permite reintentar envíos fallidos.
- `rol` identifica los permisos: `1` administrador, `2` vendedor y `3` cliente web.
- Los importes monetarios se almacenan como `DECIMAL(18,2)` luego de la migración `007_monetarios_decimal.sql`.

## Relaciones principales

1. Una categoría puede tener muchas subcategorías y productos.
2. Una venta pertenece a un cliente y a un usuario/vendedor.
3. Una venta tiene uno o más detalles; cada detalle apunta a un producto.
4. Un producto puede tener muchos cambios históricos de precio.
5. Un usuario puede registrar muchos cambios de precios.

