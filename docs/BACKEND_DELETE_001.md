# BACKEND-DELETE-001 — Informe de avance local

**Fecha:** 2026-11-18. **Estado:** preparación SDD `en_revision`; implementación `no_iniciado`; verificación `no_ejecutada`.

## Objetivo
Eliminación programada con recuperación durante siete días exactos, bloqueo de acceso común y borrado final verificable de datos personales propios, sin afectar datos compartidos ajenos.

## Contrato propuesto para revisión de Ame
- `DELETE /api/v1/users/me`: solicita eliminación de usuario activo (202 candidato).
- `GET /api/v1/users/me/deletion`: consulta estado mediante autorización limitada cuando esté pendiente.
- `POST /api/v1/auth/deletion/recovery`: reautenticación independiente y token acotado de recuperación; **nueva operación candidata**, no aprobada.
- `POST /api/v1/users/me/deletion/cancel`: cancela antes del vencimiento (200/204 por decidir).

Contratos, códigos, permisos y cuerpos **sin aprobar**. Ver `specs/014-backend-delete-001/spec.md`.

## Artefactos
`spec.md`, `plan.md`, `tasks.md`, `tests.md`, `trazabilidad.md`, `memoria-delete-001.md` en `specs/014-backend-delete-001/`.

## Evidencia actual
- Revisión del repositorio: el contrato existente usa `/api/v1`, `User.Status` permite `active`, hay `UsersController` y `MongoUserRepository` sin eliminación diferida.
- Existe prueba xUnit de revocación previa y suite de 72/72 pruebas aprobadas en otra rama; **no son pruebas de BACKEND-DELETE-001**.
- En esta fase no hay implementación ni pruebas ejecutadas de borrado. No declarar funcionalidad completa.

## Riesgos abiertos
Aprobación del contrato con Ame; seguridad de token de recuperación; carreras entre cancelación y borrado; reintentos y reinicio; integridad de datos relacionados; inventario de colecciones; respaldo y retenciones legales; compatibilidad Android.

## Restricciones
Local y sintético; $0 MXN adicional, sin consumir créditos; sin credenciales, sin recursos facturables; sin commits, push, PR, merge o despliegues hasta autorización específica.
