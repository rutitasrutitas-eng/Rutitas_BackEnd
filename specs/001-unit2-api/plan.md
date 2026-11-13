# Plan técnico — API Unidad II

> **CONCILIACIÓN TÉCNICA — 11/11/2026:** Hay implementación local de las siete operaciones y el usuario reportó **63/63 pruebas aprobadas** tras iniciar Docker. Esta es evidencia técnica local, **no** aprobación formal del Gate 0, del contrato móvil ni del despliegue. Los campos **Decisión / Ejecución / Verificación** del encabezado y la tabla de tareas describen el *registro SDD original*, que no ha sido actualizado mediante acta de autorización. La implementación real usa `PasswordHasher<User>` y filtra rutas con `Status="active"` y `Visibility="public"`; referencias a Argon2id o `isActive` en el diseño son propuestas históricas, no afirmaciones sobre el código. Consultar [estado real](../../docs/ESTADO_REAL_BACKEND.md) y [brechas](../../docs/BRECHAS_FASE1_11NOV2026.md).

**ID:** PLAN-U2-API  
**Decisión:** `propuesto` · **Ejecución:** `no_iniciado` · **Verificación:** `no_ejecutada` · **Tipo:** `experimental`  
**Spec:** [spec.md](spec.md) · **Tareas:** [task.md](task.md) · **Contrato:** [contracts/openapi.yaml](contracts/openapi.yaml)

> Plan documental. No crea proyecto, código, infraestructura ni recursos cloud. La ejecución requiere autorización humana registrada.

## 1. Stack propuesto y decisiones

| Elemento | Propuesta | Estado |
|---|---|---|
| Runtime | ASP.NET Core 10 / C# | Dirección de Unidad II; ratificación pendiente (`BACKEND-D05`) |
| Persistencia | MongoDB 8 local / MongoDB Atlas en nube | Propuesto; pendiente ADR |
| Autenticación | JWT Bearer + denylist `jti` | Propuesto (`BACKEND-D10`) |
| Contrato | OpenAPI 3.1 | Propuesto; acuerdo con Móvil pendiente (`BACKEND-D06`) |
| Pruebas | xUnit, FluentAssertions, WebApplicationFactory, Testcontainers | Propuesto |
| CI/CD | GitHub Actions + OIDC | Propuesto; proveedor cloud pendiente (`BACKEND-D09`) |
| Nube | AWS, Azure o GCP | Sin seleccionar; no se crean recursos |

Dependencias NuGet candidatas: `MongoDB.Driver`, `Microsoft.AspNetCore.Authentication.JwtBearer`, `Microsoft.AspNetCore.OpenApi` o `Swashbuckle.AspNetCore`, `Asp.Versioning.Mvc`, `Serilog.AspNetCore`, `FluentValidation.AspNetCore` (o DataAnnotations), `OpenTelemetry.*` y paquetes de prueba. Ninguna se considera aprobada hasta el ADR.

La propuesta se detalla en [ADR-001-stack](../../docs/ADR-001-stack.md); el registro de aprobación y los roles están en [APR-U2-001](../../docs/APR-U2-001.md).

## 2. Arquitectura propuesta

Monolito modular proporcional a 7 endpoints. Sin microservicios, colas, caché distribuida ni orquestadores.

```text
Rutitas.Api              -> controladores delgados, middleware, auth, OpenAPI, ProblemDetails
Rutitas.Application      -> casos de uso, DTOs, validadores, puertos
Rutitas.Domain           -> User, Route, value objects, reglas invariantes
Rutitas.Infrastructure   -> MongoDB, JWT, hashing, repositorios, índices, seed
tests/Rutitas.UnitTests
tests/Rutitas.IntegrationTests
tests/Rutitas.ContractTests
```

Dirección de dependencias: `Api -> Application -> Domain`; `Infrastructure -> Application/Domain`; `Api` compone `Infrastructure` mediante DI. No hay dependencias de `Domain` hacia infraestructura. Los controladores no acceden directamente a MongoDB.

## 3. Modelos MongoDB

| Colección | Campos | Índices / validación |
|---|---|---|
| `users` | `_id`, `email`, `passwordHash`, `displayName`, `phone`, `role`, `status`, `createdAt`, `updatedAt`, `lastLoginAt` | `email` único y normalizado; JSON Schema con campos requeridos, enums y longitudes; `passwordHash` excluido de DTOs. |
| `revoked_tokens` | `_id`, `jti`, `userId`, `expiresAt`, `revokedAt`, `reason` | `jti` único; índice TTL en `expiresAt`; limpieza automática. |
| `routes` | `_id`, `code`, `name`, `description`, `origin`, `destination`, `stops[]`, `distanceKm`, `estimatedDurationMin`, `isActive`, `visibility`, `createdAt`, `updatedAt` | `code` único; índice de texto en `name/description`; filtro `isActive/visibility`; seed sintético de 12–15 rutas. |

Reglas: `ObjectId` como id; fechas UTC; índices idempotentes; validación de esquema con `collMod`; sin migraciones destructivas; sin datos personales reales en desarrollo. La concurrencia de `PATCH` usa actualización atómica; un `version`/ETag opcional queda pendiente.

## 4. Autenticación y autorización

### 4.1 Registro y login

- `POST /auth/register`: normaliza email, valida contraseña, genera hash, inserta usuario y devuelve `201` sin token. Un duplicado de índice único se traduce a `409`.
- `POST /auth/login`: busca por email normalizado, verifica hash en tiempo constante y emite JWT. Error genérico `401` para credenciales inválidas.
- Rate limiting: ventana por IP/email; registro y login limitados; `429` con `Retry-After`.
- Hashing: Argon2id preferido; alternativa `PasswordHasher<T>` si el ADR lo justifica. Parámetros y upgrade de hash quedan en `BACKEND-D07`.

### 4.2 JWT y logout

- Claims: `sub`, `email`, `role`, `jti`, `iat`, `exp`, `iss`, `aud`.
- Vida propuesta: 30 minutos; `iss=rutitas-api`; `aud=rutitas-mobile`; tolerancia de reloj 60 s.
- Firma HS256 con secreto de 32+ bytes o RS256; clave en user-secrets/Secret Manager, nunca en repositorio.
- `POST /auth/logout`: requiere Bearer válido; inserta `jti` en `revoked_tokens` con `expiresAt` del token; devuelve `204` aunque ya estuviera revocado (idempotente). Token revocado produce `401`.
- Alternativa descartada para `001`: refresh tokens y `tokenVersion`; se revisarán si se amplía alcance.

### 4.3 Autorización

- Política de denegación por defecto; solo register/login son anónimos.
- `users/me` deriva el usuario del claim `sub`; no acepta id en la ruta.
- Rutas: lectura autenticada; solo activas/visibles según `BACKEND-D11`.
- Sin roles administrativos en `001`; el rol `user` es el valor por defecto.

## 5. Contrato y validación

- Base `/api/v1`; versionado por URL. Cambios incompatibles requieren `/api/v2`.
- JSON camelCase, fechas ISO-8601 UTC, `Content-Type: application/json`.
- Errores `application/problem+json` (RFC 9457): `type`, `title`, `status`, `detail`, `instance`, `traceId`, `errors?`.
- Códigos: `400` validación, `401` autenticación, `404` recurso, `409` conflicto, `429` límite, `500` error interno.
- Paginación: `page` ≥1, `pageSize` 1–100, `q` máximo 100; orden estable.
- Validaciones: email RFC y máximo 254; contraseña 12–128; `displayName` 1–80; `phone` E.164 opcional; `ObjectId` válido; campos desconocidos rechazados.
- El detalle completo está en [contracts/openapi.yaml](contracts/openapi.yaml).

## 6. Seguridad y modelo de amenazas

| Amenaza | Control propuesto |
|---|---|
| IDOR en perfil | Usuario derivado del `sub`; no se acepta id de otro usuario. |
| Fuerza bruta / enumeración | `401` genérico, rate limiting, bloqueo temporal opcional. |
| Robo o reutilización de JWT | HTTPS, expiración corta, denylist `jti`, no registrar tokens. |
| Inyección NoSQL | Filtros tipados, `ObjectId.TryParse`, sin concatenar entrada. |
| Exposición de datos | DTOs sin `passwordHash`; logs redactados; errores sin stack traces. |
| Secretos en repositorio | user-secrets/Secret Manager, escaneo Gitleaks, `.gitignore`. |
| Configuración cloud insegura | Atlas TLS, usuario de mínimo privilegio, allowlist IP, IaC revisada. |
| Abuso de recursos | Rate limiting, límites de request y pageSize. |
| Dependencias vulnerables | Auditoría NuGet y escaneo de imagen en CI. |

Controles de transporte: HTTPS/HSTS en entornos reales, CORS por allowlist, headers seguros y límites de tamaño. El cliente móvil (Kotlin propuesto) debe guardar el token en `EncryptedSharedPreferences`/Keystore y borrarlo ante `401`.

## 7. Pruebas y evidencia

| Nivel | Cobertura propuesta |
|---|---|
| Unitarias | Validadores, hashing, JWT, paginación, mapeos, reglas de rutas. |
| Integración | `WebApplicationFactory` + Testcontainers Mongo: registro, login, logout, me, patch, rutas, duplicados, `401/404/409/429`, token revocado. |
| Contrato | OpenAPI lint, validación de esquemas y revisión con cliente Kotlin. |
| Seguridad | OWASP API Top 10, IDOR, inyección NoSQL, JWT alterado/expirado, fuerza bruta, exposición de datos. |
| Regresión | Suite completa en CI; cobertura ≥80% aplicación/dominio y 100% de rutas críticas de autenticación. |

Evidencia esperada: resultados TRX, cobertura, OpenAPI validado, reportes de escaneo y registro de fecha/entorno. Un test escrito no equivale a test aprobado.

## 8. CI/CD propuesto

1. Restaurar con lockfile.
2. `dotnet format --verify-no-changes`.
3. Compilar con warnings como errores.
4. Ejecutar pruebas unitarias, integración y contrato con cobertura.
5. `dotnet list package --vulnerable --include-transitive`.
6. Gitleaks, Trivy sobre imagen y validación OpenAPI.
7. Publicar artefactos y evidencia.
8. Despliegue manual a staging con aprobación y OIDC; nunca automático a producción.

La configuración actual de OpenCode deniega edición/shell; su cambio para implementación requiere autorización (`BACKEND-D08`). No se modifica en esta fase.

## 9. Despliegue planificado

| Proveedor | Cómputo | Registro | Secretos | Logs | MongoDB |
|---|---|---|---|---|---|
| AWS | ECS Fargate / App Runner | ECR | Secrets Manager | CloudWatch | Atlas M0 |
| Azure | Container Apps | ACR | Key Vault | App Insights | Atlas M0 |
| GCP | Cloud Run | Artifact Registry | Secret Manager | Cloud Logging | Atlas M0 |

Recomendación no vinculante: MongoDB Atlas M0 + contenedor; Azure Container Apps o GCP Cloud Run por menor operación, o AWS si el equipo ya domina AWS. `BACKEND-D09` sigue pendiente. Entornos propuestos: local (Docker Compose), CI efímero, staging opcional; producción fuera de alcance. Sin recursos pagos, sin datos reales y sin despliegue en esta entrega.

Antes de desplegar: presupuesto, propietario, datos, allowlist, secretos, observabilidad, rollback, backups y destrucción verificable. El despliegue staging requiere aprobación humana separada (`BE-U2-17`); su aceptación es `BE-U2-20`, distinta de la aceptación local `BE-U2-18`.

## 10. Trazabilidad requisito → tarea → prueba

| Requisito / criterio | Tarea | Prueba / evidencia |
|---|---|---|
| RF-U2-01 | `BE-U2-09` | Unitarias de validación/hash + integración de registro y duplicado. |
| RF-U2-02 | `BE-U2-09` | Integración login válido/inválido + claims y rate limit. |
| RF-U2-03 | `BE-U2-10` | Integración logout, idempotencia y token revocado. |
| RF-U2-04 | `BE-U2-11` | Integración perfil propio y `401`. |
| RF-U2-05 | `BE-U2-11` | Integración PATCH, campos inmutables e IDOR. |
| RF-U2-06 | `BE-U2-12` | Integración paginación/filtro + contrato. |
| RF-U2-07 | `BE-U2-12` | Integración detalle `200/400/404`. |
| AC-U2-01 | `BE-U2-04..06`, `BE-U2-05A` | Revisión de spec/plan/task/OpenAPI y aprobación final del contrato. |
| AC-U2-02, AC-U2-03 | `BE-U2-09`, `BE-U2-14` | Integración de registro/login y pruebas de seguridad. |
| AC-U2-04 | `BE-U2-10`, `BE-U2-14` | Integración de logout y token revocado. |
| AC-U2-05 | `BE-U2-11`, `BE-U2-14` | Integración de perfil e IDOR. |
| AC-U2-06 | `BE-U2-12`, `BE-U2-14` | Integración de rutas y contrato. |
| AC-U2-07 | `BE-U2-13`, `BE-U2-14` | Validación de errores y ProblemDetails. |
| AC-U2-08 | `BE-U2-08`, `BE-U2-13`, `BE-U2-14` | Índices, TTL y esquema verificados. |
| AC-U2-09 | `BE-U2-14` | Suite completa y cobertura. |
| AC-U2-10 | `BE-U2-05A`, `BE-U2-15`, `BE-U2-19` | OpenAPI final, CI y revisión Kotlin. |
| AC-U2-11 | `BE-U2-13..15` | Logs, HTTPS/CORS/rate limit y escaneos. |
| AC-U2-12 | `BE-U2-15`, `BE-U2-18`, `BE-U2-20` | Gates, aceptación local y aceptación staging. |

## 11. Riesgos y mitigaciones

| Riesgo | Impacto | Control | Cierre |
|---|---|---|---|
| Stack sin ratificación | Bloquea scaffold | ADR y aprobación | `BACKEND-D05` |
| Contrato Kotlin no acordado | Bloquea pruebas de consumidor | Revisión OpenAPI | `BACKEND-D06`, `BACKEND-D13` |
| Logout JWT ambiguo | Bloquea implementación | Denylist `jti` | `BACKEND-D10` |
| Visibilidad de rutas ambigua | Bloquea listado | Regla autenticada activa/pública | `BACKEND-D11` |
| Nube sin presupuesto | Bloquea despliegue | Atlas M0 + contenedor; decisión | `BACKEND-D09` |
| Fuente docente sin acta | Trazabilidad incompleta | Registrar comunicación y validar | `BACKEND-D12` |
| Sobrearquitectura | Costo innecesario | Monolito modular, sin microservicios | Revisión humana |

## 12. Secuencia de implementación

1. Aprobar Gate 0-A y la dirección de stack/contrato (`BE-U2-01`); no habilita código.
2. Registrar `ADR-001` y actualizar `D05` (`BE-U2-02`).
3. Acordar alcance inicial, plataforma y errores con Móvil (`BE-U2-03`); no aprueba el OpenAPI final.
4. Revisar `spec.md` y `plan.md` (`BE-U2-04..05`).
5. Aprobar el OpenAPI final con el cliente móvil (`BE-U2-05A`).
6. Cerrar `task.md` y la matriz de trazabilidad (`BE-U2-06`).
7. Firmar `D08-B` y autorizar el scaffold (`BE-U2-07`).
8. Implementar MongoDB, índices y seed (`BE-U2-08`).
9. Implementar auth, logout, perfil y rutas (`BE-U2-09..12`).
10. Completar validación, errores, OpenAPI y observabilidad (`BE-U2-13`).
11. Ejecutar pruebas, CI y validación Kotlin (`BE-U2-14..15`, `BE-U2-19`).
12. Aceptar localmente, registrar evidencia y actualizar memoria (`BE-U2-18`).
13. Solo con `D09` y presupuesto: preparar nube/staging y aceptar staging (`BE-U2-16..17`, `BE-U2-20`).

## 13. Reutilización de agentes y skills

Se reutilizan los procedimientos y criterios de `backend-crear-especificacion`, `backend-crear-plan`, `backend-desglosar-tareas` y `backend-validar-coherencia-sdd` para estructurar y validar este incremento. Los subagentes `backend-revisor-contratos`, `backend-revisor-seguridad`, `backend-revisor-calidad` y `backend-arquitecto` aportan las revisiones de contrato, seguridad, pruebas y arquitectura. Su ámbito local sigue limitado a `000`; invocarlos formalmente sobre `001` requiere ampliación aprobada y no se autoautoriza. No implementan ni aprueban.


## 14. Anexo de conciliación técnica local (06-11-2026)

Este anexo describe **lo observado en el código entregado**, sin alterar la propuesta histórica ni aprobar los gates.

| Decisión o componente | Estado constatable | Acción documental pendiente |
|---|---|---|
| ASP.NET Core 10, solución modular | Proyectos `Rutitas.Api`, `Rutitas.Application`, `Rutitas.Domain`, `Rutitas.Infrastructure` presentes | Formalizar ratificación de stack |
| Seguridad de contraseñas | `PasswordHasher<User>` registrado en `Program.cs` | Actualizar ADR/plan si se acepta; no afirmar que Argon2id está implementado |
| MongoDB | Repositorios `users`, `revoked_tokens`, `routes` presentes | Validar índices y retención en el entorno aprobado |
| Visibilidad de rutas | Filtro `Status="active"`, `Visibility="public"` | Sustituir propuesta `isActive` una vez aprobado el contrato definitivo |
| Pruebas | Cinco proyectos: unitarias, integración, seguridad, contrato y arquitectura | Adjuntar TRX y cobertura reproducible; usuario mostró 63 aprobadas |
| CI/CD | Workflows `.github/workflows/api-ci.yml`, `api-codeql.yml`, `api-nightly.yml`, `api-cd.yml` presentes | Verificar ejecución de Actions; CD actual no publica en Azure |
| Nube | Proveedor objetivo comunicado: Azure; sin despliegue verificado | Completar primero specs funcionales, luego SDD Azure IaC y SDD CI/CD |

La URL local observada fue `https://localhost:7291/api/v1`; no es una URL accesible desde un dispositivo externo. La URL pública definitiva se añadirá al aprobar y desplegar infraestructura.
