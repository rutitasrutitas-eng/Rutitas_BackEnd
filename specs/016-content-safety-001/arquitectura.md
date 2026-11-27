# CONTENT-SAFETY-001 - Arquitectura

Estado: CANDIDATE

## Stack

- ASP.NET Core 10.
- C#.
- Arquitectura modular existente.
- Azure AI Content Safety como proveedor candidato.

## Flujo

Android -> Rutitas API -> Application
-> IContentSafetyProvider
-> Proveedor simulado o adaptador Azure autorizado.

## Api

- Autenticacion JWT.
- Autorizacion por usuario y recurso.
- Validacion HTTP.
- Rate limiting.
- ProblemDetails.

## Application

- IContentSafetyProvider.
- ModerationPolicy.
- TextModerationRequest.
- ImageModerationRequest.
- ModerationResult.
- ModerationContext.
- ModerationDecision.

## Infrastructure

- MockContentSafetyProvider.
- AzureContentSafetyProvider (futuro).
- HttpClient.
- Configuracion protegida.
- Identidad administrada cuando resulte compatible.

## Controles de publicacion

La moderacion debe vincularse al propietario,
contexto y version exacta del contenido.

No confiar en un estado approved enviado por Android.

Si cambia el contenido, debe invalidarse la decision
previa y realizarse una nueva evaluacion.

## Configuracion candidata

ContentSafety:Enabled = false
ContentSafety:Provider = Mock

La integracion real debera requerir configuracion y
autorizacion explicitas.

No se crean servicios ejecutables en esta fase.
No se modifican Program.cs ni appsettings.json.
