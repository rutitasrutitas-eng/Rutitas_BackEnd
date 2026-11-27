# BACKEND-DELETE-001 — Plan de pruebas y evidencias

**Estado:** casos diseñados, **NO ejecutados**. Usar `TimeProvider` simulado para avanzar siete días sin esperar. Datos estrictamente sintéticos. No atribuir los 72 casos previos a esta funcionalidad.

| ID | Escenario | Criterio esperado | Tipo de evidencia |
|---|---|---|---|
| DEL-V01 | Solicitud autenticada | Estado pendiente y 202 propuesto; fecha exacta UTC+7 días | HTTP + repositorio |
| DEL-V02 | Sin JWT | 401 sin cambios | HTTP |
| DEL-V03 | Solicitud duplicada | Sin prolongar vencimiento ni crear trabajos duplicados | Integración |
| DEL-V04 | Cuenta pendiente usa JWT anterior | Perfil/rutas/otros recursos denegados | Seguridad |
| DEL-V05 | Credencial `deletion:cancel` en rutas/perfil | Acceso siempre rechazado | Seguridad |
| DEL-V06 | Reautenticación válida e inválida | Solo credencial restringida, rate limit, sin enumeración | HTTP/seguridad |
| DEL-V07 | Cancelar antes de vencimiento | Vuelve a `active`, no ejecuta borrado, revoca credencial de recuperación | Unitarias/integración |
| DEL-V08 | Cancelación en o después del vencimiento | Rechazo con error pactado, sin restaurar | Reloj simulado |
| DEL-V09 | Procesar al vencer | Elimina todos los datos del inventario y conserva terceros | Mongo real Testcontainers |
| DEL-V10 | JWT anterior tras eliminación | 401 | Seguridad |
| DEL-V11 | Falla temporal Mongo | No confirma borrado; recupera con retry | Falla simulada + integración |
| DEL-V12 | Reinicio durante `deleting` | Reanuda desde checkpoint, estado final verificable | Integración |
| DEL-V13 | Cancelación contra worker concurrente | Gana una sola transición atómica | Integración concurrencia |
| DEL-V14 | Dos workers simultáneos | No duplican trabajo destructivo ni comprometen datos ajenos | Integración concurrencia |
| DEL-V15 | Regresión endpoints actuales | Siete endpoints existentes siguen compatibles | Contrato/xUnit + Android |
| DEL-V16 | Retención/respaldos/excepciones | Política autorizada y resultados verificados | Revisión documental + recuperación |

**Reglas de reporte:** registrar hora UTC, SHA/branch, comandos, suite, número real de pruebas y 0/errores/omitidos, tipo de simulación, qué dependencias se usaron y resultados. Un test escrito no cuenta como aprobado. No imprimir contraseñas, hashes, JWTs ni datos reales.

## Pruebas complementarias de seguridad

| ID | Escenario | Resultado esperado |
|---|---|---|
| DEL-V17 | JWT anterior tras cancelar eliminacion | HTTP 401; debe iniciar una sesion nueva |
| DEL-V18 | Reutilizar token de recuperacion consumido | Solicitud rechazada; no modifica el estado |
| DEL-V19 | Cancelar exactamente al vencimiento | Solicitud rechazada; no restaura la cuenta |
| DEL-V20 | Solicitudes simultaneas de eliminacion | Una sola transicion y vencimiento original |
| DEL-V21 | Error durante borrado definitivo | Estado recuperable; nunca marcar deleted antes de verificar |
| DEL-V22 | Token de recuperacion en endpoints normales | HTTP 401 o 403 segun contrato; nunca concede acceso |

Estas pruebas son propuestas pendientes de implementacion.

El codigo HTTP definitivo y el mecanismo de recuperacion
requieren acuerdo de contrato entre Backend y Movil.
