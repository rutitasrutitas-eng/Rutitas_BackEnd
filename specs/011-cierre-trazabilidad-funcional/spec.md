# 011 — Cierre de trazabilidad funcional | Especificación

**Proyecto:** Rutitas Backend. **Fecha:** 11/11/2026. **Decisión:** propuesta; aprobación pendiente. **Implementación:** no autorizada para módulos nuevos.

## Objetivo

Verificar trazabilidad de las **72 fichas** originales (E01–E52 y N01–N20), identificar funciones parcialmente documentadas y fijar decisiones de alcance antes de cerrar las especificaciones funcionales y elaborar la infraestructura de Azure.

## Fuentes y evidencia

- `specs/002-catalogo-funcional/inventario-funcional.md` (52 E + 20 N).
- `specs/003-*` a `specs/010-*` (`spec.md`) y sus planes y tareas.
- `specs/001-unit2-api/contracts/openapi.yaml`: contrato de **7 operaciones implementadas**, sin inferir implementación de las demás.
- `matriz-trazabilidad.md`: cotejo de IDs y documentos responsables; no equivale a validación funcional del código.

## Requisitos de cierre

| ID | Regla comprobable |
|---|---|
| CT-01 | Cada ficha E/N tiene un documento responsable o motivo verificable de exclusión. |
| CT-02 | Las fichas propuestas tienen criterio de aceptación, autorización, datos de entrada/salida, errores, seguridad y prueba previstos antes de implementación. |
| CT-03 | Los 7 endpoints actuales se reutilizan como contratos existentes; no se generan alias o duplicados sin aprobación. |
| CT-04 | El prefijo del backend real es `/api/v1`; las rutas del Excel `/v1/...` son propuestas, no rutas ejecutables. |
| CT-05 | `N14` permanece en Android; no se crea endpoint ficticio de permisos. |
| CT-06 | `N19` y `N20` quedan fuera de implementación mientras el equipo no delimite aportaciones informativas frente a pagos. |
| CT-07 | Publicaciones de viaje y catálogo de rutas son conceptos separados hasta decisión arquitectónica final. |
| CT-08 | No se crea implementación ni infraestructura antes de completar las aprobaciones y pruebas requeridas. |

## Brechas encontradas en el cotejo textual

Identificadores no localizados literalmente en el `spec.md` de su módulo responsable: **E08, E09, E10, E11, E12, E13, E14, E15, E22, E30, E50, N05, N06, N12, N13, N15, N16**. Esto es una alerta de trazabilidad, no prueba de ausencia de requisitos equivalentes redactados sin código E/N. Revisar `plan.md` y `task.md` antes de concluir cobertura.

## Criterios de aceptación de esta fase

1. Matriz sin fichas huérfanas y sin doble dueño sin justificación.
2. Decisiones abiertas respondidas por líder, Backend, Móvil y Seguridad/Privacidad; registrar fecha y responsable.
3. Contratos canónicos acordados para nuevas funciones, sin cambiar los 7 endpoints actuales por inercia.
4. Riesgos y pendientes documentados, sin afirmar que las 72 fichas están implementadas.
5. Solo entonces solicitar autorización para crear los specs independientes de Azure IaC y DevSecOps CI/CD.
