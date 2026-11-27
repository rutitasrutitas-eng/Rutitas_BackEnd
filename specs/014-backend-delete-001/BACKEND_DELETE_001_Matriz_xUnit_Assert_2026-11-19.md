# BACKEND-DELETE-001 — Conciliación DEL-V ↔ xUnit revisada en código

**Fecha documental:** 19/11/2026
**Fuente examinada:** ZIP de 25 archivos `.cs` de las cinco suites aportado por el usuario.
**Alcance:** revisión estática del contenido de métodos de prueba y sus aserciones; **no se ejecutó .NET ni MongoDB** durante esta revisión.

## Resumen

- **14** escenarios con evidencia directa sobre el comportamiento principal (a veces solo local/sintética).
- **8** escenarios con evidencia parcial o varios subcasos sin cerrar.
- **6** escenarios sin prueba específica del comportamiento central.

**Aclaracion de clasificacion:** DEL-V06 y DEL-V26 tienen evidencia directa de subcasos especificos, pero su cobertura global se clasifica como parcial. Por eso forman parte de los 8 escenarios parciales y no de los 14 directos.
- Los **134 PASS** proceden de ejecuciones exitosas anteriores declaradas por el equipo; **no** de este ZIP ni de una nueva ejecución.
- **28/28 escenarios Android–Backend siguen `NOT_RUN`**: los DEL-V documentales de Backend no son equivalentes a resultados de integración Android.

## Matriz de los 28 escenarios

| ID | Cobertura inferida del código | Métodos xUnit de soporte | Qué verifican los `Assert` | Limitación |
|---|---|---|---|---|
| DEL-V01 | Directa | AccountDeletionHttpTests.DeleteWithValidJwt_Returns202AndBlocksOldSession; AccountDeletionMongoTests.ScheduleDeletion_StoresAtomicTransition | Assert 202, pending_deletion, cuatro campos de respuesta y +7 dias; Mongo verifica estado y version. | Implementacion y asserts locales; sin comparar todas las respuestas contra YAML en vivo. |
| DEL-V02 | Directa | AccountDeletionHttpTests.DeleteWithoutJwt_Returns401 | Assert HTTP 401 sin JWT; no hay cuenta creada en esta prueba. | La ausencia de mutacion se infiere por no haber cuenta creada; no se audita persistencia. |
| DEL-V03 | Directa | AccountDeletionMongoTests.RepeatedRequest_DoesNotExtendDeadline; AccountDeletionMongoTests.ConcurrentRequests_OnlyOneTransitionWins; AccountDeletionHttpTests.DeleteWithValidJwt_Returns202AndBlocksOldSession | Assert segunda solicitud sin transicion y fecha original; una sola transicion concurrente. | No demuestra ausencia de todo trabajo futuro duplicado (worker no acreditado). |
| DEL-V04 | Parcial | AccountDeletionHttpTests.DeleteWithValidJwt_Returns202AndBlocksOldSession; JwtValidationTests.OldSessionVersion_RemainsInvalidAfterVersionChange | Assert JWT previo recibe 401 en /users/me; prueba separada de version invalida en /routes. | No verifica expresamente token antiguo de cuenta pending_deletion en todos los endpoints protegidos. |
| DEL-V05 | Directa en recursos HTTP probados | DeletionRecoveryScopeHttpTests.DEL_V05_V22_RecoveryToken_CannotAccessNormalResources | Assert 200 en consulta de eliminacion y 401 en perfil, rutas, detalle y PATCH. | La prueba usa TestApiFactory; no acredita cada recurso futuro. |
| DEL-V06 | Directa para subcasos; parcial global | DeletionRecoveryHttpTests.PendingAccount_CanGetOpaqueRecoveryToken; WrongPassword_Returns401; UnknownAccount_ReturnsGeneric401; UnknownAndWrongPassword_HaveSamePublicResponse; DeletionRateLimitContractHttpTests.Recovery_Request6_Returns429ProblemDetails | Valida credencial de 64 hex, SHA-256 guardado, expiresIn 300, respuestas publicas indistinguibles y limitador 5/min. | La ausencia total de enumeracion y de filtracion en logs requiere revision adicional. |
| DEL-V07 | Directa | DeletionCancellationHttpTests.ValidToken_RestoresAccountAndConsumesToken; DeletionCancellationMongoTests.ReusedToken_CannotCancelAgain | Assert HTTP 200, active, version 3, token/fechas nulos y replay 401; Mongo impide reuso. | No existe prueba de worker inexistente que verifique que no borra despues. |
| DEL-V08 | Directa | DeletionCancellationHttpTests.ExpiredDeletionDeadline_Returns410; DeletionCancellationMongoTests.ExactlyAtSevenDays_CancellationIsRejected; DeletionServerClockMongoTests.StaleApplicationClock_CannotCancelExpiredDeletion | Assert 410, rechazo en instante exacto y prevalencia del reloj Mongo; mantiene pending. | HTTP simula estado en memoria; limite exacto confirmado en Mongo. |
| DEL-V09 | Sin evidencia | Ninguna prueba de purga integral | No se observan asserts de eliminacion de inventario completo ni preservacion de terceros durante borrado. | Bloqueante para declarar borrado definitivo. |
| DEL-V10 | Sin evidencia | JwtValidationTests.OldSessionVersion_RemainsInvalidAfterVersionChange (solo relacionada) | Prueba version de sesion antigua pero no cuenta fisicamente eliminada/deleted. | No confundir revocacion por version con revocacion tras borrado final. |
| DEL-V11 | Parcial | DeletionCancellationHttpTests.FailureDuringStatusRead_Returns503ProblemDetails; AccountDeletionControllerTests.MongoFailure_Return503 | Assert 503 ProblemDetails y cuenta permanece pending ante error sintetico. | No comprueba reintentos, checkpoint ni recuperacion automatica de trabajo destructivo. |
| DEL-V12 | Sin evidencia | Ninguna prueba de reinicio de worker | No hay worker durable ni prueba de reanudacion desde checkpoint. | Pendiente. |
| DEL-V13 | Parcial, no cubre escenario central | DeletionCancellationHttpTests.ParallelCancellation_OnlyOneSucceeds; DeletionCancellationMongoTests.ParallelCancellation_OnlyOneMongoUpdateWins | Prueba cancelaciones simultaneas entre si. | No confronta cancelacion con toma de trabajo por worker irreversible. |
| DEL-V14 | Sin evidencia | Ninguna prueba de dos workers | No se verifica doble reclamo de trabajo irreversible ni aislamiento entre workers. | Pendiente. |
| DEL-V15 | Parcial | EndpointIntegrationTests.IT_01 a IT_07; RouteContractTests.VersionedRoutes_RemainCompatible; DeletionV04RouteContractTests.DeletionRoutes_MatchVersion04 | Los siete endpoints heredados se prueban a nivel Backend; nuevas cuatro rutas comprobadas por atributos. | No sustituye pruebas Android ni revision integral de compatibilidad de respuestas. |
| DEL-V16 | Sin evidencia | Ninguna prueba de politica autorizada de respaldos | No consta ratificacion ni evidencia de retencion/respaldos/excepciones. | Pendiente de responsables de privacidad. |
| DEL-V17 | Directa en flujo HTTP local | DeletionOldJwtAfterCancellationHttpTests.DEL_V17_OldJwtRemainsInvalidAfterCancellation | Assert JWT antiguo 401, nuevo login 200 y JWT nuevo con acceso 200. | No representa prueba conjunta Android-Backend. |
| DEL-V18 | Directa | DeletionCancellationHttpTests.ValidToken_RestoresAccountAndConsumesToken; DeletionCancellationMongoTests.ReusedToken_CannotCancelAgain | Assert replay de cancelacion 401 y segunda cancelacion Mongo null, estado activo. | Caso cubierto localmente. |
| DEL-V19 | Directa | DeletionCancellationMongoTests.ExactlyAtSevenDays_CancellationIsRejected; OneSecondBeforeSevenDays_CancellationSucceeds | Assert rechazo justo en deadline y exito un segundo antes. | Comparacion Mongo; no prueba HTTP al instante exacto. |
| DEL-V20 | Directa | AccountDeletionMongoTests.ConcurrentRequests_OnlyOneTransitionWins; RepeatedRequest_DoesNotExtendDeadline | Assert un resultado exitoso en carreras; solo una version y plazo sin extension. | No hay worker que comprobar en duplicados. |
| DEL-V21 | Sin evidencia | DeletionCancellationHttpTests.FailureDuringStatusRead_Returns503ProblemDetails (otro flujo) | Se prueba error en lectura de estado, no en purga irreversible. | Sin checkpoint ni estado final deleted. |
| DEL-V22 | Directa en recursos HTTP probados | DeletionRecoveryScopeHttpTests.DEL_V05_V22_RecoveryToken_CannotAccessNormalResources | Assert 401 con token restringido en perfil, rutas, detalle y PATCH. | Debe conciliarse el 403 hist?rico de tests.md. |
| DEL-V23 | Directa con simulacion del estado deleting | DeletionLoginBlockedHttpTests.DEL_V23_NormalLoginBlocked_InPendingAndDeleting | Assert login 401 en pending_deletion y deleting; JWT anterior 401. | Deleting se asigna en memoria; no se prueba el worker. |
| DEL-V24 | Parcial | DeletionUnknownResultHttpTests.DEL_V24_LostCancellationResponse_NewLoginVerifiesActive | Una cancelacion enviada, respuesta descartada, nuevo login 200 y tokens anteriores 401. | No simula perdida real de red ni prueba el caso de login fallido. |
| DEL-V25 | Parcial | DeletionStatusHttpTests.ExpiredRecoveryToken_Returns401; DeletionRecoveryHttpTests.NewRecoveryToken_InvalidatesPreviousHash | Se prueba token vencido rechazado y reemision que invalida hash anterior. | No hay flujo expirar token -> reautenticacion -> nuevo token dentro del plazo manteniendo fecha original. |
| DEL-V26 | Directa para condiciones aisladas; parcial global | DeletionStatusHttpTests.ExpiredRecoveryToken_Returns401; DeletionCancellationMongoTests.ExpiredToken_CannotRestoreAccount; DeletionIssueServerClockMongoTests.StaleClock_CannotIssueTokenAfterDeletionDeadline | Confirma token expirado inutilizable y Mongo rechaza emision cuando vencio plazo de cuenta. | No hay un unico escenario HTTP integral que encadene ambas condiciones. |
| DEL-V27 | Directa | DeletionCancellationHttpTests.AtomicCancellationConflict_Returns409; DeadlineChangesDuringCancellation_Returns410; DeletionStatusHttpTests.DeletingAccount_Returns410 | Asserts HTTP 409 en conflicto sintetico, 410 por vencimiento y deleting en status. | Simulaciones de API no prueban todos los interleavings reales de Mongo. |
| DEL-V28 | Directa en simulacion HTTP | DeletionStatusHttpTests.AfterDeletionDeadline_RecoveryIsNotAllowed | Assert HTTP 200, pending_deletion y recoveryAllowed false con token previo vigente. | Se altera deadline en memoria; no es experimento real de siete dias. |

## Correcciones de trazabilidad recomendadas

1. **DEL-V05 y DEL-V22 - observacion historica atendida:** La prueba DeletionRecoveryScopeHttpTests comprobo HTTP 401 en perfil, rutas, detalle y PATCH usando el token restringido.
2. **DEL-V04 y DEL-V17 - avance documentado:** DEL-V17 ya comprobo JWT anterior 401 despues de cancelar y nuevo login exitoso. DEL-V04 mantiene cobertura parcial de todos los recursos protegidos durante pending_deletion.
3. **DEL-V23 - observacion historica atendida:** Login HTTP 401 comprobado en pending_deletion y deleting. El estado deleting fue simulado en memoria; worker no probado.
4. **DEL-V24 - observacion historica atendida parcialmente:** Se descarto la respuesta de cancelacion y se verifico mediante nuevo login sin reintentar. No se simulo una desconexion real ni se valido el cliente Android.
5. **DEL-V25:** probar secuencia de token vencido -> reautenticación dentro del plazo -> token nuevo, sin ampliar los siete días.
6. **DEL-V09, V10, V12, V14, V16 y V21:** no cerrar como PASS hasta diseñar, autorizar y acreditar el flujo de borrado definitivo y la política de datos.
7. **DEL-R04 y DEL-V22:** conciliar los textos históricos de alcance `deletion:cancel` y HTTP `403` con la semántica aprobada de v0.4; no modificar ciegamente los archivos históricos.
8. **Riesgos documentados:** intermitencia MongoDB de causa no determinada; cobertura parcial de errores HTTP 503; persistencia vs simulación en memoria diferenciadas.

## Evidencia por archivo (ejemplos)

- `Rutitas.IntegrationTests/AccountDeletionHttpTests.cs`: comprobaciones de 202, JWT anterior 401, +7 días y segundo login rechazado.
- `Rutitas.IntegrationTests/DeletionCancellationHttpTests.cs`: cancelación 200, consumo de token, 409, 410, 503; varias verificaciones emplean `TestApiFactory` en memoria.
- `Rutitas.IntegrationTests/DeletionCancellationMongoTests.cs`: concurrencia, token único, expiración exacta y aislamiento de usuarios sobre Mongo de pruebas.
- `Rutitas.IntegrationTests/DeletionStatusHttpTests.cs`: consulta no consume token, 200 con `recoveryAllowed=false` y 410 durante `deleting`.
- `Rutitas.IntegrationTests/DeletionRateLimitContractHttpTests.cs`: 429 y `Retry-After` numérico con `application/problem+json`.
- `Rutitas.SecurityTests/JwtValidationTests.cs`: versión de sesión, JWT heredado rechazado y acceso a rutas, sin probar cada estado de eliminación.
- `Rutitas.ContractTests/DeletionV04RouteContractTests.cs`: rutas y atributos de autenticación comprobados mediante reflexión, no comparación integral de respuesta HTTP vs YAML.

## Límites y siguiente punto de control

No se autoriza integración Android–Backend, cambios de red o credenciales, uso de datos reales, activación de cuatro acciones sensibles, commits, push, PR, merge ni despliegue. La conformidad de diseño de Ame **no** es una autorización de ejecución. La matriz debe revisarse con el equipo antes de cambiar estados históricos en `trazabilidad.md` o declarar ratificadas políticas pendientes.

## Actualizacion documental posterior a la revision estatica

Se incorporaron cuatro nuevas pruebas HTTP locales, ejecutadas
satisfactoriamente y respaldadas por la ronda de cinco TRX
con 134 PASS: 38 unitarias, 26 seguridad, 9 contrato,
2 arquitectura y 59 integracion.

Las cuatro pruebas nuevas no acreditan un worker irreversible,
una desconexion real de Android ni cobertura integral de
datos personales y respaldos.

La conformidad tecnica Android es solo de diseno.
La integracion Android-Backend permanece NO AUTORIZADA.
Los 28 escenarios Android siguen NOT_RUN.
