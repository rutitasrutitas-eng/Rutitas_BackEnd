# 008 — Seguridad, incidencias, reportes y bloqueos | Especificación funcional

**Proyecto:** Rutitas UTTT — Backend  
**Estado:** PROPUESTO · pendiente de revisión y aprobación · sin implementación autorizada.  
**Dependencias:** 002 catálogo funcional; 003 identidad/perfil; 004 comunidades/verificación; 006 rutas/viajes; 007 solicitudes/reservas.  
**Fuente:** `Rutitas_Actividad_4.xlsx` y `002-catalogo-funcional/inventario-funcional.md` (E42–E45, E51–E52, N18).  
**Fuera de alcance de este módulo:** E41 valoraciones (módulo 009); E46 contactos de confianza y E47–E48 soporte (módulo posterior); N07–N08 consentimiento/privacidad (requiere política propia).

## 1. Situación y objetivo

Los siete endpoints operativos **no incluyen reportes, moderación ni bloqueos**. El alcance de este documento es definirlos antes de escribir código o alterar el contrato OpenAPI existente. Una cuenta verificada no queda automáticamente autorizada a moderar. Un bloqueo interpersonal no equivale a suspensión administrativa.

## 2. Requisitos con criterios de aceptación

| ID | Origen | Requisito | Criterio de aceptación propuesto |
|---|---|---|---|
| SI-01 | E42 | Crear reporte | Usuario autenticado informa un incidente con categoría válida, descripción acotada y referencia opcional a viaje/usuario; se genera folio, sin revelar identidad del denunciante al denunciado. |
| SI-02 | E43 | Mis reportes | Usuario consulta únicamente los que creó, con estado público, fecha y paginación; sin notas internas de moderación. |
| SI-03 | E44 | Bloquear usuario | Usuario autenticado bloquea a otro usuario; no puede bloquearse a sí mismo; operación idempotente y efecto definido en descubrimiento, solicitudes y mensajes. |
| SI-04 | E45 | Quitar bloqueo | Solo dueño del bloqueo puede levantarlo; no levanta suspensiones administrativas ni restablece reservas canceladas. |
| SI-05 | E51 | Administración consulta reportes | Moderador autorizado, con verificación de rol en servidor, puede consultar cola por estado/prioridad con mínimos datos personales. |
| SI-06 | E52 | Administración resuelve reporte | Moderador autorizado cambia estado conforme a transiciones válidas, registra motivo, actor, hora y evidencia de decisión. |
| SI-07 | N18 | Confirmar incidente vinculado a trayecto | Decidir si es un reporte de E42 con `rideId` o un evento específico; evitar duplicar el mismo incidente en dos colecciones/rutas. |
| SI-08 | transversal | Protección contra abuso | Limitar solicitudes, prevenir reportes masivos repetidos, validar referencias y evitar exposición de reportes ajenos por cambio de IDs. |
| SI-09 | transversal | Auditoría y privacidad | Registrar actor/acción/fecha sin JWT ni secretos; retención y acceso a evidencias pendientes de política aprobada. |

## 3. Entidades propuestas (NO implementadas)

- `IncidentReport`: `id`, `reporterUserId`, `targetUserId?`, `rideId?`, `category`, `description`, `status`, `createdAtUtc`, `updatedAtUtc`, `resolvedAtUtc?`, `version`. Preferir una referencia al viaje publicado, nunca al catálogo de rutas.
- `UserBlock`: `id`, `ownerUserId`, `blockedUserId`, `createdAtUtc`; índice único por pareja `(ownerUserId, blockedUserId)`; **un bloqueo es unilateral** salvo política aprobada.
- `ModerationAction`: `id`, `reportId`, `moderatorUserId`, `fromStatus`, `toStatus`, `reasonCode`, `createdAtUtc`. Evitar almacenar evidencias sensibles en logs.
- `AttachmentReference` (opcional): metadatos mínimos y referencia segura, sin almacenar imágenes/documentos como enlaces públicos.

**Decisión pendiente:** definir si habrá archivos adjuntos, dónde se almacenarán, revisión antimalware, límites de tamaño, protección contra acceso no autorizado, plazos de conservación y eliminación.

## 4. Estados y reglas

- Propuesta de estado de reporte: `submitted` → `under_review` → `resolved` o `dismissed`; decidir si un caso se puede reabrir y quién lo autoriza.
- Cambios críticos deben preservar historial/auditoría; no permitir que la API reciba `moderatorUserId` o permisos desde el cliente.
- Las acciones disciplinarias (advertencia, suspensión, revocación de conductor) son decisiones **separadas** del simple cierre de un reporte; requieren política, autorización y apelación antes de ser automatizadas.
- Un bloqueo debe aplicarse en servidor donde corresponda (chat, contacto, ofertas/solicitudes), no solo ocultar usuarios en Android. Confirmar comportamiento si ya existe un viaje/reserva entre ambas personas.
- Reportar emergencias **no sustituye** llamar a servicios de emergencia. No prometer monitoreo 24/7 si el equipo no lo garantiza.

## 5. Contratos HTTP candidatos (NO implementados)

El Excel usa `/v1`; el prefijo operativo del backend es **`/api/v1`**. Los siguientes contratos son propuestas sujetas a revisión con Móvil:

| Origen | Método | Ruta candidata | Acceso |
|---|---|---|---|
| E42 | POST | `/api/v1/reports` | Usuario autenticado |
| E43 | GET | `/api/v1/me/reports` | Dueño del reporte |
| E44 | POST | `/api/v1/me/blocks` | Dueño del bloqueo |
| E45 | DELETE | `/api/v1/me/blocks/{id}` | Dueño del bloqueo; definir si `id` es usuario bloqueado o registro de bloqueo |
| E51 | GET | `/api/v1/admin/reports` | Moderador autorizado |
| E52 | PATCH | `/api/v1/admin/reports/{id}` | Moderador autorizado |
| N18 | POST | `/api/v1/trips/{tripId}/incidents` | **Por decidir** si se conserva o unifica con POST `/reports` |

Los cuerpos JSON, enumeraciones, paginación, privacidad de respuestas, errores y semántica de `DELETE` se decidirán en OpenAPI del módulo **después del gate**. No modificar el OpenAPI de los siete endpoints por anticipado.

## 6. Seguridad y privacidad

1. Autenticación JWT y autorización **en servidor**: propietario del reporte/bloqueo o rol explícito de moderador; rechazar IDOR (`403` o `404` según política aprobada).
2. Validación del texto: longitud, caracteres, categoría, referencias existentes y sanitización de presentación; nunca incluir contenido de denuncias sin escape en interfaces.
3. Reportes de abuso y evidencias pueden contener información sensible; limitar lectura, cifrar en tránsito y reposo, definir conservación, eliminación y acceso de auditores.
4. Rate limiting, detección de duplicados e idempotencia para reintentos; no silenciar reportes genuinos por reglas automáticas sin revisión.
5. La respuesta al denunciante solo muestra el estado y resultado permitido, no identidades o documentos de terceros.
6. Índices por `reporterUserId`, estado/fecha y unicidad de bloqueo. Registrar bitácora de cambios; revisar concurrencia en actualización de reportes.
7. No almacenar credenciales, claves, tokens ni datos de localización exacta en mensajes de log o notificaciones.
8. Definir canal fuera de la app para urgencias y solicitudes de derechos de privacidad antes del lanzamiento público.

## 7. Errores y pruebas requeridas

- `POST /reports`: `201` al crear; `400` inválido; `401` sin sesión; `403` no habilitado; `404` referencia inaccesible; `409` duplicado según política; `429` abuso.
- `GET /me/reports`: solo propios, con paginación, `401` sin token.
- Bloqueos: rechazar autobloqueo; reintentos no crean duplicados; prohibir borrar bloqueos ajenos.
- Administración: usuario normal nunca puede leer cola ni cerrar reportes; probar escalada de privilegios y propiedad.
- Pruebas unitarias, contrato, integración MongoDB/Testcontainers y seguridad; comprobar retención y borrado de adjuntos si se aprueban.
- Preservar los siete endpoints existentes y la referencia local de 63/63 pruebas; **esta especificación no afirma que las nuevas pruebas se hayan ejecutado**.

## 8. Decisiones bloqueantes / gate

1. ¿Quién modera, con qué rol, autorización y proceso de apelación?
2. ¿Qué categorías de reporte y resultados están permitidos? ¿Qué SLA real puede ofrecerse?
3. ¿N18 se integra como reporte E42 vinculado a un viaje o es un flujo distinto?
4. ¿Qué implica bloquear a alguien si ya hay reserva o trayecto activo?
5. ¿Se permiten adjuntos y cuál será la retención/acceso/eliminación?
6. ¿En qué módulo se especificarán E41, E46, E47–E48 y N07–N08?

**Gate:** revisión y aprobación expresa por Producto/Liderazgo, Backend, Móvil y Seguridad/Privacidad antes de crear implementación o despliegue.
