# MAPS-001 — Lista de tareas

## Documentación y coordinación
- [ ] T01 Confirmar repositorio/branch base independiente de DELETE-001.
- [ ] T02 Revisar contrato OpenAPI candidato con DaRa.
- [ ] T03 Confirmar DTO con Android.
- [ ] T04 Obtener de Ulises confirmación de identidad, principal ID, RBAC y scope.
- [ ] T05 Confirmar cuotas gratuitas, alertas y costo $0.
- [ ] T06 Revisar términos de licencia de Azure Maps y atribución.
- [ ] T07 Resolver compatibilidad MapLibre / Azure Maps con Ame.

## Contrato
- [ ] T08 Validar `/maps/search`.
- [ ] T09 Validar `/maps/geocode`.
- [ ] T10 Validar `/maps/reverse-geocode`.
- [ ] T11 Validar `/maps/directions`.
- [ ] T12 Congelar schemas y ProblemDetails.

## Backend — solo después de aprobar el contrato
- [ ] T13 Crear DTO y validadores.
- [ ] T14 Crear interfaz `IMapsProvider`.
- [ ] T15 Crear `MockMapsProvider`.
- [ ] T16 Crear controlador protegido por JWT.
- [ ] T17 Implementar rate limiting configurable.
- [ ] T18 Implementar timeout y cancelación.
- [ ] T19 Implementar adaptador Azure.Identity.
- [ ] T20 Implementar adaptador Azure Maps Search.
- [ ] T21 Implementar adaptador Azure Maps Route.
- [ ] T22 Normalizar resultados y errores.

## Calidad
- [ ] T23 Pruebas unitarias.
- [ ] T24 Pruebas HTTP simuladas.
- [ ] T25 Pruebas de autenticación/autorización.
- [ ] T26 Pruebas de rate limit, timeout y cancelación.
- [ ] T27 Validación OpenAPI.
- [ ] T28 Confirmar cero llamadas externas en pruebas locales.
- [ ] T29 Revisión de secretos, logs y datos personales.

## Gate de prueba real y entrega
- [ ] T30 Confirmación RBAC/Managed Identity por Ulises.
- [ ] T31 Confirmación de cuotas y presupuesto.
- [ ] T32 Autorización expresa de DaRa para llamada real.
- [ ] T33 Ejecutar una prueba controlada y documentar consumo.
- [ ] T34 Obtener autorización separada para commit/push/PR.
- [ ] T35 No desplegar sin aprobación expresa.
