# CONTENT-SAFETY-001 - Evidencia de validacion

Fecha: 2026-11-26
Rama: feature/content-safety-001
Base: a95e62e2f0f9fbde46af0144c9fb89c858670c94

Estado: PASS DOCUMENTAL LOCAL.
Implementacion funcional: NO AUTORIZADA.
Produccion: NO-GO.

## Objetivo

Preparar la arquitectura y contrato candidato
para integrar moderacion de contenido en Rutitas,
sin llamadas reales a Azure y sin gastos adicionales.

## Contrato candidato

OpenAPI: 3.1.0
Version: 0.1.0-candidate
Base API: /api/v1

Operaciones propuestas:

- POST /api/v1/moderation/text
- POST /api/v1/moderation/image

Decisiones normalizadas:

- approved
- rejected
- pending_review

Las operaciones requieren autenticacion JWT.

El resultado de moderacion no constituye por
si mismo una autorizacion de publicacion.

## Politica candidata

- Severidad 0: approved.
- Severidad 2: pending_review.
- Severidad 4 o 6: rejected.
- Respuesta desconocida o incompleta:
  nunca aprobar automaticamente.

Categorias consideradas:

- Hate.
- Sexual.
- Violence.
- SelfHarm.

La politica requiere ratificacion del equipo.

## Resultados de validacion previa

| Indicador | Resultado |
|---|---:|
| Pruebas de contrato | 16 |
| Aprobadas | 16 |
| Fallidas | 0 |
| Omitidas | 0 |
| Escenarios funcionales ejecutados | 0 |
| Escenarios funcionales pendientes | 30 |
| Llamadas reales a Azure | 0 |

Resultado: 16/16 PASS LOCAL.

La validacion corresponde a estructura OpenAPI,
esquemas y ejemplos sinteticos.

No acredita endpoints HTTP implementados,
moderacion efectiva de imagenes ni consumo real
de Azure AI Content Safety.

## Controles propuestos

- Validacion de entradas.
- Autenticacion y autorizacion.
- Rate limiting.
- Timeout de proveedor.
- Fail-closed antes de publicar contenido.
- Proteccion contra decisiones manipuladas.
- Identidad y version del contenido moderado.
- Ausencia de claves de Azure en Android.
- Ausencia de contenido sensible en logs.
- Politica centralizada y versionada.

## Limites candidatos

- Texto: hasta 2000 caracteres.
- Imagen: hasta 4 MiB.
- Formatos: JPEG y PNG.
- Dimensiones propuestas: hasta 4096 x 4096.
- Timeout propuesto: 8 segundos.
- Texto: 10 solicitudes por minuto por usuario.
- Imagen: 5 solicitudes por minuto por usuario.

Estos limites no representan una confirmacion
de cuotas o capacidades definitivas del proveedor.

## Configuracion prevista

ContentSafety:Enabled=false

Proveedor simulado previsto para una futura
implementacion autorizada.

No existe adaptador funcional implementado.

## Dependencias de validacion

- PyYAML 6.0.2
- jsonschema 4.23.0
- openapi-spec-validator 0.7.2

Definidas en tests/requirements.txt.

## Pendientes

- Ratificacion del contrato con Android.
- Revision de seguridad y privacidad.
- Aprobacion de politica de moderacion.
- Verificacion de permisos y cuotas de Azure.
- Ratificacion de version API del proveedor.
- Autorizacion de implementacion funcional.
- Implementacion del proveedor simulado.
- Controles efectivos de publicacion.
- Ejecucion de los 30 escenarios funcionales.
- Revision independiente.
- Integracion de pruebas documentales en CI,
  previa autorizacion.

## Restricciones

- Sin llamadas reales a Azure.
- Sin secretos en el repositorio.
- Sin datos personales reales.
- Sin modificaciones de DELETE-001.
- Sin modificaciones de MAPS-001.
- Sin cambios de workflows.
- Sin despliegue.
- Sin merge.
- Sin recursos adicionales.

Presupuesto adicional autorizado: 0 MXN.

## Dictamen

PASS DOCUMENTAL LOCAL: 16/16.

Contrato candidato, no API funcional.
Pendiente revision y autorizacion de publicacion.
Produccion: NO-GO.
