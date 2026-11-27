# BACKEND-DELETE-001 - Seguridad y concurrencia

Decision: en_revision
Ejecucion: no_iniciado
Verificacion: no_ejecutada
Fecha: 2026-11-18

## 1. Acceso normal

- Solo cuentas active pueden acceder a recursos normales.
- pending_deletion y deleting bloquean el acceso.
- Se revisara el estado del usuario en el servidor.
- Los JWT ordinarios anteriores deben quedar invalidados.
- Un JWT de recuperacion nunca sera aceptado como JWT normal.

## 2. Recuperacion

- La recuperacion exige reautenticacion.
- Se emitira una autorizacion de alcance deletion:cancel.
- La autorizacion sera temporal y de uso restringido.
- La consulta de perfil y rutas estara prohibida.
- Al cancelar se invalidara la autorizacion usada.
- Los JWT anteriores no recuperaran validez.

## 3. Concurrencia

Transiciones propuestas:

active -> pending_deletion
pending_deletion -> active
pending_deletion -> deleting
deleting -> deleted

- La cancelacion solo procede antes del vencimiento.
- No se puede cancelar si comenzo deleting.
- Las transiciones usan filtros atomicos en MongoDB.
- Una solicitud duplicada no reinicia los siete dias.
- Se registraran versiones o identificadores de operacion.

## 4. Procesamiento de vencimientos

- Se utilizara TimeProvider para calcular fechas UTC.
- El plazo sera exactamente de siete dias.
- Un proceso local examinara solicitudes vencidas.
- Se recuperaran trabajos pendientes tras reinicios.
- Las operaciones seran idempotentes.
- TTL no sera el unico mecanismo de borrado.

## 5. Fallos parciales

- Registrar el progreso del proceso de eliminacion.
- Reintentar operaciones que no concluyeron.
- No declarar deleted antes de verificar el borrado.
- Evitar eliminar documentos de otros usuarios.
- Conservar solo auditoria minima autorizada.

## 6. Pruebas requeridas

- JWT normal rechazado durante pending_deletion.
- JWT anterior rechazado despues de cancelar.
- Token de recuperacion rechazado en perfil y rutas.
- Cancelacion y borrado concurrentes.
- Fallo de MongoDB con reintento.
- Reinicio durante la eliminacion.
- Dos ejecuciones simultaneas del procesador.
- Verificacion de ausencia de datos personales.

## 7. Decisiones pendientes

- Formato y duracion de autorizacion de recuperacion.
- Mecanismo de invalidacion por version de sesion.
- Politica de datos compartidos.
- Conservacion legal y tratamiento de respaldos.
- Estrategia concreta para fallos parciales.
- Acuerdo del contrato API con Ame.

Este documento es una propuesta.
No acredita implementacion ni pruebas ejecutadas.
