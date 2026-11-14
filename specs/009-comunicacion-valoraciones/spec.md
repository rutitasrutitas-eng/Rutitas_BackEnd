# 009 — Comunicación, notificaciones, favoritos y valoraciones | Especificación funcional

**Proyecto:** Rutitas UTTT — Backend  
**Estado:** PROPUESTO. Pendiente de revisión y aprobación; NO implementado.  
**Fuente primaria:** `Rutitas_Actividad_4.xlsx` y `specs/002-catalogo-funcional/inventario-funcional.md`, operaciones **E34–E41**.  
**Dependencias:** 003 identidad/perfil, 006 rutas/viajes, 007 solicitudes/reservas y 008 seguridad/incidencias.  
**Alcance:** especificación documental; no agrega endpoints a los siete existentes.

## 1. Objetivo y delimitación

Permitir que participantes autorizados de un viaje se comuniquen, consulten avisos, guarden rutas favoritas y valoren trayectos finalizados. **Chat, notificaciones, favoritos y valoraciones no están implementados en el backend de siete endpoints.** El chat no es un canal de emergencia, y el guardado de favoritos no crea reservas.

## 2. Requisitos y criterios de aceptación propuestos

| ID | Origen | Funcionalidad | Criterio de aceptación |
|---|---|---|---|
| CV-01 | E34 | Enviar mensaje | Solo conductor o pasajero elegible de un viaje puede enviar texto acotado; el servidor valida identidad, pertenencia, bloqueo y estado del viaje; emite ID y fecha UTC. |
| CV-02 | E35 | Leer mensajes de viaje | Solo participantes autorizados consultan mensajes ordenados y paginados; se impide leer chats ajenos mediante cambios del `tripId`. |
| CV-03 | E36 | Mis notificaciones | El usuario autenticado consulta exclusivamente sus avisos, con tipo, estado de lectura y paginación. |
| CV-04 | E37 | Marcar notificación leída | Solo el destinatario puede marcarla como leída; reintentos idempotentes, sin alterar avisos ajenos. |
| CV-05 | E38 | Guardar ruta favorita | Usuario guarda una ruta de catálogo existente, sin duplicar la combinación usuario–ruta. |
| CV-06 | E39 | Consultar favoritos | Usuario obtiene solo sus rutas favoritas, con paginación y estado de disponibilidad; un favorito no garantiza viajes disponibles. |
| CV-07 | E40 | Eliminar favorito | Solo dueño elimina favorito; idempotencia a acordar (`204` o `404`) y contrato estable para Android. |
| CV-08 | E41 | Valorar un viaje | Solo participante elegible valora un viaje finalizado; puntuación y comentario opcional conforme a política; evitar valoraciones duplicadas o por viajes ajenos. |
| CV-09 | Transversal | Seguridad, bloqueo y privacidad | Aplicar E44/E45 del módulo 008 a la comunicación; prevenir filtración de conversaciones, notificaciones o valoraciones. |
| CV-10 | Transversal | Coherencia móvil/backend | Prefijo `/api/v1`, IDs como cadenas, autenticación JWT y errores estables; la UI no considerará estas funciones operativas hasta probar integración real. |

## 3. Contratos HTTP candidatos — NO IMPLEMENTADOS

El Excel original utiliza `/v1`; la API actual utiliza `/api/v1`. Se propone este segundo prefijo para mantener consistencia, **sin aprobar todavía rutas ni DTO**.

| Origen | Método | Ruta candidata | Autorización |
|---|---|---|---|
| E34 | POST | `/api/v1/trips/{tripId}/messages` | Participante autorizado |
| E35 | GET | `/api/v1/trips/{tripId}/messages` | Participante autorizado |
| E36 | GET | `/api/v1/me/notifications` | Destinatario autenticado |
| E37 | PATCH | `/api/v1/me/notifications/{id}` | Destinatario autenticado |
| E38 | POST | `/api/v1/me/favorites` | Dueño autenticado |
| E39 | GET | `/api/v1/me/favorites` | Dueño autenticado |
| E40 | DELETE | `/api/v1/me/favorites/{routeId}` | Dueño autenticado |
| E41 | POST | `/api/v1/trips/{tripId}/ratings` | Participante elegible |

**Decisión de contrato:** definir si favoritos contienen solo `routeId` o referencias a viajes concretos; el inventario dice rutas, por lo que no se debe asumir que son viajes publicados. Las rutas existentes `GET /api/v1/routes` y `GET /api/v1/routes/{id}` se conservan.

## 4. Entidades candidatas — NO implementadas

- `TripMessage`: `id`, `tripId`, `senderUserId`, `body`, `createdAtUtc`, `deletedAtUtc?`; jamás aceptar `senderUserId` como identidad confiable enviada por móvil.
- `UserNotification`: `id`, `recipientUserId`, `type`, `referenceId?`, `createdAtUtc`, `readAtUtc?`; la carga útil debe evitar datos sensibles expuestos en notificaciones de pantalla bloqueada.
- `FavoriteRoute`: `userId`, `routeId`, `createdAtUtc`; índice único por `(userId, routeId)`.
- `TripRating`: `id`, `tripId`, `authorUserId`, `targetUserId?`, `score`, `comment?`, `createdAtUtc`; definir quién puede valorar a quién y si procede una valoración por pareja de participantes y viaje.
- `DeviceToken` (**opcional y separado**): solo si Producto aprueba push Android; gestión de consentimiento, registro, rotación, invalidación y borrado de tokens por usuario/dispositivo.

## 5. Reglas de negocio y privacidad

1. La membresía de un chat debe determinarse **en servidor** usando viaje, conductor/reserva y bloqueos. Definir qué sucede antes del viaje, tras cancelación y después de terminarlo.
2. Limitar longitud, frecuencia y volumen de mensajes; moderación y mecanismos para denunciar contenido, coordinados con 008. No registrar mensajes privados completos en logs.
3. Para notificaciones, primero definir eventos de dominio (reserva aceptada/rechazada, viaje cancelado, reporte, mensaje), su audiencia y deduplicación. **Notificación almacenada en backend no es automáticamente push en Android.**
4. Push mediante Firebase Cloud Messaging u otro proveedor es decisión técnica futura; requiere aprobación, configuración segura y pruebas en dispositivo. No almacenar credenciales de proveedor en Git ni enviar JWT en payload.
5. Favoritos: rutas borradas/inactivas deben manejarse de forma predecible y sin revelar datos privados.
6. Valoraciones: solo viajes efectivamente finalizados y con participación verificable; escala candidata 1–5, **pendiente de aprobación**. Decidir edición, borrado, apelación y prevención de represalias.
7. Todos los recursos deben impedir IDOR y acceso entre usuarios; paginación, filtros y límites de respuestas. Retención de mensajes, valoraciones y avisos requiere política de privacidad aprobada.
8. Los avisos de seguridad no sustituyen servicios de emergencia; no prometer comunicación o monitoreo en tiempo real hasta demostrar la capacidad.

## 6. Pruebas y aceptación planeadas

- Sin JWT: `401`; usuario ajeno a viaje o recurso: rechazo consistente con política `403/404`; cuerpos inválidos: `400`; duplicidad de favoritos/valoraciones: `409` o idempotencia documentada; sobreuso: `429` cuando corresponda.
- Chat: perteneciente/no perteneciente, bloqueo, estado de viaje, tamaño y paginación, contenido malicioso, mensajes simultáneos.
- Notificaciones: propiedad, marcar leído repetidas veces, orden, no divulgación de cargas sensibles y deduplicación de eventos.
- Favoritos: unicidad, rutas inexistentes, borradas o privadas, idempotencia y paginación.
- Valoraciones: viaje no finalizado, no participante, duplicado, comentarios inválidos, reglas de visibilidad.
- Pruebas unitarias, integración MongoDB/Testcontainers, contratos OpenAPI 3.1, pruebas de seguridad e integración Android (incluida entrega push si se aprueba).
- Preservar los siete endpoints actuales. Las **63 pruebas ya aprobadas** solo acreditan el backend existente; no estos módulos.

## 7. Decisiones pendientes y gate

1. ¿Quién puede escribir en el chat y cuándo expira el acceso?
2. ¿Se enviarán avisos solo dentro de la aplicación o también mediante push? ¿Qué eventos son obligatorios?
3. ¿Favoritos se aplican a rutas de catálogo o viajes publicados?
4. ¿Quién puede valorar, en qué momento, con qué escala, visibilidad, edición y moderación?
5. ¿Qué política de conservación y eliminación aplicará a mensajes, avisos y valoraciones?
6. ¿Cómo se tratarán bloqueos (008) y reservas canceladas (007) en el chat y las alertas?

**Gate:** revisar y autorizar con Producto/Liderazgo, Backend, Móvil y Seguridad/Privacidad antes de abrir PR funcional o ampliar OpenAPI implementado.
