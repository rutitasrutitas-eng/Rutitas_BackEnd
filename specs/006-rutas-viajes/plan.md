# 006 — Rutas y viajes | Plan de diseño

**Estado:** Propuesto; no ejecutar implementación ni Azure antes de aprobación.  
**Referencia de código:** `Rutitas.Api/Controllers/RoutesController.cs`, `Rutitas.Domain/Entities/Route.cs`, `Rutitas.Infrastructure/Repositories/MongoRouteRepository.cs` dentro de `Backend/Rutitas.Api/`.  
**Fuente de producto:** `002-catalogo-funcional/inventario-funcional.md`.

## 1. Plan por decisiones

1. **Congelar línea base:** conservar contrato GET `/api/v1/routes` y GET `/api/v1/routes/{id}`, ejemplos de respuestas, filtros y pruebas actuales.
2. **Aprobar distinción de dominio:** ruta de catálogo vs oferta de viaje vs ejecución del viaje; decidir nombres y compatibilidad con Android.
3. **Trazar E/N:** mapear E16–E21, N04, N09, N17 y E31–E33, señalando duplicados o dependencias en reservas.
4. **Validar precondiciones:** tomar del 003/004/005 los criterios aprobados para identidad, comunidad y conductor/vehículo.
5. **Definir política de tiempo y capacidad:** husos horarios, restricciones de asientos, recurrencia, cancelación y edición con solicitudes activas.
6. **Modelar colecciones MongoDB:** conservar `routes` existente; evaluar `ride_offers` y registro/auditoría, identificadores y concurrencia, sin migraciones destructivas.
7. **Diseñar DTO y OpenAPI propuesto:** acordar URI canónicas bajo `/api/v1`, campos, filtros, códigos 200/201/400/401/403/404/409/429 y ProblemDetails; no alterar los siete contratos existentes sin versionar y aprobar.
8. **Acordar interfaz Android:** `accessToken` en auth, `items` en listado, ObjectId como `String`, tiempo ISO-8601 y distinción entre catálogo y viaje publicado.
9. **Planificar pruebas:** unitarias, integración con Testcontainers, contrato, seguridad y concurrencia; revisar presupuesto/retención de datos.
10. **Revisión y autorización:** registrar decisión de arquitectura, firma de responsables, cambios de especificación y tareas listas para ejecutar en otra fase.

## 2. Propuesta de diseño lógico (sin código)

- `Route` existente: catálogo de trayectos reutilizables; mantener estructura y comportamiento público actual.
- `RideOffer` candidata: `routeId` opcional, `driverId`, `vehicleId`, `departureAtUtc`, `timeZoneId`, capacidad ofertada, estado, versión y ubicación autorizada.
- `TripEvent` candidato: eventos auditables de salida, fin, cancelación y cambios de estado; evitar publicar tracking personal continuo sin análisis de privacidad.
- Índices por visibilidad, fecha de salida, propietario y estado; validar utilidad y plan de consultas antes de crear índices definitivos.

## 3. Riesgos y respuesta prevista

| Riesgo | Medida |
|---|---|
| Confundir ruta de catálogo con viaje fechado | Usar entidades y DTO diferentes; no reciclar `Route` como reserva. |
| Cupos inconsistentes por concurrencia | Operaciones atómicas y pruebas de concurrencia en módulo de reservas. |
| Publicación no autorizada | Política de conductor aprobado en servidor; negativa por defecto. |
| Filtración de localizaciones/identidad | Definir precisión pública, consentimiento y retención. |
| Android consume esquemas distintos | Contrato compartido y pruebas de compatibilidad. |
| Romper los siete endpoints existentes | Mantener GET actuales, pruebas de regresión y contrato versionado. |

## 4. Criterios para pasar a implementación

- Decisiones funcionales y de seguridad aprobadas documentalmente.
- Esquema y OpenAPI revisados por Backend y Móvil.
- Dependencias de módulos 003–005 suficientes y autorizadas.
- Pruebas planeadas y responsabilidades asignadas.

**No incluye IaC ni despliegue:** Azure tiene su propio módulo SDD posterior.
