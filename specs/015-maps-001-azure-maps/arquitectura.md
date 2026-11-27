# MAPS-001 - Arquitectura candidata

Estado: CANDIDATE
Stack: ASP.NET Core 10
Proveedor propuesto: Azure Maps Gen2

## Flujo

Android -> API Rutitas -> Application -> IMapsProvider
-> Infrastructure -> Azure Maps

## Responsabilidades

- Api: controllers, autenticacion JWT, autorizacion,
  validacion HTTP, rate limiting y ProblemDetails.
- Application: contratos, DTO, validadores y casos de uso.
- Domain: reglas de negocio cuando corresponda.
- Infrastructure: adaptador Azure Maps y HttpClient.
- Tests: proveedores simulados y respuestas deterministas.

## Componentes candidatos

- MapsController
- IMapsProvider
- AzureMapsProvider
- MockMapsProvider
- MapsOptions
- CoordinateDto
- PlaceDto
- DirectionsRequestDto
- DirectionsResponseDto

## Identidad

Android utiliza exclusivamente el JWT de Rutitas
para consumir sus endpoints geoespaciales.

Backend utiliza Azure.Identity y ManagedIdentityCredential
en el entorno Azure autorizado.

Distinguir:

- MapsAccountClientId: identificador de la cuenta Maps.
- ManagedIdentityClientId: identidad administrada.
- PrincipalId: objeto para asignaciones RBAC.

Scope OAuth candidato:
https://atlas.microsoft.com/.default

El header x-ms-client-id corresponde a la cuenta Maps.

La configuracion de identidad y RBAC requiere
validacion independiente de Infraestructura.

## Aislamiento

- No dependencia funcional de DELETE-001.
- No modificar ANDROID-005.
- No almacenar credenciales en Android.
- No modificar servicios de produccion.
- No crear recursos adicionales.
- Adaptador real deshabilitado hasta autorizacion.

## Visualizacion

MapLibre Native y Azure Maps Web SDK son candidatos.
La decision permanece pendiente de licenciamiento,
seguridad, rendimiento y evaluacion Android.
