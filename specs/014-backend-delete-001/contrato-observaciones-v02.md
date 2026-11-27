# BACKEND-DELETE-001 / ANDROID-DELETE-001 — Revisión 0.2.0

**Estado:** decisión `en_revision`; ejecución `no_iniciado`; verificación funcional `no_ejecutada`. Solo documentos locales. Sin permiso para commits, push, PR ni implementación.

## Acuerdos candidatos para revisión por Ame y coordinación

| Observación | Regla propuesta | Evidencia futura |
|---|---|---|
| HTTP 202 perdido | Android no confirma éxito. Reautentica por flujo restringido y consulta el estado de la solicitud. Un reenvío del DELETE con JWT invalidado puede responder 401 sin modificar vencimiento. Si no puede verificar, muestra resultado desconocido. | Integración con respuesta perdida y consulta posterior |
| HTTP 200 de cancelación perdido | El token de recuperación pudo consumirse. Android solicita **login ordinario nuevo**. Solo el login 200 con cuenta `active` confirma recuperación. Un fallo de login es inconcluso; no inferir que sigue pendiente ni reusar token consumido. | Simulación de corte de conexión tras cancelación |
| Vencimiento | Antes del plazo: 200 `pending_deletion`, `recoveryAllowed=true`. Exactamente al plazo o después, sin comenzar borrado: 200 `pending_deletion`, `recoveryAllowed=false` **si ya se posee token restringido vigente**. Si el worker inició borrado, 410 para el titular acreditado; token ausente o vencido: 401. El endpoint público de reautenticación responde genéricamente 401 al no ser elegible, sin filtrar existencia. | `TimeProvider` en límites y pruebas de privacidad |
| Token opaco | Valor criptográficamente aleatorio, solo hash en servidor, vinculado a titular y alcances `deletion:read`/`deletion:cancel`. GET no consume. Cancelación consume con transición atómica o recuperación transaccional. Duración propuesta 300 s, **pendiente de aprobación**. | Pruebas de expiración, concurrencia y uso repetido |
| 429 | `Retry-After` como segundos enteros no negativos; `application/problem+json`; Android no reintenta automáticamente acciones destructivas. | Contrato y pruebas con límite aplicado |
| SessionVersion | Campo persistido en `User`, claim en JWT nuevos, validación global. Incremento en solicitud y cancelación. Propuesta: tokens antiguos sin versión requieren nuevo login, **pendiente de autorización por impacto**. | Regresión 7 operaciones y prueba de JWT legado |

## Reglas del procesador y datos (Backend)

- `TimeProvider` inyectado, fechas UTC y vencimiento exacto en `requestedAtUtc + 7 días`; al alcanzar el vencimiento cancelar queda prohibido incluso si worker no inició.
- Reclamación atómica `pending_deletion -> deleting` condicionada por estado y fecha; dos workers no deben reclamar el mismo trabajo en simultáneo.
- Worker local recuperable tras reinicio con checkpoint, reintentos idempotentes, y `deleted` solo después de verificar ausencia de datos personales sujetos a eliminación.
- Inventario confirmado en Fase A: `users`, `routes`, `revoked_tokens`. Las rutas actuales no declaran propiedad de usuario; no borrar rutas públicas indiscriminadamente. Futuras reservas, mensajes, documentos e incidencias no deben declararse implementadas.
- Definir tratamiento de copias de seguridad, auditoría mínima, excepciones legales y política de conservación con autoridad correspondiente antes de afirmar borrado definitivo completo.
- Las fallas de MongoDB pueden dejar el estado de una operación **sin confirmar para el cliente**; HTTP 503 y pérdida de conexión no significan necesariamente que nada cambió.

## Puntos sin ratificar

1. Duración final del token; decisiones sobre retención del hash y mecanismos de reemisión.
2. Política y migración de JWT legados sin `sessionVersion`; impacto de obligar nuevo login.
3. Condiciones precisas de 409 versus 410 cuando `deleting` empezó; prevención de enumeración de cuentas.
4. Procesamiento idempotente, semántica del token y transición atómica entre cancelación y worker.
5. Conservación legal y backups; consentimiento de coordinación y seguridad/privacidad.

**Siguiente gate:** compartir YAML 0.2.0 con Ame, registrar su conformidad escrita o correcciones, presentar contrato y decisiones a líder. No autoriza implementar.
