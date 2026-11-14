# Tareas — API Unidad II

> **Estado técnico revisado — 13/11/2026:** La API y pruebas existen en la rama de Mark; el reporte de 63 aprobadas es local e histórico. Esta tabla conserva el plan SDD original, no acredita ejecución ni aprobación de cada tarea. Las actas y ADR mencionadas son entregables futuros, no archivos existentes. Véanse [validación técnica](../../docs/VALIDACION_MARK.md) y [compatibilidad Android](../../docs/COMPATIBILIDAD_ANDROID.md).

**ID:** TASK-U2-API  
**Decisión:** `propuesto` · **Ejecución:** `no_iniciado` · **Verificación:** `no_ejecutada` · **Tipo:** `experimental`

> Todas son propuestas futuras. **No ejecutar sin autorización humana registrada.** Las tareas `BE-AB-*` de `specs/000-arquitectura-base/task.md` no se modifican. Usar `task.md` singular según la convención académica.

| ID | Descripción | Dependencias | Prioridad | Entregable esperado | Validación futura (no ejecutada) | Decisión | Ejecución | Verificación | Tipo |
|---|---|---|---|---|---|---|---|---|---|
| BE-U2-01 | Aprobar Gate 0-A: alcance, roles, NFR, controles y autorización documental | — | P0 | `APR-U2-001` etapa A firmada (`D01`, `D02`, `D03`, `D04`, `D07`, `D08-A`, `D12`, `D13`; dirección `D05`/`D06`) | Registro con fuente, fecha, alcance y firmas | propuesto | no_iniciado | no_ejecutada | — |
| BE-U2-02 | Registrar stack propuesto y ADR comparativo | BE-U2-01 | P0 | `docs/ADR-001-stack.md` y actualización de `D05` | Revisión de alternativas, soporte, costo y seguridad | propuesto | no_iniciado | no_ejecutada | — |
| BE-U2-03 | Acordar alcance inicial, plataforma y modelo de errores con Móvil | BE-U2-02 | P0 | Acta inicial `D06-A`/`D13`; no aprueba el OpenAPI final | Revisión de compatibilidad y modelo de errores | propuesto | no_iniciado | no_ejecutada | — |
| BE-U2-04 | Redactar y revisar `spec.md` | BE-U2-01, BE-U2-02 | P0 | Spec funcional revisada | Criterios `AC-U2` verificables y trazabilidad | propuesto | no_iniciado | no_ejecutada | — |
| BE-U2-05 | Redactar y revisar `plan.md` + OpenAPI | BE-U2-04 | P0 | Plan técnico y contrato preliminares | OpenAPI válido y sin endpoints adicionales | propuesto | no_iniciado | no_ejecutada | — |
| BE-U2-05A | Aprobar el OpenAPI final con el cliente móvil | BE-U2-03, BE-U2-05 | P0 | Acta de aprobación del OpenAPI final (`D06-B`) | Contrato válido, compatible y sin endpoints adicionales | propuesto | no_iniciado | no_ejecutada | — |
| BE-U2-06 | Redactar `task.md` y matriz de trazabilidad | BE-U2-05A | P0 | Tareas `BE-U2-*` y matriz RF→tarea→prueba | Dependencias, estados y cobertura completas | propuesto | no_iniciado | no_ejecutada | — |
| BE-U2-07 | Crear scaffold ASP.NET Core 10 y configuración base | BE-U2-06, `D02`, `D08-B` | P1 | Solución API/Application/Domain/Infrastructure + tests | Compilación, dependencias y ausencia de secretos | propuesto | no_iniciado | no_ejecutada | experimental |
| BE-U2-08 | Implementar modelos MongoDB, índices, validación y seed | BE-U2-07 | P1 | Colecciones `users`, `revoked_tokens`, `routes` y seed sintético | Índices únicos, TTL y JSON Schema verificados | propuesto | no_iniciado | no_ejecutada | experimental |
| BE-U2-09 | Implementar registro y login con JWT | BE-U2-08 | P1 | Endpoints `register` y `login` | Pruebas de hash, duplicado, credenciales y rate limit | propuesto | no_iniciado | no_ejecutada | experimental |
| BE-U2-10 | Implementar logout y revocación `jti` | BE-U2-09, `D10` | P1 | Endpoint `logout` + denylist TTL | Pruebas de idempotencia y token revocado | propuesto | no_iniciado | no_ejecutada | experimental |
| BE-U2-11 | Implementar `GET/PATCH /users/me` | BE-U2-09 | P1 | Perfil propio y actualización parcial | Pruebas de autorización, IDOR e inmutabilidad | propuesto | no_iniciado | no_ejecutada | experimental |
| BE-U2-12 | Implementar `GET /routes` y `GET /routes/{id}` | BE-U2-08, `D11` | P1 | Listado paginado y detalle | Pruebas de paginación, filtro, `404` y visibilidad | propuesto | no_iniciado | no_ejecutada | experimental |
| BE-U2-13 | Implementar validación, ProblemDetails, OpenAPI y observabilidad | BE-U2-07 | P1 | Middleware, validadores, logs y contrato publicado | Errores sin datos internos, `traceId` y OpenAPI consistente | propuesto | no_iniciado | no_ejecutada | experimental |
| BE-U2-14 | Ejecutar pruebas unitarias, integración, contrato y seguridad | BE-U2-09–BE-U2-13 | P0 | Suite y reporte de cobertura | Casos `AC-U2`, OWASP API Top 10 y Testcontainers | propuesto | no_iniciado | no_ejecutada | experimental |
| BE-U2-15 | Configurar CI y gates de calidad/seguridad | BE-U2-07, BE-U2-14 | P0 | Workflow CI con evidencia | Build, tests, cobertura, auditoría, Gitleaks y Trivy | propuesto | no_iniciado | no_ejecutada | experimental |
| BE-U2-16 | Decidir nube, preparar IaC, Atlas M0 y secretos | BE-U2-02, BE-U2-15, `D09` | P1 | IaC y configuración de staging propuesta | Revisión de costo, seguridad y destrucción | propuesto | no_iniciado | no_ejecutada | experimental |
| BE-U2-17 | Desplegar staging, smoke test y rollback | BE-U2-16 + aprobación de gasto | P2 | Evidencia de staging | Smoke test, logs, rollback y presupuesto aprobado | propuesto | no_iniciado | no_ejecutada | experimental |
| BE-U2-18 | Aceptar el incremento local, conservar evidencia y actualizar memoria | BE-U2-14, BE-U2-15, BE-U2-19 | P0 | Acta de aceptación local y `memoria.md` actualizada | Revisión humana y evidencia reproducible sin cloud | propuesto | no_iniciado | no_ejecutada | — |
| BE-U2-19 | Validar compatibilidad del contrato con cliente móvil (Kotlin propuesto) | BE-U2-03, BE-U2-05A, BE-U2-12 | P1 | Revisión de contrato y ajustes acordados | Pruebas de contrato y almacenamiento seguro del token | propuesto | no_iniciado | no_ejecutada | — |
| BE-U2-20 | Aceptar staging/cloud y cerrar evidencias | BE-U2-17, BE-U2-18, `D09` | P1 | Acta de aceptación staging | Smoke test, rollback, presupuesto y evidencia | propuesto | no_iniciado | no_ejecutada | experimental |

**Estado del paquete:** decisión=`propuesto`, ejecución=`no_iniciado`, verificación=`no_ejecutada`. Las tareas de implementación usan tipo=`experimental`; las documentales, `—`. La prioridad expresa criticidad y la secuencia la determinan las dependencias. Las dependencias `BACKEND-Dxx` no están aprobadas y bloquean la tarea correspondiente. Se conservan `BE-U2-01..19`; se añaden `BE-U2-05A` y `BE-U2-20` para separar aprobación final y aceptación staging.

## Avance documental (2026-11-04)

Los estados formales de la tabla no cambian. Bajo las reglas documentales actuales se revisó lo siguiente, sin ejecutar tareas ni aprobar gates:

| Tarea | Avance permitido | Estado formal | Bloqueo |
|---|---|---|---|
| BE-U2-02 | `ADR-001` revisado; stack propuesto registrado | no_iniciado | `BE-U2-01`/Gate 0-A |
| BE-U2-03 | No ejecutada; requiere acuerdo con Móvil | no_iniciado | `D06-A`, `D13` |
| BE-U2-04 | `spec.md` revisada; sin cambios | no_iniciado | `BE-U2-01`, `BE-U2-02` |
| BE-U2-05 | `plan.md` y OpenAPI revisados; contrato válido | no_iniciado | `BE-U2-04` |
| BE-U2-05A | Matriz técnica de revisión Móvil preparada en [compatibilidad Android](../../docs/COMPATIBILIDAD_ANDROID.md); aceptación humana pendiente | no_iniciado | Móvil, `D06-B` |
| BE-U2-06 | `task.md` y matriz revisadas; sin cierre | no_iniciado | `BE-U2-05A` |

La revisión preparó evidencia documental, pero **no** marca tareas como ejecutadas, verificadas o aprobadas. Gate 0-A sigue pendiente.

## Reglas de ejecución

- `D08` se aprueba en dos etapas: **A** autoriza documentar/revisar; **B** acepta los artefactos finales y autoriza `BE-U2-07`. `D08-A` no habilita código.
- No se inicia `BE-U2-07` sin `BE-U2-06`, `D02` y `D08-B`, y sin spec/plan/task/OpenAPI revisados.
- `BE-U2-03` solo acuerda el alcance inicial; la aprobación final del OpenAPI es `BE-U2-05A` (`D06-B`).
- No se implementa logout sin `BACKEND-D10`; no se implementa listado de rutas sin `BACKEND-D11`.
- `BE-U2-18` es aceptación local y no autoriza cloud. `BE-U2-20` es aceptación staging y depende de `D09`, presupuesto y `BE-U2-17`.
- No se despliega sin `BACKEND-D09`, presupuesto aprobado y autorización separada para `BE-U2-17`.
- No se usan datos reales ni se crean recursos pagos en esta fase.
- La documentación de una tarea no la convierte en implementación ni en aprobación.

## Trazabilidad resumida

| Requisito / criterio | Tareas |
|---|---|
| RF-U2-01, RF-U2-02 | BE-U2-09 |
| RF-U2-03 | BE-U2-10 |
| RF-U2-04, RF-U2-05 | BE-U2-11 |
| RF-U2-06, RF-U2-07 | BE-U2-12 |
| AC-U2-01 | BE-U2-04, BE-U2-05, BE-U2-05A, BE-U2-06 |
| AC-U2-02, AC-U2-03 | BE-U2-09, BE-U2-14 |
| AC-U2-04 | BE-U2-10, BE-U2-14 |
| AC-U2-05 | BE-U2-11, BE-U2-14 |
| AC-U2-06 | BE-U2-12, BE-U2-14 |
| AC-U2-07 | BE-U2-13, BE-U2-14 |
| AC-U2-08 | BE-U2-08, BE-U2-13, BE-U2-14 |
| AC-U2-09 | BE-U2-14 |
| AC-U2-10 | BE-U2-05A, BE-U2-15, BE-U2-19 |
| AC-U2-11 | BE-U2-13, BE-U2-14, BE-U2-15 |
| AC-U2-12 | BE-U2-15, BE-U2-18, BE-U2-20 |


## Evidencia de implementación separada del estado de aprobación (06-11-2026)

**Importante:** la tabla original refleja el flujo formal planificado. No se cambian retroactivamente aprobaciones ni dependencias. La evidencia práctica observada se registra por separado para conciliación posterior.

| Tarea | Evidencia disponible | Verificación adicional para cierre formal |
|---|---|---|
| BE-U2-07 | Solución .NET 10 y cuatro proyectos de producción presentes | Acta/gate y compilación reproducible en CI |
| BE-U2-08 | Repositorios Mongo presentes; conexión Atlas probada en capturas | Índices, JSON Schema y seed verificados contra requisitos aprobados |
| BE-U2-09 | Registro y login observados con `201/200`; negativos probados | Contrato final, límites de abuso y trazas |
| BE-U2-10 | Logout `204` y reutilización de JWT `401` mostrados | Repetición/idempotencia y persistencia revisadas |
| BE-U2-11 | GET/PATCH de perfil `200`, intento de modificar rol `400` | Revisión IDOR y casos límite |
| BE-U2-12 | Listado `200`, detalle `200/400/404` probados | Paginación, búsquedas y visibilidad completas |
| BE-U2-13 | Validación de solicitudes y ProblemDetails presentes | Concordancia integral con OpenAPI y observabilidad |
| BE-U2-14 | Captura del explorador xUnit con 63 pruebas aprobadas | Conservar TRX/cobertura y ejecutar en CI del commit correspondiente |
| BE-U2-15 | Workflows de GitHub Actions en el árbol del proyecto | Evidencia de ejecución exitosa de los gates de CI |
| BE-U2-16/17/20 | No hay evidencia de despliegue Azure | Requiere specs funcionales completos, IaC, aprobaciones y despliegue |

**Bloqueos formales vigentes:** aprobaciones Gate 0-A / D08-B, aprobación de OpenAPI con Móvil, decisiones BACKEND-D05/D06/D09/D10/D11/D12/D13 y evidencia del proceso académico. No marcar `completado` sin verificar esos elementos.
