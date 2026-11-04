# Decisiones pendientes — Rutitas Backend

Todas estas decisiones siguen **sin aprobación humana**. No impiden preparar la arquitectura base documental; sí pueden bloquear especificación detallada o ejecución futura.

| ID | Pregunta pendiente | Opciones sin seleccionar | Evidencia/autoridad requerida | Recomendación no vinculante | Consecuencia si permanece abierta | Aprobador | Estado |
|---|---|---|---|---|---|---|---|
| BACKEND-D01 | ¿Se ratifican Constitución y AGENTS propios? | Ratificar, ajustar o rechazar | Versión, fecha, aprobadores y acta/registro | No asumir ratificación | Bloquea obligatoriedad normativa | PENDIENTE_DE_DESIGNAR | propuesto |
| BACKEND-D02 | ¿Cuál es el repositorio canónico? | Seleccionar ruta y flujo aprobados | Ruta, propietarios, rama, flujo de revisión | No fijar repositorio por inferencia | Bloquea ejecución futura | PENDIENTE_DE_DESIGNAR | propuesto |
| BACKEND-D03 | ¿Quién cubre arquitectura, seguridad y release? | Designar roles humanos | Asignación humana expresa | No asignar roles a agentes | Bloquea aprobaciones y aceptación | PENDIENTE_DE_DESIGNAR | propuesto |
| BACKEND-D04 | ¿Qué restricciones no funcionales aplican? | Definir capacidad, privacidad y presupuesto | Necesidades y fuente identificable | Esperar alcance confirmado | Limita diseño proporcional | PENDIENTE_DE_DESIGNAR | propuesto |
| BACKEND-D05 | ¿Qué stack y herramientas se aprueban? | Comparar alternativas pertinentes | Comparativa y decisión aprobada | No adoptar ejemplos de clase | Bloquea scaffold y controles técnicos | PENDIENTE_DE_DESIGNAR | propuesto |
| BACKEND-D06 | ¿Qué límites rigen la integración? | Acordar contrato documental | Acuerdo entre equipos | No definir endpoints | Bloquea contratos específicos | PENDIENTE_DE_DESIGNAR | propuesto |
| BACKEND-D07 | ¿Qué controles de seguridad, datos y calidad aplican? | Definir controles proporcionales | Criterios, controles y responsables | No declarar controles implementados | Bloquea validación técnica futura | PENDIENTE_DE_DESIGNAR | propuesto |
| BACKEND-D08 | ¿Se acepta `task.md` y la configuración OpenCode? | Mantener, ajustar o rechazar convención | Revisión docente y convención aprobada | Mantener tareas sin ejecutar y agentes de solo lectura | Bloquea cambio de fase | PENDIENTE_DE_DESIGNAR | propuesto |

Esta entrega **no** decide endpoints, modelos de producto, pantallas, proveedor cloud, base de datos ni operaciones funcionales.

## Registro de alternativas exploradas (2026-11-04)

El usuario comunicó dos restricciones académicas: **NoSQL obligatoria** y alojamiento limitado a **Azure, AWS o Google Cloud**. Queda pendiente incorporar la fuente primaria/fecha docente. El equipo prefiere **ASP.NET Core 10 con C#** y **Google Cloud Run + Firestore**, con Maps y posible Firebase Authentication. Esta preferencia **no** modifica el estado de `BACKEND-D05`, que sigue `propuesto` y sin aprobador designado.

- [Razonamiento tecnológico, costos, permisos y datos](propuestas-futuras/stack-backend-y-cloud.md).
- [Siete endpoints exploratorios condicionados al proveedor de identidad](propuestas-futuras/inventario-7-endpoints.md).
- `BACKEND-D06` continúa abierto: no existe contrato Móvil ↔ Backend ratificado.
