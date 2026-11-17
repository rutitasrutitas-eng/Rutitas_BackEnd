# Rutitas — Matriz de integración Backend–Android

**Fase:** A — Integración inicial
**Fecha de actualización:** 17 de noviembre de 2026
**Backend:** ASP.NET Core 10 / MongoDB
**Aplicación móvil:** Android / Kotlin / Retrofit
**Estado:** Contrato con conformidad técnica de Android. Pendiente de revisión final del líder y pruebas de integración real.

## 1. Objetivo

Organizar las actividades necesarias para conectar la aplicación Android con el Backend real de Rutitas, estableciendo responsables, dependencias, bloqueos y criterios de aceptación.

La Fase A tiene como finalidad integrar los siete endpoints que ya están implementados, utilizando la misma estructura JSON, autenticación JWT, identificadores ObjectId y respuestas HTTP.

Durante esta etapa no se desarrollarán endpoints adicionales. Las funcionalidades relacionadas con reservas, viajes, conductores, vehículos y abordajes se atenderán en una fase posterior, después de aprobar sus contratos y decisiones correspondientes.

## 2. Estado de los siete endpoints

| Endpoint | Backend | Android | Responsable |
|---|---|---|---|
| POST /auth/register | Implementado | Preparado en PR #9; pruebas reales pendientes | Mark / Ame |
| POST /auth/login | Implementado | Preparado en PR #9; pruebas reales pendientes | Mark / Ame |
| POST /auth/logout | Implementado | Preparado en PR #9; pruebas reales pendientes | Mark / Ame |
| GET /users/me | Implementado | Preparado en PR #9; pruebas reales pendientes | Mark / Ame |
| PATCH /users/me | Implementado | Preparado en PR #9; ajuste de `phone: null` pendiente | Mark / Ame |
| GET /routes | Implementado | Preparado en PR #9; manejo de HTTP 429 pendiente | Mark / Ame |
| GET /routes/{id} | Implementado | Preparado en PR #9; pruebas reales pendientes | Mark / Ame |

Todos los endpoints utilizan el prefijo `/api/v1`.

El Backend contiene la implementación de los siete endpoints y sus pruebas automatizadas.

Ame informó que el PR #9 de Android incluye la preparación de las llamadas Retrofit, los modelos JSON, el manejo de JWT y la separación del modo DEMO.

Sin embargo, todavía no se han completado las pruebas funcionales contra un Backend remoto accesible mediante HTTPS.

Por lo tanto, la preparación de Android no debe confundirse con una integración real ya terminada.

## 3. Responsabilidades

| Actividad | Responsable | Estado |
|---|---|---|
| Actualizar OpenAPI | Mark | Actualizado; corrección HTTP 503 pendiente de validación |
| Documentar ejemplos JSON | Mark | Completado |
| Revisar DTO de Android | Ame | Revisados y compatibles con el contrato |
| Adaptar ApiService.kt | Ame | Preparado en PR #9 |
| Configurar Retrofit | Ame | Preparado en PR #9 |
| Implementar manejo seguro de JWT | Ame | Preparado en PR #9 |
| Restaurar sesión en Android | Ame | Preparado en PR #9 |
| Separar modo DEMO y servicios reales | Ame | Preparado en PR #9 |
| Diferenciar `phone` omitido y `phone: null` | Ame | Ajuste pendiente |
| Incorporar HTTP 429 y Retry-After en Android | Ame | Ajuste pendiente |
| Revisar seguridad del Backend | Mark | En seguimiento |
| Corregir documentación de HTTP 503 en logout | Mark | Corrección preparada; validación pendiente |
| Proponer hosting HTTPS y MongoDB | Mark | Propuesta en preparación |
| Confirmar contrato Backend–Android | Mark / Ame | Conformidad técnica de Ame obtenida |
| Revisar y aprobar PR #6 | Líder técnico | Pendiente |
| Autorizar infraestructura | Líder técnico | Pendiente |
| Realizar pruebas entre dispositivos | Mark / Ame | Pendiente |
| Documentar resultados de integración | Mark / Ame | Pendiente |

La coordinación entre Backend y Android se realizará mediante Pull Requests y acuerdos documentados.

Las autorizaciones de integración a `main`, infraestructura y despliegue corresponden al líder técnico.

## 4. Diferencias identificadas

### Autenticación

El Backend devuelve las siguientes propiedades durante el inicio de sesión:

- `accessToken`
- `tokenType`
- `expiresIn`
- `user`

Los modelos de Android preparados en el PR #9 fueron revisados por Ame y coinciden en estructura con el contrato.

Android deberá utilizar el JWT mediante el encabezado `Authorization: Bearer`, almacenarlo de forma segura y evitar mostrarlo en registros o mensajes.

Actualmente no existe un endpoint para renovar tokens.

Cuando el JWT expire o sea rechazado por credenciales inválidas, Android deberá invalidar la sesión correspondiente y solicitar un nuevo inicio de sesión.

### Identificadores

El Backend utiliza identificadores ObjectId de MongoDB representados como String.

Android debe distinguir los identificadores remotos de las claves locales Long utilizadas por Room.

Los identificadores locales no deberán enviarse al Backend como si fueran ObjectId válidos.

Ame confirmó que los modelos preparados en Android contemplan los identificadores remotos como String.

### Consulta de rutas

El Backend devuelve un objeto paginado con las siguientes propiedades:

- `items`
- `page`
- `pageSize`
- `total`
- `totalPages`

Android debe procesar la paginación y utilizar los datos proporcionados por el servidor.

La aplicación no deberá asumir que la respuesta consiste directamente en una lista JSON.

El Backend también utiliza rate limiting en las consultas de rutas.

Cuando se supere el límite de solicitudes, puede responder HTTP 429 e incluir el encabezado `Retry-After`.

### Perfil

El Backend permite actualizar únicamente:

- `displayName`
- `phone`

No permite modificar el correo electrónico, la contraseña, el rol ni los permisos de conductor mediante `PATCH /users/me`.

Para el teléfono se establecen las siguientes reglas:

- Si se omite `phone`, se conserva el valor existente.
- Si se envía `"phone": null`, se elimina el teléfono registrado.
- Si se envía un teléfono válido, se actualiza el valor.

El formato del teléfono debe ser internacional.

Ame identificó que Android necesita distinguir correctamente un campo omitido de un valor `null` explícito durante la serialización JSON.

Esta adaptación permanece pendiente del lado móvil.

### Cierre de sesión

El endpoint `POST /auth/logout` utiliza JWT Bearer.

Sus principales resultados son:

- HTTP 204: revocación completada correctamente.
- HTTP 401: token inválido, vencido o revocado.
- HTTP 503: no fue posible completar la revocación por una falla del servicio.

Cuando el Backend devuelve HTTP 503, Android no debe interpretar la operación como una revocación exitosa.

La aplicación podrá cerrar la sesión local, pero deberá informar que no pudo confirmar la revocación remota.

### Manejo de errores

Android deberá reconocer las respuestas HTTP y los cuerpos `ProblemDetails` cuando estén disponibles.

Los errores de validación pueden incluir una propiedad `errors` con los mensajes correspondientes a cada campo.

En las respuestas HTTP 429, Android deberá respetar `Retry-After` y evitar reintentos inmediatos.

Los errores de DNS, TLS, conexión o tiempo de espera se tratarán por separado.

Una falla de conectividad no debe provocar automáticamente la eliminación de la sesión o del JWT.

### Modo DEMO

Las cuentas, aprobaciones y permisos locales del modo DEMO no representan autorizaciones reales del Backend.

Las funcionalidades que dependan de datos todavía no disponibles en el servidor deberán mantenerse claramente separadas del ambiente de integración real.

Room podrá utilizarse para almacenamiento local, pero el Backend será la fuente de verdad para las identidades, autorizaciones y datos compartidos.

## 5. Dependencias y bloqueos

| Dependencia | Impacto | Acción |
|---|---|---|
| Aprobación final del PR #6 | Impide cerrar formalmente la revisión del contrato | Solicitar revisión y autorización del líder |
| Corrección HTTP 503 de logout | Inconsistencia entre código y contrato | Validar OpenAPI y documentación |
| Backend HTTPS no disponible | Impide pruebas remotas reales | Presentar propuesta y obtener autorización |
| MongoDB de pruebas pendiente | No existe persistencia remota de pruebas verificada | Preparar base aislada después de la autorización |
| Pruebas Retrofit–Backend pendientes | No se ha demostrado el consumo real de los siete endpoints | Integrar Android al entorno autorizado |
| Serialización de `phone: null` | Puede impedir la eliminación correcta del teléfono | Ajuste de Ame |
| Manejo HTTP 429 en Android | Riesgo de reintentos incorrectos | Incorporar tratamiento de Retry-After |
| Contrato de reservas pendiente | No pueden utilizarse reservas reales | Atender durante Fase B |
| Autorización de conductores pendiente | El estado DEMO no equivale a permisos del servidor | Definir contrato en Fase B |
| Códigos de abordaje pendientes | No pueden validarse abordajes reales | Diseñar y aprobar en Fase B |

Los bloqueos de infraestructura y despliegue requieren autorización del líder antes de realizar cambios operativos.

La preparación documental y las revisiones técnicas pueden continuar sin contratar servicios.

## 6. Orden de trabajo y PR

### PR A1 — Contrato de integración Backend–Android

**PR real:** Backend #6
**Responsable:** Mark
**Estado:** En Draft, pendiente de concluir correcciones documentales y revisión formal.

Incluye:

- Contrato Backend–Android.
- OpenAPI actualizado.
- Matriz de integración.
- Documentación de compatibilidad.
- Ejemplos de solicitudes y respuestas JSON.
- Políticas de autenticación y manejo de errores.
- Evidencia de pruebas automatizadas.

No modifica la lógica de los endpoints implementados.

Ame otorgó conformidad técnica al contrato documentado en el commit `8a74778`.

El líder solicitó ajustes adicionales sobre HTTP 503, evidencia de CI, matriz de integración y README antes de avanzar a Ready for Review.

### PR A2 — Preparación de integración Android

**PR real:** Android #9
**Responsable:** Ame
**Estado:** Preparación técnica comunicada por Android; integración real pendiente.

Incluye:

- Modelos DTO compatibles con el Backend.
- Configuración de Retrofit.
- Manejo y almacenamiento seguro de JWT.
- Restauración de sesión.
- Manejo inicial de errores.
- Separación del modo DEMO y servicios reales.
- Preparación de los siete endpoints para su consumo.

Ame informó que el PR #9 superó seis comprobaciones de GitHub Actions.

Todavía quedan ajustes para `phone: null`, HTTP 429 y Retry-After, además de las pruebas contra el servidor HTTPS.

### PR A3 — Propuesta y configuración del entorno de pruebas

**Responsable:** Mark
**Estado:** Propuesta técnica en preparación.

Requiere aprobación de infraestructura y presupuesto.

Incluye:

- Evaluación de proveedores de alojamiento.
- Arquitectura del entorno HTTPS.
- MongoDB de pruebas.
- Gestión segura de credenciales.
- Variables de entorno.
- Configuración de ambiente.
- Estrategia de despliegue.
- Pruebas de disponibilidad, conectividad y seguridad.

La propuesta documental no autoriza contratar infraestructura ni realizar despliegues.

La implementación del entorno deberá realizarse únicamente después de la autorización del líder.

### PR A4 — Validación funcional Android–Backend

**Responsables:** Mark y Ame
**Estado:** Pendiente del entorno de pruebas.

Incluirá:

- Pruebas de los siete endpoints reales.
- Verificación de autenticación JWT.
- Pruebas de registro y actualización de perfil.
- Consulta y paginación de rutas.
- Manejo de errores HTTP.
- Verificación de conectividad.
- Pruebas con dos usuarios y dos dispositivos.
- Evidencia de resultados y correcciones.

Este PR o conjunto de PR deberá definirse de acuerdo con los repositorios afectados y las tareas autorizadas.

## 7. Criterios de aceptación

### Contrato y preparación técnica

- [x] Siete endpoints implementados en Backend.
- [x] Modelos JSON revisados entre Backend y Android.
- [x] Identificadores ObjectId documentados como String.
- [x] Respuestas paginadas documentadas.
- [x] Conformidad técnica de Ame obtenida.
- [x] Evidencia de GitHub Actions registrada.
- [ ] Corrección HTTP 503 validada en OpenAPI y documentación.
- [ ] Actualización documental final revisada.
- [ ] PR #6 aprobado formalmente por el líder.

### Infraestructura

- [ ] Propuesta HTTPS y MongoDB entregada.
- [ ] Proveedor de alojamiento autorizado.
- [ ] Presupuesto aprobado, cuando corresponda.
- [ ] MongoDB de pruebas configurado.
- [ ] URL HTTPS autorizada y funcional.
- [ ] Credenciales y variables de entorno configuradas de forma segura.

### Integración Android–Backend

- [ ] Registro conectado al servidor real.
- [ ] Login real con JWT.
- [ ] Logout con revocación remota comprobada.
- [ ] Consulta del perfil propio desde Backend.
- [ ] Actualización del perfil desde Android.
- [ ] Manejo correcto de `phone: null`.
- [ ] Consulta del listado real de rutas.
- [ ] Consulta del detalle real de una ruta.
- [ ] Identificadores ObjectId tratados correctamente en ejecución.
- [ ] Paginación comprobada desde Android.
- [ ] Manejo de HTTP 401, 429 y 503 según corresponda.
- [ ] Manejo de errores de conectividad sin cierre de sesión incorrecto.
- [ ] Separación DEMO y servicios reales comprobada.
- [ ] Pruebas con dos usuarios y dos dispositivos.
- [ ] Evidencia de integración funcional documentada.

La Fase A se considerará completada cuando Android consuma correctamente los siete endpoints del Backend real y se cuente con pruebas que demuestren que la información proviene del servidor.

La aprobación del contrato por sí sola no significa que la integración funcional esté terminada.

## 8. Funcionalidades posteriores

Las siguientes operaciones pertenecen a la Fase B:

- Gestión de viajes.
- Registro y aprobación de conductores.
- Gestión de vehículos y documentos.
- Reservas y cancelaciones.
- Control atómico de lugares disponibles.
- Prevención de sobreventa.
- Abordajes mediante códigos de un solo uso.
- Eliminación remota de cuentas.

Estas operaciones requieren contratos, decisiones SDD, autorización, implementación y pruebas antes de incorporarse a Android.

Para los procesos de reservas y disponibilidad será necesario coordinar las decisiones pendientes con Byron, especialmente concurrencia, idempotencia y prevención de sobreventa (SR-10).

No se implementarán estos endpoints durante la Fase A.

## 9. Reglas de trabajo

- Trabajar mediante ramas independientes.
- No modificar directamente `main`.
- Utilizar Pull Requests revisables.
- Mantener el Backend como fuente de verdad.
- No almacenar contraseñas, tokens ni secretos en Git.
- Utilizar HTTPS en las conexiones reales de pruebas.
- Separar los ambientes DEMO, pruebas y release.
- Documentar los cambios antes de solicitar aprobación.
- No fusionar PR sin las autorizaciones correspondientes.
- No contratar servicios ni desplegar infraestructura sin aprobación.
- Mantener la trazabilidad entre contratos, implementación y pruebas.
- Respetar el alcance autorizado de los siete endpoints.

## 10. Referencias

**Repositorio Backend:**
https://github.com/rutitasrutitas-eng/Rutitas_BackEnd

**PR #6 — Contrato de integración Backend–Android:**
https://github.com/rutitasrutitas-eng/Rutitas_BackEnd/pull/6

**Repositorio Android:**
https://github.com/equipopatogg/Movil

**Documentos relacionados:**

- `docs/CONTRATO_INTEGRACION_ANDROID.md`
- `docs/COMPATIBILIDAD_ANDROID.md`
- `docs/VALIDACION_MARK.md`
- `docs/CI_CD.md`
- `specs/001-unit2-api/contracts/openapi.yaml`
- PR #5 del Backend.
- PR #9 de Android.
- Especificaciones SDD de Rutitas.

**Evidencia de GitHub Actions del Backend:**

Commit `de79303`:
https://github.com/rutitasrutitas-eng/Rutitas_BackEnd/actions/runs/44000642612

Commit `8a74778`:
https://github.com/rutitasrutitas-eng/Rutitas_BackEnd/actions/runs/44008799552

Ambas ejecuciones registraron tres comprobaciones exitosas.

---

## 11. Observaciones de revisión de Android

Durante la revisión conjunta del PR #6, Ame identificó aspectos relacionados con el contrato de los siete endpoints.

| Observación | Acuerdo documentado | Responsable |
|---|---|---|
| GET /routes y HTTP 429 | Documentar HTTP 429 en OpenAPI por el rate limiting del Backend | Mark |
| Expiración JWT y HTTP 401 | Invalidar sesión local y solicitar nuevo login cuando corresponda | Mark / Ame |
| Logout sin conexión | Permitir cierre local sin afirmar revocación remota | Mark / Ame |
| Logout y HTTP 503 | Documentar que la revocación no pudo confirmarse | Mark |
| PATCH /users/me y `phone: null` | Diferenciar campo omitido de null explícito | Mark / Ame |
| ProblemDetails | Interpretar errores estructurados y validaciones por campo | Mark / Ame |
| Retry-After | Respetar el encabezado cuando exista HTTP 429 | Ame |
| Errores de conectividad | No confundir fallos de red con respuestas HTTP | Ame |
| Funciones DEMO | No presentarlas como servicios reales de Fase A | Ame |
| Reservas, conductores y abordajes | Definir contratos e implementación en Fase B | Mark / Ame |

**Estado de los acuerdos:** Ame otorgó conformidad técnica al contrato documentado en el commit `8a74778`.

Las aclaraciones adicionales sobre HTTP 503 y la actualización documental se están preparando para la revisión solicitada por el líder.

La aprobación final del PR #6 continúa pendiente.

---

## 12. Avance coordinado con Android — PR #9

Ame informó que el PR #9 de Android completó su primera etapa de preparación y superó seis comprobaciones de GitHub Actions.

### Avances comunicados por Android

- Preparación de los siete endpoints en Retrofit.
- Modelos JSON compatibles con el contrato Backend.
- Identificadores ObjectId representados como String.
- Manejo de respuestas paginadas.
- Almacenamiento seguro de JWT.
- Restauración de sesión.
- Separación del modo DEMO y servicios reales.
- Configuración de componentes necesarios para la comunicación HTTP.

Ame revisó el commit `8a74778` del PR #6 y otorgó conformidad técnica sobre los modelos JSON, las reglas de autenticación y el manejo de errores documentado.

### Adaptaciones pendientes del equipo Android

- Diferenciar correctamente `phone` omitido de `phone: null`.
- Incorporar HTTP 429 y Retry-After en las pantallas conectadas.
- Verificar el manejo de HTTP 503 durante logout.
- Completar las pruebas contra el Backend real mediante HTTPS.
- Registrar evidencia de los resultados funcionales.

### Dependencias pendientes

- Validación de los ajustes finales del PR #6.
- Aprobación formal del líder.
- Autorización del entorno HTTPS y MongoDB.
- Disponibilidad de una URL de pruebas segura.
- Configuración de la dirección base en Android.
- Validación de los siete endpoints con información real.

### Próximas actividades conjuntas

**Mark — Backend**

- Finalizar los ajustes documentales solicitados.
- Validar OpenAPI.
- Mantener las comprobaciones de GitHub Actions.
- Preparar la propuesta del entorno de pruebas.
- Coordinar la configuración del Backend después de obtener autorización.
- Apoyar las pruebas funcionales con Ame.

**Ame — Android**

- Completar las adaptaciones de serialización y manejo de errores.
- Configurar la URL HTTPS autorizada.
- Validar las llamadas Retrofit.
- Comprobar el almacenamiento y uso del JWT.
- Confirmar que los datos mostrados provienen del Backend.
- Documentar los errores y resultados encontrados.

**Mark y Ame — Validación conjunta**

- Probar los siete endpoints reales.
- Realizar pruebas de autenticación y cierre de sesión.
- Comprobar actualización y consulta de perfiles.
- Validar paginación y detalle de rutas.
- Confirmar el manejo correcto de errores.
- Reunir evidencias para la revisión del líder.

**Estado de integración:** preparación técnica avanzada, con conformidad del contrato por parte de Android. Pendiente de autorización de infraestructura, aprobación del PR y pruebas funcionales contra el servidor real.

---

**Estado final del documento:** matriz actualizada para revisión del equipo y del líder técnico. No representa autorización de merge, contratación de servicios, despliegue ni implementación de funcionalidades de Fase B.