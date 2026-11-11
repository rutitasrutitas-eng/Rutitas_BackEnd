# CI/CD-002 — Especificación de Backend

**Decisión:** `propuesto`. **Ejecución:** `implementado` en rama de revisión apilada. **Verificación:** `no_ejecutada` hasta terminar Actions del PR. **Tipo:** `—` (controles CI; no funcionalidad Backend).

## Alcance y límites

Mejorar los controles de seguridad y trazabilidad del CI propuesto en CI/CD-001. Solo se ejecutan los controles documentales, análisis de cambios de dependencias y detección de proyectos que existen. No se crean API, tests ficticios, SDK fijado, cloud resources ni infraestructura. CI/CD-001 y este incremento siguen sujetos a revisión humana; las Constituciones permanecen propuestas.

## Requisitos

- **BE-CI2-01:** mantener validación SDD, parseo YAML de ambos workflows y Gitleaks con historial completo.
- **BE-CI2-02:** revisar en PR dependencias introducidas cuando exista solución/manifiesto real; fallar si se agrega una vulnerabilidad de severidad alta o crítica.
- **BE-CI2-03:** compilar/probar y auditar paquetes solo cuando haya solución real y SDK fijado mediante `global.json`; sin solución, indicar que no se verificó el producto.
- **BE-CI2-04:** conservar artefactos de tests solo cuando existan; el workflow manual prepara artefacto de revisión y no despliega.
- **BE-SEC2-01:** conservar permisos mínimos, acciones de seguridad fijadas a SHA y no incluir credenciales.

## Criterios

La validación remota debe identificar cada job y su estado. Dependency Review evalúa cambios de dependencias del PR solo si la detección confirma un proyecto .NET real; no es un inventario total. Si no hay proyecto/manifiestos se omite e informa como NO VERIFICADO. GitHub Dependency Graph debe estar habilitado antes de agregar dependencias reales. Si el repositorio no contiene manifiestos/producto, el reporte lo declarará y no atribuirá auditoría de paquetes ni pruebas de API. El build/CD completo queda `no_ejecutada` mientras falten código y decisiones.
