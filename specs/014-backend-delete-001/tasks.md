# BACKEND-DELETE-001 — Tareas SDD

**Decisión:** `en_revision` · **Ejecución:** `no_iniciado` · **Verificación:** `no_ejecutada`; todas las tareas pendientes de ejecución/aceptación. Priorizar gates y no declarar trabajo hecho por tener la tarea escrita.

| ID | Entregable/actividad | Dependencia | Comprobación prevista |
|---|---|---|---|
| DEL-T01 | Confirmar Constitución, AGENTS, decisión autorizada y referencia E04 | Autorización del líder | Revisión SDD y trazabilidad |
| DEL-T02 | Cerrar contrato con Ame: operaciones, códigos, DTO, recuperación | T01 | Aceptación Backend/Móvil documentada |
| DEL-T03 | Inventariar datos personales y recursos compartidos + política de respaldos/excepciones | T01, privacidad | Matriz de cada campo/colección y destino |
| DEL-T04 | Modelar estados, versionado, reloj inyectado y autorización de dos clases de token | T02 | Pruebas de transición y seguridad |
| DEL-T05 | Repositorio Mongo y solicitud atómica con plazo exacto | T03, T04 | Unitarias e integración duplicados/UTC |
| DEL-T06 | Reautenticación, token restringido y cancelación atómica | T02, T04, T05 | 401/409/410; token restringido no accede a rutas |
| DEL-T07 | Worker durable idempotente y reanudación en reinicio | T03, T05 | Reintentos, reinicio, condiciones de carrera |
| DEL-T08 | Borrado por inventario con aislamiento entre titulares | T03, T07 | Ausencia comprobada de datos personales propios y preservación de otros |
| DEL-T09 | Integrar API/OpenAPI y mantener 7 operaciones previas compatibles | T02, T04–T08 | Pruebas de contrato y regresión |
| DEL-T10 | Evidencia técnica, informe, riesgos y memoria/trazabilidad | T01–T09 | `BACKEND_DELETE_001.md` con resultados ejecutados reales |

No subir cambios ni crear commits hasta autorización específica. Delimitar tipos `real`/`simulada` para cada prueba y evidencia.
