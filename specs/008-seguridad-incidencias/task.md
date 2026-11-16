# 008 — Tareas SDD | Seguridad e incidencias

**Estado global:** PROPUESTO. **No ejecutar** hasta revisión y aprobación formal.  
**Dependencias:** 002 catálogo, 003 identidad, 004 comunidades, 006 viajes, 007 reservas.  
**Criterio:** cada tarea debe aportar evidencia documental y una referencia trazable; ninguna implica código aún.

| ID | Tarea | Responsable propuesto | Estado | Dependencia / evidencia |
|---|---|---|---|---|
| SI-T01 | Verificar mapeo E42, E43, E44, E45, E51, E52 y N18 | Producto + Backend | propuesto | Inventario 002 |
| SI-T02 | Resolver duplicidad E42 frente a N18 | Producto + Móvil | propuesto | SI-T01 |
| SI-T03 | Definir categorías, estados, sanciones y apelación | Producto + Seguridad | propuesto | SI-T01 |
| SI-T04 | Acordar reglas de bloqueo en chat, reservas y viajes activos | Producto + Móvil | propuesto | 006/007, SI-T03 |
| SI-T05 | Establecer rol de moderación, controles RBAC y auditoría | Seguridad + Backend | propuesto | 003/004, SI-T03 |
| SI-T06 | Establecer política de datos, adjuntos, conservación y acceso | Seguridad + Producto | propuesto | SI-T03 |
| SI-T07 | Diseñar entidades MongoDB, índices y concurrencia | Backend | propuesto | SI-T02, SI-T04, SI-T06 |
| SI-T08 | Definir contratos `/api/v1`, DTO y códigos HTTP | Backend + Móvil | propuesto | SI-T02, SI-T05, SI-T07 |
| SI-T09 | Documentar pruebas de permisos, IDOR, rate limiting, duplicados y auditoría | Backend + Seguridad | propuesto | SI-T05, SI-T08 |
| SI-T10 | Registrar riesgos, responsables y aceptación documental | Lead + Seguridad | propuesto | SI-T01–SI-T09 |
| SI-T11 | Solicitar gate formal de ejecución | Lead/Release | bloqueada | SI-T10 y aprobaciones documentadas |

## Criterios de cierre documental

- Cada fuente E/N está incluida o marcada explícitamente fuera del alcance.
- El equipo ha acordado el comportamiento de bloqueos y N18, sin duplicar rutas.
- El acceso de moderadores está autorizado en servidor y definido antes de implementarse.
- Existe decisión sobre retención de evidencia y datos privados.
- Móvil y Backend aceptan nombres y semántica de contratos REST.
- Riesgos de seguridad, auditoría, casos de prueba y evidencia de aprobación se registran.

**Prohibido declarar PASS o cerrar tareas de implementación sin evidencia.** Las 63 pruebas existentes corresponden al backend previo, no a este módulo.
