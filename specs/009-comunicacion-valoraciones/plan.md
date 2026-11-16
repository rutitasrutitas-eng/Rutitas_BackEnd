# 009 — Plan SDD | Comunicación y valoraciones

**Estado:** PROPUESTO · solo diseño · ejecución no autorizada.  
**Trazabilidad:** E34, E35, E36, E37, E38, E39, E40, E41.  
**Dependencias:** 003, 006, 007 y 008.

## 1. Secuencia de diseño

1. Confirmar con Producto el mapa de experiencia de Móvil: chat de viaje, bandeja de avisos, favoritos y valoración posterior al trayecto.
2. Unificar reglas de identidad y participación por viaje (006/007); incorporar bloqueos y reportes del módulo 008.
3. Validar nombres de rutas, identificadores `string`, JWT, paginación y formatos de error con el equipo Android.
4. Definir estados, restricciones y retención de cada entidad; distinguir **notificaciones internas** de **notificaciones push**.
5. Diseñar eventos que originan avisos y la estrategia de entrega/reintentos sin duplicar avisos.
6. Evaluar si push es necesario para el piloto y qué credenciales/consentimientos se requieren; no incorporarlo implícitamente a la API.
7. Diseñar índices MongoDB, auditoría y controles de concurrencia; evitar consultas que permitan leer recursos ajenos.
8. Establecer escenarios de pruebas de seguridad, integración y contrato, incluidas reglas de bloqueo y duplicidad.
9. Tras aprobación funcional y técnica, preparar OpenAPI 3.1 y plan de implementación; mantener intactos los siete endpoints en funcionamiento.

## 2. Arquitectura candidata

| Capa | Diseño previsto (aún no implementado) |
|---|---|
| `Rutitas.Api` | Controladores `/trips/{tripId}/messages`, `/me/notifications`, `/me/favorites`, `/trips/{tripId}/ratings`; JWT y rate limits |
| `Rutitas.Application` | Casos de uso, autorización por participación, reglas de bloqueo, eventos de notificación y validación de valoración |
| `Rutitas.Domain` | Modelos de mensajes, notificaciones, favoritos y valoraciones, transiciones y reglas independientes de infraestructura |
| `Rutitas.Infrastructure` | Repositorios MongoDB, índices compuestos, transiciones atómicas; integración futura con proveedor push si se aprueba |
| Pruebas | Unidades, contratos, integración con Testcontainers, seguridad y validación Android |

**No se recomienda integrar un servicio de mensajería en tiempo real sin requerimiento y presupuesto aprobados.** Para un piloto, primero debe decidirse si lectura periódica con paginación es suficiente.

## 3. Entregables y criterios de salida

| Etapa | Entregable | Salida requerida |
|---|---|---|
| A | Matriz E34–E41 | Todas las operaciones mapeadas una sola vez |
| B | Reglas de acceso, notificación y valoración | Validadas por Producto y Seguridad |
| C | DTO, errores y contrato REST propuesto | Aceptación Móvil + Backend |
| D | Modelo de datos, índices, retención | Revisión de privacidad y concurrencia |
| E | Plan de pruebas y amenazas | Incluye IDOR, bloqueo, duplicados y datos privados |
| F | Gate documental | Aprobación explícita antes de implementar |

## 4. Riesgos a tratar

- **Chat ajeno:** autorización del servidor por participación; no confiar en identificadores suministrados por Android.
- **Spam y abuso:** límites de envío, reglas de bloqueo, reportes y moderación.
- **Push inseguro:** credenciales solo en entorno seguro y payload mínimo; consentimiento/rotación de tokens.
- **Duplicidad de avisos y valoraciones:** índices y operaciones idempotentes/atómicas según contrato.
- **Privacidad y conservación:** límites de acceso, caducidad y solicitudes de eliminación.
- **Acoplamiento excesivo:** no mezclar funciones de alertas, chats y reservas en un único controlador sin justificación.

## 5. Exclusiones

No implementar código, modificar `Program.cs`, MongoDB Atlas, archivos de configuración, OpenAPI operativo, GitHub Actions, recursos Azure ni publicar push. El código existente con siete endpoints permanece sin cambios.
