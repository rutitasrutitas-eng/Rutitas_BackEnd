# MAPS-001 — Plan técnico

## Fase 0 — Aislamiento y control
1. Mantener intacta la carpeta/working tree de DELETE-001.
2. No crear rama MAPS-001 desde una rama con cambios mezclados.
3. Preparar MAPS-001 en un directorio/branch independiente después de acordar el punto base con el equipo.
4. No commit/push/PR/merge/deploy sin autorización.

## Fase 1 — Contrato
1. Revisar el OpenAPI candidato con DaRa y Android.
2. Confirmar nombres de propiedades y códigos de error.
3. Confirmar la política de autorización JWT existente.
4. Congelar el contrato candidato antes de implementar.

## Fase 2 — Diseño Backend
Componentes propuestos:
- `MapsController`: endpoints HTTP, autorización y ProblemDetails.
- `IMapsProvider`: abstracción del proveedor.
- `AzureMapsProvider`: adaptador Azure Maps.
- `MockMapsProvider`: respuestas deterministas para pruebas.
- DTO propios: nunca filtrar modelos del SDK/proveedor.
- `MapsOptions`: endpoint, API versions, timeouts y límites no secretos.
- `TokenCredential`: Azure.Identity; ManagedIdentityCredential en Azure y DefaultAzureCredential solo en entorno local autorizado.
- `HttpClient` tipado con timeout y cancelación.

No se recomienda persistir resultados en MongoDB ni activar caché hasta verificar condiciones de licencia.

## Fase 3 — Integración upstream
Search:
- Búsqueda de lugares/POI: Fuzzy Search API `1.0` (`/search/fuzzy/json`).
- Geocoding: Search API `2026-01-01`, operación task-specific.
- Reverse geocoding: Search API `2026-01-01`, operación task-specific.
- Confirmar los paths y parámetros definitivos de cada operación al congelar el contrato upstream.
Route:
- POST Route Directions.
- Enviar GeoJSON FeatureCollection con coordenadas en orden longitud, latitud.
- Normalizar distancia, duración y geometría.
- Mantener versiones Search y Route configurables/aisladas.

## Fase 4 — Controles
- JWT obligatorio en endpoints públicos de Rutitas.
- Rate limiting por identidad autenticada y límite global.
- Validación estricta de query, país y coordenadas.
- Timeout, cancelación, circuit breaker solo si se justifica.
- Logs sin tokens ni datos de ubicación sensibles.
- Errores del proveedor traducidos a ProblemDetails.
- No reintentar 400/401/403; limitar reintentos de 429/5xx según `Retry-After` y política aprobada.

## Fase 5 — Pruebas sin Azure
- Unitarias de validadores y normalizadores.
- Controller tests de JWT, autorización y ProblemDetails.
- Tests del provider con `HttpMessageHandler` falso.
- Contrato OpenAPI.
- Casos: 200, 400, 401, 403, 404, 429, 500/502, timeout y cancelación.
- Verificar que no se envía tráfico externo durante la suite.

## Fase 6 — Gate para prueba real
No ejecutar hasta que Ulises confirme:
- Identidad administrada adjunta al recurso de cómputo.
- Principal/Object ID correcto.
- RBAC efectivo al scope de la cuenta Maps.
- `x-ms-client-id` de la cuenta Maps correcto.
- Cuotas y presupuesto confirmados.
- Recurso de ejecución disponible y autorizado.
- Aprobación expresa de DaRa para la prueba real.

## Fase 7 — Gate de visualización
Ame evalúa Azure Maps Web SDK y MapLibre. La combinación de datos/recorridos de Azure Maps con mapas de terceros queda bloqueada hasta revisión de licencia y atribución. No usar resultados de Azure Maps para ML antes de autorización de licencia y privacidad.
