# 012 — Tareas de preparación Azure

**Estado documental:** `decisión=propuesto` · `ejecución=no_iniciado` · `verificación=no_ejecutada` · `tipo=—`.

**No ejecutar recursos ni gastos hasta gates humanos.** Las siguientes tareas describen trabajo futuro propuesto y verificaciones.

| ID | Tarea | Dependencia | Evidencia de cierre futuro | Estado |
|---|---|---|---|---|
| AZ-T01 | Inventariar versión de Dockerfile, Program.cs, configuración y CI | 001 | Inventario firmado | `no_iniciado` |
| AZ-T02 | Comparar Container Apps/App Service y seleccionar vía ADR | AZ-T01 | ADR aprobado | `no_iniciado` |
| AZ-T03 | Definir región, entornos y convención de nombres | AZ-T02 | Tabla de ambientes | `no_iniciado` |
| AZ-T04 | Resolver acceso seguro desde Azure hacia MongoDB Atlas | AZ-T02 | Diagrama, prueba planificada, costo, responsable | `no_iniciado` |
| AZ-T05 | Diseñar Key Vault, identidad administrada y rotación | AZ-T03 | Matriz permisos/secretos | `no_iniciado` |
| AZ-T06 | Diseñar Azure OIDC, RBAC mínimo y protección del entorno GitHub | AZ-T03 | Diagrama trust + revisión Seguridad | `no_iniciado` |
| AZ-T07 | Estimar costo total en escenarios bajo/medio y definir alertas | AZ-T03-06 | Estimación revisada por propietario suscripción | `no_iniciado` |
| AZ-T08 | Obtener aprobación explícita de arquitectura, costo y despliegue staging | AZ-T02-07 | Acta/PR/ADR vinculada, responsables y fecha | `no_iniciado` |
| AZ-T09 | Crear módulos IaC mínimos en tecnología aprobada | AZ-T08 | PR Bicep/Terraform, lint/check, sin secrets | `no_iniciado` |
| AZ-T10 | Ejecutar `what-if`/plan sin aplicar y revisar con equipo | AZ-T09 | Diff revisado, sin recursos creados | `no_iniciado` |
| AZ-T11 | Definir integración controlada con pipeline 013 | AZ-T06,AZ-T09 | Contrato de deploy por SHA y gate manual | `no_iniciado` |
| AZ-T12 | Autorizar y aplicar infraestructura staging | AZ-T08,AZ-T10,AZ-T11 | Registro de cambio, costo y recursos | `no_iniciado` |
| AZ-T13 | Desplegar imagen aprobada y realizar smoke/security checks | AZ-T12 | Logs saneados, matriz de smoke tests | `no_iniciado` |
| AZ-T14 | Probar observabilidad, recuperación y rollback | AZ-T13 | Evidencia de rollbacks y alertas | `no_iniciado` |
| AZ-T15 | Documentar handoff a Móvil (URL staging, contrato, JWT y errores) | AZ-T13-14 | Acuerdo Backend/Móvil firmado | `no_iniciado` |

## Gates
- **Gate A — Documental:** revisar constitución candidata, specs y ADR; asignar autoridad.
- **Gate B — Financiero/seguridad:** suscripción, presupuesto, secreto, red Atlas, roles y acceso.
- **Gate C — IaC:** solo se permite aplicar después de revisar el plan de cambios y aprobación humana.
- **Gate D — Release:** smoke tests, alertas y rollback; ningún deploy producción por inercia.

## Nota de trazabilidad
Cada tarea debe enlazarse en PR/registro con los requisitos AZ-01..AZ-15 de `spec.md`. Los estados permanecen no iniciados; esta tabla no equivale a ejecución.
