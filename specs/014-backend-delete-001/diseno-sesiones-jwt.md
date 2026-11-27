# BACKEND-DELETE-001 - Diseno de sesiones JWT

Decision: en_revision
Ejecucion: no_iniciado
Verificacion: no_ejecutada
Fecha: 2026-11-18

## 1. Implementacion actual comprobada

AuthController.cs:
- Genera JWT firmados con HmacSha256.
- Expiracion del token: 30 minutos.
- Claims: sub, email, role, jti, iat.
- No incluye version de sesion.

Program.cs:
- Verifica firma, emisor, audiencia y expiracion.
- Consulta revocacion mediante jti.
- Consulta usuario en MongoDB.
- Rechaza cuentas distintas de active.
- Falla de manera segura ante errores de consulta.

## 2. Modificacion propuesta

Agregar SessionVersion al usuario.

Valor inicial propuesto: 1.

Agregar claim sessionVersion al JWT ordinario.

El servidor debe comprobar:
- Usuario existente.
- Usuario con Status active.
- Token no revocado.
- SessionVersion coincidente.
- Token ordinario, sin permisos de recuperacion.

## 3. Solicitud de eliminacion

Operacion atomica en MongoDB:

active -> pending_deletion

Incrementar SessionVersion.

Registrar:
- requestedAtUtc
- deleteAfterUtc
- estado de eliminacion
- identificador de operacion

Los JWT ordinarios anteriores quedan invalidados.

## 4. Recuperacion

- Solicitar reautenticacion.
- Verificar credenciales en servidor.
- Aplicar limite de intentos.
- Emitir autorizacion limitada deletion:cancel.
- Definir expiracion corta.
- Prohibir acceso a endpoints normales.
- Evitar reutilizacion de la autorizacion.

El mecanismo de consulta del estado de eliminacion
debe acordarse con Android.

## 5. Cancelacion

Solo se permite cuando:
- La cuenta esta pending_deletion.
- La fecha actual es anterior a deleteAfterUtc.
- El borrado irreversible no ha comenzado.
- La autorizacion restringida es valida.

La transicion debe ser atomica:

pending_deletion -> active

Incrementar SessionVersion.
Invalidar autorizaciones de recuperacion anteriores.
Exigir nuevo inicio de sesion normal.

## 6. Compatibilidad

Las cuentas existentes no tienen SessionVersion.

Antes de implementar:
- Definir migracion segura de cuentas actuales.
- Definir tratamiento de JWT sin sessionVersion.
- Evitar reactivar JWT antiguos tras cancelar.
- Agregar pruebas de regresion para Android.
- Revisar los siete endpoints actuales.

La politica definitiva requiere aprobacion SDD.

## 7. Pruebas relacionadas

DEL-V04: JWT anterior en pending_deletion.
DEL-V05: credencial restringida en recursos normales.
DEL-V07: cancelacion valida.
DEL-V10: JWT tras eliminacion.
DEL-V17: JWT antiguo tras cancelar.
DEL-V18: reutilizacion de token de recuperacion.
DEL-V22: aislamiento de permisos.

## 8. Decisiones pendientes

- Formato de la credencial de recuperacion.
- Tiempo de expiracion.
- Politica de migracion SessionVersion.
- Comportamiento ante errores de MongoDB.
- Contrato definitivo Backend-Movil.
- Aprobacion de seguridad y privacidad.

No se ha implementado ni ejecutado este diseno.
