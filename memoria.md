# Memoria de trabajo — Rutitas Backend

## Revisión independiente de la entrega de Mark (13/11/2026)

- Se confirmó en GitHub que `feature/sdd-azure-devsecops` apunta a `2cb6e016cf7e012e4b5f07e71ff330e0789694ae`; `main` apunta a `cc3be2ddbdb837fd9aa6c4a2a68d0ced3028ae11`. No había PR abierto para la rama de Mark al iniciar. La corrección se prepara en `fix/mark-api-devsecops-review`, dirigida a la rama de Mark, sin modificarla directamente.
- La solución contiene API real y cinco proyectos de pruebas. Se restauró con SDK 10.0.400 y se compiló en Release con cero advertencias y errores, también en una copia temporal. Windows Control de aplicaciones bloqueó la carga de DLL (`0x800711C7`) tanto desde OneDrive como desde `%LOCALAPPDATA%\Temp`; Architecture produjo TRX con 2 fallos de carga. Se buscará evidencia de CI por SHA; no se atribuyen a CI las 63 pruebas históricas.
- Se detectó acceso a rutas con JWT de usuario inactivo y aceptación silenciosa de `role` en registro; se prepararon correcciones y pruebas negativas. `global.json`, triggers, TRX y cobertura se alinean para revisión. Véanse [informe técnico](docs/VALIDACION_MARK.md) y [compatibilidad Android](docs/COMPATIBILIDAD_ANDROID.md).
- La Constitución, el contrato entre equipos, el stack y decisiones Azure siguen pendientes de aprobación humana. Esta memoria registra implementación y pruebas observadas, no una ratificación.
- El run [43464562923](https://github.com/rutitasrutitas-eng/Rutitas_BackEnd/actions/runs/43464562923) del SHA `77d3e1e` aprobó 69/69 pruebas en cinco suites, incluidos tres casos Mongo/Testcontainers; build Release sin advertencias, SDD y Gitleaks correctos, NuGet con 0 advisories. Publicó TRX y Cobertura XML. Un cambio posterior de rate limiting/análisis requiere run propio antes de atribuirle estos resultados.

**Estado:** preparación documental para revisión docente. Constitución y AGENTS propuestos, **no ratificados**.

**Origen:** separación del paquete conjunto `Proyecto Rutitas.zip` entregado para esta tarea.

**Alcance:** proyecto Backend únicamente.

## Actividad documental registrada

1. Se utilizó como fuente la Constitución conjunta candidata de Rutitas y se creó una Constitución autónoma para Backend, manteniendo los 13 principios generales y delimitando su ámbito.
2. Se adaptó AGENTS a este proyecto y se separó la memoria factual.
3. Se conservaron los 11 requisitos, plan y 7 tareas futuras de arquitectura base extraídos de `Backend/arquitectura-base/` de la entrega previa; esa ruta histórica no está incluida en este paquete.
4. Se prepararon definiciones **documentales** de cinco subagentes y cinco skills propias.
5. Se crearon fuentes, estados, decisiones, trazabilidad, integración propuesta y matriz de alineación para que este paquete sea autocontenido.
6. Se creó el repositorio Git local y el commit `8286c36` como respaldo verificable anterior a esta reorganización; los cambios documentales posteriores no están confirmados en Git.
7. Se migraron los cinco subagentes y cinco skills a `.opencode/` con permisos de solo lectura; no se invocaron para ejecutar tareas.
8. Se normalizaron estados, trazabilidad requisito-plan-tarea, referencias locales y el registro de decisiones sin aprobar decisiones humanas.

## Corrección de auditoría (2026-11-03)

- Se corrigieron hallazgos documentales AUD-01..AUD-20 sin aprobar decisiones ni ejecutar tareas.
- Se denegó `websearch` en la configuración y en los cinco agentes, además de `webfetch` y `bash`.
- Se añadieron actores genéricos, supuestos y criterios `AC-BE-01..05` a la spec; se reforzó la trazabilidad y se evitó inventar probabilidades de riesgo.
- Se precisaron las dependencias de `BE-AB-07` con `BACKEND-D06` y el acuerdo de integración.
- Antes de editar se creó un respaldo externo; el commit `8286c36` sigue siendo un snapshot anterior.

## Qué NO se realizó

- No se ejecutó ninguna de las siete tareas del proyecto. Todos los estados son ejecución=`no_iniciado`, verificación=`no_ejecutada`, aprobación pendiente.
- No se implementó software, ni se crearon funcionalidades, endpoints o pantallas específicas.
- No se invocaron subagentes, skills ni herramientas para ejecutar tareas; la configuración OpenCode añadida es documental y de solo lectura.
- No se ejecutaron tests, builds, instalaciones o despliegues para este paquete documental.

## Antecedentes, no resultados de esta entrega

Los informes previos reportaron 83 pruebas de una aplicación experimental RU-03 en Fase 4.1; no pertenecen a esta entrega académica. La candidata móvil React/TypeScript/Vite/Capacitor no acredita una elección oficial de stack. Los ejemplos académicos de Kotlin, .NET y AWS no se consideran decisiones adoptadas.

## Decisiones pendientes y próxima actividad

Ratificar la Constitución propia y AGENTS, confirmar autoridad y repositorio canónico, acordar restricciones del proyecto, revisar integraciones propuestas con el otro equipo y obtener retroalimentación docente. Persisten las inconsistencias en [este reporte](docs/inconsistencias-pendientes.md). Después, esperar la autorización del profesor antes de especificar funcionalidades o ejecutar tareas.

Fuentes detalladas: [documentos consultados](docs/fuentes-consultadas.md). Estado de arquitectura: [spec](specs/000-arquitectura-base/spec.md), [plan](specs/000-arquitectura-base/plan.md), [task](specs/000-arquitectura-base/task.md).

## Preparación para publicación GitHub (2026-11-04)

- Se mantiene este proyecto en su raíz local independiente `Rutitas_Backend`.
- Repositorio oficial asignado: `https://github.com/rutitasrutitas-eng/Rutitas_BackEnd`; remoto local `origin` configurado para preparar la publicación.
- Se preparó un `.gitignore` local para archivos privados, temporales, editor, dependencias y artefactos generados; no excluye documentación SDD.
- Se actualizó `README.md` con alcance y estado documental, estructura, referencias locales y restricciones.
- Preparación validada documentalmente; la publicación queda pendiente de commit y confirmación del push. No afirmar publicada hasta verificar HEAD remoto.
- Constitución pendiente de ratificación; 11 requisitos conservados; 7 tareas continúan `no_iniciado` / `no_ejecutada`.
- Los cinco agentes y cinco skills permanecen como definiciones documentales de OpenCode, sin ejecución ni compatibilidad Codex acreditada.
- No se implementó software, no se definieron operaciones/endpoints, no se seleccionó stack y no se ejecutaron tareas.


## Publicación inicial verificada en GitHub (2026-11-04)

- Se confirmó el push normal de la rama main a https://github.com/rutitasrutitas-eng/Rutitas_BackEnd y se verificó que el HEAD remoto coincide con el commit local.
- El repositorio es público; la publicación de la autoría histórica fue autorizada por el usuario.
- Esta constancia registra únicamente la publicación documental. La Constitución sigue pendiente de ratificación, las siete tareas permanecen sin ejecutar y no se realizó implementación ni selección tecnológica.

## Registro exploratorio de tecnologías y API (2026-11-04)

- Por comunicación del usuario, se registra **NoSQL como requisito docente reportado** y **Azure, AWS o Google Cloud** como únicos proveedores de hospedaje considerados permitidos; falta verificar el documento primario.
- El equipo propuso **ASP.NET Core 10/C#**; se documentó **Google Cloud Run + Firestore NoSQL** como candidato principal, Maps para puntos públicos/rutas y Firebase Authentication como alternativa no elegida.
- Se añadieron anexos de [stack](docs/propuestas-futuras/stack-backend-y-cloud.md) y [7 endpoints exploratorios](docs/propuestas-futuras/inventario-7-endpoints.md) con dos escenarios dependiendo de autenticación. No son `spec.md`, contratos ni decisiones ratificadas.
- Constitución y espejo no modificados; 11 requisitos y las 7 tareas de arquitectura base se mantienen. No hubo implementación, despliegue, pruebas ni ejecución. La actualización documental se publicó mediante push normal en main; el HEAD remoto se verificó coincidente con el commit local.

## Publicación de anexos documentales verificada (2026-11-04)

- Se confirmó el push normal de esta actualización documental a la rama main del repositorio propio.
- El HEAD remoto se comprobó coincidente con el commit local; la revisión directa de GitHub confirmó los documentos del paquete.
- No se implementó software ni se ejecutaron tareas; propuestas tecnológicas y el inventario de endpoints y las alternativas tecnológicas permanecen exploratorios.

## Integración CI/CD-001 (2026-11-11)

- Fuente de archivos: `Rutitas_Pipelines_CI_CD_Preparados.zip`; se integró únicamente la carpeta Backend a la rama `feature/ci-cd-001-bootstrap`, creada desde `main` tras volver a comparar `origin/main`.
- Se añadieron workflows de SDD/Gitleaks, detección de solución .NET, compilación condicional y empaquetado manual de revisión; junto con `docs/CI_CD.md` y `specs/001-ci-cd/`.
- Se añadió `docs/trazabilidad-ci-cd.md` para relacionar CI-01..04, CD-01 y SEG-01 con controles, evidencia y límites. El empaquetado manual no se ejecutó.
- El repositorio no contiene solución `.sln/.slnx` ni `.csproj`; por eso no se afirma build/test de backend. La ruta futura requiere `global.json` con SDK acordado; el workflow no selecciona una versión .NET por inferencia.
- La Constitución y su espejo no se modificaron y siguen `PENDIENTE_RATIFICACION`. Se conservaron ARCH-BE-01..11 y las siete tareas de arquitectura base (`no_iniciado` / `no_ejecutada`).
- El escaneo local de patrones de secretos encontró cero coincidencias. El PR #1 está abierto y el run PR 43058283697 pasó validación YAML, SDD y Gitleaks; detección .NET aprobada y build/tests omitidos por falta de proyecto. No se implementó API ni se desplegó infraestructura.

## CI/CD-003 — Auditoría de integración y preparación (2026-11-11)

- Se comprobó en GitHub que los PR #1 y #2 de CI/CD de Rutitas Backend aparecen **fusionados** en sus respectivas ramas destino; esto no prueba que CI/CD-002 se encuentre en `main` (el PR #2 estaba dirigido a la rama CI/CD-001).
- En el árbol revisado de `main` continúan únicamente el SDD y los workflows; **no se halló código de aplicación ni proyectos compilables**. No se generan proyectos ficticios ni se afirman pruebas de producto.
- Los tests .NET se ejecutarán explícitamente cuando se integren proyectos de pruebas reales; sin .sln/.csproj no se acreditan tests.
- La nueva rama de trabajo CI/CD-003 no autoriza automáticamente un merge a `main`, publicación en Google Play ni despliegue a nube. La Constitución permanece `PENDIENTE_RATIFICACION`; se conservan requisitos/tareas de arquitectura base.
- Antes de editar, el worktree estaba limpio. Se respaldaron todas las refs en ../_Rutitas_CI_CD_003_Backups/2026-11-11_ci003/Rutitas_BackEnd.bundle (SHA-256 5BC0283026EFBF362874DCEFF773461B2DE31D9156FE919E540709C31B5F3A53) y se verificó el bundle. Las versiones originales de .github/workflows/ci.yml y memoria.md quedaron fuera del repo en ../_Rutitas_CI_CD_003_Backup_local/Rutitas_Backend/ con SHA-256 5FC8924FFEEDAC31818837DDD5B9A1E8822ABAB806CDE6AB3752086863243F0E y 8832E694EFECAC9CA8577E9208AC904EC0AA21C4BE7E5F6BA0626A7ADBCB8101.
- El workflow conserva restore/build y ahora encuentra todos los proyectos *Tests.csproj/*Test.csproj, ejecuta dotnet test para cada uno y falla si no encuentra ninguno cuando existe solución .NET. En main no se halló solución ni proyecto; este run no pudo ejercitar el bucle de pruebas reales.

### Publicación y verificación remota

- Rama: feature/ci-cd-003-backend-tests; commit de código verificado: ab68e349dccba431d0873f4f95d8b720d83ff158.
- PR #3: https://github.com/rutitasrutitas-eng/Rutitas_BackEnd/pull/3, abierto hacia main y sin fusionar.
- Actions PR run: https://github.com/rutitasrutitas-eng/Rutitas_BackEnd/actions/runs/43081375095. Validación YAML, SDD y Gitleaks: success; detección .NET: success; build/tests: skipped por ausencia de solución/proyecto .NET.
- No se afirma que tests de producto hayan pasado. Pendientes: incorporar código y pruebas reales, aprobar SDK/stack, habilitar Dependency Graph y revisar CI con evidencia por SHA. No se cambió main, Constitución, requisitos ni estados de arquitectura base; no hubo despliegue.

## Corrección de conflictos y advertencias del PR #4 (2026-11-13)

- Antes de modificar se comprobó el worktree limpio y se creó la referencia de respaldo `backup/fix-mark-api-devsecops-review-before-merge-2026-11-13` en `84e9564733a03323cefe1f683d5f795521fd5ac0`.
- Se integró la rama `feature/sdd-azure-devsecops` en su HEAD remoto observado `0f05c07d099c8a4bb396719d804ec0698a29254a` a `fix/mark-api-devsecops-review` mediante merge normal. Merge local resultante: `5c2824e`. Conflictos resueltos en workflow CI, `global.json` y los proyectos Architecture/Contract. No se reescribió historia ni se tocó `main` o la rama de Mark.
- Commit de correcciones de código: `c08f998` (`fix: validate backend inputs and dispose test content`). La actualización documental se confirmará por separado antes del push normal.
- `global.json` fija `10.0.400` con `rollForward: disable`, única versión .NET 10 verificada instalada localmente. Los dos proyectos de pruebas conservan `coverlet.collector` una sola vez, con assets privados.
- La revisión del merge detectó que el commit entrante removía checks de estado activo/revocación, rechazo de role en registro y pruebas de seguridad existentes. Se conservaron las protecciones del PR correctivo y sus regresiones automatizadas; esto requiere revisión de Mark.
- Restore Release completado. Build con análisis completo: 0 errores y 186 advertencias de calidad/API. Se añadieron guardas CA1062 en controladores/repositorios y liberación CA2000 de tres `HttpContent`; el inventario por regla, archivo y prioridad está en `docs/ADVERTENCIAS_ANALISIS_CSHARP.md`. Quedan advertencias CA1031 y otras por revisar; CA1308 no se cambió sin prueba de regresión.
- Suites locales: Architecture 2/2, Contract 3/3, Security 23/23 y Unit 29/29; Integration 10/13. Las otras 3 fallaron al iniciar Testcontainers porque Docker no está disponible en esta máquina. Total observado: 67 aprobadas y 3 bloqueadas por infraestructura de 70.
- Gitleaks local 8.30.0 revisó 18 commits y no detectó secretos; NuGet Audit transitive encontró 0 advisories. CodeQL/Trivy no están instalados y Docker no está disponible.
- GitHub Actions [run 43537627859](https://github.com/rutitasrutitas-eng/Rutitas_BackEnd/actions/runs/43537627859) para `f03bdf2c1ebd50f8d830c9eeaf788e084d87837d` terminó con tres jobs success; Gitleaks y YAML/SDD aprobaron. Las cinco suites: Architecture 2, Contract 3, Integration 13, Security 23 y Unit 29 = 70/70; 0 fallidas/omitidas. Auditoría NuGet: 0 advisories. TRX/Cobertura y JSON de auditoría se conservaron como artefactos.
- El run [43538064486](https://github.com/rutitasrutitas-eng/Rutitas_BackEnd/actions/runs/43538064486) del commit documental `e6ad72b` también terminó con sus tres jobs success y cinco suites ejecutadas. GitHub reportó el PR abierto, no fusionado y mergeable contra el SHA vigente de Mark `0f05c07`.
- Run final comprobado antes de esta anotación: [43538524666](https://github.com/rutitasrutitas-eng/Rutitas_BackEnd/actions/runs/43538524666), SHA `9d60ab2c21fc52ea43addc64273b2df21594f3b0`, tres jobs success y 70/70: Architecture 2, Contract 3, Integration 13, Security 23 y Unit 29; 0 fallidas/omitidas. Build normal: 0 warnings/errores; análisis: 186 warnings/0 errores; Gitleaks success; NuGet 0 advisories.
- Se intentó actualizar el cuerpo del PR #4 y publicar un comentario con la evidencia; el conector rechazó ambas escrituras con HTTP 403 `Resource not accessible by integration`. El acceso de lectura funciona, pero el PR no se pudo editar desde esta sesión. No se intentó eludir la restricción; véase `docs/VALIDACION_MARK.md`.
- No se cambió arquitectura base, Constitución ni tareas funcionales; no se usaron credenciales Atlas, no se desplegó a Azure y el PR #4 permanece sin fusionar.
