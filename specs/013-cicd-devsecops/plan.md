# 013 — Plan de integración y entrega DevSecOps

**Estado:** propuesto, no ejecutar CD hasta aprobación.

## Fase A — Preparación GitHub (sin despliegue)
1. Confirmar repositorio oficial, permisos de escritura y rama base (`main` o `develop`, nunca suponerla).
2. Realizar respaldo de `Backend` y validar `.gitignore` y secretos. La copia local puede contener más cambios recientes que el último ZIP; **esa copia es la fuente de verdad**.
3. Clonar el repositorio oficial en una carpeta separada y crear rama de feature. No ejecutar `git init` sobre el ZIP ni forzar push; si hay contenido remoto, preservar historial.
4. Copiar al clon únicamente el contenido autorizado del backend, preservando raíz correcta y excluyendo temporales, secretos y `.git` local.
5. Revisar `git status` y `git diff --cached --stat` antes de confirmar; PR, revisión y merge posteriores según gate.

## Fase B — CI en PR
1. Revisar rama base y triggers de `api-ci.yml`: ya existe CI para `main`; extender a `develop` solo si la rama está acordada y protegida.
2. Validar solución `Rutitas.Api/Rutitas.Api.slnx` desde la raíz del repo; Testcontainers requiere runner con Docker.
3. Activar/perfeccionar NuGet Audit, Gitleaks, Semgrep, CodeQL, test/coverage y artifacts; verificar que checks se ejecutan, no solo archivos presentes.
4. Anotar falsos positivos y excepciones de seguridad mediante proceso formal.
5. Definir protección de rama y revisiones obligatorias cuando el dueño del repositorio lo autorice.

## Fase C — Preparación CD (bloqueada)
1. Cerrar decisiones de `012-infraestructura-azure`: región, estimación, identidad, red MongoDB Atlas, contenedor, registro, logs.
2. Definir `staging` en GitHub Environments con revisores y controles.
3. Provisionar mediante IaC **solo tras aprobación**; verificar `what-if`/plan e inventario.
4. Configurar federación GitHub→Entra OIDC con mínimo privilegio y permisos de lectura de Key Vault.
5. Modificar `api-cd.yml` para publicar imagen firmada/escaneada por SHA y desplegar revisión de Container Apps.
6. Ejecutar smoke tests, observabilidad, rollback de ensayo, registrar costos y evidencia.

## Decisiones que deben quedar firmadas
- BASE-01: rama base, quién revisa PR y quién autoriza merge.
- CICD-ADR-01: herramientas/umbrales y licencias de seguridad.
- CICD-ADR-02: estrategia de tags, firma, digest y promoción.
- CICD-ADR-03: aprobaciones de staging/rollback.
- AZ-ADR: arquitectura, costos, conectividad Atlas, secretos y permisos.

## Criterio de cierre documental
Los archivos SDD son coherentes con `constitution.md`, con `012`, y distinguen: CI ya existente, mejoras propuestas, CD pendiente y restricciones de aprobación. La documentación por sí sola **no equivale a pipeline probado**.
