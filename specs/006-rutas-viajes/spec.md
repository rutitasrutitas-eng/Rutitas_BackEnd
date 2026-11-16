# 006 — Rutas y viajes | Especificación funcional

**Proyecto:** Rutitas UTTT — Backend  
**Estado:** Propuesto · revisión pendiente · implementación no autorizada para nuevas funcionalidades.  
**Dependencias:** 000–005; `002-catalogo-funcional/inventario-funcional.md`; acuerdos Backend/Móvil/Seguridad.  
**Objetivo:** Distinguir trayectos del catálogo de publicaciones reales de viajes y establecer reglas previas al desarrollo.

## 1. Situación técnica comprobada

En el Backend ya existen, bajo `/api/v1`, dos consultas de **rutas de catálogo**:

- `GET /api/v1/routes` (E16, equivalencia funcional; el Excel propone `/v1/routes`): búsqueda opcional y paginación; devuelve `{items, page, pageSize, total, totalPages}`.
- `GET /api/v1/routes/{id}` (E17, equivalencia funcional; el Excel propone `/v1/routes/{id}`): devuelve origen, destino, paradas ordenadas, distancia y duración; identificador `string` ObjectId de 24 caracteres hexadecimales.

La entidad actual `Route` tiene `Id`, `Code`, `Name`, `Description`, `Origin`, `Destination`, `Stops`, `DistanceKm`, `EstimatedDurationMin`, `Status`, `Visibility`, `CreatedAt`, `UpdatedAt`. **No contiene conductor, vehículo, fecha/hora de salida, disponibilidad ni reservas**. Por tanto, no representa por sí sola un viaje ofertado y no permite vender/reservar un asiento. Las consultas actuales filtran `Status="active"` y `Visibility="public"`.

## 2. Decisión arquitectónica pendiente (bloqueante)

Se propone separar:

1. **RouteTemplate / ruta de catálogo:** recorrido reutilizable (origen, destino, paradas y estimaciones). Mantener contratos actuales y sus pruebas.
2. **RideOffer / viaje publicado:** conductor autorizado, vehículo habilitado, ruta asociada u origen/destino, fecha y hora con zona horaria, plazas ofertadas, estado y reglas de cancelación.
3. **Trip / viaje en ejecución:** ciclo de vida de salida, progreso y finalización; un viaje publicado puede pasar a ejecución sin duplicar el concepto. Nombres definitivos por decidir.

La creación, edición o cancelación de `RideOffer` **no debe sobreescribir** documentos de rutas de catálogo existentes. Antes de desarrollar debe aprobarse si E18–E21 se exponen como `/api/v1/rides` o si el equipo mantiene semántica `/api/v1/routes`; no publicar ambas convenciones por inercia.

## 3. Requisitos funcionales propuestos

| ID | Fuente | Requisito | Criterio de aceptación propuesto |
|---|---|---|---|
| RV-01 | E16 | Listar rutas de catálogo | Mantener paginación, filtros de visibilidad y contrato compatible con lo implementado. |
| RV-02 | E17 | Consultar detalle de ruta | ID ObjectId válido; respuestas 400/404 sin revelar rutas privadas; paradas ordenadas. |
| RV-03 | E18 | Publicar viaje | Solo conductor aprobado con vehículo habilitado; hora futura, plazas válidas, ruta y datos consistentes. |
| RV-04 | E19 | Editar publicación | Solo propietario autorizado; impedir cambios incompatibles con solicitudes confirmadas o viaje iniciado. |
| RV-05 | E20 | Cancelar publicación | Cancelación idempotente, motivo, auditoría y notificación/efecto sobre solicitudes según política aprobada. |
| RV-06 | E21 | Consultar mis publicaciones | Autenticación; paginación; nunca mostrar publicaciones ajenas privadas. |
| RV-07 | N04 | Encontrar viajes cercanos/sugeridos | Filtros geográficos y temporales acotados; ubicación solo con permiso y consentimiento. |
| RV-08 | N09 | Consultar paradas | Confirmar si el detalle E17 cubre el caso y evitar endpoint duplicado. |
| RV-09 | N17 | Filtrar viajes propios | Resolver solapamiento con E21 en una sola operación canónica. |
| RV-10 | E31–E33 | Ciclo e historial de viajes | Estados, permisos, transiciones e historial se definirán en contrato independiente al cerrar este módulo. |

**Fuera de alcance actual:** aprobación de conductores (005); solicitudes y asignación atómica de asientos (módulo posterior); chat, pagos, geolocalización continua, despliegue Azure y publicación Play Store.

## 4. Ciclo de vida y reglas de dominio propuestas

- Estados de oferta candidatos: `draft`, `published`, `cancelled`, `in_progress`, `completed`; rechazar transiciones no permitidas.
- La autorización se verifica en el servidor y no se acepta `driverApproved=true` del cliente.
- Zona horaria de negocio: documentar zona del origen (México); persistir fecha/hora UTC y representar zona original cuando proceda, especialmente ante cambios de horario.
- Validar origen y destino distintos, coordenadas plausibles, asientos > 0 y límites configurables, fecha futura y vehículo vigente.
- No exponer contacto personal, documentos ni coordenadas exactas privadas en listados públicos sin política de privacidad aprobada.
- Reservas y cupos deben usar cambios atómicos/concurrencia controlada en MongoDB cuando se especifique el módulo de solicitudes.

## 5. Contratos API: existentes frente a candidatos

| Método | Ruta | Estado | Nota |
|---|---|---|---|
| GET | `/api/v1/routes` | **Implementado** | E16; paginación y `items` existentes. |
| GET | `/api/v1/routes/{id}` | **Implementado** | E17; ObjectId `string`. |
| POST | `/v1/routes` (Excel E18) | **No implementado** | Propuesta por reconciliar con `POST /api/v1/rides`. |
| PATCH | `/v1/routes/{id}` (Excel E19) | **No implementado** | Requiere decisión sobre entidad objetivo. |
| POST | `/v1/routes/{id}/cancel` (Excel E20) | **No implementado** | Requiere política de cancelación. |
| GET | `/v1/me/routes` (Excel E21) | **No implementado** | Reconciliar con N17 `/v1/drivers/me/routes`. |
| GET | `/v1/routes/nearby` (Excel N04) | **No implementado** | Acordar si devuelve rutas catálogo u ofertas de viajes. |
| GET | `/v1/routes/{routeId}/stops` (Excel N09) | **Pendiente de decisión** | Quizá ya cubierto por detalle E17. |
| POST | `/v1/trips/{id}/start` (E31) | **No implementado** | Contrato del ciclo de viaje pendiente. |
| POST | `/v1/trips/{id}/finish` (E32) | **No implementado** | Contrato del ciclo de viaje pendiente. |
| GET | `/v1/me/trips` (E33) | **No implementado** | Historial pendiente. |

**IMPORTANTE:** Todas las rutas `/v1/...` de esta tabla son texto literal del Excel, **no URLs válidas confirmadas del backend**. El prefijo implementado es `/api/v1`. Las propuestas no deben añadirse todavía al OpenAPI vigente ni a Android como endpoints activos.

## 6. Borrador de campos para una oferta (no DTO definitivo)

- `id: string` (ObjectId serializado); `routeId?: string` si se reutiliza catálogo; `driverId: string`; `vehicleId: string`.
- `departureAtUtc: datetime`; `timeZoneId: string` (definir formato aceptado); `seatsTotal: integer`; `seatsAvailable: integer` (calculado/controlado por servidor).
- `origin`, `destination`, `meetingPoint` según privacidad y permiso; `status`, `createdAtUtc`, `updatedAtUtc`, `version` para concurrencia.

Ningún valor derivado de permisos o cupos se considera fiable por venir de Android.

## 7. Seguridad, pruebas y aceptación

- JWT obligatorio en publicar/editar/cancelar/mis viajes; autorización de propiedad y habilitación de conductor comprobadas en Backend.
- Pruebas de validación, estados, paginación, aislamiento por usuario, acceso anónimo, escalación de privilegios, concurrencia y contratos JSON.
- Conservar pruebas de los siete endpoints actuales y repetir las 63 de referencia tras cambios reales.
- Acordar con Móvil nombres de campos, errores ProblemDetails, filtros, horarios y ObjectId.
- Requiere visto bueno de liderazgo/producto, Backend, Móvil y Seguridad/Privacidad antes de implementar.

## 8. Preguntas por resolver antes de aprobar

1. ¿`routes` continúa siendo el catálogo y `rides` las ofertas? Se recomienda esta separación.
2. ¿Se publican viajes recurrentes o solo viajes con fecha/hora concretas?
3. ¿Qué límites de asientos, ventana de cancelación y puntos de encuentro se permiten?
4. ¿N04 consulta rutas estáticas o viajes disponibles cercanos?
5. ¿E21 y N17 quedan en un endpoint único de publicaciones propias?
6. ¿Se permite iniciar un viaje sin pasajeros confirmados?

**Cierre:** esta especificación es propuesta documental, no evidencia de endpoints nuevos ni permiso de despliegue.

## 9. Anexo de trazabilidad explícita (revisión 11/11/2026)

Estas operaciones estaban pendientes de referencia literal en el módulo. Son **propuestas documentales**, no endpoints implementados.

| ID de origen | Función de catálogo | Relación propuesta | Decisión o prueba pendiente |
|---|---|---|---|
| E15 | Autocompletar lugares | Búsqueda/origen y destino del catálogo | Decidir proveedor de geocodificación, privacidad, costos y si el servicio corresponde a Backend o Móvil |
| E22 | Crear recurrencia | Publicación recurrente de RideOffer | Definir calendario, zona horaria, excepciones y cómo se generan ofertas concretas |
| N05 | Guardar zona de salida | Preferencias de búsqueda del usuario | Determinar si se guarda localmente o en perfil del servidor; minimizar ubicación persistida |
| N06 | Consultar o actualizar horarios | Preferencias y filtros del usuario | Distinguir horario personal del horario de un viaje; definir persistencia y validaciones |
| N12 | Estado del trayecto en vivo | Ciclo de vida de Trip | Acordar frecuencia, permisos, retención y si existe ubicación en tiempo real en el MVP |
| N13 | Registrar abordaje | Inicio y participantes del viaje | Definir verificación, rol autorizado, evidencias mínimas e idempotencia |

No cambiar los contratos operativos `GET /api/v1/routes` ni `GET /api/v1/routes/{id}` para implementar estas funciones. Cualquier propuesta de ubicación en tiempo real necesita revisión de seguridad y privacidad previa.
