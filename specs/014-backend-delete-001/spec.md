# BACKEND-DELETE-001 — Especificación SDD de eliminación programada

**Fuente de autorización:** instrucciones de coordinación del 18/11/2026. **Catálogo relacionado:** E04 en `specs/003-identidad-perfil/spec.md`. **Estado:** decisión `en_revision`; ejecución `no_iniciado`; verificación `no_ejecutada`; tipo previsto `real` (no implementado). **Alcance:** exclusivamente Backend local, datos sintéticos, costo adicional autorizado $0 MXN. **No se autoriza** commit, push, PR, merge, despliegue ni recursos de nube para esta tarea.

## Objetivo y alcance
Permitir al titular autenticado solicitar la eliminación de su cuenta, bloquear su uso normal inmediatamente, permitir recuperación segura solo antes de siete días exactos y ejecutar el borrado definitivo y verificable de los datos personales asociados cuando venza el plazo. La desactivación **no** equivale a la eliminación definitiva. No incluye crear servicios de pago ni modificar los siete endpoints existentes de manera incompatible.

## Requisitos verificables
| ID | Requisito | Criterio de aceptación |
|---|---|---|
| DEL-R01 | Solicitud autenticada | Identidad exclusivamente del JWT validado; sin JWT, HTTP 401; solicitud exitosa registra estado pendiente y fechas UTC en una operación atómica de cambio de estado. |
| DEL-R02 | Plazo exacto | `deleteAfterUtc = requestedAtUtc + TimeSpan.FromDays(7)` desde un `TimeProvider` inyectable; comparaciones UTC. |
| DEL-R03 | Suspensión inmediata | JWTs ordinarios, incluidos tokens anteriores, no permiten perfil, rutas, modificación ni cualquier recurso protegido tras pasar a `pending_deletion`. |
| DEL-R04 | Recuperación restringida | Reautenticación independiente contra credenciales existentes y emisión de credencial temporal de alcance `deletion:cancel` únicamente; nunca aceptar esa credencial como JWT normal. No devolver credenciales de recuperación en logs. |
| DEL-R05 | Cancelación limitada | Solo antes del vencimiento, si no empezó el borrado irreversible, usando credencial restringida validada; cambio atómico a `active`, invalidación de autorización de recuperación y de sesiones anteriores. |
| DEL-R06 | Duplicados y carreras | Solicitudes repetidas no renuevan la fecha; cancelación versus inicio de eliminación se decide con actualización condicional atómica y versión/estado. |
| DEL-R07 | Ejecución idempotente | Al vencer, procesador recuperable tras reinicio reclama el trabajo; reintentos no duplican efectos ni borran datos ajenos; no depender solo de TTL. |
| DEL-R08 | Borrado integral | Inventario aprobado de cuenta, credenciales, perfiles, fotos/documentos, rutas, reservas, mensajes, incidencias, relaciones y copias personales; borrar/desvincular solamente propiedad del titular. |
| DEL-R09 | Fallas parciales | Registrar progreso mínimo y puntos de reanudación; reconciliar fallas en MongoDB, sin declarar borrado concluido antes de verificar todos los pasos. |
| DEL-R10 | Protección de terceros | Preservar información compartida de otros usuarios y resolver anonimización/desvinculación en datos compartidos. Excepciones legales, respaldo y retención requieren aprobación específica de privacidad. |
| DEL-R11 | Revocación final | Al concluir, credenciales y sesiones antiguas imposibilitan acceso; impedir recuperación/reutilización de cuenta eliminada de manera no autorizada. |
| DEL-R12 | Evidencia y privacidad | Pruebas con datos ficticios, sin JWT/contraseñas/PII en trazas, sin llamadas a nube ni costos; documentar qué datos se eliminaron realmente. |

## Estados de dominio propuestos
`active` → `pending_deletion` → `deleting` → `deleted` (si existe registro mínimo de auditoría permitido). Cancelación: `pending_deletion` → `active` solo si `nowUtc < deleteAfterUtc` y el borrado no ha sido reclamado. `deleting` no puede cancelarse. Los campos y registros de auditoría deben tener una política de datos mínima aprobada.

## Contrato API candidato — pendiente de acuerdo con Ame
Base `/api/v1`, formato JSON camelCase y RFC 7807/9457 `ProblemDetails` compatible con el servicio actual.

| Método y ruta candidata | Autorización | Respuesta candidata | Errores candidatos |
|---|---|---|---|
| `DELETE /users/me` | JWT normal de cuenta activa | `202 Accepted`, `{status:"pending_deletion",requestedAtUtc,deleteAfterUtc}` | 401, 409, 429, 503 |
| `GET /users/me/deletion` | JWT normal solo hasta solicitar; después exige alcance de recuperación | `200 OK`, `{status,requestedAtUtc,deleteAfterUtc}` | 401, 404, 429 |
| `POST /auth/deletion/recovery` | Reautenticación controlada; no JWT normal | `200 OK`, credencial corta con alcance cancel únicamente | 400, 401, 429, 503 |
| `POST /users/me/deletion/cancel` | Credencial restringida `deletion:cancel` | `200 OK`, `{status:"active"}` (o `204`; decidir con Ame) | 401, 409, 410, 429, 503 |

**IMPORTANTE:** estos códigos, cuerpos, rutas y credenciales son propuestas, no un contrato aprobado. Para `GET` durante suspensión, acordar si el token restringido permite únicamente consultar estado y cancelar, o si se usa una autorización adicional `deletion:status`. Nunca permitir privilegios generales. Definir protección de duplicados, enumeración de cuentas, CSRF según cliente, reintentos, revocación y compatibilidad Android con Ame antes del código de API.

## Inventario de datos por cerrar
Confirmados en implementación actual: colección `users` (email, `PasswordHash`, nombre, teléfono, estado, fechas), `revoked_tokens` (revocación JWT) y `routes` (propiedad/relación todavía por comprobar). Otros módulos en `specs/004` a `010` describen funcionalidades futuras, **no se consideran colecciones ya implementadas**. Deben inventariarse colecciones, almacenamiento externo, índices, referencias, auditorías, datos compartidos y respaldos antes de declarar borrado integral. Prohibido eliminar rutas compartidas sin reglas de propiedad verificadas.

## Decisiones abiertas bloqueantes
1. Aprobación del contrato con Ame y coordinación, incluidas las semánticas HTTP definitivas.
2. Mecanismo de reautenticación, token de recuperación (lifetime, one-time/rotación, revocación, rate limit), y comportamiento de login ordinario mientras la cuenta está pendiente.
3. Inventario real de datos y propietarios, política de retención y excepciones legales aprobadas.
4. Estrategia de transición atómica/reintentos y si se usarán transacciones MongoDB, con soporte real del entorno local.
5. Semántica de borrado final, protección frente a reconstrucción desde respaldos y conservación mínima de auditoría.

## Límites de aprobación
La instrucción del líder aprueba trabajo local, pero el flujo SDD exige revisar el contrato con Ame antes de implementar las rutas. La Constitución y AGENTS heredados aún indican una fase documental histórica; registrar el cambio de alcance autorizado sin auto-ratificarlos.
