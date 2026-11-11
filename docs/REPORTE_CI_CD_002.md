# Reporte CI/CD-002 — Rutitas Backend

**Fecha:** 2026-11-11. **Estado:** revisión/CI en progreso; no fusionado ni desplegado.

## 1. Auditoría inicial

- Repositorio: `Rutitas_BackEnd`; `origin` apunta al repositorio oficial.
- `main` y `origin/main`: `b0dc60b91b73536e27dc6616132a225663d29aed`; sincronizados tras `git fetch origin --prune`.
- Rama auditada: `feature/ci-cd-001-bootstrap` en `3ec870e2e827d6cd6307bbd688c57e64d15c5933`, sincronizada con `origin`; worktree limpio, sin cambios indexados/no indexados ni archivos no rastreados.
- Ramas remotas observadas: `main` y `feature/ci-cd-001-bootstrap`. PR #1 está abierto hacia `main`, sin fusionar.
- Workflows existentes en la rama CI/CD-001: `ci.yml`, `package-release.yml`; no existían en main al inicio de esa fase. En main y rama no se encontraron `.sln`, `.slnx`, `.csproj`, tests ni código .NET.
- README, AGENTS, Constitución local, `memoria.md`, spec/plan/task de arquitectura base y `docs/decisiones-pendientes.md` revisados. Constitución/espejo SHA-256 `0C0EEC34E4ADDAC827B5B1684C09ECDDA6784D47AD07B9D398FBBE99FF03FC37`, coinciden y siguen `PENDIENTE_RATIFICACION`. Se conservan 11 requisitos y 7 tareas (`no_iniciado`/`no_ejecutada`).
- No se encontraron archivos sensibles por nombre entre los rastreados (`.env`, llaves, keystores, credenciales); `.gitignore` excluye formatos privados habituales.
- Decisiones relevantes: preferencia ASP.NET Core/Google Cloud/Firestore y antecedentes Azure/MongoDB constan como propuestas, no aprobaciones; `global.json`, SDK, proveedor, NoSQL concreto, balanceo/instancias, costo, identidad y diseño operativo siguen pendientes. La fuente primaria del profesor no está en el checkout.

## 2. Respaldo, base y archivos protegidos

- Bundle privado fuera del repo: `..\_Rutitas_CI_CD_002_Backups\2026-11-11\Rutitas_BackEnd.bundle`; SHA-256 `3B9EFEADE2915C2EFE6FDEC9EA10354C8BBB0F3BD920C8A980571A95923E40F0`.
- Manifiesto: `Rutitas_BackEnd.manifest.txt`; SHA-256 `224AFD1E487D60F40A50108573D5CEC9C16C9D589F1373EB4B156B4B377EA0B2`.
- `git bundle verify` aprobó historial y refs; clon espejo de recuperación confirmó `feature/ci-cd-001-bootstrap` en `3ec870e...` y `main` en `b0dc60b...`. Worktree estaba limpio, no había archivos no rastreados; por eso no se omitieron cambios locales.
- Se preservaron Constitución, AGENTS, README, memoria, archivos base `specs/000-arquitectura-base/*`, historial y PR #1. No se modifica `main`.

## 3. Implementación y archivos

- Base de trabajo: `feature/ci-cd-002-backend`, creada desde `feature/ci-cd-001-bootstrap` para apilar el PR #2 sobre el PR #1 y evitar duplicar su diff.
- Cambiado: `.github/workflows/ci.yml` (el PR trigger incluye main y la rama padre CI/CD-001; nuevo job Dependency Review fijado a `a1d282b36b6f3519aa1f3fc636f609c47dddb294`, falla ante vulnerabilidades nuevas altas o críticas).
- Añadidos: `specs/002-ci-cd/{spec.md,plan.md,tasks.md,checklist.md}`, `docs/trazabilidad-ci-cd-002.md`, este reporte.
- No se crearon servicio, API, tests ficticios, cloud resources ni configuración de secretos. `package-release.yml` permanece manual, dependiente de proyecto/SDK aprobado y sin despliegue.

## 4. Arquitectura de pipeline y verificación local

El job SDD/YAML/Gitleaks se conserva. El job Dependency Review solo corre en PR. La detección existente busca solución .NET; el build exige `global.json` y tests reales. Sin código, reporta y omite el build. La publicación manual solo produciría un artefacto revisable cuando exista proyecto real.

- Estado inicial CI/CD-001 (no esta rama): [run 43058862314](https://github.com/rutitasrutitas-eng/Rutitas_BackEnd/actions/runs/43058862314) pasó parser YAML de ambos workflows, SDD, Gitleaks y detección; build .NET quedó `skipped` por no existir solución.
- No se pudieron ejecutar pruebas de producto: no hay proyecto .NET ni tests. No se ejecutó empaquetado manual, despliegue, Terraform, ni creación de recursos facturables.
- Verificación CI/CD-002, Gitleaks y Dependency Review: pendiente del PR de esta rama.
- Auditoría de dependencias del proyecto: NO VERIFICADA; no hay manifiestos ni paquete real. Dependency Review solo cubrirá cambios de manifiestos en el PR.

## 5. Resultados, PR y estados

- PR CI/CD-001: https://github.com/rutitasrutitas-eng/Rutitas_BackEnd/pull/1 (abierto, base `main`, no fusionado).
- PR CI/CD-002: pendiente de abrir hacia `feature/ci-cd-001-bootstrap`.
- SHA final y URL de Actions de CI/CD-002: se completarán tras publicar y validar el PR.
- No se generaron artefactos de aplicación/API. No hay resultado de pruebas backend.

## 6. Requisitos cubiertos y pendientes

Cubiertos documentalmente: SDD local, escaneo de secretos, validación YAML, detección de proyecto, omisión honesta del build ausente, PR por fases y trazabilidad. Pendientes: código y tests reales; inventario/auditoría de dependencias del producto; SDK y stack ratificados; OpenAPI/contratos aprobados; fuente docente primaria; arquitectura cloud, NoSQL/Redis, red, balanceador/HTTPS, costo, identidad, observabilidad, CI/CD de entornos, secretos/OIDC/aprobadores, smoke tests y rollback.

## 7. Revisión humana y CD

Revisar y aprobar PR #1 hacia `main` primero; después revisar PR #2 hacia `feature/ci-cd-001-bootstrap`. Tras integrar #1, retargetear #2 a `main` y volver a revisar su diff/checks antes de cualquier merge manual. La autorización actual no incluye merge, selección de stack ni despliegue. Para CD real se requieren decisiones aprobadas, permisos/environment protegidos, credenciales federadas, infraestructura revisada y pruebas de operación/recuperación.

Local checks before publication: `git diff --cached --check` passed. A local YAML parser/actionlint is unavailable in this Windows environment, so YAML validation is recorded as NO VERIFICADO locally; the PR workflow will parse both YAML files with its existing Ruby/Psych step. No product tests could run because the repository contains no .NET project or test project.

## Hallazgo y corrección del primer run de CI/CD-002

El primer run de ambos PR (Backend 43061238943 y Móvil 43061238655) hizo fallar Dependency Review con el mensaje: `Dependency review is not supported on this repository. Please ensure that Dependency graph is enabled`. No fue un hallazgo de vulnerabilidad: la característica del repositorio está deshabilitada y no existen manifiestos Android. Para que el gate no ejecute análisis sin proyecto, el job se condiciona ahora a `detect-project.present == true`; si no hay wrapper, la revisión queda `skipped` y registrada como NO VERIFICADA. No se cambiaron settings del repositorio. Antes de incorporar dependencias reales, el administrador debe habilitar Dependency Graph en GitHub y el job volverá a ejecutarse.
