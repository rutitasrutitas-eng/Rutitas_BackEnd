# MAPS-001 - Matriz de pruebas candidatas

Estado general: NOT_RUN

Todas las pruebas iniciales deben usar datos sinteticos
y dependencias simuladas, sin consumo de Azure.

| ID | Escenario | Resultado esperado |
|---|---|---|
| MAP-V01 | Busqueda valida | 200 |
| MAP-V02 | Busqueda vacia o invalida | 400 |
| MAP-V03 | Busqueda sin resultados | 200 lista vacia |
| MAP-V04 | Geocodificacion valida | 200 |
| MAP-V05 | Direccion no encontrada | 404 |
| MAP-V06 | Geocodificacion inversa | 200 |
| MAP-V07 | Coordenadas fuera de rango | 400 |
| MAP-V08 | Recorrido valido | 200 |
| MAP-V09 | Waypoints superiores al limite | 400 |
| MAP-V10 | Recorrido no encontrado | 404 |
| MAP-V11 | DTO y GeoJSON validos | Esquema conforme |
| MAP-V12 | JWT ausente o invalido | 401 |
| MAP-V13 | Usuario sin autorizacion | 403 |
| MAP-V14 | Rate limit excedido | 429 |
| MAP-V15 | Proveedor indisponible | 503 |
| MAP-V16 | Timeout del proveedor | 504 |
| MAP-V17 | Cancelacion de solicitud | Cancelacion propagada |
| MAP-V18 | Identidad no configurada | Sin acceso real |
| MAP-V19 | Tokens y secretos | No expuestos |
| MAP-V20 | Logs de ubicacion | Datos minimizados |
| MAP-V21 | Error no transitorio | Sin reintento |
| MAP-V22 | Resultados del proveedor | Sin persistencia |
| MAP-V23 | Proveedor real deshabilitado | Cero llamadas |
| MAP-V24 | Respuesta upstream invalida | 502 |

## Evidencia requerida

- Identificador de prueba.
- Resultado esperado y obtenido.
- SHA del commit evaluado.
- Entorno utilizado.
- Reporte automatizado.
- Confirmacion de ausencia de llamadas reales.

Esta matriz no constituye evidencia de pruebas aprobadas.
