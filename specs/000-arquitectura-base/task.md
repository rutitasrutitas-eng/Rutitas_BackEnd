# Tareas futuras — Arquitectura base de Backend

> Todas son propuestas futuras. **No ejecutar en esta entrega.** El nombre singular `task.md` se usa por instrucción académica.

| ID | Descripción | Dependencias | Prioridad | Entregable esperado | Validación futura (no ejecutada) | Decisión | Ejecución | Verificación | Tipo |
|---|---|---|---|---|---|---|---|---|---|
| BE-AB-01 | Confirmar alcance arquitectónico, fuentes vigentes, restricciones y repositorio | — | P0 | Registro de alcance y restricciones | Revisión humana y fuentes enlazadas | propuesto | no_iniciado | no_ejecutada | — |
| BE-AB-02 | Evaluar y registrar lenguaje, runtime, framework y topología proporcional, sin asumir proveedores | BE-AB-01 | P0 | ADR comparativo aprobado | Justificación contra requisitos, soporte, capacidad y costo | propuesto | no_iniciado | no_ejecutada | — |
| BE-AB-03 | Definir límites de responsabilidades y dirección de dependencias del scaffold | BE-AB-02 | P1 | Estructura base aprobada | Revisión de límites y ausencia de ciclos/acoplamiento indebido | propuesto | no_iniciado | no_ejecutada | — |
| BE-AB-04 | Preparar configuración por entorno, validación de entrada, manejo de errores, logging, manejo de secretos y políticas de autorización del servidor según decisiones | BE-AB-03 | P1 | Convenciones y plantilla de configuración/validación/autorización | Revisión de valores sensibles, validación de entrada, redacción, entornos y políticas de autorización | propuesto | no_iniciado | no_ejecutada | — |
| BE-AB-05 | Definir puertos de persistencia y límites de adaptadores sin modelar dominio de producto | BE-AB-03 | P1 | Interfaces y decisiones documentadas | Inspección de dependencias y ausencia de esquema funcional | propuesto | no_iniciado | no_ejecutada | — |
| BE-AB-06 | Establecer tipos de pruebas arquitectónicas y controles de calidad según stack aprobado | BE-AB-03, BE-AB-04, BE-AB-05 | P1 | Plan de pruebas y calidad | Revisión de cobertura de riesgos y comandos reproducibles | propuesto | no_iniciado | no_ejecutada | — |
| BE-AB-07 | Revisar arquitectura base, documentación, criterios de aceptación, políticas de autorización y acuerdo de integración propuesto con Movil | BE-AB-06, BACKEND-D06 | P0 | Acta/revisión, versión identificable, registro del acuerdo y verificación documental de autorización | Evidencia de revisión humana, verificación de autorización, acuerdo de integración revisado y pendientes trazables | propuesto | no_iniciado | no_ejecutada | — |

**Estado del paquete:** decisión=`propuesto`, ejecución=`no_iniciado`, verificación=`no_ejecutada`, tipo=`—`. Las siete tareas continúan sin ejecutar; ninguna comprobación futura se afirma ejecutada. `BE-AB-01` y `BE-AB-02` son habilitadoras; la prioridad expresa criticidad y la secuencia la determinan las dependencias. Las dependencias con ID `BACKEND-Dxx` no están aprobadas y bloquean la tarea hasta su resolución.
