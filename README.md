# Rutitas — Backend

## Estado técnico observado (13/11/2026)

La rama `feature/sdd-azure-devsecops` contiene una API ASP.NET Core 10 con MongoDB y siete endpoints bajo `/api/v1`: registro, login, logout, consulta y edición del perfil propio, listado y detalle de rutas. La rama correctiva `fix/mark-api-devsecops-review` integra mediante merge normal el HEAD vigente de Mark (`0f05c07`) con la revisión previa (`84e9564`). El commit `c08f998` añade guardas de entrada y libera recursos en pruebas. Para SHA `9d60ab2`, el run [43538524666](https://github.com/rutitasrutitas-eng/Rutitas_BackEnd/actions/runs/43538524666) pasó los tres jobs y las cinco suites (70/70); consulte el [informe de validación](docs/VALIDACION_MARK.md), el [inventario del análisis C#](docs/ADVERTENCIAS_ANALISIS_CSHARP.md), el [contrato OpenAPI](specs/001-unit2-api/contracts/openapi.yaml) y la [matriz Android](docs/COMPATIBILIDAD_ANDROID.md).

Para reproducir localmente: instalar el SDK definido en [`global.json`](global.json) y Docker; ejecutar `dotnet restore Rutitas.Api/Rutitas.Api.slnx`, `dotnet build Rutitas.Api/Rutitas.Api.slnx -c Release --no-restore` y `dotnet test Rutitas.Api/Rutitas.Api.slnx -c Release --no-restore --collect "XPlat Code Coverage"`. Las pruebas de repositorio usan un MongoDB temporal de Testcontainers. En la validación local del 13/11/2026 pasaron 67 de 70 casos; tres pruebas Mongo no pudieron conectarse porque Docker no estaba disponible. Los resultados de GitHub Actions deben leerse para el SHA exacto y se registran en el informe de validación. No se requiere Atlas para las pruebas.

**Gobierno:** la Constitución y las decisiones de stack/contrato móvil permanecen sin ratificación. Los apartados documentales históricos siguientes describen el estado previo a la entrega de Mark; no sustituyen el estado técnico anterior.

## Descripción

Rutitas es un proyecto universitario de software. Este repositorio contiene la preparación documental de su proyecto Backend y define una arquitectura base para orientar una posible implementación futura.

## Propósito y alcance actual

El objetivo actual es describir responsabilidades y límites generales de un servicio Backend: entradas, validación, aplicación, dominio futuro, adaptadores, persistencia abstracta, errores, seguridad y observabilidad. No se especifican funcionalidades de producto, operaciones, endpoints, modelos de negocio ni despliegues.

La especificación registra **11 requisitos arquitectónicos**. El plan los relaciona con responsabilidades, tareas futuras y evidencia prevista. `task.md` contiene **7 tareas** propuestas; todas siguen pendientes, con ejecución `no_iniciado` y verificación `no_ejecutada`.

## Metodología SDD

El trabajo sigue Spec-Driven Development: fuente y necesidad autorizadas → especificación y criterios → revisión → plan → tareas → autorización humana → ejecución y verificación futuras. Esta entrega termina antes de autorizar implementación.

## Arquitectura base

El plan describe límites conceptuales y dependencias, manteniendo una organización proporcional. No selecciona lenguaje, framework, base de datos, proveedor ni topología de despliegue. **No existe todavía una implementación Backend aprobada** ni un stack definitivo seleccionado.

## Estructura

```text
Rutitas_Backend/
├── .opencode/
│   ├── agents/                 # cinco perfiles documentales
│   └── skills/                 # cinco guías documentales
├── .specify/memory/constitution.md
├── docs/                       # decisiones, trazabilidad, integración y contexto
├── skills/README.md
├── specs/000-arquitectura-base/
│   ├── spec.md
│   ├── plan.md
│   └── task.md
├── subagentes/README.md
├── AGENTS.md
├── constitution.md             # espejo de la canónica
├── memoria.md
├── opencode.json
└── README.md
```

## Gobierno y documentación

- Constitución canónica propuesta: [`.specify/memory/constitution.md`](.specify/memory/constitution.md). `constitution.md` es su espejo exacto. **Estado: PENDIENTE_RATIFICACION**.
- Procedimiento local: [`AGENTS.md`](AGENTS.md). Registro factual: [`memoria.md`](memoria.md).
- Spec, plan y tareas: [`specs/000-arquitectura-base/`](specs/000-arquitectura-base/spec.md), [`plan.md`](specs/000-arquitectura-base/plan.md), [`task.md`](specs/000-arquitectura-base/task.md).
- Los perfiles están en [`.opencode/agents/`](.opencode/agents/) y las skills en [`.opencode/skills/`](.opencode/skills/). Son definiciones para **OpenCode**; su instalación, ejecución o compatibilidad con Codex no está acreditada.

## Estado académico y restricciones

Preparación documental para revisión docente. No ejecutar las siete tareas, implementar software, definir endpoints funcionales, seleccionar stack, desplegar ni ratificar documentos. Las decisiones de autoridad, alcance, stack, controles técnicos e integración siguen pendientes de aprobación humana.

## Relación futura con la aplicación móvil

La comunicación con Móvil se mantiene como propuesta en [`docs/integracion-entre-proyectos.md`](docs/integracion-entre-proyectos.md), no como contrato aprobado. Una futura integración requiere acuerdo independiente entre ambos proyectos; la aplicación cliente no reemplaza la autorización del servidor.

## Repositorio

Repositorio oficial asignado: [Rutitas_BackEnd](https://github.com/rutitasrutitas-eng/Rutitas_BackEnd). La rama `main` está publicada en GitHub; el HEAD remoto se verificó después del push.

## Propuestas tecnológicas y funcionales para discusión futura (2026-11-04)

El equipo comunicó que el profesor **requiere una base NoSQL** y permite elegir alojamiento entre **Azure, AWS o Google Cloud** (fuentes primarias pendientes de anexar). Se propone evaluar **ASP.NET Core 10/C# + Google Cloud Run + Firestore**, con Google Maps Platform y posible Firebase Authentication. Son **opciones candidatas, no un stack aprobado**.

- [Propuesta de stack Backend y cloud](docs/propuestas-futuras/stack-backend-y-cloud.md).
- [Inventario exploratorio de 7 endpoints (dos alternativas según autenticación)](docs/propuestas-futuras/inventario-7-endpoints.md).
- [Decisiones aún abiertas](docs/decisiones-pendientes.md).

**Separación de fases:** estos anexos no forman parte de `specs/000-arquitectura-base/`, no son especificaciones funcionales aprobadas, no alteran los 11 requisitos ni autorizan implementar, desplegar o ejecutar tareas. La entrega académica SDD sigue restringida a arquitectura base.
