# Trazabilidad SDD — CI/CD-001 Backend

## Anexo de evidencia del PR correctivo #4 (13/11/2026)

Este anexo registra trabajo técnico de una rama de revisión; no ratifica la Constitución, no convierte las tareas SDD en aprobadas y no habilita CD.

| Elemento / control | Cambio o evidencia | Estado y límite |
|---|---|---|
| Rama correctiva | `fix/mark-api-devsecops-review`; respaldo local previo en `backup/fix-mark-api-devsecops-review-before-merge-2026-11-13`; merge normal de `84e9564` con el HEAD de Mark `0f05c07`, resultado `5c2824e`; commit correctivo de código `c08f998`. | El SHA documental/final debe quedar registrado en PR #4 después del push normal. No se modifica `main` ni la rama de Mark. |
| CI / YAML | `.github/workflows/ci.yml` conserva PR hacia `main` y `feature/sdd-azure-devsecops`, push correctivo, `workflow_dispatch`, SDD, Gitleaks, análisis, NuGet, cinco suites, TRX y Cobertura. | Validación remota pendiente para el nuevo SHA. |
| SDK | `global.json`: .NET SDK `10.0.400`, `rollForward: disable`. | Verificado instalado localmente; 10.0.401 no está instalado. |
| CA1062 | Guardas de entrada en `AuthController` y repositorios Mongo. | Corregido en código productivo objetivo; ubicaciones restantes son fixtures/pruebas. |
| CA2000 | Tres `HttpContent` de pruebas ahora usan liberación determinista. | Sin ubicaciones restantes en el análisis completo. |
| CA1031 / CA1308 | No se aplican supresiones amplias; correo no se modifica sin regresión específica. | CA1031 conserva cinco ubicaciones productivas a revisar; CA1308 sigue pendiente de prueba/diseño. |
| Pruebas | Arquitectura 2/2, Contrato 3/3, Seguridad 23/23 y Unitarias 29/29; Integration 10/13. | 67 aprobadas; 3 Mongo bloqueadas por Docker local ausente. Actions Linux debe validar Testcontainers. |
| Seguridad de dependencias | Gitleaks 8.30.0 local: 18 commits revisados, 0 secretos detectados. NuGet Audit: 0 advisories transitive. | Los jobs remotos del SHA final aún deben ejecutarse. |
| SAST / imagen | CodeQL, Trivy y Docker no están disponibles localmente. | Pendiente; no se publicaron imágenes ni se desplegó. |
| Riesgo merge | Se conservaron los checks de usuario activo/revocación, control de role y pruebas de rate limit que se retiraban en el commit entrante. | Mark debe revisar y confirmar la intención del cambio antes de aprobar. |

El desglose actualizado de diagnóstico por regla/archivo/prioridad está en [ADVERTENCIAS_ANALISIS_CSHARP.md](ADVERTENCIAS_ANALISIS_CSHARP.md); los resultados operativos y bloqueos están en [VALIDACION_MARK.md](VALIDACION_MARK.md). La ejecución de tareas SDD permanece sujeta a su autoridad y aprobación humanas.

## Corrección para revisión de Mark (13/11/2026)

`global.json` → selección de SDK 10.0.400; PR a `feature/sdd-azure-devsecops` → disparador de `ci.yml`; cinco `*Tests.csproj` → TRX y cobertura Cobertura XML; Gitleaks y SDD → controles conservados. El estado de ejecución para el SHA final y los hallazgos se registran en [VALIDACION_MARK.md](VALIDACION_MARK.md). La evidencia histórica siguiente corresponde a otros commits y no valida la API de Mark.

**Ámbito:** exclusivamente Rutitas Backend. **Autoridad:** Constitución local candidata, `PENDIENTE_RATIFICACION`. **Decisión CI/CD:** propuesta; revisión humana del PR pendiente. Este anexo no cambia la arquitectura base ni aprueba stack, SDK, proveedores o infraestructura.

| Requisito | Evidencia preparada | Criterio de cierre de esta fase | Límite actual |
|---|---|---|---|
| CI-01 | `.github/workflows/ci.yml` → job `sdd-and-security` | GitHub Actions valida archivos SDD y el run queda enlazado en `docs/CI_CD.md` | Validación documental, no revisión académica ni ratificación |
| CI-02 | `.github/workflows/ci.yml` → Gitleaks con historial completo | Paso Gitleaks aprobado en el run enlazado; registrar hallazgos sin secretos | No acredita ausencia de riesgos no detectados |
| CI-03 | `.github/workflows/ci.yml` → `detect-project` y `dotnet-build` condicional | Sin solución en el repositorio, build/test queda omitido y reportado como NO VERIFICADO | No hay código API ni pruebas de producto |
| CI-04 | Retención de reporte TRX del job de build si llega a existir | Artefacto presente solo cuando el build real produce resultados | No se genera reporte de tests cuando el job se omite |
| CI-05 | `.github/workflows/ci.yml` → parseo con Ruby/Psych de `ci.yml` y `package-release.yml` | Ambos archivos se parsean sin error en el run enlazado | Comprueba sintaxis YAML; no sustituye aprobación de cambios del pipeline |
| CD-01 | `.github/workflows/package-release.yml` manual y limitado a artefacto | Revisión documental; ejecución manual queda fuera de alcance | Sin despliegue a Azure, AWS o Google Cloud |
| SEG-01 | `permissions: contents: read`, checkout sin credenciales persistentes; environment en empaquetado | Revisar configuración y mantener credenciales fuera del repositorio | Environment y aprobadores deben configurarse por responsables humanos |
| P02 / P09 / P10 / P12 | Spec/plan/tareas de CI-CD-001, workflow y resultados de Actions | Evidencia SHA + URL, estados separados y aprobación humana pendiente | La Constitución local sigue propuesta; CI verde no ratifica normas |

## Evidencia de integración

- Rama: `feature/ci-cd-001-bootstrap` desde `main`.
- Commit(s) de integración: `52022e94ca960201a2cdd0aa0930ceac9b1490ea` (integración) y `7ae1351ca3be1ff87bc585e067e0964fce6245ab` (validador YAML).
- Pull Request: [#1 abierto hacia main](https://github.com/rutitasrutitas-eng/Rutitas_BackEnd/pull/1); aprobación/merge pendientes.
- PR run: [43058283697](https://github.com/rutitasrutitas-eng/Rutitas_BackEnd/actions/runs/43058283697), éxito. Sintaxis de `ci.yml`/`package-release.yml`, SDD y Gitleaks aprobados; detección .NET exitosa; job .NET omitido por falta de solución.
- SDK .NET: no seleccionado; una solución futura deberá aportar `global.json` aprobado.
- Constitución / ARCH-BE-01..11 / 7 tareas: sin modificación; tareas permanecen `no_iniciado` y `no_ejecutada`.
