# Diseño de ventas y detalles

## Situación actual

La base ya contiene las tablas `venta` y `detalle_venta`. La Web registra una compra desde `CompraService` dentro de una única transacción:

1. Obtiene o crea el cliente asociado al usuario.
2. Lee cada producto con bloqueo de actualización (`UPDLOCK`).
3. Verifica el stock disponible.
4. Inserta la cabecera en `venta`.
5. Inserta un renglón por producto en `detalle_venta`.
6. Guarda `precio_unitario` como fotografía histórica del precio.
7. Descuenta el stock y confirma la transacción.

La separación buscada es:

`Controller -> CompraService -> transacción SQL -> venta + detalle_venta + producto`

El controlador administra HTTP y sesión. El servicio concentra las reglas de negocio. La base garantiza consistencia mediante claves, restricciones y una transacción.

## Responsabilidad de cada tabla

### `venta`

Representa la cabecera de la operación:

- identificador de venta;
- fecha y hora;
- cliente y usuario/vendedor;
- método de pago;
- subtotal, descuento y total final;
- estado de la venta.

### `detalle_venta`

Representa los renglones vendidos:

- venta a la que pertenece;
- producto vendido;
- cantidad;
- precio unitario histórico;
- subtotal calculado como `cantidad * precio_unitario`.

El subtotal no necesita guardarse porque se deriva sin perder información. El nombre y demás datos actuales del producto se consultan desde `producto`; el precio sí debe quedar congelado en el detalle.

## Modelo objetivo

```sql
CREATE TABLE venta (
    id INT IDENTITY(1,1) PRIMARY KEY,
    fecha DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    cliente_id INT NOT NULL,
    usuario_id INT NOT NULL,
    metodo_de_pago VARCHAR(45) NOT NULL,
    subtotal DECIMAL(18,2) NOT NULL,
    descuento DECIMAL(18,2) NOT NULL DEFAULT 0,
    precio_total DECIMAL(18,2) NOT NULL,
    estado VARCHAR(20) NOT NULL DEFAULT 'CONFIRMADA',
    CONSTRAINT CK_venta_importes CHECK (
        subtotal >= 0 AND descuento >= 0 AND precio_total >= 0
    ),
    CONSTRAINT CK_venta_estado CHECK (
        estado IN ('PENDIENTE', 'CONFIRMADA', 'CANCELADA')
    ),
    FOREIGN KEY (cliente_id) REFERENCES cliente(id),
    FOREIGN KEY (usuario_id) REFERENCES usuario(id)
);

CREATE TABLE detalle_venta (
    id INT IDENTITY(1,1) PRIMARY KEY,
    venta_id INT NOT NULL,
    producto_id INT NOT NULL,
    cantidad INT NOT NULL,
    precio_unitario DECIMAL(18,2) NOT NULL,
    CONSTRAINT CK_detalle_cantidad CHECK (cantidad > 0),
    CONSTRAINT CK_detalle_precio CHECK (precio_unitario >= 0),
    CONSTRAINT UQ_detalle_venta_producto UNIQUE (venta_id, producto_id),
    FOREIGN KEY (venta_id) REFERENCES venta(id),
    FOREIGN KEY (producto_id) REFERENCES producto(id)
);
```

## Cambios recomendados por etapas

1. Mantener el flujo transaccional actual y mapear `Venta`/`DetalleVenta` como modelos del dominio Web.
2. Reemplazar `REAL` por `DECIMAL(18,2)` para importes.
3. Reemplazar `DATE` por `DATETIME2` para conservar la hora.
4. Agregar restricciones `NOT NULL`, `CHECK` y el estado de venta.
5. Migrar las claves a `IDENTITY` y eliminar todos los usos de `MAX(id) + 1`.
6. Hacer que Desktop use el mismo servicio/reglas transaccionales o una API compartida.
7. Incorporar cancelación: cambia el estado y repone stock dentro de otra transacción; no elimina físicamente la venta.

La migración a `IDENTITY` debe hacerse en una etapa controlada porque SQL Server no permite convertir directamente una columna existente en identidad. Requiere crear tablas nuevas, copiar datos, recrear claves y renombrar.
