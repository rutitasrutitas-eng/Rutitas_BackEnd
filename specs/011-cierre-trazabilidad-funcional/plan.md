# 011 — Plan de revisión y cierre de catálogo

**Estado:** propuesto, pendiente de aprobación. No ejecutar funcionalidades.

## Orden de trabajo

1. Validar `matriz-trazabilidad.md` con el Excel y la última versión de `002-catalogo-funcional`.
2. Completar en 005 las referencias de E08–E14, E50, N15 y N16: su spec actual usa requisitos COND pero no trazabilidad literal E/N; resolver también discrepancia de rutas (`/driver-applications` frente a `/drivers/applications`).
3. Completar o justificar E15, E22, E30, N05, N06, N12 y N13 en los módulos 006/007. Ver matriz para el resto de los IDs no encontrados.
4. Unificar el concepto `Route` de catálogo, `RideOffer` publicado y `Trip` ejecutado antes de definir creación, reservas y cupos; conservar GET de rutas existentes.
5. Resolver duplicaciones posibles: N09 vs E17, N17 vs E21, N18 vs E42; decidir si N16 necesita endpoint propio.
6. Definir N19–N20: si solo reflejan aportaciones informativas o requieren capacidades financieras nuevas. No implementar pagos sin alcance aprobado.
7. Revisar DTO, códigos HTTP, límites, retención de datos, roles, autorización servidor, pruebas de concurrencia y seguridad, con aprobación de Móvil.
8. Actualizar tareas, matriz y OpenAPI **propuesto** de módulos futuros en artefacto independiente; conservar OpenAPI implementado de 7 endpoints como referencia vigente.
9. Emitir acta de revisión y registrar decisiones. Solo tras aprobación, iniciar specs de IaC Azure y CI/CD DevSecOps en carpetas separadas.

## Entregables

- Matriz completa de 72 fichas con responsable y estado.
- Registro de decisiones y brechas resueltas.
- Contratos propuestos revisados por Backend/Móvil/Seguridad.
- Evidencia de aprobación formal o lista de bloqueos.

## No alcance

No modificar C#, JWT, MongoDB, GitHub Actions, Azure ni la app Android durante esta fase documental.
