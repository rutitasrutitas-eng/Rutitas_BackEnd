# 007 — Solicitudes y reservas | Plan de diseño

**Estado:** Propuesto, no aprobado para implementar.  
**Dependencias obligatorias:** decisiones de `006-rutas-viajes`, política de identidad/verificación de 003–005 y contrato común con Móvil.

## 1. Objetivo

Especificar el flujo de solicitudes y reservas sin sobreventa, con autorización del servidor, control de concurrencia y contratos REST compatibles con Android, evitando modificar los siete endpoints actuales.

## 2. Secuencia documental (aún sin programar)

1. **Alinear alcance con Producto/Móvil:** solicitudes vs reserva inmediata; múltiple asiento; cancelaciones; estimación económica y comprobantes.
2. **Cerrar dependencia 006:** confirmar entidad `RideOffer` y ruta API canónica `/api/v1/rides`, diferenciada de `Route` de catálogo.
3. **Modelar estados:** transiciones, propietarios, permisos, cambios sobre viaje y política de cupos.
4. **Diseñar datos MongoDB:** documentos/índices, garantía de unicidad, cálculo de asientos y opción de transacciones; evitar doble confirmación.
5. **Diseñar contratos:** DTO, ProblemDetails/errores, paginación, idempotencia, códigos HTTP y reglas de actualización.
6. **Diseñar controles DevSecOps:** casos de abuso/IDOR, análisis de dependencia, secretos, logging seguro, pruebas de carga y concurrencia.
7. **Revisar con Móvil:** mapear cada pantalla, método y estado; acordar retención de datos y mensajes de error.
8. **Solicitar aprobaciones formales** antes de desglosar implementación en tareas ejecutables o actualizar OpenAPI vigente.

## 3. Diseño propuesto (sujeto a aprobación)

- **API:** controladores separados para solicitudes y reservas con DTO y validación; sin lógica de cupos en controladores.
- **Application:** comandos `RequestSeat`, `AcceptRequest`, `RejectRequest`, `CancelBooking` y consultas autorizadas.
- **Domain:** reglas de capacidad, transiciones y propiedad; ninguna confianza en campos de identidad entregados por cliente.
- **Infrastructure:** repositorios MongoDB, índices y mecanismo atómico/transaccional documentado; reconciliación ante fallos parciales.
- **Android:** consume el contrato aprobado; interfaz no determina unilateralmente quién es conductor ni cuántos asientos hay.

## 4. Riesgos y mitigaciones previstos

| Riesgo | Mitigación de diseño |
|---|---|
| Sobreventa con peticiones simultáneas | Reserva atómica de cupo, unicidad, versión y pruebas concurrentes. |
| Doble reserva por reintentos de red | Idempotencia y transiciones de estado estrictas. |
| Usuarios accediendo a datos ajenos | Autorización en cada consulta/operación y pruebas IDOR. |
| Liberar cupo dos veces | Cancelación idempotente, límite superior de capacidad y auditoría. |
| Duplicación de conceptos Route/Ride | Exigir aprobación del modelo de 006 antes de definir rutas definitivas. |
| Costos/transacciones en MongoDB | Validar características del plan/entorno antes de elegir transacciones. |

## 5. Estrategia de verificación futura

- Contrato REST y OpenAPI de propuestas aprobado antes de código.
- Pruebas de unidad por transición y frontera de capacidad; integración Testcontainers/MongoDB; seguridad 401/403/404 e IDOR; pruebas de concurrencia real y recuperación ante fallo.
- Tras implementación autorizada: repetir 63 pruebas preexistentes + nuevas pruebas; validar Android contra staging y endpoints desplegados con HTTPS.

## 6. Entregables de este plan

`spec.md`, `plan.md`, `task.md` de 007 y decisiones de negocio/API registradas posteriormente por los responsables. Este paquete **no incluye** código, cambios en base de datos, OpenAPI publicado ni despliegue Azure.
