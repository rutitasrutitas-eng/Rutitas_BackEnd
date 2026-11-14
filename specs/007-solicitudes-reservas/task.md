# 007 — Solicitudes y reservas | Registro de tareas SDD

**Decisión:** propuesto · **Ejecución:** no iniciada · **Verificación:** no ejecutada.  
**Regla:** las tareas siguientes son **documentales**; NO autorizan crear endpoints, desplegar Azure ni editar colecciones MongoDB.

| ID | Tarea | Dependencia | Responsable sugerido | Estado |
|---|---|---|---|---|
| SR-T01 | Revisar inventario E23–E29, N10 y N11 y evitar duplicados | 002 | Producto / Backend | Pendiente |
| SR-T02 | Confirmar `RideOffer` vs `Route` y su identificador | 006 | Backend / Móvil | Bloqueado por decisión 006 |
| SR-T03 | Acordar aceptación manual vs reserva inmediata y múltiples asientos | SR-T01 | Producto / Móvil | Pendiente |
| SR-T04 | Precisar estados, permisos y cancelaciones | SR-T02, SR-T03 | Backend / Seguridad | Pendiente |
| SR-T05 | Diseñar invariantes de cupos, idempotencia e índices | SR-T04 | Backend / Seguridad | Pendiente |
| SR-T06 | Evaluar atomicidad/operaciones multi-documento en MongoDB | SR-T05 | Backend | Pendiente |
| SR-T07 | Definir contratos de E23–E29, DTO y ProblemDetails | SR-T02–SR-T06 | Backend / Móvil | Pendiente |
| SR-T08 | Decidir alcance de cotización N10 y comprobante N11 | SR-T03 | Producto / Privacidad | Pendiente |
| SR-T09 | Preparar matriz de pruebas unitarias, integración, concurrencia y seguridad | SR-T05–SR-T08 | QA / Seguridad | Pendiente |
| SR-T10 | Obtener aprobación formal del módulo y trazabilidad con Excel | SR-T01–SR-T09 | Liderazgo / responsables | Pendiente |
| SR-T11 | Crear tareas de implementación **en fase posterior autorizada** | SR-T10 | Backend | No autorizado |

## Criterios de cierre documental

- Reglas claras para `request`/`booking`/`ride`, cancelación, cupos y acceso a datos.
- Rutas del Excel `/v1/...` reconciliadas con prefijo real `/api/v1/...`.
- Decisiones y dudas asentadas por escrito; Product Owner/Backend/Móvil/Seguridad aprueban o registran observaciones.
- Ningún campo o endpoint propuesto se presenta como ya implementado.

## No ejecutar todavía

No añadir controladores, modelos, migraciones, pruebas nuevas obligatorias, variables de Azure o pipelines de despliegue durante esta tarea documental. Las 63 pruebas actuales se conservan como línea base local.
