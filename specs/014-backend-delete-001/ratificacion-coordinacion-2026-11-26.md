# BACKEND-DELETE-001 - Ratificacion de coordinacion

Fecha local: 2026-11-26
Responsable de autorizacion: DaRa - Coordinacion Rutitas
PR: Draft #7
Rama: feature/backend-delete-001

## Identificacion de la revision independiente

SHA DELETE-001 revisado:

1c569b99f56b0376503e9ceb72d73d20552b65a4

SHA autorizado MAPS CI:

ccb861b8b3b7c716f565dcee3af86220fa09fefb

Nuevo main despues de integrar PR #9:

43378a55aa7aaad63f05cce5c6cdb0a06190729a

## Decisiones ratificadas

### RAT-DELETE-01 - Arquitectura G3

Estado: RATIFICADO.

Se acepta la separacion de users y
account_deletion_jobs mediante coordinacion
transaccional en MongoDB.

Se conserva el ADR-DELETE-001-G3.md
como registro arquitectonico.

### RAT-DELETE-02 - Rate limiting

Estado: RATIFICADO PARA TESTING.

Limite inicial:
5 solicitudes DELETE por minuto por usuario.

No representa una autorizacion de produccion.

### RAT-DELETE-03 - Equivalencia de Gates

Estado: RATIFICADO.

Se acepta la equivalencia documental
propuesta entre Gates A-E y G0-G5.

La ratificacion de la equivalencia
no implica aprobacion automatica de los Gates.

### RAT-DELETE-04 - Correcciones tecnicas

Coordinacion acepta tecnicamente:

- Rate limiting.
- Fail-closed.
- Readiness.
- Renovacion de leases.
- Fencing.
- Comprobacion transaccional.

Los riesgos residuales permanecen documentados.

## Integracion condicionada

Se autoriza:

- Sincronizar la rama DELETE-001 con main.
- Conservar el replica set aislado.
- Conservar el job de contratos MAPS-001.
- Registrar las decisiones de coordinacion.
- Publicar el nuevo SHA para revision.
- Ejecutar CI integrado.

Se requiere confirmar:

- Worker deshabilitado por defecto.
- Worker restringido a Testing.
- Bloqueo de nuevas solicitudes DELETE
  en entornos no autorizados.
- Cuatro jobs CI exitosos.
- 170 pruebas Backend.
- 14 pruebas de contrato MAPS-001.

Resultado esperado:
184 pruebas automatizadas aprobadas.

El resultado integrado queda pendiente
hasta ejecutar CI sobre el nuevo SHA.

## Pendientes de seguridad y privacidad

Responsable: Ulises.

- Retencion de trabajos.
- Respaldos y restauracion.
- Minimizacion de identificadores.
- Recuperacion de trabajos bloqueados.

## Pendientes de integracion movil

Responsable: Ame.

- Conciliacion contractual.
- Preparacion de pruebas conjuntas.
- 28 escenarios Android-Backend: NOT_RUN.

## Riesgos residuales

- Resultado desconocido de commit.
- Presupuesto temporal cooperativo.
- Disponibilidad transaccional permanente.
- Cobertura consolidada de ramas.
- Validacion TLS productiva.

## Restricciones

- No fusionar DELETE-001 a main
  sin nueva autorizacion de coordinacion.
- No desplegar a produccion.
- No eliminar datos reales.
- No modificar Azure.
- No modificar MongoDB Atlas.
- No activar el worker en produccion.
- No realizar force push.
- No generar gastos adicionales.

Presupuesto adicional autorizado: 0 MXN.

## Dictamen

Correcciones tecnicas: ACEPTADAS.
ADR G3: RATIFICADO.
Equivalencia documental G0-G5: RATIFICADA.
Integracion DELETE-001: CONDICIONADA.
CI integrado: PENDIENTE.
Merge DELETE-001: NO AUTORIZADO.
Produccion: NO-GO.
