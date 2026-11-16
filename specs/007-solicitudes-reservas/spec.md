# 007 — Solicitudes, reservas y asignación de asientos | Especificación funcional

**Proyecto:** Rutitas UTTT — Backend  
**Estado:** PROPUESTO · pendiente de revisión y aprobación · sin implementación autorizada.  
**Dependencias:** 002 catálogo funcional; 003 identidad/perfil; 004 comunidades/verificación; 005 conductores; 006 rutas/viajes.  
**Fuentes:** `Rutitas_Actividad_4.xlsx` (E23–E29, N10–N11), `002-catalogo-funcional/inventario-funcional.md` y separación Route/RideOffer propuesta en 006.

## 1. Estado técnico y alcance

Los **siete endpoints existentes** del backend NO ofrecen solicitudes de asiento ni reservas. `GET /api/v1/routes` y `GET /api/v1/routes/{id}` consultan **rutas de catálogo**, que carecen de fecha, conductor y cupos. No se deben reservar asientos sobre una ruta estática ni reutilizar su `routeId` como si fuera el identificador de un viaje ofertado.

Este módulo **especifica**: pedir asiento en una oferta de viaje, revisar solicitudes, aceptar/rechazar, cancelar una reserva y consultar reservas; propone contratos para cotización y comprobante solo si negocio los confirma. No crea controladores, colecciones ni modifica OpenAPI operativo.

## 2. Catálogo de requisitos y criterios de aceptación

| ID | Origen | Operación | Criterio de aceptación propuesto |
|---|---|---|---|
| SR-01 | E23 | Solicitar asiento | Pasajero autenticado y habilitado; oferta publicada y futura; sin solicitud activa duplicada; número de asientos válido; registra solicitud propia sin confirmar automáticamente. |
| SR-02 | E24 | Consultar mis solicitudes | Solo solicitudes del usuario autenticado; paginación, estado y orden estable. |
| SR-03 | E25 | Consultar solicitudes recibidas | Solo propietario autorizado de la oferta; datos personales mínimos y paginación. |
| SR-04 | E26 | Aceptar solicitud | Solo conductor dueño de la oferta; verifica cupo real de manera atómica antes de confirmar; rechazo controlado si no hay asientos. |
| SR-05 | E27 | Rechazar solicitud | Solo conductor autorizado; transición válida, motivo opcional controlado y auditoría. |
| SR-06 | E28 | Cancelar reserva | Pasajero propietario o administrador autorizado; libera asientos exactamente una vez, conforme a ventana de cancelación aprobada. |
| SR-07 | E29 | Consultar mis reservas | Solo reservas propias, con estado, oferta y horarios pertinentes, sin revelar contactos privados. |
| SR-08 | N10 | Cotizar reserva | **Por decidir** si existe cobro/aportación; sin autorización de pagos, devolver solo estimaciones permitidas o excluir este endpoint. |
| SR-09 | N11 | Comprobante | **Por decidir** si se necesita comprobante; acceso únicamente a participante autorizado, con información mínima y sin confundir con comprobante fiscal. |
| SR-10 | transversal | Idempotencia y concurrencia | Reintentos, solicitudes simultáneas y cancelaciones repetidas no generan duplicados ni sobreventa. |

## 3. Vocabulario y modelo propuestos (NO implementados)

- `RideOffer`: publicación de viaje con fecha, conductor, vehículo y `seatsTotal`; se define en 006.
- `SeatRequest`: solicitud de pasajero: `id`, `rideId`, `passengerId`, `requestedSeats`, `status`, `createdAtUtc`, `decidedAtUtc`, `version`.
- `Booking`: reserva confirmada: `id`, `requestId`, `rideId`, `passengerId`, `confirmedSeats`, `status`, `confirmedAtUtc`, `cancelledAtUtc`, `version`.
- `seatsAvailable`: valor derivado/controlado **por el servidor**; jamás aceptar como verdad el valor que mande Android.
- Los identificadores expuestos serán `string`; validar formato ObjectId si se adopta MongoDB ObjectId para las nuevas colecciones.

**Decisión pendiente:** confirmar si la aprobación crea un documento `Booking` independiente o si `SeatRequest(status=accepted)` representa la reserva. No desarrollar ambos modelos como sinónimos sin acuerdo.

## 4. Estados y transiciones sugeridas

- Solicitud: `pending` → `accepted` **o** `rejected` **o** `cancelled`; no aceptar después de rechazo/cancelación.
- Reserva: `confirmed` → `cancelled` / `completed` según estado del viaje y reglas aprobadas.
- Oferta cancelada: no admite nuevas solicitudes y requiere política explícita para reservas anteriores.
- Una solicitud no puede confirmar más asientos que los disponibles ni más que la capacidad real del vehículo.
- Un pasajero no puede reservar su propia oferta; acordar otras restricciones (membresía, suspensión, duplicados, máximo de plazas).

## 5. Borrador de API para alinear con Móvil

El Excel propone rutas `/v1/...`; **el backend actual usa `/api/v1`**. La tabla siguiente propone normalizar a `/api/v1/rides`, sujeto a aprobación de 006 y de los responsables:

| Fuente | Método | Ruta candidata, NO implementada | Nota |
|---|---|---|---|
| E23 | POST | `/api/v1/rides/{rideId}/requests` | No usar ID de ruta de catálogo. |
| E24 | GET | `/api/v1/me/requests` | Paginación. |
| E25 | GET | `/api/v1/rides/{rideId}/requests` | Solo conductor propietario. |
| E26 | POST | `/api/v1/requests/{requestId}/accept` | Confirmación atómica. |
| E27 | POST | `/api/v1/requests/{requestId}/reject` | Validación de estado. |
| E28 | POST | `/api/v1/bookings/{bookingId}/cancel` | Liberación idempotente. |
| E29 | GET | `/api/v1/me/bookings` | Solo reservas propias. |
| N10 | POST | `/api/v1/rides/{rideId}/booking-quote` | Solo si hay alcance de aportaciones/costo aprobado. |
| N11 | GET | `/api/v1/bookings/{bookingId}/receipt` | Solo si existe comprobante de reserva aprobado. |

No agregar estas rutas al OpenAPI actual ni dar por compatibles las pantallas de Android mientras no exista contrato final con DTO, autenticación y errores definidos.

## 6. Seguridad, privacidad, concurrencia e integridad

1. JWT Bearer en todas las operaciones; autorización de propiedad, permisos de conductor, usuario verificado y estado de cuenta **en servidor**.
2. Evitar IDOR: un usuario no podrá leer ni alterar solicitudes/reservas ajenas por cambiar IDs.
3. Confirmación concurrente: actualizar cupo mediante condición atómica (p. ej. compare-and-set sobre cupo/version) y confirmar la transición exactamente una vez. **Si la operación involucra varias colecciones, estudiar transacciones MongoDB y requisitos del clúster**; definir comportamiento ante fallos parciales antes de implementar.
4. Idempotencia: acordar `Idempotency-Key` para `POST` de creación/aceptación o índices únicos + control de estado; las claves deben vincularse a identidad y operación.
5. Índices únicos/compuestos para evitar dos solicitudes activas equivalentes según política; nunca depender solo de validaciones en la aplicación.
6. En cancelaciones y errores de red, evitar liberar dos veces asientos o confirmar una reserva invisible al pasajero.
7. No enviar teléfono, correo institucional, ubicación exacta u otros datos personales sin justificación y control de acceso aprobados. Registrar auditoría sin JWT ni información sensible.
8. Definir límites por usuario, rate limiting, expiración de solicitudes, ventanas de cancelación, lista de espera y política de no-show antes de implementar.

## 7. Códigos y pruebas esperadas (borrador)

- Crear solicitud: `201`; datos inválidos `400`; sin JWT `401`; no autorizado `403`; viaje ausente `404`; duplicado/estado incompatible `409`.
- Aceptar solicitud: éxito `200` o `204` **por acordar**; conflicto de cupo o estado `409`; acceso indebido `403` o `404` según política de privacidad.
- Cancelar reserva: éxito `200`/`204` **por acordar**; reintentos deben ser seguros sin liberar cupos dos veces.
- Conservar los siete endpoints actuales y sus 63 pruebas de referencia. Agregar pruebas unitarias y de contrato, seguridad (IDOR), integración con MongoDB/Testcontainers, simultaneidad de aceptación, reintentos y compensación/rollback.
- Ensayar dos pasajeros compitiendo por el último asiento: **máximo uno confirmado**, sin valores negativos. Validar que cancelación concurrente no incremente cupos por encima del total.

## 8. Preguntas bloqueantes para aprobación

1. ¿Reserva por solicitud/aceptación del conductor, inmediata, o ambas?
2. ¿Una solicitud puede pedir varios asientos? ¿Cuántos como máximo?
3. ¿Qué ocurre con reservas al editar/cancelar viaje, suspender conductor o vencer documentos?
4. ¿Se permite lista de espera y expiración automática? ¿Con qué plazos?
5. ¿Existen aportaciones económicas? ¿N10 queda dentro del alcance MVP?
6. ¿Las reservas requieren comprobante N11 y qué información puede mostrarse?
7. ¿Se unifica `requests` y `bookings` en almacenamiento? ¿Qué nombres/DTO acuerdan Backend y Android?

**Gate:** Producto/liderazgo, Backend, Móvil y Seguridad/Privacidad revisan y autorizan antes de crear código, migraciones, infraestructura o desplegar.

## 9. Anexo de trazabilidad explícita: lista de espera (revisión 11/11/2026)

| ID de origen | Función | Relación propuesta | Decisiones pendientes |
|---|---|---|---|
| E30 | Lista de espera | SR-10 (concurrencia e integridad) y nueva regla SR-11 propuesta | Confirmar si forma parte del MVP, prioridad de solicitudes, expiración, notificación y promoción atómica al liberarse un asiento |

**SR-11 (propuesto, NO implementado):** si Producto aprueba una lista de espera, el backend debe mantener orden estable y autorización por pasajero, impedir promociones duplicadas y respetar siempre el límite de asientos confirmados. La promoción no implica reserva confirmada automática hasta que el negocio defina esa transición.

No agregar endpoints de lista de espera al OpenAPI ni implementar trabajos de fondo hasta que el alcance E30 sea autorizado y exista diseño de concurrencia validado.
