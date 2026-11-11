# Memoria de trabajo — Rutitas Backend

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
