# 013 — Especificación CI/CD DevSecOps de Rutitas Backend

**Fecha:** 12/11/2026  
**Estado de decisión:** propuesto · **Ejecución:** no_iniciado · **Aprobación:** pendiente  
**Alcance:** backend ASP.NET Core 10, siete endpoints existentes, pruebas, contenedor y despliegue futuro a Azure staging.

## 1. Objetivo
Establecer una cadena trazable y reproducible para integrar código, detectar fallos de calidad o seguridad, generar una imagen verificable y, **solo después de aprobación**, desplegarla a Azure. No confundir CI funcional con CD habilitado ni la existencia del documento con aprobación del pipeline.

## 2. Línea base verificada en el ZIP de Backend
- `.github/workflows/api-ci.yml`: CI en `push` y PR a `main` más ejecución manual; .NET 10, NuGet Audit, build con advertencias como errores, tests/coverage, Gitleaks y Semgrep.
- `.github/workflows/api-codeql.yml` y `api-nightly.yml`: análisis adicionales existentes; validar ejecución real en GitHub.
- `.github/workflows/api-cd.yml`: solo `workflow_dispatch` con frase `DEPLOY-STAGING`; realiza pruebas, build Docker, Trivy y SBOM. **No publica imagen ni despliega a Azure**.
- `Rutitas.Api/Rutitas.Api/Dockerfile`: .NET 10 multi-etapa, escucha en puerto 8080, proceso sin root.
- El equipo reporta **63/63 pruebas locales** aprobadas con Docker; en GitHub todavía debe comprobarse CI, especialmente Testcontainers con Docker disponible.
- Repositorio destino: `https://github.com/rutitasrutitas-eng/Rutitas_BackEnd.git`; `main` es rama predeterminada. La cuenta vinculada al chat carecía de push al revisarla el 12/11/2026.

## 3. Requisitos y aceptación
| ID | Requisito | Evidencia/criterio |
|---|---|---|
| CICD-01 | PR hacia rama base autorizada | PR con autor, responsables, historial y controles ejecutados; sin commits directos a `main` |
| CICD-02 | Triggers de CI coherentes con estrategia de ramas | CI verifica PR dirigidos a `main` o `develop`, **según exista y se apruebe**; los eventos push quedan explícitos |
| CICD-03 | Restore/build .NET 10 | `dotnet restore` con auditoría y build Release sin advertencias toleradas, o excepción aprobada |
| CICD-04 | Suite de pruebas | Test unitarios, integración Mongo/Testcontainers, seguridad, contrato y arquitectura; TRX y cobertura publicadas; no basarse solo en resultado local |
| CICD-05 | SAST | Semgrep y CodeQL activos con reglas, alertas y tratamiento documentado |
| CICD-06 | Detección de secretos | Gitleaks sobre cambios/historia según alcance; cero credenciales reales en repositorio, artifacts o logs |
| CICD-07 | Dependencias y supply chain | NuGet Audit, dependencias de Actions preferentemente fijadas a SHA, SBOM y revisión de vulnerabilidades |
| CICD-08 | Construcción de imagen | Docker build desde `Rutitas.Api`, etiqueta por SHA de commit y metadatos de procedencia |
| CICD-09 | Contenedor | Trivy bloquea vulnerabilidades críticas/altas sin excepción aprobada; SBOM disponible |
| CICD-10 | Permisos mínimos | `GITHUB_TOKEN` por defecto `contents: read`; permisos elevados únicamente por job, OIDC `id-token: write` solo en deploy aprobado |
| CICD-11 | Secretos de Azure | Federated credentials OIDC y variables por GitHub Environment; no publicar claves largas ni Mongo/JWT en GitHub |
| CICD-12 | Despliegue staging | Gate de ambiente con aprobadores, presupuesto aprobado, IaC validada, control de imagen y autorización D08-B/equivalente |
| CICD-13 | Verificación postdeploy | Health/smoke tests de los 7 endpoints con códigos esperados, TLS, acceso a Atlas y trazas sin PII |
| CICD-14 | Rollback | Revisión anterior conservada, procedimiento probado y criterio de activación documentado |
| CICD-15 | Evidencia | PR, estados de checks, artifacts de test, SHA imagen, decisión de release y bitácora |

## 4. Política de seguridad
- No introducir `.env`, `appsettings.Local.json`, `*.local.http`, secretos de usuario, cadenas MongoDB, certificados ni tokens en Git.
- Mantener exclusión de `bin/`, `obj/`, `.vs/`, `TestResults/` y artifacts temporales.
- Las 63 pruebas **no demuestran** por sí solas aptitud de producción: se requieren pruebas en CI, firma/autorización, seguridad de nube y pruebas de integración móvil.
- Ningún workflow de este módulo puede desplegar automáticamente por el simple hecho de hacer push.
- No abrir el acceso a Atlas `0.0.0.0/0` como sustituto de una política de egreso segura.

## 5. Límites del alcance
Este documento no implementa los endpoints propuestos en `003` a `010`, ni concede permisos de repositorio, ni aprovisiona recursos de Azure. La dependencia `012-infraestructura-azure` debe tener ADR, IaC, permisos, presupuesto y conectividad resueltos antes de habilitar CD.
