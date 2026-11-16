# Compatibilidad de la API con Android

**Fuentes observadas el 13/11/2026:** Backend `feature/sdd-azure-devsecops` en `2cb6e016`; Android `feature/integracion-android-ci-cd` en `4b2ea8d10f4dbc94830db515a65dd3d00c6a66ad`. El contrato entre equipos sigue propuesto. `ApiService.kt` declara tres llamadas remotas futuras y el modo DEMO usa Room; la pantalla actual no consume estos siete endpoints de forma integral.

| Endpoint Backend | Estado Android actual | Diferencia para integrar |
|---|---|---|
| `POST /api/v1/auth/register` | `AuthRepository` registra en Room y crea sesión local. | El backend requiere `email`, `password`, `displayName`, `phone?`, responde `201` con `UserProfile` y no entrega token. Android pide `name`, `type` e email institucional; debe acordarse política y login posterior. |
| `POST /api/v1/auth/login` | `ApiService` declara `auth/login`, pero espera `{token,name,email}`; `AuthRepository` autentica localmente. | API devuelve `{accessToken,tokenType,expiresIn,user}`. Configurar base URL `/api/v1/`, mapeo DTO, encabezado Bearer y manejo de `400/401/429/500`. |
| `POST /api/v1/auth/logout` | `SessionManager` sólo limpia sesión en memoria. | Invocar revocación remota antes de descartar el token, manejar 204/401/500 y contemplar fallos de conexión. |
| `GET /api/v1/users/me` | Perfil sale de Room/`Session`; no hay llamada Retrofit equivalente. | ID del servidor es ObjectId string, Android usa `Long`. DTO servidor: `id,email,displayName,phone,role,createdAt`; Android: `id,name,email,type,university`. Requiere mapeo explícito. |
| `PATCH /api/v1/users/me` | Android permite editar nombre, email, tipo, zona y horario localmente. | Backend sólo acepta `displayName` y `phone`; rechaza `role`, email y otras propiedades. No enviar el modelo Room completo. Manejar `400/401/500`. |
| `GET /api/v1/routes` | `ApiService.routes()` espera `List<RouteDto>`; Room usa IDs `Long`. | Backend devuelve objeto `{items,page,pageSize,total,totalPages}` con rutas activas públicas. Usa ID ObjectId string, `code`, ubicaciones `{name,lat,lng}`, distancia y duración. Android espera horario, tarifa, asientos, conductor, vehículo, días y paradas string; faltan en resumen. Autenticación Bearer obligatoria; manejar `400/401/500`. |
| `GET /api/v1/routes/{id}` | No hay llamada Retrofit de detalle; datos salen de Room. | Backend usa ObjectId string, devuelve paradas con orden/coordenadas, descripción y ubicaciones; `400/401/404/500`. No puede pasarse un ID `Long` local. |

## Decisiones de compatibilidad pendientes

- **Identidad y roles:** Backend emite rol `user`; Android usa `UserType` (`ALUMNO`, `DOCENTE`, `PERSONAL`) y estados de conductor propios. No mapear estos conceptos por coincidencia de nombre ni conceder permisos del servidor desde datos locales.
- **Token:** `SessionManager` actual sólo guarda sesión en memoria; no hay almacenamiento de JWT ni interceptor Bearer. Definir almacenamiento seguro, expiración, logout, cierre por `401` y política de reconexión. Evitar logs de tokens.
- **Errores:** acordar parseo de `ProblemDetails`, `ValidationProblemDetails`, `Retry-After` y mensajes de usuario; las excepciones Retrofit no representan directamente errores de dominio.
- **Viajes y reservas:** Android tiene rutas, viajes, cupos, solicitudes y reservas en Room; Backend sólo consulta rutas. `ApiService.createBooking()` apunta a una operación inexistente. La transacción Room protege un dispositivo, no la concurrencia entre dispositivos. Diseñar contrato y operación atómica en otro alcance aprobado.
- **Datos existentes:** definir migración o coexistencia de IDs `Long` de Room y ObjectId string, más propietario `ownerUserId`. No atribuir datos históricos sin identidad verificable.

---

## Actualización de integración — 15 de noviembre de 2026

### Estado actual del Backend

El PR #5 fue integrado a `main` y contiene los siete
endpoints de autenticación, perfil y consulta de rutas.

La validación local posterior a las correcciones del PR #4
registró 71 pruebas aprobadas, 0 fallidas y 0 omitidas.

El Backend todavía no dispone de un entorno HTTPS
de pruebas autorizado y verificado.

### Contrato de integración

El contrato propuesto para coordinar Backend y Android
está documentado en:

- `docs/CONTRATO_INTEGRACION_ANDROID.md`
- `docs/MATRIZ_INTEGRACION.md`
- `specs/001-unit2-api/contracts/openapi.yaml`

Estos documentos están pendientes de revisión y
aprobación conjunta con el equipo móvil.

### Adaptaciones pendientes en Android

1. Adaptar el DTO de login a `accessToken`,
   `tokenType`, `expiresIn` y `user`.
2. Separar los identificadores ObjectId `String`
   de las claves locales `Long` de Room.
3. Procesar las rutas desde `items` y manejar
   sus metadatos de paginación.
4. Adaptar registro y perfil a los campos
   aceptados por el Backend.
5. Implementar almacenamiento seguro de JWT
   y encabezados Bearer.
6. Manejar respuestas HTTP 401 y 429,
   además de los errores de conectividad.
7. Mantener separadas las funciones DEMO
   de los servicios reales.

### Funcionalidades que todavía no existen en el Backend

- Reservas y cancelaciones.
- Gestión y aprobación real de conductores.
- Gestión de vehículos y documentos.
- Viajes y control atómico de cupos.
- Validación de abordaje de un solo uso.
- Eliminación remota de cuentas.

Estas funcionalidades requieren contratos,
implementación, pruebas y aprobación posteriores.

### Próximos pasos

- Backend: presentar el contrato para revisión.
- Android: revisar Retrofit, ApiService.kt y DTO.
- Ambos: acordar los modelos definitivos.
- Backend: proponer el entorno HTTPS de pruebas.
- Equipo: revisar y aprobar los PR correspondientes.

**Estado:** Fase A en preparación. No se ha
realizado despliegue productivo.
