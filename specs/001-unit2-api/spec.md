# Especificación funcional — API Unidad II

> **Estado técnico revisado — 13/11/2026:** La rama de Mark contiene las siete operaciones. El reporte de **63/63** es histórico y local; no hay evidencia CI de ese número para el SHA revisado. El registro SDD original conserva sus estados de decisión y aprobación pendientes. La implementación observada usa `PasswordHasher<User>` y filtros `Status="active"`, `Visibility="public"`. Véanse [validación técnica](../../docs/VALIDACION_MARK.md) y [compatibilidad Android](../../docs/COMPATIBILIDAD_ANDROID.md).

**ID:** SPEC-U2-API  
**Decisión:** `propuesto` · **Ejecución:** `no_iniciado` · **Verificación:** `no_ejecutada` · **Tipo:** `experimental`  
**Incremento:** `001-unit2-api`  
**Fase:** Unidad II (3–4 nov 2026)  
**Fuente de fase:** instrucción comunicada por el usuario; fuente primaria pendiente (`BACKEND-D12`).

> Documento de especificación. No implementa código, no despliega infraestructura y no sustituye la aprobación humana. La dirección de Unidad II está registrada; la ratificación constitucional, el stack formal, el contrato con Móvil y la nube siguen pendientes.

## 1. Objetivo

Definir el comportamiento funcional de 7 endpoints REST para autenticación, perfil propio y consulta de rutas de Rutitas Backend, con ASP.NET Core 10/C#, MongoDB y JWT, para la evaluación de Unidad II.

## 2. Alcance

Incluye:

- Registro, login y logout de usuarios.
- Consulta y actualización del perfil propio.
- Consulta paginada de rutas y detalle de ruta.
- Contrato OpenAPI 3.1.
- Modelo de datos MongoDB, validación, autorización, revocación de JWT, pruebas, CI/CD y planificación de despliegue.

## 3. Exclusiones

- Refresh tokens, recuperación de contraseña y verificación de correo.
- Roles administrativos y CRUD de rutas.
- Mapas, geolocalización en tiempo real, notificaciones, pagos y analítica.
- Implementación de la aplicación móvil (Kotlin propuesto); solo se define compatibilidad de contrato.
- Despliegue real, creación de recursos pagos y uso de datos personales reales.
- Cambios en `specs/000-arquitectura-base/`.

## 4. Actores y permisos

| Actor | Permisos |
|---|---|
| Visitante | Registrar cuenta e iniciar sesión. |
| Usuario autenticado | Cerrar sesión, consultar y actualizar su propio perfil, listar y consultar rutas. |
| Cliente móvil (Kotlin propuesto) | Consumir el contrato; no es autoridad de permisos del servidor. |
| Equipo Backend | Documentar, revisar y, tras autorización, implementar y verificar. |

## 5. Fuentes, supuestos y dependencias

- [Constitución del proyecto](../../.specify/memory/constitution.md), [AGENTS](../../AGENTS.md) y [arquitectura base](../000-arquitectura-base/spec.md).
- Instrucción de Unidad II comunicada por el usuario; fuente primaria pendiente.
- Stack presente en código: ASP.NET Core 10/C#, MongoDB y JWT; su aprobación sigue pendiente (`BACKEND-D05` en [decisiones pendientes](../../docs/decisiones-pendientes.md)). No existe `ADR-001-stack.md` ratificado.
- Contrato con cliente móvil (Kotlin propuesto; `BACKEND-D06`, `BACKEND-D13`).
- Nube sin seleccionar (`BACKEND-D09`); no se crean recursos.
- Aprobación y roles: pendientes según [decisiones pendientes](../../docs/decisiones-pendientes.md). No existe acta `APR-U2-001` en este repositorio.

**Supuestos:** no existe implementación previa de esta API; el entorno local usa MongoDB de desarrollo; los datos de rutas son sintéticos; la autorización se valida en el servidor.

## 6. Requisitos funcionales

| ID | Endpoint | Descripción | Autorización |
|---|---|---|---|
| RF-U2-01 | `POST /api/v1/auth/register` | Crea una cuenta con email único, contraseña y nombre visible. Devuelve `201` con perfil; no devuelve token. | Pública |
| RF-U2-02 | `POST /api/v1/auth/login` | Valida credenciales y devuelve un JWT de acceso con expiración y datos del usuario. | Pública |
| RF-U2-03 | `POST /api/v1/auth/logout` | Revoca el `jti` del token actual. Es idempotente y devuelve `204`. | Bearer |
| RF-U2-04 | `GET /api/v1/users/me` | Devuelve el perfil del usuario autenticado, sin campos sensibles. | Bearer |
| RF-U2-05 | `PATCH /api/v1/users/me` | Actualiza `displayName` y/o `phone` del usuario autenticado. No permite email, contraseña ni rol. | Bearer |
| RF-U2-06 | `GET /api/v1/routes` | Lista rutas activas y visibles con paginación y búsqueda opcional. | Bearer |
| RF-U2-07 | `GET /api/v1/routes/{id}` | Devuelve el detalle de una ruta visible. | Bearer |

### 6.1 Reglas de autenticación

- Email normalizado a minúsculas y sin espacios; índice único.
- Contraseña de 12 a 128 caracteres; hash con Argon2id preferido o `PasswordHasher<T>` si el ADR lo aprueba; nunca se almacena ni devuelve en claro.
- Login inválido devuelve `401` genérico para no revelar si el email existe.
- JWT con claims `sub`, `email`, `role`, `jti`, `iat`, `exp`, `iss`, `aud`; vida propuesta de 30 minutos.
- Logout agrega el `jti` a una denylist con TTL hasta la expiración; un token revocado produce `401`.
- Rate limiting en registro y login; `429` con `Retry-After`.

### 6.2 Reglas de perfil

- `GET /users/me` y `PATCH /users/me` derivan el usuario del claim `sub`; no aceptan id de usuario en la ruta.
- `PATCH` acepta al menos un campo; `phone: null` borra el teléfono; `displayName` no acepta `null`.
- La respuesta excluye `passwordHash` y metadatos internos.

### 6.3 Reglas de rutas

- Solo rutas `isActive=true` y visibles según `BACKEND-D11`.
- `page` inicia en 1; `pageSize` por defecto 20 y máximo 100; orden estable.
- `q` busca en nombre/descripción con texto escapado.
- `id` debe ser un `ObjectId` válido; formato inválido `400`, inexistente `404`.

## 7. Datos y privacidad

| Colección | Datos | Finalidad | Retención propuesta |
|---|---|---|---|
| `users` | Email, nombre visible, teléfono opcional, hash de contraseña, rol, estado, fechas | Autenticación y perfil | Cuenta hasta baja; baja fuera de alcance |
| `revoked_tokens` | `jti`, usuario, expiración, motivo | Revocación de sesión | TTL hasta expiración del token |
| `routes` | Código, nombre, descripción, origen, destino, paradas, distancia, duración | Catálogo de rutas | Datos sintéticos en desarrollo |

- Minimización: teléfono opcional; no se registra ubicación del usuario ni historial de viajes.
- Logs: sin contraseñas, tokens ni email completo; retención propuesta de 30 días.
- Desarrollo y pruebas: datos sintéticos; no usar datos personales reales.
- La clasificación y retención definitivas dependen de `BACKEND-D07`.

## 8. Requisitos no funcionales

- **Seguridad:** mínimo privilegio, denegación por defecto, autorización por recurso, validación en el borde, HTTPS/HSTS, CORS por allowlist y secretos fuera del repositorio.
- **Integridad:** email único, actualización atómica del perfil, logout idempotente y errores sin filtrar detalles internos.
- **Rendimiento:** lecturas paginadas; objetivo académico p95 < 500 ms, no medido.
- **Disponibilidad:** una instancia académica; sin SLA ni promesa de producción.
- **Observabilidad:** logs estructurados con `traceId`; endpoints operativos de salud fuera de los 7 endpoints funcionales.
- **Compatibilidad:** JSON camelCase, fechas ISO-8601 UTC, JWT Bearer, errores `application/problem+json`, OpenAPI 3.1.

## 9. Contrato API

El contrato normativo propuesto está en [contracts/openapi.yaml](contracts/openapi.yaml). Resumen:

| Método | Ruta | Éxito | Errores principales |
|---|---|---|---|
| `POST` | `/auth/register` | `201` perfil | `400`, `409`, `429` |
| `POST` | `/auth/login` | `200` token + perfil | `400`, `401`, `429` |
| `POST` | `/auth/logout` | `204` | `401` |
| `GET` | `/users/me` | `200` perfil | `401` |
| `PATCH` | `/users/me` | `200` perfil | `400`, `401` |
| `GET` | `/routes` | `200` página | `400`, `401` |
| `GET` | `/routes/{id}` | `200` detalle | `400`, `401`, `404` |

Base: `/api/v1`. Los errores usan RFC 9457 con `type`, `title`, `status`, `detail`, `instance`, `traceId` y, cuando aplica, `errors`.

## 10. Criterios de aceptación

| ID | Criterio verificable |
|---|---|
| AC-U2-01 | Los 7 endpoints están documentados en `spec.md`, `plan.md`, `task.md` y OpenAPI con método, ruta, autenticación, entradas, salidas y errores. |
| AC-U2-02 | Registro crea un usuario con email único y contraseña hasheada; duplicado devuelve `409`; la respuesta no contiene `passwordHash`. |
| AC-U2-03 | Login válido devuelve JWT con claims y expiración; credenciales inválidas devuelven `401` genérico; exceso devuelve `429`. |
| AC-U2-04 | Logout revoca el `jti`, es idempotente (`204`) y el token revocado produce `401`. |
| AC-U2-05 | `GET/PATCH /users/me` solo operan sobre el usuario del token; no hay IDOR; email, contraseña y rol no son editables. |
| AC-U2-06 | `GET /routes` pagina y filtra rutas activas/visibles; detalle devuelve `200` o `404`. |
| AC-U2-07 | Validaciones devuelven `400` con `ValidationProblemDetails`; no se exponen stack traces ni datos internos. |
| AC-U2-08 | MongoDB define índices únicos, validación de esquema y TTL de revocación; no hay secretos en claro. |
| AC-U2-09 | Pasan pruebas unitarias, integración, contrato y seguridad; cobertura ≥80% en aplicación/dominio. |
| AC-U2-10 | OpenAPI coincide con la implementación y el cliente móvil (Kotlin propuesto) revisa compatibilidad. |
| AC-U2-11 | Logs sin secretos/PII; HTTPS, CORS y rate limiting configurados para el entorno aprobado. |
| AC-U2-12 | CI ejecuta gates de calidad/seguridad y la aceptación conserva evidencia; no se despliega sin aprobación. |

## 11. Riesgos y decisiones abiertas

| Riesgo / decisión | Impacto | Control propuesto | Referencia |
|---|---|---|---|
| Stack sin ratificación formal | Bloquea scaffold/aprobación | ADR y ratificación humana | `BACKEND-D05` |
| Contrato con Kotlin no acordado | Bloquea pruebas de consumidor | Revisión OpenAPI y acuerdo | `BACKEND-D06`, `BACKEND-D13` |
| Logout JWT no decidido | Bloquea implementación | Denylist `jti` con TTL | `BACKEND-D10` |
| Visibilidad de rutas no decidida | Bloquea comportamiento de listado | Catálogo autenticado activo/público | `BACKEND-D11` |
| Nube sin seleccionar | Bloquea despliegue | Atlas M0 + contenedor; decisión pendiente | `BACKEND-D09` |
| Fuente docente sin acta | Brecha de trazabilidad | Registrar comunicación y validar fuente | `BACKEND-D12` |

## 12. Trazabilidad

Los requisitos `RF-U2-01..07` se despliegan en [plan.md](plan.md) y en las tareas `BE-U2-*` de [task.md](task.md). Los criterios `AC-U2-01..12` se validan con pruebas y evidencia descritas en el plan. Las tareas `BE-AB-*` de `000` no se modifican.


## 13. Anexo de contraste con la implementación local (06-11-2026)

**Naturaleza:** constatación técnica posterior a la redacción de Unidad II. No modifica la decisión formal `propuesto`, no equivale a aprobación del docente y no autoriza despliegue.

Se localizaron en el ZIP los controladores `AuthController`, `UsersController` y `RoutesController`, y siete acciones HTTP concordantes con `RF-U2-01` a `RF-U2-07`. La comprobación del usuario en Visual Studio mostró, durante la sesión local, registro `201`, login `200`, perfil `200`, actualización `200`, logout `204`, token revocado `401`, listado de rutas `200` y detalle `200/400/404`. Esto es evidencia comunicada, **no una ejecución independiente del ZIP**.

- `POST /api/v1/auth/register` y `POST /api/v1/auth/login`: accesibles sin exigir JWT en las acciones observadas.
- `POST /api/v1/auth/logout`: `[Authorize]` en la acción.
- `GET/PATCH /api/v1/users/me`: `[Authorize]` a nivel de controlador.
- `GET /api/v1/routes` y `GET /api/v1/routes/{id}`: `[Authorize]` a nivel de controlador.
- En `MongoRouteRepository`, la visibilidad real filtra `Status == "active"` y `Visibility == "public"`, no un campo `isActive`. Cualquier cambio de modelo necesita acuerdo documental y pruebas.
- El servicio usa `PasswordHasher<User>`; Argon2id permanece como alternativa de diseño sin adoptar.
- El contrato definitivo con la aplicación móvil, la decisión cloud y la autorización de gasto permanecen pendientes.

**Brechas por verificar:** cabecera `Location` del registro; esquema y semántica de cada respuesta de error; aprobación del contrato con Android; ejecución de los workflows en GitHub y cobertura mínima exigida por el plan. No asumir cumplimiento completo por el solo hecho de tener pruebas verdes.
