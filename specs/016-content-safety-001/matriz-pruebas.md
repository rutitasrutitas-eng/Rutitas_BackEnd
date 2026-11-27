# CONTENT-SAFETY-001 - Matriz de pruebas

Estado general: NOT_RUN

| ID | Escenario | Resultado esperado |
|---|---|---|
| CS-V01 | Texto permitido | approved |
| CS-V02 | Texto severidad intermedia | pending_review |
| CS-V03 | Texto severidad alta | rejected |
| CS-V04 | Imagen permitida | approved |
| CS-V05 | Imagen ambigua | pending_review |
| CS-V06 | Imagen severidad alta | rejected |
| CS-V07 | Texto vacio | 400 |
| CS-V08 | Texto supera limite | 400 |
| CS-V09 | Contexto invalido | 400 |
| CS-V10 | Imagen demasiado grande | 413 |
| CS-V11 | Formato no permitido | 415 |
| CS-V12 | Imagen corrupta | 400 |
| CS-V13 | JWT ausente | 401 |
| CS-V14 | JWT invalido | 401 |
| CS-V15 | Recurso de otro usuario | 403 |
| CS-V16 | Rate limit excedido | 429 |
| CS-V17 | Respuesta upstream invalida | 502 sin publicacion |
| CS-V18 | Proveedor indisponible | 503 sin publicacion |
| CS-V19 | Timeout upstream | 504 sin publicacion |
| CS-V20 | Cancelacion de solicitud | Cancelacion propagada |
| CS-V21 | Estado manipulado por cliente | No aprobado |
| CS-V22 | Cambio posterior de contenido | Nueva evaluacion |
| CS-V23 | Publicacion pendiente | Bloqueada |
| CS-V24 | Publicacion rechazada | Bloqueada |
| CS-V25 | Logs y credenciales | Sin exposicion |
| CS-V26 | Proveedor real deshabilitado | Cero llamadas |
| CS-V27 | Severidad desconocida | Sin aprobacion |
| CS-V28 | Categoria ausente | Sin aprobacion |
| CS-V29 | Imagen con URL arbitraria | Rechazada |
| CS-V30 | Revision humana no autorizada | 403 |

Todas las pruebas funcionales requieren una
implementacion posterior autorizada.

Los ejemplos iniciales seran sinteticos.
No utilizar fotografias ni textos de alumnos reales.
