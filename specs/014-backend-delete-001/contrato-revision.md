# BACKEND-DELETE-001 — Revisión de contrato para Ame y coordinación

**Fecha:** 18/11/2026. **Estado:** `en_revision`; **ejecución:** `no_iniciado`; **verificación:** `no_ejecutada`. **Fuente:** acuerdo preliminar Backend–Android expresado por Ame. **Alcance:** documento local, sin commits, pushes ni despliegues.

## Ubicación propuesta

Guardar el YAML adjunto en `specs/014-backend-delete-001/contracts/openapi-delete-draft.yaml`. No sobrescribir `specs/001-unit2-api/contracts/openapi.yaml`: éste contiene los siete endpoints actuales y sus esquemas.

## Contrato candidato

| Ruta | Autorización | Éxito | Cuerpo |
|---|---|---|---|
| `DELETE /api/v1/users/me` | JWT ordinario de cuenta activa | 202 | `DeletionStatus` |
| `GET /api/v1/users/me/deletion` | Credencial de recuperación restringida | 200 | `DeletionStatus` |
| `POST /api/v1/auth/deletion/recovery` | Email + contraseña por canal protegido | 200 | `RecoveryAuthenticationResponse` |
| `POST /api/v1/users/me/deletion/cancel` | Credencial de recuperación restringida no consumida | 200 | `DeletionCancellationResponse` |

**Respuestas `DeletionStatus`:** `status`, `requestedAtUtc`, `deleteAfterUtc`, `recoveryAllowed`. Se calculan en servidor, UTC, con 7 días exactos. **Respuestas de error:** `application/problem+json`; 401 (sin autenticación/credencial inválida), 403 (sin alcance), 409 (transición/conflicto), 410 (plazo vencido, si el solicitante tiene autorización suficiente), 429 (límite de intentos), 503 (dependencia fallida y efecto no confirmado). También 400 para JSON de recuperación inválido.

## Decisiones de seguridad pendientes antes de implementación

1. **Reintentos tras 202:** como el JWT normal deja de funcionar inmediatamente, un segundo DELETE con el mismo JWT devolverá 401. Es idempotencia en la persistencia (no mover el plazo), no en el código HTTP. Android debe manejar la respuesta incierta de una interrupción y reautenticarse por el flujo de recuperación.
2. **Token restringido:** se propone token opaco de corta duración (300 segundos a revisar), guardado únicamente como hash y consumido al cancelar. `GET /deletion` no lo consume; `POST /cancel` sí. Dos esquemas OpenAPI bearer se distinguen **solo por validación real en servidor**, no por nombre: prohibido que la credencial restringida pase el esquema ordinario JWT.
3. **Migración de `SessionVersion`:** los usuarios y JWT actuales no incluyen versión. Definir migración y política de JWT sin claim, sin abrir una ventana de reactivación tras cancelar.
4. **Autorización de estado:** `GET /deletion` solo con token restringido tras la solicitud. Si Móvil necesita ver estado antes de solicitar borrado, acordar una respuesta separada y segura, sin debilitar la protección.
5. **Falla de MongoDB:** no devolver 202/200 si la transición no quedó confirmada. Distinguir estado incierto y recuperación tras reinicio.
6. **Borrado irreversible:** la eliminación real requiere inventario, protección de datos compartidos, reintentos y verificación final. No basta pasar a `pending_deletion`.
7. **Privacidad:** aprobar retención mínima, tratamiento de respaldos y excepciones legales; evitar enumeración de cuentas en recuperación.
8. **Infraestructura:** ejemplos del YAML son solo entorno local; no implican HTTPS remoto, ni autorización de publicar.

## Estado de las pruebas

Hay 22 casos documentados (`DEL-V01`–`DEL-V22`), **ninguno ejecutado para BACKEND-DELETE-001**. Los 72 tests pasados de Fase A + simulación de HTTP 503 pertenecen a otro alcance y rama.

## Próximo gate

Solicitar a Ame la conformidad con estos DTO y semánticas precisas; después presentar el borrador completo a coordinación para autorización específica de contrato/implementación local. La aprobación del concepto no autoriza commits de este módulo.
