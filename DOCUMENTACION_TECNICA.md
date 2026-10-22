# Documentación Técnica — Sistema Administrativo

## Arquitectura

Se implementó una arquitectura limpia de 4 capas con dependencias unidireccionales:

Web -> Application -> Core <- Infrastructure

### Por qué esta arquitectura

- `Core` no depende de ninguna tecnología externa — las entidades y contratos 
  son estables ante cambios de framework o base de datos
- `Infrastructure` puede reemplazarse (ej. cambiar MySQL por PostgreSQL) 
  sin tocar la lógica de negocio
- `Application` concentra todas las validaciones y reglas de negocio 
  en un solo lugar testeable

## Decisiones de diseño

### ADO.NET sobre Entity Framework
Se eligió ADO.NET para demostrar dominio directo del acceso a datos. 
Permite control total sobre las queries ejecutadas y facilita la optimización 
de consultas críticas como la generación de facturas.

### Transacción en facturación
La emisión de una factura ejecuta 3 operaciones en una sola transacción:
1. Insertar cabecera de factura
2. Insertar cada línea de detalle
3. Descontar stock por producto

Si cualquier operación falla, la transacción se revierte completamente. 
Esto garantiza que nunca quede una factura sin detalles ni stock descontado 
sin factura correspondiente.

### ISV persistido en base de datos
El ISV se calcula y guarda en la tabla `Facturas` en lugar de recalcularse 
en cada consulta. Esto permite auditar facturas históricas correctamente 
aunque la tasa impositiva cambie en el futuro.

### Número de factura correlativo
El número de factura se genera como `FACT-000001` mediante una tabla de correlativos
actualizada dentro de la misma transacción. La actualización bloquea la fila del
correlativo mientras la transacción está activa, evitando que dos facturas concurrentes
reciban el mismo número.

### Integridad de precios y stock
El precio utilizado para facturar se obtiene nuevamente desde la base de datos y no se
confía en el valor recibido desde la interfaz. Los productos repetidos se consolidan antes
de calcular la factura.

El descuento de inventario se realiza con una actualización condicional
(`Stock >= Cantidad`) dentro de la transacción. Si otro proceso consume el stock antes,
la operación falla y toda la factura se revierte.

## Respuesta a pregunta de análisis

### ¿Por qué el módulo de facturas puede volverse lento con el tiempo?

A medida que la tabla `Facturas` y `FacturaDetalles` crecen, 
las consultas pueden degradarse por las siguientes razones:

**1. Ausencia de índices en columnas de filtro frecuente**
Si se consulta por rango de fechas o por cliente sin índice, 
MySQL realiza un *full table scan* — recorre toda la tabla fila por fila.
*Solución:* Índices en `Facturas.Fecha` y `Facturas.ClienteId` 
(ya incluidos en el schema).

**2. JOINs sin índices en claves foráneas**
El JOIN entre `FacturaDetalles` y `Productos` en los reportes puede ser 
costoso si `ProductoId` no tiene índice.
*Solución:* Índice en `FacturaDetalles.ProductoId` (ya incluido).

**3. Crecimiento sin paginación**
Traer todas las facturas en una sola query sin LIMIT/OFFSET 
significa cargar potencialmente millones de registros en memoria.
*Solución:* Implementar paginación en la lista de facturas 
(mejora futura recomendada).

**4. Cálculos agregados en reportes sin caché**
Los reportes de top productos y clientes recalculan SUM/COUNT 
sobre toda la historia cada vez que se consultan.
*Solución:* Implementar vistas materializadas o caché de resultados 
con invalidación periódica.

**5. Conexiones no reutilizadas**
Si se abren múltiples conexiones simultáneas sin pool de conexiones, 
el servidor MySQL puede saturarse.
*Solución:* MySqlConnector maneja connection pooling automáticamente 
con la configuración actual.

## Mejoras futuras recomendadas

- Paginación en listados de facturas y clientes
- Autenticación JWT para proteger los módulos
- Exportación de reportes a PDF o Excel
- Filtros por rango de fecha en el módulo de facturación
- Soft delete en lugar de cambio de estado para clientes