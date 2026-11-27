# BACKEND-DELETE-001 - Conciliacion de Gates G0-G5

Fecha: 2026-11-26
Rama: feature/backend-delete-001
PR: Draft #7
Observacion: DEL-FIX-06

## Fuentes

- spec.md
- plan.md
- tasks.md
- trazabilidad.md
- inventario-datos.md
- contracts/openapi-delete-v04-local.yaml
- ADR-DELETE-001-G3.md
- evidencia-cobertura-consolidada.md
- cierre-local-publicacion-2026-11-26.md

## Criterio de conciliacion

El plan original define Gates A, B, C, D y E.

La equivalencia G0-G5 es una propuesta para
ratificacion de coordinacion.

No sustituye las autorizaciones originales.
No implica aprobacion automatica de los Gates.

## Matriz de conciliacion

| Gate | Fuente | Alcance | Estado |
|---|---|---|---|
| G0 | A / DEL-T01 | Autorizacion y alcance | Autorizacion local y publicacion Draft |
| G1 | A / DEL-T02 | Contrato API y conciliacion movil | PARCIAL |
| G2 | B / DEL-T03-T04 | Seguridad, inventario y privacidad | PARCIAL |
| G3 | C / DEL-T05-T09 | Implementacion local | PASS parcial de alcance implementado |
| G4 | D | Pruebas automatizadas y conjuntas | PARCIAL |
| G5 | E / DEL-T10 | Evidencias y revision | EN REVISION |

## G0 - Autorizacion

Autorizado:

- Desarrollo y pruebas locales.
- Datos sinteticos.
- Commit y push de la rama autorizada.
- Actualizacion del PR Draft existente.
- GitHub Actions.

No autorizado:

- Merge a main.
- Despliegue productivo.
- Eliminacion de datos reales.
- Recursos adicionales de nube.
- Force push.

Presupuesto adicional autorizado: 0 MXN.

La ratificacion formal de documentos SDD
heredados permanece pendiente.

## G1 - Contrato

Contrato local:

contracts/openapi-delete-v04-local.yaml

Operaciones:

- DELETE /api/v1/users/me
- GET /api/v1/users/me/deletion
- POST /api/v1/auth/deletion/recovery
- POST /api/v1/users/me/deletion/cancel

Se preservan las siete operaciones anteriores.

Pendiente:

- Conciliacion documental con Ame.
- Identificacion del SHA definitivo.
- Ratificacion de compatibilidad Android.
- Ejecucion de los 28 escenarios conjuntos.

## G2 - Seguridad y privacidad

Controles implementados y probados:

- JWT ordinario y credencial de recuperacion separados.
- Invalidacion de sesiones anteriores.
- Rate limiting especifico.
- Bloqueo de nuevas solicitudes si no existe readiness.
- Recuperacion disponible para solicitudes existentes.
- Leases, fencing y transacciones.

Pendiente:

- Retencion de account_deletion_jobs.
- Tratamiento de identificadores.
- Backups y restauraciones.
- Excepciones legales.
- Inventario integral de recursos personales.
- Validacion TLS productiva.

## G3 - Implementacion

Implementacion local disponible:

- Solicitud atomica de eliminacion.
- Periodo exacto de siete dias.
- Recuperacion y cancelacion.
- Worker durable.
- Checkpoints persistentes.
- Renovacion de lease.
- Validacion de identidad del trabajo.
- Comprobacion transaccional de lectura.
- Presupuesto temporal cooperativo por etapa.

Worker deshabilitado por defecto.
Activacion restringida a Testing.
Repositorio destructivo restringido a MongoDB local.

El ADR G3 permanece pendiente de ratificacion.

## G4 - Verificacion

Evidencia historica:

- 148/148 en Debug.
- 148/148 en Release.
- 148/148 en CI anterior.

Evidencia local posterior:

- 166/166 en regresion Release.
- 2/2 casos adicionales de endurecimiento.

Suite esperada: 168 casos.

La nueva regresion completa permanece pendiente.

Pruebas conjuntas Android-Backend:
28 escenarios NOT_RUN.

## G5 - Evidencias y revision

Revision independiente anterior:
PASS LOCAL / PASS CI / OBSERVACIONES.

Las correcciones actuales requieren:

- Regresion final.
- Cobertura actualizada.
- SHA publicado.
- CI correspondiente al SHA publicado.
- Actualizacion del PR Draft #7.
- Ratificacion de coordinacion.

## Dictamen

Gates conciliados documentalmente mediante
equivalencia propuesta.

No existe aprobacion global de G0-G5.

Merge: NO AUTORIZADO.
Produccion: NO-GO.


## Actualizacion final de verificacion

La regresion Release posterior a las correcciones
aprobo 170/170 casos, sin fallos ni omisiones.

Cobertura consolidada de lineas:
2037 de 2650, equivalente a 76.87%.

La evidencia anterior conserva su valor historico.
Consultar evidencia-final-correcciones-pr7.md
para el estado actualizado.

CI del nuevo commit: pendiente de verificacion.
Aprobaciones pendientes y restricciones:
sin modificaciones.

Produccion: NO-GO.
