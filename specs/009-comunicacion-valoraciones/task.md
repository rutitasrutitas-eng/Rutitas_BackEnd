# 009 — Tareas SDD | Comunicación, notificaciones, favoritos y valoraciones

**Estado global:** PROPUESTO · NO EJECUTAR antes de la aprobación del equipo.  
**Fuentes:** E34–E41 del inventario funcional 002.  
**Responsables:** roles propuestos, no designaciones ni aprobaciones confirmadas.

| ID | Tarea documental | Responsable propuesto | Estado | Dependencia / evidencia requerida |
|---|---|---|---|---|
| CV-T01 | Conciliar E34–E41 con inventario 002 y pantallas Android | Producto + Móvil | propuesto | Inventario y mapa de pantallas |
| CV-T02 | Definir acceso a chat según estado de viaje, reservas y bloqueos | Backend + Seguridad | propuesto | 006, 007, 008 |
| CV-T03 | Definir formato, tamaño, frecuencia y moderación de mensajes | Producto + Seguridad | propuesto | CV-T02 |
| CV-T04 | Definir eventos y tipos de notificaciones, lectura y deduplicación | Backend + Móvil | propuesto | 006, 007, CV-T01 |
| CV-T05 | Resolver si habrá push y protocolo de tokens/consentimiento | Producto + Móvil + Seguridad | propuesto | CV-T04 |
| CV-T06 | Confirmar si favorito referencia ruta de catálogo o viaje publicado | Producto + Backend | propuesto | 006, CV-T01 |
| CV-T07 | Acordar escala, elegibilidad, visibilidad y moderación de valoraciones | Producto + Seguridad | propuesto | 006, 007, 008 |
| CV-T08 | Diseñar entidades MongoDB, índices, retención y concurrencia | Backend + Seguridad | propuesto | CV-T02–CV-T07 |
| CV-T09 | Acordar contratos `/api/v1`, DTO y respuestas HTTP con Android | Backend + Móvil | propuesto | CV-T08 |
| CV-T10 | Especificar pruebas unitarias, Testcontainers, contratos, IDOR y spam | Backend + Seguridad | propuesto | CV-T09 |
| CV-T11 | Registrar decisiones, riesgos y matriz de trazabilidad | Lead + Producto | propuesto | CV-T01–CV-T10 |
| CV-T12 | Solicitar aprobación formal para implementación futura | Lead/Release | bloqueada | CV-T11; aprobaciones verificables |

## Criterios de cierre documental

- E34–E41 trazadas sin duplicar funcionalidades de 006–008.
- El equipo acuerda acceso al chat, límites y política de privacidad.
- Se define si notificaciones son internas, push o ambas; no presumir Firebase configurado.
- Favoritos y valoraciones tienen reglas de negocio y propiedad claras.
- El equipo Móvil acepta contrato, IDs, cuerpos, errores y paginación.
- Existe plan de pruebas de seguridad, errores, bloqueos, duplicados y concurrencia.
- Las aprobaciones están registradas; **añadir una carpeta a `specs` no equivale a aprobar la implementación**.

**Referencia:** los 63 tests locales validados corresponden exclusivamente a las funciones implementadas antes de este módulo.
