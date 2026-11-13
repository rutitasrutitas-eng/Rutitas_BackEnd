# Inventario consolidado — Rutitas Backend
**Fecha:** 11/11/2026  | **Estado:** inventario para revisión, no contrato aprobado ni implementación.
**Fuentes:** `Rutitas_Actividad_4.xlsx` (hojas `01 Catálogo`, E01–E52, N01–N20) y código/contrato de `001-unit2-api`. Las rutas `/v1/...` se preservan **literalmente como propuestas del Excel**; el Backend real usa `/api/v1/...`. No deben copiarse a controladores sin aprobar un contrato canónico.
**Cobertura de inventario:** 52 operaciones E + 20 N = 72 fichas; N14 no es endpoint REST. El total de fichas **no equivale** al total de endpoints únicos por crear. El Backend actual tiene 7 operaciones REST, incluyendo logout, que no está listado en el catálogo E/N.
## Contrato actualmente implementado (referencia)
- `POST /api/v1/auth/register` — implementado localmente en `001-unit2-api`; **no** reespecificar como endpoint nuevo.
- `POST /api/v1/auth/login` — implementado localmente en `001-unit2-api`; **no** reespecificar como endpoint nuevo.
- `POST /api/v1/auth/logout` — implementado localmente en `001-unit2-api`; **no** reespecificar como endpoint nuevo.
- `GET /api/v1/users/me` — implementado localmente en `001-unit2-api`; **no** reespecificar como endpoint nuevo.
- `PATCH /api/v1/users/me` — implementado localmente en `001-unit2-api`; **no** reespecificar como endpoint nuevo.
- `GET /api/v1/routes` — implementado localmente en `001-unit2-api`; **no** reespecificar como endpoint nuevo.
- `GET /api/v1/routes/{id}` — implementado localmente en `001-unit2-api`; **no** reespecificar como endpoint nuevo.

## Matriz por módulo

### Identidad y perfil

| ID | Función | Método y ruta propuesta del Excel | Clasificación preliminar |
|---|---|---|---|
| E01 | Verificar correo | `POST /v1/auth/verify-email` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E02 | Consultar mi perfil | `GET /v1/me` | Equivale funcionalmente a un endpoint existente: `GET /api/v1/users/me` (ruta del Excel distinta o sin prefijo). |
| E03 | Editar mi perfil | `PATCH /v1/me` | Equivale funcionalmente a un endpoint existente: `PATCH /api/v1/users/me` (ruta del Excel distinta o sin prefijo). |
| E04 | Eliminar mi cuenta | `DELETE /v1/me` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| N01 | Iniciar sesión | `POST /v1/auth/login` | Equivale funcionalmente a un endpoint existente: `POST /api/v1/auth/login` (ruta del Excel distinta o sin prefijo). |
| N02 | Recuperación de acceso | `POST /v1/auth/password-reset` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| N03 | Crear cuenta | `POST /v1/auth/register` | Equivale funcionalmente a un endpoint existente: `POST /api/v1/auth/register` (ruta del Excel distinta o sin prefijo). |

### Comunidades

| ID | Función | Método y ruta propuesta del Excel | Clasificación preliminar |
|---|---|---|---|
| E05 | Listar comunidades | `GET /v1/communities` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E06 | Vincular comunidad | `POST /v1/me/memberships` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E07 | Mis membresías | `GET /v1/me/memberships` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |

### Conductores y documentos

| ID | Función | Método y ruta propuesta del Excel | Clasificación preliminar |
|---|---|---|---|
| E08 | Solicitar ser conductor | `POST /v1/drivers/applications` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E09 | Consultar mi solicitud | `GET /v1/drivers/applications/me` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E10 | Preparar documento | `POST /v1/drivers/documents/upload-intent` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E50 | Admin aprobar conductor | `POST /v1/admin/drivers/applications/{id}/review` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| N15 | Finalizar expediente de | `POST /v1/drivers/applications/{id}/submit` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| N16 | Resumen del conductor | `GET /v1/drivers/dashboard` | Revisar duplicidad/alcance: Resumen derivado; evaluar necesidad real de endpoint |

### Vehículos

| ID | Función | Método y ruta propuesta del Excel | Clasificación preliminar |
|---|---|---|---|
| E11 | Registrar vehículo | `POST /v1/vehicles` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E12 | Mis vehículos | `GET /v1/vehicles` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E13 | Editar vehículo | `PATCH /v1/vehicles/{id}` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E14 | Desactivar vehículo | `DELETE /v1/vehicles/{id}` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |

### Rutas, lugares y preferencias

| ID | Función | Método y ruta propuesta del Excel | Clasificación preliminar |
|---|---|---|---|
| E15 | Autocompletar lugares | `GET /v1/places/suggest` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E16 | Buscar rutas | `GET /v1/routes` | Equivale funcionalmente a un endpoint existente: `GET /api/v1/routes` (ruta del Excel distinta o sin prefijo). |
| E17 | Detalle de ruta | `GET /v1/routes/{id}` | Equivale funcionalmente a un endpoint existente: `GET /api/v1/routes/{id}` (ruta del Excel distinta o sin prefijo). |
| E18 | Publicar ruta | `POST /v1/routes` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E19 | Editar ruta | `PATCH /v1/routes/{id}` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E20 | Cancelar ruta | `POST /v1/routes/{id}/cancel` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E21 | Mis rutas publicadas | `GET /v1/me/routes` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E22 | Crear recurrencia | `POST /v1/routes/{id}/recurrences` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| N04 | Rutas cercanas y sugerid | `GET /v1/routes/nearby` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| N05 | Guardar zona de salida | `PATCH /v1/me/travel-preferences` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| N06 | Consultar o actualizar h | `PUT /v1/me/schedule` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| N09 | Consultar paradas de rut | `GET /v1/routes/{routeId}/stops` | Revisar duplicidad/alcance: Posible información incluida en detalle de ruta E17; decidir contrato |
| N17 | Filtrar mis rutas | `GET /v1/drivers/me/routes` | Revisar duplicidad/alcance: Posible superposición con E21; decidir una ruta canónica |

### Reservas, solicitudes y espera

| ID | Función | Método y ruta propuesta del Excel | Clasificación preliminar |
|---|---|---|---|
| E23 | Solicitar asiento | `POST /v1/routes/{id}/requests` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E24 | Mis solicitudes | `GET /v1/me/requests` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E25 | Solicitudes de mi ruta | `GET /v1/routes/{id}/requests` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E26 | Aceptar solicitud | `POST /v1/requests/{id}/accept` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E27 | Rechazar solicitud | `POST /v1/requests/{id}/reject` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E28 | Cancelar reserva | `POST /v1/bookings/{id}/cancel` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E29 | Mis reservas | `GET /v1/me/bookings` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E30 | Lista de espera | `POST /v1/routes/{id}/waitlist` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| N10 | Cotizar reserva | `POST /v1/routes/{routeId}/booking-quote` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| N11 | Consultar comprobante de | `GET /v1/reservations/{reservationId}/receipt` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |

### Viajes y abordaje

| ID | Función | Método y ruta propuesta del Excel | Clasificación preliminar |
|---|---|---|---|
| E31 | Iniciar viaje | `POST /v1/trips/{id}/start` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E32 | Finalizar viaje | `POST /v1/trips/{id}/finish` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E33 | Historial de viajes | `GET /v1/me/trips` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| N12 | Estado del trayecto en v | `GET /v1/trips/{tripId}/live-status` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| N13 | Registrar abordaje | `POST /v1/trips/{tripId}/boardings` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |

### Mensajes y notificaciones

| ID | Función | Método y ruta propuesta del Excel | Clasificación preliminar |
|---|---|---|---|
| E34 | Enviar mensaje | `POST /v1/trips/{id}/messages` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E35 | Leer chat de viaje | `GET /v1/trips/{id}/messages` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E36 | Notificaciones | `GET /v1/me/notifications` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E37 | Marcar aviso leído | `PATCH /v1/me/notifications/{id}` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |

### Favoritos y valoraciones

| ID | Función | Método y ruta propuesta del Excel | Clasificación preliminar |
|---|---|---|---|
| E38 | Guardar favorito | `POST /v1/me/favorites` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E39 | Mis favoritos | `GET /v1/me/favorites` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E40 | Quitar favorito | `DELETE /v1/me/favorites/{routeId}` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E41 | Valorar viaje | `POST /v1/trips/{id}/ratings` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |

### Seguridad, soporte y consentimientos

| ID | Función | Método y ruta propuesta del Excel | Clasificación preliminar |
|---|---|---|---|
| E42 | Reportar incidente | `POST /v1/reports` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E43 | Mis reportes | `GET /v1/me/reports` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E44 | Bloquear usuario | `POST /v1/me/blocks` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E45 | Quitar bloqueo | `DELETE /v1/me/blocks/{id}` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E46 | Contactos de confianza | `POST /v1/me/trusted-contacts` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E47 | Crear ticket soporte | `POST /v1/support/tickets` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E48 | Mis tickets | `GET /v1/support/tickets` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E51 | Admin revisar reportes | `GET /v1/admin/reports` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| E52 | Admin resolver reporte | `PATCH /v1/admin/reports/{id}` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| N07 | Consultar aviso de priva | `GET /v1/legal/privacy-notice` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| N08 | Aceptar aviso de privaci | `POST /v1/me/consents` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |
| N18 | Confirmar incidencia del | `POST /v1/trips/{tripId}/incidents` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |

### Administración de comunidades

| ID | Función | Método y ruta propuesta del Excel | Clasificación preliminar |
|---|---|---|---|
| E49 | Admin comunidades | `POST /v1/admin/communities` | Propuesto; sin implementación acreditada en los 7 endpoints actuales. |

### Aportaciones informativas

| ID | Función | Método y ruta propuesta del Excel | Clasificación preliminar |
|---|---|---|---|
| N19 | Resumen de aportaciones | `GET /v1/drivers/me/contributions/summary` | Revisar duplicidad/alcance: Resumen de aportaciones; validar alcance económico |
| N20 | Movimientos de aportacio | `GET /v1/drivers/me/contributions` | Revisar duplicidad/alcance: Movimientos de aportaciones; validar alcance económico |

### Solo Android

| ID | Función | Método y ruta propuesta del Excel | Clasificación preliminar |
|---|---|---|---|
| N14 | Permisos nativos Android | `LOCAL Capacitor/Android (sin endpoint)` | Función local Android. No crear endpoint. |

## Decisiones imprescindibles antes de crear specs por módulo

1. Confirmar con líder, Backend y Móvil si el catálogo E01–E52 + N01–N20 es el alcance oficial o una propuesta de diseño.
2. Decidir si `routes` representa catálogo de trayectos o publicación de viajes fechados: la reserva necesita conductor, vehículo, fecha, cupos y estados; no se infieren del modelo actual.
3. Fijar prefijo canónico `/api/v1`, DTO JSON, seguridad y errores; coordinar migración de rutas propuestas `/v1/me` y `/v1/...` sin duplicar controladores.
4. Definir identidad/verificación de comunidad y aprobación de conductor **en servidor** antes de publicar viajes; permisos del dispositivo no conceden privilegios.
5. Definir privacidad, retención, borrado/anominización, consentimiento, acceso a documentos y ubicación; no prometer seguimiento ni cifrado E2E no implementado.
6. Decidir reglas de concurrencia de asientos, cancelación, lista de espera y transiciones de viaje; transacciones/actualizaciones atómicas en MongoDB.
7. Definir el alcance de aportaciones: solo información registrada, no pagos ni liquidación sin servicios y aprobaciones correspondientes.
8. Aprobar dependencias, prioridades y responsables antes de diseñar IaC en Azure y el pipeline CI/CD.
