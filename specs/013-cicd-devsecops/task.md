# 013 — Tareas CI/CD DevSecOps

> **Cotejo 13/11/2026:** la tabla conserva las tareas propuestas originales. La rama correctiva implementa preparación de CI, pero cada tarea sólo se verificará con el run del SHA exacto y revisión humana. Véase [validación técnica](../../docs/VALIDACION_MARK.md).

**Decisión:** propuesto · **Ejecución:** no_iniciado · **Verificación:** no_ejecutada.

| ID | Tarea | Dependencias | Evidencia de cierre | Estado |
|---|---|---|---|---|
| CICD-T01 | Confirmar repositorio, propietarios y permiso push | D02/D03 | Captura permiso y acuerdo de rama | Pendiente |
| CICD-T02 | Respaldar y limpiar archivos sensibles | T01 | `git status`, revisión de secretos | Pendiente |
| CICD-T03 | Abrir rama feature, preparar PR documental | T01,T02 | URL de PR y commits | Pendiente |
| CICD-T04 | Alinear triggers de CI a ramas aprobadas | T01 | PR y workflow validado | Pendiente |
| CICD-T05 | Validar restore/build Release en runner | T04 | Log de Actions exitoso | Pendiente |
| CICD-T06 | Ejecutar 63 casos o suite actualizada con Docker | T05 | TRX y coverage de GitHub | Pendiente |
| CICD-T07 | Validar escáneres: Gitleaks/Semgrep/CodeQL/NuGet | T04 | Logs y decisiones de hallazgos | Pendiente |
| CICD-T08 | Validar Docker, Trivy y SBOM | T05 | Digest/SBOM y resultado del scan | Pendiente |
| CICD-T09 | Establecer protecciones de ramas y revisores | T01 | Evidencia configuración aprobada | Pendiente |
| CICD-T10 | Aprobar IaC/costos/red y ambiente Azure staging | `012` y gates | ADR y presupuesto | Bloqueada |
| CICD-T11 | Configurar OIDC, identidades y secretos Azure | T10 | Evidencia RBAC / acceso controlado | Bloqueada |
| CICD-T12 | Implementar publicación y despliegue gated | T08,T11 | PR de workflow CD, revisión | Bloqueada |
| CICD-T13 | Ejecutar smoke tests y revisar logs/costos | T12 | Resultados, métricas y capturas | Bloqueada |
| CICD-T14 | Probar rollback con autorización | T13 | Bitácora de rollback | Bloqueada |
| CICD-T15 | Revisión final y cierre documental | T03-T14 | Matriz y aprobaciones | Pendiente |

**Regla:** los pasos bloqueados no se habilitan mediante edición del texto, y no deben generarse secretos ni operaciones sobre la nube sin autorización.

## Evidencia de revisión técnica (13/11/2026; sin cambiar estados de tareas)

La rama correctiva integra mediante merge normal la rama de Mark en el SHA observado `0f05c07`; se resolvieron cuatro archivos en conflicto y se fijó SDK `10.0.400` comprobado localmente. El restore y build Release/análisis se ejecutaron; el build terminó con cero errores y 186 advertencias reportadas. Localmente, cuatro suites pasaron (57 casos); Integration pasó 10/13 y 3 pruebas Mongo quedaron bloqueadas por falta de Docker. GitHub Actions run [43537627859](https://github.com/rutitasrutitas-eng/Rutitas_BackEnd/actions/runs/43537627859) verificó las cinco suites: 70/70, sin fallos ni omisiones; Gitleaks pasó y NuGet Audit reportó 0 advisories. La evidencia detallada está en [VALIDACION_MARK.md](../../docs/VALIDACION_MARK.md). Esto no cierra formalmente CICD-T06/T07: CodeQL/Trivy siguen pendientes y la revisión humana del PR continúa requerida. Se conservan los estados originales de tareas y la autoridad de aprobación.
