# Compatibilidad de la API con Android

**Fuentes observadas el 13/11/2026:** Backend `feature/sdd-azure-devsecops` en `2cb6e016`; Android `feature/integracion-android-ci-cd` en `4b2ea8d10f4dbc94830db515a65dd3d00c6a66ad`. El contrato entre equipos sigue propuesto. `ApiService.kt` declara tres llamadas remotas futuras y el modo DEMO usa Room; la pantalla actual no consume estos siete endpoints de forma integral.

| Endpoint Backend | Estado Android actual | Diferencia para integrar |
|---|---|---|
| `POST /api/v1/auth/register` | `AuthRepository` registra en Room y crea sesión local. | El backend requiere `email`, `password`, `displayName`, `phone?`, responde `201` con `UserProfile` y no entrega token. Android pide `name`, `type` e email institucional; debe acordarse política y login posterior. |
| `POST /api/v1/auth/login` | `ApiService` declara `auth/login`, pero espera `{token,name,email}`; `AuthRepository` autentica localmente. | API devuelve `{accessToken,tokenType,expiresIn,user}`. Configurar base URL `/api/v1/`, mapeo DTO, encabezado Bearer y manejo de `400/401/429/500`. |
| `POST /api/v1/auth/logout` | `SessionManager` sólo limpia sesión en memoria. | Invocar revocación remota antes de descartar token, manejar `204/401/503` y estado offline. |
| `GET /api/v1/users/me` | Perfil sale de Room/`Session`; no hay llamada Retrofit equivalente. | ID del servidor es ObjectId string, Android usa `Long`. DTO servidor: `id,email,displayName,phone,role,createdAt`; Android: `id,name,email,type,university`. Requiere mapeo explícito. |
| `PATCH /api/v1/users/me` | Android permite editar nombre, email, tipo, zona y horario localmente. | Backend sólo acepta `displayName` y `phone`; rechaza `role`, email y otras propiedades. No enviar el modelo Room completo. Manejar `400/401/500`. |
| `GET /api/v1/routes` | `ApiService.routes()` espera `List<RouteDto>`; Room usa IDs `Long`. | Backend devuelve objeto `{items,page,pageSize,total,totalPages}` con rutas activas públicas. Usa ID ObjectId string, `code`, ubicaciones `{name,lat,lng}`, distancia y duración. Android espera horario, tarifa, asientos, conductor, vehículo, días y paradas string; faltan en resumen. Autenticación Bearer obligatoria; manejar `400/401/429/500`. |
| `GET /api/v1/routes/{id}` | No hay llamada Retrofit de detalle; datos salen de Room. | Backend usa ObjectId string, devuelve paradas con orden/coordenadas, descripción y ubicaciones; `400/401/404/500`. No puede pasarse un ID `Long` local. |

## Decisiones de compatibilidad pendientes

- **Identidad y roles:** Backend emite rol `user`; Android usa `UserType` (`ALUMNO`, `DOCENTE`, `PERSONAL`) y estados de conductor propios. No mapear estos conceptos por coincidencia de nombre ni conceder permisos del servidor desde datos locales.
- **Token:** `SessionManager` actual sólo guarda sesión en memoria; no hay almacenamiento de JWT ni interceptor Bearer. Definir almacenamiento seguro, expiración, logout, cierre por `401` y política de reconexión. Evitar logs de tokens.
- **Errores:** acordar parseo de `ProblemDetails`, `ValidationProblemDetails`, `Retry-After` y mensajes de usuario; las excepciones Retrofit no representan directamente errores de dominio.
- **Viajes y reservas:** Android tiene rutas, viajes, cupos, solicitudes y reservas en Room; Backend sólo consulta rutas. `ApiService.createBooking()` apunta a una operación inexistente. La transacción Room protege un dispositivo, no la concurrencia entre dispositivos. Diseñar contrato y operación atómica en otro alcance aprobado.
- **Datos existentes:** definir migración o coexistencia de IDs `Long` de Room y ObjectId string, más propietario `ownerUserId`. No atribuir datos históricos sin identidad verificable.
