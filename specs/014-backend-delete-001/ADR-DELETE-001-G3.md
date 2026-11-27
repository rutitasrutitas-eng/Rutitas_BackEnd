# ADR-DELETE-001-G3 - Ejecucion durable

Estado: RATIFICADO POR COORDINACION - 2026-11-26
Modulo: BACKEND-DELETE-001
Referencia: PR Draft #7

## Contexto

La revision independiente solicita conciliar el
cambio arquitectonico respecto a G3.

La implementacion local incorpora una coleccion
independiente denominada account_deletion_jobs.

## Decision implementada

Separar el estado del usuario del estado operativo
del proceso durable de eliminacion.

Coleccion users:

- Estado de cuenta.
- SessionVersion.
- Fecha de solicitud.
- Fecha limite de recuperacion.

Coleccion account_deletion_jobs:

- Identificador determinista.
- Identificador del titular.
- Version esperada de sesion.
- Estado operativo.
- Checkpoint.
- LeaseOwner.
- LeaseVersion.
- LeaseExpiresAtUtc.
- AttemptCount.
- NextAttemptAtUtc.
- LastErrorCode sanitizado.

## Coordinacion transaccional

MongoDB replica set permite coordinar las operaciones
sobre users y account_deletion_jobs.

Se utilizan:

- ReadConcern.Snapshot.
- ReadPreference.Primary.
- WriteConcern.WMajority.

La transicion de la cuenta a deleting y el checkpoint
account_claimed se confirman conjuntamente.

La eliminacion del documento del titular y el
checkpoint account_removed tambien se confirman
en una misma transaccion.

La verificacion posterior registra verified.

## Concurrencia

El fencing verifica simultaneamente:

- Identificador del trabajo.
- Estado processing.
- Propietario del lease.
- Version del lease.
- Vigencia segun reloj MongoDB.

Cada reclamacion incrementa LeaseVersion.

La renovacion condicionada conserva la version,
no reactiva leases expirados y no reduce su vigencia.

La renovacion se propone antes de cada checkpoint.

## Justificacion

La coleccion independiente permite conservar
el progreso operativo incluso cuando el documento
del usuario ya fue eliminado.

Facilita recuperacion, reintentos, reconciliacion
y verificacion posterior.

## Riesgos y limitaciones

- La coleccion conserva identificadores del titular.
- La politica definitiva de retencion esta pendiente.
- Los respaldos no se consideran eliminados.
- Los trabajos blocked requieren reconciliacion.
- No existe autorizacion para datos reales.
- La renovacion entre etapas no garantiza que una
  transaccion individual termine antes del vencimiento.
- Debe verificarse el comportamiento ante operaciones
  prolongadas y resultados de commit desconocidos.

## Alcance

No modificar rutas compartidas sin inventario
y reglas de propiedad ratificadas.

No crear TTL sobre users para ejecutar eliminaciones.

No incorporar borrado de respaldos ni colecciones
adicionales sin autorizacion.

## Ratificaciones pendientes

- DaRa: decision arquitectonica y gate G3.
- Ulises: privacidad, retencion y respaldos.
- Ame: conciliacion documental y pruebas conjuntas.
- Backend: evidencias de concurrencia y renovacion.

## Dictamen

ARQUITECTURA IMPLEMENTADA LOCALMENTE.
RATIFICACION G3 PENDIENTE.
PRODUCCION NO-GO.


## Ratificacion de coordinacion

Fecha: 2026-11-26.

DaRa ratifica la separacion arquitectonica
de users y account_deletion_jobs mediante
coordinacion transaccional en MongoDB.

Consultar:
ratificacion-coordinacion-2026-11-26.md

Esta decision no autoriza el merge
ni el despliegue productivo.
