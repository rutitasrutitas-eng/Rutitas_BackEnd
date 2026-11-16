# TASK — 002 Catálogo funcional integral

**Estado formal:** `propuesto` · **Ejecución de código:** `no_iniciada` · **Aprobación humana:** pendiente.

Los estados `preparado_para_revision` se refieren solo a documentos creados, **no** a tareas ejecutadas/aprobadas formalmente.

| ID | Tarea documental | Dependencia | Entregable | Avance de borrador | Aprobación formal |
|---|---|---|---|---|---|
| BE-CAT-01 | Consolidar fichas E01–E52 y N01–N20 con fuente/módulo | — | inventario-funcional.md | preparado_para_revision | pendiente |
| BE-CAT-02 | Contrastar endpoints reales contra E/N y normalizar rutas en propuesta | BE-CAT-01 | Matriz de correspondencias | preparado_para_revision | pendiente |
| BE-CAT-03 | Identificar funciones locales y duplicidades | BE-CAT-01 | Registro de solapamientos | preparado_para_revision | pendiente |
| BE-CAT-04 | Acordar separación ruta/catálogo/viaje con Móvil y Backend | BE-CAT-02 | Decisión documentada | pendiente | pendiente |
| BE-CAT-05 | Aprobar mapa de dominios, alcance MVP y prioridades | BE-CAT-02,03,04 | Acta con responsable y fecha | pendiente | pendiente |
| BE-CAT-06 | Acordar modelo de identidades, comunidades y aprobación de conductores | BE-CAT-05 | ADR/criterios autorización | pendiente | pendiente |
| BE-CAT-07 | Acordar modelos de solicitudes, asientos, concurrencia, cancelación | BE-CAT-05 | ADR/estados/índices | pendiente | pendiente |
| BE-CAT-08 | Acordar privacidad, retención, documentos y ubicación | BE-CAT-05 | Políticas aprobadas | pendiente | pendiente |
| BE-CAT-09 | Definir ruta canónica OpenAPI y compatibilidad Kotlin | BE-CAT-04,05 | Contrato revisado por Móvil | pendiente | pendiente |
| BE-CAT-10 | Crear spec/plan/task por módulo aprobado (sin implementación) | BE-CAT-05..09 | Carpetas specs funcionales | pendiente | pendiente |
| BE-CAT-11 | Validar trazabilidad RF→AC→tareas→pruebas y revisión constitucional | BE-CAT-10 | Matriz y acta de revisión | pendiente | pendiente |
| BE-CAT-12 | Habilitar fase posterior SDD Azure IaC tras alcance completo | BE-CAT-11 | Decisión de transición | bloqueada | pendiente |
| BE-CAT-13 | Habilitar fase posterior SDD CI/CD DevSecOps tras Azure | BE-CAT-12 | Decisión de transición | bloqueada | pendiente |

## Reglas de ejecución
- No introducir nuevos endpoints, modelos Mongo, roles o servicios sin spec aprobado.
- Conservar historial de `001-unit2-api`; la aprobación formal no se infiere de 63 pruebas verdes.
- Antes de Azure deben estar definidos y aprobados todos los specs funcionales del alcance seleccionado por el equipo.
- No generar ni aplicar IaC, crear recursos pagos ni desplegar sin consentimiento y presupuesto aprobados.
