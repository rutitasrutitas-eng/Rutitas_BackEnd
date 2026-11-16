# SPEC — 004 Comunidades y verificación institucional

**Estado:** propuesto | **Ejecución:** no iniciada | **Aprobaciones:** pendientes | **Fuente de alcance:** `../002-catalogo-funcional/inventario-funcional.md` (E05, E06, E07 y E49).

## 1. Objetivo y límites
Definir cómo un usuario autenticado consulta comunidades universitarias, solicita vincularse a una de ellas y consulta sus membresías. La administración podrá registrar comunidades cuando exista autorización. **Ninguna de estas operaciones está acreditada como implementada en los siete endpoints existentes.**

Este módulo no registra automáticamente conductores, no aprueba documentos, no publica rutas y no concede permisos de administrador a través de la app. La verificación de correo de 003 acredita acceso al correo, pero no prueba por sí sola la pertenencia vigente a una institución.

## 2. Inventario de operaciones propuestas
Las rutas originales del Excel usan `/v1/...`. Aquí se propone el prefijo `/api/v1` por consistencia con los endpoints existentes; **las rutas, parámetros y JSON aún requieren aprobación conjunta Backend–Móvil**.

| ID | Operación propuesta | Autorización prevista | Resultado esperado |
|---|---|---|---|
| RF-COM-01 / E05 | `GET /api/v1/communities` | Según política de visibilidad por definir | Catálogo paginado de comunidades visibles, sin exponer datos privados de miembros. |
| RF-COM-02 / E06 | `POST /api/v1/users/me/memberships` | JWT de usuario | Registrar solicitud de vinculación, no otorgar membresía verificada sin prueba y aprobación. |
| RF-COM-03 / E07 | `GET /api/v1/users/me/memberships` | JWT de usuario | Devolver únicamente membresías propias y sus estados verificables. |
| RF-COM-04 / E49 | `POST /api/v1/admin/communities` | JWT + rol/política administrativa verificada en servidor | Dar de alta comunidad con controles de unicidad y auditoría. |

**Nota de rutas:** E06/E07 en el Excel son `/v1/me/memberships`; la nueva propuesta `users/me` sigue la convención de `001-unit2-api`. No crear ambas variantes sin decisión explícita.

## 3. Requisitos funcionales
- **RF-COM-01:** permitir listar comunidades habilitadas, con paginación, búsqueda limitada y datos públicos mínimos; no enumerar miembros ni correos.
- **RF-COM-02:** permitir a un usuario solicitar asociación con una comunidad elegible; impedir duplicados y que una persona cree membresías de otros usuarios.
- **RF-COM-03:** distinguir estados de solicitud (`pending`, `verified`, `rejected`, `revoked`, sujetos a aprobación) y comprobar en servidor la transición de cada estado.
- **RF-COM-04:** permitir al usuario consultar sus propias solicitudes y membresías, incluyendo la comunidad, estado y fechas relevantes sin filtrar información sensible.
- **RF-COM-05:** permitir la creación administrativa de comunidades sin aceptar un campo de rol enviado por Android como autorización.
- **RF-COM-06:** revocar o suspender la pertenencia cuando una institución o el responsable competente retire la autorización, mediante un proceso administrativo por especificar; no inventar un endpoint sin aprobarlo.
- **RF-COM-07:** mantener la membresía independiente de la habilitación para conducir: la aprobación del conductor pertenece a su módulo específico.

## 4. Decisiones de negocio pendientes
1. **Alcance institucional:** ¿solo UTTT en piloto o varias universidades desde el inicio? ¿Quién autoriza una comunidad y qué dominios institucionales son válidos?
2. **Prueba de pertenencia:** correo institucional verificado, credencial con revisión manual u otro mecanismo; decidir caducidad y renovación. No subir fotografías de credenciales sin política de privacidad y almacenamiento.
3. **Múltiples membresías:** ¿puede un alumno estar en más de una comunidad? Definir conflicto con rutas compartidas entre universidades.
4. **Estados y aprobadores:** quién revisa, rechazo y apelaciones, revocación y auditoría.
5. **Accesibilidad de datos:** catálogo público o autenticado; qué atributos puede leer Móvil y qué datos nunca aparecen en las respuestas.
6. **Desactivación de cuenta:** reconciliar con E04 de 003 y retención acordada.

## 5. Seguridad y privacidad
- Autorización y comprobación de titularidad desde el backend; denegar por defecto escrituras administrativas.
- No confiar en dominio del correo proporcionado por el cliente: validar titularidad y criterios institucionales desde el servidor.
- Evitar duplicados por usuario y comunidad con validación e índice apropiado cuando se implemente; las operaciones repetidas deben tener un comportamiento definido.
- Limitar solicitudes masivas y enumeración de comunidades según política; proteger datos de membresía y evidencia institucional.
- Registrar eventos administrativos relevantes sin guardar tokens, contraseñas o documentos completos en logs.
- Establecer periodo de conservación y acceso restringido a evidencias de identidad antes de definir almacenamiento.

## 6. Criterios de aceptación documental
| ID | Condición para autorizar implementación futura |
|---|---|
| AC-COM-01 | Contratos E05–E07 y E49 con rutas canónicas, DTO, paginación, respuestas y errores acordados con Móvil. |
| AC-COM-02 | La política UTTT/interuniversidades y el mecanismo de prueba institucional están documentados y aprobados. |
| AC-COM-03 | Estados y transiciones de membresía especificados, con decisiones sobre duplicados, reintentos y revocación. |
| AC-COM-04 | Todas las escrituras tienen autorización por servidor y pruebas negativas de acceso cruzado y escalamiento de privilegios. |
| AC-COM-05 | Existe trazabilidad RF → contrato OpenAPI → tarea → prueba, con responsable y evidencia de conformidad. |
| AC-COM-06 | Se distinguen explícitamente verificación de cuenta, pertenencia institucional y aprobación del conductor. |

## 7. Fuera de alcance
E01/E04/N02 (identidad en 003), solicitud y aprobación de conductor E08–E10/E50, registro de vehículos, rutas, reservas, escaneo de credenciales en Android y despliegue en Azure. **No modificar** el contrato de los siete endpoints de `001-unit2-api` ni implementar este spec hasta contar con autorización.
