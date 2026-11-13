# 010 — Tareas SDD | Soporte, privacidad y administración transversal

**Estado global:** PROPUESTO · NO INICIADO · NO EJECUTADO.  
**Regla:** este listado es planificación; no es evidencia de aprobación o código.

| Tarea | Descripción | Dependencia | Estado |
|---|---|---|---|
| SPA-T01 | Validar E46–E48/N07–N08 contra el catálogo 002 y distinguir E49/E50/E51/E52 | 002, 004, 005, 008 | Propuesta |
| SPA-T02 | Determinar finalidades, datos mínimos y responsables de contactos de confianza | SPA-T01, privacidad | Propuesta |
| SPA-T03 | Definir acceso, alcance, límites y consentimiento para terceros | SPA-T02 | Propuesta |
| SPA-T04 | Especificar creación, propiedad, paginación y estados de tickets | SPA-T01 | Propuesta |
| SPA-T05 | Especificar contenido/versionado del aviso y registro de consentimiento | SPA-T01, privacidad | Propuesta |
| SPA-T06 | Acordar rutas, DTO, estados HTTP, JWT y manejo de errores con Móvil | SPA-T03–T05 | Propuesta |
| SPA-T07 | Diseñar entidades, índices, auditoría, borrado y retención de MongoDB | SPA-T06 | Propuesta |
| SPA-T08 | Preparar matriz de pruebas unitarias, integración, contratos y seguridad | SPA-T06–T07 | Propuesta |
| SPA-T09 | Revisar amenaza IDOR, filtración de datos, spam y abuso | SPA-T07–T08 | Propuesta |
| SPA-T10 | Solicitar aprobación a liderazgo, Backend, Móvil y Seguridad/Privacidad | SPA-T01–T09 | Pendiente de autorización |
| SPA-T11 | Tras la aprobación, crear OpenAPI y tareas de implementación en PR separado | SPA-T10 | Bloqueada |

## Criterios de cierre documental

- Todas las operaciones cubiertas sin duplicar módulos vecinos.
- Campos, permisos y límites con decisiones trazables, sin suponerlas aprobadas.
- Revisión del aviso/versionado y tratamientos de datos de terceros.
- Definición verificable de pruebas de contrato y seguridad.
- Aprobación registrada por responsables antes de código o despliegue.

**No cambiar los siete endpoints actuales.** Mantener intactas las 63 pruebas existentes y repetirlas si en el futuro se aprueban cambios funcionales.
