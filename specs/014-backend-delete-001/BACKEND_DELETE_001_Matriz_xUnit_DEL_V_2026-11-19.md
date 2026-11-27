# BACKEND-DELETE-001 — Correspondencia preliminar de escenarios DEL-V con xUnit

**Fecha de inventario:** 19/11/2026
**Fuente:** `inventario_pruebas_xunit_2026-11-19.txt`
**Alcance:** descubrimiento de pruebas y asignación por nombre; **no** equivale a ejecución ni a revisión de `Assert`.

## Resumen

| Suite | Pruebas descubiertas |
|---|---:|
| Rutitas.UnitTests | 38 |
| Rutitas.SecurityTests | 26 |
| Rutitas.ContractTests | 9 |
| Rutitas.ArchitectureTests | 2 |
| Rutitas.IntegrationTests | 55 |
| **Total** | **130** |

Las 130 pruebas aprobaron en las últimas ejecuciones exitosas reportadas previamente, pero el archivo de descubrimiento no certifica por separado el resultado de cada escenario DEL-V.

**Android:** las 28 pruebas conjuntas continúan en `NOT_RUN`. Las relaciones siguientes son únicamente con pruebas locales del Backend.

## Matriz DEL-V → pruebas candidatas

| Escenario | Descripción abreviada | Coincidencias por nombre | Evaluación documental |
|---|---|---|---|
| DEL-V01 | Solicitud autenticada 202 y fecha UTC+7 días | `AccountDeletionHttpTests.DeleteWithValidJwt_Returns202AndBlocksOldSession`<br>`AccountDeletionMongoTests.ScheduleDeletion_StoresAtomicTransition`<br>`AccountDeletionControllerTests.ValidAccount_Returns202WithContractFields` | Fecha exacta y contrato deben revisarse en Assert. |
| DEL-V02 | Solicitud sin JWT: 401 sin cambios | `AccountDeletionHttpTests.DeleteWithoutJwt_Returns401` | Comprobar que también se afirme ausencia de mutación. |
| DEL-V03 | Solicitud duplicada sin renovar plazo | `AccountDeletionMongoTests.RepeatedRequest_DoesNotExtendDeadline` | Comprobar que no cree trabajo duplicado. |
| DEL-V04 | JWT anterior bloqueado tras pending_deletion | `AccountDeletionHttpTests.DeleteWithValidJwt_Returns202AndBlocksOldSession`<br>`JwtValidationTests.InactiveUser_CannotAccessRoutesWithValidToken` | Revisar cobertura de perfil, rutas y todos los recursos protegidos. |
| DEL-V05 | Token restringido rechazado en perfil/rutas | `DeletionV04RouteContractTests.RecoveryEndpoints_UseRestrictedTokenHandling`<br>`JwtValidationTests.InactiveUser_CannotAccessRoutesWithValidToken` | No se identifica prueba HTTP inequívoca de inyección del token restringido en endpoints ordinarios. |
| DEL-V06 | Reautenticación, rate limit, respuesta sin enumeración | `DeletionRecoveryHttpTests.PendingAccount_CanGetOpaqueRecoveryToken`<br>`DeletionRecoveryHttpTests.WrongPassword_Returns401`<br>`DeletionRecoveryHttpTests.UnknownAndWrongPassword_HaveSamePublicResponse`<br>`DeletionRateLimitContractHttpTests.Recovery_Request6_Returns429ProblemDetails` | Revisar longitud/expiración exacta de 300 s y respuestas completas. |
| DEL-V07 | Cancelación antes de plazo: active + consumo | `DeletionCancellationHttpTests.ValidToken_RestoresAccountAndConsumesToken`<br>`DeletionCancellationMongoTests.OneSecondBeforeSevenDays_CancellationSucceeds` | Comprobar revocación de sesiones previas. |
| DEL-V08 | Cancelación al vencer o después: rechazo | `DeletionCancellationMongoTests.ExactlyAtSevenDays_CancellationIsRejected`<br>`DeletionCancellationHttpTests.ExpiredDeletionDeadline_Returns410`<br>`DeletionServerClockMongoTests.StaleApplicationClock_CannotCancelExpiredDeletion` | Cobertura nominal explícita. |
| DEL-V09 | Borrado integral al vencer y preservación de terceros | **Sin coincidencia específica** | Sin prueba específica identificada; worker y purga integral no acreditados. |
| DEL-V10 | JWT anterior rechazado tras borrado definitivo | **Sin coincidencia específica** | OldSessionVersion_RemainsInvalidAfterVersionChange trata revocación, no demuestra borrado definitivo. |
| DEL-V11 | Falla temporal Mongo y recuperación de borrado | `DeletionCancellationHttpTests.FailureDuringStatusRead_Returns503ProblemDetails` | Esta prueba cubre lectura de estado, no retry de borrado definitivo. |
| DEL-V12 | Reinicio durante deleting y reanudación | **Sin coincidencia específica** | Sin prueba específica identificada. |
| DEL-V13 | Cancelación frente a worker concurrente | `DeletionCancellationMongoTests.ParallelCancellation_OnlyOneMongoUpdateWins`<br>`DeletionCancellationHttpTests.DeadlineChangesDuringCancellation_Returns410` | Pruebas de cancelaciones concurrentes, no de carrera con worker de borrado. |
| DEL-V14 | Dos workers simultáneos sin duplicar purga | **Sin coincidencia específica** | Sin prueba específica identificada. |
| DEL-V15 | Regresión de siete endpoints anteriores | `Endpoints.EndpointIntegrationTests.IT_01_Register_Returns201AndPublicProfile`<br>`Endpoints.EndpointIntegrationTests.IT_02_Login_ReturnsBearerToken`<br>`Endpoints.EndpointIntegrationTests.IT_03_GetProfile_ReturnsAuthenticatedUser`<br>`Endpoints.EndpointIntegrationTests.IT_04_PatchProfile_UpdatesDisplayName`<br>`Endpoints.EndpointIntegrationTests.IT_05_Logout_RevokesPreviouslyValidToken`<br>`Endpoints.EndpointIntegrationTests.IT_06_ListRoutes_ReturnsPagedJson`<br>`Endpoints.EndpointIntegrationTests.IT_07_GetRouteById_ReturnsDetail` | Cobertura backend nominal; Android conjunto NOT_RUN. |
| DEL-V16 | Retención, respaldos y excepciones aprobadas | **Sin coincidencia específica** | Políticas pendientes de ratificación. |
| DEL-V17 | JWT anterior inválido tras cancelar | `JwtValidationTests.OldSessionVersion_RemainsInvalidAfterVersionChange`<br>`DeletionCancellationHttpTests.ValidToken_RestoresAccountAndConsumesToken` | Revisar que prueba reproduzca explícitamente cancelar y reutilizar JWT antiguo. |
| DEL-V18 | Token consumido no reutilizable | `DeletionCancellationMongoTests.ReusedToken_CannotCancelAgain` | Cobertura nominal explícita. |
| DEL-V19 | Cancelar exactamente al vencimiento | `DeletionCancellationMongoTests.ExactlyAtSevenDays_CancellationIsRejected` | Cobertura nominal explícita. |
| DEL-V20 | Solicitudes simultáneas: una transición | `AccountDeletionMongoTests.ConcurrentRequests_OnlyOneTransitionWins` | Cobertura nominal explícita. |
| DEL-V21 | Error durante borrado definitivo: recuperación | **Sin coincidencia específica** | No confundir 503 de lectura con reanudación de purga. |
| DEL-V22 | Token de recuperación rechazado en endpoints normales | `DeletionV04RouteContractTests.RecoveryEndpoints_UseRestrictedTokenHandling` | Prueba de manejo restringido, no demuestra por nombre acceso HTTP a rutas/perfil; respuesta esperada debe conciliarse con contrato. |
| DEL-V23 | Login ordinario bloqueado en pending_deletion/deleting | `JwtValidationTests.InactiveUser_CannotAccessRoutesWithValidToken`<br>`AccountDeletionHttpTests.DeleteWithValidJwt_Returns202AndBlocksOldSession` | No se identifica prueba explícita de login en ambos estados. |
| DEL-V24 | Respuesta 200 de cancelación perdida: login nuevo verifica | `DeletionCancellationHttpTests.ValidToken_RestoresAccountAndConsumesToken` | No se identifica simulación explícita de respuesta perdida y posterior login. |
| DEL-V25 | Reautenticación tras caducar token sin extender plazo | `DeletionStatusHttpTests.ExpiredRecoveryToken_Returns401`<br>`DeletionRecoveryHttpTests.NewRecoveryToken_InvalidatesPreviousHash`<br>`DeletionIssueServerClockMongoTests.StaleClock_CannotIssueTokenAfterDeletionDeadline` | Hay pruebas relacionadas; revisar flujo completo con token expirado antes del plazo. |
| DEL-V26 | Token vencido no se reutiliza ni se reemite tras plazo | `DeletionCancellationMongoTests.ExpiredToken_CannotRestoreAccount`<br>`DeletionIssueServerClockMongoTests.StaleClock_CannotIssueTokenAfterDeletionDeadline` | Comprobar rechazo de reemisión y mutación con reloj de servidor. |
| DEL-V27 | 409 transitorio frente a 410 definitivo | `DeletionCancellationHttpTests.AtomicCancellationConflict_Returns409`<br>`DeletionCancellationHttpTests.ExpiredDeletionDeadline_Returns410`<br>`DeletionStatusHttpTests.DeletingAccount_Returns410` | Cobertura nominal de respuestas, falta comprobar interpretación integral. |
| DEL-V28 | GET estado tras plazo: 200 y recoveryAllowed=false | `DeletionStatusHttpTests.AfterDeletionDeadline_RecoveryIsNotAllowed` | Verificar status 200 y token previo vigente en los Assert. |

## Diferencias y pendientes

- `DEL-V09`, `DEL-V12`, `DEL-V14`, `DEL-V16` y `DEL-V21`: no se localizaron pruebas nominales de purga integral, reanudación de worker o ratificación de políticas.
- `DEL-V10`: la revocación de sesiones no es evidencia de revocación posterior a una purga definitiva.
- `DEL-V11`: existe una prueba de error HTTP 503 en consulta, pero no se acredita retry de trabajo destructivo.
- `DEL-V13`: la concurrencia entre cancelaciones no prueba una carrera contra un worker de eliminación.
- `DEL-V05` y `DEL-V22`: hace falta revisar pruebas y Assertions para confirmar rechazo efectivo del token restringido en todos los endpoints ordinarios.
- `DEL-V23` y `DEL-V24`: no se observan por nombre pruebas completas de ambos estados de login ni de respuesta perdida seguida de nuevo login.
- `DEL-V28`: verificar que el Assertion confirme específicamente `200` y `recoveryAllowed=false` con token vigente.
- `DEL-R04` en `spec.md` aún describe alcance histórico solo `deletion:cancel`; debe conciliarse con uso restringido de status y cancelación de v0.4.
- `DEL-V22` en `tests.md` contempla `401 o 403`; revisar texto respecto del contrato vigente sin inferir que todas las respuestas de todos los endpoints sean idénticas.
- Seguimiento documental de Mongo intermitente (causa desconocida) y cobertura parcial HTTP `503`.

## Límites de aprobación

- No autoriza pruebas conjuntas Android–Backend, habilitar acciones sensibles, ni cambios de red o credenciales.
- No autoriza datos reales, commits, push, PR, merge ni despliegue.
- Pendientes las ratificaciones de respaldos, datos compartidos, excepciones legales y publicación.

## Siguiente validación

Revisar en los archivos `.cs` los `Assert` y la configuración de cada prueba candidata; documentar después, escenario por escenario, los resultados de ejecuciones concretas y sus logs. No marcar `PASS` a partir del nombre de un test.
