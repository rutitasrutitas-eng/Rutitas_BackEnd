# Inventario del análisis C#

Fecha: 2026-11-13. Rama: `fix/mark-api-devsecops-review`. Base integrada: `0f05c07d099c8a4bb396719d804ec0698a29254a`. Este inventario procede del build Release de la solución con `EnableNETAnalyzers=true`, `AnalysisMode=All` y `RunAnalyzersDuringBuild=true`. Resultado del build: 0 errores, 186 advertencias reportadas por MSBuild. La tabla agrupa ubicaciones únicas archivo/regla/línea para evitar contar repetidamente una advertencia cuando MSBuild vuelve a evaluar un proyecto referenciado.

Las prioridades indican orden de revisión, no severidad asignada por el analizador: P1 seguridad/robustez productiva; P2 corrección y consistencia técnica; P3 estilo, API o convenciones. No se aplicaron supresiones globales.

## Resumen por regla

| Prioridad | Regla | Ubicaciones únicas | Evaluación |
|---|---|---:|---|
| P2 | `CA1001` | 1 | pendiente de revisión contextual |
| P2 | `CA1002` | 1 | pendiente de revisión contextual |
| P1 | `CA1031` | 5 | revisar cada frontera; se conservaron catches necesarios para respuestas seguras/fail-closed |
| P3 | `CA1032` | 1 | pendiente de revisión contextual |
| P2 | `CA1054` | 1 | pendiente de revisión contextual |
| P1 | `CA1062` | 4 | guardas agregadas en controladores/repositorios; remanentes en fixtures y pruebas |
| P3 | `CA1305` | 2 | pendiente de revisión contextual |
| P3 | `CA1307` | 2 | pendiente de revisión contextual |
| P2 | `CA1308` | 5 | no se cambió normalización de email sin prueba; revisión pendiente |
| P3 | `CA1515` | 18 | pendiente de revisión contextual |
| P3 | `CA1707` | 49 | subrayados en nombres de pruebas/contratos; mantener legibilidad por ahora |
| P3 | `CA1724` | 1 | pendiente de revisión contextual |
| P3 | `CA1725` | 9 | pendiente de revisión contextual |
| P3 | `CA1819` | 2 | pendiente de revisión contextual |
| P3 | `CA1848` | 5 | pendiente de revisión contextual |
| P3 | `CA1849` | 1 | pendiente de revisión contextual |
| P3 | `CA1859` | 4 | pendiente de revisión contextual |
| P3 | `CA1860` | 3 | pendiente de revisión contextual |
| P2 | `CA2007` | 39 | ConfigureAwait contextual; revisar, sin cambio masivo |
| P3 | `CA2227` | 1 | pendiente de revisión contextual |
| P2 | `CA2234` | 26 | uso de URI en pruebas; P2 de mantenibilidad |

## Ubicaciones por archivo y regla

| Prioridad | Archivo | Regla | Ubicaciones |
|---|---|---:|---:|
| P1 | `Rutitas.Api\Rutitas.Api\Controllers\AuthController.cs` | `CA1031` | 3 |
| P3 | `Rutitas.Api\Rutitas.Api\Controllers\AuthController.cs` | `CA1305` | 1 |
| P2 | `Rutitas.Api\Rutitas.Api\Controllers\AuthController.cs` | `CA1308` | 2 |
| P3 | `Rutitas.Api\Rutitas.Api\Controllers\AuthController.cs` | `CA1515` | 5 |
| P3 | `Rutitas.Api\Rutitas.Api\Controllers\AuthController.cs` | `CA1848` | 3 |
| P3 | `Rutitas.Api\Rutitas.Api\Controllers\AuthController.cs` | `CA1859` | 2 |
| P2 | `Rutitas.Api\Rutitas.Api\Controllers\AuthController.cs` | `CA2007` | 4 |
| P3 | `Rutitas.Api\Rutitas.Api\Controllers\RoutesController.cs` | `CA1515` | 6 |
| P3 | `Rutitas.Api\Rutitas.Api\Controllers\RoutesController.cs` | `CA1819` | 2 |
| P2 | `Rutitas.Api\Rutitas.Api\Controllers\RoutesController.cs` | `CA2007` | 2 |
| P3 | `Rutitas.Api\Rutitas.Api\Controllers\UsersController.cs` | `CA1515` | 1 |
| P3 | `Rutitas.Api\Rutitas.Api\Controllers\UsersController.cs` | `CA1859` | 2 |
| P2 | `Rutitas.Api\Rutitas.Api\Controllers\UsersController.cs` | `CA2007` | 2 |
| P1 | `Rutitas.Api\Rutitas.Api\Program.cs` | `CA1031` | 2 |
| P3 | `Rutitas.Api\Rutitas.Api\Program.cs` | `CA1515` | 1 |
| P3 | `Rutitas.Api\Rutitas.Api\Program.cs` | `CA1848` | 2 |
| P3 | `Rutitas.Api\Rutitas.Api\Program.cs` | `CA1849` | 1 |
| P2 | `Rutitas.Api\Rutitas.Api\Program.cs` | `CA2007` | 6 |
| P3 | `Rutitas.Api\Rutitas.Application\Exceptions\DuplicateEmailException.cs` | `CA1032` | 1 |
| P2 | `Rutitas.Api\Rutitas.Domain\Entities\Route.cs` | `CA1002` | 1 |
| P3 | `Rutitas.Api\Rutitas.Domain\Entities\Route.cs` | `CA2227` | 1 |
| P2 | `Rutitas.Api\Rutitas.Infrastructure\Repositories\MongoRouteRepository.cs` | `CA1308` | 1 |
| P2 | `Rutitas.Api\Rutitas.Infrastructure\Repositories\MongoRouteRepository.cs` | `CA2007` | 3 |
| P2 | `Rutitas.Api\Rutitas.Infrastructure\Repositories\MongoTokenRevocationRepository.cs` | `CA2007` | 2 |
| P2 | `Rutitas.Api\Rutitas.Infrastructure\Repositories\MongoUserRepository.cs` | `CA1308` | 2 |
| P2 | `Rutitas.Api\Rutitas.Infrastructure\Repositories\MongoUserRepository.cs` | `CA2007` | 5 |
| P3 | `Rutitas.Api\tests\Rutitas.ContractTests\RouteContractTests.cs` | `CA1707` | 1 |
| P3 | `Rutitas.Api\tests\Rutitas.ContractTests\RouteContractTests.cs` | `CA1860` | 3 |
| P3 | `Rutitas.Api\tests\Rutitas.IntegrationTests\ApiFlowTests.cs` | `CA1707` | 1 |
| P2 | `Rutitas.Api\tests\Rutitas.IntegrationTests\ApiFlowTests.cs` | `CA2007` | 3 |
| P2 | `Rutitas.Api\tests\Rutitas.IntegrationTests\ApiFlowTests.cs` | `CA2234` | 5 |
| P3 | `Rutitas.Api\tests\Rutitas.IntegrationTests\Endpoints\EndpointIntegrationTests.cs` | `CA1307` | 2 |
| P3 | `Rutitas.Api\tests\Rutitas.IntegrationTests\Endpoints\EndpointIntegrationTests.cs` | `CA1707` | 7 |
| P2 | `Rutitas.Api\tests\Rutitas.IntegrationTests\Endpoints\EndpointIntegrationTests.cs` | `CA2007` | 4 |
| P2 | `Rutitas.Api\tests\Rutitas.IntegrationTests\Endpoints\EndpointIntegrationTests.cs` | `CA2234` | 7 |
| P2 | `Rutitas.Api\tests\Rutitas.IntegrationTests\MongoRepositoryTests.cs` | `CA1001` | 1 |
| P3 | `Rutitas.Api\tests\Rutitas.IntegrationTests\MongoRepositoryTests.cs` | `CA1515` | 1 |
| P3 | `Rutitas.Api\tests\Rutitas.IntegrationTests\MongoRepositoryTests.cs` | `CA1707` | 3 |
| P2 | `Rutitas.Api\tests\Rutitas.IntegrationTests\MongoRepositoryTests.cs` | `CA2007` | 2 |
| P1 | `Rutitas.Api\tests\Rutitas.IntegrationTests\TestApiFactory.cs` | `CA1062` | 3 |
| P3 | `Rutitas.Api\tests\Rutitas.IntegrationTests\TestApiFactory.cs` | `CA1515` | 4 |
| P3 | `Rutitas.Api\tests\Rutitas.IntegrationTests\TestApiFactory.cs` | `CA1725` | 9 |
| P3 | `Rutitas.Api\tests\Rutitas.SecurityTests\Endpoints\EndpointSecurityTests.cs` | `CA1707` | 7 |
| P2 | `Rutitas.Api\tests\Rutitas.SecurityTests\Endpoints\EndpointSecurityTests.cs` | `CA2007` | 3 |
| P2 | `Rutitas.Api\tests\Rutitas.SecurityTests\Endpoints\EndpointSecurityTests.cs` | `CA2234` | 6 |
| P3 | `Rutitas.Api\tests\Rutitas.SecurityTests\JwtValidationTests.cs` | `CA1707` | 3 |
| P2 | `Rutitas.Api\tests\Rutitas.SecurityTests\JwtValidationTests.cs` | `CA2007` | 3 |
| P2 | `Rutitas.Api\tests\Rutitas.SecurityTests\JwtValidationTests.cs` | `CA2234` | 3 |
| P2 | `Rutitas.Api\tests\Rutitas.SecurityTests\SecurityTests.cs` | `CA1054` | 1 |
| P3 | `Rutitas.Api\tests\Rutitas.SecurityTests\SecurityTests.cs` | `CA1707` | 6 |
| P3 | `Rutitas.Api\tests\Rutitas.SecurityTests\SecurityTests.cs` | `CA1724` | 1 |
| P2 | `Rutitas.Api\tests\Rutitas.SecurityTests\SecurityTests.cs` | `CA2234` | 5 |
| P3 | `Rutitas.Api\tests\Rutitas.UnitTests\ControllerUnitTests.cs` | `CA1305` | 1 |
| P3 | `Rutitas.Api\tests\Rutitas.UnitTests\ControllerUnitTests.cs` | `CA1707` | 17 |
| P1 | `Rutitas.Api\tests\Rutitas.UnitTests\Endpoints\EndpointUnitCoverageTests.cs` | `CA1062` | 1 |
| P3 | `Rutitas.Api\tests\Rutitas.UnitTests\Endpoints\EndpointUnitCoverageTests.cs` | `CA1707` | 4 |

## Cierre de correcciones prioritarias

- **CA1062:** se añadieron validaciones de argumentos en `AuthController` y repositorios Mongo. Las ubicaciones que restan están en infraestructura de pruebas (`TestApiFactory` y pruebas unitarias), no en los controladores/repositorios productivos.
- **CA1031:** quedan 5 ubicaciones únicas en `AuthController` y `Program.cs`. No se reemplazaron con excepciones genéricas ni se ocultaron; Mark debe revisar que cada frontera preserve la respuesta segura, el registro y el comportamiento fail-closed.
- **CA2000:** se corrigieron tres usos de `HttpContent` sin liberación en pruebas de integración/seguridad. Esta regla no aparece entre los diagnósticos restantes.
- **CA1308:** se dejó intacta la normalización de correo. Antes de cualquier cambio se requiere una prueba de regresión que cubra registro, búsqueda y duplicados con variantes Unicode/case.
- **CA2007, CA1707, CA2234:** no se aplicaron cambios indiscriminados; muchas ubicaciones pertenecen a pruebas. Evaluar por contexto en una revisión posterior.

El inventario es una fotografía del build indicado, no una aprobación de la calidad total ni de las decisiones de arquitectura. CodeQL y Trivy no se pudieron ejecutar localmente; véase [VALIDACION_MARK.md](VALIDACION_MARK.md).
