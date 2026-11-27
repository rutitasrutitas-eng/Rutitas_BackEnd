# MAPS-001 — Especificación

## 1. Objetivo
Incorporar una capa Backend para que Rutitas Android solicite búsqueda de ubicaciones, geocodificación, geocodificación inversa y cálculo de recorridos sin consumir Azure Maps directamente ni contener credenciales del proveedor.

## 2. Alcance autorizado
Incluye:
- Especificación y contrato OpenAPI candidato.
- Diseño de integración ASP.NET Core 10.
- DTO normalizados independientes del proveedor.
- Autenticación/autorización mediante el mecanismo JWT existente.
- Validación de entrada, límites de consumo, timeout y manejo de errores.
- Pruebas unitarias y de integración con respuestas simuladas.
- Evaluación documental de compatibilidad cartográfica y licencias.

No incluye:
- Llamadas reales a Azure Maps en esta fase.
- Nuevos recursos Azure, cambios de suscripción o gasto.
- Cambios a Android DEMO, AAB firmado o ANDROID-005.
- Seguimiento permanente de ubicación o navegación en segundo plano.
- Entrenamiento ML con resultados del proveedor.
- Caché o persistencia de resultados hasta revisar licencia.
- Commit, push, PR, merge o despliegue.

## 3. Endpoints candidatos
| Método | Ruta | Propósito |
|---|---|---|
| GET | `/api/v1/maps/search` | Buscar lugares/direcciones por texto |
| GET | `/api/v1/maps/geocode` | Convertir dirección a coordenadas |
| GET | `/api/v1/maps/reverse-geocode` | Convertir coordenadas a dirección |
| POST | `/api/v1/maps/directions` | Calcular recorrido origen-destino y waypoints |

Todos requieren JWT válido y autorización definida por Backend. La autorización final debe alinearse con la política existente de Rutitas.

## 4. DTO normalizados

### CoordinateDto
- `latitude`: decimal entre -90 y 90.
- `longitude`: decimal entre -180 y 180.

### PlaceDto
- `providerId`: identificador opaco del resultado cuando el proveedor lo entregue.
- `label`: nombre/dirección legible.
- `formattedAddress`: dirección normalizada.
- `position`: `CoordinateDto`.
- `countryCode`: código de país cuando exista.
- `resultType`: tipo de resultado cuando exista.

### DirectionsRequestDto
- `origin`: coordenada.
- `destination`: coordenada.
- `waypoints`: lista opcional y ordenada de coordenadas.
- `travelMode`: `driving` o `walking`.
- `departAt`: fecha/hora ISO 8601 opcional.

### DirectionsResponseDto
- `origin`, `destination`.
- `distanceMeters`: entero positivo.
- `durationSeconds`: entero positivo.
- `geometry`: GeoJSON LineString/MultiLineString o geometría normalizada equivalente.
- `provider`: identificador del proveedor.
- `calculatedAtUtc`: instante de cálculo.

No exponer tokens, headers internos, claves ni respuestas crudas completas del proveedor.

## 5. Requisitos funcionales
- RF-01: buscar lugares con texto no vacío y límites de longitud.
- RF-02: geocodificar una dirección.
- RF-03: resolver una dirección aproximada desde coordenadas.
- RF-04: calcular distancia, duración y geometría de un recorrido.
- RF-05: permitir que Android consuma exclusivamente los DTO del Backend.
- RF-06: distinguir errores de validación, autenticación, autorización, cuota, timeout y proveedor no disponible.

## 6. Requisitos no funcionales
- RNF-01: autenticación JWT del Backend en todos los endpoints.
- RNF-02: Azure Maps se autentica exclusivamente desde servidor mediante Microsoft Entra ID/Managed Identity en el entorno Azure autorizado.
- RNF-03: nunca incluir secretos o tokens en Android, logs o respuestas.
- RNF-04: timeout configurable y cancelación propagada.
- RNF-05: rate limiting por usuario autenticado y límite global de protección.
- RNF-06: logs estructurados sin coordenadas precisas ni direcciones completas salvo necesidad aprobada.
- RNF-07: no reintentar automáticamente errores 400/401/403/404; reintentos limitados solo para errores transitorios idempotentes y con backoff.
- RNF-08: no cachear ni persistir resultados hasta aprobación legal/licenciamiento.
- RNF-09: presupuesto adicional de $0 MXN.

## 7. Seguridad y autenticación
- Android → Rutitas: JWT existente.
- Rutitas → Azure Maps: Managed Identity + Microsoft Entra ID, cuando el Backend se ejecute en un recurso Azure compatible.
- Header `x-ms-client-id`: GUID de la cuenta Azure Maps, no el Client ID de la identidad administrada.
- Scope OAuth esperado: `https://atlas.microsoft.com/.default`.
- `Azure Maps Data Reader` debe verificarse en el ámbito del recurso y probarse con las operaciones no batch requeridas.
- No asumir que el rol permite operaciones batch: Microsoft documenta requisitos de rol distintos para batch.

## 8. Estados y errores
- `400`: parámetros o coordenadas inválidas.
- `401`: JWT ausente/inválido.
- `403`: usuario sin autorización.
- `404`: ubicación/resultado no encontrado cuando aplique.
- `429`: límite de Rutitas o proveedor excedido.
- `502`: respuesta inválida/error del proveedor.
- `503`: proveedor temporalmente no disponible o identidad no configurada.
- `504`: timeout del proveedor.
Los errores usarán `application/problem+json` y no revelarán detalles internos.

## 9. Criterios de aceptación
- CA-01: los cuatro endpoints aparecen en OpenAPI y tienen esquemas de request/response.
- CA-02: los cuatro endpoints rechazan solicitudes sin JWT.
- CA-03: entradas inválidas producen 400 con ProblemDetails.
- CA-04: pruebas simuladas cubren éxito, no encontrado, 429, 5xx y timeout.
- CA-05: no se realizan llamadas reales a Azure durante pruebas locales.
- CA-06: no hay secretos en código, configuración versionada, Android ni logs.
- CA-07: el rate limiting y timeout son configurables.
- CA-08: se verifica la asignación RBAC y la identidad antes de cualquier prueba real.
- CA-09: se resuelve por escrito la compatibilidad de licencia antes de combinar resultados Azure Maps con MapLibre u otro mapa de terceros.
- CA-10: no se crea ni despliega infraestructura y no se excede $0 MXN.

## 10. Dependencias y decisiones pendientes
- Confirmación de Ulises: Managed Identity correcta, Object/Principal ID, rol efectivo, scope del recurso y conectividad.
- Confirmación de cuotas/créditos gratuitos y presupuesto.
- Confirmar región/endpoint geográfico apropiado.
- Validar API versions habilitadas y operaciones cubiertas.
- Revisión de términos de Azure Maps para visualización, atribución, almacenamiento, caché y ML.
- Decisión Android entre Azure Maps Web SDK y MapLibre, condicionada por licencia y evaluación de Ame.
- Definir límites concretos de rate limit y timeout con coordinación.
