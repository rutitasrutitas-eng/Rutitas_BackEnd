# BACKEND-DELETE-001 — Plan técnico SDD

**Decisión:** `en_revision` · **Ejecución:** `no_iniciado` · **Verificación:** `no_ejecutada` · No autoriza commit/push/PR/despliegue.

## Arquitectura prevista
- `Rutitas.Domain`: transición de estado y fechas UTC invariantes; historial mínimo sin credenciales.
- `Rutitas.Application`: `RequestDeletion`, `GetDeletionStatus`, `ReauthenticateForCancellation`, `CancelDeletion`, `ProcessDueDeletions`, políticas de retención y autorización.
- `Rutitas.Infrastructure`: operaciones MongoDB condicionales por `userId`, `status`, `version`; registro persistente de job y pasos de borrado recuperables, repositorios dedicados; índices necesarios sin TTL como único ejecutor.
- `Rutitas.Api`: contrato `/api/v1`; middleware/política para denegar JWT normales de cuentas suspendidas; autorización con esquema/alcance de recuperación separado; `TimeProvider` inyectado.
- Worker local hospedado en el mismo proceso .NET (`BackgroundService`) solo cuando esté aprobado el diseño; reconciliación al inicio, periodicidad configurable, cancelación cooperativa, reintentos acotados.

## Flujo técnico
1. Con JWT normal activo, establecer `pending_deletion`, `requestedAtUtc`, `deleteAfterUtc = requestedAtUtc + 7 días`, versión y operación persistente de forma condicional; responder resultado acordado.
2. Middleware de autorización verifica usuario activo en servidor, no solo expiración del JWT; invalidar acceso general de tokens emitidos antes del cambio.
3. Reautenticar expresamente, con limitación de intentos, emitiendo credencial restringida que no funcione contra recursos ordinarios.
4. Cancelar con compare-and-swap sobre estado/versión/fecha y reinicializar autorización de sesiones. Si el worker reclamó `deleting`, cancelar falla sin cambios.
5. Worker detecta vencidos mediante consulta indexada, reclama trabajo con compare-and-swap y registra fase; ejecuta borrado por etapas idempotentes; verifica ausencia de datos en inventario.
6. En fallo transitorio deja checkpoint y reintenta después de reinicio sin perder el estado; alertas/logs sin datos personales.

## Riesgos y mitigaciones previstas
| Riesgo | Mitigación/decisión |
|---|---|
| Carrera cancelación-borrado | Operaciones Mongo atómicas sobre estado y versión, pruebas concurrentes |
| Proceso se interrumpe | Registro de job/checkpoints, reanudación al iniciar, pasos idempotentes |
| JWT antiguo permanece válido | Comprobación del estado del usuario en servidor en cada petición, separación de esquemas de tokens |
| Borrado parcial | Inventario de recursos, orquestación por pasos reanudables, verificación antes de marcar concluido |
| Datos de otros titulares | Filtros por propiedad y política de anonimización de recursos compartidos |
| MongoDB temporalmente caído | Error explícito, no respuesta de éxito, reintento limitado y sin pérdida de solicitud confirmada |
| Excepciones legales y backups | Validación con responsables, política de retención y de restauración |

## Gates
A. Revisar especificación con liderazgo/privacidad y acordar contrato con Ame. B. Confirmar inventario de datos y decisiones de seguridad. C. Implementar exclusivamente local con datos sintéticos, sin alterar contratos existentes. D. Ejecutar unitarias, contrato, integración Testcontainers, seguridad y concurrencia. E. Documentar evidencias y riesgos; **no** commit/push/PR/merge/despliegue sin autorización separada.
