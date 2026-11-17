# Rutitas — Contrato de integración Backend–Android
**Proyecto:** Rutitas UTTT
**Fase:** A — Integración inicial
**Fecha:** 15 de noviembre de 2026
**Backend:** ASP.NET Core 10 / MongoDB
**Cliente:** Android / Kotlin / Retrofit
**Versión del contrato:** 1.0.0-draft
**Estado:** Propuesto para revisión conjunta de Backend, Móvil y líder técnico.

## 1. Objetivo

Establecer el contrato de comunicación entre la aplicación Android de Rutitas y su Backend, para integrar autenticación, perfil de usuario y consulta de rutas.

Este documento describe las operaciones existentes, los modelos JSON, las reglas de seguridad, los errores y las responsabilidades de ambos equipos.

La integración deberá utilizar el servidor como fuente de verdad para identidades y datos compartidos. Room podrá continuar utilizándose para almacenamiento local, pero no para conceder permisos del servidor.

## 2. Estado actual

El PR #5 del Backend fue integrado a `main`.

La última validación local posterior al PR #4 registró:

- 71 pruebas aprobadas.
- 0 pruebas fallidas.
- 0 pruebas omitidas.
- Compilación Release exitosa.
- Cinco suites xUnit ejecutadas.
- Pruebas de MongoDB con Testcontainers exitosas.

Esto demuestra el funcionamiento de las pruebas ejecutadas, pero no equivale a validar una integración Android–Backend por Internet.

**Estado de despliegue:** no existe todavía una URL HTTPS de pruebas aprobada y verificada.

**Fuente del contrato:**

`specs/001-unit2-api/contracts/openapi.yaml`

### 2.1 Evidencia técnica verificada

**Validación local**

Después de integrar el PR #4 se ejecutaron las cinco
suites de pruebas xUnit con .NET 10.

Resultados:
- 71 pruebas aprobadas.
- 0 fallidas.
- 0 omitidas.
- Compilación Release exitosa.
- MongoDB con Testcontainers funcionando.

Estos resultados corresponden a la ejecución local.

**GitHub Actions — PR #6**

Commit verificado: `b40122b`

Ejecución:
https://github.com/rutitasrutitas-eng/Rutitas_BackEnd/actions/runs/43811605917

Estado de la ejecución: completada correctamente.

Las comprobaciones incluyen compilación y análisis .NET,
suites de pruebas, validación SDD/YAML y Gitleaks.

La ejecución automatizada y la validación local son
evidencias independientes.

**Alcance**

Las pruebas automatizadas no sustituyen las futuras
pruebas funcionales entre Android y el Backend HTTPS.

**GitHub Actions — Actualización de la Fase A**

Commit: `de79303`

Ejecución:
https://github.com/rutitasrutitas-eng/Rutitas_BackEnd/actions/runs/44000642612

Resultado: tres comprobaciones exitosas.

**Validación adicional**

Commit: `8a74778`

Ejecución:
https://github.com/rutitasrutitas-eng/Rutitas_BackEnd/actions/runs/44008799552

Resultado: tres comprobaciones exitosas.

Los resultados de GitHub Actions corresponden
a los commits indicados y son independientes
de las pruebas ejecutadas localmente.


## 3. Dirección base

Todos los endpoints existentes utilizan el prefijo:

`/api/v1`

La URL final del entorno de pruebas será proporcionada después de aprobar y desplegar la infraestructura.

Las direcciones de ejemplo presentes en el OpenAPI no deben utilizarse como servidores reales.

Android deberá configurar la dirección base por ambiente: DEMO, pruebas y release.

## 4. Endpoints implementados

| Método | Endpoint | Autenticación | Éxito |
|---|---|---|---|
| POST | /auth/register | No | 201 |
| POST | /auth/login | No | 200 |
| POST | /auth/logout | JWT Bearer | 204 |
| GET | /users/me | JWT Bearer | 200 |
| PATCH | /users/me | JWT Bearer | 200 |
| GET | /routes | JWT Bearer | 200 |
| GET | /routes/{id} | JWT Bearer | 200 |

Las rutas anteriores son relativas al prefijo `/api/v1`.

## 5. Registro de usuario

### POST /api/v1/auth/register

**Petición JSON de ejemplo:**

```json
{
  "email": "alumno@ejemplo.edu.mx",
  "password": "EjemploSeguro123!",
  "displayName": "Alumno Prueba",
  "phone": "+525512345678"
}
```

Reglas definidas en el OpenAPI:

- `email`: obligatorio, formato de correo electrónico, máximo 254 caracteres.
- `password`: obligatoria, entre 12 y 128 caracteres.
- `displayName`: obligatorio, entre 1 y 80 caracteres.
- `phone`: opcional, acepta `null` o formato internacional.
- No se admiten propiedades adicionales.
- El cliente no puede asignarse un rol administrativo.

**Respuesta 201 de ejemplo:**

```json
{
  "id": "507f1f77bcf86cd799439011",
  "email": "alumno@ejemplo.edu.mx",
  "displayName": "Alumno Prueba",
  "phone": "+525512345678",
  "role": "user",
  "createdAt": "2026-11-15T12:00:00Z"
}
```

El registro no devuelve un JWT. Android debe solicitar posteriormente el inicio de sesión.

**Errores documentados:** 400, 409, 429 y 500.

## 6. Inicio de sesión

### POST /api/v1/auth/login

**Petición JSON:**

```json
{
  "email": "alumno@ejemplo.edu.mx",
  "password": "EjemploSeguro123!"
}
```

**Respuesta 200 de ejemplo:**

```json
{
  "accessToken": "<jwt-de-prueba>",
  "tokenType": "Bearer",
  "expiresIn": 3600,
  "user": {
    "id": "507f1f77bcf86cd799439011",
    "email": "alumno@ejemplo.edu.mx",
    "displayName": "Alumno Prueba",
    "phone": "+525512345678",
    "role": "user",
    "createdAt": "2026-11-15T12:00:00Z"
  }
}
```

`expiresIn` representa la duración del token en segundos. El valor 3600 es ilustrativo.

Android deberá utilizar los nombres exactos:

- `accessToken`
- `tokenType`
- `expiresIn`
- `user`

No debe asumir que el Backend devuelve las propiedades `token`, `name` y `email` en el nivel principal.

**Errores documentados:** 400, 401, 429 y 500.

## 7. Cierre de sesión

### POST /api/v1/auth/logout

Requiere:

`Authorization: Bearer <token>`

**Respuesta de éxito:** HTTP 204, sin cuerpo.

El Backend revoca el identificador `jti` del JWT. Android deberá invalidar su sesión local y evitar reutilizar el token.

Un token inválido, vencido o revocado puede producir HTTP 401.

**Errores documentados:** 401 y 503.

Cuando la revocación del token falla, el Backend
responde HTTP 503 mediante ProblemDetails.

Android no deberá interpretar HTTP 503 como una
revocación exitosa.

Puede cerrar la sesión local de forma segura, pero
deberá informar que no se confirmó la revocación remota.

No se debe reutilizar el token ni registrar su valor
en mensajes o logs.


El comportamiento sin conexión debe acordarse con el equipo móvil, especialmente cuando no sea posible confirmar la revocación remota.

## 8. Consulta de perfil

### GET /api/v1/users/me

Requiere JWT Bearer.

**Respuesta 200 de ejemplo:**

```json
{
  "id": "507f1f77bcf86cd799439011",
  "email": "alumno@ejemplo.edu.mx",
  "displayName": "Alumno Prueba",
  "phone": null,
  "role": "user",
  "createdAt": "2026-11-15T12:00:00Z"
}
```

El perfil corresponde al usuario autenticado. El cliente no debe poder consultar otro perfil simplemente cambiando un identificador local.

**Errores documentados:** 401 y 500.

## 9. Actualización del perfil

### PATCH /api/v1/users/me

Requiere JWT Bearer.

**Petición JSON:**

```json
{
  "displayName": "Alumno Actualizado",
  "phone": "+525512345678"
}
```

Se puede enviar uno o ambos campos.

No se permite modificar mediante esta operación:

- Email.
- Contraseña.
- Rol.
- Permisos de conductor.

La respuesta 200 contiene el perfil actualizado con el mismo esquema de `UserProfile`.

**Errores documentados:** 400, 401 y 500.

## 10. Consulta de rutas

### GET /api/v1/routes

Requiere JWT Bearer.

Parámetros opcionales:

| Parámetro | Tipo | Regla |
|---|---|---|
| page | Integer | Mínimo 1; predeterminado 1 |
| pageSize | Integer | Entre 1 y 100; predeterminado 20 |
| q | String | Búsqueda, entre 1 y 100 caracteres |

**Ejemplo de solicitud:**

`GET /api/v1/routes?page=1&pageSize=20`

**Respuesta 200 de ejemplo:**

```json
{
  "items": [
    {
      "id": "507f1f77bcf86cd799439012",
      "code": "RUT-001",
      "name": "Ruta Universidad",
      "origin": {
        "name": "Origen",
        "lat": 20.0,
        "lng": -99.3
      },
      "destination": {
        "name": "UTTT",
        "lat": 20.0,
        "lng": -99.4
      },
      "distanceKm": 12.5,
      "estimatedDurationMin": 25
    }
  ],
  "page": 1,
  "pageSize": 20,
  "total": 1,
  "totalPages": 1
}
```

El Backend devuelve un objeto paginado, no una lista JSON directa.

Android deberá obtener las rutas desde `items` y utilizar los metadatos para implementar la paginación.

Este resumen no contiene todavía toda la información necesaria para reservar lugares, como conductor, vehículo, cupos o fechas de viaje.

**Errores documentados:** 400, 401, 429 y 500.

## 11. Detalle de una ruta

### GET /api/v1/routes/{id}

Requiere JWT Bearer.

`id` debe ser un ObjectId hexadecimal de 24 caracteres.

**Ejemplo de solicitud:**

`GET /api/v1/routes/507f1f77bcf86cd799439012`

**Respuesta 200 de ejemplo:**

```json
{
  "id": "507f1f77bcf86cd799439012",
  "code": "RUT-001",
  "name": "Ruta Universidad",
  "description": "Trayecto de prueba hacia la universidad",
  "origin": {
    "name": "Origen",
    "lat": 20.0,
    "lng": -99.3
  },
  "destination": {
    "name": "UTTT",
    "lat": 20.0,
    "lng": -99.4
  },
  "stops": [
    {
      "order": 1,
      "name": "Parada de prueba",
      "lat": 20.0,
      "lng": -99.35
    }
  ],
  "distanceKm": 12.5,
  "estimatedDurationMin": 25
}
```

**Errores documentados:** 400, 401, 404, 429 y 500.

## 12. Autenticación JWT

El Backend utiliza autenticación HTTP Bearer con tokens JWT.

Android deberá:

1. Obtener el token mediante login.
2. Almacenarlo mediante un mecanismo seguro.
3. Enviarlo en el encabezado `Authorization`.
4. Controlar el tiempo de expiración.
5. Gestionar HTTP 401.
6. Borrar o invalidar la sesión local cuando corresponda.
7. No registrar tokens ni contraseñas en logs.

El Backend verifica los datos del JWT, incluyendo emisor, audiencia, vigencia y los identificadores `sub` y `jti`.

También comprueba revocación y estado de usuario.

La gestión de renovación de tokens no forma parte de los siete endpoints actuales. Android no debe asumir que existe un endpoint de refresh.

### 12.1 Política propuesta de sesiones Android

Esta política se encuentra pendiente de confirmación conjunta
con el equipo móvil.

**JWT vencido**

Cuando el token expire, Android deberá dejar de utilizarlo
y solicitar al usuario que inicie sesión nuevamente.

Actualmente no existe un endpoint de renovación de tokens.

**Respuesta HTTP 401**

Cuando una operación protegida devuelva HTTP 401 por
credenciales inválidas, vencidas o revocadas, Android deberá:

1. Invalidar la sesión local.
2. Eliminar el token almacenado de forma segura.
3. Evitar reintentos automáticos con el mismo token.
4. Solicitar un nuevo inicio de sesión.

**Cierre de sesión con conexión**

Android deberá solicitar POST /api/v1/auth/logout para
revocar el token en el servidor y después eliminar la
sesión local.

**Cierre de sesión sin conexión**

Si el usuario solicita cerrar sesión sin Internet, Android
podrá eliminar la sesión y bloquear el acceso local.

Sin embargo, deberá informar que la revocación remota
no pudo confirmarse.

El cierre local no garantiza que el token haya sido
revocado en el servidor.

El token no deberá reutilizarse ni quedar disponible
para otras cuentas.

La estrategia de revocación diferida, si fuera necesaria,
requiere un diseño adicional de seguridad.

**Errores de conectividad**

Una falla de conexión no debe interpretarse automáticamente
como un HTTP 401.

Android deberá distinguir errores de red, respuestas HTTP
y credenciales inválidas.

## 13. Manejo de errores

El contrato utiliza respuestas `application/problem+json`.

Estructura general ilustrativa:

```json
{
  "type": "about:blank",
  "title": "Bad Request",
  "status": 400,
  "detail": "La solicitud contiene datos inválidos.",
  "traceId": "ejemplo-trace-id"
}
```

Los errores de validación pueden incluir un objeto adicional llamado `errors`, con mensajes asociados a cada campo.

| Código | Significado | Acción propuesta para Android |
|---|---|---|
| 400 | Solicitud inválida | Mostrar qué datos deben corregirse |
| 401 | No autorizado | Gestionar expiración o cierre de sesión |
| 404 | Recurso inexistente | Mostrar que la ruta no está disponible |
| 409 | Conflicto | Informar, por ejemplo, de email duplicado |
| 429 | Demasiadas solicitudes | Respetar `Retry-After` cuando exista |
| 500 | Error del servidor | Mostrar mensaje general y permitir reintento |

No todos estos códigos están declarados para todos los endpoints.

Errores de conectividad, DNS y tiempo de espera deben manejarse por separado de las respuestas HTTP.

### 13.1 Acuerdos de manejo de errores y actualización de perfil

**Actualización de teléfono**

En PATCH /api/v1/users/me:

- Si se omite `phone`, se conserva el valor existente.
- Si se envía `"phone": null`, se elimina el teléfono.
- Si se envía un string válido, se actualiza el teléfono.
- El formato aceptado es internacional, por ejemplo:
  `+525512345678`.

Android deberá diferenciar un campo ausente de uno
enviado explícitamente como null.

**ProblemDetails**

Android deberá interpretar las respuestas HTTP de
error y, cuando exista, el cuerpo ProblemDetails.

Los errores de validación pueden incluir la propiedad
`errors`, con mensajes específicos por campo.

La aplicación no debe depender de textos exactos de
`title` o `detail` para decidir su comportamiento.

**HTTP 429 y Retry-After**

El Backend aplica rate limiting a las consultas de rutas.

Actualmente devuelve:
- HTTP 429.
- Header `Retry-After: 60`.
- Cuerpo de error basado en ProblemDetails.

Android deberá respetar Retry-After y evitar
reintentos inmediatos o repetitivos.

No debe realizar reintentos automáticos de operaciones
que puedan producir efectos secundarios sin una política
de idempotencia aprobada.

**Errores de conectividad**

Las fallas de DNS, TLS, conexión y tiempo de espera no
son respuestas HTTP 401 ni HTTP 500 del servidor.

Android deberá informar el problema y permitir
reintentar cuando corresponda, sin cerrar sesión
automáticamente por una falla de red.

**Entorno HTTPS**

La URL definitiva de pruebas queda pendiente de
autorización del líder.

El Backend y Android realizarán las pruebas reales
cuando exista un entorno HTTPS autorizado,
configurado y validado.

La Fase A no se considerará completa únicamente
con pruebas locales, mocks o modo DEMO.

## 14. Compatibilidad con Android

| Elemento | Backend | Adaptación Android |
|---|---|---|
| Identificador | ObjectId String | Crear campo de ID remoto String |
| Registro | UserProfile sin JWT | Hacer login posteriormente |
| Login | accessToken y user | Actualizar DTO |
| Perfil | displayName, phone, role | Mapear datos sin elevar permisos |
| Rutas | PagedRoutes | Leer items y paginación |
| Sesión | JWT Bearer | Almacenamiento seguro e interceptor |
| Roles | user | No equiparar con estado DEMO de conductor |
| Reservas | No implementadas | No enviar solicitudes reales todavía |

Los IDs `Long` de Room no deben enviarse como si fueran ObjectId del servidor.

No se deben importar automáticamente identidades o permisos DEMO al entorno real.

## 15. Endpoints pendientes del MVP

Los siguientes módulos necesitan especificación, aprobación, implementación y pruebas:

- Creación y gestión de viajes por conductores.
- Solicitudes y reservas.
- Cancelación de reservas.
- Actualización atómica de asientos disponibles.
- Registro y revisión de conductores.
- Documentos y vehículos.
- Aprobación administrativa de conductores.
- Generación y validación de códigos de abordaje de un solo uso.
- Eliminación de cuenta y tratamiento de datos asociados.

Todavía no forman parte de los siete endpoints implementados.

Los nombres, cuerpos JSON y respuestas de estas operaciones se definirán en un contrato posterior aprobado.

### 15.1 Datos de rutas no disponibles en Fase A

El Backend actual permite consultar información básica
de las rutas, pero todavía no ofrece un contrato completo
para operaciones de transporte.

Los siguientes datos quedan pendientes de Fase B:

- Información del conductor autorizado.
- Información del vehículo.
- Horarios y fechas de salida.
- Lugares disponibles.
- Precio o contribución del viaje.
- Estado de reservas.
- Confirmación de abordajes.

Durante la Fase A, Android no deberá presentar estos datos
como información real del servidor cuando procedan
únicamente de Room o del modo DEMO.

Las pantallas que dependan de estas funcionalidades
deberán mantenerse claramente identificadas como DEMO,
ocultarse o deshabilitarse en el ambiente de integración
real, según lo acordado por el equipo.

La fuente de verdad para disponibilidad, reservas,
identidad y autorización será el Backend una vez
implementadas las operaciones correspondientes.

## 16. Responsabilidades

### Backend — Mark

- Mantener los siete endpoints actuales.
- Actualizar documentación OpenAPI.
- Preparar los ejemplos de integración.
- Proponer infraestructura HTTPS de pruebas.
- Documentar operaciones nuevas antes de implementarlas.
- Mantener pruebas automatizadas y seguridad.

### Android — Ame

- Revisar ApiService.kt.
- Adaptar los DTO a los modelos del servidor.
- Configurar Retrofit por ambiente.
- Implementar manejo seguro del JWT.
- Separar DEMO y autenticación real.
- Integrar los siete endpoints disponibles.
- Mantener Room solo para los datos locales necesarios.

### Trabajo conjunto

- Aprobar modelos JSON definitivos.
- Revisar la matriz de integración.
- Comprobar la comunicación Android–Backend.
- Realizar pruebas con dos usuarios y dos dispositivos.
- Documentar resultados y errores encontrados.

## 17. Criterios de aceptación de Fase A

La primera integración se considerará lista para revisión cuando:

- El contrato haya sido aprobado por Backend y Android.
- Exista una URL HTTPS de pruebas autorizada.
- Registro y login utilicen el servidor.
- Android guarde y envíe el token correctamente.
- El perfil se obtenga del usuario autenticado.
- Las rutas se consulten desde el Backend.
- La paginación y los ObjectId se manejen correctamente.
- Los errores 400, 401, 429 y de red tengan tratamiento adecuado.
- Las funciones no implementadas estén separadas del modo real.
- Existan pruebas y evidencias de funcionamiento.

## 18. Decisiones pendientes

- Aprobación conjunta del contrato.
- Reglas de verificación de pertenencia a la universidad.
- Diferenciación entre usuario, tipo de comunidad y conductor.
- Estrategia de almacenamiento del JWT en Android.
- Política de sesión y recuperación después de errores de red.
- Hosting, costos y aprobación del entorno de pruebas.
- Diseño de reservas, cupos y abordajes.
- Política de eliminación y retención de datos.
- Alcance del primer MVP publicable.

## 19. Referencias

- `specs/001-unit2-api/contracts/openapi.yaml`
- `docs/COMPATIBILIDAD_ANDROID.md`
- `docs/VALIDACION_MARK.md`
- `docs/CI_CD.md`
- Pull Request Backend #5.
- Especificaciones SDD de Rutitas.

---

**Nota:** todos los valores JSON de este documento son ejemplos de estructura, no datos reales ni respuestas capturadas de un servidor desplegado.

**Estado final:** documento inicial para revisión y acuerdo entre Backend y Android. No implica aprobación de despliegue ni de nuevas funcionalidades.
