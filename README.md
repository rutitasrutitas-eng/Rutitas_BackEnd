# Rutitas — Backend

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
