# 005 — Conductores y documentos | Tareas

**Estado general:** Propuesto  
**Ejecución:** No iniciada  
**Verificación:** No ejecutada  
**Condición:** Requiere revisión documental y aprobación antes de implementar.

## Tareas de especificación y diseño

| ID | Tarea | Dependencia | Estado |
|---|---|---|---|
| COND-T01 | Confirmar correspondencia con el catálogo E/N y prototipo Móvil. | 002 y revisión del equipo | propuesta |
| COND-T02 | Aprobar criterios de elegibilidad, verificación y roles. | COND-T01, módulo 004 | propuesta |
| COND-T03 | Definir documentos estrictamente necesarios, privacidad y retención. | COND-T02, Seguridad/Privacidad | propuesta |
| COND-T04 | Revisar estados/transiciones de solicitud y vehículo. | COND-T02 | propuesta |
| COND-T05 | Diseñar modelos de datos y auditoría. | COND-T03, COND-T04 | propuesta |
| COND-T06 | Definir endpoints y esquemas JSON candidatos. | COND-T04, COND-T05 | propuesta |
| COND-T07 | Revisar autorización, límites y validación de archivos. | COND-T03, COND-T06 | propuesta |
| COND-T08 | Acordar contrato y mensajes con el equipo Móvil. | COND-T06 | propuesta |
| COND-T09 | Elaborar matriz de pruebas y criterios de aceptación. | COND-T06, COND-T07 | propuesta |
| COND-T10 | Registrar aprobación o correcciones formales de la especificación. | COND-T01 a COND-T09 | propuesta |

## Tareas futuras condicionadas (NO AUTORIZADAS)

| ID | Posible tarea posterior | Puerta de entrada | Estado |
|---|---|---|---|
| COND-I01 | Implementar entidades, repositorios y migración/índices necesarios. | COND-T10 aprobado | no_iniciada |
| COND-I02 | Implementar endpoints y autorización en servidor. | COND-I01 y contrato aprobado | no_iniciada |
| COND-I03 | Implementar almacenamiento documental privado y controles de seguridad. | Política de privacidad aprobada | no_iniciada |
| COND-I04 | Integrar flujos de Móvil. | API funcional en entorno aprobado | no_iniciada |
| COND-I05 | Ejecutar pruebas automatizadas y validación de seguridad. | Implementación completa | no_ejecutada |

## Reglas de cierre

- Ninguna tarea propuesta equivale a implementación.
- No cambiar el estado sin evidencia o autorización del responsable.
- Antes de desarrollar, actualizar el OpenAPI compartido y las matrices de trazabilidad, y obtener aceptación de Móvil.
- Mantener funcionales los siete endpoints existentes y las pruebas base.
