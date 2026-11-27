# MAPS-001 — Integración de servicios geoespaciales

**Estado:** preparación documental / contrato candidato\
**Proyecto:** Rutitas Backend\
**Stack objetivo:** ASP.NET Core 10, C#, MongoDB existente, Azure Maps Gen2\
**Presupuesto adicional:** $0 MXN\
**Autorización vigente:** especificación, planificación, evaluación técnica y pruebas locales simuladas.

## Restricciones
- No realizar llamadas reales a Azure Maps hasta que Infraestructura confirme identidad administrada, asignación RBAC efectiva, conectividad, cuotas y controles de consumo.
- No crear recursos Azure, secretos, claves ni tokens.
- No hacer commit, push, PR, merge ni despliegue.
- No modificar `main`, la API productiva ni los cambios locales de DELETE-001.
- Mantener MAPS-001 aislado de DELETE-001 y ANDROID-005.
- No persistir ni cachear resultados de Azure Maps hasta validar términos y permisos de licencia.

## Artefactos
- `spec.md`: alcance, requisitos y criterios de aceptación.
- `plan.md`: arquitectura, seguridad, riesgos y estrategia de implementación.
- `tasks.md`: tareas secuenciadas.
- `contracts/openapi-maps-001.yaml`: contrato REST candidato para revisión.

## Versiones upstream propuestas
- Azure Maps Fuzzy Search (búsqueda de lugares/POI): API `1.0`.
- Azure Maps Geocoding y Reverse Geocoding: Search API `2026-01-01` (validar disponibilidad/compatibilidad en la cuenta).
- Azure Maps Route Directions: API `2025-01-01`.
- Las versiones upstream no cambian la versión pública de Rutitas (`/api/v1`).
