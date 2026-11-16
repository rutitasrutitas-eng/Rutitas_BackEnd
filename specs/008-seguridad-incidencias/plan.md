# 008 — Plan de diseño | Seguridad e incidencias

**Estado:** PROPUESTO · planificación solamente, ejecución no autorizada.  
**Dependencias:** 002, 003, 004, 006 y 007.  
**Objetivo:** definir una API segura de reportes, bloqueos y moderación, trazable al inventario E42–E45, E51–E52 y N18.

## 1. Estrategia documental

1. Confirmar con Producto las categorías, las reglas de bloqueo, los responsables de moderación y el proceso de apelación.
2. Conciliar E42 y N18 para que un mismo incidente no genere contratos o colecciones duplicadas.
3. Acordar con Android los contratos y DTO, `id` tipo `string`, autenticación y errores usando prefijo `/api/v1`.
4. Diseñar `IncidentReport`, `UserBlock` y `ModerationAction`, con índices MongoDB, estados y trazabilidad.
5. Definir permisos de usuario y moderador basados en decisiones del servidor. Evitar confiar en `role` o identificadores enviados por el cliente.
6. Definir política de privacidad: evidencias, consentimientos relacionados, retención, acceso, eliminación y respuesta a urgencias.
7. Crear contrato OpenAPI 3.1 del módulo **solo después de aprobar la especificación**; conservar los siete endpoints existentes.
8. Planificar pruebas unitarias, integración MongoDB/Testcontainers, contrato, abuso e IDOR, auditoría y revisión de seguridad.
9. Solicitar gate formal antes de abrir tareas de implementación y pull requests funcionales.

## 2. Arquitectura candidata

- `Rutitas.Api`: controladores de reportes, bloqueos y administración; JWT, rate limits y validación HTTP.
- `Rutitas.Application`: casos de uso y reglas de autorización por recurso, transiciones de estado y controles de idempotencia.
- `Rutitas.Domain`: modelos de reporte, bloqueo y registro de moderación sin dependencias de MongoDB.
- `Rutitas.Infrastructure`: repositorios MongoDB, índices, control de concurrencia y auditoría; almacenamiento seguro de adjuntos **solo si se aprueba**.
- `Rutitas.*Tests`: pruebas de dominio, integración, contrato, seguridad y arquitectura.

**Nota:** son componentes de diseño; no se han creado ni validado en ejecución.

## 3. Orden y dependencias

| Fase | Entregable | Condición de salida |
|---|---|---|
| A | Matriz E42–E45/E51–E52/N18 y decisiones | Sin duplicidad de incidentes |
| B | Política de moderación y privacidad | Revisada por responsable de seguridad |
| C | Modelo y estados propuestos | Consistencia con 003/006/007 |
| D | Contrato REST/DTO/errores | Aceptación de Móvil y Backend |
| E | Diseño de pruebas y amenazas | Casos de IDOR, abuso, evidencia y auditoría definidos |
| F | Gate formal | Permiso explícito para implementar |

## 4. Riesgos y mitigaciones propuestas

- **Fuga de reportes/evidencia:** autorización por recurso, minimización y auditoría.
- **Falsos reportes/abuso:** rate limiting y revisión humana; evitar decisiones sancionatorias totalmente automáticas.
- **Bloqueo incompleto:** centralizar reglas en Application/Backend, no solo ocultar en la UI.
- **Doble incidente E42/N18:** acordar modelo y ruta canónicos con Producto.
- **Datos sensibles en logs:** mensajes redactados sin cuerpos privados, JWT ni archivos.
- **Moderador con privilegios excesivos:** permisos mínimos, bitácora y revisión de roles.

## 5. Fuera de alcance en esta fase

No implementar controladores, repositorios, migraciones, anexos OpenAPI operativos, cuentas administrativas, integración Azure, CI/CD ni desplegar recursos. La información económica de Azure se abordará en una especificación de infraestructura independiente.
