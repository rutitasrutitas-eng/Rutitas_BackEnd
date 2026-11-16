
# Rutitas — Matriz de integración Backend–Android

**Fase:** A — Integración inicial
**Fecha:** 15 de noviembre de 2026
**Estado:** Propuesta pendiente de revisión conjunta

## 1. Objetivo

Organizar las actividades necesarias para conectar Android
con el Backend real de Rutitas, estableciendo responsables,
dependencias, bloqueos y criterios de aceptación.

## 2. Estado de los siete endpoints

| Endpoint | Backend | Android | Responsable |
|---|---|---|---|
| POST /auth/register | Implementado | Adaptación pendiente | Mark / Ame |
| POST /auth/login | Implementado | Adaptación pendiente | Mark / Ame |
| POST /auth/logout | Implementado | Adaptación pendiente | Mark / Ame |
| GET /users/me | Implementado | Adaptación pendiente | Mark / Ame |
| PATCH /users/me | Implementado | Adaptación pendiente | Mark / Ame |
| GET /routes | Implementado | Adaptación pendiente | Mark / Ame |
| GET /routes/{id} | Implementado | Adaptación pendiente | Mark / Ame |

Todos los endpoints utilizan el prefijo `/api/v1`.

El estado Android debe confirmarse con Ame después de
revisar su implementación actual de Retrofit y Room.

## 3. Responsabilidades

| Actividad | Responsable | Estado |
|---|---|---|
| Actualizar OpenAPI | Mark | En preparación |
| Documentar ejemplos JSON | Mark | En preparación |
| Revisar DTO Android | Ame | Pendiente de confirmación |
| Adaptar ApiService.kt | Ame | Pendiente |
| Configurar Retrofit | Ame | Pendiente |
| Implementar manejo JWT en Android | Ame | Pendiente |
| Revisar seguridad del Backend | Mark | En seguimiento |
| Proponer hosting HTTPS | Mark | Pendiente |
| Aprobar contrato | Mark y Ame | Pendiente |
| Revisar PR | Equipo | Pendiente |
| Pruebas entre dispositivos | Mark y Ame | Pendiente |

## 4. Diferencias identificadas

### Autenticación

El Backend devuelve `accessToken`, `tokenType`,
`expiresIn` y `user`.

Android debe adaptar sus modelos a estos nombres.

### Identificadores

El Backend utiliza ObjectId de MongoDB como String.
Android debe diferenciar identificadores remotos y
claves locales Long de Room.

### Consulta de rutas

El Backend devuelve un objeto paginado con:

- items
- page
- pageSize
- total
- totalPages

Android debe procesar la paginación correctamente.

### Perfil

El Backend permite actualizar `displayName` y `phone`.

No permite editar el rol o el correo electrónico
desde PATCH /users/me.

### Modo DEMO

Las cuentas y aprobaciones locales no representan
autorizaciones reales del servidor.

Las funciones DEMO deben mantenerse separadas del
ambiente de integración.

## 5. Dependencias y bloqueos

| Dependencia | Impacto | Acción |
|---|---|---|
| Contrato sin aprobación conjunta | Riesgo de incompatibilidades | Revisión de Mark y Ame |
| Backend HTTPS no desplegado | Impide pruebas remotas reales | Proponer hosting y obtener autorización |
| MongoDB de pruebas pendiente | Sin persistencia remota verificada | Preparar base aislada |
| Retrofit pendiente de adaptación | Android no consume los siete endpoints | Trabajo de Ame |
| Reservas sin API | No hay reservas reales entre dispositivos | Implementar durante Fase B |
| Conductores sin autorización del servidor | Estado DEMO no es válido en producción | Implementar durante Fase B |
| Abordajes sin API | No se pueden validar códigos reales | Implementar durante Fase B |

## 6. Orden de trabajo y PR

### PR A1 — Contrato de integración
Responsable: Mark.

Incluye:
- Contrato Backend–Android.
- OpenAPI actualizado.
- Matriz de integración.
- Documentación de compatibilidad.

No modifica la lógica de los endpoints.

### PR A2 — Integración Android
Responsable: Ame.

Depende del contrato aprobado.

Incluye:
- Modelos DTO compatibles.
- Retrofit configurado.
- Gestión segura de JWT.
- Manejo de errores HTTP y de conectividad.
- Separación DEMO y servicios reales.

### PR A3 — Configuración Backend de pruebas
Responsable: Mark.

Requiere aprobación de infraestructura y presupuesto.

Incluye:
- Entorno HTTPS.
- MongoDB de pruebas.
- Secretos externos al repositorio.
- Configuración de ambiente.
- Pruebas de disponibilidad y seguridad.

No implica despliegue productivo.

## 7. Criterios de aceptación

- [ ] Contrato revisado y aprobado por Mark y Ame.
- [ ] OpenAPI actualizado y validado.
- [ ] PR de documentación revisado.
- [ ] URL HTTPS autorizada y funcional.
- [ ] Registro conectado al Backend.
- [ ] Login real con JWT.
- [ ] Logout con revocación remota.
- [ ] Consulta y actualización de perfil real.
- [ ] Consulta y detalle de rutas del servidor.
- [ ] Identificadores ObjectId tratados correctamente.
- [ ] Paginación implementada en Android.
- [ ] Manejo de errores 401, 429 y de conectividad.
- [ ] Pruebas con dos usuarios y dos dispositivos.
- [ ] Evidencia de CI y pruebas funcionales.

## 8. Funcionalidades posteriores

Las siguientes operaciones pertenecen a la Fase B:

- Gestión de viajes.
- Registro y aprobación de conductores.
- Gestión de vehículos y documentos.
- Reservas y cancelaciones.
- Control atómico de lugares disponibles.
- Abordajes de un solo uso.
- Eliminación remota de cuentas.

Estas operaciones requieren contratos, autorización,
implementación y pruebas antes de integrarse a Android.

## 9. Reglas de trabajo

- Trabajar en ramas independientes.
- No modificar directamente main.
- Utilizar PR revisables.
- Mantener el Backend como fuente de verdad.
- No almacenar secretos en Git.
- No desplegar producción sin autorización.
- Mantener separados los entornos DEMO, pruebas y release.

## 10. Referencias

- docs/CONTRATO_INTEGRACION_ANDROID.md
- docs/COMPATIBILIDAD_ANDROID.md
- specs/001-unit2-api/contracts/openapi.yaml
- PR #5 del Backend

**Estado:** documento propuesto para revisión del equipo.
