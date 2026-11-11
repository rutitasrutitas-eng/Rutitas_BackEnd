# Trazabilidad SDD — CI/CD-001 Backend

**Ámbito:** exclusivamente Rutitas Backend. **Autoridad:** Constitución local candidata, `PENDIENTE_RATIFICACION`. **Decisión CI/CD:** propuesta; revisión por PR pendiente. Este anexo no cambia la arquitectura base ni aprueba stack, SDK, proveedores o infraestructura.

| Requisito | Evidencia preparada | Criterio de cierre de esta fase | Límite actual |
|---|---|---|---|
| CI-01 | `.github/workflows/ci.yml` → job `sdd-and-security` | GitHub Actions valida archivos SDD y el run queda enlazado en `docs/CI_CD.md` | Validación documental, no revisión académica ni ratificación |
| CI-02 | `.github/workflows/ci.yml` → Gitleaks con historial completo | Paso Gitleaks aprobado en el run enlazado; registrar hallazgos sin secretos | No acredita ausencia de riesgos no detectados |
| CI-03 | `.github/workflows/ci.yml` → `detect-project` y `dotnet-build` condicional | Sin solución en el repositorio, build/test queda omitido y reportado como NO VERIFICADO | No hay código API ni pruebas de producto |
| CI-04 | Retención de reporte TRX del job de build si llega a existir | Artefacto presente solo cuando el build real produce resultados | No se genera reporte de tests cuando el job se omite |
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

| CI-05 | `.github/workflows/ci.yml` → parseo con Ruby/Psych de `ci.yml` y `package-release.yml` | Ambos archivos se parsean sin error en el run enlazado | Comprueba sintaxis YAML; no sustituye aprobación de cambios del pipeline |
