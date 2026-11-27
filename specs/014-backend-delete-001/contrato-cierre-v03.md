# BACKEND-DELETE-001 / ANDROID-DELETE-001 — Cierre de contrato v0.3 candidato

**Fecha:** 18/11/2026
**Decisión:** `en_revision` · **Ejecución:** `no_iniciado` · **Verificación del módulo:** `no_ejecutada`
**Autoridad:** coordinación aprobó preliminarmente las cuatro operaciones, pero no su implementación, commits, PR, push o despliegue.

## Cambios desde v0.2

1. **Login normal y HTTP 200 perdido.** El login emite un nuevo JWT únicamente a cuenta `active`, tras comprobar estado y credenciales en servidor. Si se pierde la respuesta de cancelación, un login nuevo exitoso confirma que la cuenta está activa; un login fallido NO demuestra que continúe pendiente.
2. **Credencial vencida.** Una nueva autenticación restringida puede emitir otra credencial opaca si la cuenta aún está `pending_deletion`, la hora actual es anterior a `deleteAfterUtc` y no inició el borrado. No renueva el plazo; credenciales antiguas inválidas no se reactivan; aplicar rate limiting.
3. **409 vs. 410.** `409` significa un conflicto de transición/concurrencia que requiere comprobación. `410` significa recuperación definitivamente no disponible para un titular acreditado al cumplirse siete días exactos o iniciarse `deleting`. Respuesta segura `Account recovery is no longer available.`; reautenticación pública mantiene respuestas genéricas.
4. **Consulta vencida.** `GET /users/me/deletion` puede devolver 200 `recoveryAllowed=false` únicamente si el trabajo aún está `pending_deletion` y el solicitante conserva una credencial restringida *todavía válida* emitida antes del vencimiento. Si el token expiró, corresponde 401; no se emite uno nuevo después del vencimiento.
5. **Error 429.** `Retry-After` usa segundos enteros (`delta-seconds`), no fecha HTTP.
6. **Tokens antiguos.** Política candidata: rechazar JWT ordinarios sin `sessionVersion` y exigir login nuevo, sujeto a autorización explícita por impacto en Android.

## DTO y operaciones

- `DELETE /api/v1/users/me` → 202 `DeletionStatus`.
- `GET /api/v1/users/me/deletion` → 200 `DeletionStatus`, con credencial restringida.
- `POST /api/v1/auth/deletion/recovery` → 200 `RecoveryAuthenticationResponse` para credenciales elegibles.
- `POST /api/v1/users/me/deletion/cancel` → 200 `DeletionCancellationResponse`.

Los esquemas JSON y `application/problem+json` se definen en el YAML adjunto, complementario a las siete operaciones actuales.

## Pruebas propuestas adicionales, NO EJECUTADAS

| ID | Escenario |
|---|---|
| DEL-V23 | Login ordinario denegado para cuenta `pending_deletion` o `deleting` |
| DEL-V24 | Se pierde 200 de cancelación: un nuevo login exitoso verifica `active`; fallo no concluyente |
| DEL-V25 | Expira el token restringido: nueva reautenticación válida antes de plazo sin renovarlo |
| DEL-V26 | Rechazar token vencido y no emitir uno nuevo cuando `nowUtc >= deleteAfterUtc` |
| DEL-V27 | Diferenciar 409 de 410 y proteger mensajes contra enumeración |
| DEL-V28 | `GET` devuelve 200 `recoveryAllowed=false` con token vigente y pendiente vencido no procesado |

## Pendiente antes de implementación

- Conformidad escrita Backend–Android sobre versión 0.3 y revisión de coordinación.
- Aprobación de duración candidata de 300 segundos y migración de JWT sin `sessionVersion`.
- Diseñar/autorizar atomicidad de credencial y cancelación, reintentos, worker durable y reanudación.
- Inventario de borrado, manejo de relaciones de terceros, respaldo y retención/excepciones legales.
- No se ha ejecutado ningún caso DEL-V01..DEL-V28. No declarar endpoints funcionales.

**Restricciones:** exclusivamente documentación y preparación de pruebas locales, datos sintéticos, $0 MXN de gasto adicional autorizado; sin commits, push, PR, merge ni despliegues.
