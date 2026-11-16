# 006 — Rutas y viajes | Tareas propuestas

**Estado global:** `propuesto` · **Ejecución:** `no_iniciada` · **Aprobación:** `pendiente`.  
**Nota:** cada tarea siguiente es de análisis/diseño; ninguna autoriza implementar ni desplegar.

| ID | Tarea | Dependencia | Evidencia de cierre | Estado |
|---|---|---|---|---|
| RV-T01 | Verificar línea base GET routes y detalle | 001 | Inventario de rutas/DTO existentes | Pendiente |
| RV-T02 | Aprobar distinción catálogo/oferta/viaje | RV-T01 | Decisión arquitectónica firmada | Pendiente |
| RV-T03 | Reconciliar E16–E21, N04/N09/N17 y E31–E33 | RV-T02 | Matriz sin duplicidades | Pendiente |
| RV-T04 | Acordar habilitación de conductor/vehículo | 003–005 | Reglas autorizadas en servidor | Pendiente |
| RV-T05 | Definir estados, horarios, asientos y cancelación | RV-T02, RV-T04 | Tabla de reglas y transiciones | Pendiente |
| RV-T06 | Proponer esquema MongoDB no destructivo | RV-T05 | Modelo de colecciones e índices | Pendiente |
| RV-T07 | Proponer contratos OpenAPI versionados | RV-T03, RV-T06 | Esquemas/ejemplos en revisión | Pendiente |
| RV-T08 | Revisar compatibilidad con Móvil | RV-T07 | Conformidad de campos, IDs y errores | Pendiente |
| RV-T09 | Definir pruebas y amenazas | RV-T05, RV-T07 | Matriz de pruebas/seguridad | Pendiente |
| RV-T10 | Obtener autorizaciones de equipo | RV-T01–T09 | Acta/gate formal | Pendiente |

## Lista de verificación previa a la aprobación

- [ ] Se mantiene el comportamiento de los dos GET implementados.
- [ ] Se decide nombre canónico de las publicaciones (`rides` frente a `routes`).
- [ ] Se evita duplicar E21 y N17; se resuelve N09.
- [ ] Están claros autorizaciones, propiedad y estado de conductor.
- [ ] Se definen UTC, zona horaria, cupos y cancelaciones.
- [ ] Android y Backend revisan los mismos contratos.
- [ ] Seguridad/Privacidad valida datos de ubicación y auditoría.
- [ ] Responsable aprueba el cambio de estado documental.

**Prohibido marcar RV-T01–RV-T10 como implementadas solo por crear estos documentos.**
