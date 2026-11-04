# Inconsistencias pendientes - Rutitas Backend

**Estado:** `PENDIENTE_RATIFICACION` / decisión=`propuesto`. Este reporte no autoriza implementación ni modifica el estado de las tareas.

| ID | Pendiente | Impacto | Acción y autoridad requerida |
|---|---|---|---|
| BE-PEND-01 | Las instrucciones docentes no tienen versión, fecha verificable ni identificador de fuente primaria en el registro; la fecha 29-10-2026 permanece sin verificar. | No permite demostrar completamente la procedencia del alcance. | Mantener la fecha como pendiente, registrar la fuente primaria y validarla con el docente. |
| BE-PEND-02 | Constitución, AGENTS y responsables siguen sin ratificación ni designación humana. | Bloquea aprobación normativa y ejecución futura. | Resolver BACKEND-D01 y BACKEND-D03. |
| BE-PEND-03 | Repositorio canónico, restricciones, stack y controles técnicos permanecen abiertos. | Bloquea scaffold, pruebas y decisiones tecnológicas. | Resolver BACKEND-D02, BACKEND-D04, BACKEND-D05 y BACKEND-D07. |
| BE-PEND-04 | El acuerdo de integración con Móvil es propuesto, no vinculante. | No se pueden definir contratos específicos. | Resolver BACKEND-D06 mediante acuerdo explícito. |
| BE-PEND-05 | La configuración OpenCode está restringida y no fue validada mediante una ejecución de OpenCode. | La compatibilidad declarada debe comprobarse antes de uso operativo. | Revisar configuración cargada en un entorno autorizado, sin habilitar edición ni shell. |

No hay tareas ejecutadas, decisiones aprobadas ni afirmaciones de controles técnicos ejecutados.

## Corrección de auditoría (2026-11-03)

Correcciones documentales aplicadas; no se aprobaron decisiones ni se ejecutaron tareas. Estados: corregidos AUD-01..AUD-11, AUD-14..AUD-16 y AUD-20; documentados/mitigados AUD-12, AUD-13 y AUD-17..AUD-19.

- AUD-01/AUD-20: `websearch` denegado en `opencode.json` y en los cinco agentes.
- AUD-02: `BE-AB-01` y `BE-AB-02` registradas como habilitadoras en `plan.md`.
- AUD-03: `BE-AB-04` incluye validación de entrada.
- AUD-04: actores genéricos y supuestos en `spec.md` §3.1–3.2.
- AUD-06: `BE-AB-07` depende de `BACKEND-D06` y del acuerdo de integración.
- AUD-07/AUD-08: README raíz corregido (este paquete).
- AUD-09: índices de subagentes y skills sin la etiqueta “histórico”.
- AUD-12: ruta histórica identificada como no incluida en `memoria.md`.
- AUD-13: commit anterior frente a cambios actuales documentado en `memoria.md`.
- AUD-14: prioridad frente a secuencia aclarada en `task.md`.
- AUD-15: `AC-BE-01..05` y riesgos sin estimación inventada en `spec.md` y `plan.md`.
- AUD-16: este reporte actualizado.
- AUD-17: política `task: deny` documentada en `AGENTS.md`; cambio de política pendiente de aprobación.
- AUD-18: control de copias duplicadas añadido a `CHECKLIST_ENTREGA.md`.
- AUD-19: ZIP original marcado como no incluido/no verificable en `fuentes-consultadas.md` y nota de migración.
- AUD-05/AUD-10/AUD-11: corregidos en el paquete Movil (auditoría cruzada).
